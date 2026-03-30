# Observabilidade

## Descrição

Documenta a estratégia de observabilidade: Correlation ID para rastreamento de requisições, Serilog para logging estruturado, Datadog Agent para métricas de container. Deve ser consultado ao entender logging, tracing ou monitoramento.

## Contexto

A observabilidade é implementada em três frentes: rastreamento por requisição via Correlation ID (GUID v7), logging estruturado via Serilog com padrão storytelling (SNP-001) e métricas de infraestrutura via Datadog Agent em Docker. O APM (traces de aplicação) não está ativo nesta configuração.

---

## Correlation ID

### Funcionamento

Cada requisição HTTP recebe um GUID v7 único, garantido pelo `CorrelationIdMiddleware`. Este identificador permite rastrear todas as operações de uma mesma requisição nos logs.

### Resolução do Correlation ID

| Condição | Comportamento |
|---|---|
| Header `X-Correlation-Id` presente com GUID v7 válido | Reutiliza o valor recebido |
| Header ausente ou GUID inválido | Gera novo GUID v7 via `Guid.CreateVersion7()` |

### Propagação

- **Response header**: o valor é retornado no header `X-Correlation-Id` da resposta
- **Logs**: todos os logs dentro do scope da requisição são enriquecidos via `Serilog.Context.LogContext` com `{ CorrelationId: <guid-v7> }`
- **Transparência**: o Correlation ID é completamente opaco para Features e Endpoints — nenhum código de Feature precisa lidar com ele

### Componentes

| Componente | Localização | Responsabilidade |
|---|---|---|
| `CorrelationIdMiddleware` | `Infra/Middlewares/` | Garante GUID v7 por request; enriquece Serilog LogContext; adiciona header de resposta |
| `GuidV7` | `Infra/Correlation/` | Utilitário de geração (`Guid.CreateVersion7()`) e validação de GUID v7 (uso interno de Infra) |

### Posição no Pipeline

O `CorrelationIdMiddleware` é registrado **antes** de `UseExceptionHandler()` no pipeline, garantindo que mesmo exceções não tratadas tenham o Correlation ID nos logs.

---

## Logging Estruturado (Serilog)

### Configuração

| Item | Valor |
|---|---|
| Framework | Serilog com `Serilog.AspNetCore` |
| Console Sink | `Serilog.Sinks.Console` com `AnsiConsoleTheme.Code` |
| Enrichment | `Enrich.FromLogContext` (CorrelationId, UserId, UserName) |

### Template de Console

```
[{Timestamp:dd/MM/yyyy HH:mm:ss.fffffff}] [{CorrelationId}] [{UserName}] {Message:lj}{NewLine}{Exception}
```

Exemplo de saída:
```
[22/03/2026 14:30:45.1234567] [01960a1b-...] [User] [ExemploEndpoint][Get] Processar requisição
```

### Padrão SNP-001 — Storytelling por Classe e Método

Todo log segue o formato obrigatório `[NomeDaClasse][NomeDoMétodo] DescriçãoBreve` em linguagem imperativa:

- **Log de entrada**: o que será executado + parâmetros recebidos
- **Log de saída**: o que está sendo retornado
- **Log antes de loop**: o que será iterado
- **Log após loop**: iteração concluída
- **Isolamento visual**: linha em branco acima e abaixo de cada instrução `logger.Log*()`

### Datadog HTTP Sink

| Componente | Localização | Responsabilidade |
|---|---|---|
| `DatadogHttpSink` | `Infra/Logging/` | Serilog `ILogEventSink` customizado: envia logs diretamente ao Datadog via HTTP (`/api/v2/logs`); batching assíncrono via `Channel` |
| `DatadogLogEntry` | `Infra/Logging/` | Modelo de entrada de log para o Datadog + `DatadogLogJsonContext` para serialização AOT-compatível |

**Funcionamento do batching**:
- Utiliza `System.Threading.Channels.Channel` como fila assíncrona produtor-consumidor
- Eventos de log são enfileirados no channel pelo método `Emit` (non-blocking)
- Uma task em background consome o channel, agrupa eventos em batches e envia ao Datadog via HTTP POST para `/api/v2/logs`
- Serialização AOT-compatível via `DatadogLogJsonContext`

**Ativação**: o sink é ativado condicionalmente pela configuração `Datadog:DirectLogs` em `appsettings.json` (padrão: `false`). Quando desativado, os logs fluem apenas pelo Serilog Console Sink e são coletados pelo Datadog Agent via Docker log collection.

---

## Datadog Agent (Docker)

### Funcionamento

O Datadog Agent é executado como container adjacente via `docker-compose`, coletando métricas de container e host via Docker socket e DogStatsD. **Não há APM** — traces de aplicação não estão disponíveis nesta configuração.

### Contextos de Ambiente

| Contexto | `DD_ENV` | Quando |
|---|---|---|
| Local | `local` | Execução via `docker compose` no ambiente de desenvolvimento |
| CI | `ci` | Execução no pipeline de GitHub Actions |
| Build | `build` | Durante o job de build no CI |

### Configuração do Datadog Agent

| Variável | Valor | Propósito |
|---|---|---|
| `DD_API_KEY` | Secret do ambiente | Autenticação com Datadog |
| `DD_SITE` | `datadoghq.com` | Região do Datadog |
| `DD_ENV` | `${DD_ENV:-local}` | Ambiente para filtragem nos dashboards |
| `DD_HOSTNAME` | Configurável por projeto | Hostname fixo para evitar erro de detecção automática |
| `DD_LOGS_ENABLED` | `true` | Coleta de logs ativa |
| `DD_LOGS_CONFIG_CONTAINER_COLLECT_ALL` | `true` | Coleta de logs de todos os containers |
| `DD_CONVERT_DD_SITE_FQDN_ENABLED` | `false` | Desabilita FQDN com trailing dot (incompatível com proxy) |
| `DD_DOGSTATSD_NON_LOCAL_TRAFFIC` | `true` | Aceita métricas DogStatsD de outros containers |

### Health Check com Datadog Agent

O endpoint `GET /health` verifica a disponibilidade do Datadog Agent além da própria aplicação (RN-005):

| Estado do Datadog Agent | Resposta do Health Check |
|---|---|
| Acessível e respondendo | HTTP 200 — `Healthy` |
| Respondendo com status inesperado | HTTP 200 — `Degraded` |
| Inacessível | HTTP 503 — `Unhealthy` |

O componente `DatadogAgentHealthCheck` em `Infra/HealthChecks/` implementa essa verificação via HTTP.

### Containers Docker

| Serviço | Imagem | Portas |
|---|---|---|
| `app` | Build local via Dockerfile | `8080:8080` (host) |
| `datadog-agent` | `registry.datadoghq.com/agent:7` | Nenhuma exposta ao host (rede interna Docker) |

---

## Enrichment Transversal

O Serilog LogContext é enriquecido automaticamente em dois pontos do pipeline:

| Middleware/Filter | Propriedades Adicionadas | Escopo |
|---|---|---|
| `CorrelationIdMiddleware` | `CorrelationId` | Toda requisição |
| `AuthenticateFilter` | `UserId`, `UserName` | Requisições a endpoints protegidos com `[Authenticate]` |

Ambos os enrichments são transparentes para Features — nenhum código de aplicação precisa interagir diretamente com o LogContext.

---

## Referências

- [Arquitetura](Governance-Architecture) — fluxo de requisição com middlewares de observabilidade
- [Operação](Governance-Operation) — portas, URLs e configuração de containers
- [Convenções de Código](Governance-Code-Conventions) — padrão de logging SNP-001
