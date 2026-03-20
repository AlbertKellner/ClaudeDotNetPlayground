using System.Text.Json.Serialization;

namespace Albert.Playground.ECS.AOT.Api.Features.Query.PokemonSearchGet;

public sealed class PokemonSearchGetOutput
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
    public List<PokemonAbilitySlot> Abilities { get; init; } = [];

    [JsonPropertyName("forms")]
    public List<PokemonNamedResource> Forms { get; init; } = [];

    [JsonPropertyName("game_indices")]
    public List<PokemonGameIndex> GameIndices { get; init; } = [];

    [JsonPropertyName("held_items")]
    public List<PokemonHeldItem> HeldItems { get; init; } = [];

    [JsonPropertyName("moves")]
    public List<PokemonMoveSlot> Moves { get; init; } = [];

    [JsonPropertyName("species")]
    public PokemonNamedResource Species { get; init; } = new();

    [JsonPropertyName("sprites")]
    public PokemonSprites Sprites { get; init; } = new();

    [JsonPropertyName("cries")]
    public PokemonCries Cries { get; init; } = new();

    [JsonPropertyName("stats")]
    public List<PokemonStatSlot> Stats { get; init; } = [];

    [JsonPropertyName("types")]
    public List<PokemonTypeSlot> Types { get; init; } = [];

    [JsonPropertyName("past_types")]
    public List<PokemonPastType> PastTypes { get; init; } = [];

    [JsonPropertyName("past_abilities")]
    public List<PokemonPastAbility> PastAbilities { get; init; } = [];
}

public sealed class PokemonNamedResource
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;
}

public sealed class PokemonAbilitySlot
{
    [JsonPropertyName("ability")]
    public PokemonNamedResource? Ability { get; init; }

    [JsonPropertyName("is_hidden")]
    public bool IsHidden { get; init; }

    [JsonPropertyName("slot")]
    public int Slot { get; init; }
}

public sealed class PokemonGameIndex
{
    [JsonPropertyName("game_index")]
    public int GameIndexValue { get; init; }

    [JsonPropertyName("version")]
    public PokemonNamedResource Version { get; init; } = new();
}

public sealed class PokemonHeldItem
{
    [JsonPropertyName("item")]
    public PokemonNamedResource Item { get; init; } = new();

    [JsonPropertyName("version_details")]
    public List<PokemonHeldItemVersionDetail> VersionDetails { get; init; } = [];
}

public sealed class PokemonHeldItemVersionDetail
{
    [JsonPropertyName("rarity")]
    public int Rarity { get; init; }

    [JsonPropertyName("version")]
    public PokemonNamedResource Version { get; init; } = new();
}

public sealed class PokemonMoveSlot
{
    [JsonPropertyName("move")]
    public PokemonNamedResource Move { get; init; } = new();

    [JsonPropertyName("version_group_details")]
    public List<PokemonMoveVersionGroupDetail> VersionGroupDetails { get; init; } = [];
}

public sealed class PokemonMoveVersionGroupDetail
{
    [JsonPropertyName("level_learned_at")]
    public int LevelLearnedAt { get; init; }

    [JsonPropertyName("move_learn_method")]
    public PokemonNamedResource MoveLearnMethod { get; init; } = new();

    [JsonPropertyName("version_group")]
    public PokemonNamedResource VersionGroup { get; init; } = new();

    [JsonPropertyName("order")]
    public int? MoveOrder { get; init; }
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

public sealed class PokemonTypeSlot
{
    [JsonPropertyName("slot")]
    public int Slot { get; init; }

    [JsonPropertyName("type")]
    public PokemonNamedResource Type { get; init; } = new();
}

public sealed class PokemonSprites
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
    public PokemonSpritesOther Other { get; init; } = new();
}

public sealed class PokemonSpritesOther
{
    [JsonPropertyName("dream_world")]
    public PokemonDreamWorldSprites DreamWorld { get; init; } = new();

    [JsonPropertyName("home")]
    public PokemonHomeSprites Home { get; init; } = new();

    [JsonPropertyName("official-artwork")]
    public PokemonOfficialArtwork OfficialArtwork { get; init; } = new();

    [JsonPropertyName("showdown")]
    public PokemonShowdownSprites Showdown { get; init; } = new();
}

public sealed class PokemonDreamWorldSprites
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; init; }

    [JsonPropertyName("front_female")]
    public string? FrontFemale { get; init; }
}

public sealed class PokemonHomeSprites
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

public sealed class PokemonOfficialArtwork
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; init; }

    [JsonPropertyName("front_shiny")]
    public string? FrontShiny { get; init; }
}

public sealed class PokemonShowdownSprites
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

public sealed class PokemonCries
{
    [JsonPropertyName("latest")]
    public string? Latest { get; init; }

    [JsonPropertyName("legacy")]
    public string? Legacy { get; init; }
}

public sealed class PokemonPastType
{
    [JsonPropertyName("generation")]
    public PokemonNamedResource Generation { get; init; } = new();

    [JsonPropertyName("types")]
    public List<PokemonTypeSlot> Types { get; init; } = [];
}

public sealed class PokemonPastAbility
{
    [JsonPropertyName("generation")]
    public PokemonNamedResource Generation { get; init; } = new();

    [JsonPropertyName("abilities")]
    public List<PokemonAbilitySlot> Abilities { get; init; } = [];
}
