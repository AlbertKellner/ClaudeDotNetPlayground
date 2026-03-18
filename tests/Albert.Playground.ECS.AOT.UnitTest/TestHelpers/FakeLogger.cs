using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.TestHelpers;

public sealed record FakeLogRecord(LogLevel Level, string Message);

public sealed class FakeLogger<T> : ILogger<T>
{
    private readonly List<FakeLogRecord> _records = [];

    public IReadOnlyList<FakeLogRecord> GetSnapshot() => _records.AsReadOnly();

    public void Clear() => _records.Clear();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => NullDisposable.Instance;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        _records.Add(new FakeLogRecord(logLevel, message));
    }

    private sealed class NullDisposable : IDisposable
    {
        public static readonly NullDisposable Instance = new();
        public void Dispose() { }
    }
}
