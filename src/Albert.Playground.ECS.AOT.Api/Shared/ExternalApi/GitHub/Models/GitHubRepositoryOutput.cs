using System.Text.Json.Serialization;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;

public sealed class GitHubRepositoryOutput
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("clone_url")]
    public string CloneUrl { get; init; } = string.Empty;

    [JsonPropertyName("updated_at")]
    public string UpdatedAt { get; init; } = string.Empty;
}

[JsonSerializable(typeof(List<GitHubRepositoryOutput>))]
internal sealed partial class GitHubJsonContext : JsonSerializerContext { }
