using Starter.Template.AOT.Api.Infra.Logging;
using Serilog;
using Serilog.Events;

namespace Starter.Template.AOT.UnitTest.Infra.Logging;

public sealed class DatadogHttpSinkTests
{
    [Fact]
    public void Emit_ShouldNotThrow_WhenLogEventIsValid()
    {
        var sink = new DatadogHttpSink(
            apiKey: "fake-api-key",
            service: "test-service",
            host: "test-host",
            env: "test");

        var logEvent = new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            exception: null,
            new MessageTemplate("Test message", []),
            []);

        var exception = Record.Exception(() => sink.Emit(logEvent));

        Assert.Null(exception);
    }

    [Fact]
    public void Emit_ShouldNotThrow_WhenMultipleEventsAreEmitted()
    {
        var sink = new DatadogHttpSink(
            apiKey: "fake-api-key",
            service: "test-service",
            host: "test-host",
            env: "test");

        for (var i = 0; i < 100; i++)
        {
            var logEvent = new LogEvent(
                DateTimeOffset.UtcNow,
                LogEventLevel.Information,
                exception: null,
                new MessageTemplate($"Message {i}", []),
                []);

            sink.Emit(logEvent);
        }

        Assert.True(true);
    }

    [Fact]
    public void Emit_ShouldAcceptAllLogLevels()
    {
        var sink = new DatadogHttpSink(
            apiKey: "fake-api-key",
            service: "test-service",
            host: "test-host",
            env: "test");

        var levels = new[]
        {
            LogEventLevel.Verbose,
            LogEventLevel.Debug,
            LogEventLevel.Information,
            LogEventLevel.Warning,
            LogEventLevel.Error,
            LogEventLevel.Fatal
        };

        foreach (var level in levels)
        {
            var logEvent = new LogEvent(
                DateTimeOffset.UtcNow,
                level,
                exception: null,
                new MessageTemplate($"Test {level}", []),
                []);

            var exception = Record.Exception(() => sink.Emit(logEvent));

            Assert.Null(exception);
        }
    }

    [Fact]
    public async Task DisposeAsync_ShouldNotThrow()
    {
        var sink = new DatadogHttpSink(
            apiKey: "fake-api-key",
            service: "test-service",
            host: "test-host",
            env: "test");

        var exception = await Record.ExceptionAsync(async () => await sink.DisposeAsync());

        Assert.Null(exception);
    }

    [Fact]
    public void SerilogIntegration_ShouldNotThrow_WhenSinkIsConfigured()
    {
        var sink = new DatadogHttpSink(
            apiKey: "fake-api-key",
            service: "test-service",
            host: "test-host",
            env: "test");

        var logger = new LoggerConfiguration()
            .WriteTo.Sink(sink)
            .CreateLogger();

        var exception = Record.Exception(() =>
            logger.Information("[DatadogHttpSinkTests][SerilogIntegration] Validar integração com Serilog"));

        Assert.Null(exception);
    }
}
