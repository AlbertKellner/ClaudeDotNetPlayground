using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Infra.Security;

public sealed class AuthenticateFilterTests
{
    private sealed class FakeTokenService : ITokenService
    {
        private readonly AuthenticatedUser? _user;

        public FakeTokenService(AuthenticatedUser? user) => _user = user;

        public string GenerateToken(int userId, string userName) => "fake-token";
        public AuthenticatedUser? ValidateToken(string token) => _user;
    }

    private static ActionExecutingContext CreateContext(string? authHeader = null)
    {
        var httpContext = new DefaultHttpContext();

        if (authHeader is not null)
            httpContext.Request.Headers.Authorization = authHeader;

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        return new ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), new object());
    }

    [Fact]
    public async Task OnActionExecutionAsync_SemToken_DeveRegistrarLogWarning()
    {
        var fakeLogger = new FakeLogger<AuthenticateFilter>();
        var filter = new AuthenticateFilter(new FakeTokenService(null), fakeLogger);
        var context = CreateContext();
        var nextCalled = false;
        ActionExecutionDelegate next = () =>
        {
            nextCalled = true;
            return Task.FromResult<ActionExecutedContext>(null!);
        };

        await filter.OnActionExecutionAsync(context, next);

        Assert.False(nextCalled);
        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Warning &&
            l.Message.Contains("token ausente"));
    }

    [Fact]
    public async Task OnActionExecutionAsync_ComTokenInvalido_DeveRegistrarLogWarning()
    {
        var fakeLogger = new FakeLogger<AuthenticateFilter>();
        var filter = new AuthenticateFilter(new FakeTokenService(null), fakeLogger);
        var context = CreateContext("Bearer token-invalido");
        var nextCalled = false;
        ActionExecutionDelegate next = () =>
        {
            nextCalled = true;
            return Task.FromResult<ActionExecutedContext>(null!);
        };

        await filter.OnActionExecutionAsync(context, next);

        Assert.False(nextCalled);
        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Warning &&
            l.Message.Contains("token inválido"));
    }

    [Fact]
    public async Task OnActionExecutionAsync_ComTokenValido_DeveRegistrarLogInformationDeProsseguimento()
    {
        var fakeLogger = new FakeLogger<AuthenticateFilter>();
        var user = new AuthenticatedUser(1, "testuser");
        var filter = new AuthenticateFilter(new FakeTokenService(user), fakeLogger);
        var context = CreateContext("Bearer token-valido");
        ActionExecutionDelegate next = () => Task.FromResult<ActionExecutedContext>(null!);

        await filter.OnActionExecutionAsync(context, next);

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Prosseguir"));
    }

    [Fact]
    public async Task OnActionExecutionAsync_DeveRegistrarLogInformationNoInicio()
    {
        var fakeLogger = new FakeLogger<AuthenticateFilter>();
        var filter = new AuthenticateFilter(new FakeTokenService(null), fakeLogger);
        var context = CreateContext();
        ActionExecutionDelegate next = () => Task.FromResult<ActionExecutedContext>(null!);

        await filter.OnActionExecutionAsync(context, next);

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Validar"));
    }

    [Fact]
    public async Task OnActionExecutionAsync_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeLogger = new FakeLogger<AuthenticateFilter>();
        var filter = new AuthenticateFilter(new FakeTokenService(null), fakeLogger);
        var context = CreateContext();
        ActionExecutionDelegate next = () => Task.FromResult<ActionExecutedContext>(null!);

        await filter.OnActionExecutionAsync(context, next);

        var logs = fakeLogger.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("AuthenticateFilter", l.Message));
    }
}
