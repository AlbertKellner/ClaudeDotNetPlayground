using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Microsoft.AspNetCore.Mvc;

namespace Albert.Playground.ECS.AOT.Api.Features.Query.PokemonGet;

[ApiController]
[Route("pokemon")]
[Authenticate]
public sealed class PokemonGetEndpoint(
    PokemonGetUseCase useCase,
    ILogger<PokemonGetEndpoint> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[PokemonGetEndpoint][Get] Processar requisição GET /pokemon");

        var result = await useCase.ExecuteAsync(cancellationToken);

        logger.LogInformation(
            "[PokemonGetEndpoint][Get] Resposta recebida da API. Id={Id}, Name={Name}, Height={Height}, Weight={Weight}, BaseExperience={BaseExperience}, Types={@Types}, Abilities={@Abilities}, Stats={@Stats}",
            result.Id,
            result.Name,
            result.Height,
            result.Weight,
            result.BaseExperience,
            result.Types,
            result.Abilities,
            result.Stats);

        logger.LogInformation(
            "[PokemonGetEndpoint][Get] Retornar resposta do endpoint com ficha do Pokémon");

        return Ok(result);
    }
}
