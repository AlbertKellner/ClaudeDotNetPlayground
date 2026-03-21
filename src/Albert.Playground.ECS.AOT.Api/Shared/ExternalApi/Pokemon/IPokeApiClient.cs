namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon;

public interface IPokeApiClient
{
    Task<PokeApiOutput> GetPokemonByIdAsync(int id, CancellationToken cancellationToken = default);
}
