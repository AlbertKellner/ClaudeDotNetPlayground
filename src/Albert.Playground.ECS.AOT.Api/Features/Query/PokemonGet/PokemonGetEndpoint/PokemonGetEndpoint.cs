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
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        logger.LogInformation("[PokemonGetEndpoint][Get] Processar requisicao GET /pokemon/{PokemonId}", id);

        var result = await useCase.ExecuteAsync(id, cancellationToken);

        logger.LogInformation("[PokemonGetEndpoint][Get] Retornar resposta do endpoint com dados do Pokemon. PokemonId={PokemonId}, PokemonName={PokemonName}", id, result.Name);

        return Ok(result);
    }
}
