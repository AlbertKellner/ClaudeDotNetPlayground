var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<global::ClaudeDotNetPlayground.Features.Query.TestGet.TestGetUseCase.TestGetUseCase>();

var app = builder.Build();

app.MapControllers();

app.Run();
