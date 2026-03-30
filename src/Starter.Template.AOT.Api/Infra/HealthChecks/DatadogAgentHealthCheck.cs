using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Starter.Template.AOT.Api.Infra.HealthChecks;

public class DatadogAgentHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DatadogAgentHealthCheck> _logger;

    public DatadogAgentHealthCheck(IHttpClientFactory httpClientFactory, ILogger<DatadogAgentHealthCheck> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[DatadogAgentHealthCheck][CheckHealthAsync] Verificar disponibilidade do Datadog Agent");

        try
        {
            var client = _httpClientFactory.CreateClient("datadog-agent");

            var response = await client.GetAsync("/info", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("[DatadogAgentHealthCheck][CheckHealthAsync] Retornar Healthy — Datadog Agent disponível");

                return HealthCheckResult.Healthy("Datadog Agent disponível");
            }

            _logger.LogWarning("[DatadogAgentHealthCheck][CheckHealthAsync] Retornar Degraded — Datadog Agent respondeu com status {StatusCode}", (int)response.StatusCode);

            return HealthCheckResult.Degraded($"Datadog Agent respondeu com status inesperado: {(int)response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[DatadogAgentHealthCheck][CheckHealthAsync] Retornar Unhealthy — Datadog Agent indisponível");

            return HealthCheckResult.Unhealthy("Datadog Agent indisponível", ex);
        }
    }
}
