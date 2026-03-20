using Albert.Playground.ECS.AOT.Api.Features.Query.GitHubRepoSearch;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Features.Query.GitHubRepoSearch;

public sealed class GitHubRepoSearchEndpointTests
{
    private sealed class FakeGitHubApiClient : IGitHubApiClient
    {
        public Task<List<GitHubRepositoryOutput>> GetRepositoriesAsync(
            string username,
            CancellationToken cancellationToken = default)
        {
            var result = new List<GitHubRepositoryOutput>
            {
                new()
                {
                    Name = "repo-1",
                    FullName = "AlbertKellner/repo-1",
                    GitUrl = "git://github.com/AlbertKellner/repo-1.git"
                }
            };

            return Task.FromResult(result);
        }
    }

    [Fact]
    public async Task Get_DeveRegistrarLogInformationNoInicio()
    {
        var fakeClient = new FakeGitHubApiClient();
        var fakeLoggerUseCase = new FakeLogger<GitHubRepoSearchUseCase>();
        var useCase = new GitHubRepoSearchUseCase(fakeClient, fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<GitHubRepoSearchEndpoint>();
        var endpoint = new GitHubRepoSearchEndpoint(useCase, fakeLoggerEndpoint);

        await endpoint.Get(CancellationToken.None);

        var logs = fakeLoggerEndpoint.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Processar"));
    }

    [Fact]
    public async Task Get_DeveRegistrarLogInformationNoRetorno()
    {
        var fakeClient = new FakeGitHubApiClient();
        var fakeLoggerUseCase = new FakeLogger<GitHubRepoSearchUseCase>();
        var useCase = new GitHubRepoSearchUseCase(fakeClient, fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<GitHubRepoSearchEndpoint>();
        var endpoint = new GitHubRepoSearchEndpoint(useCase, fakeLoggerEndpoint);

        await endpoint.Get(CancellationToken.None);

        var logs = fakeLoggerEndpoint.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar"));
    }

    [Fact]
    public async Task Get_DeveRetornarOkComOutputCompleto()
    {
        var fakeClient = new FakeGitHubApiClient();
        var fakeLoggerUseCase = new FakeLogger<GitHubRepoSearchUseCase>();
        var useCase = new GitHubRepoSearchUseCase(fakeClient, fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<GitHubRepoSearchEndpoint>();
        var endpoint = new GitHubRepoSearchEndpoint(useCase, fakeLoggerEndpoint);

        var actionResult = await endpoint.Get(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var output = Assert.IsType<GitHubRepoSearchOutput>(okResult.Value);
        Assert.Single(output.Repositories);
        Assert.Equal("repo-1", output.Repositories[0].Name);
    }

    [Fact]
    public async Task Get_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeClient = new FakeGitHubApiClient();
        var fakeLoggerUseCase = new FakeLogger<GitHubRepoSearchUseCase>();
        var useCase = new GitHubRepoSearchUseCase(fakeClient, fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<GitHubRepoSearchEndpoint>();
        var endpoint = new GitHubRepoSearchEndpoint(useCase, fakeLoggerEndpoint);

        await endpoint.Get(CancellationToken.None);

        var logs = fakeLoggerEndpoint.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("GitHubRepoSearchEndpoint", l.Message));
    }
}
