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

    [JsonPropertyName("base_experience")]
    public int BaseExperience { get; init; }

    [JsonPropertyName("types")]
    public List<PokemonGetType> Types { get; init; } = [];

    [JsonPropertyName("abilities")]
    public List<PokemonGetAbility> Abilities { get; init; } = [];

    [JsonPropertyName("stats")]
    public List<PokemonGetStat> Stats { get; init; } = [];
}

public sealed class PokemonGetType
{
    [JsonPropertyName("slot")]
    public int Slot { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}

public sealed class PokemonGetAbility
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("is_hidden")]
    public bool IsHidden { get; init; }

    [JsonPropertyName("slot")]
    public int Slot { get; init; }
}

public sealed class PokemonGetStat
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("base_stat")]
    public int BaseStat { get; init; }

    [JsonPropertyName("effort")]
    public int Effort { get; init; }
}
