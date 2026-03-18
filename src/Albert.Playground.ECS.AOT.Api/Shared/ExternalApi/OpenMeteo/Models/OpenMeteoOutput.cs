using System.Text.Json.Serialization;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.OpenMeteo;

public sealed class OpenMeteoOutput
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; init; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; init; }

    [JsonPropertyName("generationtime_ms")]
    public double GenerationtimeMs { get; init; }

    [JsonPropertyName("utc_offset_seconds")]
    public int UtcOffsetSeconds { get; init; }

    [JsonPropertyName("timezone")]
    public string Timezone { get; init; } = string.Empty;

    [JsonPropertyName("timezone_abbreviation")]
    public string TimezoneAbbreviation { get; init; } = string.Empty;

    [JsonPropertyName("elevation")]
    public double Elevation { get; init; }

    [JsonPropertyName("current_units")]
    public OpenMeteoCurrentUnits CurrentUnits { get; init; } = new();

    [JsonPropertyName("current")]
    public OpenMeteoCurrent Current { get; init; } = new();
}

public sealed class OpenMeteoCurrentUnits
{
    [JsonPropertyName("time")]
    public string Time { get; init; } = string.Empty;

    [JsonPropertyName("interval")]
    public string Interval { get; init; } = string.Empty;

    [JsonPropertyName("temperature_2m")]
    public string Temperature2m { get; init; } = string.Empty;

    [JsonPropertyName("relative_humidity_2m")]
    public string RelativeHumidity2m { get; init; } = string.Empty;

    [JsonPropertyName("apparent_temperature")]
    public string ApparentTemperature { get; init; } = string.Empty;

    [JsonPropertyName("is_day")]
    public string IsDay { get; init; } = string.Empty;

    [JsonPropertyName("precipitation")]
    public string Precipitation { get; init; } = string.Empty;

    [JsonPropertyName("rain")]
    public string Rain { get; init; } = string.Empty;

    [JsonPropertyName("showers")]
    public string Showers { get; init; } = string.Empty;

    [JsonPropertyName("weather_code")]
    public string WeatherCode { get; init; } = string.Empty;

    [JsonPropertyName("cloud_cover")]
    public string CloudCover { get; init; } = string.Empty;

    [JsonPropertyName("wind_speed_10m")]
    public string WindSpeed10m { get; init; } = string.Empty;

    [JsonPropertyName("wind_direction_10m")]
    public string WindDirection10m { get; init; } = string.Empty;
}

public sealed class OpenMeteoCurrent
{
    [JsonPropertyName("time")]
    public string Time { get; init; } = string.Empty;

    [JsonPropertyName("interval")]
    public int Interval { get; init; }

    [JsonPropertyName("temperature_2m")]
    public double Temperature2m { get; init; }

    [JsonPropertyName("relative_humidity_2m")]
    public int RelativeHumidity2m { get; init; }

    [JsonPropertyName("apparent_temperature")]
    public double ApparentTemperature { get; init; }

    [JsonPropertyName("is_day")]
    public int IsDay { get; init; }

    [JsonPropertyName("precipitation")]
    public double Precipitation { get; init; }

    [JsonPropertyName("rain")]
    public double Rain { get; init; }

    [JsonPropertyName("showers")]
    public double Showers { get; init; }

    [JsonPropertyName("weather_code")]
    public int WeatherCode { get; init; }

    [JsonPropertyName("cloud_cover")]
    public int CloudCover { get; init; }

    [JsonPropertyName("wind_speed_10m")]
    public double WindSpeed10m { get; init; }

    [JsonPropertyName("wind_direction_10m")]
    public int WindDirection10m { get; init; }
}

[JsonSerializable(typeof(OpenMeteoOutput))]
internal sealed partial class OpenMeteoJsonContext : JsonSerializerContext { }
