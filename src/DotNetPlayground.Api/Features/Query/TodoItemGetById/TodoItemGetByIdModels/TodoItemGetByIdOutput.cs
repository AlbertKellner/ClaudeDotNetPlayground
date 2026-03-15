namespace DotNetPlayground.Api.Features.Query.TodoItemGetById;

public sealed record TodoItemGetByIdOutput(Guid Id, string Title, string Description, bool IsCompleted, DateTimeOffset CreatedAt);
