namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;

public sealed class GitHubApiClient(
    IGitHubApi api,
    ILogger<GitHubApiClient> logger) : IGitHubApiClient
{
    public async Task<List<GitHubRepositoryOutput>> GetRepositoriesAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[GitHubApiClient][GetRepositoriesAsync] Executar requisição HTTP ao GitHub. Username={Username}",
            username);

        var allRepositories = new List<GitHubRepositoryOutput>();
        var page = 1;

        logger.LogInformation(
            "[GitHubApiClient][GetRepositoriesAsync] Iniciar iteração de páginas da API GitHub");

        while (true)
        {
            var repositories = await api.GetRepositoriesAsync(username, per_page: 100, page: page, cancellationToken: cancellationToken);

            if (repositories.Count == 0)
            {
                break;
            }

            allRepositories.AddRange(repositories);
            page++;
        }

        logger.LogInformation(
            "[GitHubApiClient][GetRepositoriesAsync] Concluir iteração de páginas. TotalRepositórios={Total}",
            allRepositories.Count);

        logger.LogInformation(
            "[GitHubApiClient][GetRepositoriesAsync] Retornar lista de repositórios do GitHub. Total={Total}",
            allRepositories.Count);

        return allRepositories;
    }
}
