using Albert.Playground.ECS.AOT.Api.Features.Command.RepositoriesSyncAll;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Features.Command.RepositoriesSyncAll;

public sealed class RepositoriesSyncAllUseCaseTests
{
    private static IConfiguration CreateConfiguration(string jsonFilePath, string? syncRootPath = null)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Repositories:JsonFilePath"] = jsonFilePath,
                ["Repositories:SyncRootPath"] = syncRootPath ?? Path.GetTempPath()
            })
            .Build();
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogInformationNoInicio()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        try
        {
            var config = CreateConfiguration(tempFile);
            var fakeLogger = new FakeLogger<RepositoriesSyncAllUseCase>();
            var useCase = new RepositoriesSyncAllUseCase(config, fakeLogger);

            await useCase.ExecuteAsync();

            var logs = fakeLogger.GetSnapshot();
            Assert.Contains(logs, l =>
                l.Level == LogLevel.Information &&
                l.Message.Contains("Executar"));
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogErroQuandoArquivoNaoExiste()
    {
        var nonExistentFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
        var config = CreateConfiguration(nonExistentFile);
        var fakeLogger = new FakeLogger<RepositoriesSyncAllUseCase>();
        var useCase = new RepositoriesSyncAllUseCase(config, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Error &&
            l.Message.Contains("não encontrado"));
    }

    [Fact]
    public async Task ExecuteAsync_DeveRetornarOutputVazioQuandoArquivoNaoExiste()
    {
        var nonExistentFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
        var config = CreateConfiguration(nonExistentFile);
        var fakeLogger = new FakeLogger<RepositoriesSyncAllUseCase>();
        var useCase = new RepositoriesSyncAllUseCase(config, fakeLogger);

        var result = await useCase.ExecuteAsync();

        Assert.Equal(0, result.TotalRepositories);
        Assert.Equal(0, result.SuccessCount);
        Assert.Equal(0, result.ErrorCount);
        Assert.Empty(result.Results);
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogsComPrefixoCorreto()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        try
        {
            var config = CreateConfiguration(tempFile);
            var fakeLogger = new FakeLogger<RepositoriesSyncAllUseCase>();
            var useCase = new RepositoriesSyncAllUseCase(config, fakeLogger);

            await useCase.ExecuteAsync();

            var logs = fakeLogger.GetSnapshot();
            Assert.All(logs, l => Assert.Contains("RepositoriesSyncAllUseCase", l.Message));
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogAntesDeIterarQuandoArquivoExiste()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        try
        {
            await File.WriteAllTextAsync(tempFile, "[]");
            var config = CreateConfiguration(tempFile);
            var fakeLogger = new FakeLogger<RepositoriesSyncAllUseCase>();
            var useCase = new RepositoriesSyncAllUseCase(config, fakeLogger);

            await useCase.ExecuteAsync();

            var logs = fakeLogger.GetSnapshot();
            Assert.Contains(logs, l =>
                l.Level == LogLevel.Information &&
                l.Message.Contains("Iterar"));
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogAposIteracaoQuandoArquivoExiste()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        try
        {
            await File.WriteAllTextAsync(tempFile, "[]");
            var config = CreateConfiguration(tempFile);
            var fakeLogger = new FakeLogger<RepositoriesSyncAllUseCase>();
            var useCase = new RepositoriesSyncAllUseCase(config, fakeLogger);

            await useCase.ExecuteAsync();

            var logs = fakeLogger.GetSnapshot();
            Assert.Contains(logs, l =>
                l.Level == LogLevel.Information &&
                l.Message.Contains("concluída"));
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
}
