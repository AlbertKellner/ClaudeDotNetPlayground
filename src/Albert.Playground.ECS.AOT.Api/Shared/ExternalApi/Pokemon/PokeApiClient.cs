namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon;

public sealed class PokeApiClient(
    IPokeApi api,
    ILogger<PokeApiClient> logger) : IPokeApiClient
{
    public async Task<PokeApiOutput> GetPokemonByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[PokeApiClient][GetPokemonByIdAsync] Executar requisição HTTP à PokéAPI. PokemonId={PokemonId}",
            id);

        var result = await api.GetPokemonByIdAsync(id, cancellationToken);

        logger.LogInformation(
            "[PokeApiClient][GetPokemonByIdAsync] Retornar resposta da PokéAPI. PokemonName={PokemonName}",
            result.Name);

        return result;
    }
}
