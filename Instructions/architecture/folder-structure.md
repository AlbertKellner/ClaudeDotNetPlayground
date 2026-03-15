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
├── .claude/
│   ├── rules/                          # Comportamento operacional do assistente
│   │   ├── ambiguity-handling.md
│   │   ├── architecture-governance.md
│   │   ├── business-ingestion.md
│   │   ├── change-propagation.md
│   │   ├── folder-governance.md
│   │   ├── implementation-alignment.md
│   │   ├── naming-governance.md
│   │   ├── natural-language-normalization.md
│   │   ├── repository-context-evolution.md
│   │   ├── snippet-handling.md
│   │   ├── source-of-truth-priority.md
│   │   └── technical-ingestion.md
│   │
│   ├── skills/                         # Skills operacionais
│   │   ├── apply-user-snippet/
│   │   ├── evolve-governance/
│   │   ├── implement-request/
│   │   ├── ingest-definition/
│   │   ├── resolve-ambiguity/
│   │   └── review-alignment/
│   │
│   └── hooks/                          # Scripts de enforcement
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
    └── snippets/                       # Snippets normativos canônicos
        ├── README.md
        └── canonical-snippets.md
```

---

## Estrutura de Implementação

A estrutura de implementação do projeto segue **Vertical Slice Architecture** com segregação **Command/Query**.

```
src/
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
    │    ├── Middlewares/
    │    │    └── CorrelationIdMiddleware.cs   # Garante GUID v7 por request; enriquece Serilog LogContext; opaco para Features
    │    └── Security/
    │         ├── ITokenService.cs             # Contrato de geração e validação de JWT
    │         ├── AuthenticatedUser.cs         # Modelo do usuário autenticado (Id, UserName)
    │         ├── TokenService.cs              # Implementação JWT HS256
    │         ├── AuthenticateFilter.cs        # IAsyncActionFilter: valida Bearer Token; enriquece LogContext com UserId e UserName
    │         └── AuthenticateAttribute.cs     # TypeFilterAttribute: decorador [Authenticate] para Controllers
    │
    └── Shared/
         ├── <Abstrações e interfaces genéricas reutilizáveis entre Slices>
         ├── <Utilitários e helpers>
         ├── <Clientes de serviços externos>
         └── <Configurações de infraestrutura compartilhada>
```

### Regras de existência condicional

- **Não criar pastas vazias** — toda pasta deve conter ao menos um arquivo com uso real.
- **Não criar arquivos sem uso real** — scripts SQL, interfaces e models só existem quando necessários para a Slice.
- `<NomeDaFeature>Input.cs` existe apenas em Commands (ou em Queries que recebem parâmetros de busca complexos).
- `<NomeDaFeature>Output.cs` existe apenas quando há retorno estruturado.
- `<NomeDaFeature>Entity.cs` existe apenas quando a Slice materializa objetos de domínio tipados.
- `Scripts/` e `<NomeDaFeature>.sql` existem apenas quando a Slice acessa banco de dados via SQL.

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
