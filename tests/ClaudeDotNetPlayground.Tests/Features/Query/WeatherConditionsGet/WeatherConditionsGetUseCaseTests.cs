using ClaudeDotNetPlayground.Features.Query.WeatherConditionsGet;
using ClaudeDotNetPlayground.Shared.ExternalApi.OpenMeteo;
using ClaudeDotNetPlayground.Tests.TestHelpers;
using Microsoft.Extensions.Logging;

namespace ClaudeDotNetPlayground.Tests.Features.Query.WeatherConditionsGet;

public sealed class WeatherConditionsGetUseCaseTests
{
    private sealed class FakeOpenMeteoApiClient : IOpenMeteoApiClient
    {
        public Task<OpenMeteoOutput> GetForecastAsync(
            OpenMeteoInput input,
            CancellationToken cancellationToken = default)
        {
            var output = new OpenMeteoOutput
            {
                Latitude = input.Latitude,
                Longitude = input.Longitude,
                Timezone = "America/Sao_Paulo"
            };

            return Task.FromResult(output);
        }
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogInformationNoInicio()
    {
        var fakeClient = new FakeOpenMeteoApiClient();
        var fakeLogger = new FakeLogger<WeatherConditionsGetUseCase>();
        var useCase = new WeatherConditionsGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Executar"));
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogInformationAoConsultarApi()
    {
        var fakeClient = new FakeOpenMeteoApiClient();
        var fakeLogger = new FakeLogger<WeatherConditionsGetUseCase>();
        var useCase = new WeatherConditionsGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Consultar"));
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogInformationNoRetorno()
    {
        var fakeClient = new FakeOpenMeteoApiClient();
        var fakeLogger = new FakeLogger<WeatherConditionsGetUseCase>();
        var useCase = new WeatherConditionsGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar"));
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeClient = new FakeOpenMeteoApiClient();
        var fakeLogger = new FakeLogger<WeatherConditionsGetUseCase>();
        var useCase = new WeatherConditionsGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("WeatherConditionsGetUseCase", l.Message));
    }

    [Fact]
    public async Task ExecuteAsync_DeveRetornarOutputCompletoSemMapeamentoParcial()
    {
        var fakeClient = new FakeOpenMeteoApiClient();
        var fakeLogger = new FakeLogger<WeatherConditionsGetUseCase>();
        var useCase = new WeatherConditionsGetUseCase(fakeClient, fakeLogger);

        var result = await useCase.ExecuteAsync();

        Assert.IsType<OpenMeteoOutput>(result);
        Assert.Equal("America/Sao_Paulo", result.Timezone);
    }
}
