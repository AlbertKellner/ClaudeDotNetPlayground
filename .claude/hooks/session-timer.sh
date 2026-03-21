#!/bin/bash
# Hook: session-timer.sh
# Propósito: Ler timer de sessão e exibir tempo efetivo acumulado
# Ativação: PostToolUse em Bash
# Comportamento: Informativo (exit 0 sempre)

REPO_ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
TIMER_FILE="$REPO_ROOT/.claude/.session-timer"

# Se o arquivo de timer não existe, não há sessão ativa — sair silenciosamente
[ -f "$TIMER_FILE" ] || exit 0

# Carregar variáveis do timer
source "$TIMER_FILE" 2>/dev/null || exit 0

# Calcular tempo efetivo incluindo o segmento em andamento
NOW=$(date +%s%3N)
CURRENT_SEGMENT_MS=$((NOW - LAST_SEGMENT_START))
TOTAL_MS=$((SEGMENTS_TOTAL_MS + CURRENT_SEGMENT_MS))
TOTAL_SEC=$((TOTAL_MS / 1000))
HOURS=$((TOTAL_SEC / 3600))
MINS=$(( (TOTAL_SEC % 3600) / 60 ))
SECS=$((TOTAL_SEC % 60))

if [ "$HOURS" -gt 0 ]; then
    printf "[Tempo efetivo: %02d:%02d:%02d]\n" "$HOURS" "$MINS" "$SECS"
else
    printf "[Tempo efetivo: %02d:%02d]\n" "$MINS" "$SECS"
fi

exit 0
