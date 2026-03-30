using Starter.Template.AOT.Api.Infra.ExceptionHandling;
using Starter.Template.AOT.Api.Infra.Json;
using Starter.Template.AOT.Api.Infra.ModelBinding;
using Starter.Template.AOT.Api.Infra.ModelValidation;
using Starter.Template.AOT.Api.Infra.Middlewares;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Starter.Template.AOT.Api.Infra.HealthChecks;
using Starter.Template.AOT.Api.Infra.Security;
using Starter.Template.AOT.Api.Infra.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

const string OutputTemplate =
    "[{Timestamp:dd/MM/yyyy HH:mm:ss.fffffff}] [{CorrelationId}] [{UserName}] {Message:lj}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: OutputTemplate)
    .CreateBootstrapLogger();

Log.Information("[Program] Iniciar aplicação");

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
            service: "starter-template-aot-api",
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
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
    options.SuppressModelStateInvalidFilter = true);
builder.Services.AddSingleton<IObjectModelValidator, NoOpObjectModelValidator>();

builder.Services.AddHttpClient("datadog-agent", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["Datadog:AgentUrl"] ?? "http://datadog-agent:8126");
    c.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddHealthChecks()
    .AddCheck<DatadogAgentHealthCheck>("datadog-agent");

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

Log.Information("[Program] Registrar dependências das features");

// TODO: Registrar Use Cases das features aqui
// Exemplo: builder.Services.AddScoped<NomeDaFeatureUseCase>();

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
    // TODO: Adicionar DynamicDependency para cada Controller implementado
    // Requer: using System.Diagnostics.CodeAnalysis; no topo do arquivo
    // Exemplo:
    // [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(NomeDaFeatureEndpoint))]
    internal static void PreserveControllers() { }
}
