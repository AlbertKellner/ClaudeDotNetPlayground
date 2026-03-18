# Regra: Revisão Obrigatória de Instruções

## Propósito

Esta regra é uma **meta-regra permanente**. Estabelece que toda criação, alteração ou remoção de arquivos de instrução, regras, skills ou governança deve passar pelo processo de revisão estruturado definido em `REVIEW.md`.

---

## Princípio Fundamental

> Instruções governam o comportamento do assistente. Instruções inconsistentes, duplicadas ou mal posicionadas degradam a qualidade de toda interação futura. A revisão de instruções é tão obrigatória quanto a revisão de código.

---

## Quando Esta Regra Se Aplica

Esta regra é ativada automaticamente sempre que qualquer um destes arquivos for **criado, alterado ou removido**:

| Escopo | Padrão |
|---|---|
| Governança global | `CLAUDE.md`, `REVIEW.md` |
| Modelo operacional | `Instructions/operating-model.md` |
| Instruções de domínio | `Instructions/**/*.md` |
| Regras operacionais | `.claude/rules/*.md` |
| Skills | `.claude/skills/*/SKILL.md` |
| Configuração | `.claude/settings.json` |
| Hooks | `.claude/hooks/*` |
| Logs de governança | `open-questions.md`, `assumptions-log.md` |

---

## Comportamento Obrigatório

### Após qualquer alteração nos arquivos acima:

1. **Executar o checklist completo** de `REVIEW.md` para os checklists aplicáveis à mudança
2. **Corrigir** qualquer violação encontrada antes de prosseguir
3. **Reportar** o resultado da revisão no relatório final da tarefa

### Verificações mínimas obrigatórias (sempre):

- **Duplicação**: o conteúdo adicionado ou alterado já existe em outro arquivo?
- **Localização**: o conteúdo está no tipo correto de arquivo (rule vs skill vs instruction)?
- **Separação**: rules contêm apenas políticas? Skills contêm apenas workflows?
- **Imports**: CLAUDE.md reflete a estrutura atual de arquivos?
- **Referências**: referências cruzadas entre arquivos estão corretas?

---

## Enforcement

- O hook `instruction-change-detector.sh` emite lembrete automático quando arquivos de instrução são alterados
- A skill `review-instructions` pode ser invocada para executar a verificação completa
- Esta regra é **auto-referente**: alterações nela mesma também ativam revisão

---

## Relação com Outras Rules

- `governance-policies.md` — políticas que esta regra protege contra inconsistência
- `source-of-truth-priority.md` — hierarquia usada para resolver conflitos encontrados durante revisão
- `REVIEW.md` — checklist executado por esta regra

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-18 | Criado: meta-regra permanente de revisão de instruções | Reestruturação de governança |
