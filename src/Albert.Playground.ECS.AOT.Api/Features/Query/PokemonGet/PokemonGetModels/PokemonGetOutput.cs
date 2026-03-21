using System.Text.Json.Serialization;

namespace Albert.Playground.ECS.AOT.Api.Features.Query.PokemonGet;

public sealed class PokemonGetOutput
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("height")]
    public int Height { get; init; }

    [JsonPropertyName("weight")]
    public int Weight { get; init; }

    [JsonPropertyName("types")]
    public List<PokemonGetType> Types { get; init; } = [];

    [JsonPropertyName("spriteUrl")]
    public string? SpriteUrl { get; init; }
}

public sealed class PokemonGetType
{
    [JsonPropertyName("slot")]
    public int Slot { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}
