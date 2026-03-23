using Albert.Playground.ECS.AOT.Api.Features.Query.WeatherConditionsGet;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.OpenMeteo;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Features.Query.WeatherConditionsGet;

public sealed class WeatherConditionsGetUseCaseTests
{
    private sealed class FakeOpenMeteoApiClient : IOpenMeteoApiClient
    {
        public OpenMeteoInput? LastInput { get; private set; }

        public Task<OpenMeteoOutput> GetForecastAsync(
            OpenMeteoInput input,
            CancellationToken cancellationToken = default)
        {
            LastInput = input;

            var output = new OpenMeteoOutput
            {
                Latitude = input.Latitude,
                Longitude = input.Longitude,
                Timezone = "America/Sao_Paulo"
            };

            return Task.FromResult(output);
        }
    }

    private static WeatherConditionsGetInput CreateDefaultInput() => new()
    {
        Latitude = -23.5475,
        Longitude = -46.6361
    };

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogInformationNoInicio()
    {
        var fakeClient = new FakeOpenMeteoApiClient();
        var fakeLogger = new FakeLogger<WeatherConditionsGetUseCase>();
        var useCase = new WeatherConditionsGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync(CreateDefaultInput());

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

        await useCase.ExecuteAsync(CreateDefaultInput());

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Consultar"));
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogInformationAoMapear()
    {
        var fakeClient = new FakeOpenMeteoApiClient();
        var fakeLogger = new FakeLogger<WeatherConditionsGetUseCase>();
        var useCase = new WeatherConditionsGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync(CreateDefaultInput());

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Mapear"));
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogInformationNoRetorno()
    {
        var fakeClient = new FakeOpenMeteoApiClient();
        var fakeLogger = new FakeLogger<WeatherConditionsGetUseCase>();
        var useCase = new WeatherConditionsGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync(CreateDefaultInput());

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

        await useCase.ExecuteAsync(CreateDefaultInput());

        var logs = fakeLogger.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("WeatherConditionsGetUseCase", l.Message));
    }

    [Fact]
    public async Task ExecuteAsync_DeveRetornarOutputCompletoSemMapeamentoParcial()
    {
        var fakeClient = new FakeOpenMeteoApiClient();
        var fakeLogger = new FakeLogger<WeatherConditionsGetUseCase>();
        var useCase = new WeatherConditionsGetUseCase(fakeClient, fakeLogger);
        var input = CreateDefaultInput();

        var result = await useCase.ExecuteAsync(input);

        Assert.IsType<WeatherConditionsGetOutput>(result);
        Assert.Equal("America/Sao_Paulo", result.Timezone);
        Assert.Equal(input.Latitude, result.Latitude);
        Assert.Equal(input.Longitude, result.Longitude);
    }

    [Fact]
    public async Task ExecuteAsync_DevePassarCoordenadasDoInputParaApi()
    {
        var fakeClient = new FakeOpenMeteoApiClient();
        var fakeLogger = new FakeLogger<WeatherConditionsGetUseCase>();
        var useCase = new WeatherConditionsGetUseCase(fakeClient, fakeLogger);
        var input = new WeatherConditionsGetInput
        {
            Latitude = 40.7128,
            Longitude = -74.0060
        };

        await useCase.ExecuteAsync(input);

        Assert.NotNull(fakeClient.LastInput);
        Assert.Equal(40.7128, fakeClient.LastInput.Latitude);
        Assert.Equal(-74.0060, fakeClient.LastInput.Longitude);
    }

    [Fact]
    public async Task ExecuteAsync_DevePassarCamposDeCondicaoAtualParaApi()
    {
        var fakeClient = new FakeOpenMeteoApiClient();
        var fakeLogger = new FakeLogger<WeatherConditionsGetUseCase>();
        var useCase = new WeatherConditionsGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync(CreateDefaultInput());

        Assert.NotNull(fakeClient.LastInput);
        Assert.Contains("temperature_2m", fakeClient.LastInput.Current);
        Assert.Contains("weather_code", fakeClient.LastInput.Current);
        Assert.Contains("wind_speed_10m", fakeClient.LastInput.Current);
    }
}
