using Microsoft.AspNetCore.Mvc;

namespace ClaudeDotNetPlayground.Features.Command.UserLogin;

[ApiController]
[Route("login")]
public sealed class UserLoginEndpoint(UserLoginUseCase useCase, ILogger<UserLoginEndpoint> logger) : ControllerBase
{
    [HttpPost]
    public IActionResult Post([FromBody] UserLoginInput input)
    {
        logger.LogInformation("[UserLoginEndpoint][Post] Processar requisição POST /login. UserName={UserName}", input.UserName);

        var result = useCase.Execute(input);

        if (result is null)
        {
            logger.LogWarning("[UserLoginEndpoint][Post] Retornar 401 - credenciais inválidas. UserName={UserName}", input.UserName);

            return Problem(
                detail: "Invalid username or password.",
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized",
                type: "https://tools.ietf.org/html/rfc9110#section-15.5.2"
            );
        }

        logger.LogInformation("[UserLoginEndpoint][Post] Retornar resposta do endpoint com autenticação bem-sucedida. UserName={UserName}", input.UserName);

        return Ok(result);
    }
}
