using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.PokeApi;

namespace Albert.Playground.ECS.AOT.Api.Features.Query.PokemonGet;

public sealed class PokemonGetUseCase(
    IPokeApiClient pokeApiClient,
    ILogger<PokemonGetUseCase> logger)
{
    private const string DefaultPokemonName = "pikachu";

    public async Task<PokemonGetOutput> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[PokemonGetUseCase][ExecuteAsync] Executar caso de uso de busca de Pokémon. Name={Name}",
            DefaultPokemonName);

        logger.LogInformation(
            "[PokemonGetUseCase][ExecuteAsync] Consultar PokeAPI para o Pokémon {Name}",
            DefaultPokemonName);

        var result = await pokeApiClient.GetPokemonAsync(DefaultPokemonName, cancellationToken);

        logger.LogInformation(
            "[PokemonGetUseCase][ExecuteAsync] Mapear resposta da PokeAPI para model da Feature");

        var output = new PokemonGetOutput
        {
            Id = result.Id,
            Name = result.Name,
            Height = result.Height,
            Weight = result.Weight,
            BaseExperience = result.BaseExperience,
            Types = result.Types.Select(t => new PokemonGetType
            {
                Slot = t.Slot,
                Name = t.Type.Name
            }).ToList(),
            Abilities = result.Abilities.Select(a => new PokemonGetAbility
            {
                Name = a.Ability.Name,
                IsHidden = a.IsHidden,
                Slot = a.Slot
            }).ToList(),
            Stats = result.Stats.Select(s => new PokemonGetStat
            {
                Name = s.Stat.Name,
                BaseStat = s.BaseStat,
                Effort = s.Effort
            }).ToList()
        };

        logger.LogInformation(
            "[PokemonGetUseCase][ExecuteAsync] Retornar ficha do Pokémon. Id={Id}, Name={PokemonName}",
            output.Id,
            output.Name);

        return output;
    }
}
