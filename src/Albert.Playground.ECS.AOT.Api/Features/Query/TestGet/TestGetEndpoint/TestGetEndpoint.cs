using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Microsoft.AspNetCore.Mvc;

namespace Albert.Playground.ECS.AOT.Api.Features.Query.TestGet;

[ApiController]
[Route("test")]
[Authenticate]
public sealed class TestGetEndpoint(TestGetUseCase useCase, ILogger<TestGetEndpoint> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        logger.LogInformation("[TestGetEndpoint][Get] Processar requisição GET /test");

        var result = useCase.Execute();

        logger.LogInformation("[TestGetEndpoint][Get] Retornar resposta do endpoint. Result={Result}", result);

        return Ok(result);
    }
}
