namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.PokeApi;

public sealed class PokeApiClient(
    IPokeApi api,
    ILogger<PokeApiClient> logger) : IPokeApiClient
{
    public async Task<PokeApiOutput> GetPokemonAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[PokeApiClient][GetPokemonAsync] Executar requisição HTTP à PokeAPI. Name={Name}",
            name);

        var result = await api.GetPokemonAsync(name, cancellationToken);

        logger.LogInformation(
            "[PokeApiClient][GetPokemonAsync] Retornar resposta da PokeAPI. Id={Id}, Name={PokemonName}",
            result.Id,
            result.Name);

        return result;
    }
}
