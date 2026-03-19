namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;

public sealed class GitHubApiClient(
    IGitHubApi api,
    ILogger<GitHubApiClient> logger) : IGitHubApiClient
{
    public async Task<List<GitHubRepositoryOutput>> GetTeamReposAsync(
        string org,
        string teamSlug,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[GitHubApiClient][GetTeamReposAsync] Executar requisição HTTP ao GitHub. Org={Org}, Team={Team}",
            org,
            teamSlug);

        var result = await api.GetTeamReposAsync(org, teamSlug, cancellationToken);

        logger.LogInformation(
            "[GitHubApiClient][GetTeamReposAsync] Retornar resposta da API GitHub. RepositoryCount={Count}",
            result.Count);

        return result;
    }
}
