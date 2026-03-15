# Visão Geral Técnica

## Propósito

Este arquivo descreve a visão arquitetural de alto nível deste repositório. É o ponto de entrada para entender a estrutura técnica do projeto, como os componentes se relacionam e quais decisões fundamentais já foram tomadas.

---

## Stack Tecnológica

| Camada | Tecnologia | Decisão |
|---|---|---|
| Linguagem principal | C# (.NET 10) | DA-004, DA-012 |
| Framework principal | ASP.NET Core — Controllers com Actions | DA-004, DA-008 |
| Logging estruturado | Serilog (com Enrich.FromLogContext) | DA-011 |
| Persistência | A definir por Feature | — |
| Mensageria | A definir | — |
| Containerização | A definir | — |
| CI/CD | A definir | — |
| Observabilidade (logging) | Serilog — Console estruturado + enrichment por request | DA-011, DP-004 parcial |
| Observabilidade (tracing) | A definir | DP-004 |
| Observabilidade (métricas) | A definir | DP-004 |

---

## Estilo Arquitetural

O projeto adota **Vertical Slice Architecture** com segregação explícita de operações de leitura (**Query**) e escrita (**Command**).

Cada funcionalidade é implementada como uma Slice vertical isolada, contendo todos os artefatos necessários (endpoint, use case, repository, models, interfaces, scripts SQL) dentro da sua própria pasta, sob `Features/Query` ou `Features/Command`.

Não há camadas horizontais globais (ex.: pasta `Services/` ou `Repositories/` global). Toda lógica especializada por funcionalidade reside dentro da Slice correspondente. Lógica genuinamente compartilhada entre Slices reside em `Shared/`.

*Referência: DA-004, DA-005 — `Instructions/architecture/architecture-decisions.md`*

---

## Componentes Principais

| Componente | Localização | Responsabilidade |
|---|---|---|
| Controllers com Actions | `Features/<tipo>/<Feature>/<Feature>Endpoint/` | Orquestração de request/response, logging de fluxo |
| Use Cases | `Features/<tipo>/<Feature>/<Feature>UseCase/` | Orquestração da lógica de negócio da Slice |
| Repositories | `Features/<tipo>/<Feature>/<Feature>Repository/` | Acesso a dados; materialização de objetos de domínio |
| Models (Input/Output/Entity) | `Features/<tipo>/<Feature>/<Feature>Models/` | Contratos de entrada, saída e entidades de domínio por Slice |
| Interfaces | `Features/<tipo>/<Feature>/<Feature>Interfaces/` | Contratos para repositórios e integrações externos ao UseCase |
| Shared | `Shared/` | Abstrações, utilitários, clientes e helpers reutilizáveis entre Slices |
| Exception Handler | `Infra/ExceptionHandling/GlobalExceptionHandler.cs` | Handler centralizado de exceções; retorna Problem Details (RFC 7807) |
| Correlation ID Middleware | `Infra/Middlewares/CorrelationIdMiddleware.cs` | Garante GUID v7 por request; enriquece logs via Serilog LogContext; completamente opaco para Features |
| GuidV7 | `Infra/Correlation/GuidV7.cs` | Utilitário de geração e validação de GUID v7 (uso interno da Infra) |

---

## Fronteiras e Responsabilidades

- **Features/Query**: Slices de leitura — não alteram estado.
- **Features/Command**: Slices de escrita — alteram estado.
- **Shared**: Recursos reutilizáveis sem lógica especializada para uma única Slice.
- **Infra**: Componentes de infraestrutura transversal (middlewares, exception handling, utilitários de infra). Features não dependem de Infra diretamente.
- Slices **não se comunicam diretamente entre si**. Lógica compartilhada vai para `Shared/`.
- **Correlation ID é opaco para Features e Endpoints** — enriquecido automaticamente pelo middleware no Serilog LogContext.

---

## Fluxos Principais de Alto Nível

```
Request HTTP
    └── CorrelationIdMiddleware (Infra/Middlewares — garante GUID v7; abre LogContext com CorrelationId)
            └── GlobalExceptionHandler (Infra/ExceptionHandling — captura exceções não tratadas)
                    └── Controller / Action (pasta Endpoint)
                            └── UseCase
                                    └── Repository (via Interface)
                                            └── Banco de dados / serviço externo

Serilog Enrichment (transversal):
    └── Todo log event dentro do using-scope do CorrelationIdMiddleware recebe { CorrelationId: <guid-v7> }
```

O Controller não contém lógica de negócio — apenas orquestra request/response, define status codes e escreve logs relevantes.

---

## Dependências Externas

| Pacote | Versão | Uso |
|---|---|---|
| `Serilog.AspNetCore` | latest | Logging estruturado com enrichment por request via LogContext |

---

## Restrições Técnicas Conhecidas

- Todo código deve compilar sem erros (`dotnet build`) antes de qualquer commit.
- Todos os testes devem passar sem erros antes de qualquer commit.
- Slices não podem depender de outras Slices diretamente.
- `Shared/` não pode depender de Features.
- Lógica de negócio não pode estar em Endpoints nem em Repositories.
- Validação de payload deve estar no objeto `Input` de cada Slice (em `<Feature>Models/`), não em repositórios ou componentes de persistência.

---

## Referências Cruzadas

- `Instructions/architecture/engineering-principles.md` — princípios que guiam as decisões
- `Instructions/architecture/patterns.md` — padrões adotados
- `Instructions/architecture/architecture-decisions.md` — decisões registradas
- `Instructions/architecture/folder-structure.md` — estrutura de pastas
- `Instructions/business/domain-model.md` — modelo de domínio relacionado

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| Bootstrap | Estrutura inicial criada | — |
| 2026-03-15 | Stack, arquitetura e componentes definidos | DA-004, DA-005 |
| 2026-03-15 | Framework HTTP atualizado: Minimal API substituída por Controllers com Actions | DA-008 |
| 2026-03-15 | GlobalExceptionHandler adicionado: handler centralizado de exceções em Shared/Middleware/ | DA-010, PAD-008 |
| 2026-03-15 | Runtime atualizado para .NET 10; Serilog adicionado; Infra/ criada; CorrelationIdMiddleware adicionado; GlobalExceptionHandler movido para Infra/ExceptionHandling/; DP-004 parcialmente resolvida | DA-011, DA-012 |
