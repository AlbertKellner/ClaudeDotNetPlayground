using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Albert.Playground.ECS.AOT.Api.Features.Command.UserLogin;
using Albert.Playground.ECS.AOT.Api.Features.Query.WeatherConditionsGet;
using Albert.Playground.ECS.AOT.Api.Infra.ExceptionHandling;
using Albert.Playground.ECS.AOT.Api.Infra.Json;
using Albert.Playground.ECS.AOT.Api.Infra.ModelBinding;
using Albert.Playground.ECS.AOT.Api.Infra.ModelValidation;
using Albert.Playground.ECS.AOT.Api.Infra.Middlewares;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Albert.Playground.ECS.AOT.Api.Infra.HealthChecks;
using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.OpenMeteo;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Refit;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

const string OutputTemplate =
    "[{Timestamp:dd/MM/yyyy HH:mm:ss.fffffff}] [{CorrelationId}] [{UserName}] {Message:lj}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: OutputTemplate)
    .CreateBootstrapLogger();

Log.Information("[Program] Iniciar aplicação Albert.Playground.ECS.AOT.Api");

var builder = WebApplication.CreateBuilder(args);

Log.Information("[Program] Configurar Serilog com console colorido e enrichment por request");

builder.Host.UseSerilog((ctx, services, config) =>
    config
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: OutputTemplate));

Log.Information("[Program] Registrar dependências de infraestrutura");

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers(options =>
    {
        var simple = options.ModelBinderProviders.OfType<SimpleTypeModelBinderProvider>().FirstOrDefault();
        if (simple is not null)
        {
            var index = options.ModelBinderProviders.IndexOf(simple);
            options.ModelBinderProviders[index] = new FallbackSimpleTypeModelBinderProvider();
        }

        var nullProvider = new NullModelBinderProvider();
        var tryParseType = typeof(Microsoft.AspNetCore.Mvc.ModelBinding.Binders.TryParseModelBinderProvider);
        var tryParseIndex = options.ModelBinderProviders
            .Select((p, i) => (p, i))
            .FirstOrDefault(x => x.p.GetType() == tryParseType).i;
        if (tryParseIndex > 0)
            options.ModelBinderProviders[tryParseIndex] = nullProvider;

        var floatType = typeof(Microsoft.AspNetCore.Mvc.ModelBinding.Binders.FloatingPointTypeModelBinderProvider);
        var floatIndex = options.ModelBinderProviders
            .Select((p, i) => (p, i))
            .FirstOrDefault(x => x.p.GetType() == floatType).i;
        if (floatIndex > 0)
            options.ModelBinderProviders[floatIndex] = nullProvider;
    })
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonContext.Default));
builder.Services.AddSingleton<IObjectModelValidator, NoOpObjectModelValidator>();

builder.Services.AddHttpClient("datadog-agent", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["Datadog:AgentUrl"] ?? "http://datadog-agent:8126");
    c.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddHealthChecks()
    .AddCheck<DatadogAgentHealthCheck>("datadog-agent");

Log.Information("[Program] Registrar integrações com APIs externas");

builder.Services
    .AddRefitClient<IOpenMeteoApi>(new RefitSettings
    {
        ContentSerializer = new SystemTextJsonContentSerializer(
            new JsonSerializerOptions
            {
                TypeInfoResolver = OpenMeteoJsonContext.Default
            })
    })
    .ConfigureHttpClient(c =>
        c.BaseAddress = new Uri(builder.Configuration["ExternalApi:OpenMeteo:BaseUrl"]!))
    .AddResilienceHandler("open-meteo", resilienceBuilder =>
    {
        var maxRetryAttempts = builder.Configuration.GetValue<int>(
            "ExternalApi:OpenMeteo:WeatherConditionsGet:MaxRetryAttempts", 3);
        var delaySeconds = builder.Configuration.GetValue<double>(
            "ExternalApi:OpenMeteo:WeatherConditionsGet:DelaySeconds", 3);
        var backoffType = builder.Configuration.GetValue<DelayBackoffType>(
            "ExternalApi:OpenMeteo:WeatherConditionsGet:BackoffType", DelayBackoffType.Exponential);

        resilienceBuilder.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = maxRetryAttempts,
            Delay = TimeSpan.FromSeconds(delaySeconds),
            BackoffType = backoffType,
            UseJitter = false
        });

        resilienceBuilder.AddTimeout(TimeSpan.FromSeconds(delaySeconds));
    });

builder.Services.AddScoped<IOpenMeteoApiClient, OpenMeteoApiClient>();

Log.Information("[Program] Registrar dependências das features");

builder.Services.AddScoped<Albert.Playground.ECS.AOT.Api.Features.Query.TestGet.TestGetUseCase>();
builder.Services.AddScoped<UserLoginUseCase>();
builder.Services.AddScoped<WeatherConditionsGetUseCase>();

Log.Information("[Program] Registrar segurança e autenticação");

builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddTransient<AuthenticateFilter>();

var app = builder.Build();

// Workaround para .NET 10 + PublishAot: definir IsEnhancedModelMetadataSupported antes do primeiro request.
EnhancedModelMetadataActivator.Activate(app.Services.GetRequiredService<ILogger<Program>>());

Log.Information("[Program] Configurar pipeline de middlewares");

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();
app.MapControllers();
app.MapHealthChecks("/health");

Log.Information("[Program] Iniciar execução da aplicação");

// AOT: garante que os tipos de Controller sejam preservados pelo linker.
// PreserveControllers() deve ser chamado antes de app.Run() para que as
// DynamicDependency declarations tenham efeito na compilação AOT.
AotControllerPreservation.PreserveControllers();

app.Run();

// AOT: DynamicDependency deve ser aplicado em método, constructor ou field — não em assembly.
// Esta classe preserva os tipos de Controller para Native AOT sem usar [assembly: DynamicDependency].
// IMPORTANTE: PreserveControllers() deve ser chamado durante o startup para que as DynamicDependency
// tenham efeito — um método privado nunca chamado é trimado pelo AOT junto com seus atributos.
internal static class AotControllerPreservation
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Albert.Playground.ECS.AOT.Api.Features.Query.TestGet.TestGetEndpoint))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Albert.Playground.ECS.AOT.Api.Features.Command.UserLogin.UserLoginEndpoint))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Albert.Playground.ECS.AOT.Api.Features.Query.WeatherConditionsGet.WeatherConditionsGetEndpoint))]
    internal static void PreserveControllers() { }
}
