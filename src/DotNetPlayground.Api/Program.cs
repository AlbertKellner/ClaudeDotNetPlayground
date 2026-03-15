using DotNetPlayground.Api.Features.Command.TodoItemInsert;
using DotNetPlayground.Api.Features.Command.TodoItemInsert.TodoItemInsertInterfaces;
using DotNetPlayground.Api.Features.Query.TodoItemGetById;
using DotNetPlayground.Api.Features.Query.TodoItemGetById.TodoItemGetByIdInterfaces;
using DotNetPlayground.Api.Features.Query.TodoItemsGetAll;
using DotNetPlayground.Api.Features.Query.TodoItemsGetAll.TodoItemsGetAllInterfaces;
using DotNetPlayground.Api.Shared.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<InMemoryTodoStore>();

builder.Services.AddScoped<ITodoItemsGetAllRepository, TodoItemsGetAllRepository>();
builder.Services.AddScoped<TodoItemsGetAllUseCase>();

builder.Services.AddScoped<ITodoItemGetByIdRepository, TodoItemGetByIdRepository>();
builder.Services.AddScoped<TodoItemGetByIdUseCase>();

builder.Services.AddScoped<ITodoItemInsertRepository, TodoItemInsertRepository>();
builder.Services.AddScoped<TodoItemInsertUseCase>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapTodoItemsGetAll();
app.MapTodoItemGetById();
app.MapTodoItemInsert();

app.Run();
