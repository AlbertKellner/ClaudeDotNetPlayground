using Microsoft.AspNetCore.Mvc;
using UseCase = ClaudeDotNetPlayground.Features.Query.TestGet.TestGetUseCase.TestGetUseCase;

namespace ClaudeDotNetPlayground.Features.Query.TestGet.TestGetEndpoint;

[ApiController]
[Route("test")]
public class TestGetEndpoint(UseCase useCase, ILogger<TestGetEndpoint> logger) : ControllerBase
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
