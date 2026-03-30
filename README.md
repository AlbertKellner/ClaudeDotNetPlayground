# Starter.Template.AOT

## Propósito

Template reutilizável para aplicações .NET 10 com ASP.NET Core, compilado com Native AOT, operado pelo Claude Code com um sistema de governança persistente.

Este repositório preserva toda a governança técnica, infraestrutura, pipeline de CI/CD e padrões arquiteturais — pronto para receber um novo sistema com domínio diferente.

---

## Stack Tecnológica

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

## Estrutura do Projeto

### Aplicação (`src/`)

API REST em C# (.NET 10), organizada em **Vertical Slice Architecture** com segregação Command/Query.

```
src/
├── Starter.Template.AOT.slnx
├── Starter.Template.AOT.Api/
│   ├── Features/
│   │   ├── Query/       # Slices de leitura (prontas para novas features)
│   │   └── Command/     # Slices de escrita (prontas para novas features)
│   ├── Infra/           # Infraestrutura transversal
│   └── Shared/
│       └── ExternalApi/ # Integrações HTTP externas (Refit + Polly)
└── Starter.Template.AOT.UnitTest/
```

### Infraestrutura Pronta (`Infra/`)

| Componente | Responsabilidade |
|------------|------------------|
| **CorrelationIdMiddleware** | GUID v7 por requisição; enriquecimento automático de logs |
| **GlobalExceptionHandler** | Tratamento centralizado de exceções; Problem Details (RFC 7807) |
| **AuthenticateFilter** | Validação JWT; enriquecimento de logs com UserId e UserName |
| **DatadogHttpSink** | Envio de logs diretamente ao Datadog via HTTP |
| **AppJsonContext** | Serialização AOT-compatível via source generators |
| **DatadogAgentHealthCheck** | Verificação de disponibilidade do Datadog Agent |

### Governança (`CLAUDE.md`, `.claude/`, `Instructions/`)

Sistema de governança persistente que opera como "sistema operacional" de todas as interações com o Claude Code.

| Grupo | Propósito |
|---|---|
| `CLAUDE.md` | Ponto de entrada: pipeline pré-commit e imports de governança |
| `.claude/rules/` | 13 políticas operacionais (o quê deve ser feito) |
| `.claude/skills/` | 12 workflows procedurais (como executar) |
| `.claude/hooks/` | Scripts de enforcement automatizado |
| `Instructions/architecture/` | Memória arquitetural: princípios, padrões, decisões, nomenclatura |
| `Instructions/business/` | Memória de negócio: regras, invariantes, fluxos, domínio |
| `Instructions/snippets/` | Snippets normativos canônicos (SNP-001: padrão de logging) |
| `REVIEW.md` | Meta-governança: checklist de revisão obrigatória |
| `scripts/governance-audit.sh` | Auditoria automatizada de consistência |

### CI/CD (`.github/workflows/`)

| Workflow | Propósito |
|----------|-----------|
| `ci.yml` | Pipeline: Compilação (AOT) → Execução → Testes → Health Check (debug + publish) |
| `wiki-publish.yml` | Publicação automática da wiki a cada push em `main` |

---

## Execução

```bash
# Build
dotnet build src/Starter.Template.AOT.Api/Starter.Template.AOT.Api.csproj

# Testes
dotnet test src/Starter.Template.AOT.UnitTest/Starter.Template.AOT.UnitTest.csproj

# Docker (Release/Native AOT)
docker compose up -d --build
curl http://localhost:8080/health
```

---

## Como Usar Este Template

1. Renomear `Starter.Template.AOT` para o nome do novo projeto
2. Definir regras de negócio em `Instructions/business/business-rules.md`
3. Criar Features em `Features/Query/` e `Features/Command/` seguindo os padrões em `Instructions/architecture/patterns.md`
4. Adicionar integrações externas em `Shared/ExternalApi/` seguindo o padrão Refit + Polly
5. Registrar tipos novos no `AppJsonContext` para compatibilidade AOT
6. Atualizar `AotControllerPreservation` com `[DynamicDependency]` para novos Controllers
