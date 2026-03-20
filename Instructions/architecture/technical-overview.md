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
| Memory Cache | `IMemoryCache` (`Microsoft.Extensions.Caching.Memory`) — cache por usuário autenticado; duração e expiração configuráveis via `appsettings.json` seção `EndpointCache` | DA-018 |
| Integração GitHub API | Refit (`Refit.HttpClientFactory`) — interface para GitHub REST API v3; autenticação via `DelegatingHandler` com PAT; resiliência Polly v8 | DA-019 |
| Integração PokeAPI | Refit (`Refit.HttpClientFactory`) — interface para PokeAPI REST v2; resiliência Polly v8 | DA-021 |
| Persistência em arquivo | Arquivo JSON local — `RepositoryFileEntry` com path configurável via `appsettings.json` seção `Repositories` | DA-019 |
| Persistência | A definir por Feature | — |
| Mensageria | A definir | — |
| Containerização | Docker — Dockerfile multi-stage (Native AOT) + docker-compose com Datadog Agent | DA-016 |
| CI/CD | GitHub Actions — workflows: Validar Execução (`ci.yml`: Compilação → Execução → unit-tests → Validar Health Check (Debug) \|\| Validar Health Check (Publish)), wiki-publish; GitHub Environment: ClaudeCode | — |
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
| CachedOpenMeteoApiClient | `Shared/ExternalApi/OpenMeteo/CachedOpenMeteoApiClient.cs` | Decorator de IOpenMeteoApiClient; implementa Memory Cache por usuário autenticado com duração configurável; chave de cache definida no código |
| OpenMeteoInput/Output | `Shared/ExternalApi/OpenMeteo/Models/` | Modelos de entrada (coordenadas + fields) e saída completa da Open-Meteo; inclui OpenMeteoJsonContext para AOT |
| IGitHubApi | `Shared/ExternalApi/GitHub/IGitHubApi.cs` | Interface Refit para a API GitHub; contrato HTTP com rota `/orgs/{org}/teams/{teamSlug}/repos` |
| IGitHubApiClient | `Shared/ExternalApi/GitHub/IGitHubApiClient.cs` | Interface de serviço; contrato que Features injetam via DI |
| GitHubApiClient | `Shared/ExternalApi/GitHub/GitHubApiClient.cs` | Implementa IGitHubApiClient; usa IGitHubApi (Refit + Polly via HttpClient); aplica logging SNP-001 |
| GitHubAuthenticationHandler | `Shared/ExternalApi/GitHub/GitHubAuthenticationHandler.cs` | DelegatingHandler: injeta Bearer PAT, User-Agent e Accept no pipeline HTTP |
| GitHubRepositoryOutput | `Shared/ExternalApi/GitHub/Models/GitHubRepositoryOutput.cs` | Modelo de resposta da API GitHub + GitHubJsonContext para AOT |
| IPokeApi | `Shared/ExternalApi/PokeApi/IPokeApi.cs` | Interface Refit para a PokeAPI; contrato HTTP com rota `/api/v2/pokemon/{name}` |
| IPokeApiClient | `Shared/ExternalApi/PokeApi/IPokeApiClient.cs` | Interface de serviço; contrato que Features injetam via DI |
| PokeApiClient | `Shared/ExternalApi/PokeApi/PokeApiClient.cs` | Implementa IPokeApiClient; usa IPokeApi (Refit + Polly via HttpClient); aplica logging SNP-001 |
| PokeApiOutput | `Shared/ExternalApi/PokeApi/Models/PokeApiOutput.cs` | Modelo de resposta da PokeAPI com perfil essencial do Pokemon + PokeApiJsonContext para AOT |
| RepositoryFileEntry | `Shared/Repositories/RepositoryFileEntry.cs` | Modelo compartilhado do arquivo JSON de repositórios + RepositoryFileJsonContext para AOT |
| Exception Handler | `Infra/ExceptionHandling/GlobalExceptionHandler.cs` | Handler centralizado de exceções; retorna Problem Details (RFC 7807) |
| Correlation ID Middleware | `Infra/Middlewares/CorrelationIdMiddleware.cs` | Garante GUID v7 por request; enriquece logs via Serilog LogContext; completamente opaco para Features |
| GuidV7 | `Infra/Correlation/GuidV7.cs` | Utilitário de geração e validação de GUID v7 (uso interno da Infra) |
| ITokenService | `Infra/Security/ITokenService.cs` | Contrato de geração e validação de JWT Bearer Token |
| AuthenticatedUser | `Infra/Security/AuthenticatedUser.cs` | Modelo de usuário autenticado extraído do token (Id, UserName) |
| TokenService | `Infra/Security/TokenService.cs` | Implementação JWT HS256: geração e validação de Bearer Token |
| AuthenticateFilter | `Infra/Security/AuthenticateFilter.cs` | IAsyncActionFilter: valida Bearer Token, retorna 401 se inválido, enriquece logs com UserId e UserName, armazena AuthenticatedUser em HttpContext.Items para acesso por camadas downstream |
| AuthenticateAttribute | `Infra/Security/AuthenticateAttribute.cs` | TypeFilterAttribute: decorador aplicado nos Controllers para ativar AuthenticateFilter via DI |
| AppJsonContext | `Infra/Json/AppJsonContext.cs` | JsonSerializerContext source-generated para serialização AOT-compatível; inserido no TypeInfoResolverChain do MVC |
| NullModelBinderProvider | `Infra/ModelBinding/NullModelBinderProvider.cs` | Substitui providers de model binding incompatíveis com AOT (TryParse, FloatingPoint) por implementação no-op |
| FallbackSimpleTypeModelBinderProvider | `Infra/ModelBinding/FallbackSimpleTypeModelBinderProvider.cs` | Substitui SimpleTypeModelBinderProvider por versão AOT-compatível |
| EnhancedModelMetadataActivator | `Infra/ModelBinding/EnhancedModelMetadataActivator.cs` | Workaround AOT: ativa IsEnhancedModelMetadataSupported antes do primeiro request via chamada explícita em startup |
| NoOpObjectModelValidator | `Infra/ModelValidation/NoOpObjectModelValidator.cs` | Substitui IObjectModelValidator padrão (reflection-based) por implementação vazia AOT-compatível |

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

## Recursos Operacionais do Assistente

Recursos externos disponíveis para o assistente durante o desenvolvimento, configurados via `.mcp.json` ou variáveis de ambiente do container.

| Recurso | Tipo | Configuração | Variáveis Requeridas | Capacidade |
|---|---|---|---|---|
| Datadog MCP | MCP Server (HTTP) | `.mcp.json` → `datadog` | `DD_API_KEY`, `DD_APP_KEY` | Acesso a logs de todos os ambientes (local, CI, produção) via ferramentas MCP do Datadog |
| GitHub API | CLI + Token | `gh` CLI + `GH_TOKEN` | `GH_TOKEN` | Criação, atualização e consulta de Pull Requests; manipulação de issues e releases via `gh api` |

### Como Novos Recursos São Registrados

Quando o usuário disponibilizar um novo recurso operacional (MCP server, integração, token):
1. Registrar nesta tabela com tipo, configuração, variáveis e capacidade
2. Atualizar `scripts/required-vars.md` com as variáveis necessárias
3. Atualizar `.claude/rules/environment-readiness.md` com o item de validação correspondente
4. Se for MCP server: atualizar `.mcp.json` com a configuração do servidor

*Referência: `Instructions/operating-model.md` — ferramentas e recursos MCP são definições duráveis*

---

## Restrições Técnicas Conhecidas

- Todo código deve compilar sem erros (`dotnet build`) antes de qualquer commit.
- A aplicação deve ser validada em modo debug (`dotnet run`) antes de executar os testes — o processo deve iniciar e responder em `/health` (qualquer código HTTP) antes de ser encerrado.
- Todos os testes devem passar em modo debug (`dotnet test`) antes de qualquer `docker compose up -d`. O publish Release/AOT só é executado após aprovação no gate de testes.
- A aplicação deve ser iniciada via `docker compose up -d` e responder HTTP 200 em `/health` antes de qualquer commit.
- Toda feature com endpoint criada ou alterada deve ser validada via chamada HTTP real ao endpoint, com a aplicação em execução, antes do commit. Se o endpoint exigir autenticação, o Bearer Token deve ser obtido via `POST /login` antes da chamada. Ver `.claude/rules/endpoint-validation.md`.
- O Datadog Agent deve estar ativo durante a execução de validação pré-commit para que logs fluam ao Datadog.
- `docker compose down` deve ser executado após a validação pré-commit.
- Slices não podem depender de outras Slices diretamente.
- `Shared/` não pode depender de Features.
- Lógica de negócio não pode estar em Endpoints nem em Repositories.
- Validação de payload deve estar no objeto `Input` de cada Slice (em `<Feature>Models/`), não em repositórios ou componentes de persistência.
- Models de Input e Output de cada Feature devem residir exclusivamente em `<Feature>Models/` dentro da própria Slice. Não podem ser compartilhados via `Shared/`. Features não devem usar models de `Shared/` (incluindo `Shared/ExternalApi/*/Models/`) como tipo de retorno de seus Use Cases ou Endpoints — devem possuir seu próprio Output model em `<Feature>Models/` e mapear os dados internamente. Models de APIs externas em `Shared/ExternalApi/*/Models/` permanecem como contratos do cliente HTTP; models de dados compartilhados em `Shared/Repositories/` permanecem como models de persistência compartilhada (DA-020).

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
| 2026-03-17 | Restrição adicionada: toda feature com endpoint deve ser validada via chamada HTTP real antes do commit; geração de token quando necessário | endpoint-validation rule |
| 2026-03-17 | Workflows de CI/CD reorganizados: pr-language-check e docker-build removidos; CI renomeado para "Validar Execução"; jobs renomeados para Compilação, Execução e Validar Health Check; unit-tests inserido na cadeia sequencial entre Execução e Validar Health Check | — |
| 2026-03-17 | Restrições adicionadas: validação em modo debug (dotnet run + dotnet test) como gate obrigatório antes do docker compose up -d (publish Release/AOT); health check pós-publish explicitamente separado da validação debug | Pipeline pré-commit |
| 2026-03-18 | Dockerfile corrigido: COPY da pasta de publicação completa (incluindo appsettings.json) + propagação de CA cert do build para runtime stage | Erro 9, Erro 10 |
| 2026-03-18 | AotControllerPreservation.PreserveControllers() tornado internal e chamado explicitamente em Program.cs; resolve trim de Controllers em Native AOT | Erro 8 |
| 2026-03-18 | CI/CD: job `healthcheck` dividido em dois jobs paralelos — `healthcheck-debug` (dotnet run) e `healthcheck-publish` (binário AOT); ambos com `needs: unit-tests` | Instrução do usuário |
| 2026-03-18 | Componentes Infra AOT adicionados à tabela: AppJsonContext, NullModelBinderProvider, FallbackSimpleTypeModelBinderProvider, EnhancedModelMetadataActivator, NoOpObjectModelValidator | Revisão de governança |
| 2026-03-19 | Seção "Recursos Operacionais do Assistente" adicionada: Datadog MCP e GitHub API registrados como recursos disponíveis; protocolo de registro de novos recursos definido | Lacuna de governança identificada |
| 2026-03-19 | Memory Cache adicionado à stack; CachedOpenMeteoApiClient adicionado aos componentes; AuthenticateFilter atualizado para armazenar AuthenticatedUser em HttpContext.Items; configuração ExternalApi reestruturada em HttpRequest/CircuitBreaker/EndpointCache | DA-018 |
| 2026-03-19 | Integração GitHub API adicionada: Refit + Polly + DelegatingHandler para PAT; Shared/ExternalApi/GitHub/ criada; Shared/Repositories/ criada; Features RepositoriesGetAll e RepositoriesSyncAll implementadas; RN-006, RN-007 | DA-019, RN-006, RN-007 |
| 2026-03-19 | Restrição adicionada: models de Input e Output de Features devem residir exclusivamente em `<Feature>Models/`, não em Shared | DA-020 |
| 2026-03-20 | Integração PokeAPI adicionada: Refit + Polly; Shared/ExternalApi/PokeApi/ criada; Feature PokemonGet implementada; RN-008 | DA-021, RN-008 |
