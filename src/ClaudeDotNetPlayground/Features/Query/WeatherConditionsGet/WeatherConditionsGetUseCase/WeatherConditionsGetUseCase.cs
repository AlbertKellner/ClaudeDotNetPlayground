using ClaudeDotNetPlayground.Shared.ExternalApi.OpenMeteo;

namespace ClaudeDotNetPlayground.Features.Query.WeatherConditionsGet;

public sealed class WeatherConditionsGetUseCase(
    IOpenMeteoApiClient openMeteoApiClient,
    ILogger<WeatherConditionsGetUseCase> logger)
{
    private const double SaoPauloLatitude = -23.5475;
    private const double SaoPauloLongitude = -46.6361;
    private const string CurrentFields = "temperature_2m,relative_humidity_2m,apparent_temperature,is_day,precipitation,rain,showers,weather_code,cloud_cover,wind_speed_10m,wind_direction_10m";

    public async Task<OpenMeteoOutput> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[WeatherConditionsGetUseCase][ExecuteAsync] Executar caso de uso de condições climáticas de São Paulo");

        var input = new OpenMeteoInput
        {
            Latitude = SaoPauloLatitude,
            Longitude = SaoPauloLongitude,
            Current = CurrentFields
        };

        logger.LogInformation(
            "[WeatherConditionsGetUseCase][ExecuteAsync] Consultar API Open-Meteo. Latitude={Latitude}, Longitude={Longitude}",
            input.Latitude,
            input.Longitude);

        var result = await openMeteoApiClient.GetForecastAsync(input, cancellationToken);

        logger.LogInformation(
            "[WeatherConditionsGetUseCase][ExecuteAsync] Retornar condições climáticas obtidas da Open-Meteo. Timezone={Timezone}",
            result.Timezone);

        return result;
    }
}
