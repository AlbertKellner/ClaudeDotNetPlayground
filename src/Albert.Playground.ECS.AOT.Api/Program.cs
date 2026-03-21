using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Albert.Playground.ECS.AOT.Api.Features.Command.UserLogin;
using Albert.Playground.ECS.AOT.Api.Features.Query.GitHubRepoSearch;
using Albert.Playground.ECS.AOT.Api.Features.Query.PokemonGet;
using Albert.Playground.ECS.AOT.Api.Features.Query.WeatherConditionsGet;
using Albert.Playground.ECS.AOT.Api.Infra.ExceptionHandling;
using Albert.Playground.ECS.AOT.Api.Infra.Json;
using Albert.Playground.ECS.AOT.Api.Infra.ModelBinding;
using Albert.Playground.ECS.AOT.Api.Infra.ModelValidation;
using Albert.Playground.ECS.AOT.Api.Infra.Middlewares;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Albert.Playground.ECS.AOT.Api.Infra.HealthChecks;
using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.OpenMeteo;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Refit;
using Albert.Playground.ECS.AOT.Api.Infra.Logging;
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
{
    config
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: OutputTemplate);

    var ddApiKey = Environment.GetEnvironmentVariable("DD_API_KEY");
    var ddDirectLogs = ctx.Configuration.GetValue<bool>("Datadog:DirectLogs", false);

    if (!string.IsNullOrEmpty(ddApiKey) && ddDirectLogs)
    {
        var ddEnv = Environment.GetEnvironmentVariable("DD_ENV") ?? "local";
        var ddHost = Environment.GetEnvironmentVariable("DD_HOSTNAME") ?? Environment.MachineName;

        config.WriteTo.Sink(new DatadogHttpSink(
            apiKey: ddApiKey,
            service: "albert-playground-ecs-aot-api",
            host: ddHost,
            env: ddEnv));

        Log.Information("[Program] Datadog HTTP Sink ativado — logs enviados diretamente ao Datadog. Env={Env}, Host={Host}", ddEnv, ddHost);
    }
});

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

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

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
        c.BaseAddress = new Uri(builder.Configuration["ExternalApi:OpenMeteo:HttpRequest:BaseUrl"]!))
    .AddResilienceHandler("open-meteo", resilienceBuilder =>
    {
        var maxRetryAttempts = builder.Configuration.GetValue<int>(
            "ExternalApi:OpenMeteo:CircuitBreaker:MaxRetryAttempts", 3);
        var delaySeconds = builder.Configuration.GetValue<double>(
            "ExternalApi:OpenMeteo:CircuitBreaker:DelaySeconds", 3);
        var backoffType = builder.Configuration.GetValue<DelayBackoffType>(
            "ExternalApi:OpenMeteo:CircuitBreaker:BackoffType", DelayBackoffType.Exponential);

        resilienceBuilder.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = maxRetryAttempts,
            Delay = TimeSpan.FromSeconds(delaySeconds),
            BackoffType = backoffType,
            UseJitter = false
        });

        resilienceBuilder.AddTimeout(TimeSpan.FromSeconds(delaySeconds));
    });

builder.Services.AddScoped<OpenMeteoApiClient>();
builder.Services.AddScoped<IOpenMeteoApiClient, CachedOpenMeteoApiClient>();

builder.Services.AddTransient<GitHubAuthenticationHandler>();
builder.Services
    .AddRefitClient<IGitHubApi>(new RefitSettings
    {
        ContentSerializer = new SystemTextJsonContentSerializer(
            new JsonSerializerOptions
            {
                TypeInfoResolver = GitHubJsonContext.Default
            })
    })
    .ConfigureHttpClient(c =>
        c.BaseAddress = new Uri(builder.Configuration["ExternalApi:GitHub:HttpRequest:BaseUrl"]!))
    .AddHttpMessageHandler<GitHubAuthenticationHandler>()
    .AddResilienceHandler("github", resilienceBuilder =>
    {
        var maxRetryAttempts = builder.Configuration.GetValue<int>(
            "ExternalApi:GitHub:CircuitBreaker:MaxRetryAttempts", 3);
        var delaySeconds = builder.Configuration.GetValue<double>(
            "ExternalApi:GitHub:CircuitBreaker:DelaySeconds", 3);
        var backoffType = builder.Configuration.GetValue<DelayBackoffType>(
            "ExternalApi:GitHub:CircuitBreaker:BackoffType", DelayBackoffType.Exponential);

        resilienceBuilder.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = maxRetryAttempts,
            Delay = TimeSpan.FromSeconds(delaySeconds),
            BackoffType = backoffType,
            UseJitter = false
        });

        resilienceBuilder.AddTimeout(TimeSpan.FromSeconds(delaySeconds));
    });

builder.Services.AddScoped<GitHubApiClient>();
builder.Services.AddScoped<IGitHubApiClient, CachedGitHubApiClient>();

builder.Services
    .AddRefitClient<IPokeApi>(new RefitSettings
    {
        ContentSerializer = new SystemTextJsonContentSerializer(
            new JsonSerializerOptions
            {
                TypeInfoResolver = PokeApiJsonContext.Default
            })
    })
    .ConfigureHttpClient(c =>
        c.BaseAddress = new Uri(builder.Configuration["ExternalApi:Pokemon:HttpRequest:BaseUrl"]!))
    .AddResilienceHandler("pokemon", resilienceBuilder =>
    {
        var maxRetryAttempts = builder.Configuration.GetValue<int>(
            "ExternalApi:Pokemon:CircuitBreaker:MaxRetryAttempts", 3);
        var delaySeconds = builder.Configuration.GetValue<double>(
            "ExternalApi:Pokemon:CircuitBreaker:DelaySeconds", 3);
        var backoffType = builder.Configuration.GetValue<DelayBackoffType>(
            "ExternalApi:Pokemon:CircuitBreaker:BackoffType", DelayBackoffType.Exponential);

        resilienceBuilder.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = maxRetryAttempts,
            Delay = TimeSpan.FromSeconds(delaySeconds),
            BackoffType = backoffType,
            UseJitter = false
        });

        resilienceBuilder.AddTimeout(TimeSpan.FromSeconds(delaySeconds));
    });

builder.Services.AddScoped<PokeApiClient>();
builder.Services.AddScoped<IPokeApiClient, CachedPokeApiClient>();

Log.Information("[Program] Registrar dependências das features");

builder.Services.AddScoped<Albert.Playground.ECS.AOT.Api.Features.Query.TestGet.TestGetUseCase>();
builder.Services.AddScoped<UserLoginUseCase>();
builder.Services.AddScoped<WeatherConditionsGetUseCase>();
builder.Services.AddScoped<GitHubRepoSearchUseCase>();
builder.Services.AddScoped<PokemonGetUseCase>();

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
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Albert.Playground.ECS.AOT.Api.Features.Query.GitHubRepoSearch.GitHubRepoSearchEndpoint))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Albert.Playground.ECS.AOT.Api.Features.Query.PokemonGet.PokemonGetEndpoint))]
    internal static void PreserveControllers() { }
}
