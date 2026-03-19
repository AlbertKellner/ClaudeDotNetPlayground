using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Microsoft.AspNetCore.Mvc;

namespace Albert.Playground.ECS.AOT.Api.Features.Command.RepositoriesSyncAll;

[ApiController]
[Route("repositories")]
[Authenticate]
public sealed class RepositoriesSyncAllEndpoint(
    RepositoriesSyncAllUseCase useCase,
    ILogger<RepositoriesSyncAllEndpoint> logger) : ControllerBase
{
    [HttpPost("sync")]
    public async Task<IActionResult> Post(CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[RepositoriesSyncAllEndpoint][Post] Processar requisição POST /repositories/sync");

        var result = await useCase.ExecuteAsync(cancellationToken);

        logger.LogInformation(
            "[RepositoriesSyncAllEndpoint][Post] Retornar resposta do endpoint com resultado da sincronização");

        return Ok(result);
    }
}
