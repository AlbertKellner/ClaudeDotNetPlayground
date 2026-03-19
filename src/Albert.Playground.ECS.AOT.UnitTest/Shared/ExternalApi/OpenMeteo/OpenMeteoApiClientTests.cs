using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.OpenMeteo;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Shared.ExternalApi.OpenMeteo;

public sealed class OpenMeteoApiClientTests
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

    private static OpenMeteoApiClient CreateClient(
        FakeOpenMeteoApi api,
        FakeLogger<OpenMeteoApiClient> logger,
        int userId = 1,
        IMemoryCache? memoryCache = null,
        IConfiguration? configuration = null)
    {
        var cache = memoryCache ?? new MemoryCache(new MemoryCacheOptions());

        var httpContext = new DefaultHttpContext();
        httpContext.Items["AuthenticatedUser"] = new AuthenticatedUser(userId, "test-user");
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };

        var config = configuration ?? new ConfigurationBuilder().Build();

        return new OpenMeteoApiClient(api, logger, cache, httpContextAccessor, config);
    }

    private static OpenMeteoInput CreateDefaultInput() => new()
    {
        Latitude = -23.5475,
        Longitude = -46.6361,
        Current = "temperature_2m"
    };

    [Fact]
    public async Task GetForecastAsync_DeveRegistrarLogInformationNoInicio()
    {
        var fakeApi = new FakeOpenMeteoApi();
        var fakeLogger = new FakeLogger<OpenMeteoApiClient>();
        var client = CreateClient(fakeApi, fakeLogger);

        await client.GetForecastAsync(CreateDefaultInput());

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Executar"));
    }

    [Fact]
    public async Task GetForecastAsync_DeveRegistrarLogInformationNoRetorno()
    {
        var fakeApi = new FakeOpenMeteoApi();
        var fakeLogger = new FakeLogger<OpenMeteoApiClient>();
        var client = CreateClient(fakeApi, fakeLogger);

        await client.GetForecastAsync(CreateDefaultInput());

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar"));
    }

    [Fact]
    public async Task GetForecastAsync_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeApi = new FakeOpenMeteoApi();
        var fakeLogger = new FakeLogger<OpenMeteoApiClient>();
        var client = CreateClient(fakeApi, fakeLogger);

        await client.GetForecastAsync(CreateDefaultInput());

        var logs = fakeLogger.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("OpenMeteoApiClient", l.Message));
    }

    [Fact]
    public async Task GetForecastAsync_DeveRetornarOutputDaApi()
    {
        var fakeApi = new FakeOpenMeteoApi();
        var fakeLogger = new FakeLogger<OpenMeteoApiClient>();
        var client = CreateClient(fakeApi, fakeLogger);

        var result = await client.GetForecastAsync(CreateDefaultInput());

        Assert.Equal(-23.5475, result.Latitude);
        Assert.Equal(-46.6361, result.Longitude);
        Assert.Equal("America/Sao_Paulo", result.Timezone);
    }

    [Fact]
    public async Task GetForecastAsync_SegundaChamadaComMesmoUsuario_DeveRetornarDoCache()
    {
        var fakeApi = new FakeOpenMeteoApi();
        var fakeLogger = new FakeLogger<OpenMeteoApiClient>();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var client = CreateClient(fakeApi, fakeLogger, userId: 1, memoryCache: cache);
        var input = CreateDefaultInput();

        await client.GetForecastAsync(input);
        await client.GetForecastAsync(input);

        Assert.Equal(1, fakeApi.CallCount);
    }

    [Fact]
    public async Task GetForecastAsync_ChamadasComUsuariosDiferentes_DeveChamarApiDuasVezes()
    {
        var fakeApi = new FakeOpenMeteoApi();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var input = CreateDefaultInput();

        var logger1 = new FakeLogger<OpenMeteoApiClient>();
        var client1 = CreateClient(fakeApi, logger1, userId: 1, memoryCache: cache);
        await client1.GetForecastAsync(input);

        var logger2 = new FakeLogger<OpenMeteoApiClient>();
        var client2 = CreateClient(fakeApi, logger2, userId: 2, memoryCache: cache);
        await client2.GetForecastAsync(input);

        Assert.Equal(2, fakeApi.CallCount);
    }

    [Fact]
    public async Task GetForecastAsync_CacheHit_DeveRegistrarLogDeCacheHit()
    {
        var fakeApi = new FakeOpenMeteoApi();
        var fakeLogger = new FakeLogger<OpenMeteoApiClient>();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var client = CreateClient(fakeApi, fakeLogger, userId: 1, memoryCache: cache);
        var input = CreateDefaultInput();

        await client.GetForecastAsync(input);
        await client.GetForecastAsync(input);

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar resposta do cache"));
    }

    [Fact]
    public async Task GetForecastAsync_CacheMiss_DeveRegistrarLogDeCacheMiss()
    {
        var fakeApi = new FakeOpenMeteoApi();
        var fakeLogger = new FakeLogger<OpenMeteoApiClient>();
        var client = CreateClient(fakeApi, fakeLogger);
        var input = CreateDefaultInput();

        await client.GetForecastAsync(input);

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Cache miss"));
    }
}
