using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Microsoft.Extensions.Caching.Memory;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.OpenMeteo;

public sealed class OpenMeteoApiClient(
    IOpenMeteoApi api,
    ILogger<OpenMeteoApiClient> logger,
    IMemoryCache memoryCache,
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration) : IOpenMeteoApiClient
{
    private const string CacheKeyPrefix = "WeatherConditionsGet";

    public async Task<OpenMeteoOutput> GetForecastAsync(
        OpenMeteoInput input,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[OpenMeteoApiClient][GetForecastAsync] Executar requisição HTTP ao Open-Meteo. Latitude={Latitude}, Longitude={Longitude}",
            input.Latitude,
            input.Longitude);

        var userId = GetAuthenticatedUserId();
        var cacheKey = $"{CacheKeyPrefix}:{userId}";

        logger.LogInformation(
            "[OpenMeteoApiClient][GetForecastAsync] Verificar cache. CacheKey={CacheKey}",
            cacheKey);

        if (memoryCache.TryGetValue(cacheKey, out OpenMeteoOutput? cachedResult) && cachedResult is not null)
        {
            logger.LogInformation(
                "[OpenMeteoApiClient][GetForecastAsync] Retornar resposta do cache. CacheKey={CacheKey}",
                cacheKey);

            return cachedResult;
        }

        logger.LogInformation(
            "[OpenMeteoApiClient][GetForecastAsync] Cache miss. Consultar API Open-Meteo. CacheKey={CacheKey}",
            cacheKey);

        var result = await api.GetForecastAsync(input.Latitude, input.Longitude, input.Current, cancellationToken);

        var durationSeconds = configuration.GetValue<int>(
            "ExternalApi:OpenMeteo:EndpointCache:WeatherConditionsGet:DurationSeconds", 10);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(durationSeconds)
        };

        memoryCache.Set(cacheKey, result, cacheOptions);

        logger.LogInformation(
            "[OpenMeteoApiClient][GetForecastAsync] Retornar resposta da API Open-Meteo e armazenar em cache. Timezone={Timezone}, DurationSeconds={DurationSeconds}",
            result.Timezone,
            durationSeconds);

        return result;
    }

    private int GetAuthenticatedUserId()
    {
        var user = httpContextAccessor.HttpContext?.Items["AuthenticatedUser"] as AuthenticatedUser;
        return user?.Id ?? 0;
    }
}
