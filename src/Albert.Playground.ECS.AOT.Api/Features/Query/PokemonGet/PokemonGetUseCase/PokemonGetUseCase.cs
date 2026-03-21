using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon;

namespace Albert.Playground.ECS.AOT.Api.Features.Query.PokemonGet;

public sealed class PokemonGetUseCase(
    IPokeApiClient pokeApiClient,
    ILogger<PokemonGetUseCase> logger)
{
    private const int PikachuId = 25;

    public async Task<PokemonGetOutput> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[PokemonGetUseCase][ExecuteAsync] Executar caso de uso de consulta de Pokémon");

        logger.LogInformation(
            "[PokemonGetUseCase][ExecuteAsync] Consultar PokéAPI. PokemonId={PokemonId}",
            PikachuId);

        var result = await pokeApiClient.GetPokemonByIdAsync(PikachuId, cancellationToken);

        logger.LogInformation(
            "[PokemonGetUseCase][ExecuteAsync] Mapear resposta da PokéAPI para model da Feature. PokemonName={PokemonName}",
            result.Name);

        var types = new List<PokemonGetType>(result.Types.Count);

        logger.LogInformation(
            "[PokemonGetUseCase][ExecuteAsync] Iniciar iteração de tipos do Pokémon para mapeamento");

        foreach (var typeSlot in result.Types)
        {
            types.Add(new PokemonGetType
            {
                Slot = typeSlot.Slot,
                Name = typeSlot.Type.Name
            });
        }

        logger.LogInformation(
            "[PokemonGetUseCase][ExecuteAsync] Concluir iteração de tipos. Total={Total}",
            types.Count);

        var output = new PokemonGetOutput
        {
            Id = result.Id,
            Name = result.Name,
            Height = result.Height,
            Weight = result.Weight,
            Types = types,
            SpriteUrl = result.Sprites.FrontDefault
        };

        logger.LogInformation(
            "[PokemonGetUseCase][ExecuteAsync] Retornar dados do Pokémon. Id={Id}, Name={Name}",
            output.Id,
            output.Name);

        return output;
    }
}
