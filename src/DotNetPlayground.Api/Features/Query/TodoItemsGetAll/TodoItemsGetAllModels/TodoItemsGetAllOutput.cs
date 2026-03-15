namespace DotNetPlayground.Api.Features.Query.TodoItemsGetAll;

public sealed record TodoItemsGetAllOutput(Guid Id, string Title, string Description, bool IsCompleted, DateTimeOffset CreatedAt);
