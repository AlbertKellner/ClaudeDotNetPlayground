#!/bin/bash
# Hook: branch-guard.sh
# Propósito: Detectar operações de branch incorretas durante pr-analysis.
# Ativação: PostToolUse em Bash
#
# Durante análise de PR (quando .claude/.pr-analysis-context existe),
# verifica se o branch atual corresponde ao head.ref esperado do PR.
# Se o branch estiver incorreto, emite alerta.
#
# Fora de contexto de pr-analysis, este hook não faz nada.

REPO_ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
CONTEXT_FILE="$REPO_ROOT/.claude/.pr-analysis-context"

# Só ativar durante pr-analysis
if [ ! -f "$CONTEXT_FILE" ]; then
  exit 0
fi

# Ler o branch esperado do contexto
EXPECTED_BRANCH=$(grep -oP 'head_ref=\K.*' "$CONTEXT_FILE" 2>/dev/null || echo "")

if [ -z "$EXPECTED_BRANCH" ]; then
  exit 0
fi

# Verificar branch atual
CURRENT_BRANCH=$(git -C "$REPO_ROOT" rev-parse --abbrev-ref HEAD 2>/dev/null || echo "")

if [ -n "$CURRENT_BRANCH" ] && [ "$CURRENT_BRANCH" != "$EXPECTED_BRANCH" ]; then
  echo "[ALERTA branch-guard] Branch atual ($CURRENT_BRANCH) difere do esperado ($EXPECTED_BRANCH) para pr-analysis."
  echo "                       Durante análise de PR, todos os commits devem ser feitos no branch do PR: $EXPECTED_BRANCH"
fi

exit 0
