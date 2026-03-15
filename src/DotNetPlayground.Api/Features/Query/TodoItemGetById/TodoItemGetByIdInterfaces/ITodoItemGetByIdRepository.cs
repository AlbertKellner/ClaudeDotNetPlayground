using DotNetPlayground.Api.Features.Query.TodoItemGetById.TodoItemGetByIdModels;

namespace DotNetPlayground.Api.Features.Query.TodoItemGetById.TodoItemGetByIdInterfaces;

public interface ITodoItemGetByIdRepository
{
    TodoItemGetByIdEntity? GetById(Guid id);
}
