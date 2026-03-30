# Exemplo Completo: Program.cs

## Descrição

Este documento apresenta o `Program.cs` real deste repositório como exemplo completo e anotado do padrão de logging estruturado com storytelling (SNP-001). O `Program.cs` é o ponto de entrada da aplicação e segue regras específicas de logging: um log por bloco lógico, escrito antes do conjunto de instruções do bloco.

---

## Program.cs Anotado

```csharp
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Starter.Template.AOT.Api.Features.Command.UserLogin;
using Starter.Template.AOT.Api.Features.Query.GitHubRepoSearch;
using Starter.Template.AOT.Api.Features.Query.PokemonGet;
using Starter.Template.AOT.Api.Features.Query.WeatherConditionsGet;
using Starter.Template.AOT.Api.Infra.ExceptionHandling;
using Starter.Template.AOT.Api.Infra.Json;
using Starter.Template.AOT.Api.Infra.ModelBinding;
using Starter.Template.AOT.Api.Infra.ModelValidation;
using Starter.Template.AOT.Api.Infra.Middlewares;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Starter.Template.AOT.Api.Infra.HealthChecks;
using Starter.Template.AOT.Api.Infra.Security;
using Starter.Template.AOT.Api.Shared.ExternalApi.GitHub;
using Starter.Template.AOT.Api.Shared.ExternalApi.OpenMeteo;
using Starter.Template.AOT.Api.Shared.ExternalApi.Pokemon;
using Starter.Template.AOT.Api.Shared.ExternalApi.Pokemon.Models;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Refit;
using Starter.Template.AOT.Api.Infra.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

// ─────────────────────────────────────────────────────────────────────
// 1. TEMPLATE DE CONSOLE (normativo — SNP-001)
//    Campos: Timestamp, CorrelationId, UserName, Message, Exception
//    CorrelationId e UserName são enriquecidos automaticamente por
//    CorrelationIdMiddleware e AuthenticateFilter respectivamente.
// ─────────────────────────────────────────────────────────────────────
const string OutputTemplate =
    "[{Timestamp:dd/MM/yyyy HH:mm:ss.fffffff}] [{CorrelationId}] [{UserName}] {Message:lj}{NewLine}{Exception}";

// ─────────────────────────────────────────────────────────────────────
// 2. BOOTSTRAP LOGGER
//    Logger temporário para capturar logs durante a inicialização,
//    antes do host estar construído. Usa o mesmo template e tema.
// ─────────────────────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: OutputTemplate)
    .CreateBootstrapLogger();

// ─────────────────────────────────────────────────────────────────────
// LOG DE INÍCIO — primeiro log da aplicação
// Segue o padrão [Program] (sem método, pois é top-level statements)
// ─────────────────────────────────────────────────────────────────────
Log.Information("[Program] Iniciar aplicação Starter.Template.AOT.Api");

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────────────
// 3. CONFIGURAÇÃO DO SERILOG
//    Log de bloco: um log antes de todo o bloco de configuração.
//    Não há log por instrução individual dentro do bloco.
// ─────────────────────────────────────────────────────────────────────
Log.Information("[Program] Configurar Serilog com console colorido e enrichment por request");

builder.Host.UseSerilog((ctx, services, config) =>
{
    config
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()          // Habilita enriquecimento via LogContext.PushProperty
        .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: OutputTemplate);

    // Datadog HTTP Sink (opcional — ativado por DD_API_KEY + Datadog:DirectLogs)
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

        Log.Information(
            "[Program] Datadog HTTP Sink ativado — logs enviados diretamente ao Datadog. Env={Env}, Host={Host}",
            ddEnv, ddHost);
    }
});

// ─────────────────────────────────────────────────────────────────────
// 4. DEPENDÊNCIAS DE INFRAESTRUTURA
//    Log de bloco antes de todas as instruções de DI de infraestrutura.
// ─────────────────────────────────────────────────────────────────────
Log.Information("[Program] Registrar dependências de infraestrutura");

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers(options =>
    {
        // Substituições AOT-compatíveis de model binding providers
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

// ─────────────────────────────────────────────────────────────────────
// 5. INTEGRAÇÕES COM APIs EXTERNAS
//    Log de bloco antes de todos os registros de APIs externas.
//    Cada API segue o padrão: Refit + Polly + Memory Cache (decorator).
// ─────────────────────────────────────────────────────────────────────
Log.Information("[Program] Registrar integrações com APIs externas");

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

// --- Open-Meteo API ---
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

// --- GitHub API ---
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

// --- PokéAPI ---
builder.Services
    .AddRefitClient<IPokemonApi>(new RefitSettings
    {
        ContentSerializer = new SystemTextJsonContentSerializer(
            new JsonSerializerOptions
            {
                TypeInfoResolver = PokemonJsonContext.Default
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

builder.Services.AddScoped<PokemonApiClient>();
builder.Services.AddScoped<IPokemonApiClient, CachedPokemonApiClient>();

// ─────────────────────────────────────────────────────────────────────
// 6. DEPENDÊNCIAS DAS FEATURES
//    Log de bloco antes do registro de Use Cases.
// ─────────────────────────────────────────────────────────────────────
Log.Information("[Program] Registrar dependências das features");

builder.Services.AddScoped<UserLoginUseCase>();
builder.Services.AddScoped<WeatherConditionsGetUseCase>();
builder.Services.AddScoped<GitHubRepoSearchUseCase>();
builder.Services.AddScoped<PokemonGetUseCase>();

// ─────────────────────────────────────────────────────────────────────
// 7. SEGURANÇA E AUTENTICAÇÃO
//    Log de bloco antes do registro de serviços de segurança.
// ─────────────────────────────────────────────────────────────────────
Log.Information("[Program] Registrar segurança e autenticação");

builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddTransient<AuthenticateFilter>();

var app = builder.Build();

// Workaround para .NET 10 + PublishAot
EnhancedModelMetadataActivator.Activate(app.Services.GetRequiredService<ILogger<Program>>());

// ─────────────────────────────────────────────────────────────────────
// 8. PIPELINE DE MIDDLEWARES
//    Log de bloco antes da configuração do pipeline HTTP.
//    Ordem crítica: CorrelationIdMiddleware ANTES de UseExceptionHandler.
// ─────────────────────────────────────────────────────────────────────
Log.Information("[Program] Configurar pipeline de middlewares");

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();
app.MapControllers();
app.MapHealthChecks("/health");

// ─────────────────────────────────────────────────────────────────────
// 9. INICIAR EXECUÇÃO
//    Último log antes de app.Run() — a aplicação está pronta.
// ─────────────────────────────────────────────────────────────────────
Log.Information("[Program] Iniciar execução da aplicação");

// AOT: preserva Controllers para o linker (deve ser chamado antes de app.Run)
AotControllerPreservation.PreserveControllers();

app.Run();
```

---

## Regras de Logging Aplicadas no Program.cs

| Regra | Como é aplicada |
|-------|-----------------|
| **Prefixo `[Program]`** | Todo log usa `[Program]` como prefixo — não há nome de método pois são top-level statements |
| **Um log por bloco lógico** | Cada seção de configuração (Serilog, infraestrutura, APIs externas, features, segurança, pipeline) tem exatamente um log antes das instruções |
| **Sem log por instrução individual** | Dentro de cada bloco, as instruções de DI não têm logs individuais |
| **Linguagem imperativa** | "Configurar", "Registrar", "Iniciar" — verbos no imperativo |
| **Template normativo (SNP-001)** | `[{Timestamp}] [{CorrelationId}] [{UserName}] {Message:lj}{NewLine}{Exception}` |
| **Console colorido** | `AnsiConsoleTheme.Code` com output template padronizado |
| **Enrichment via LogContext** | `Enrich.FromLogContext()` habilita CorrelationId e UserName injetados por middlewares |

---

## Sequência de Logs na Inicialização

Ao iniciar a aplicação, os logs produzidos pelo `Program.cs` são:

```
[23/03/2026 14:00:00.0000000] [] [] [Program] Iniciar aplicação Starter.Template.AOT.Api
[23/03/2026 14:00:00.1000000] [] [] [Program] Configurar Serilog com console colorido e enrichment por request
[23/03/2026 14:00:00.2000000] [] [] [Program] Registrar dependências de infraestrutura
[23/03/2026 14:00:00.3000000] [] [] [Program] Registrar integrações com APIs externas
[23/03/2026 14:00:00.4000000] [] [] [Program] Registrar dependências das features
[23/03/2026 14:00:00.5000000] [] [] [Program] Registrar segurança e autenticação
[23/03/2026 14:00:00.6000000] [] [] [Program] Configurar pipeline de middlewares
[23/03/2026 14:00:00.7000000] [] [] [Program] Iniciar execução da aplicação
```

Note que:
- `[CorrelationId]` e `[UserName]` estão vazios — não há requisição HTTP durante a inicialização
- Cada log corresponde a um bloco lógico distinto da configuração
- A sequência permite verificar que todos os blocos foram executados na ordem correta

---

## Diferenças entre Program.cs e Classes Normais

| Aspecto | Program.cs | Classes normais |
|---------|-----------|-----------------|
| **Prefixo** | `[Program]` | `[NomeDaClasse][NomeDoMétodo]` |
| **Granularidade** | Um log por bloco lógico | Logs de entrada, saída e iterações |
| **Logger** | `Log.Information()` (Serilog estático) | `logger.LogInformation()` (DI) |
| **Isolamento visual** | Não obrigatório (blocos de configuração) | Obrigatório (linha em branco acima e abaixo) |

---

## Referências

- [01-padroes-de-logging.md](01-padroes-de-logging.md) — regras completas do padrão storytelling
- [02-enriquecimento-de-contexto.md](02-enriquecimento-de-contexto.md) — CorrelationId e UserName
- [06-prompt-implementacao-logs.md](06-prompt-implementacao-logs.md) — prompt genérico para implementar em outros projetos
- `Instructions/snippets/canonical-snippets.md` — SNP-001
- `src/Starter.Template.AOT.Api/Program.cs` — arquivo-fonte real
