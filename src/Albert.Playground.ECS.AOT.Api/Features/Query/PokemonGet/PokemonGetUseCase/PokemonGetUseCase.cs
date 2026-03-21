using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon;

namespace Albert.Playground.ECS.AOT.Api.Features.Query.PokemonGet;

public sealed class PokemonGetUseCase(
    IPokemonApiClient pokemonApiClient,
    ILogger<PokemonGetUseCase> logger)
{
    private const int PikachuId = 25;

    public async Task<PokemonGetOutput> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[PokemonGetUseCase][ExecuteAsync] Executar caso de uso de consulta de Pokemon. PokemonId={PokemonId}", PikachuId);

        logger.LogInformation("[PokemonGetUseCase][ExecuteAsync] Consultar PokeAPI. PokemonId={PokemonId}", PikachuId);

        var result = await pokemonApiClient.GetByIdAsync(PikachuId, cancellationToken);

        logger.LogInformation("[PokemonGetUseCase][ExecuteAsync] Mapear resposta da PokeAPI para model da Feature");

        var types = new List<PokemonGetTypeItem>();

        logger.LogInformation("[PokemonGetUseCase][ExecuteAsync] Iterar tipos do Pokemon. Count={Count}", result.Types.Count);

        foreach (var typeSlot in result.Types)
        {
            types.Add(new PokemonGetTypeItem
            {
                Slot = typeSlot.Slot,
                Name = typeSlot.Type.Name
            });
        }

        logger.LogInformation("[PokemonGetUseCase][ExecuteAsync] Iteracao de tipos concluida");

        var abilities = new List<PokemonGetAbilityItem>();

        logger.LogInformation("[PokemonGetUseCase][ExecuteAsync] Iterar habilidades do Pokemon. Count={Count}", result.Abilities.Count);

        foreach (var abilitySlot in result.Abilities)
        {
            abilities.Add(new PokemonGetAbilityItem
            {
                Name = abilitySlot.Ability.Name,
                IsHidden = abilitySlot.IsHidden
            });
        }

        logger.LogInformation("[PokemonGetUseCase][ExecuteAsync] Iteracao de habilidades concluida");

        var stats = new List<PokemonGetStatItem>();

        logger.LogInformation("[PokemonGetUseCase][ExecuteAsync] Iterar stats do Pokemon. Count={Count}", result.Stats.Count);

        foreach (var statSlot in result.Stats)
        {
            stats.Add(new PokemonGetStatItem
            {
                Name = statSlot.Stat.Name,
                BaseStat = statSlot.BaseStat,
                Effort = statSlot.Effort
            });
        }

        logger.LogInformation("[PokemonGetUseCase][ExecuteAsync] Iteracao de stats concluida");

        var output = new PokemonGetOutput
        {
            Id = result.Id,
            Name = result.Name,
            Height = result.Height,
            Weight = result.Weight,
            BaseExperience = result.BaseExperience,
            FrontDefaultSprite = result.Sprites.FrontDefault,
            FrontShinySprite = result.Sprites.FrontShiny,
            Types = types,
            Abilities = abilities,
            Stats = stats
        };

        logger.LogInformation(
            "[PokemonGetUseCase][ExecuteAsync] Retornar dados do Pokemon. PokemonId={PokemonId}, PokemonName={PokemonName}",
            output.Id,
            output.Name);

        return output;
    }
}
