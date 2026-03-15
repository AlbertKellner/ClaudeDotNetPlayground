using ClaudeDotNetPlayground.Infra.Correlation;
using Serilog.Context;

namespace ClaudeDotNetPlayground.Infra.Middlewares;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-Correlation-Id";
    internal const string HttpContextItemKey = "CorrelationId";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = ResolveCorrelationId(context);

        context.Items[HttpContextItemKey] = correlationId;
        context.Response.Headers[HeaderName] = correlationId.ToString();

        using (LogContext.PushProperty(HttpContextItemKey, correlationId))
        {
            await next(context);
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
