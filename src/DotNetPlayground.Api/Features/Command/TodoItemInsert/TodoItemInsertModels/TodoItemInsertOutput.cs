namespace DotNetPlayground.Api.Features.Command.TodoItemInsert;

public sealed record TodoItemInsertOutput(Guid Id, string Title, string Description, bool IsCompleted, DateTimeOffset CreatedAt);
