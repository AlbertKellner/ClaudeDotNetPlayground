using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Microsoft.AspNetCore.Mvc;

namespace Albert.Playground.ECS.AOT.Api.Features.Query.WeatherConditionsGet;

[ApiController]
[Route("weather-conditions")]
[Authenticate]
public sealed class WeatherConditionsGetEndpoint(
    WeatherConditionsGetUseCase useCase,
    ILogger<WeatherConditionsGetEndpoint> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[WeatherConditionsGetEndpoint][Get] Processar requisição GET /weather-conditions. Latitude={Latitude}, Longitude={Longitude}",
            latitude,
            longitude);

        var input = new WeatherConditionsGetInput
        {
            Latitude = latitude,
            Longitude = longitude
        };

        var result = await useCase.ExecuteAsync(input, cancellationToken);

        logger.LogInformation(
            "[WeatherConditionsGetEndpoint][Get] Retornar resposta do endpoint com condições climáticas. Latitude={Latitude}, Longitude={Longitude}",
            latitude,
            longitude);

        return Ok(result);
    }
}
