using System.Text.Json.Serialization;

namespace Albert.Playground.ECS.AOT.Api.Features.Query.GitHubRepoSearch;

public sealed class GitHubRepoSearchOutput
{
    [JsonPropertyName("repositories")]
    public List<GitHubRepoSearchItem> Repositories { get; init; } = [];
}

public sealed class GitHubRepoSearchItem
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("gitUrl")]
    public string GitUrl { get; init; } = string.Empty;
}
