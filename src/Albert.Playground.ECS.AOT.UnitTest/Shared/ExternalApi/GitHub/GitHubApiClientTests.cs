using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Shared.ExternalApi.GitHub;

public sealed class GitHubApiClientTests
{
    private sealed class FakeGitHubApi : IGitHubApi
    {
        public Task<List<GitHubRepositoryOutput>> GetTeamReposAsync(
            string org,
            string teamSlug,
            CancellationToken cancellationToken = default)
        {
            var repos = new List<GitHubRepositoryOutput>
            {
                new()
                {
                    Name = "repo-one",
                    Description = "First repo",
                    CloneUrl = "https://github.com/WebMotors/repo-one.git",
                    UpdatedAt = "2026-03-19T10:00:00Z"
                },
                new()
                {
                    Name = "repo-two",
                    Description = "Second repo",
                    CloneUrl = "https://github.com/WebMotors/repo-two.git",
                    UpdatedAt = "2026-03-18T10:00:00Z"
                }
            };

            return Task.FromResult(repos);
        }
    }

    [Fact]
    public async Task GetTeamReposAsync_DeveRegistrarLogInformationNoInicio()
    {
        var fakeApi = new FakeGitHubApi();
        var fakeLogger = new FakeLogger<GitHubApiClient>();
        var client = new GitHubApiClient(fakeApi, fakeLogger);

        await client.GetTeamReposAsync("WebMotors", "IntegrationRepos");

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Executar"));
    }

    [Fact]
    public async Task GetTeamReposAsync_DeveRegistrarLogInformationNoRetorno()
    {
        var fakeApi = new FakeGitHubApi();
        var fakeLogger = new FakeLogger<GitHubApiClient>();
        var client = new GitHubApiClient(fakeApi, fakeLogger);

        await client.GetTeamReposAsync("WebMotors", "IntegrationRepos");

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar"));
    }

    [Fact]
    public async Task GetTeamReposAsync_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeApi = new FakeGitHubApi();
        var fakeLogger = new FakeLogger<GitHubApiClient>();
        var client = new GitHubApiClient(fakeApi, fakeLogger);

        await client.GetTeamReposAsync("WebMotors", "IntegrationRepos");

        var logs = fakeLogger.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("GitHubApiClient", l.Message));
    }

    [Fact]
    public async Task GetTeamReposAsync_DeveRetornarListaDeRepositorios()
    {
        var fakeApi = new FakeGitHubApi();
        var fakeLogger = new FakeLogger<GitHubApiClient>();
        var client = new GitHubApiClient(fakeApi, fakeLogger);

        var result = await client.GetTeamReposAsync("WebMotors", "IntegrationRepos");

        Assert.Equal(2, result.Count);
        Assert.Equal("repo-one", result[0].Name);
        Assert.Equal("repo-two", result[1].Name);
    }
}
