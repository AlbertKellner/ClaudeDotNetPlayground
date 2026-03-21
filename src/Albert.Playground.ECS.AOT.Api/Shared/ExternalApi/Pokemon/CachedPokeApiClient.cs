using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Microsoft.Extensions.Caching.Memory;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon;

public sealed class CachedPokeApiClient(
    PokeApiClient innerClient,
    IMemoryCache memoryCache,
    IHttpContextAccessor httpContextAccessor,
    ILogger<CachedPokeApiClient> logger,
    IConfiguration configuration) : IPokeApiClient
{
    private const string CacheKeyPrefix = "Pokemon:PokemonGet";

    public async Task<PokeApiOutput> GetPokemonByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[CachedPokeApiClient][GetPokemonByIdAsync] Verificar cache para Pokémon");

        var userId = GetAuthenticatedUserId();
        var cacheKey = $"{CacheKeyPrefix}:{userId}";

        if (memoryCache.TryGetValue(cacheKey, out PokeApiOutput? cached) && cached is not null)
        {
            logger.LogInformation(
                "[CachedPokeApiClient][GetPokemonByIdAsync] Retornar resposta do cache. CacheKey={CacheKey}",
                cacheKey);

            return cached;
        }

        logger.LogInformation(
            "[CachedPokeApiClient][GetPokemonByIdAsync] Cache miss. Consultar API externa. CacheKey={CacheKey}",
            cacheKey);

        var result = await innerClient.GetPokemonByIdAsync(id, cancellationToken);

        var durationSeconds = configuration.GetValue<int>(
            "ExternalApi:Pokemon:EndpointCache:PokemonGet:DurationSeconds", 60);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(durationSeconds)
        };

        memoryCache.Set(cacheKey, result, cacheOptions);

        logger.LogInformation(
            "[CachedPokeApiClient][GetPokemonByIdAsync] Armazenar resposta no cache. CacheKey={CacheKey}, DurationSeconds={DurationSeconds}",
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
