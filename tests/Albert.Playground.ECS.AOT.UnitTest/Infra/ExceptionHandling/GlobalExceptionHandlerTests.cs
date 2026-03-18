using Albert.Playground.ECS.AOT.Api.Infra.ExceptionHandling;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Infra.ExceptionHandling;

public sealed class GlobalExceptionHandlerTests
{
    private sealed class FakeProblemDetailsService : IProblemDetailsService
    {
        public ValueTask<bool> TryWriteAsync(ProblemDetailsContext context) => ValueTask.FromResult(true);
        public ValueTask WriteAsync(ProblemDetailsContext context) => ValueTask.CompletedTask;
    }

    [Fact]
    public async Task TryHandleAsync_DeveRegistrarLogErrorComExcecao()
    {
        var fakeLogger = new FakeLogger<GlobalExceptionHandler>();
        var handler = new GlobalExceptionHandler(new FakeProblemDetailsService(), fakeLogger);
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        var exception = new InvalidOperationException("Erro de teste");

        await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Error &&
            l.Message.Contains("Capturar"));
    }

    [Fact]
    public async Task TryHandleAsync_DeveRegistrarLogErrorNoRetorno()
    {
        var fakeLogger = new FakeLogger<GlobalExceptionHandler>();
        var handler = new GlobalExceptionHandler(new FakeProblemDetailsService(), fakeLogger);
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        var exception = new InvalidOperationException("Erro de teste");

        await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Error &&
            l.Message.Contains("Retornar"));
    }

    [Fact]
    public async Task TryHandleAsync_DeveDefinirStatusCode500()
    {
        var fakeLogger = new FakeLogger<GlobalExceptionHandler>();
        var handler = new GlobalExceptionHandler(new FakeProblemDetailsService(), fakeLogger);
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        var exception = new InvalidOperationException("Erro de teste");

        await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.Equal(StatusCodes.Status500InternalServerError, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeLogger = new FakeLogger<GlobalExceptionHandler>();
        var handler = new GlobalExceptionHandler(new FakeProblemDetailsService(), fakeLogger);
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        var exception = new InvalidOperationException("Erro de teste");

        await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        var logs = fakeLogger.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("GlobalExceptionHandler", l.Message));
    }
}
