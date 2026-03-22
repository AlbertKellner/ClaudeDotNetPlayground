#!/bin/bash
# Hook: observe-tool-use.sh
# Propósito: Capturar uso de ferramentas como observações para o sistema de aprendizado contínuo
# Ativação: PostToolUse em * (todas as ferramentas)
# Comportamento: Informativo (exit 0 sempre — nunca bloqueia)
#
# Dados de entrada: JSON via stdin (padrão Claude Code para hooks)
# Campos disponíveis: tool_name, tool_input, tool_response, session_id, tool_use_id
#
# Captura: timestamp, ferramenta, resumo de input/output, resultado, sessão
# Scrubbing: remove tokens, API keys, senhas antes de persistir
# Storage: .claude/learning/observations.jsonl (gitignored, auto-archive em >5MB)
#
# Guards anti-loop:
# - Não observa se ECC_SKIP_OBSERVE=1
# - Não observa o próprio hook (tool_input contém observe-tool-use.sh)
# - Requer jq para parsing de JSON — sai silenciosamente se indisponível

# === CONFIGURAÇÃO ===
MAX_FILE_SIZE_BYTES=$((5 * 1024 * 1024))  # 5MB
MAX_OUTPUT_CHARS=500

# === GUARD: Skip explícito ===
if [ "${ECC_SKIP_OBSERVE:-0}" = "1" ]; then
    exit 0
fi

# === GUARD: jq disponível ===
if ! command -v jq &>/dev/null; then
    exit 0
fi

# === RESOLUÇÃO DE CAMINHOS ===
REPO_ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
LEARNING_DIR="$REPO_ROOT/.claude/learning"
OBS_FILE="$LEARNING_DIR/observations.jsonl"
ARCHIVE_DIR="$LEARNING_DIR/observations.archive"
CONFIG_FILE="$LEARNING_DIR/config.json"

# === GUARD: Sistema desabilitado ===
if [ -f "$CONFIG_FILE" ]; then
    ENABLED=$(grep -o '"enabled"[[:space:]]*:[[:space:]]*[a-z]*' "$CONFIG_FILE" | head -1 | grep -o '[a-z]*$')
    if [ "$ENABLED" = "false" ]; then
        exit 0
    fi
fi

# === LER JSON DO STDIN ===
STDIN_JSON=$(cat)

if [ -z "$STDIN_JSON" ]; then
    exit 0
fi

# === EXTRAIR CAMPOS DO JSON ===
TOOL_NAME=$(echo "$STDIN_JSON" | jq -r '.tool_name // "unknown"' 2>/dev/null)
TOOL_INPUT=$(echo "$STDIN_JSON" | jq -r '.tool_input // "" | tostring' 2>/dev/null)
TOOL_RESPONSE=$(echo "$STDIN_JSON" | jq -r '.tool_response // "" | tostring' 2>/dev/null)
SESSION_ID=$(echo "$STDIN_JSON" | jq -r '.session_id // "unknown"' 2>/dev/null)
TOOL_USE_ID=$(echo "$STDIN_JSON" | jq -r '.tool_use_id // ""' 2>/dev/null)
TIMESTAMP=$(date -u +"%Y-%m-%dT%H:%M:%SZ")

# === GUARD: Não observar o próprio hook ===
if echo "$TOOL_INPUT" | grep -q "observe-tool-use.sh" 2>/dev/null; then
    exit 0
fi

# === TRUNCAMENTO ===
truncate_text() {
    local text="$1"
    local max_len="$2"
    if [ ${#text} -gt "$max_len" ]; then
        echo "${text:0:$max_len}..."
    else
        echo "$text"
    fi
}

# === PREPARAR DADOS ===
INPUT_SUMMARY=$(truncate_text "$TOOL_INPUT" "$MAX_OUTPUT_CHARS")
OUTPUT_SUMMARY=$(truncate_text "$TOOL_RESPONSE" "$MAX_OUTPUT_CHARS")

# Detectar resultado (heurística simples)
RESULT="success"
if echo "$TOOL_RESPONSE" | grep -qiE "(error|fail|exception|denied|refused|BLOQUEADO)" 2>/dev/null; then
    RESULT="failure"
fi

# === GARANTIR DIRETÓRIO ===
mkdir -p "$LEARNING_DIR" 2>/dev/null || exit 0

# === AUTO-ARCHIVE ===
if [ -f "$OBS_FILE" ]; then
    FILE_SIZE=$(stat -f%z "$OBS_FILE" 2>/dev/null || stat -c%s "$OBS_FILE" 2>/dev/null || echo 0)
    if [ "$FILE_SIZE" -gt "$MAX_FILE_SIZE_BYTES" ]; then
        mkdir -p "$ARCHIVE_DIR" 2>/dev/null
        ARCHIVE_NAME="observations-$(date +%Y%m%d-%H%M%S).jsonl"
        mv "$OBS_FILE" "$ARCHIVE_DIR/$ARCHIVE_NAME" 2>/dev/null || true
    fi
fi

# === CONSTRUIR E ESCREVER OBSERVAÇÃO VIA JQ (escapa JSON corretamente) ===
jq -n -c \
    --arg ts "$TIMESTAMP" \
    --arg tool "$TOOL_NAME" \
    --arg input "$INPUT_SUMMARY" \
    --arg output "$OUTPUT_SUMMARY" \
    --arg result "$RESULT" \
    --arg tuid "$TOOL_USE_ID" \
    --arg sid "$SESSION_ID" \
    '{timestamp: $ts, tool: $tool, input_summary: $input, output_summary: $output, result: $result, tool_use_id: $tuid, session_id: $sid}' \
    >> "$OBS_FILE" 2>/dev/null || true

exit 0
