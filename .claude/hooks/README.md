# Hooks do Claude Code

## Propósito

Esta pasta contém os hooks de enforcement do Claude Code para este repositório. Hooks são scripts executados automaticamente antes ou depois de operações de ferramentas.

---

## Hooks Ativos

| Hook | Tipo | Matcher | Propósito |
|---|---|---|---|
| `instruction-change-detector.sh` | PostToolUse | Write\|Edit | Detecta mudanças em arquivos de governança e emite lembrete de revisão via REVIEW.md |
| `pre-commit-gate.sh` | Manual | — | Gate de validação: dotnet build + dotnet test antes de commit |
| `branch-guard.sh` | PostToolUse | Bash | Detecta operações de branch incorretas durante pr-analysis; emite alerta se o branch não for o head.ref esperado |

---

## Configuração

Os hooks são configurados em `.claude/settings.json` na seção `hooks`. Os hooks `instruction-change-detector.sh` e `branch-guard.sh` são acionados automaticamente (PostToolUse). O `pre-commit-gate.sh` é referência para execução manual no pipeline pré-commit.

---

## Relação com Governança

- `instruction-change-detector.sh` → ativa `.claude/rules/instruction-review.md` → executa `REVIEW.md`
- `pre-commit-gate.sh` → implementa parte do pipeline de validação pré-commit definido em `CLAUDE.md`
- `branch-guard.sh` → protege o branch correto durante pr-analysis; usa `.claude/.pr-analysis-context` como contexto

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-18 | Criado: hooks reais substituindo placeholders | Reestruturação de governança |
| 2026-03-20 | Adicionado: branch-guard.sh para proteção de branch durante pr-analysis | Correção de workflow de PR |
