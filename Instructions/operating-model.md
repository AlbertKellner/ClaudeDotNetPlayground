# Modelo Operacional do Repositório

## Propósito

Este documento descreve como mensagens do usuário são processadas e roteadas para as skills corretas.
É a referência central de classificação de interações neste repositório.

---

## Como Este Repositório Funciona

Este repositório opera com um sistema de governança persistente. Todo conteúdo de governança acumulado nos arquivos de `Instructions/`, `.claude/rules/` e `.claude/skills/` constitui o contexto operacional permanente.

**O usuário não precisa reexplicar processo.** O processo está escrito aqui e nas rules.
**O usuário não precisa reexplicar contexto acumulado.** O contexto está nos arquivos de governança.

---

## Como Tipos de Mensagem São Classificados

| Tipo | Descrição | Skill Ativada |
|---|---|---|
| Nova definição durável | Introduz ou altera regra, princípio, invariante, convenção, decisão | `ingest-definition` |
| Solicitação de implementação | Cria, altera, remove ou revisa artefato do repositório | `implement-request` |
| Revisão de alinhamento | Verifica consistência entre artefatos | `review-alignment` |
| Evolução de governança | Reorganiza, consolida ou melhora a base de governança | `evolve-governance` |
| Resolução de ambiguidade | Responde a dúvida registrada ou esclarece lacuna | `resolve-ambiguity` |
| Fornecimento de trecho técnico | Entrega código, configuração, schema ou artefato para aplicar | `apply-user-snippet` |
| Alteração de instrução | Cria, altera ou remove arquivos de governança | `review-instructions` |
| Análise de PR | Analisa solicitações de mudança em pull request aberto | `pr-analysis` |

Uma mensagem pode ativar múltiplos tipos simultaneamente.

### Skills de Pipeline (não ativadas por tipo de mensagem)

As seguintes skills são invocadas como passos do pipeline pré-commit (definido em `CLAUDE.md`), não por classificação de mensagem:

| Skill | Passo do Pipeline | Propósito |
|---|---|---|
| `validate-endpoints` | Passo 6 | Validação HTTP real de endpoints após implementação |
| `verify-environment` | Passo 0 | Verificação de pré-requisitos de ambiente |
| `manage-pr-lifecycle` | Passos 10-11 | Criação/atualização de PR e acompanhamento de CI |

---

## O Que Conta Como Nova Definição Durável

Uma definição é durável quando deve continuar guiando trabalho futuro. Inclui:

- Nova regra de negócio ou alteração de regra existente
- Nova restrição ou nova exceção
- Novo fluxo ou conceito de domínio
- Novo invariante ou alteração de invariante
- Novo contrato ou alteração de contrato
- Novo cenário BDD
- Mudança no comportamento esperado do sistema
- Nova convenção de nomenclatura
- Nova regra de organização de pastas
- Nova regra arquitetural ou princípio técnico
- Nova decisão tecnológica relevante
- Ambiguidade resolvida que deva persistir como conhecimento
- Snippet canônico que deva governar futuras implementações
- Nova ferramenta, recurso MCP ou capacidade operacional disponível para o assistente
- Alteração ou remoção de ferramenta ou recurso MCP existente

---

## Separação entre Rules, Skills e Hooks

### Rules (`.claude/rules/`) — Políticas
Definem **o quê** deve ser feito ou respeitado. São restrições, prioridades e critérios.
Não contêm workflows procedurais detalhados.

### Skills (`.claude/skills/`) — Workflows
Definem **como** executar processos. São workflows procedurais auto-contidos.
Referenciam as policies das rules mas não as redefinem.

### Hooks (`.claude/hooks/`) — Enforcement
Forçam automaticamente o cumprimento de rules via scripts.
Não contêm lógica de negócio ou governança.

### CLAUDE.md — Princípios Globais
Contém princípios que afetam toda interação, pipeline pré-commit e imports.
Delega detalhes operacionais para rules e skills.

### REVIEW.md — Meta-governança
Define o checklist obrigatório para revisão de instruções.
Ativado pela meta-regra `instruction-review.md`.

---

## Uso de Subagents

O assistente pode lançar subagents especializados quando a tarefa beneficiar de paralelismo:

| Cenário | Tipo de Subagent | Quando Usar |
|---|---|---|
| Exploração de codebase | `subagent_type: Explore` | Antes de implementar, para entender a estrutura atual |
| Revisão de alinhamento | `subagent_type: general-purpose` | Após mudanças significativas, verificar consistência |
| Validação de testes | `subagent_type: general-purpose` | Executar testes em paralelo com outras tarefas |

---

## Referências Cruzadas

- `CLAUDE.md` — ponto de entrada global com princípios e pipeline
- `REVIEW.md` — checklist de revisão de instruções
- `.claude/rules/` — políticas operacionais
- `.claude/skills/` — workflows executáveis
- `.claude/hooks/` — enforcement automatizado
- `.claude/settings.json` — permissões e configuração de hooks

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| Bootstrap | Estrutura inicial criada | — |
| 2026-03-18 | Simplificado: removidas seções duplicadas com rules e CLAUDE.md; adicionada separação rules/skills/hooks; adicionada seção de subagents; adicionada referência a review-instructions | Reestruturação de governança |
| 2026-03-19 | Adicionado: ferramentas, recursos MCP e capacidades operacionais como definições duráveis | Lacuna de governança identificada |
| 2026-03-21 | Adicionado: 3 skills de pipeline extraídas das rules (validate-endpoints, verify-environment, manage-pr-lifecycle); rule governance-audit.md e script governance-audit.sh criados | Auditoria de governança |
| 2026-03-21 | Adicionado: seção "Skills de Pipeline" distinguindo skills ativadas por tipo de mensagem (8) de skills de passos do pipeline (3) | Análise estrutural de governança |
