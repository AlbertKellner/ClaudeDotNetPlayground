using Albert.Playground.ECS.AOT.Api.Features.Query.TestGet;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Features.Query.TestGet;

public sealed class TestGetEndpointTests
{
    [Fact]
    public void Get_DeveRegistrarLogInformationNoInicio()
    {
        var fakeLoggerUseCase = new FakeLogger<TestGetUseCase>();
        var useCase = new TestGetUseCase(fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<TestGetEndpoint>();
        var endpoint = new TestGetEndpoint(useCase, fakeLoggerEndpoint);

        endpoint.Get();

        var logs = fakeLoggerEndpoint.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Processar"));
    }

    [Fact]
    public void Get_DeveRegistrarLogInformationNoRetorno()
    {
        var fakeLoggerUseCase = new FakeLogger<TestGetUseCase>();
        var useCase = new TestGetUseCase(fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<TestGetEndpoint>();
        var endpoint = new TestGetEndpoint(useCase, fakeLoggerEndpoint);

        endpoint.Get();

        var logs = fakeLoggerEndpoint.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar"));
    }

    [Fact]
    public void Get_DeveRetornarOkComResultadoCorreto()
    {
        var fakeLoggerUseCase = new FakeLogger<TestGetUseCase>();
        var useCase = new TestGetUseCase(fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<TestGetEndpoint>();
        var endpoint = new TestGetEndpoint(useCase, fakeLoggerEndpoint);

        var actionResult = endpoint.Get();

        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal("funcionando", okResult.Value);
    }

    [Fact]
    public void Get_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeLoggerUseCase = new FakeLogger<TestGetUseCase>();
        var useCase = new TestGetUseCase(fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<TestGetEndpoint>();
        var endpoint = new TestGetEndpoint(useCase, fakeLoggerEndpoint);

        endpoint.Get();

        var logs = fakeLoggerEndpoint.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("TestGetEndpoint", l.Message));
    }
}
