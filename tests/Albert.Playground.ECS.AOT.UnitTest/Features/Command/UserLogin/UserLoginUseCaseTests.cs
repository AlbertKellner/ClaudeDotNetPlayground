using Albert.Playground.ECS.AOT.Api.Features.Command.UserLogin;
using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Features.Command.UserLogin;

public sealed class UserLoginUseCaseTests
{
    private sealed class FakeTokenService : ITokenService
    {
        public string GenerateToken(int userId, string userName) => $"fake-token-{userId}-{userName}";
        public AuthenticatedUser? ValidateToken(string token) => null;
    }

    [Fact]
    public void Execute_ComCredenciaisValidas_DeveRegistrarLogInformationNoInicio()
    {
        var fakeLogger = new FakeLogger<UserLoginUseCase>();
        var useCase = new UserLoginUseCase(new FakeTokenService(), fakeLogger);
        var input = new UserLoginInput { UserName = "Albert", Password = "albert123" };

        useCase.Execute(input);

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Executar"));
    }

    [Fact]
    public void Execute_ComCredenciaisValidas_DeveRegistrarLogInformationDeLocalizacao()
    {
        var fakeLogger = new FakeLogger<UserLoginUseCase>();
        var useCase = new UserLoginUseCase(new FakeTokenService(), fakeLogger);
        var input = new UserLoginInput { UserName = "Albert", Password = "albert123" };

        useCase.Execute(input);

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Localizar"));
    }

    [Fact]
    public void Execute_ComCredenciaisValidas_DeveRegistrarLogInformationDeGeracaoToken()
    {
        var fakeLogger = new FakeLogger<UserLoginUseCase>();
        var useCase = new UserLoginUseCase(new FakeTokenService(), fakeLogger);
        var input = new UserLoginInput { UserName = "Albert", Password = "albert123" };

        useCase.Execute(input);

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Gerar"));
    }

    [Fact]
    public void Execute_ComCredenciaisValidas_DeveRegistrarLogInformationNoRetorno()
    {
        var fakeLogger = new FakeLogger<UserLoginUseCase>();
        var useCase = new UserLoginUseCase(new FakeTokenService(), fakeLogger);
        var input = new UserLoginInput { UserName = "Albert", Password = "albert123" };

        useCase.Execute(input);

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar"));
    }

    [Fact]
    public void Execute_ComCredenciaisInvalidas_DeveRegistrarLogWarning()
    {
        var fakeLogger = new FakeLogger<UserLoginUseCase>();
        var useCase = new UserLoginUseCase(new FakeTokenService(), fakeLogger);
        var input = new UserLoginInput { UserName = "invalido", Password = "invalido" };

        useCase.Execute(input);

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Warning &&
            l.Message.Contains("não encontrado"));
    }

    [Fact]
    public void Execute_ComCredenciaisInvalidas_NaoDeveRegistrarLogDeGeracaoToken()
    {
        var fakeLogger = new FakeLogger<UserLoginUseCase>();
        var useCase = new UserLoginUseCase(new FakeTokenService(), fakeLogger);
        var input = new UserLoginInput { UserName = "invalido", Password = "invalido" };

        useCase.Execute(input);

        var logs = fakeLogger.GetSnapshot();
        Assert.DoesNotContain(logs, l => l.Message.Contains("Gerar"));
    }

    [Fact]
    public void Execute_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeLogger = new FakeLogger<UserLoginUseCase>();
        var useCase = new UserLoginUseCase(new FakeTokenService(), fakeLogger);
        var input = new UserLoginInput { UserName = "Albert", Password = "albert123" };

        useCase.Execute(input);

        var logs = fakeLogger.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("UserLoginUseCase", l.Message));
    }
}
