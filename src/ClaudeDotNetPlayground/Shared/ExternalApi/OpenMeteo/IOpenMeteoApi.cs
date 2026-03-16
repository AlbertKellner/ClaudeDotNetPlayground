using Refit;

namespace ClaudeDotNetPlayground.Shared.ExternalApi.OpenMeteo;

public interface IOpenMeteoApi
{
    [Get("/v1/forecast")]
    Task<OpenMeteoOutput> GetForecastAsync(
        [Query] double latitude,
        [Query] double longitude,
        [Query] string current,
        CancellationToken cancellationToken = default);
}
