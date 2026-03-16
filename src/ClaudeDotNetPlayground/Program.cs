using System.Text.Json;
using ClaudeDotNetPlayground.Features.Command.UserLogin;
using ClaudeDotNetPlayground.Features.Query.WeatherConditionsGet;
using ClaudeDotNetPlayground.Infra.ExceptionHandling;
using ClaudeDotNetPlayground.Infra.Middlewares;
using ClaudeDotNetPlayground.Infra.Security;
using ClaudeDotNetPlayground.Shared.ExternalApi.OpenMeteo;
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

Log.Information("[Program] Iniciar aplicação ClaudeDotNetPlayground");

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
builder.Services.AddControllers();
builder.Services.AddHealthChecks();

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
        c.BaseAddress = new Uri(builder.Configuration["OpenMeteo:BaseAddress"]!))
    .AddResilienceHandler("open-meteo", resilienceBuilder =>
    {
        resilienceBuilder.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(3),
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = false
        });

        resilienceBuilder.AddTimeout(TimeSpan.FromSeconds(3));
    });

builder.Services.AddScoped<IOpenMeteoApiClient, OpenMeteoApiClient>();

Log.Information("[Program] Registrar dependências das features");

builder.Services.AddScoped<ClaudeDotNetPlayground.Features.Query.TestGet.TestGetUseCase>();
builder.Services.AddScoped<UserLoginUseCase>();
builder.Services.AddScoped<WeatherConditionsGetUseCase>();

Log.Information("[Program] Registrar segurança e autenticação");

builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddTransient<AuthenticateFilter>();

var app = builder.Build();

Log.Information("[Program] Configurar pipeline de middlewares");

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();
app.MapControllers();
app.MapHealthChecks("/health");

Log.Information("[Program] Iniciar execução da aplicação");

app.Run();

public partial class Program { }
