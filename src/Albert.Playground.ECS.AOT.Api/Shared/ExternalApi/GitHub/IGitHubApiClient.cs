namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;

public interface IGitHubApiClient
{
    Task<List<GitHubRepositoryOutput>> GetTeamReposAsync(
        string org,
        string teamSlug,
        CancellationToken cancellationToken = default);
}
