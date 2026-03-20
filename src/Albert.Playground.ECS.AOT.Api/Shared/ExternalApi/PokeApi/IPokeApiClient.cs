namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.PokeApi;

public interface IPokeApiClient
{
    Task<PokeApiOutput> GetPokemonAsync(string name, CancellationToken cancellationToken = default);
}
