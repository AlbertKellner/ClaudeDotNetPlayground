using System.Text.Json.Serialization;

namespace Albert.Playground.ECS.AOT.Api.Features.Command.RepositoriesSyncAll;

public sealed class RepositoriesSyncAllOutput
{
    [JsonPropertyName("totalRepositories")]
    public int TotalRepositories { get; init; }

    [JsonPropertyName("successCount")]
    public int SuccessCount { get; init; }

    [JsonPropertyName("errorCount")]
    public int ErrorCount { get; init; }

    [JsonPropertyName("results")]
    public List<RepositorySyncResult> Results { get; init; } = [];
}

public sealed class RepositorySyncResult
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; init; }
}
