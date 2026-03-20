using System.Text.Json.Serialization;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;

public sealed class GitHubRepositoryOutput
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("full_name")]
    public string FullName { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; init; } = string.Empty;

    [JsonPropertyName("git_url")]
    public string GitUrl { get; init; } = string.Empty;

    [JsonPropertyName("clone_url")]
    public string CloneUrl { get; init; } = string.Empty;
}

[JsonSerializable(typeof(List<GitHubRepositoryOutput>))]
internal sealed partial class GitHubJsonContext : JsonSerializerContext { }
