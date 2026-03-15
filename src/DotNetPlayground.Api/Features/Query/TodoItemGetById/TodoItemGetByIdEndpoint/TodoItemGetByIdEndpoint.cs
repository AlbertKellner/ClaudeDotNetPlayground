using DotNetPlayground.Api.Shared.Errors;

namespace DotNetPlayground.Api.Features.Query.TodoItemGetById;

public static class TodoItemGetByIdEndpoint
{
    public static void MapTodoItemGetById(this IEndpointRouteBuilder app)
    {
        app.MapGet("/todo-items/{id:guid}", Handle)
            .WithName("GetTodoItemById")
            .Produces<TodoItemGetByIdOutput>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }

    private static IResult Handle(Guid id, TodoItemGetByIdUseCase useCase, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(nameof(TodoItemGetByIdEndpoint));

        logger.LogInformation("Retrieving todo item with id {Id}", id);

        var result = useCase.Execute(id);

        if (result is null)
        {
            logger.LogWarning("Todo item with id {Id} not found", id);
            return Results.NotFound(new ErrorResponse($"Todo item with id '{id}' was not found."));
        }

        logger.LogInformation("Retrieved todo item with id {Id}", id);

        return Results.Ok(result);
    }
}
