using Microsoft.AspNetCore.Mvc;

namespace ClaudeDotNetPlayground.Features.Query.TestGet;

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
