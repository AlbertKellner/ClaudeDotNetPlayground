using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;

namespace Albert.Playground.ECS.AOT.Api.Features.Query.GitHubRepoSearch;

public sealed class GitHubRepoSearchUseCase(
    IGitHubApiClient gitHubApiClient,
    ILogger<GitHubRepoSearchUseCase> logger)
{
    private const string GitHubUsername = "AlbertKellner";

    public async Task<GitHubRepoSearchOutput> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[GitHubRepoSearchUseCase][ExecuteAsync] Executar caso de uso de pesquisa de repositórios GitHub");

        logger.LogInformation(
            "[GitHubRepoSearchUseCase][ExecuteAsync] Consultar API GitHub. Username={Username}",
            GitHubUsername);

        var repositories = await gitHubApiClient.GetRepositoriesAsync(GitHubUsername, cancellationToken);

        logger.LogInformation(
            "[GitHubRepoSearchUseCase][ExecuteAsync] Mapear repositórios para model da Feature. Total={Total}",
            repositories.Count);

        var items = new List<GitHubRepoSearchItem>(repositories.Count);

        logger.LogInformation(
            "[GitHubRepoSearchUseCase][ExecuteAsync] Iniciar iteração de repositórios para mapeamento");

        foreach (var repo in repositories)
        {
            items.Add(new GitHubRepoSearchItem
            {
                Name = repo.Name,
                GitUrl = repo.GitUrl
            });
        }

        logger.LogInformation(
            "[GitHubRepoSearchUseCase][ExecuteAsync] Concluir iteração de repositórios. Total={Total}",
            items.Count);

        var output = new GitHubRepoSearchOutput
        {
            Repositories = items
        };

        logger.LogInformation(
            "[GitHubRepoSearchUseCase][ExecuteAsync] Retornar lista de repositórios GitHub. Total={Total}",
            output.Repositories.Count);

        return output;
    }
}
