using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon.Models;
using Refit;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon;

public interface IPokemonApi
{
    [Get("/api/v2/pokemon/{id}")]
    Task<PokemonOutput> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
