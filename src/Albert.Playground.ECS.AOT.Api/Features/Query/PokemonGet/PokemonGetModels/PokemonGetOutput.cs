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

    [JsonPropertyName("front_default_sprite")]
    public string? FrontDefaultSprite { get; init; }

    [JsonPropertyName("front_shiny_sprite")]
    public string? FrontShinySprite { get; init; }

    [JsonPropertyName("types")]
    public List<PokemonGetTypeItem> Types { get; init; } = [];

    [JsonPropertyName("abilities")]
    public List<PokemonGetAbilityItem> Abilities { get; init; } = [];

    [JsonPropertyName("stats")]
    public List<PokemonGetStatItem> Stats { get; init; } = [];
}

public sealed class PokemonGetTypeItem
{
    [JsonPropertyName("slot")]
    public int Slot { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}

public sealed class PokemonGetAbilityItem
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("is_hidden")]
    public bool IsHidden { get; init; }
}

public sealed class PokemonGetStatItem
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("base_stat")]
    public int BaseStat { get; init; }

    [JsonPropertyName("effort")]
    public int Effort { get; init; }
}
