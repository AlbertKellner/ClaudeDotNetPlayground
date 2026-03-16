using ClaudeDotNetPlayground.Infra.Security;
using Microsoft.AspNetCore.Mvc;

namespace ClaudeDotNetPlayground.Features.Query.WeatherConditionsGet;

[ApiController]
[Route("weather-conditions")]
[Authenticate]
public sealed class WeatherConditionsGetEndpoint(
    WeatherConditionsGetUseCase useCase,
    ILogger<WeatherConditionsGetEndpoint> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        logger.LogInformation("[WeatherConditionsGetEndpoint][Get] Processar requisição GET /weather-conditions");

        var result = await useCase.ExecuteAsync(cancellationToken);

        logger.LogInformation("[WeatherConditionsGetEndpoint][Get] Retornar resposta do endpoint com condições climáticas de São Paulo");

        return Ok(result);
    }
}
