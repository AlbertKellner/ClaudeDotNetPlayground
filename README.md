# ClaudeDotNetPlayground

## Propósito

Este repositório é um **playground de exploração funcional** que demonstra práticas profissionais de desenvolvimento em .NET 10 com ASP.NET Core, operado inteiramente pelo Claude Code com um sistema de governança persistente.

O objetivo é servir como laboratório para experimentar e consolidar padrões de engenharia de software em um projeto real: arquitetura vertical slice, autenticação JWT, integração com APIs externas via Refit + Polly, observabilidade com Serilog e Datadog, containerização com Docker e Native AOT, e um pipeline de CI/CD completo com GitHub Actions.

Todo o ciclo de desenvolvimento — desde a definição de regras de negócio até a validação de endpoints em Docker — é governado por um sistema de instruções persistentes que garante consistência, rastreabilidade e qualidade em cada interação.

---

## Seções do Repositório

### Aplicação (`src/`)

A aplicação é uma API REST em C# (.NET 10) com ASP.NET Core usando Controllers com Actions, organizada em **Vertical Slice Architecture** com segregação Command/Query.

#### Features Implementadas

| Feature | Tipo | Endpoint | Descrição |
|---------|------|----------|-----------|
| **UserLogin** | Command | `POST /login` | Autenticação com credenciais; retorna JWT Bearer Token |
| **WeatherConditionsGet** | Query | `GET /weather-conditions?latitude=X&longitude=Y` | Condições climáticas via Open-Meteo API, cache por usuário |
| **GitHubRepoSearch** | Query | `GET /github-repo-search` | Repositórios da conta AlbertKellner via GitHub API, cache por usuário |
| **PokemonGet** | Query | `GET /pokemon/{id}` | Dados de Pokémon por ID via PokéAPI, cache por usuário |
| **Health Check** | — | `GET /health` | Disponibilidade da aplicação + Datadog Agent |

#### Infraestrutura (`Infra/`)

| Componente | Responsabilidade |
|------------|------------------|
| **CorrelationIdMiddleware** | GUID v7 por requisição; enriquecimento automático de logs |
| **GlobalExceptionHandler** | Tratamento centralizado de exceções; Problem Details (RFC 7807) |
| **AuthenticateFilter** | Validação JWT; enriquecimento de logs com UserId e UserName |
| **DatadogHttpSink** | Envio de logs diretamente ao Datadog via HTTP |
| **AppJsonContext** | Serialização AOT-compatível via source generators |
| **DatadogAgentHealthCheck** | Verificação de disponibilidade do Datadog Agent |

#### Integrações Externas (`Shared/ExternalApi/`)

Todas seguem o mesmo padrão: interface Refit + resiliência Polly v8 + Memory Cache (decorator por usuário autenticado).

| Integração | API | Padrão |
|------------|-----|--------|
| **OpenMeteo** | `api.open-meteo.com` | Refit + Polly + Cache |
| **GitHub** | `api.github.com` | Refit + Polly + Cache + DelegatingHandler (PAT) |
| **Pokemon** | `pokeapi.co` | Refit + Polly + Cache |

#### Testes (`Albert.Playground.ECS.AOT.UnitTest`)

95+ testes unitários cobrindo endpoints, use cases, integrações e validação de logs com `FakeLogger<T>`.

---

### Stack Tecnológica

| Camada | Tecnologia |
|--------|------------|
| Linguagem | C# (.NET 10) |
| Framework | ASP.NET Core — Controllers com Actions |
| Logging | Serilog (console ANSI + Datadog HTTP Sink) |
| HTTP Clients | Refit (`Refit.HttpClientFactory`) source-generated |
| Resiliência | Polly v8 via `Microsoft.Extensions.Http.Resilience` |
| Cache | `IMemoryCache` — por usuário autenticado |
| Autenticação | JWT HS256 (`System.IdentityModel.Tokens.Jwt`) |
| Compilação | Native AOT (`PublishAot=true`) |
| Containerização | Docker multi-stage + docker-compose com Datadog Agent |
| CI/CD | GitHub Actions |

---

### Documentação de Logging (`LogsDocs/`)

Documentação completa do padrão de logging estruturado com storytelling (SNP-001) adotado no projeto.

| Documento | Conteúdo |
|-----------|----------|
| [01-padroes-de-logging.md](LogsDocs/01-padroes-de-logging.md) | Formato de prefixo, regras de escrita, responsabilidade por camada |
| [02-enriquecimento-de-contexto.md](LogsDocs/02-enriquecimento-de-contexto.md) | CorrelationId, UserId, UserName — injeção automática nos logs |
| [03-exemplo-requisicao-completa.md](LogsDocs/03-exemplo-requisicao-completa.md) | Exemplo ponta a ponta com logs reais de cada camada |
| [04-integracao-datadog.md](LogsDocs/04-integracao-datadog.md) | DatadogHttpSink, configuração por ambiente, docker-compose |
| [05-testes-de-logging.md](LogsDocs/05-testes-de-logging.md) | FakeLogger, padrão de asserção, exemplos de testes |
| [06-prompt-implementacao-logs.md](LogsDocs/06-prompt-implementacao-logs.md) | Prompt genérico para implementar logging em qualquer repositório |
| [07-exemplo-program-cs.md](LogsDocs/07-exemplo-program-cs.md) | Program.cs real completo e anotado com o padrão storytelling |

---

### Governança (`CLAUDE.md`, `.claude/`, `Instructions/`)

Sistema de governança persistente que opera como "sistema operacional" de todas as interações com o Claude Code.

| Grupo | Propósito |
|---|---|
| `CLAUDE.md` | Ponto de entrada: pipeline pré-commit de 12 passos e imports de governança |
| `.claude/rules/` | 13 políticas operacionais (o quê deve ser feito) |
| `.claude/skills/` | 12 workflows procedurais (como executar) |
| `.claude/hooks/` | Scripts de enforcement automatizado |
| `Instructions/architecture/` | Memória arquitetural: princípios, padrões, decisões (23 ADRs), nomenclatura, estrutura |
| `Instructions/business/` | Memória de negócio: regras (RN-002 a RN-009), invariantes, fluxos, domínio |
| `Instructions/bdd/` | Especificação por comportamento: convenções Gherkin |
| `Instructions/contracts/` | Contratos formais: OpenAPI, AsyncAPI, JSON Schema |
| `Instructions/glossary/` | Linguagem ubíqua: termos canônicos e proibidos |
| `Instructions/decisions/` | ADRs: template e política de registro de decisões |
| `Instructions/snippets/` | Snippets normativos canônicos (SNP-001: padrão de logging) |
| `REVIEW.md` | Meta-governança: checklist de revisão obrigatória para instruções |
| `scripts/governance-audit.sh` | Auditoria automatizada com 36 verificações de consistência |

---

### Wiki (`wiki/`)

24 páginas organizadas por agrupamento, publicadas automaticamente via GitHub Actions:

- **Governança** (11 páginas) — Arquitetura, padrões, convenções, testes, segurança, observabilidade, CI/CD, integrações, operação, qualidade, decisões
- **Domínio** (2 páginas) — Visão geral e regras de negócio
- **Features** (5 páginas) — Documentação individual de cada endpoint
- **Claude** (4 páginas) — Skills, hooks, convenções e visão geral operacional

---

### CI/CD (`.github/workflows/`)

| Workflow | Propósito |
|----------|-----------|
| `ci.yml` | Pipeline principal: Compilação → Execução (debug) → Testes unitários → Health Check (debug) + Health Check (publish AOT) |
| `wiki-publish.yml` | Publicação automática da wiki a cada push em `main` |

---

## Configuração

### GitHub Personal Access Token (PAT)

A feature `GET /github-repo-search` requer um PAT do GitHub (opcional — sem PAT, rate limiting mais restritivo).

- **Via `appsettings.json`**:
  ```json
  {
    "ExternalApi": {
      "GitHub": {
        "HttpRequest": {
          "PersonalAccessToken": "ghp_seu_token_aqui"
        }
      }
    }
  }
  ```
- **Via variável de ambiente** (recomendado para CI/Docker):
  ```
  GITHUB_PAT=ghp_seu_token_aqui
  ```

### Autenticação JWT

Todos os endpoints (exceto `/login` e `/health`) exigem Bearer Token JWT.

```bash
# Obter token
curl -X POST http://localhost:8080/login \
  -H "Content-Type: application/json" \
  -d '{"userName": "Albert", "password": "albert123"}'

# Consumir endpoint protegido
curl http://localhost:8080/github-repo-search \
  -H "Authorization: Bearer <token>"
```

---

## Execução

```bash
# Build
dotnet build src/Albert.Playground.ECS.AOT.Api/Albert.Playground.ECS.AOT.Api.csproj

# Testes
dotnet test src/Albert.Playground.ECS.AOT.UnitTest/Albert.Playground.ECS.AOT.UnitTest.csproj

# Docker (Release/Native AOT)
docker compose up -d --build
curl http://localhost:8080/health
```
