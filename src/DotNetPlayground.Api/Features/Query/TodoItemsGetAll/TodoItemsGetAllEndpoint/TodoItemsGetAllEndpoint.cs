namespace DotNetPlayground.Api.Features.Query.TodoItemsGetAll;

public static class TodoItemsGetAllEndpoint
{
    public static void MapTodoItemsGetAll(this IEndpointRouteBuilder app)
    {
        app.MapGet("/todo-items", Handle)
            .WithName("GetAllTodoItems")
            .Produces<IReadOnlyList<TodoItemsGetAllOutput>>(StatusCodes.Status200OK);
    }

    private static IResult Handle(TodoItemsGetAllUseCase useCase, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(nameof(TodoItemsGetAllEndpoint));

        logger.LogInformation("Retrieving all todo items");

        var result = useCase.Execute();

        logger.LogInformation("Retrieved {Count} todo items", result.Count);

        return Results.Ok(result);
    }
}
