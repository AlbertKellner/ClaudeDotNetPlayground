using ClaudeDotNetPlayground.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();

builder.Services.AddScoped<ClaudeDotNetPlayground.Features.Query.TestGet.TestGetUseCase>();

var app = builder.Build();

app.UseExceptionHandler();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
