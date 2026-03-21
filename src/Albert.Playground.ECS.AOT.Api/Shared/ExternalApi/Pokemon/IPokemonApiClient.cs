using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon.Models;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon;

public interface IPokemonApiClient
{
    Task<PokemonOutput> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
