#!/bin/bash
# Hook: instruction-change-detector.sh
# Propósito: Detectar quando arquivos de instrução são alterados e emitir lembrete
# Ativação: PostToolUse em Write|Edit
#
# Este hook verifica se o arquivo alterado pertence ao escopo de governança
# e emite um lembrete para executar a revisão via REVIEW.md.

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

for pattern in "${GOVERNANCE_PATTERNS[@]}"; do
  if [[ "$FILE_PATH" == *"$pattern"* ]]; then
    echo "[Revisão de instrução necessária] O arquivo '$FILE_PATH' foi alterado. Executar checklist de REVIEW.md antes de prosseguir."
    exit 0
  fi
done

# Arquivo não é de governança — nenhuma ação necessária
exit 0
