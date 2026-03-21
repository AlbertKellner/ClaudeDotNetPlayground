using System.Text.Json.Serialization;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon;

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

    [JsonPropertyName("types")]
    public List<PokeApiTypeSlot> Types { get; init; } = [];

    [JsonPropertyName("sprites")]
    public PokeApiSprites Sprites { get; init; } = new();
}

public sealed class PokeApiTypeSlot
{
    [JsonPropertyName("slot")]
    public int Slot { get; init; }

    [JsonPropertyName("type")]
    public PokeApiTypeInfo Type { get; init; } = new();
}

public sealed class PokeApiTypeInfo
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;
}

public sealed class PokeApiSprites
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; init; }
}

[JsonSerializable(typeof(PokeApiOutput))]
[JsonSerializable(typeof(List<PokeApiTypeSlot>))]
[JsonSerializable(typeof(PokeApiSprites))]
internal sealed partial class PokeApiJsonContext : JsonSerializerContext { }
