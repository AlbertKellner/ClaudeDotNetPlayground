using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon;

public sealed class CachedPokemonApiClient(
    PokemonApiClient innerClient,
    IMemoryCache memoryCache,
    IHttpContextAccessor httpContextAccessor,
    ILogger<CachedPokemonApiClient> logger,
    IConfiguration configuration) : IPokemonApiClient
{
    private const string CacheKeyPrefix = "Pokemon:PokemonGet";

    public async Task<PokemonOutput> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[CachedPokemonApiClient][GetByIdAsync] Verificar cache para Pokemon. PokemonId={PokemonId}", id);

        var userId = GetAuthenticatedUserId();
        var cacheKey = $"{CacheKeyPrefix}:{userId}:{id}";

        if (memoryCache.TryGetValue(cacheKey, out PokemonOutput? cached) && cached is not null)
        {
            logger.LogInformation(
                "[CachedPokemonApiClient][GetByIdAsync] Retornar resposta do cache. CacheKey={CacheKey}",
                cacheKey);

            return cached;
        }

        logger.LogInformation(
            "[CachedPokemonApiClient][GetByIdAsync] Cache miss. Consultar API externa. CacheKey={CacheKey}",
            cacheKey);

        var result = await innerClient.GetByIdAsync(id, cancellationToken);

        var durationSeconds = configuration.GetValue<int>(
            "ExternalApi:Pokemon:EndpointCache:PokemonGet:DurationSeconds", 60);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(durationSeconds)
        };

        memoryCache.Set(cacheKey, result, cacheOptions);

        logger.LogInformation(
            "[CachedPokemonApiClient][GetByIdAsync] Armazenar resposta no cache. CacheKey={CacheKey}, DurationSeconds={DurationSeconds}",
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
