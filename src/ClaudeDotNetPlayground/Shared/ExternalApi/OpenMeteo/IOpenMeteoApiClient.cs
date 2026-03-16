namespace ClaudeDotNetPlayground.Shared.ExternalApi.OpenMeteo;

public interface IOpenMeteoApiClient
{
    Task<OpenMeteoOutput> GetForecastAsync(OpenMeteoInput input, CancellationToken cancellationToken = default);
}
