using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;

namespace Starter.Template.AOT.Api.Infra.Security;

public sealed class AuthenticateFilter(ITokenService tokenService, ILogger<AuthenticateFilter> logger) : IAsyncActionFilter
{
    public const string AuthenticatedUserItemKey = "AuthenticatedUser";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        logger.LogInformation("[AuthenticateFilter][OnActionExecutionAsync] Validar Bearer Token da requisição");

        var token = ExtractBearerToken(context.HttpContext);

        if (token is null)
        {
            logger.LogWarning("[AuthenticateFilter][OnActionExecutionAsync] Retornar 401 - token ausente na requisição");

            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "Authorization header with Bearer token is required.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.2"
            })
            { StatusCode = StatusCodes.Status401Unauthorized };
            return;
        }

        var user = tokenService.ValidateToken(token);

        if (user is null)
        {
            logger.LogWarning("[AuthenticateFilter][OnActionExecutionAsync] Retornar 401 - token inválido ou expirado");

            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "The provided Bearer token is invalid or expired.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.2"
            })
            { StatusCode = StatusCodes.Status401Unauthorized };
            return;
        }

        context.HttpContext.Items[AuthenticatedUserItemKey] = user;

        using (LogContext.PushProperty("UserId", user.Id))
        using (LogContext.PushProperty("UserName", user.UserName))
        {
            logger.LogInformation("[AuthenticateFilter][OnActionExecutionAsync] Prosseguir com requisição autenticada. UserId={UserId}, UserName={UserName}", user.Id, user.UserName);

            await next();
        }
    }

    private static string? ExtractBearerToken(HttpContext context)
    {
        var header = context.Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(header) || !header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return null;

        var token = header["Bearer ".Length..].Trim();
        return string.IsNullOrWhiteSpace(token) ? null : token;
    }
}
