using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Starter.Template.AOT.Api.Infra.ExceptionHandling;

public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "[GlobalExceptionHandler][TryHandleAsync] Capturar e tratar exceção não tratada. ExceptionMessage={ExceptionMessage}", exception.Message);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var result = await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                Detail = $"{exception.GetType().FullName}: {exception.Message}{Environment.NewLine}{exception.StackTrace}"
            }
        });

        logger.LogError("[GlobalExceptionHandler][TryHandleAsync] Retornar Problem Details 500. Resultado={Resultado}", result);

        return result;
    }
}
