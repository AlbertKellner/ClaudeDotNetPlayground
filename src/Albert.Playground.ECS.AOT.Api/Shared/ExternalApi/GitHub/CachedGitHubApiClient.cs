using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Microsoft.Extensions.Caching.Memory;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;

public sealed class CachedGitHubApiClient(
    GitHubApiClient innerClient,
    IMemoryCache memoryCache,
    IHttpContextAccessor httpContextAccessor,
    ILogger<CachedGitHubApiClient> logger,
    IConfiguration configuration) : IGitHubApiClient
{
    private const string CacheKeyPrefix = "GitHub:GitHubRepoSearch";

    public async Task<List<GitHubRepositoryOutput>> GetRepositoriesAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[CachedGitHubApiClient][GetRepositoriesAsync] Verificar cache para repositórios GitHub");

        var userId = GetAuthenticatedUserId();
        var cacheKey = $"{CacheKeyPrefix}:{userId}";

        if (memoryCache.TryGetValue(cacheKey, out List<GitHubRepositoryOutput>? cached) && cached is not null)
        {
            logger.LogInformation(
                "[CachedGitHubApiClient][GetRepositoriesAsync] Retornar resposta do cache. CacheKey={CacheKey}",
                cacheKey);

            return cached;
        }

        logger.LogInformation(
            "[CachedGitHubApiClient][GetRepositoriesAsync] Cache miss. Consultar API externa. CacheKey={CacheKey}",
            cacheKey);

        var result = await innerClient.GetRepositoriesAsync(username, cancellationToken);

        var durationSeconds = configuration.GetValue<int>(
            "ExternalApi:GitHub:EndpointCache:GitHubRepoSearch:DurationSeconds", 60);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(durationSeconds)
        };

        memoryCache.Set(cacheKey, result, cacheOptions);

        logger.LogInformation(
            "[CachedGitHubApiClient][GetRepositoriesAsync] Armazenar resposta no cache. CacheKey={CacheKey}, DurationSeconds={DurationSeconds}",
            cacheKey,
            durationSeconds);

        return result;
    }

    private int GetAuthenticatedUserId()
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext?.Items.TryGetValue(AuthenticateFilter.AuthenticatedUserItemKey, out var userObj) == true
            && userObj is AuthenticatedUser user)
        {
            return user.Id;
        }

        return 0;
    }
}
