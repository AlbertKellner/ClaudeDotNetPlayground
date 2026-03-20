using System.Text.Json.Serialization;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.PokeApi;

public sealed class PokeApiOutput
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("base_experience")]
    public int BaseExperience { get; init; }

    [JsonPropertyName("height")]
    public int Height { get; init; }

    [JsonPropertyName("weight")]
    public int Weight { get; init; }

    [JsonPropertyName("is_default")]
    public bool IsDefault { get; init; }

    [JsonPropertyName("order")]
    public int Order { get; init; }

    [JsonPropertyName("location_area_encounters")]
    public string LocationAreaEncounters { get; init; } = string.Empty;

    [JsonPropertyName("abilities")]
    public List<PokeApiAbilitySlot> Abilities { get; init; } = [];

    [JsonPropertyName("forms")]
    public List<PokeApiNamedResource> Forms { get; init; } = [];

    [JsonPropertyName("game_indices")]
    public List<PokeApiGameIndex> GameIndices { get; init; } = [];

    [JsonPropertyName("held_items")]
    public List<PokeApiHeldItem> HeldItems { get; init; } = [];

    [JsonPropertyName("moves")]
    public List<PokeApiMoveSlot> Moves { get; init; } = [];

    [JsonPropertyName("species")]
    public PokeApiNamedResource Species { get; init; } = new();

    [JsonPropertyName("sprites")]
    public PokeApiSprites Sprites { get; init; } = new();

    [JsonPropertyName("cries")]
    public PokeApiCries Cries { get; init; } = new();

    [JsonPropertyName("stats")]
    public List<PokeApiStatSlot> Stats { get; init; } = [];

    [JsonPropertyName("types")]
    public List<PokeApiTypeSlot> Types { get; init; } = [];

    [JsonPropertyName("past_types")]
    public List<PokeApiPastType> PastTypes { get; init; } = [];

    [JsonPropertyName("past_abilities")]
    public List<PokeApiPastAbility> PastAbilities { get; init; } = [];
}

public sealed class PokeApiNamedResource
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;
}

public sealed class PokeApiAbilitySlot
{
    [JsonPropertyName("ability")]
    public PokeApiNamedResource? Ability { get; init; }

    [JsonPropertyName("is_hidden")]
    public bool IsHidden { get; init; }

    [JsonPropertyName("slot")]
    public int Slot { get; init; }
}

public sealed class PokeApiGameIndex
{
    [JsonPropertyName("game_index")]
    public int GameIndexValue { get; init; }

    [JsonPropertyName("version")]
    public PokeApiNamedResource Version { get; init; } = new();
}

public sealed class PokeApiHeldItem
{
    [JsonPropertyName("item")]
    public PokeApiNamedResource Item { get; init; } = new();

    [JsonPropertyName("version_details")]
    public List<PokeApiHeldItemVersionDetail> VersionDetails { get; init; } = [];
}

public sealed class PokeApiHeldItemVersionDetail
{
    [JsonPropertyName("rarity")]
    public int Rarity { get; init; }

    [JsonPropertyName("version")]
    public PokeApiNamedResource Version { get; init; } = new();
}

public sealed class PokeApiMoveSlot
{
    [JsonPropertyName("move")]
    public PokeApiNamedResource Move { get; init; } = new();

    [JsonPropertyName("version_group_details")]
    public List<PokeApiMoveVersionGroupDetail> VersionGroupDetails { get; init; } = [];
}

public sealed class PokeApiMoveVersionGroupDetail
{
    [JsonPropertyName("level_learned_at")]
    public int LevelLearnedAt { get; init; }

    [JsonPropertyName("move_learn_method")]
    public PokeApiNamedResource MoveLearnMethod { get; init; } = new();

    [JsonPropertyName("version_group")]
    public PokeApiNamedResource VersionGroup { get; init; } = new();

    [JsonPropertyName("order")]
    public int? MoveOrder { get; init; }
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

public sealed class PokeApiTypeSlot
{
    [JsonPropertyName("slot")]
    public int Slot { get; init; }

    [JsonPropertyName("type")]
    public PokeApiNamedResource Type { get; init; } = new();
}

public sealed class PokeApiSprites
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; init; }

    [JsonPropertyName("front_female")]
    public string? FrontFemale { get; init; }

    [JsonPropertyName("front_shiny")]
    public string? FrontShiny { get; init; }

    [JsonPropertyName("front_shiny_female")]
    public string? FrontShinyFemale { get; init; }

    [JsonPropertyName("back_default")]
    public string? BackDefault { get; init; }

    [JsonPropertyName("back_female")]
    public string? BackFemale { get; init; }

    [JsonPropertyName("back_shiny")]
    public string? BackShiny { get; init; }

    [JsonPropertyName("back_shiny_female")]
    public string? BackShinyFemale { get; init; }

    [JsonPropertyName("other")]
    public PokeApiSpritesOther Other { get; init; } = new();
}

public sealed class PokeApiSpritesOther
{
    [JsonPropertyName("dream_world")]
    public PokeApiDreamWorldSprites DreamWorld { get; init; } = new();

    [JsonPropertyName("home")]
    public PokeApiHomeSprites Home { get; init; } = new();

    [JsonPropertyName("official-artwork")]
    public PokeApiOfficialArtwork OfficialArtwork { get; init; } = new();

    [JsonPropertyName("showdown")]
    public PokeApiShowdownSprites Showdown { get; init; } = new();
}

public sealed class PokeApiDreamWorldSprites
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; init; }

    [JsonPropertyName("front_female")]
    public string? FrontFemale { get; init; }
}

public sealed class PokeApiHomeSprites
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; init; }

    [JsonPropertyName("front_female")]
    public string? FrontFemale { get; init; }

    [JsonPropertyName("front_shiny")]
    public string? FrontShiny { get; init; }

    [JsonPropertyName("front_shiny_female")]
    public string? FrontShinyFemale { get; init; }
}

public sealed class PokeApiOfficialArtwork
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; init; }

    [JsonPropertyName("front_shiny")]
    public string? FrontShiny { get; init; }
}

public sealed class PokeApiShowdownSprites
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; init; }

    [JsonPropertyName("front_female")]
    public string? FrontFemale { get; init; }

    [JsonPropertyName("front_shiny")]
    public string? FrontShiny { get; init; }

    [JsonPropertyName("front_shiny_female")]
    public string? FrontShinyFemale { get; init; }

    [JsonPropertyName("back_default")]
    public string? BackDefault { get; init; }

    [JsonPropertyName("back_female")]
    public string? BackFemale { get; init; }

    [JsonPropertyName("back_shiny")]
    public string? BackShiny { get; init; }

    [JsonPropertyName("back_shiny_female")]
    public string? BackShinyFemale { get; init; }
}

public sealed class PokeApiCries
{
    [JsonPropertyName("latest")]
    public string? Latest { get; init; }

    [JsonPropertyName("legacy")]
    public string? Legacy { get; init; }
}

public sealed class PokeApiPastType
{
    [JsonPropertyName("generation")]
    public PokeApiNamedResource Generation { get; init; } = new();

    [JsonPropertyName("types")]
    public List<PokeApiTypeSlot> Types { get; init; } = [];
}

public sealed class PokeApiPastAbility
{
    [JsonPropertyName("generation")]
    public PokeApiNamedResource Generation { get; init; } = new();

    [JsonPropertyName("abilities")]
    public List<PokeApiAbilitySlot> Abilities { get; init; } = [];
}

[JsonSerializable(typeof(PokeApiOutput))]
internal sealed partial class PokeApiJsonContext : JsonSerializerContext { }
