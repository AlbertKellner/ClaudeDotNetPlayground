using ClaudeDotNetPlayground.Infra.Security;
using Microsoft.AspNetCore.Mvc;

namespace ClaudeDotNetPlayground.Features.Query.TestGet;

[ApiController]
[Route("test")]
[Authenticate]
public sealed class TestGetEndpoint(TestGetUseCase useCase, ILogger<TestGetEndpoint> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        logger.LogInformation("TestGet request received");
        var result = useCase.Execute();
        logger.LogInformation("TestGet returning: {Result}", result);
        return Ok(result);
    }
}
