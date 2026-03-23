# Integração com Datadog

## Visão Geral

O projeto envia logs ao Datadog por dois mecanismos complementares:

| Mecanismo | Como Funciona | Quando Usar |
|-----------|---------------|-------------|
| **Coleta via Docker Agent** | O Datadog Agent coleta logs de stdout dos containers Docker | Ambiente Docker (local e CI) |
| **Envio direto via HTTP** | `DatadogHttpSink` envia logs diretamente à API HTTP do Datadog | Quando habilitado via configuração |

---

## Mecanismo 1: Coleta via Datadog Agent (Docker)

### Como Funciona

O Datadog Agent roda como container adjacente à aplicação via `docker-compose`. Ele coleta automaticamente os logs de stdout de todos os containers Docker através do Docker socket.

### Configuração no docker-compose.yml

```yaml
services:
  app:
    build:
      context: src
      dockerfile: Albert.Playground.ECS.AOT.Api/Dockerfile
      args:
        EXTRA_CA_CERT: ${EXTRA_CA_CERT:-}
    ports:
      - "8080:8080"
    environment:
      - ExternalApi__GitHub__HttpRequest__PersonalAccessToken=${GITHUB_PAT:-}
    depends_on:
      - datadog-agent
    labels:
      com.datadoghq.ad.logs: '[{"source": "dotnet", "service": "albert-playground-ecs-aot-api"}]'

  datadog-agent:
    image: registry.datadoghq.com/agent:7
    container_name: dd-agent
    entrypoint: >
      /bin/bash -c "
        cat /custom-certs/proxy-ca.crt >> /etc/ssl/certs/cacert.pem &&
        exec /bin/entrypoint.sh
      "
    environment:
      - DD_API_KEY=${DD_API_KEY}
      - DD_SITE=datadoghq.com
      - DD_DOGSTATSD_NON_LOCAL_TRAFFIC=true
      - DD_ENV=${DD_ENV:-local}
      - DD_LOGS_ENABLED=true
      - DD_LOGS_CONFIG_CONTAINER_COLLECT_ALL=true
      - DD_HOSTNAME=albert-playground-ecs-aot-local
      - DD_CONVERT_DD_SITE_FQDN_ENABLED=false
      - DD_SYSTEM_PROBE_ENABLED=false
      - DD_PROCESS_AGENT_ENABLED=false
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock:ro
      - /proc/:/host/proc/:ro
      - /sys/fs/cgroup/:/host/sys/fs/cgroup:ro
      - /var/lib/docker/containers:/var/lib/docker/containers:ro
      - /usr/local/share/ca-certificates/swp-ca-production.crt:/custom-certs/proxy-ca.crt:ro
```

### Variáveis de Ambiente do Datadog Agent

| Variável | Valor | Propósito |
|----------|-------|-----------|
| `DD_API_KEY` | Secret do ambiente | Autenticação com Datadog |
| `DD_SITE` | `datadoghq.com` | Região do Datadog |
| `DD_ENV` | `${DD_ENV:-local}` | Tag de ambiente para filtragem (`local`, `ci`, `build`) |
| `DD_HOSTNAME` | `albert-playground-ecs-aot-local` | Hostname fixo para evitar erro de detecção em sandbox |
| `DD_LOGS_ENABLED` | `true` | Ativa coleta de logs |
| `DD_LOGS_CONFIG_CONTAINER_COLLECT_ALL` | `true` | Coleta logs de todos os containers |
| `DD_CONVERT_DD_SITE_FQDN_ENABLED` | `false` | Desabilita trailing dot no FQDN (compatibilidade com proxy) |
| `DD_DOGSTATSD_NON_LOCAL_TRAFFIC` | `true` | Aceita métricas DogStatsD de outros containers |

### Labels de Autodiscovery

A label `com.datadoghq.ad.logs` na aplicação configura o Datadog Agent para classificar os logs:

```yaml
labels:
  com.datadoghq.ad.logs: '[{"source": "dotnet", "service": "albert-playground-ecs-aot-api"}]'
```

- `source: "dotnet"` — pipeline de processamento de logs .NET no Datadog
- `service: "albert-playground-ecs-aot-api"` — nome do serviço para filtragem nos dashboards

### DD_ENV por Contexto de Execução

O valor de `DD_ENV` varia por contexto, permitindo filtragem nos dashboards:

| Contexto | DD_ENV | Onde |
|----------|--------|------|
| Desenvolvimento local | `local` | `docker-compose.yml` (default) |
| CI/CD — job de build | `build` | `.github/workflows/ci.yml` |
| CI/CD — jobs de execução | `ci` | `.github/workflows/ci.yml` |

---

## Mecanismo 2: Envio Direto via HTTP (DatadogHttpSink)

### Quando É Ativado

O `DatadogHttpSink` é ativado condicionalmente em `Program.cs` quando **ambas** as condições são verdadeiras:

1. A variável de ambiente `DD_API_KEY` está definida
2. A configuração `Datadog:DirectLogs` é `true` no `appsettings.json`

```csharp
var ddApiKey = Environment.GetEnvironmentVariable("DD_API_KEY");
var ddDirectLogs = ctx.Configuration.GetValue<bool>("Datadog:DirectLogs", false);

if (!string.IsNullOrEmpty(ddApiKey) && ddDirectLogs)
{
    var ddEnv = Environment.GetEnvironmentVariable("DD_ENV") ?? "local";
    var ddHost = Environment.GetEnvironmentVariable("DD_HOSTNAME") ?? Environment.MachineName;

    config.WriteTo.Sink(new DatadogHttpSink(
        apiKey: ddApiKey,
        service: "albert-playground-ecs-aot-api",
        host: ddHost,
        env: ddEnv));

    Log.Information(
        "[Program] Datadog HTTP Sink ativado — logs enviados diretamente ao Datadog. Env={Env}, Host={Host}",
        ddEnv, ddHost);
}
```

### Arquitetura do DatadogHttpSink

O sink utiliza `System.Threading.Channels` para processamento assíncrono em batch, sem bloquear a thread da aplicação:

```
Aplicação → Emit(log) → Channel (buffer de 1000) → ProcessBatchAsync → HTTP POST → Datadog
                              ↑                          ↓
                        DropOldest se cheio         Batches de 50 entradas
```

### Implementação Completa

```csharp
internal sealed class DatadogHttpSink : ILogEventSink, IAsyncDisposable
{
    private readonly HttpClient _httpClient;
    private readonly Channel<DatadogLogEntry> _channel;
    private readonly Task _processTask;
    private readonly CancellationTokenSource _cts = new();
    private readonly string _service;
    private readonly string _host;
    private readonly string _env;

    public DatadogHttpSink(string apiKey, string service, string host, string env)
    {
        _service = service;
        _host = host;
        _env = env;

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://http-intake.logs.datadoghq.com"),
            DefaultRequestHeaders = { { "DD-API-KEY", apiKey } }
        };

        _channel = Channel.CreateBounded<DatadogLogEntry>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true
        });

        _processTask = Task.Run(ProcessBatchAsync);
    }

    public void Emit(LogEvent logEvent)
    {
        var entry = new DatadogLogEntry
        {
            Message = logEvent.RenderMessage(),
            Timestamp = logEvent.Timestamp.ToUnixTimeMilliseconds(),
            Level = logEvent.Level.ToString().ToLowerInvariant(),
            Service = _service,
            Host = _host,
            DdTags = $"env:{_env}"
        };

        _channel.Writer.TryWrite(entry);
    }

    private async Task ProcessBatchAsync()
    {
        var batch = new List<DatadogLogEntry>(50);

        while (!_cts.Token.IsCancellationRequested)
        {
            try
            {
                batch.Clear();

                if (await _channel.Reader.WaitToReadAsync(_cts.Token))
                {
                    while (batch.Count < 50 && _channel.Reader.TryRead(out var entry))
                    {
                        batch.Add(entry);
                    }

                    if (batch.Count > 0)
                        await SendBatchAsync(batch);
                }
            }
            catch (OperationCanceledException) { break; }
            catch { await Task.Delay(5000, _cts.Token); }
        }

        // Drena entradas remanescentes no shutdown
        while (_channel.Reader.TryRead(out var remaining))
            batch.Add(remaining);

        if (batch.Count > 0)
            await SendBatchAsync(batch);
    }

    private async Task SendBatchAsync(List<DatadogLogEntry> batch)
    {
        var json = JsonSerializer.Serialize(batch, DatadogLogJsonContext.Default.ListDatadogLogEntry);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        await _httpClient.PostAsync("/api/v2/logs", content);
    }

    public async ValueTask DisposeAsync()
    {
        _channel.Writer.Complete();
        await _cts.CancelAsync();

        try { await _processTask.WaitAsync(TimeSpan.FromSeconds(5)); }
        catch (TimeoutException) { }
        catch (OperationCanceledException) { }

        _cts.Dispose();
        _httpClient.Dispose();
    }
}
```

### Modelo de Log para o Datadog

O `DatadogLogEntry` é serializado com `JsonSerializerContext` source-generated para compatibilidade com Native AOT:

```csharp
internal sealed class DatadogLogEntry
{
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; init; }

    [JsonPropertyName("level")]
    public string Level { get; init; } = string.Empty;

    [JsonPropertyName("service")]
    public string Service { get; init; } = string.Empty;

    [JsonPropertyName("host")]
    public string Host { get; init; } = string.Empty;

    [JsonPropertyName("ddtags")]
    public string DdTags { get; init; } = string.Empty;
}

[JsonSerializable(typeof(DatadogLogEntry))]
[JsonSerializable(typeof(List<DatadogLogEntry>))]
internal sealed partial class DatadogLogJsonContext : JsonSerializerContext { }
```

### JSON Enviado ao Datadog

Cada batch gera um POST para `https://http-intake.logs.datadoghq.com/api/v2/logs`:

```json
[
  {
    "message": "[PokemonGetEndpoint][Get] Processar requisicao GET /pokemon/25",
    "timestamp": 1711198321123,
    "level": "information",
    "service": "albert-playground-ecs-aot-api",
    "host": "albert-playground-ecs-aot-local",
    "ddtags": "env:local"
  },
  {
    "message": "[PokemonGetUseCase][ExecuteAsync] Executar caso de uso de consulta de Pokemon. PokemonId=25",
    "timestamp": 1711198321124,
    "level": "information",
    "service": "albert-playground-ecs-aot-api",
    "host": "albert-playground-ecs-aot-local",
    "ddtags": "env:local"
  }
]
```

---

## Configuração por Environment (appsettings.json)

### appsettings.json (base)

```json
{
  "Datadog": {
    "AgentUrl": "http://datadog-agent:8126",
    "DirectLogs": false
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning",
        "System": "Warning"
      }
    }
  }
}
```

### Campos de Configuração

| Seção | Campo | Valor | Propósito |
|-------|-------|-------|-----------|
| `Datadog` | `AgentUrl` | `http://datadog-agent:8126` | URL do Datadog Agent na rede Docker |
| `Datadog` | `DirectLogs` | `false` | Habilita/desabilita o DatadogHttpSink |
| `Serilog` | `MinimumLevel:Default` | `Information` | Nível mínimo de log |
| `Serilog` | `MinimumLevel:Override:Microsoft` | `Warning` | Suprime logs verbosos do ASP.NET Core |

### Exemplo: Ativando Envio Direto ao Datadog

Para ativar o `DatadogHttpSink` em um environment específico, crie `appsettings.Production.json`:

```json
{
  "Datadog": {
    "DirectLogs": true
  }
}
```

Ou via variável de ambiente:

```bash
Datadog__DirectLogs=true
```

### Exemplo: Alterando Nível de Log por Environment

Para debug em ambiente de staging (`appsettings.Staging.json`):

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Information"
      }
    }
  }
}
```

---

## Health Check do Datadog Agent

O endpoint `/health` da aplicação inclui verificação do Datadog Agent via HTTP:

| Cenário | Status HTTP | Corpo |
|---------|-------------|-------|
| App + Agent OK | 200 | `Healthy` |
| App OK + Agent com status inesperado | 200 | `Degraded` |
| App OK + Agent inacessível | 503 | `Unhealthy` |

A implementação está em `Infra/HealthChecks/DatadogAgentHealthCheck.cs`.

---

## Variáveis de Ambiente Necessárias

| Variável | Onde Definir | Obrigatória | Propósito |
|----------|-------------|-------------|-----------|
| `DD_API_KEY` | `.env` / GitHub Secrets | Sim (para Datadog) | Autenticação com Datadog |
| `DD_ENV` | `.env` / CI | Não (default: `local`) | Tag de ambiente |
| `DD_HOSTNAME` | `docker-compose.yml` | Sim (em sandbox) | Hostname fixo |
| `DD_APP_KEY` | Variável de ambiente | Para MCP apenas | Acesso ao Datadog MCP |

---

## Filtragem no Datadog

Com o padrão de logging adotado, as seguintes queries são eficientes no Datadog:

```
# Todos os logs de um serviço
service:albert-playground-ecs-aot-api

# Filtrar por environment
service:albert-playground-ecs-aot-api env:ci

# Filtrar por classe específica
service:albert-playground-ecs-aot-api *PokemonGetEndpoint*

# Filtrar por método específico
service:albert-playground-ecs-aot-api *ExecuteAsync*

# Filtrar por nível de log
service:albert-playground-ecs-aot-api status:warn

# Combinação: erros em produção de um endpoint
service:albert-playground-ecs-aot-api env:local status:error *WeatherConditions*
```

---

## Referências

- [01-padroes-de-logging.md](01-padroes-de-logging.md) — padrão de formato dos logs
- [02-enriquecimento-de-contexto.md](02-enriquecimento-de-contexto.md) — CorrelationId e campos de usuário
- `Infra/Logging/DatadogHttpSink.cs` — implementação do sink HTTP
- `Infra/Logging/DatadogLogEntry.cs` — modelo de log para Datadog
- `Infra/HealthChecks/DatadogAgentHealthCheck.cs` — health check do Agent
- `docker-compose.yml` — configuração dos containers
- `scripts/operational-runbook.md` — guia operacional com troubleshooting
