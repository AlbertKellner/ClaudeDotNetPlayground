using System.Text.Json.Serialization;

namespace Albert.Playground.ECS.AOT.Api.Infra.Logging;

internal sealed class DatadogLogEntry
{
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; init; }

    [JsonPropertyName("level")]
    public string Level { get; init; } = string.Empty;

    [JsonPropertyName("service")]
    public string Service { get; init; } = string.Empty;

    [JsonPropertyName("host")]
    public string Host { get; init; } = string.Empty;

    [JsonPropertyName("ddtags")]
    public string DdTags { get; init; } = string.Empty;
}

[JsonSerializable(typeof(DatadogLogEntry))]
[JsonSerializable(typeof(List<DatadogLogEntry>))]
internal sealed partial class DatadogLogJsonContext : JsonSerializerContext { }
