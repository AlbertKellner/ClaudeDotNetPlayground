using Albert.Playground.ECS.AOT.Api.Features.Command.UserLogin;
using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Features.Command.UserLogin;

public sealed class UserLoginEndpointTests
{
    private sealed class FakeTokenService : ITokenService
    {
        public string GenerateToken(int userId, string userName) => $"fake-token-{userId}-{userName}";
        public AuthenticatedUser? ValidateToken(string token) => null;
    }

    [Fact]
    public void Post_ComCredenciaisValidas_DeveRegistrarLogInformationNoInicio()
    {
        var fakeLoggerUseCase = new FakeLogger<UserLoginUseCase>();
        var useCase = new UserLoginUseCase(new FakeTokenService(), fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<UserLoginEndpoint>();
        var endpoint = new UserLoginEndpoint(useCase, fakeLoggerEndpoint);
        var input = new UserLoginInput { UserName = "Albert", Password = "albert123" };

        endpoint.Post(input);

        var logs = fakeLoggerEndpoint.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Processar"));
    }

    [Fact]
    public void Post_ComCredenciaisValidas_DeveRegistrarLogInformationNoRetorno()
    {
        var fakeLoggerUseCase = new FakeLogger<UserLoginUseCase>();
        var useCase = new UserLoginUseCase(new FakeTokenService(), fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<UserLoginEndpoint>();
        var endpoint = new UserLoginEndpoint(useCase, fakeLoggerEndpoint);
        var input = new UserLoginInput { UserName = "Albert", Password = "albert123" };

        endpoint.Post(input);

        var logs = fakeLoggerEndpoint.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar"));
    }

    [Fact]
    public void Post_ComCredenciaisInvalidas_DeveRegistrarLogWarning()
    {
        var fakeLoggerUseCase = new FakeLogger<UserLoginUseCase>();
        var useCase = new UserLoginUseCase(new FakeTokenService(), fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<UserLoginEndpoint>();
        var endpoint = new UserLoginEndpoint(useCase, fakeLoggerEndpoint);
        var input = new UserLoginInput { UserName = "invalido", Password = "invalido" };

        endpoint.Post(input);

        var logs = fakeLoggerEndpoint.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Warning &&
            l.Message.Contains("credenciais inválidas"));
    }

    [Fact]
    public void Post_ComCredenciaisValidas_DeveRetornarOk()
    {
        var fakeLoggerUseCase = new FakeLogger<UserLoginUseCase>();
        var useCase = new UserLoginUseCase(new FakeTokenService(), fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<UserLoginEndpoint>();
        var endpoint = new UserLoginEndpoint(useCase, fakeLoggerEndpoint);
        var input = new UserLoginInput { UserName = "Albert", Password = "albert123" };

        var actionResult = endpoint.Post(input);

        Assert.IsType<OkObjectResult>(actionResult);
    }

    [Fact]
    public void Post_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeLoggerUseCase = new FakeLogger<UserLoginUseCase>();
        var useCase = new UserLoginUseCase(new FakeTokenService(), fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<UserLoginEndpoint>();
        var endpoint = new UserLoginEndpoint(useCase, fakeLoggerEndpoint);
        var input = new UserLoginInput { UserName = "Albert", Password = "albert123" };

        endpoint.Post(input);

        var logs = fakeLoggerEndpoint.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("UserLoginEndpoint", l.Message));
    }
}
