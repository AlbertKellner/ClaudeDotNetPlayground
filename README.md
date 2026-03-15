# Sistema de Governança para Claude Code

Este repositório utiliza um sistema de governança persistente que serve como o **sistema operacional** de todas as interações futuras com o assistente.

---

## Finalidade

Este sistema transforma toda mensagem em linguagem natural em **entrada operacional suficiente**.
O assistente interpreta, classifica, consulta a governança acumulada, atualiza definições quando necessário e implementa — sem exigir que o usuário forneça instruções de processo.

---

## Princípio Central

> O bootstrap é genérico apenas como mecanismo de inicialização.
> Este repositório se torna específico por meio das definições acumuladas ao longo do tempo.
> O comportamento futuro é governado pelos arquivos criados aqui e evoluídos pelas interações reais.

---

## Como Funciona

1. **Você escreve em linguagem natural** — informal, fragmentada, ditada, imperativa.
2. **O assistente interpreta semanticamente** — normaliza a intenção, resolve ambiguidades com o menor pressuposto coerente.
3. **O assistente consulta a governança** — lê arquivos relevantes antes de agir.
4. **O assistente verifica ambiguidades** — registra dúvidas materiais e responde antes de implementar quando necessário.
5. **O assistente atualiza a governança primeiro** — quando a mensagem introduz ou altera uma definição durável.
6. **O assistente implementa** — baseado nas fontes de verdade do repositório.
7. **O assistente relata** — o que foi interpretado, consultado, alterado, classificado e assumido.

---

## Estrutura do Sistema

### `CLAUDE.md`
Ponto de entrada. Define comportamento obrigatório, pipeline de execução e imports de governança.

### `.claude/rules/`
Comportamento operacional do assistente. Escrito como orientação normativa.
- `natural-language-normalization.md` — como interpretar entrada imperfeita
- `technical-ingestion.md` — como ingerir definições técnicas
- `business-ingestion.md` — como ingerir definições de negócio
- `source-of-truth-priority.md` — prioridade entre fontes de verdade
- `implementation-alignment.md` — workflow obrigatório de implementação
- `ambiguity-handling.md` — como tratar dúvidas e ambiguidades
- `snippet-handling.md` — como classificar e tratar trechos técnicos
- `architecture-governance.md` — governança arquitetural
- `naming-governance.md` — governança de nomenclatura
- `folder-governance.md` — governança de estrutura de pastas
- `change-propagation.md` — propagação de mudanças entre artefatos
- `repository-context-evolution.md` — como o contexto do repositório evolui

### `.claude/skills/`
Skills operacionais que reforçam o modelo de execução:
- `ingest-definition/` — ingere novas definições duráveis
- `implement-request/` — executa solicitações de implementação
- `review-alignment/` — revisa coerência entre artefatos
- `evolve-governance/` — evolui a base de governança
- `resolve-ambiguity/` — trata dúvidas e ambiguidades
- `apply-user-snippet/` — classifica e aplica trechos do usuário

### `.claude/hooks/`
Scripts placeholder e configurações que reforçam o modelo operacional. Devem ser adaptados quando a stack do repositório se tornar concreta.

### `Instructions/operating-model.md`
Documento central consolidado do modelo operacional. Explica como tudo funciona junto.

### `Instructions/architecture/`
Memória arquitetural e instruções técnicas:
- `technical-overview.md`, `engineering-principles.md`, `patterns.md`
- `naming-conventions.md`, `folder-structure.md`, `architecture-decisions.md`

### `Instructions/business/`
Memória de negócio e regras de domínio:
- `business-rules.md`, `invariants.md`, `workflows.md`, `domain-model.md`, `assumptions.md`

### `Instructions/bdd/`
Cenários comportamentais:
- `README.md`, `conventions.md`, `example.feature`
- BDD é importante, mas **não obrigatório para toda mudança**.

### `Instructions/contracts/`
Artefatos formais de interface, contratos de mensagens, schemas:
- `README.md`, `openapi.yaml`, `asyncapi.yaml`, schemas e exemplos
- Contratos são importantes, mas **não obrigatórios para toda mudança**.

### `Instructions/glossary/`
Governança terminológica:
- `ubiquitous-language.md` — termos canônicos, sinônimos permitidos, termos proibidos

### `Instructions/decisions/`
Decisões registradas e ADRs:
- `README.md`, `adr-template.md`

### `Instructions/snippets/`
Referências canônicas de snippets normativos:
- Snippets normativos explicitamente exigidos pelo usuário são preservados na íntegra.
- Exemplos ilustrativos enviados pelo usuário podem ser adaptados ao contexto.

### `open-questions.md`
Dúvidas e ambiguidades abertas. Contém apenas itens ainda pendentes.

### `assumptions-log.md`
Premissas adotadas e ainda ativas. Contém apenas itens ainda válidos.

---

## Regras Fundamentais

| Situação | Comportamento |
|---|---|
| Mensagem informal ou fragmentada | Interpretação semântica, nunca rejeição |
| Nova definição durável | Governança atualizada **antes** da implementação |
| Dúvida material | Registrada, reportada, implementação aguarda confirmação |
| Snippet normativo explícito | Copiado na íntegra, sem reescrita livre |
| Exemplo ilustrativo | Adaptado ao contexto do projeto |
| Conflito entre fontes | Reportado, resolvido pela ordem de prioridade |
| Solicitação curta | Contexto do repositório consultado primeiro |

---

## Prioridade entre Fontes de Verdade

1. Contratos executáveis, artefatos formais e snippets normativos declarados canônicos
2. BDD
3. Regras de negócio estruturadas
4. Arquitetura e padrões técnicos
5. Convenções de nomenclatura, estilo e organização

---

## Começando

Escreva qualquer mensagem em linguagem natural. O sistema de governança cuida do resto.
