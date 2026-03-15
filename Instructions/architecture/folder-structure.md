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
│   └── hooks/                          # Scripts de enforcement (placeholders)
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

> **Pendente de definição.** A estrutura de pastas do código e artefatos de implementação será definida aqui quando a stack e arquitetura forem estabelecidas.

```
# Exemplo genérico — substituir pela estrutura real quando a stack for definida
/
└── [pasta-do-projeto]/
    └── [a definir conforme stack e estilo arquitetural]
```

---

## Regras para Criação de Novas Pastas

1. Toda nova pasta de implementação deve ser registrada neste arquivo com seu propósito
2. Não criar pastas sem justificativa explícita
3. Não criar pastas com apenas um arquivo quando múltiplos são improvávels
4. Não duplicar estrutura quando há local existente apropriado
5. Mudanças significativas de estrutura devem ser registradas como ADR

---

## Regras para Arquivos de Governança

A estrutura de governança em `.claude/` e `Instructions/` é **imutável** sem instrução explícita.
Qualquer adição à estrutura de governança deve:
1. Ser solicitada explicitamente pelo usuário
2. Ser registrada neste arquivo
3. Ser refletida nos imports de `CLAUDE.md`

---

## Referências Cruzadas

- `Instructions/architecture/technical-overview.md` — visão geral que reflete esta estrutura
- `Instructions/architecture/patterns.md` — padrões que determinam a organização
- `.claude/rules/folder-governance.md` — regras de governança de estrutura

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| Bootstrap | Estrutura inicial de governança criada | — |
