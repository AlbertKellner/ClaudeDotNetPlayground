using Microsoft.AspNetCore.Mvc;

namespace ClaudeDotNetPlayground.Features.Command.UserLogin;

[ApiController]
[Route("login")]
public sealed class UserLoginEndpoint(UserLoginUseCase useCase, ILogger<UserLoginEndpoint> logger) : ControllerBase
{
    [HttpPost]
    public IActionResult Post([FromBody] UserLoginInput input)
    {
        logger.LogInformation("UserLogin request received for user {UserName}", input.UserName);

        var result = useCase.Execute(input);

        if (result is null)
        {
            logger.LogWarning("UserLogin failed: invalid credentials for user {UserName}", input.UserName);
            return Problem(
                detail: "Invalid username or password.",
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized",
                type: "https://tools.ietf.org/html/rfc9110#section-15.5.2"
            );
        }

        logger.LogInformation("UserLogin succeeded for user {UserName}", input.UserName);
        return Ok(result);
    }
}
