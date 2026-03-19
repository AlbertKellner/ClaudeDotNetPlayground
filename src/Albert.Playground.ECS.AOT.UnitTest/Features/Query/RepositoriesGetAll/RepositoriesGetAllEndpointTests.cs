using Albert.Playground.ECS.AOT.Api.Features.Query.RepositoriesGetAll;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Features.Query.RepositoriesGetAll;

public sealed class RepositoriesGetAllEndpointTests
{
    private sealed class FakeGitHubApiClient : IGitHubApiClient
    {
        public Task<List<GitHubRepositoryOutput>> GetTeamReposAsync(
            string org,
            string teamSlug,
            CancellationToken cancellationToken = default)
        {
            var repos = new List<GitHubRepositoryOutput>
            {
                new() { Name = "repo-one", CloneUrl = "https://github.com/WebMotors/repo-one.git", UpdatedAt = "2026-03-19T10:00:00Z" }
            };

            return Task.FromResult(repos);
        }
    }

    private static IConfiguration CreateConfiguration(string jsonFilePath)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Repositories:JsonFilePath"] = jsonFilePath
            })
            .Build();
    }

    [Fact]
    public async Task Get_DeveRegistrarLogInformationNoInicio()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            var fakeClient = new FakeGitHubApiClient();
            var config = CreateConfiguration(tempFile);
            var fakeLoggerUseCase = new FakeLogger<RepositoriesGetAllUseCase>();
            var useCase = new RepositoriesGetAllUseCase(fakeClient, config, fakeLoggerUseCase);
            var fakeLoggerEndpoint = new FakeLogger<RepositoriesGetAllEndpoint>();
            var endpoint = new RepositoriesGetAllEndpoint(useCase, fakeLoggerEndpoint);

            await endpoint.Get(CancellationToken.None);

            var logs = fakeLoggerEndpoint.GetSnapshot();
            Assert.Contains(logs, l =>
                l.Level == LogLevel.Information &&
                l.Message.Contains("Processar"));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task Get_DeveRegistrarLogInformationNoRetorno()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            var fakeClient = new FakeGitHubApiClient();
            var config = CreateConfiguration(tempFile);
            var fakeLoggerUseCase = new FakeLogger<RepositoriesGetAllUseCase>();
            var useCase = new RepositoriesGetAllUseCase(fakeClient, config, fakeLoggerUseCase);
            var fakeLoggerEndpoint = new FakeLogger<RepositoriesGetAllEndpoint>();
            var endpoint = new RepositoriesGetAllEndpoint(useCase, fakeLoggerEndpoint);

            await endpoint.Get(CancellationToken.None);

            var logs = fakeLoggerEndpoint.GetSnapshot();
            Assert.Contains(logs, l =>
                l.Level == LogLevel.Information &&
                l.Message.Contains("Retornar"));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task Get_DeveRetornarOkComOutput()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            var fakeClient = new FakeGitHubApiClient();
            var config = CreateConfiguration(tempFile);
            var fakeLoggerUseCase = new FakeLogger<RepositoriesGetAllUseCase>();
            var useCase = new RepositoriesGetAllUseCase(fakeClient, config, fakeLoggerUseCase);
            var fakeLoggerEndpoint = new FakeLogger<RepositoriesGetAllEndpoint>();
            var endpoint = new RepositoriesGetAllEndpoint(useCase, fakeLoggerEndpoint);

            var actionResult = await endpoint.Get(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var output = Assert.IsType<RepositoriesGetAllOutput>(okResult.Value);
            Assert.Single(output.Repositories);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task Get_DeveRegistrarLogsComPrefixoCorreto()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            var fakeClient = new FakeGitHubApiClient();
            var config = CreateConfiguration(tempFile);
            var fakeLoggerUseCase = new FakeLogger<RepositoriesGetAllUseCase>();
            var useCase = new RepositoriesGetAllUseCase(fakeClient, config, fakeLoggerUseCase);
            var fakeLoggerEndpoint = new FakeLogger<RepositoriesGetAllEndpoint>();
            var endpoint = new RepositoriesGetAllEndpoint(useCase, fakeLoggerEndpoint);

            await endpoint.Get(CancellationToken.None);

            var logs = fakeLoggerEndpoint.GetSnapshot();
            Assert.All(logs, l => Assert.Contains("RepositoriesGetAllEndpoint", l.Message));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}
