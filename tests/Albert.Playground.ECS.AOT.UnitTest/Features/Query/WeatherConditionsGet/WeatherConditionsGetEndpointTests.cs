using Albert.Playground.ECS.AOT.Api.Features.Query.WeatherConditionsGet;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.OpenMeteo;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Features.Query.WeatherConditionsGet;

public sealed class WeatherConditionsGetEndpointTests
{
    private sealed class FakeOpenMeteoApiClient : IOpenMeteoApiClient
    {
        public Task<OpenMeteoOutput> GetForecastAsync(
            OpenMeteoInput input,
            CancellationToken cancellationToken = default)
        {
            var output = new OpenMeteoOutput
            {
                Latitude = -23.5475,
                Longitude = -46.6361,
                Timezone = "America/Sao_Paulo"
            };

            return Task.FromResult(output);
        }
    }

    [Fact]
    public async Task Get_DeveRegistrarLogInformationNoInicio()
    {
        var fakeClient = new FakeOpenMeteoApiClient();
        var fakeLoggerUseCase = new FakeLogger<WeatherConditionsGetUseCase>();
        var useCase = new WeatherConditionsGetUseCase(fakeClient, fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<WeatherConditionsGetEndpoint>();
        var endpoint = new WeatherConditionsGetEndpoint(useCase, fakeLoggerEndpoint);

        await endpoint.Get(CancellationToken.None);

        var logs = fakeLoggerEndpoint.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Processar"));
    }

    [Fact]
    public async Task Get_DeveRegistrarLogInformationNoRetorno()
    {
        var fakeClient = new FakeOpenMeteoApiClient();
        var fakeLoggerUseCase = new FakeLogger<WeatherConditionsGetUseCase>();
        var useCase = new WeatherConditionsGetUseCase(fakeClient, fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<WeatherConditionsGetEndpoint>();
        var endpoint = new WeatherConditionsGetEndpoint(useCase, fakeLoggerEndpoint);

        await endpoint.Get(CancellationToken.None);

        var logs = fakeLoggerEndpoint.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar"));
    }

    [Fact]
    public async Task Get_DeveRetornarOkComOutputCompleto()
    {
        var fakeClient = new FakeOpenMeteoApiClient();
        var fakeLoggerUseCase = new FakeLogger<WeatherConditionsGetUseCase>();
        var useCase = new WeatherConditionsGetUseCase(fakeClient, fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<WeatherConditionsGetEndpoint>();
        var endpoint = new WeatherConditionsGetEndpoint(useCase, fakeLoggerEndpoint);

        var actionResult = await endpoint.Get(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var output = Assert.IsType<OpenMeteoOutput>(okResult.Value);
        Assert.Equal("America/Sao_Paulo", output.Timezone);
    }

    [Fact]
    public async Task Get_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeClient = new FakeOpenMeteoApiClient();
        var fakeLoggerUseCase = new FakeLogger<WeatherConditionsGetUseCase>();
        var useCase = new WeatherConditionsGetUseCase(fakeClient, fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<WeatherConditionsGetEndpoint>();
        var endpoint = new WeatherConditionsGetEndpoint(useCase, fakeLoggerEndpoint);

        await endpoint.Get(CancellationToken.None);

        var logs = fakeLoggerEndpoint.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("WeatherConditionsGetEndpoint", l.Message));
    }
}
