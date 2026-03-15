using DotNetPlayground.Api.Features.Query.TodoItemGetById.TodoItemGetByIdInterfaces;
using DotNetPlayground.Api.Features.Query.TodoItemGetById.TodoItemGetByIdModels;

namespace DotNetPlayground.Api.Features.Query.TodoItemGetById;

public sealed class TodoItemGetByIdUseCase(ITodoItemGetByIdRepository repository)
{
    public TodoItemGetByIdOutput? Execute(Guid id)
    {
        var entity = repository.GetById(id);

        if (entity is null)
            return null;

        return new TodoItemGetByIdOutput(entity.Id, entity.Title, entity.Description, entity.IsCompleted, entity.CreatedAt);
    }
}
