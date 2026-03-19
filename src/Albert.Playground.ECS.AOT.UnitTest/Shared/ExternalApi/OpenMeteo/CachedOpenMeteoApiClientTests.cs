using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.OpenMeteo;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Shared.ExternalApi.OpenMeteo;

public sealed class CachedOpenMeteoApiClientTests
{
    private sealed class FakeOpenMeteoApi : IOpenMeteoApi
    {
        public int CallCount { get; private set; }

        public Task<OpenMeteoOutput> GetForecastAsync(
            double latitude,
            double longitude,
            string current,
            CancellationToken cancellationToken = default)
        {
            CallCount++;

            var output = new OpenMeteoOutput
            {
                Latitude = latitude,
                Longitude = longitude,
                Timezone = "America/Sao_Paulo"
            };

            return Task.FromResult(output);
        }
    }

    private sealed class FakeHttpContextAccessor(HttpContext httpContext) : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; } = httpContext;
    }

    private static (CachedOpenMeteoApiClient cached, FakeOpenMeteoApi fakeApi, FakeLogger<CachedOpenMeteoApiClient> logger) CreateSut(
        int userId = 1,
        int durationSeconds = 10)
    {
        var fakeApi = new FakeOpenMeteoApi();
        var innerLogger = new FakeLogger<OpenMeteoApiClient>();
        var innerClient = new OpenMeteoApiClient(fakeApi, innerLogger);

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var httpContext = new DefaultHttpContext();
        httpContext.Items[AuthenticateFilter.AuthenticatedUserItemKey] = new AuthenticatedUser(userId, "testuser");
        var httpContextAccessor = new FakeHttpContextAccessor(httpContext);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ExternalApi:OpenMeteo:EndpointCache:WeatherConditionsGet:DurationSeconds"] = durationSeconds.ToString()
            })
            .Build();

        var logger = new FakeLogger<CachedOpenMeteoApiClient>();

        var cached = new CachedOpenMeteoApiClient(innerClient, memoryCache, httpContextAccessor, logger, configuration);
        return (cached, fakeApi, logger);
    }

    private static OpenMeteoInput CreateInput() => new()
    {
        Latitude = -23.5475,
        Longitude = -46.6361,
        Current = "temperature_2m"
    };

    [Fact]
    public async Task GetForecastAsync_CacheMiss_DeveChamarClienteInterno()
    {
        var (cached, fakeApi, _) = CreateSut();
        var input = CreateInput();

        await cached.GetForecastAsync(input);

        Assert.Equal(1, fakeApi.CallCount);
    }

    [Fact]
    public async Task GetForecastAsync_CacheHit_NaoDeveChamarClienteInternoNovamente()
    {
        var (cached, fakeApi, _) = CreateSut();
        var input = CreateInput();

        await cached.GetForecastAsync(input);
        await cached.GetForecastAsync(input);

        Assert.Equal(1, fakeApi.CallCount);
    }

    [Fact]
    public async Task GetForecastAsync_CacheHit_DeveRetornarMesmoResultado()
    {
        var (cached, _, _) = CreateSut();
        var input = CreateInput();

        var first = await cached.GetForecastAsync(input);
        var second = await cached.GetForecastAsync(input);

        Assert.Same(first, second);
    }

    [Fact]
    public async Task GetForecastAsync_UsuariosDiferentes_DevemTerCachesSeparados()
    {
        var fakeApi = new FakeOpenMeteoApi();
        var innerLogger = new FakeLogger<OpenMeteoApiClient>();
        var innerClient = new OpenMeteoApiClient(fakeApi, innerLogger);
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ExternalApi:OpenMeteo:EndpointCache:WeatherConditionsGet:DurationSeconds"] = "10"
            })
            .Build();

        var httpContext1 = new DefaultHttpContext();
        httpContext1.Items[AuthenticateFilter.AuthenticatedUserItemKey] = new AuthenticatedUser(1, "user1");
        var accessor1 = new FakeHttpContextAccessor(httpContext1);
        var logger1 = new FakeLogger<CachedOpenMeteoApiClient>();
        var cached1 = new CachedOpenMeteoApiClient(innerClient, memoryCache, accessor1, logger1, configuration);

        var httpContext2 = new DefaultHttpContext();
        httpContext2.Items[AuthenticateFilter.AuthenticatedUserItemKey] = new AuthenticatedUser(2, "user2");
        var accessor2 = new FakeHttpContextAccessor(httpContext2);
        var logger2 = new FakeLogger<CachedOpenMeteoApiClient>();
        var cached2 = new CachedOpenMeteoApiClient(innerClient, memoryCache, accessor2, logger2, configuration);

        var input = CreateInput();

        await cached1.GetForecastAsync(input);
        await cached2.GetForecastAsync(input);

        Assert.Equal(2, fakeApi.CallCount);
    }

    [Fact]
    public async Task GetForecastAsync_CacheMiss_DeveRegistrarLogDeCacheMiss()
    {
        var (cached, _, logger) = CreateSut();
        var input = CreateInput();

        await cached.GetForecastAsync(input);

        var logs = logger.GetSnapshot();

        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Cache miss"));
    }

    [Fact]
    public async Task GetForecastAsync_CacheHit_DeveRegistrarLogDeCacheHit()
    {
        var (cached, _, logger) = CreateSut();
        var input = CreateInput();

        await cached.GetForecastAsync(input);
        await cached.GetForecastAsync(input);

        var logs = logger.GetSnapshot();

        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar resposta do cache"));
    }

    [Fact]
    public async Task GetForecastAsync_DeveRegistrarLogsComPrefixoCorreto()
    {
        var (cached, _, logger) = CreateSut();
        var input = CreateInput();

        await cached.GetForecastAsync(input);

        var logs = logger.GetSnapshot();

        Assert.All(logs, l => Assert.Contains("CachedOpenMeteoApiClient", l.Message));
    }

    [Fact]
    public async Task GetForecastAsync_DeveRegistrarLogDeArmazenamentoNoCache()
    {
        var (cached, _, logger) = CreateSut();
        var input = CreateInput();

        await cached.GetForecastAsync(input);

        var logs = logger.GetSnapshot();

        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Armazenar resposta no cache"));
    }
}
