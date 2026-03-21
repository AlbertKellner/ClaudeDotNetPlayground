using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Shared.ExternalApi.Pokemon;

public sealed class CachedPokeApiClientTests
{
    private sealed class FakePokeApi : IPokeApi
    {
        public int CallCount { get; private set; }

        public Task<PokeApiOutput> GetPokemonByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            CallCount++;

            var result = new PokeApiOutput
            {
                Id = id,
                Name = "pikachu",
                Height = 4,
                Weight = 60,
                Types =
                [
                    new PokeApiTypeSlot
                    {
                        Slot = 1,
                        Type = new PokeApiTypeInfo { Name = "electric", Url = "https://pokeapi.co/api/v2/type/13/" }
                    }
                ],
                Sprites = new PokeApiSprites
                {
                    FrontDefault = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/25.png"
                }
            };

            return Task.FromResult(result);
        }
    }

    private sealed class FakeHttpContextAccessor(HttpContext httpContext) : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; } = httpContext;
    }

    private static (CachedPokeApiClient cached, FakePokeApi fakeApi, FakeLogger<CachedPokeApiClient> logger) CreateSut(
        int userId = 1,
        int durationSeconds = 60)
    {
        var fakeApi = new FakePokeApi();
        var innerLogger = new FakeLogger<PokeApiClient>();
        var innerClient = new PokeApiClient(fakeApi, innerLogger);

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var httpContext = new DefaultHttpContext();
        httpContext.Items[AuthenticateFilter.AuthenticatedUserItemKey] = new AuthenticatedUser(userId, "testuser");
        var httpContextAccessor = new FakeHttpContextAccessor(httpContext);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ExternalApi:Pokemon:EndpointCache:PokemonGet:DurationSeconds"] = durationSeconds.ToString()
            })
            .Build();

        var logger = new FakeLogger<CachedPokeApiClient>();

        var cached = new CachedPokeApiClient(innerClient, memoryCache, httpContextAccessor, logger, configuration);
        return (cached, fakeApi, logger);
    }

    [Fact]
    public async Task GetPokemonByIdAsync_CacheMiss_DeveChamarClienteInterno()
    {
        var (cached, fakeApi, _) = CreateSut();

        await cached.GetPokemonByIdAsync(25);

        Assert.Equal(1, fakeApi.CallCount);
    }

    [Fact]
    public async Task GetPokemonByIdAsync_CacheHit_NaoDeveChamarClienteInternoNovamente()
    {
        var (cached, fakeApi, _) = CreateSut();

        await cached.GetPokemonByIdAsync(25);
        await cached.GetPokemonByIdAsync(25);

        Assert.Equal(1, fakeApi.CallCount);
    }

    [Fact]
    public async Task GetPokemonByIdAsync_CacheHit_DeveRetornarMesmoResultado()
    {
        var (cached, _, _) = CreateSut();

        var first = await cached.GetPokemonByIdAsync(25);
        var second = await cached.GetPokemonByIdAsync(25);

        Assert.Same(first, second);
    }

    [Fact]
    public async Task GetPokemonByIdAsync_UsuariosDiferentes_DevemTerCachesSeparados()
    {
        var fakeApi = new FakePokeApi();
        var innerLogger = new FakeLogger<PokeApiClient>();
        var innerClient = new PokeApiClient(fakeApi, innerLogger);
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ExternalApi:Pokemon:EndpointCache:PokemonGet:DurationSeconds"] = "60"
            })
            .Build();

        var httpContext1 = new DefaultHttpContext();
        httpContext1.Items[AuthenticateFilter.AuthenticatedUserItemKey] = new AuthenticatedUser(1, "user1");
        var accessor1 = new FakeHttpContextAccessor(httpContext1);
        var logger1 = new FakeLogger<CachedPokeApiClient>();
        var cached1 = new CachedPokeApiClient(innerClient, memoryCache, accessor1, logger1, configuration);

        var httpContext2 = new DefaultHttpContext();
        httpContext2.Items[AuthenticateFilter.AuthenticatedUserItemKey] = new AuthenticatedUser(2, "user2");
        var accessor2 = new FakeHttpContextAccessor(httpContext2);
        var logger2 = new FakeLogger<CachedPokeApiClient>();
        var cached2 = new CachedPokeApiClient(innerClient, memoryCache, accessor2, logger2, configuration);

        await cached1.GetPokemonByIdAsync(25);
        await cached2.GetPokemonByIdAsync(25);

        Assert.Equal(2, fakeApi.CallCount);
    }

    [Fact]
    public async Task GetPokemonByIdAsync_CacheMiss_DeveRegistrarLogDeCacheMiss()
    {
        var (cached, _, logger) = CreateSut();

        await cached.GetPokemonByIdAsync(25);

        var logs = logger.GetSnapshot();

        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Cache miss"));
    }

    [Fact]
    public async Task GetPokemonByIdAsync_CacheHit_DeveRegistrarLogDeCacheHit()
    {
        var (cached, _, logger) = CreateSut();

        await cached.GetPokemonByIdAsync(25);
        await cached.GetPokemonByIdAsync(25);

        var logs = logger.GetSnapshot();

        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar resposta do cache"));
    }

    [Fact]
    public async Task GetPokemonByIdAsync_DeveRegistrarLogsComPrefixoCorreto()
    {
        var (cached, _, logger) = CreateSut();

        await cached.GetPokemonByIdAsync(25);

        var logs = logger.GetSnapshot();

        Assert.All(logs, l => Assert.Contains("CachedPokeApiClient", l.Message));
    }

    [Fact]
    public async Task GetPokemonByIdAsync_DeveRegistrarLogDeArmazenamentoNoCache()
    {
        var (cached, _, logger) = CreateSut();

        await cached.GetPokemonByIdAsync(25);

        var logs = logger.GetSnapshot();

        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Armazenar resposta no cache"));
    }
}
