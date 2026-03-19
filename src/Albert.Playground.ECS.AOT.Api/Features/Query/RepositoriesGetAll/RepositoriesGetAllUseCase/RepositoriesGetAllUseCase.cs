using System.Text.Json;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;
using Albert.Playground.ECS.AOT.Api.Shared.Repositories;

namespace Albert.Playground.ECS.AOT.Api.Features.Query.RepositoriesGetAll;

public sealed class RepositoriesGetAllUseCase(
    IGitHubApiClient gitHubApiClient,
    IConfiguration configuration,
    ILogger<RepositoriesGetAllUseCase> logger)
{
    public async Task<RepositoriesGetAllOutput> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[RepositoriesGetAllUseCase][ExecuteAsync] Executar caso de uso de listagem de repositórios");

        var repositories = await gitHubApiClient.GetTeamReposAsync("WebMotors", "IntegrationRepos", cancellationToken);

        logger.LogInformation(
            "[RepositoriesGetAllUseCase][ExecuteAsync] Iterar {Count} repositórios retornados pela API",
            repositories.Count);

        var entries = new List<RepositoryFileEntry>();

        foreach (var repo in repositories)
        {
            logger.LogInformation(
                "[RepositoriesGetAllUseCase][ExecuteAsync] Repositório encontrado. Name={Name}",
                repo.Name);

            entries.Add(new RepositoryFileEntry
            {
                Name = repo.Name,
                Description = repo.Description,
                GitUrl = repo.CloneUrl,
                LastModifiedDate = repo.UpdatedAt,
                LastSyncDate = string.Empty
            });
        }

        logger.LogInformation(
            "[RepositoriesGetAllUseCase][ExecuteAsync] Iteração dos repositórios concluída");

        var jsonFilePath = configuration["Repositories:JsonFilePath"] ?? "data/repositories.json";
        var directory = Path.GetDirectoryName(jsonFilePath);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(entries, RepositoryFileJsonContext.Default.ListRepositoryFileEntry);
        await File.WriteAllTextAsync(jsonFilePath, json, cancellationToken);

        logger.LogInformation(
            "[RepositoriesGetAllUseCase][ExecuteAsync] Salvar lista de repositórios no arquivo JSON. Path={Path}",
            jsonFilePath);

        var output = new RepositoriesGetAllOutput { Repositories = entries };

        logger.LogInformation(
            "[RepositoriesGetAllUseCase][ExecuteAsync] Retornar lista de repositórios. Count={Count}",
            entries.Count);

        return output;
    }
}
