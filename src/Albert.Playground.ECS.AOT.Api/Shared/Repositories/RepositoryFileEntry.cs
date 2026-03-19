using System.Text.Json.Serialization;

namespace Albert.Playground.ECS.AOT.Api.Shared.Repositories;

public sealed class RepositoryFileEntry
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("gitUrl")]
    public string GitUrl { get; init; } = string.Empty;

    [JsonPropertyName("lastModifiedDate")]
    public string LastModifiedDate { get; init; } = string.Empty;

    [JsonPropertyName("lastSyncDate")]
    public string LastSyncDate { get; set; } = string.Empty;
}

[JsonSerializable(typeof(List<RepositoryFileEntry>))]
internal sealed partial class RepositoryFileJsonContext : JsonSerializerContext { }
