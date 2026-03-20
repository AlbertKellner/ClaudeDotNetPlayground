#!/bin/bash
# Hook: branch-guard.sh
# Propósito: Detectar operações de branch incorretas durante pr-analysis
# Ativação: PostToolUse em Bash
#
# Quando existe .claude/.pr-analysis-context com o head.ref esperado,
# qualquer git checkout ou git branch para um branch diferente emite alerta.

PR_CONTEXT_FILE=".claude/.pr-analysis-context"

if [ ! -f "$PR_CONTEXT_FILE" ]; then
  exit 0
fi

EXPECTED_BRANCH=$(head -1 "$PR_CONTEXT_FILE" 2>/dev/null)
COMMAND="${CLAUDE_TOOL_INPUT_COMMAND:-}"

if [ -z "$EXPECTED_BRANCH" ] || [ -z "$COMMAND" ]; then
  exit 0
fi

if echo "$COMMAND" | grep -qE "git (checkout|switch|branch)"; then
  if ! echo "$COMMAND" | grep -q "$EXPECTED_BRANCH"; then
    echo "[ALERTA - branch-guard] Contexto de pr-analysis ativo. Branch esperado: $EXPECTED_BRANCH. O comando pode estar mudando para um branch incorreto. Durante pr-analysis, todos os commits devem ser feitos no branch de origem do PR (head.ref)."
  fi
fi

exit 0
