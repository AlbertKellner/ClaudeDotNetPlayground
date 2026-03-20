using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Shared.ExternalApi.GitHub;

public sealed class GitHubApiClientTests
{
    private sealed class FakeGitHubApi : IGitHubApi
    {
        private readonly List<List<GitHubRepositoryOutput>> _pages;

        public FakeGitHubApi(List<List<GitHubRepositoryOutput>>? pages = null)
        {
            _pages = pages ?? [CreateDefaultPage()];
        }

        public int CallCount { get; private set; }

        public Task<List<GitHubRepositoryOutput>> GetRepositoriesAsync(
            string username,
            int per_page = 100,
            int page = 1,
            CancellationToken cancellationToken = default)
        {
            CallCount++;

            var result = page <= _pages.Count ? _pages[page - 1] : [];

            return Task.FromResult(result);
        }

        private static List<GitHubRepositoryOutput> CreateDefaultPage() =>
        [
            new GitHubRepositoryOutput
            {
                Name = "repo-1",
                FullName = "AlbertKellner/repo-1",
                GitUrl = "git://github.com/AlbertKellner/repo-1.git"
            },
            new GitHubRepositoryOutput
            {
                Name = "repo-2",
                FullName = "AlbertKellner/repo-2",
                GitUrl = "git://github.com/AlbertKellner/repo-2.git"
            }
        ];
    }

    [Fact]
    public async Task GetRepositoriesAsync_DeveRegistrarLogInformationNoInicio()
    {
        var fakeApi = new FakeGitHubApi();
        var fakeLogger = new FakeLogger<GitHubApiClient>();
        var client = new GitHubApiClient(fakeApi, fakeLogger);

        await client.GetRepositoriesAsync("AlbertKellner");

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Executar"));
    }

    [Fact]
    public async Task GetRepositoriesAsync_DeveRegistrarLogInformationNoRetorno()
    {
        var fakeApi = new FakeGitHubApi();
        var fakeLogger = new FakeLogger<GitHubApiClient>();
        var client = new GitHubApiClient(fakeApi, fakeLogger);

        await client.GetRepositoriesAsync("AlbertKellner");

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar"));
    }

    [Fact]
    public async Task GetRepositoriesAsync_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeApi = new FakeGitHubApi();
        var fakeLogger = new FakeLogger<GitHubApiClient>();
        var client = new GitHubApiClient(fakeApi, fakeLogger);

        await client.GetRepositoriesAsync("AlbertKellner");

        var logs = fakeLogger.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("GitHubApiClient", l.Message));
    }

    [Fact]
    public async Task GetRepositoriesAsync_DeveRetornarRepositoriosDaApi()
    {
        var fakeApi = new FakeGitHubApi();
        var fakeLogger = new FakeLogger<GitHubApiClient>();
        var client = new GitHubApiClient(fakeApi, fakeLogger);

        var result = await client.GetRepositoriesAsync("AlbertKellner");

        Assert.Equal(2, result.Count);
        Assert.Equal("repo-1", result[0].Name);
        Assert.Equal("repo-2", result[1].Name);
    }

    [Fact]
    public async Task GetRepositoriesAsync_DeveIterarMultiplasPaginas()
    {
        var page1 = new List<GitHubRepositoryOutput>
        {
            new() { Name = "repo-1", GitUrl = "git://github.com/AlbertKellner/repo-1.git" }
        };
        var page2 = new List<GitHubRepositoryOutput>
        {
            new() { Name = "repo-2", GitUrl = "git://github.com/AlbertKellner/repo-2.git" }
        };
        var fakeApi = new FakeGitHubApi([page1, page2]);
        var fakeLogger = new FakeLogger<GitHubApiClient>();
        var client = new GitHubApiClient(fakeApi, fakeLogger);

        var result = await client.GetRepositoriesAsync("AlbertKellner");

        Assert.Equal(2, result.Count);
        Assert.Equal("repo-1", result[0].Name);
        Assert.Equal("repo-2", result[1].Name);
        Assert.Equal(3, fakeApi.CallCount);
    }

    [Fact]
    public async Task GetRepositoriesAsync_DeveRegistrarLogDeIteracao()
    {
        var fakeApi = new FakeGitHubApi();
        var fakeLogger = new FakeLogger<GitHubApiClient>();
        var client = new GitHubApiClient(fakeApi, fakeLogger);

        await client.GetRepositoriesAsync("AlbertKellner");

        var logs = fakeLogger.GetSnapshot();

        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Iniciar iteração"));

        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Concluir iteração"));
    }
}
