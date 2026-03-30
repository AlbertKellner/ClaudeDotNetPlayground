using Starter.Template.AOT.Api.Infra.Correlation;
using Serilog.Context;

namespace Starter.Template.AOT.Api.Infra.Middlewares;

public sealed class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
{
    public const string HeaderName = "X-Correlation-Id";
    internal const string HttpContextItemKey = "CorrelationId";

    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogInformation("[CorrelationIdMiddleware][InvokeAsync] Processar requisição e garantir CorrelationId");

        var correlationId = ResolveCorrelationId(context);

        context.Items[HttpContextItemKey] = correlationId;
        context.Response.Headers[HeaderName] = correlationId.ToString();

        using (LogContext.PushProperty(HttpContextItemKey, correlationId))
        {
            logger.LogInformation("[CorrelationIdMiddleware][InvokeAsync] Prosseguir com CorrelationId enriquecido no contexto. CorrelationId={CorrelationId}", correlationId);

            await next(context);

            logger.LogInformation("[CorrelationIdMiddleware][InvokeAsync] Retornar resposta com CorrelationId enriquecido. CorrelationId={CorrelationId}", correlationId);
        }
    }

    private static Guid ResolveCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderName, out var headerValue)
            && Guid.TryParse(headerValue, out var parsed)
            && GuidV7.IsVersion7(parsed))
        {
            return parsed;
        }

        return GuidV7.Create();
    }
}
