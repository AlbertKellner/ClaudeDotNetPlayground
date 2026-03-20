using Refit;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.PokeApi;

public interface IPokeApi
{
    [Get("/api/v2/pokemon/{name}")]
    Task<PokeApiOutput> GetPokemonAsync(
        string name,
        CancellationToken cancellationToken = default);
}
