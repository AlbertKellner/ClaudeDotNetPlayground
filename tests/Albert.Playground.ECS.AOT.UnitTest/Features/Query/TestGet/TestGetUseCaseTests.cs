using Albert.Playground.ECS.AOT.Api.Features.Query.TestGet;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Features.Query.TestGet;

public sealed class TestGetUseCaseTests
{
    [Fact]
    public void Execute_DeveRegistrarLogInformationNoInicio()
    {
        var fakeLogger = new FakeLogger<TestGetUseCase>();
        var useCase = new TestGetUseCase(fakeLogger);

        useCase.Execute();

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Executar"));
    }

    [Fact]
    public void Execute_DeveRegistrarLogInformationNoRetorno()
    {
        var fakeLogger = new FakeLogger<TestGetUseCase>();
        var useCase = new TestGetUseCase(fakeLogger);

        useCase.Execute();

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar"));
    }

    [Fact]
    public void Execute_DeveRegistrarDoisLogsNoTotal()
    {
        var fakeLogger = new FakeLogger<TestGetUseCase>();
        var useCase = new TestGetUseCase(fakeLogger);

        useCase.Execute();

        var logs = fakeLogger.GetSnapshot();
        Assert.Equal(2, logs.Count);
    }

    [Fact]
    public void Execute_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeLogger = new FakeLogger<TestGetUseCase>();
        var useCase = new TestGetUseCase(fakeLogger);

        useCase.Execute();

        var logs = fakeLogger.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("TestGetUseCase", l.Message));
    }
}
