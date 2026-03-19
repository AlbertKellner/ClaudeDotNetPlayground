using System.Text.Json.Serialization;
using Albert.Playground.ECS.AOT.Api.Shared.Repositories;

namespace Albert.Playground.ECS.AOT.Api.Features.Query.RepositoriesGetAll;

public sealed class RepositoriesGetAllOutput
{
    [JsonPropertyName("repositories")]
    public List<RepositoryFileEntry> Repositories { get; init; } = [];
}
