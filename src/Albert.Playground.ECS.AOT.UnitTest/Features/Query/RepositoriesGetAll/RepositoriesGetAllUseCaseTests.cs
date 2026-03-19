using Albert.Playground.ECS.AOT.Api.Features.Query.RepositoriesGetAll;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Features.Query.RepositoriesGetAll;

public sealed class RepositoriesGetAllUseCaseTests
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
                new()
                {
                    Name = "repo-alpha",
                    Description = "Alpha repository",
                    CloneUrl = "https://github.com/WebMotors/repo-alpha.git",
                    UpdatedAt = "2026-03-19T10:00:00Z"
                },
                new()
                {
                    Name = "repo-beta",
                    Description = null,
                    CloneUrl = "https://github.com/WebMotors/repo-beta.git",
                    UpdatedAt = "2026-03-18T08:30:00Z"
                }
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
    public async Task ExecuteAsync_DeveRegistrarLogInformationNoInicio()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            var fakeClient = new FakeGitHubApiClient();
            var config = CreateConfiguration(tempFile);
            var fakeLogger = new FakeLogger<RepositoriesGetAllUseCase>();
            var useCase = new RepositoriesGetAllUseCase(fakeClient, config, fakeLogger);

            await useCase.ExecuteAsync();

            var logs = fakeLogger.GetSnapshot();
            Assert.Contains(logs, l =>
                l.Level == LogLevel.Information &&
                l.Message.Contains("Executar"));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogAntesDeIterar()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            var fakeClient = new FakeGitHubApiClient();
            var config = CreateConfiguration(tempFile);
            var fakeLogger = new FakeLogger<RepositoriesGetAllUseCase>();
            var useCase = new RepositoriesGetAllUseCase(fakeClient, config, fakeLogger);

            await useCase.ExecuteAsync();

            var logs = fakeLogger.GetSnapshot();
            Assert.Contains(logs, l =>
                l.Level == LogLevel.Information &&
                l.Message.Contains("Iterar"));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogParaCadaRepositorio()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            var fakeClient = new FakeGitHubApiClient();
            var config = CreateConfiguration(tempFile);
            var fakeLogger = new FakeLogger<RepositoriesGetAllUseCase>();
            var useCase = new RepositoriesGetAllUseCase(fakeClient, config, fakeLogger);

            await useCase.ExecuteAsync();

            var logs = fakeLogger.GetSnapshot();
            Assert.Contains(logs, l =>
                l.Level == LogLevel.Information &&
                l.Message.Contains("repo-alpha"));
            Assert.Contains(logs, l =>
                l.Level == LogLevel.Information &&
                l.Message.Contains("repo-beta"));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogAposIteracao()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            var fakeClient = new FakeGitHubApiClient();
            var config = CreateConfiguration(tempFile);
            var fakeLogger = new FakeLogger<RepositoriesGetAllUseCase>();
            var useCase = new RepositoriesGetAllUseCase(fakeClient, config, fakeLogger);

            await useCase.ExecuteAsync();

            var logs = fakeLogger.GetSnapshot();
            Assert.Contains(logs, l =>
                l.Level == LogLevel.Information &&
                l.Message.Contains("concluída"));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogNoRetorno()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            var fakeClient = new FakeGitHubApiClient();
            var config = CreateConfiguration(tempFile);
            var fakeLogger = new FakeLogger<RepositoriesGetAllUseCase>();
            var useCase = new RepositoriesGetAllUseCase(fakeClient, config, fakeLogger);

            await useCase.ExecuteAsync();

            var logs = fakeLogger.GetSnapshot();
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
    public async Task ExecuteAsync_DeveRegistrarLogsComPrefixoCorreto()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            var fakeClient = new FakeGitHubApiClient();
            var config = CreateConfiguration(tempFile);
            var fakeLogger = new FakeLogger<RepositoriesGetAllUseCase>();
            var useCase = new RepositoriesGetAllUseCase(fakeClient, config, fakeLogger);

            await useCase.ExecuteAsync();

            var logs = fakeLogger.GetSnapshot();
            Assert.All(logs, l => Assert.Contains("RepositoriesGetAllUseCase", l.Message));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_DeveRetornarRepositoriosMapeadosCorretamente()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            var fakeClient = new FakeGitHubApiClient();
            var config = CreateConfiguration(tempFile);
            var fakeLogger = new FakeLogger<RepositoriesGetAllUseCase>();
            var useCase = new RepositoriesGetAllUseCase(fakeClient, config, fakeLogger);

            var result = await useCase.ExecuteAsync();

            Assert.Equal(2, result.Repositories.Count);
            Assert.Equal("repo-alpha", result.Repositories[0].Name);
            Assert.Equal("https://github.com/WebMotors/repo-alpha.git", result.Repositories[0].GitUrl);
            Assert.Equal(string.Empty, result.Repositories[0].LastSyncDate);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_DeveSalvarArquivoJson()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            var fakeClient = new FakeGitHubApiClient();
            var config = CreateConfiguration(tempFile);
            var fakeLogger = new FakeLogger<RepositoriesGetAllUseCase>();
            var useCase = new RepositoriesGetAllUseCase(fakeClient, config, fakeLogger);

            await useCase.ExecuteAsync();

            var fileContent = await File.ReadAllTextAsync(tempFile);
            Assert.Contains("repo-alpha", fileContent);
            Assert.Contains("repo-beta", fileContent);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}
