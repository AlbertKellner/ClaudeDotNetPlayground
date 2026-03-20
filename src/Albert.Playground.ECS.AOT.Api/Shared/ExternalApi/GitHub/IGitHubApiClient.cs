namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;

public interface IGitHubApiClient
{
    Task<List<GitHubRepositoryOutput>> GetRepositoriesAsync(string username, CancellationToken cancellationToken = default);
}
