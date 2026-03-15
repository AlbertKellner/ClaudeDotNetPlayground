namespace DotNetPlayground.Api.Features.Query.TodoItemGetById.TodoItemGetByIdModels;

public sealed record TodoItemGetByIdEntity(Guid Id, string Title, string Description, bool IsCompleted, DateTimeOffset CreatedAt);
