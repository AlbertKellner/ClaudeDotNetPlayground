using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon.Models;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon;

public sealed class PokemonApiClient(
    IPokemonApi api,
    ILogger<PokemonApiClient> logger) : IPokemonApiClient
{
    public async Task<PokemonOutput> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[PokemonApiClient][GetByIdAsync] Executar requisicao HTTP a PokeAPI. PokemonId={PokemonId}",
            id);

        var result = await api.GetByIdAsync(id, cancellationToken);

        logger.LogInformation(
            "[PokemonApiClient][GetByIdAsync] Retornar resposta da PokeAPI. PokemonName={PokemonName}",
            result.Name);

        return result;
    }
}
