using Refit;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon;

public interface IPokeApi
{
    [Get("/api/v2/pokemon/{id}")]
    Task<PokeApiOutput> GetPokemonByIdAsync(
        [AliasAs("id")] int id,
        CancellationToken cancellationToken = default);
}
