using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Microsoft.Extensions.Caching.Memory;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.OpenMeteo;

public sealed class CachedOpenMeteoApiClient(
    OpenMeteoApiClient innerClient,
    IMemoryCache memoryCache,
    IHttpContextAccessor httpContextAccessor,
    ILogger<CachedOpenMeteoApiClient> logger,
    IConfiguration configuration) : IOpenMeteoApiClient
{
    private const string CacheKeyPrefix = "OpenMeteo:WeatherConditionsGet";

    public async Task<OpenMeteoOutput> GetForecastAsync(
        OpenMeteoInput input,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[CachedOpenMeteoApiClient][GetForecastAsync] Verificar cache para condições climáticas");

        var userId = GetAuthenticatedUserId();
        var cacheKey = $"{CacheKeyPrefix}:{userId}";

        if (memoryCache.TryGetValue(cacheKey, out OpenMeteoOutput? cached) && cached is not null)
        {
            logger.LogInformation(
                "[CachedOpenMeteoApiClient][GetForecastAsync] Retornar resposta do cache. CacheKey={CacheKey}",
                cacheKey);

            return cached;
        }

        logger.LogInformation(
            "[CachedOpenMeteoApiClient][GetForecastAsync] Cache miss. Consultar API externa. CacheKey={CacheKey}",
            cacheKey);

        var result = await innerClient.GetForecastAsync(input, cancellationToken);

        var durationSeconds = configuration.GetValue<int>(
            "ExternalApi:OpenMeteo:EndpointCache:WeatherConditionsGet:DurationSeconds", 10);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(durationSeconds)
        };

        memoryCache.Set(cacheKey, result, cacheOptions);

        logger.LogInformation(
            "[CachedOpenMeteoApiClient][GetForecastAsync] Armazenar resposta no cache. CacheKey={CacheKey}, DurationSeconds={DurationSeconds}",
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
