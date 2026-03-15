using DotNetPlayground.Api.Features.Command.TodoItemInsert.TodoItemInsertModels;

namespace DotNetPlayground.Api.Features.Command.TodoItemInsert.TodoItemInsertInterfaces;

public interface ITodoItemInsertRepository
{
    TodoItemInsertEntity Insert(string title, string description);
}
