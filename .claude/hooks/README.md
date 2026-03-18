# Hooks do Claude Code

## Propósito

Esta pasta contém os hooks de enforcement do Claude Code para este repositório. Hooks são scripts executados automaticamente antes ou depois de operações de ferramentas.

---

## Hooks Ativos

| Hook | Tipo | Matcher | Propósito |
|---|---|---|---|
| `instruction-change-detector.sh` | PostToolUse | Write\|Edit | Detecta mudanças em arquivos de governança e emite lembrete de revisão via REVIEW.md |
| `pre-commit-gate.sh` | Manual | — | Gate de validação: dotnet build + dotnet test antes de commit |

---

## Configuração

Os hooks são configurados em `.claude/settings.json` na seção `hooks`. Apenas `instruction-change-detector.sh` é acionado automaticamente. O `pre-commit-gate.sh` é referência para execução manual no pipeline pré-commit.

---

## Relação com Governança

- `instruction-change-detector.sh` → ativa `.claude/rules/instruction-review.md` → executa `REVIEW.md`
- `pre-commit-gate.sh` → implementa parte do pipeline de validação pré-commit definido em `CLAUDE.md`

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-18 | Criado: hooks reais substituindo placeholders | Reestruturação de governança |
