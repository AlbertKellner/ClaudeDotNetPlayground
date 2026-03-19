using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.OpenMeteo;

namespace Albert.Playground.ECS.AOT.Api.Features.Query.WeatherConditionsGet;

public sealed class WeatherConditionsGetUseCase(
    IOpenMeteoApiClient openMeteoApiClient,
    ILogger<WeatherConditionsGetUseCase> logger)
{
    private const double SaoPauloLatitude = -23.5475;
    private const double SaoPauloLongitude = -46.6361;
    private const string CurrentFields = "temperature_2m,relative_humidity_2m,apparent_temperature,is_day,precipitation,rain,showers,weather_code,cloud_cover,wind_speed_10m,wind_direction_10m";

    public async Task<WeatherConditionsGetOutput> ExecuteAsync(CancellationToken cancellationToken = default)
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
            "[WeatherConditionsGetUseCase][ExecuteAsync] Mapear resposta da Open-Meteo para model da Feature");

        var output = new WeatherConditionsGetOutput
        {
            Latitude = result.Latitude,
            Longitude = result.Longitude,
            GenerationtimeMs = result.GenerationtimeMs,
            UtcOffsetSeconds = result.UtcOffsetSeconds,
            Timezone = result.Timezone,
            TimezoneAbbreviation = result.TimezoneAbbreviation,
            Elevation = result.Elevation,
            CurrentUnits = new WeatherConditionsGetCurrentUnits
            {
                Time = result.CurrentUnits.Time,
                Interval = result.CurrentUnits.Interval,
                Temperature2m = result.CurrentUnits.Temperature2m,
                RelativeHumidity2m = result.CurrentUnits.RelativeHumidity2m,
                ApparentTemperature = result.CurrentUnits.ApparentTemperature,
                IsDay = result.CurrentUnits.IsDay,
                Precipitation = result.CurrentUnits.Precipitation,
                Rain = result.CurrentUnits.Rain,
                Showers = result.CurrentUnits.Showers,
                WeatherCode = result.CurrentUnits.WeatherCode,
                CloudCover = result.CurrentUnits.CloudCover,
                WindSpeed10m = result.CurrentUnits.WindSpeed10m,
                WindDirection10m = result.CurrentUnits.WindDirection10m
            },
            Current = new WeatherConditionsGetCurrent
            {
                Time = result.Current.Time,
                Interval = result.Current.Interval,
                Temperature2m = result.Current.Temperature2m,
                RelativeHumidity2m = result.Current.RelativeHumidity2m,
                ApparentTemperature = result.Current.ApparentTemperature,
                IsDay = result.Current.IsDay,
                Precipitation = result.Current.Precipitation,
                Rain = result.Current.Rain,
                Showers = result.Current.Showers,
                WeatherCode = result.Current.WeatherCode,
                CloudCover = result.Current.CloudCover,
                WindSpeed10m = result.Current.WindSpeed10m,
                WindDirection10m = result.Current.WindDirection10m
            }
        };

        logger.LogInformation(
            "[WeatherConditionsGetUseCase][ExecuteAsync] Retornar condições climáticas obtidas da Open-Meteo. Timezone={Timezone}",
            output.Timezone);

        return output;
    }
}
