using DotNetPlayground.Api.Features.Command.TodoItemInsert.TodoItemInsertModels;

namespace DotNetPlayground.Api.Features.Command.TodoItemInsert;

public static class TodoItemInsertEndpoint
{
    public static void MapTodoItemInsert(this IEndpointRouteBuilder app)
    {
        app.MapPost("/todo-items", Handle)
            .WithName("InsertTodoItem")
            .Produces<TodoItemInsertOutput>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
    }

    private static IResult Handle(TodoItemInsertInput input, TodoItemInsertUseCase useCase, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(nameof(TodoItemInsertEndpoint));

        logger.LogInformation("Inserting new todo item with title '{Title}'", input.Title);

        var result = useCase.Execute(input);

        logger.LogInformation("Todo item inserted with id {Id}", result.Id);

        return Results.Created($"/todo-items/{result.Id}", result);
    }
}
