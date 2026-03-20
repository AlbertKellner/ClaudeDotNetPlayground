using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Microsoft.AspNetCore.Mvc;

namespace Albert.Playground.ECS.AOT.Api.Features.Query.PokemonSearchGet;

[ApiController]
[Route("pokemon-search")]
[Authenticate]
public sealed class PokemonSearchGetEndpoint(
    PokemonSearchGetUseCase useCase,
    ILogger<PokemonSearchGetEndpoint> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        logger.LogInformation("[PokemonSearchGetEndpoint][Get] Processar requisição GET /pokemon-search");

        var result = await useCase.ExecuteAsync(cancellationToken);

        logger.LogInformation("[PokemonSearchGetEndpoint][Get] Retornar ficha completa do Pokémon. Id={Id}, Name={Name}",
            result.Id,
            result.Name);

        return Ok(result);
    }
}
