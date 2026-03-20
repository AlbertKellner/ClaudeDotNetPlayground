# Estrutura de Pastas

## Propósito

Este arquivo define e documenta a estrutura de pastas e módulos deste repositório. Toda mudança estrutural relevante deve ser registrada aqui antes de ser executada.

---

## Estrutura de Governança (Imutável sem Instrução Explícita)

A estrutura abaixo foi criada no bootstrap e não deve ser alterada sem instrução explícita do usuário:

```
/
├── CLAUDE.md                           # Ponto de entrada da governança
├── README.md                           # Documentação pública do repositório
├── open-questions.md                   # Dúvidas e ambiguidades abertas
├── assumptions-log.md                  # Premissas ativas
│
├── wiki/                               # Arquivos-fonte da GitHub Wiki (fonte canônica)
│   ├── Home.md
│   ├── Project-Setup.md
│   ├── Architecture.md
│   ├── Feature-Health.md
│   ├── Feature-UserLogin.md
│   ├── Feature-TestGet.md
│   ├── Feature-WeatherConditionsGet.md
│   ├── Feature-RepositoriesGetAll.md
│   ├── Feature-RepositoriesSyncAll.md
│   ├── Feature-PokemonSearchGet.md
│   ├── Infra-Correlation-ID.md
│   ├── Infra-Authentication.md
│   ├── Infra-Exception-Handling.md
│   ├── Business-Rules.md
│   ├── CI-CD.md
│   └── _Sidebar.md
│
├── .claude/
│   ├── rules/                          # Políticas operacionais (o quê)
│   │   ├── architecture-governance.md
│   │   ├── bash-error-logging.md
│   │   ├── endpoint-validation.md
│   │   ├── environment-readiness.md
│   │   ├── folder-governance.md
│   │   ├── governance-policies.md       # Consolidação: normalização, contexto, propagação, ambiguidade, snippets
│   │   ├── instruction-review.md        # Meta-regra: revisão obrigatória via REVIEW.md
│   │   ├── naming-governance.md
│   │   ├── pr-metadata-governance.md
│   │   └── source-of-truth-priority.md
│   │
│   ├── skills/                         # Workflows operacionais (como)
│   │   ├── apply-user-snippet/
│   │   ├── evolve-governance/
│   │   ├── implement-request/
│   │   ├── ingest-definition/
│   │   ├── resolve-ambiguity/
│   │   ├── review-alignment/
│   │   └── review-instructions/         # Executa REVIEW.md
│   │
│   ├── hooks/                          # Scripts de enforcement
│   │   ├── instruction-change-detector.sh
│   │   ├── pre-commit-gate.sh
│   │   └── README.md
│
└── Instructions/
    ├── operating-model.md              # Modelo operacional consolidado
    ├── architecture/                   # Memória arquitetural e técnica
    │   ├── technical-overview.md
    │   ├── engineering-principles.md
    │   ├── patterns.md
    │   ├── naming-conventions.md
    │   ├── folder-structure.md         # (este arquivo)
    │   └── architecture-decisions.md
    ├── business/                       # Memória de negócio e domínio
    │   ├── business-rules.md
    │   ├── invariants.md
    │   ├── workflows.md
    │   ├── domain-model.md
    │   └── assumptions.md
    ├── bdd/                            # Cenários comportamentais
    │   ├── README.md
    │   ├── conventions.md
    │   └── example.feature
    ├── contracts/                      # Contratos formais de interface
    │   ├── README.md
    │   ├── openapi.yaml
    │   ├── asyncapi.yaml
    │   ├── schemas/
    │   └── examples/
    ├── glossary/                       # Governança terminológica
    │   └── ubiquitous-language.md
    ├── decisions/                      # ADRs e decisões registradas
    │   ├── README.md
    │   └── adr-template.md
    ├── snippets/                       # Snippets normativos canônicos
    │   ├── README.md
    │   └── canonical-snippets.md
    └── wiki/                           # Governança da GitHub Wiki
        └── wiki-governance.md
```

---

## Estrutura de Implementação

A estrutura de implementação do projeto segue **Vertical Slice Architecture** com segregação **Command/Query**.

Todos os projetos ficam contidos na pasta `src/` na raiz do repositório. A solution (`.slnx`) fica na raiz de `src/`. Cada pasta de projeto tem exatamente o mesmo nome do projeto que contém, sendo permitido apenas um projeto por pasta.

```
src/
├── Albert.Playground.ECS.AOT.slnx          # Solution file
├── Albert.Playground.ECS.AOT.UnitTest/      # Projeto de testes unitários
│   └── (espelha estrutura do projeto principal: Features/, Infra/, Shared/, TestHelpers/)
│
└── <NomeDoProjeto>/
    ├── Features/
    │   ├── Query/
    │   │   └── <NomeDaFeature>/
    │   │        ├── <NomeDaFeature>Endpoint/
    │   │        │    └── <NomeDaFeature>Endpoint.cs
    │   │        ├── <NomeDaFeature>UseCase/
    │   │        │    └── <NomeDaFeature>UseCase.cs
    │   │        ├── <NomeDaFeature>Interfaces/
    │   │        │    └── I<NomeDaFeature>Repository.cs
    │   │        ├── <NomeDaFeature>Models/
    │   │        │    ├── <NomeDaFeature>Output.cs
    │   │        │    └── <NomeDaFeature>Entity.cs    (quando aplicável)
    │   │        └── <NomeDaFeature>Repository/
    │   │             ├── <NomeDaFeature>Repository.cs
    │   │             └── Scripts/
    │   │                  └── <NomeDaFeature>.sql
    │   │
    │   └── Command/
    │        └── <NomeDaFeature>/
    │             ├── <NomeDaFeature>Endpoint/
    │             │    └── <NomeDaFeature>Endpoint.cs
    │             ├── <NomeDaFeature>UseCase/
    │             │    └── <NomeDaFeature>UseCase.cs
    │             ├── <NomeDaFeature>Interfaces/
    │             │    └── I<NomeDaFeature>Repository.cs
    │             ├── <NomeDaFeature>Models/
    │             │    ├── <NomeDaFeature>Input.cs
    │             │    ├── <NomeDaFeature>Output.cs   (quando aplicável)
    │             │    └── <NomeDaFeature>Entity.cs   (quando aplicável)
    │             └── <NomeDaFeature>Repository/
    │                  ├── <NomeDaFeature>Repository.cs
    │                  └── Scripts/
    │                       └── <NomeDaFeature>.sql
    │
    ├── Infra/
    │    ├── Correlation/
    │    │    └── GuidV7.cs                    # Geração (Guid.CreateVersion7) e validação de GUID v7 (uso interno de Infra)
    │    ├── ExceptionHandling/
    │    │    └── GlobalExceptionHandler.cs   # Handler centralizado de exceções (IExceptionHandler + Problem Details RFC 7807)
    │    ├── HealthChecks/
    │    │    └── DatadogAgentHealthCheck.cs  # Verifica disponibilidade do Datadog Agent via HTTP (RN-005)
    │    ├── Json/
    │    │    └── AppJsonContext.cs            # JsonSerializerContext source-generated para serialização AOT-compatível da aplicação
    │    ├── Middlewares/
    │    │    └── CorrelationIdMiddleware.cs   # Garante GUID v7 por request; enriquece Serilog LogContext; opaco para Features
    │    ├── ModelBinding/
    │    │    ├── NullModelBinderProvider.cs           # Substituição de providers incompatíveis com AOT (TryParse, FloatingPoint)
    │    │    ├── EnhancedModelMetadataActivator.cs    # Workaround AOT: ativa IsEnhancedModelMetadataSupported antes do primeiro request
    │    │    └── FallbackSimpleTypeModelBinderProvider.cs  # Substitui SimpleTypeModelBinderProvider para compatibilidade AOT
    │    ├── ModelValidation/
    │    │    └── NoOpObjectModelValidator.cs  # Substitui IObjectModelValidator padrão (reflection-based) por implementação AOT-compatível
    │    └── Security/
    │         ├── ITokenService.cs             # Contrato de geração e validação de JWT
    │         ├── AuthenticatedUser.cs         # Modelo do usuário autenticado (Id, UserName)
    │         ├── TokenService.cs              # Implementação JWT HS256
    │         ├── AuthenticateFilter.cs        # IAsyncActionFilter: valida Bearer Token; enriquece LogContext com UserId e UserName
    │         └── AuthenticateAttribute.cs     # TypeFilterAttribute: decorador [Authenticate] para Controllers
    │
    └── Shared/
         ├── ExternalApi/                           # Integrações com APIs HTTP externas (DA-017)
         │    └── <Servico>/                        # Uma subpasta por serviço externo integrado
         │         ├── I<Servico>Api.cs             # Interface Refit (contrato HTTP; rota hardcoded)
         │         ├── I<Servico>ApiClient.cs       # Interface de serviço (Features injetam este contrato)
         │         ├── <Servico>ApiClient.cs        # Implementação: usa I<Servico>Api; aplica logging SNP-001
         │         ├── <Servico>AuthenticationHandler.cs  # DelegatingHandler (quando API requer auth)
         │         └── Models/
         │              ├── <Servico>Input.cs       # Parâmetros da requisição à API
         │              └── <Servico>Output.cs      # Modelo completo da resposta da API + JsonSerializerContext
         ├── Repositories/                          # Modelos compartilhados de persistência em arquivo
         │    └── RepositoryFileEntry.cs            # Modelo do arquivo JSON de repositórios + JsonSerializerContext
         ├── <Abstrações e interfaces genéricas reutilizáveis entre Slices>
         ├── <Utilitários e helpers>
         └── <Configurações de infraestrutura compartilhada>
```

### Regras de existência condicional

- **Não criar pastas vazias** — toda pasta deve conter ao menos um arquivo com uso real.
- **Não criar arquivos sem uso real** — scripts SQL, interfaces e models só existem quando necessários para a Slice.
- `<NomeDaFeature>Input.cs` existe apenas em Commands (ou em Queries que recebem parâmetros de busca complexos).
- `<NomeDaFeature>Output.cs` existe apenas quando há retorno estruturado.
- `<NomeDaFeature>Entity.cs` existe apenas quando a Slice materializa objetos de domínio tipados.
- `Scripts/` e `<NomeDaFeature>.sql` existem apenas quando a Slice acessa banco de dados via SQL.
- `<NomeDaFeature>Input.cs` e `<NomeDaFeature>Output.cs` pertencem exclusivamente a `<Feature>Models/` da Slice. Não devem ser movidos ou duplicados em `Shared/`. Models em `Shared/ExternalApi/*/Models/` são models de APIs externas (contrato do cliente HTTP, não de Features) e seguem regras próprias (DA-017). Models em `Shared/Repositories/` são models de dados compartilhados e não se confundem com models de Features (DA-020).
- Features que consomem APIs externas via `Shared/ExternalApi/` devem possuir seu próprio model de Output em `<Feature>Models/`, mesmo que a estrutura espelhe o model da API externa. O model de Shared é o contrato do cliente HTTP; o model da Feature é o contrato do endpoint. Features não devem usar types de `Shared/` como tipo de retorno de seus Use Cases ou Endpoints (DA-020).

---

## Regras para Criação de Novas Pastas

1. Toda nova pasta de implementação deve ser registrada neste arquivo com seu propósito.
2. Não criar pastas sem justificativa explícita.
3. Não criar subpastas para conter apenas um arquivo quando múltiplos são improváveis.
4. Não duplicar estrutura quando há local existente apropriado.
5. Mudanças significativas de estrutura devem ser registradas como ADR.

---

## Regras para Arquivos de Governança

A estrutura de governança em `.claude/` e `Instructions/` é **imutável** sem instrução explícita.
Qualquer adição à estrutura de governança deve:
1. Ser solicitada explicitamente pelo usuário.
2. Ser registrada neste arquivo.
3. Ser refletida nos imports de `CLAUDE.md`.

---

## Referências Cruzadas

- `Instructions/architecture/technical-overview.md` — visão geral que reflete esta estrutura
- `Instructions/architecture/patterns.md` — padrões que determinam a organização
- `Instructions/architecture/naming-conventions.md` — nomenclatura de pastas e arquivos
- `.claude/rules/folder-governance.md` — regras de governança de estrutura

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| Bootstrap | Estrutura inicial de governança criada | — |
| 2026-03-15 | Estrutura de implementação definida: Features/Query, Features/Command, Shared | DA-004, DA-005 |
| 2026-03-15 | Shared/Middleware/ criada: GlobalExceptionHandler registrado | DA-010, PAD-008 |
| 2026-03-15 | Infra/ criada com subpastas Correlation/, ExceptionHandling/, Middlewares/; Shared/Middleware/ removida; GlobalExceptionHandler movido para Infra/ExceptionHandling/ | DA-011 |
| 2026-03-15 | Infra/Security/ criada com ITokenService, AuthenticatedUser, TokenService, AuthenticateFilter, AuthenticateAttribute | DA-013 |
| 2026-03-15 | wiki/ criada na raiz: 12 arquivos-fonte da GitHub Wiki; Instructions/wiki/ criada com wiki-governance.md | Instrução do usuário |
| 2026-03-16 | Shared/ExternalApi/ documentada: padrão de integração HTTP externa com Refit + Polly (DA-017); primeira integração: Open-Meteo | Instrução do usuário |
| 2026-03-17 | Infra/HealthChecks/ adicionada: DatadogAgentHealthCheck implementa RN-005 | Instrução do usuário |
| 2026-03-17 | wiki/ atualizada: Feature-WeatherConditionsGet.md adicionada à lista | Revisão de governança |
| 2026-03-18 | Infra/Json/, Infra/ModelBinding/, Infra/ModelValidation/ documentadas: presentes no código mas ausentes do registro estrutural | Revisão de governança |
| 2026-03-18 | Reorganização: solution movida para `src/`; projeto de testes movido de `tests/` para `src/`; pasta `tests/` removida; todos os projetos agora em `src/` | Instrução do usuário |
| 2026-03-19 | Shared/ExternalApi/GitHub/ criada (integração GitHub API); Shared/Repositories/ criada (modelo JSON compartilhado); Features RepositoriesGetAll e RepositoriesSyncAll adicionadas | DA-019, RN-006, RN-007 |
| 2026-03-19 | Regra de residência de models adicionada: Input e Output de Features exclusivamente em `<Feature>Models/`, não em Shared | DA-020 |
| 2026-03-20 | Shared/ExternalApi/PokeApi/ criada (integração PokeAPI pública); Feature PokemonSearchGet adicionada em Features/Query/; wiki/ atualizada com Feature-PokemonSearchGet.md, Feature-RepositoriesGetAll.md e Feature-RepositoriesSyncAll.md | DA-021, RN-008 |
