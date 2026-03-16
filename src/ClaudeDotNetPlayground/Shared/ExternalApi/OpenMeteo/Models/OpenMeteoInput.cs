namespace ClaudeDotNetPlayground.Shared.ExternalApi.OpenMeteo;

public sealed class OpenMeteoInput
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string Current { get; init; } = string.Empty;
}
