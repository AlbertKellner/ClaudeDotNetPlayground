using Albert.Playground.ECS.AOT.Api.Infra.Middlewares;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Infra.Middlewares;

public sealed class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_DeveRegistrarLogInformationNoInicio()
    {
        var fakeLogger = new FakeLogger<CorrelationIdMiddleware>();
        var nextCalled = false;
        var middleware = new CorrelationIdMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        }, fakeLogger);

        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(httpContext);

        Assert.True(nextCalled);
        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Processar"));
    }

    [Fact]
    public async Task InvokeAsync_DeveRegistrarLogInformationNoRetorno()
    {
        var fakeLogger = new FakeLogger<CorrelationIdMiddleware>();
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask, fakeLogger);
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(httpContext);

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar"));
    }

    [Fact]
    public async Task InvokeAsync_DeveAdicionarCorrelationIdNoResponseHeader()
    {
        var fakeLogger = new FakeLogger<CorrelationIdMiddleware>();
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask, fakeLogger);
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(httpContext);

        Assert.True(httpContext.Response.Headers.ContainsKey(CorrelationIdMiddleware.HeaderName));
    }

    [Fact]
    public async Task InvokeAsync_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeLogger = new FakeLogger<CorrelationIdMiddleware>();
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask, fakeLogger);
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(httpContext);

        var logs = fakeLogger.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("CorrelationIdMiddleware", l.Message));
    }
}
