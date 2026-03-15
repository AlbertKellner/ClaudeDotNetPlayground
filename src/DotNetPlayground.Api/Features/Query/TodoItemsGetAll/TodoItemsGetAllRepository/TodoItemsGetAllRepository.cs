using DotNetPlayground.Api.Features.Query.TodoItemsGetAll.TodoItemsGetAllInterfaces;
using DotNetPlayground.Api.Features.Query.TodoItemsGetAll.TodoItemsGetAllModels;
using DotNetPlayground.Api.Shared.Storage;

namespace DotNetPlayground.Api.Features.Query.TodoItemsGetAll;

public sealed class TodoItemsGetAllRepository(InMemoryTodoStore store) : ITodoItemsGetAllRepository
{
    public IReadOnlyList<TodoItemsGetAllEntity> GetAll()
    {
        return store.GetAll()
            .Select(item => new TodoItemsGetAllEntity(item.Id, item.Title, item.Description, item.IsCompleted, item.CreatedAt))
            .ToList()
            .AsReadOnly();
    }
}
