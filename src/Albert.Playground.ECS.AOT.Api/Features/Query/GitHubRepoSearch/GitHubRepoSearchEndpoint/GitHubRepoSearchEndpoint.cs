using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Microsoft.AspNetCore.Mvc;

namespace Albert.Playground.ECS.AOT.Api.Features.Query.GitHubRepoSearch;

[ApiController]
[Route("github-repo-search")]
[Authenticate]
public sealed class GitHubRepoSearchEndpoint(
    GitHubRepoSearchUseCase useCase,
    ILogger<GitHubRepoSearchEndpoint> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        logger.LogInformation("[GitHubRepoSearchEndpoint][Get] Processar requisição GET /github-repo-search");

        var result = await useCase.ExecuteAsync(cancellationToken);

        logger.LogInformation("[GitHubRepoSearchEndpoint][Get] Retornar resposta do endpoint com repositórios GitHub");

        return Ok(result);
    }
}
