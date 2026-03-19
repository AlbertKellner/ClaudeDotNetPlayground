using Refit;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;

public interface IGitHubApi
{
    [Get("/orgs/{org}/teams/{teamSlug}/repos")]
    Task<List<GitHubRepositoryOutput>> GetTeamReposAsync(
        string org,
        string teamSlug,
        CancellationToken cancellationToken = default);
}
