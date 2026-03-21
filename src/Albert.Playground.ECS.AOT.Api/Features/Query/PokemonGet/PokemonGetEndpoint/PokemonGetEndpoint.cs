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
        logger.LogInformation("[PokemonGetEndpoint][Get] Processar requisição GET /pokemon");

        var result = await useCase.ExecuteAsync(cancellationToken);

        logger.LogInformation("[PokemonGetEndpoint][Get] Retornar resposta do endpoint com dados do Pokémon");

        return Ok(result);
    }
}
