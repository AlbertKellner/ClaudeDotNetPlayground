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
| Integração HTTP externa | Refit (`Refit.HttpClientFactory`) — interfaces decoradas com atributos HTTP; implementação source-generated | DA-017 |
| Resiliência HTTP | Polly v8 via `Microsoft.Extensions.Http.Resilience` — retry exponencial + timeout por tentativa | DA-017 |
| Persistência | A definir por Feature | — |
| Mensageria | A definir | — |
| Containerização | Docker — Dockerfile multi-stage (Native AOT) + docker-compose com Datadog Agent | DA-016 |
| CI/CD | GitHub Actions — workflows: build (Native AOT), run, healthcheck, docker-build, pr-language-check, wiki-publish; GitHub Environment: ClaudeCode | — |
| Observabilidade (logging) | Serilog — Console colorido (AnsiConsoleTheme.Code) + storytelling por classe/método + enrichment por request | DA-011, DA-015, DP-004 parcial |
| Observabilidade (tracing) | A definir | DP-004 |
| Observabilidade (métricas) | Datadog Agent (Docker) — métricas de container e host via Docker socket; DogStatsD para métricas customizadas; filtros por env: build, ci, local | DA-016, DP-004 parcial |

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
| IOpenMeteoApi | `Shared/ExternalApi/OpenMeteo/IOpenMeteoApi.cs` | Interface Refit para a API Open-Meteo; contrato HTTP com rota `/v1/forecast` hardcoded |
| IOpenMeteoApiClient | `Shared/ExternalApi/OpenMeteo/IOpenMeteoApiClient.cs` | Interface de serviço; contrato que Features injetam via DI |
| OpenMeteoApiClient | `Shared/ExternalApi/OpenMeteo/OpenMeteoApiClient.cs` | Implementa IOpenMeteoApiClient; usa IOpenMeteoApi (Refit + Polly via HttpClient); aplica logging SNP-001 |
| OpenMeteoInput/Output | `Shared/ExternalApi/OpenMeteo/Models/` | Modelos de entrada (coordenadas + fields) e saída completa da Open-Meteo; inclui OpenMeteoJsonContext para AOT |
| Exception Handler | `Infra/ExceptionHandling/GlobalExceptionHandler.cs` | Handler centralizado de exceções; retorna Problem Details (RFC 7807) |
| Correlation ID Middleware | `Infra/Middlewares/CorrelationIdMiddleware.cs` | Garante GUID v7 por request; enriquece logs via Serilog LogContext; completamente opaco para Features |
| GuidV7 | `Infra/Correlation/GuidV7.cs` | Utilitário de geração e validação de GUID v7 (uso interno da Infra) |
| ITokenService | `Infra/Security/ITokenService.cs` | Contrato de geração e validação de JWT Bearer Token |
| AuthenticatedUser | `Infra/Security/AuthenticatedUser.cs` | Modelo de usuário autenticado extraído do token (Id, UserName) |
| TokenService | `Infra/Security/TokenService.cs` | Implementação JWT HS256: geração e validação de Bearer Token |
| AuthenticateFilter | `Infra/Security/AuthenticateFilter.cs` | IAsyncActionFilter: valida Bearer Token, retorna 401 se inválido, enriquece logs com UserId e UserName |
| AuthenticateAttribute | `Infra/Security/AuthenticateAttribute.cs` | TypeFilterAttribute: decorador aplicado nos Controllers para ativar AuthenticateFilter via DI |

---

## Fronteiras e Responsabilidades

- **Features/Query**: Slices de leitura — não alteram estado.
- **Features/Command**: Slices de escrita — alteram estado.
- **Shared**: Recursos reutilizáveis sem lógica especializada para uma única Slice.
- **Infra**: Componentes de infraestrutura transversal (middlewares, exception handling, utilitários de infra). Features não dependem de Infra diretamente.
- Slices **não se comunicam diretamente entre si**. Lógica compartilhada vai para `Shared/`.
- **Correlation ID é opaco para Features e Endpoints** — enriquecido automaticamente pelo middleware no Serilog LogContext.
- **UserId e UserName são opacos para Features e Endpoints** — enriquecidos automaticamente pelo `AuthenticateFilter` no Serilog LogContext quando o token é válido. Endpoints apenas aplicam `[Authenticate]`; nenhuma lógica de autenticação é visível nas Features.

---

## Fluxos Principais de Alto Nível

```
Request HTTP
    └── CorrelationIdMiddleware (Infra/Middlewares — garante GUID v7; abre LogContext com CorrelationId)
            └── GlobalExceptionHandler (Infra/ExceptionHandling — captura exceções não tratadas)
                    └── Controller / Action (pasta Endpoint)
                            ├── [sem Authenticate] POST /login → UserLoginEndpoint → UserLoginUseCase → ITokenService
                            └── [com Authenticate] outros endpoints → AuthenticateFilter (valida JWT; enriquece LogContext)
                                    └── UseCase
                                            └── Repository (via Interface)
                                                    └── Banco de dados / serviço externo

Serilog Enrichment (transversal):
    └── Todo log event dentro do using-scope do CorrelationIdMiddleware recebe { CorrelationId: <guid-v7> }
    └── Todo log event dentro do using-scope do AuthenticateFilter (endpoints protegidos) recebe { UserId: <int>, UserName: <string> }
```

O Controller não contém lógica de negócio — apenas orquestra request/response, define status codes e escreve logs relevantes.

---

## Dependências Externas

| Pacote | Versão | Uso |
|---|---|---|
| `Serilog.AspNetCore` | latest | Logging estruturado com enrichment por request via LogContext |
| `Serilog.Sinks.Console` | latest | Console sink com suporte a temas ANSI coloridos (AnsiConsoleTheme.Code) | DA-015 |
| `System.IdentityModel.Tokens.Jwt` | latest | Geração e validação de JWT HS256 para Bearer Token | DA-013 |
| `Refit.HttpClientFactory` | latest | Clientes HTTP fortemente tipados via interfaces decoradas com atributos; source-generated (AOT-compatível) | DA-017 |
| `Microsoft.Extensions.Http.Resilience` | latest | Resiliência HTTP via Polly v8: retry exponencial + timeout por tentativa, integrado ao IHttpClientBuilder | DA-017 |

---

## Restrições Técnicas Conhecidas

- Todo código deve compilar sem erros (`dotnet build`) antes de qualquer commit.
- Todos os testes devem passar sem erros antes de qualquer commit.
- A aplicação deve ser iniciada via `docker compose up -d` e responder HTTP 200 em `/health` antes de qualquer commit.
- O Datadog Agent deve estar ativo durante a execução de validação pré-commit para que logs fluam ao Datadog.
- `docker compose down` deve ser executado após a validação pré-commit.
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
| 2026-03-15 | Infra/Security/ criada: ITokenService, AuthenticatedUser, TokenService, AuthenticateFilter, AuthenticateAttribute; JWT Bearer Token adicionado; enriquecimento de logs com UserId e UserName | DA-013, RN-002, RN-003 |
| 2026-03-15 | CI/CD definido: GitHub Actions com três workflows encadeados via workflow_run — build (Native AOT), run e healthcheck | — |
| 2026-03-15 | CI/CD expandido: workflow pr-language-check adicionado — valida título e corpo de PRs; template de PR em português criado | DA-014 |
| 2026-03-15 | Padrões de logging definidos: formato `[Classe][Método]`, storytelling, console colorido ANSI, template com timestamp/correlationId/userName, isolamento visual, testes de log | DA-015, SNP-001 |
| 2026-03-16 | Containerização adicionada: Dockerfile multi-stage (Native AOT) + docker-compose com Datadog Agent; GitHub Environment ClaudeCode; DD_ENV por contexto (build, ci, local) para filtragem no Datadog | DA-016 |
| 2026-03-16 | Integração HTTP externa adicionada: Refit + Polly; Shared/ExternalApi/OpenMeteo/ criada; Feature WeatherConditionsGet implementada; RN-004 | DA-017, RN-004 |
