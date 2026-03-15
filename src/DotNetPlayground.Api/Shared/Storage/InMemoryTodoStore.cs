namespace DotNetPlayground.Api.Shared.Storage;

public sealed class InMemoryTodoStore
{
    private readonly List<TodoStorageItem> _items = [];

    public InMemoryTodoStore()
    {
        _items.Add(new TodoStorageItem(Guid.NewGuid(), "Buy groceries", "Milk, bread, eggs", false, DateTimeOffset.UtcNow.AddDays(-2)));
        _items.Add(new TodoStorageItem(Guid.NewGuid(), "Read a book", "Finish 'Clean Architecture'", false, DateTimeOffset.UtcNow.AddDays(-1)));
        _items.Add(new TodoStorageItem(Guid.NewGuid(), "Exercise", "30 minutes of cardio", true, DateTimeOffset.UtcNow));
    }

    public IReadOnlyList<TodoStorageItem> GetAll() => _items.AsReadOnly();

    public TodoStorageItem? GetById(Guid id) => _items.FirstOrDefault(item => item.Id == id);

    public TodoStorageItem Add(string title, string description)
    {
        var newItem = new TodoStorageItem(Guid.NewGuid(), title, description, false, DateTimeOffset.UtcNow);
        _items.Add(newItem);
        return newItem;
    }
}

public sealed record TodoStorageItem(Guid Id, string Title, string Description, bool IsCompleted, DateTimeOffset CreatedAt);
