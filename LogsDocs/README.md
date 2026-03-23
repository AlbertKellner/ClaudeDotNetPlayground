# Documentação de Logging

Esta documentação detalha o sistema de logging estruturado adotado neste repositório, cobrindo padrões, implementação, integração com Datadog e estratégia de testes.

---

## Sumário de Documentos

| Documento | Conteúdo |
|-----------|----------|
| [01-padroes-de-logging.md](01-padroes-de-logging.md) | Padrão Storytelling, formato de prefixo, regras de escrita, ganhos e benefícios |
| [02-enriquecimento-de-contexto.md](02-enriquecimento-de-contexto.md) | CorrelationId, UserId, UserName — como o contexto é injetado automaticamente nos logs |
| [03-exemplo-requisicao-completa.md](03-exemplo-requisicao-completa.md) | Exemplo ponta a ponta de uma requisição real com logs de cada camada |
| [04-integracao-datadog.md](04-integracao-datadog.md) | DatadogHttpSink, configuração por environment, docker-compose e appsettings.json |
| [05-testes-de-logging.md](05-testes-de-logging.md) | FakeLogger, padrão de asserção, exemplos de testes de log |
| [06-prompt-implementacao-logs.md](06-prompt-implementacao-logs.md) | Prompt único e genérico para implementar boas práticas de logging em qualquer repositório |

---

## Referências de Governança

- `Instructions/snippets/canonical-snippets.md` — SNP-001: snippet normativo do padrão de logging
- `Instructions/architecture/architecture-decisions.md` — DA-015: decisão arquitetural do padrão de logging
- `Instructions/architecture/technical-overview.md` — visão geral da stack de observabilidade
- `.claude/rules/endpoint-validation.md` — política de exibição de logs de storytelling na validação
