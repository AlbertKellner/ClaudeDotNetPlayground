# Health Check

## Descrição

Documenta o endpoint de verificação de disponibilidade da aplicação (`GET /health`). Esta página cobre o contrato de entrada e saída, o comportamento esperado incluindo verificação do Datadog Agent, e a regra de negócio associada (RN-005). Consulte quando precisar entender o mecanismo de health check ou diagnosticar problemas de disponibilidade. Relaciona-se com [Segurança](Governance-Security) (exceção à autenticação) e [Observabilidade](Governance-Observability) (Datadog Agent).

## Resumo

Endpoint de verificação de disponibilidade da aplicação. Retorna o status de saúde da aplicação, indicando que ela está em execução e respondendo a requisições. É implementado pelo mecanismo nativo de HealthChecks do ASP.NET Core.

## Autenticação

**Não requer autenticação.** Este endpoint é acessível publicamente e é explicitamente excluído da proteção por Bearer Token. Consulte [Segurança](Governance-Security) para entender como a proteção funciona nos demais endpoints.

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

Regras de negócio relacionadas: [RN-005](Domain-Business-Rules#rn-005--health-check-com-verificação-do-datadog-agent)

## Testes Automatizados

Nenhum teste automatizado presente no repositório.

## BDD

Nenhum cenário BDD definido para esta funcionalidade.

## Referências

- [Regras de Negócio](Domain-Business-Rules) — RN-005
- [Segurança](Governance-Security) — exceção à autenticação por Bearer Token
- [Observabilidade](Governance-Observability) — integração com Datadog Agent
- [Arquitetura](Governance-Architecture) — posição no fluxo de request
