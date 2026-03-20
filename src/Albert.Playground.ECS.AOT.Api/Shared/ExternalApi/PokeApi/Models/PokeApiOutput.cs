using System.Text.Json.Serialization;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.PokeApi;

public sealed class PokeApiOutput
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
    public List<PokeApiTypeSlot> Types { get; init; } = [];

    [JsonPropertyName("abilities")]
    public List<PokeApiAbilitySlot> Abilities { get; init; } = [];

    [JsonPropertyName("stats")]
    public List<PokeApiStatSlot> Stats { get; init; } = [];

    [JsonPropertyName("sprites")]
    public PokeApiSprites Sprites { get; init; } = new();
}

public sealed class PokeApiTypeSlot
{
    [JsonPropertyName("slot")]
    public int Slot { get; init; }

    [JsonPropertyName("type")]
    public PokeApiNamedResource Type { get; init; } = new();
}

public sealed class PokeApiAbilitySlot
{
    [JsonPropertyName("ability")]
    public PokeApiNamedResource Ability { get; init; } = new();

    [JsonPropertyName("is_hidden")]
    public bool IsHidden { get; init; }

    [JsonPropertyName("slot")]
    public int Slot { get; init; }
}

public sealed class PokeApiStatSlot
{
    [JsonPropertyName("base_stat")]
    public int BaseStat { get; init; }

    [JsonPropertyName("effort")]
    public int Effort { get; init; }

    [JsonPropertyName("stat")]
    public PokeApiNamedResource Stat { get; init; } = new();
}

public sealed class PokeApiSprites
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; init; }

    [JsonPropertyName("front_shiny")]
    public string? FrontShiny { get; init; }

    [JsonPropertyName("front_female")]
    public string? FrontFemale { get; init; }

    [JsonPropertyName("back_default")]
    public string? BackDefault { get; init; }
}

public sealed class PokeApiNamedResource
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;
}

[JsonSerializable(typeof(PokeApiOutput))]
internal sealed partial class PokeApiJsonContext : JsonSerializerContext { }
