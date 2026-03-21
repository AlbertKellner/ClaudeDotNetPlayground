using System.Text.Json.Serialization;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon.Models;

public sealed class PokemonOutput
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
    public List<PokemonTypeSlot> Types { get; init; } = [];

    [JsonPropertyName("abilities")]
    public List<PokemonAbilitySlot> Abilities { get; init; } = [];

    [JsonPropertyName("stats")]
    public List<PokemonStatSlot> Stats { get; init; } = [];

    [JsonPropertyName("sprites")]
    public PokemonSprites Sprites { get; init; } = new();
}

public sealed class PokemonTypeSlot
{
    [JsonPropertyName("slot")]
    public int Slot { get; init; }

    [JsonPropertyName("type")]
    public PokemonNamedResource Type { get; init; } = new();
}

public sealed class PokemonAbilitySlot
{
    [JsonPropertyName("is_hidden")]
    public bool IsHidden { get; init; }

    [JsonPropertyName("slot")]
    public int Slot { get; init; }

    [JsonPropertyName("ability")]
    public PokemonNamedResource Ability { get; init; } = new();
}

public sealed class PokemonStatSlot
{
    [JsonPropertyName("base_stat")]
    public int BaseStat { get; init; }

    [JsonPropertyName("effort")]
    public int Effort { get; init; }

    [JsonPropertyName("stat")]
    public PokemonNamedResource Stat { get; init; } = new();
}

public sealed class PokemonSprites
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; init; }

    [JsonPropertyName("front_shiny")]
    public string? FrontShiny { get; init; }
}

public sealed class PokemonNamedResource
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;
}

[JsonSerializable(typeof(PokemonOutput))]
internal sealed partial class PokemonJsonContext : JsonSerializerContext { }
