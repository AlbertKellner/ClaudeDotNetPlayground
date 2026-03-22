#!/bin/bash
# Hook: observe-tool-use.sh
# Propósito: Capturar uso de ferramentas como observações para o sistema de aprendizado contínuo
# Ativação: PostToolUse em * (todas as ferramentas)
# Comportamento: Informativo (exit 0 sempre — nunca bloqueia)
#
# Captura: timestamp, ferramenta, resumo de input/output, resultado, sessão
# Scrubbing: remove tokens, API keys, senhas antes de persistir
# Storage: .claude/learning/observations.jsonl (gitignored, auto-archive em >5MB)
#
# Guards anti-loop:
# - Não observa subagentes (CLAUDE_AGENT_ID != vazio e != "main")
# - Não observa se ECC_SKIP_OBSERVE=1
# - Não observa o próprio hook (tool = observe-tool-use.sh)

# === CONFIGURAÇÃO ===
MAX_FILE_SIZE_BYTES=$((5 * 1024 * 1024))  # 5MB
MAX_OUTPUT_CHARS=500
SECRET_PATTERN='(api[_-]?key|token|secret|password|authorization|credentials?|auth|bearer)([\"'"'"'\s:=]+)[^\s"'"'"']{8,}'

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

# === GUARD: Skip explícito ===
if [ "${ECC_SKIP_OBSERVE:-0}" = "1" ]; then
    exit 0
fi

# === GUARD: Subagentes ===
if [ -n "${CLAUDE_AGENT_ID:-}" ] && [ "${CLAUDE_AGENT_ID}" != "main" ] && [ "${CLAUDE_AGENT_ID}" != "cli" ]; then
    exit 0
fi

# === COLETAR DADOS DO EVENTO ===
TOOL_NAME="${CLAUDE_TOOL_USE_NAME:-unknown}"
TOOL_INPUT="${CLAUDE_TOOL_USE_INPUT:-}"
TOOL_OUTPUT="${CLAUDE_TOOL_USE_OUTPUT:-}"
TOOL_ID="${CLAUDE_TOOL_USE_ID:-}"
SESSION_ID="${CLAUDE_SESSION_ID:-unknown}"
TIMESTAMP=$(date -u +"%Y-%m-%dT%H:%M:%SZ")

# === GUARD: Não observar o próprio hook ===
if echo "$TOOL_INPUT" | grep -q "observe-tool-use.sh" 2>/dev/null; then
    exit 0
fi

# === SCRUBBING DE SECRETS ===
scrub_secrets() {
    local text="$1"
    # Substituir padrões de secrets por [REDACTED]
    echo "$text" | sed -E "s/$SECRET_PATTERN/\1\2[REDACTED]/gi" 2>/dev/null || echo "$text"
}

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
INPUT_SUMMARY=$(scrub_secrets "$(truncate_text "$TOOL_INPUT" "$MAX_OUTPUT_CHARS")")
OUTPUT_SUMMARY=$(scrub_secrets "$(truncate_text "$TOOL_OUTPUT" "$MAX_OUTPUT_CHARS")")

# Detectar resultado (heurística simples)
RESULT="success"
if echo "$TOOL_OUTPUT" | grep -qiE "(error|fail|exception|denied|refused|BLOQUEADO)" 2>/dev/null; then
    RESULT="failure"
fi

# === ESCAPAR PARA JSON ===
json_escape() {
    local text="$1"
    # Escapar caracteres especiais para JSON
    text="${text//\\/\\\\}"
    text="${text//\"/\\\"}"
    text="${text//$'\n'/\\n}"
    text="${text//$'\r'/\\r}"
    text="${text//$'\t'/\\t}"
    echo "$text"
}

ESCAPED_INPUT=$(json_escape "$INPUT_SUMMARY")
ESCAPED_OUTPUT=$(json_escape "$OUTPUT_SUMMARY")
ESCAPED_TOOL=$(json_escape "$TOOL_NAME")
ESCAPED_SESSION=$(json_escape "$SESSION_ID")

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

# === ESCREVER OBSERVAÇÃO ===
OBSERVATION="{\"timestamp\":\"$TIMESTAMP\",\"tool\":\"$ESCAPED_TOOL\",\"input_summary\":\"$ESCAPED_INPUT\",\"output_summary\":\"$ESCAPED_OUTPUT\",\"result\":\"$RESULT\",\"tool_use_id\":\"$TOOL_ID\",\"session_id\":\"$ESCAPED_SESSION\"}"

echo "$OBSERVATION" >> "$OBS_FILE" 2>/dev/null || true

exit 0
