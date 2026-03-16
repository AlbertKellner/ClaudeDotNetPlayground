using ClaudeDotNetPlayground.Shared.ExternalApi.OpenMeteo;
using ClaudeDotNetPlayground.Tests.TestHelpers;
using Microsoft.Extensions.Logging;

namespace ClaudeDotNetPlayground.Tests.Shared.ExternalApi.OpenMeteo;

public sealed class OpenMeteoApiClientTests
{
    private sealed class FakeOpenMeteoApi : IOpenMeteoApi
    {
        public Task<OpenMeteoOutput> GetForecastAsync(
            double latitude,
            double longitude,
            string current,
            CancellationToken cancellationToken = default)
        {
            var output = new OpenMeteoOutput
            {
                Latitude = latitude,
                Longitude = longitude,
                Timezone = "America/Sao_Paulo"
            };

            return Task.FromResult(output);
        }
    }

    [Fact]
    public async Task GetForecastAsync_DeveRegistrarLogInformationNoInicio()
    {
        var fakeApi = new FakeOpenMeteoApi();
        var fakeLogger = new FakeLogger<OpenMeteoApiClient>();
        var client = new OpenMeteoApiClient(fakeApi, fakeLogger);
        var input = new OpenMeteoInput { Latitude = -23.5475, Longitude = -46.6361, Current = "temperature_2m" };

        await client.GetForecastAsync(input);

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
        var client = new OpenMeteoApiClient(fakeApi, fakeLogger);
        var input = new OpenMeteoInput { Latitude = -23.5475, Longitude = -46.6361, Current = "temperature_2m" };

        await client.GetForecastAsync(input);

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
        var client = new OpenMeteoApiClient(fakeApi, fakeLogger);
        var input = new OpenMeteoInput { Latitude = -23.5475, Longitude = -46.6361, Current = "temperature_2m" };

        await client.GetForecastAsync(input);

        var logs = fakeLogger.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("OpenMeteoApiClient", l.Message));
    }

    [Fact]
    public async Task GetForecastAsync_DeveRetornarOutputDaApi()
    {
        var fakeApi = new FakeOpenMeteoApi();
        var fakeLogger = new FakeLogger<OpenMeteoApiClient>();
        var client = new OpenMeteoApiClient(fakeApi, fakeLogger);
        var input = new OpenMeteoInput { Latitude = -23.5475, Longitude = -46.6361, Current = "temperature_2m" };

        var result = await client.GetForecastAsync(input);

        Assert.Equal(-23.5475, result.Latitude);
        Assert.Equal(-46.6361, result.Longitude);
        Assert.Equal("America/Sao_Paulo", result.Timezone);
    }
}
