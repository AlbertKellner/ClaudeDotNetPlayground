using DotNetPlayground.Api.Features.Command.TodoItemInsert.TodoItemInsertInterfaces;
using DotNetPlayground.Api.Features.Command.TodoItemInsert.TodoItemInsertModels;

namespace DotNetPlayground.Api.Features.Command.TodoItemInsert;

public sealed class TodoItemInsertUseCase(ITodoItemInsertRepository repository)
{
    public TodoItemInsertOutput Execute(TodoItemInsertInput input)
    {
        var entity = repository.Insert(input.Title, input.Description);

        return new TodoItemInsertOutput(entity.Id, entity.Title, entity.Description, entity.IsCompleted, entity.CreatedAt);
    }
}
