#!/bin/bash
# Hook: instruction-change-detector.sh
# Propósito: Detectar quando arquivos de instrução são alterados, emitir lembrete
# e executar auditoria automatizada de governança.
# Ativação: PostToolUse em Write|Edit
#
# Este hook verifica se o arquivo alterado pertence ao escopo de governança,
# emite um lembrete para executar a revisão via REVIEW.md, e executa o script
# de auditoria automatizada para detectar inconsistências estruturais.

FILE_PATH="${1:-}"

# Padrões de arquivos de governança
GOVERNANCE_PATTERNS=(
  "CLAUDE.md"
  "REVIEW.md"
  "Instructions/"
  ".claude/rules/"
  ".claude/skills/"
  ".claude/settings.json"
  ".claude/hooks/"
  "open-questions.md"
  "assumptions-log.md"
)

IS_GOVERNANCE=false
for pattern in "${GOVERNANCE_PATTERNS[@]}"; do
  if [[ "$FILE_PATH" == *"$pattern"* ]]; then
    IS_GOVERNANCE=true
    break
  fi
done

if [ "$IS_GOVERNANCE" = true ]; then
  echo "[Revisão de instrução necessária] O arquivo '$FILE_PATH' foi alterado. Executar checklist de REVIEW.md antes de prosseguir."

  # Executar auditoria automatizada se o script existir
  REPO_ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
  AUDIT_SCRIPT="$REPO_ROOT/scripts/governance-audit.sh"
  if [ -x "$AUDIT_SCRIPT" ]; then
    echo ""
    bash "$AUDIT_SCRIPT" 2>/dev/null || true
  fi
fi

exit 0
