using DotNetPlayground.Api.Features.Query.TodoItemsGetAll.TodoItemsGetAllInterfaces;
using DotNetPlayground.Api.Features.Query.TodoItemsGetAll.TodoItemsGetAllModels;

namespace DotNetPlayground.Api.Features.Query.TodoItemsGetAll;

public sealed class TodoItemsGetAllUseCase(ITodoItemsGetAllRepository repository)
{
    public IReadOnlyList<TodoItemsGetAllOutput> Execute()
    {
        var entities = repository.GetAll();

        return entities
            .Select(entity => new TodoItemsGetAllOutput(entity.Id, entity.Title, entity.Description, entity.IsCompleted, entity.CreatedAt))
            .ToList()
            .AsReadOnly();
    }
}
