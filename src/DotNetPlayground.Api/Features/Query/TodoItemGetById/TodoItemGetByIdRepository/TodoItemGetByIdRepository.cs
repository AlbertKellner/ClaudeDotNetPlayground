using DotNetPlayground.Api.Features.Query.TodoItemGetById.TodoItemGetByIdInterfaces;
using DotNetPlayground.Api.Features.Query.TodoItemGetById.TodoItemGetByIdModels;
using DotNetPlayground.Api.Shared.Storage;

namespace DotNetPlayground.Api.Features.Query.TodoItemGetById;

public sealed class TodoItemGetByIdRepository(InMemoryTodoStore store) : ITodoItemGetByIdRepository
{
    public TodoItemGetByIdEntity? GetById(Guid id)
    {
        var item = store.GetById(id);

        if (item is null)
            return null;

        return new TodoItemGetByIdEntity(item.Id, item.Title, item.Description, item.IsCompleted, item.CreatedAt);
    }
}
