using Albert.Playground.ECS.AOT.Api.Features.Query.GitHubRepoSearch;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Features.Query.GitHubRepoSearch;

public sealed class GitHubRepoSearchUseCaseTests
{
    private sealed class FakeGitHubApiClient : IGitHubApiClient
    {
        public string? LastUsername { get; private set; }

        public Task<List<GitHubRepositoryOutput>> GetRepositoriesAsync(
            string username,
            CancellationToken cancellationToken = default)
        {
            LastUsername = username;

            var result = new List<GitHubRepositoryOutput>
            {
                new()
                {
                    Name = "repo-1",
                    FullName = "AlbertKellner/repo-1",
                    GitUrl = "git://github.com/AlbertKellner/repo-1.git"
                },
                new()
                {
                    Name = "repo-2",
                    FullName = "AlbertKellner/repo-2",
                    GitUrl = "git://github.com/AlbertKellner/repo-2.git"
                }
            };

            return Task.FromResult(result);
        }
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogInformationNoInicio()
    {
        var fakeClient = new FakeGitHubApiClient();
        var fakeLogger = new FakeLogger<GitHubRepoSearchUseCase>();
        var useCase = new GitHubRepoSearchUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Executar"));
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogInformationAoConsultarApi()
    {
        var fakeClient = new FakeGitHubApiClient();
        var fakeLogger = new FakeLogger<GitHubRepoSearchUseCase>();
        var useCase = new GitHubRepoSearchUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Consultar"));
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogInformationAoMapear()
    {
        var fakeClient = new FakeGitHubApiClient();
        var fakeLogger = new FakeLogger<GitHubRepoSearchUseCase>();
        var useCase = new GitHubRepoSearchUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Mapear"));
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogInformationNoRetorno()
    {
        var fakeClient = new FakeGitHubApiClient();
        var fakeLogger = new FakeLogger<GitHubRepoSearchUseCase>();
        var useCase = new GitHubRepoSearchUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar"));
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeClient = new FakeGitHubApiClient();
        var fakeLogger = new FakeLogger<GitHubRepoSearchUseCase>();
        var useCase = new GitHubRepoSearchUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("GitHubRepoSearchUseCase", l.Message));
    }

    [Fact]
    public async Task ExecuteAsync_DeveRetornarOutputComRepositoriosMapeados()
    {
        var fakeClient = new FakeGitHubApiClient();
        var fakeLogger = new FakeLogger<GitHubRepoSearchUseCase>();
        var useCase = new GitHubRepoSearchUseCase(fakeClient, fakeLogger);

        var result = await useCase.ExecuteAsync();

        Assert.IsType<GitHubRepoSearchOutput>(result);
        Assert.Equal(2, result.Repositories.Count);
        Assert.Equal("repo-1", result.Repositories[0].Name);
        Assert.Equal("git://github.com/AlbertKellner/repo-1.git", result.Repositories[0].GitUrl);
        Assert.Equal("repo-2", result.Repositories[1].Name);
        Assert.Equal("git://github.com/AlbertKellner/repo-2.git", result.Repositories[1].GitUrl);
    }

    [Fact]
    public async Task ExecuteAsync_DevePassarUsernameAlbertKellnerParaApi()
    {
        var fakeClient = new FakeGitHubApiClient();
        var fakeLogger = new FakeLogger<GitHubRepoSearchUseCase>();
        var useCase = new GitHubRepoSearchUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        Assert.Equal("AlbertKellner", fakeClient.LastUsername);
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogDeIteracao()
    {
        var fakeClient = new FakeGitHubApiClient();
        var fakeLogger = new FakeLogger<GitHubRepoSearchUseCase>();
        var useCase = new GitHubRepoSearchUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();

        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Iniciar iteração"));

        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Concluir iteração"));
    }
}
