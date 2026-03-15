using ClaudeDotNetPlayground.Features.Query.TestGet.TestGetUseCase;
using Microsoft.AspNetCore.Mvc;

namespace ClaudeDotNetPlayground.Features.Query.TestGet.TestGetEndpoint;

[ApiController]
[Route("test")]
public class TestGetEndpoint(TestGetUseCase useCase, ILogger<TestGetEndpoint> logger) : ControllerBase
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
