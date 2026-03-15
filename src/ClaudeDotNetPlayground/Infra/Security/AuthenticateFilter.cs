using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;

namespace ClaudeDotNetPlayground.Infra.Security;

public sealed class AuthenticateFilter(ITokenService tokenService) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var token = ExtractBearerToken(context.HttpContext);

        if (token is null)
        {
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

        using (LogContext.PushProperty("UserId", user.Id))
        using (LogContext.PushProperty("UserName", user.UserName))
        {
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
