using Starter.Template.AOT.Api.Infra.Security;
using Starter.Template.AOT.UnitTest.TestHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Starter.Template.AOT.UnitTest.Infra.Security;

public sealed class TokenServiceTests
{
    private static TokenService CreateTokenService(FakeLogger<TokenService> logger)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "test-secret-key-for-unit-tests-minimum-length"
            })
            .Build();

        return new TokenService(configuration, logger);
    }

    [Fact]
    public void GenerateToken_DeveRegistrarLogInformationNoInicio()
    {
        var fakeLogger = new FakeLogger<TokenService>();
        var service = CreateTokenService(fakeLogger);

        service.GenerateToken(1, "testuser");

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Gerar"));
    }

    [Fact]
    public void GenerateToken_DeveRegistrarLogInformationNoRetorno()
    {
        var fakeLogger = new FakeLogger<TokenService>();
        var service = CreateTokenService(fakeLogger);

        service.GenerateToken(1, "testuser");

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar"));
    }

    [Fact]
    public void ValidateToken_ComTokenValido_DeveRegistrarLogInformationNoInicio()
    {
        var fakeLogger = new FakeLogger<TokenService>();
        var service = CreateTokenService(fakeLogger);
        var token = service.GenerateToken(1, "testuser");
        fakeLogger.Clear();

        service.ValidateToken(token);

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Validar"));
    }

    [Fact]
    public void ValidateToken_ComTokenValido_DeveRegistrarLogInformationNoRetorno()
    {
        var fakeLogger = new FakeLogger<TokenService>();
        var service = CreateTokenService(fakeLogger);
        var token = service.GenerateToken(1, "testuser");
        fakeLogger.Clear();

        service.ValidateToken(token);

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar"));
    }

    [Fact]
    public void ValidateToken_ComTokenInvalido_DeveRegistrarLogWarning()
    {
        var fakeLogger = new FakeLogger<TokenService>();
        var service = CreateTokenService(fakeLogger);

        service.ValidateToken("token-invalido");

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Warning &&
            l.Message.Contains("Retornar nulo"));
    }

    [Fact]
    public void GenerateToken_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeLogger = new FakeLogger<TokenService>();
        var service = CreateTokenService(fakeLogger);

        service.GenerateToken(1, "testuser");

        var logs = fakeLogger.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("TokenService", l.Message));
    }
}
