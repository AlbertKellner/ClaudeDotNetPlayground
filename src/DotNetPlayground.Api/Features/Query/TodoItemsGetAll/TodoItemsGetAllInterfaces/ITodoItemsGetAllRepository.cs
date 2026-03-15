using DotNetPlayground.Api.Features.Query.TodoItemsGetAll.TodoItemsGetAllModels;

namespace DotNetPlayground.Api.Features.Query.TodoItemsGetAll.TodoItemsGetAllInterfaces;

public interface ITodoItemsGetAllRepository
{
    IReadOnlyList<TodoItemsGetAllEntity> GetAll();
}
