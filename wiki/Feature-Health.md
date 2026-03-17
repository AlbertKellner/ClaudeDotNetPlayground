# Health Check

## Resumo

Endpoint de verificação de disponibilidade da aplicação. Retorna o status de saúde da aplicação, indicando que ela está em execução e respondendo a requisições. É implementado pelo mecanismo nativo de HealthChecks do ASP.NET Core.

## Autenticação

**Não requer autenticação.** Este endpoint é acessível publicamente e é explicitamente excluído da proteção por Bearer Token. Consulte [Autenticação JWT](Infra-Authentication) para entender como a proteção funciona nos demais endpoints.

## Contrato de Entrada

| Campo | Valor |
|-------|-------|
| **Método** | `GET` |
| **Rota** | `/health` |
| **Headers** | Nenhum obrigatório |
| **Body** | Nenhum |

## Contrato de Saída

| Status | Corpo | Descrição |
|--------|-------|-----------|
| `200 OK` | `Healthy` | Aplicação e Datadog Agent disponíveis |
| `200 OK` | `Degraded` | Datadog Agent respondeu com status inesperado |
| `503 Service Unavailable` | `Unhealthy` | Datadog Agent inacessível |

## Comportamento

- Verifica a disponibilidade da aplicação e do Datadog Agent.
- Consulta o endpoint `/info` do APM Trace Agent do Datadog (porta 8126) via HTTP para confirmar que o agente está ativo.
- Retorna `HTTP 200 Healthy` somente quando ambos (aplicação e Datadog Agent) estão disponíveis.
- Retorna `HTTP 200 Degraded` quando o Datadog Agent responde com status HTTP inesperado.
- Retorna `HTTP 503 Unhealthy` quando o Datadog Agent está inacessível.
- Não exige autenticação — é exceção explícita à regra de proteção por Bearer Token.

Regras de negócio relacionadas: [RN-001](Business-Rules#rn-001---verificação-de-disponibilidade-da-aplicação), [RN-005](Business-Rules#rn-005---health-check-com-verificação-do-datadog-agent)

## Testes Automatizados

Nenhum teste automatizado presente no repositório.

## BDD

Nenhum cenário BDD definido para esta funcionalidade.
