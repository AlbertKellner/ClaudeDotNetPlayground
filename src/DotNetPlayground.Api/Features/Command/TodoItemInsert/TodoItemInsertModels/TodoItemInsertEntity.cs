namespace DotNetPlayground.Api.Features.Command.TodoItemInsert.TodoItemInsertModels;

public sealed record TodoItemInsertEntity(Guid Id, string Title, string Description, bool IsCompleted, DateTimeOffset CreatedAt);
