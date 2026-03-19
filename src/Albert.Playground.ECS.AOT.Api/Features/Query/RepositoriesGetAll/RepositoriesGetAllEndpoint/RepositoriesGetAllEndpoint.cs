using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Microsoft.AspNetCore.Mvc;

namespace Albert.Playground.ECS.AOT.Api.Features.Query.RepositoriesGetAll;

[ApiController]
[Route("repositories")]
[Authenticate]
public sealed class RepositoriesGetAllEndpoint(
    RepositoriesGetAllUseCase useCase,
    ILogger<RepositoriesGetAllEndpoint> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[RepositoriesGetAllEndpoint][Get] Processar requisição GET /repositories");

        var result = await useCase.ExecuteAsync(cancellationToken);

        logger.LogInformation(
            "[RepositoriesGetAllEndpoint][Get] Retornar resposta do endpoint com lista de repositórios");

        return Ok(result);
    }
}
