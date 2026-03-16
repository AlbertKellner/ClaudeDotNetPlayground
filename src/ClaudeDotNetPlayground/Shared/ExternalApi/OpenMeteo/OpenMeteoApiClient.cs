namespace ClaudeDotNetPlayground.Shared.ExternalApi.OpenMeteo;

public sealed class OpenMeteoApiClient(
    IOpenMeteoApi api,
    ILogger<OpenMeteoApiClient> logger) : IOpenMeteoApiClient
{
    public async Task<OpenMeteoOutput> GetForecastAsync(
        OpenMeteoInput input,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[OpenMeteoApiClient][GetForecastAsync] Executar requisição HTTP ao Open-Meteo. Latitude={Latitude}, Longitude={Longitude}",
            input.Latitude,
            input.Longitude);

        var result = await api.GetForecastAsync(input.Latitude, input.Longitude, input.Current, cancellationToken);

        logger.LogInformation(
            "[OpenMeteoApiClient][GetForecastAsync] Retornar resposta da API Open-Meteo. Timezone={Timezone}",
            result.Timezone);

        return result;
    }
}
