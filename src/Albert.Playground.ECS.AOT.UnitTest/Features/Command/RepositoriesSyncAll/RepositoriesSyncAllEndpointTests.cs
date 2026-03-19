using Albert.Playground.ECS.AOT.Api.Features.Command.RepositoriesSyncAll;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Features.Command.RepositoriesSyncAll;

public sealed class RepositoriesSyncAllEndpointTests
{
    private static IConfiguration CreateConfiguration(string jsonFilePath)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Repositories:JsonFilePath"] = jsonFilePath,
                ["Repositories:SyncRootPath"] = Path.GetTempPath()
            })
            .Build();
    }

    [Fact]
    public async Task Post_DeveRegistrarLogInformationNoInicio()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        try
        {
            var config = CreateConfiguration(tempFile);
            var fakeLoggerUseCase = new FakeLogger<RepositoriesSyncAllUseCase>();
            var useCase = new RepositoriesSyncAllUseCase(config, fakeLoggerUseCase);
            var fakeLoggerEndpoint = new FakeLogger<RepositoriesSyncAllEndpoint>();
            var endpoint = new RepositoriesSyncAllEndpoint(useCase, fakeLoggerEndpoint);

            await endpoint.Post(CancellationToken.None);

            var logs = fakeLoggerEndpoint.GetSnapshot();
            Assert.Contains(logs, l =>
                l.Level == LogLevel.Information &&
                l.Message.Contains("Processar"));
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task Post_DeveRegistrarLogInformationNoRetorno()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        try
        {
            var config = CreateConfiguration(tempFile);
            var fakeLoggerUseCase = new FakeLogger<RepositoriesSyncAllUseCase>();
            var useCase = new RepositoriesSyncAllUseCase(config, fakeLoggerUseCase);
            var fakeLoggerEndpoint = new FakeLogger<RepositoriesSyncAllEndpoint>();
            var endpoint = new RepositoriesSyncAllEndpoint(useCase, fakeLoggerEndpoint);

            await endpoint.Post(CancellationToken.None);

            var logs = fakeLoggerEndpoint.GetSnapshot();
            Assert.Contains(logs, l =>
                l.Level == LogLevel.Information &&
                l.Message.Contains("Retornar"));
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task Post_DeveRetornarOkComOutput()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        try
        {
            var config = CreateConfiguration(tempFile);
            var fakeLoggerUseCase = new FakeLogger<RepositoriesSyncAllUseCase>();
            var useCase = new RepositoriesSyncAllUseCase(config, fakeLoggerUseCase);
            var fakeLoggerEndpoint = new FakeLogger<RepositoriesSyncAllEndpoint>();
            var endpoint = new RepositoriesSyncAllEndpoint(useCase, fakeLoggerEndpoint);

            var actionResult = await endpoint.Post(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.IsType<RepositoriesSyncAllOutput>(okResult.Value);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task Post_DeveRegistrarLogsComPrefixoCorreto()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        try
        {
            var config = CreateConfiguration(tempFile);
            var fakeLoggerUseCase = new FakeLogger<RepositoriesSyncAllUseCase>();
            var useCase = new RepositoriesSyncAllUseCase(config, fakeLoggerUseCase);
            var fakeLoggerEndpoint = new FakeLogger<RepositoriesSyncAllEndpoint>();
            var endpoint = new RepositoriesSyncAllEndpoint(useCase, fakeLoggerEndpoint);

            await endpoint.Post(CancellationToken.None);

            var logs = fakeLoggerEndpoint.GetSnapshot();
            Assert.All(logs, l => Assert.Contains("RepositoriesSyncAllEndpoint", l.Message));
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
}
