# Sistema de Governança para Claude Code

## Árvore de Arquivos

```
.claude/
  hooks/         (6 scripts + settings.json)
  rules/         (12 rules operacionais)
  skills/        (6 skills com SKILL.md cada)
CLAUDE.md
Instructions/
  architecture/  (6 arquivos)
  bdd/           (3 arquivos + example.feature)
  business/      (5 arquivos)
  contracts/     (openapi.yaml, asyncapi.yaml, schema, exemplos)
  decisions/     (README + template ADR)
  glossary/      (ubiquitous-language.md)
  operating-model.md
  snippets/      (README + canonical-snippets.md)
README.md
assumptions-log.md
open-questions.md
```

---

## Propósito Operacional por Grupo

| Grupo | Propósito |
|---|---|
| `CLAUDE.md` | Ponto de entrada: pipeline obrigatório de execução e imports de governança |
| `.claude/rules/` | **Comportamento operacional persistente do assistente** — 12 rules que definem como interpretar, classificar, priorizar, propagar e implementar |
| `.claude/skills/` | 6 skills que mapeiam tipos de solicitação a workflows completos de execução |
| `.claude/hooks/` | Scripts placeholder de enforcement — prontos para especialização quando a stack for definida |
| `Instructions/operating-model.md` | Documento central: explica como tudo funciona junto em runtime |
| `Instructions/architecture/` | Memória arquitetural: princípios, padrões, decisões, nomenclatura, estrutura |
| `Instructions/business/` | Memória de negócio: regras, invariantes, fluxos, modelo de domínio, premissas |
| `Instructions/bdd/` | Especificação por comportamento: convenções e exemplo Gherkin agnóstico |
| `Instructions/contracts/` | Contratos formais: OpenAPI, AsyncAPI, JSON Schema, exemplos válido/inválido |
| `Instructions/glossary/` | Linguagem ubíqua: termos canônicos, sinônimos permitidos, termos proibidos |
| `Instructions/decisions/` | ADRs: template e README com política de quando e como registrar decisões |
| `Instructions/snippets/` | Snippets canônicos: registro de trechos normativos para preservação literal futura |
| `open-questions.md` | Dúvidas e ambiguidades abertas — apenas itens ainda pendentes |
| `assumptions-log.md` | Premissas ativas — apenas itens ainda válidos (3 premissas de bootstrap registradas) |

---

## O Que os Arquivos Gerados Contêm

- **Pipeline obrigatório** de 13 passos para toda mensagem (`CLAUDE.md`, `implementation-alignment.md`)
- **Regras explícitas** de tratamento de linguagem natural imperfeita (`natural-language-normalization.md`)
- **Política completa de ambiguidades** com distinção bloqueante/não-bloqueante, registro e resolução (`ambiguity-handling.md`)
- **Classificação de snippets** em 4 categorias com regras de preservação e adaptação (`snippet-handling.md`)
- **Hierarquia de prioridade** entre fontes de verdade com regras de conflito (`source-of-truth-priority.md`)
- **Propagação de mudanças** com mapa transversal por tipo de artefato (`change-propagation.md`)
- **Política de governança primeiro** com definição de "definição durável" (`implementation-alignment.md`)
- **Evolução específica** do contexto do repositório com proteção contra regressão (`repository-context-evolution.md`)
- **6 skills operacionais** com workflows internos completos para cada tipo de solicitação
- **Estrutura durável** em todos os arquivos de `Instructions/` — prontos para receber conteúdo específico do domínio

---

## Começando

A partir de agora, qualquer mensagem em linguagem natural é entrada operacional suficiente.
