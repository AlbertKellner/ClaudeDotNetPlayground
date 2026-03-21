#!/bin/bash
# Hook: instruction-change-detector.sh
# Propósito: Detectar quando arquivos de instrução são alterados e emitir lembrete
# para revisão via REVIEW.md.
# Ativação: PostToolUse em Write|Edit
#
# Este hook verifica se o arquivo alterado pertence ao escopo de governança e
# emite um lembrete para executar a revisão via REVIEW.md. A auditoria completa
# (scripts/governance-audit.sh) é executada no passo 0.1 do pipeline pré-commit,
# não por este hook — o hook é informativo, não bloqueante.

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
  echo "        A auditoria completa será executada automaticamente antes do commit (passo 0.1 do pipeline)."
fi

exit 0
