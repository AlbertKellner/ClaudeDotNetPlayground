namespace DotNetPlayground.Api.Features.Query.TodoItemsGetAll.TodoItemsGetAllModels;

public sealed record TodoItemsGetAllEntity(Guid Id, string Title, string Description, bool IsCompleted, DateTimeOffset CreatedAt);
