using DotNetPlayground.Api.Features.Command.TodoItemInsert.TodoItemInsertInterfaces;
using DotNetPlayground.Api.Features.Command.TodoItemInsert.TodoItemInsertModels;
using DotNetPlayground.Api.Shared.Storage;

namespace DotNetPlayground.Api.Features.Command.TodoItemInsert;

public sealed class TodoItemInsertRepository(InMemoryTodoStore store) : ITodoItemInsertRepository
{
    public TodoItemInsertEntity Insert(string title, string description)
    {
        var item = store.Add(title, description);
        return new TodoItemInsertEntity(item.Id, item.Title, item.Description, item.IsCompleted, item.CreatedAt);
    }
}
