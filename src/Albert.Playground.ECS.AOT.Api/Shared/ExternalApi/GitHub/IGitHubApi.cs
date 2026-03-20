using Refit;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;

public interface IGitHubApi
{
    [Get("/users/{username}/repos")]
    Task<List<GitHubRepositoryOutput>> GetRepositoriesAsync(
        [AliasAs("username")] string username,
        [Query] int per_page = 100,
        [Query] int page = 1,
        CancellationToken cancellationToken = default);
}
