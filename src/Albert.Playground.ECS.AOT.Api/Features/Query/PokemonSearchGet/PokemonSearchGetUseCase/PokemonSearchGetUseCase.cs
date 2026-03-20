using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.PokeApi;

namespace Albert.Playground.ECS.AOT.Api.Features.Query.PokemonSearchGet;

public sealed class PokemonSearchGetUseCase(
    IPokeApiClient pokeApiClient,
    ILogger<PokemonSearchGetUseCase> logger)
{
    private const string HardcodedPokemonName = "pikachu";

    public async Task<PokemonSearchGetOutput> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[PokemonSearchGetUseCase][ExecuteAsync] Executar caso de uso de busca de Pokémon");

        logger.LogInformation(
            "[PokemonSearchGetUseCase][ExecuteAsync] Consultar PokeAPI. PokemonName={PokemonName}",
            HardcodedPokemonName);

        var result = await pokeApiClient.GetPokemonAsync(HardcodedPokemonName, cancellationToken);

        logger.LogInformation(
            "[PokemonSearchGetUseCase][ExecuteAsync] Mapear resposta da PokeAPI para model da Feature");

        var output = MapToOutput(result);

        logger.LogInformation(
            "[PokemonSearchGetUseCase][ExecuteAsync] Retornar ficha do Pokémon obtida da PokeAPI. Id={Id}, Name={Name}",
            output.Id,
            output.Name);

        return output;
    }

    private static PokemonSearchGetOutput MapToOutput(PokeApiOutput result)
    {
        return new PokemonSearchGetOutput
        {
            Id = result.Id,
            Name = result.Name,
            BaseExperience = result.BaseExperience,
            Height = result.Height,
            Weight = result.Weight,
            IsDefault = result.IsDefault,
            Order = result.Order,
            LocationAreaEncounters = result.LocationAreaEncounters,
            Abilities = result.Abilities.Select(a => new PokemonAbilitySlot
            {
                Ability = a.Ability is not null ? new PokemonNamedResource { Name = a.Ability.Name, Url = a.Ability.Url } : null,
                IsHidden = a.IsHidden,
                Slot = a.Slot
            }).ToList(),
            Forms = result.Forms.Select(f => new PokemonNamedResource { Name = f.Name, Url = f.Url }).ToList(),
            GameIndices = result.GameIndices.Select(g => new PokemonGameIndex
            {
                GameIndexValue = g.GameIndexValue,
                Version = new PokemonNamedResource { Name = g.Version.Name, Url = g.Version.Url }
            }).ToList(),
            HeldItems = result.HeldItems.Select(h => new PokemonHeldItem
            {
                Item = new PokemonNamedResource { Name = h.Item.Name, Url = h.Item.Url },
                VersionDetails = h.VersionDetails.Select(v => new PokemonHeldItemVersionDetail
                {
                    Rarity = v.Rarity,
                    Version = new PokemonNamedResource { Name = v.Version.Name, Url = v.Version.Url }
                }).ToList()
            }).ToList(),
            Moves = result.Moves.Select(m => new PokemonMoveSlot
            {
                Move = new PokemonNamedResource { Name = m.Move.Name, Url = m.Move.Url },
                VersionGroupDetails = m.VersionGroupDetails.Select(v => new PokemonMoveVersionGroupDetail
                {
                    LevelLearnedAt = v.LevelLearnedAt,
                    MoveLearnMethod = new PokemonNamedResource { Name = v.MoveLearnMethod.Name, Url = v.MoveLearnMethod.Url },
                    VersionGroup = new PokemonNamedResource { Name = v.VersionGroup.Name, Url = v.VersionGroup.Url },
                    MoveOrder = v.MoveOrder
                }).ToList()
            }).ToList(),
            Species = new PokemonNamedResource { Name = result.Species.Name, Url = result.Species.Url },
            Sprites = new PokemonSprites
            {
                FrontDefault = result.Sprites.FrontDefault,
                FrontFemale = result.Sprites.FrontFemale,
                FrontShiny = result.Sprites.FrontShiny,
                FrontShinyFemale = result.Sprites.FrontShinyFemale,
                BackDefault = result.Sprites.BackDefault,
                BackFemale = result.Sprites.BackFemale,
                BackShiny = result.Sprites.BackShiny,
                BackShinyFemale = result.Sprites.BackShinyFemale,
                Other = new PokemonSpritesOther
                {
                    DreamWorld = new PokemonDreamWorldSprites
                    {
                        FrontDefault = result.Sprites.Other.DreamWorld.FrontDefault,
                        FrontFemale = result.Sprites.Other.DreamWorld.FrontFemale
                    },
                    Home = new PokemonHomeSprites
                    {
                        FrontDefault = result.Sprites.Other.Home.FrontDefault,
                        FrontFemale = result.Sprites.Other.Home.FrontFemale,
                        FrontShiny = result.Sprites.Other.Home.FrontShiny,
                        FrontShinyFemale = result.Sprites.Other.Home.FrontShinyFemale
                    },
                    OfficialArtwork = new PokemonOfficialArtwork
                    {
                        FrontDefault = result.Sprites.Other.OfficialArtwork.FrontDefault,
                        FrontShiny = result.Sprites.Other.OfficialArtwork.FrontShiny
                    },
                    Showdown = new PokemonShowdownSprites
                    {
                        FrontDefault = result.Sprites.Other.Showdown.FrontDefault,
                        FrontFemale = result.Sprites.Other.Showdown.FrontFemale,
                        FrontShiny = result.Sprites.Other.Showdown.FrontShiny,
                        FrontShinyFemale = result.Sprites.Other.Showdown.FrontShinyFemale,
                        BackDefault = result.Sprites.Other.Showdown.BackDefault,
                        BackFemale = result.Sprites.Other.Showdown.BackFemale,
                        BackShiny = result.Sprites.Other.Showdown.BackShiny,
                        BackShinyFemale = result.Sprites.Other.Showdown.BackShinyFemale
                    }
                }
            },
            Cries = new PokemonCries
            {
                Latest = result.Cries.Latest,
                Legacy = result.Cries.Legacy
            },
            Types = result.Types.Select(t => new PokemonTypeSlot
            {
                Slot = t.Slot,
                Type = new PokemonNamedResource { Name = t.Type.Name, Url = t.Type.Url }
            }).ToList(),
            PastTypes = result.PastTypes.Select(p => new PokemonPastType
            {
                Generation = new PokemonNamedResource { Name = p.Generation.Name, Url = p.Generation.Url },
                Types = p.Types.Select(t => new PokemonTypeSlot
                {
                    Slot = t.Slot,
                    Type = new PokemonNamedResource { Name = t.Type.Name, Url = t.Type.Url }
                }).ToList()
            }).ToList(),
            PastAbilities = result.PastAbilities.Select(p => new PokemonPastAbility
            {
                Generation = new PokemonNamedResource { Name = p.Generation.Name, Url = p.Generation.Url },
                Abilities = p.Abilities.Select(a => new PokemonAbilitySlot
                {
                    Ability = a.Ability is not null ? new PokemonNamedResource { Name = a.Ability.Name, Url = a.Ability.Url } : null,
                    IsHidden = a.IsHidden,
                    Slot = a.Slot
                }).ToList()
            }).ToList()
        };
    }
}
