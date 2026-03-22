#!/bin/bash
# Script: instinct-manager.sh
# Propósito: CLI para gestão de instintos do sistema de aprendizado contínuo
#
# Comandos:
#   list       — Lista todos os instintos com confidence e domínio
#   decay      — Aplica decay semanal a instintos sem observação recente
#   prune      — Remove instintos com confidence < 0.2
#   promote    — Move instinto de tentative/ para active/ quando confidence >= 0.5
#   graduate   — Move instinto maduro para graduated/ (requer id)
#   stats      — Estatísticas do sistema de aprendizado
#   help       — Exibe esta ajuda
#
# Uso: bash scripts/instinct-manager.sh <comando> [opções]

set -euo pipefail
shopt -s nullglob

# === CONFIGURAÇÃO ===
REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
LEARNING_DIR="$REPO_ROOT/.claude/learning"
CONFIG_FILE="$LEARNING_DIR/config.json"
ACTIVE_DIR="$LEARNING_DIR/instincts/active"
TENTATIVE_DIR="$LEARNING_DIR/instincts/tentative"
GRADUATED_DIR="$LEARNING_DIR/graduated"
OBS_FILE="$LEARNING_DIR/observations.jsonl"

# Thresholds (defaults, overridden by config.json if available)
ACTIVE_MIN=0.5
GRADUATION_MIN=0.85
AUTO_REMOVE_BELOW=0.2
WEEKLY_DECAY=0.02

# Carregar thresholds do config.json se disponível
if [ -f "$CONFIG_FILE" ]; then
    ACTIVE_MIN=$(grep -o '"active_min"[[:space:]]*:[[:space:]]*[0-9.]*' "$CONFIG_FILE" 2>/dev/null | grep -o '[0-9.]*$' || echo "$ACTIVE_MIN")
    GRADUATION_MIN=$(grep -o '"graduation_min"[[:space:]]*:[[:space:]]*[0-9.]*' "$CONFIG_FILE" 2>/dev/null | grep -o '[0-9.]*$' || echo "$GRADUATION_MIN")
    AUTO_REMOVE_BELOW=$(grep -o '"auto_remove_below"[[:space:]]*:[[:space:]]*[0-9.]*' "$CONFIG_FILE" 2>/dev/null | grep -o '[0-9.]*$' || echo "$AUTO_REMOVE_BELOW")
    WEEKLY_DECAY=$(grep -o '"weekly_decay"[[:space:]]*:[[:space:]]*-\?[0-9.]*' "$CONFIG_FILE" 2>/dev/null | grep -o '[0-9.]*$' || echo "$WEEKLY_DECAY")
fi

# === FUNÇÕES AUXILIARES ===

# Extrair campo do frontmatter YAML de um arquivo .md
get_field() {
    local file="$1"
    local field="$2"
    grep "^${field}:" "$file" 2>/dev/null | head -1 | sed "s/^${field}:[[:space:]]*//" | sed 's/^["'"'"']//;s/["'"'"']$//'
}

# Atualizar campo no frontmatter YAML
set_field() {
    local file="$1"
    local field="$2"
    local value="$3"
    if grep -q "^${field}:" "$file" 2>/dev/null; then
        sed -i "s|^${field}:.*|${field}: ${value}|" "$file"
    fi
}

# Calcular dias desde uma data ISO
days_since() {
    local date_str="$1"
    local now_epoch=$(date +%s)
    local then_epoch=$(date -d "$date_str" +%s 2>/dev/null || date -j -f "%Y-%m-%d" "$date_str" +%s 2>/dev/null || echo "$now_epoch")
    echo $(( (now_epoch - then_epoch) / 86400 ))
}

# Comparar floats (retorna 0 se $1 < $2)
float_lt() {
    awk "BEGIN { exit !($1 < $2) }"
}

# Subtrair floats
float_sub() {
    awk "BEGIN { printf \"%.2f\", $1 - $2 }"
}

# === COMANDO: list ===
cmd_list() {
    local total=0
    local format="%-35s %-12s %-10s %-15s %s\n"

    printf "\n"
    printf "$format" "ID" "CONFIDENCE" "STATUS" "DOMÍNIO" "ÚLTIMA OBS."
    printf "$format" "---" "---" "---" "---" "---"

    # Instintos ativos
    if [ -d "$ACTIVE_DIR" ]; then
        for f in "$ACTIVE_DIR"/*.md; do
            [ -f "$f" ] || continue
            [ "$(basename "$f")" = "README.md" ] && continue
            local id=$(get_field "$f" "id")
            local conf=$(get_field "$f" "confidence")
            local domain=$(get_field "$f" "domain")
            local last=$(get_field "$f" "last_observed")
            printf "$format" "$id" "$conf" "ativo" "$domain" "$last"
            total=$((total + 1))
        done
    fi

    # Instintos tentativos
    if [ -d "$TENTATIVE_DIR" ]; then
        for f in "$TENTATIVE_DIR"/*.md; do
            [ -f "$f" ] || continue
            [ "$(basename "$f")" = "README.md" ] && continue
            local id=$(get_field "$f" "id")
            local conf=$(get_field "$f" "confidence")
            local domain=$(get_field "$f" "domain")
            local last=$(get_field "$f" "last_observed")
            printf "$format" "$id" "$conf" "tentativo" "$domain" "$last"
            total=$((total + 1))
        done
    fi

    printf "\nTotal: %d instintos\n" "$total"
}

# === COMANDO: decay ===
cmd_decay() {
    local today=$(date +%Y-%m-%d)
    local decayed=0
    local dirs=("$ACTIVE_DIR" "$TENTATIVE_DIR")

    for dir in "${dirs[@]}"; do
        [ -d "$dir" ] || continue
        for f in "$dir"/*.md; do
            [ -f "$f" ] || continue
            [ "$(basename "$f")" = "README.md" ] && continue

            local last_observed=$(get_field "$f" "last_observed")
            local last_decay=$(get_field "$f" "last_decay")
            local confidence=$(get_field "$f" "confidence")
            local id=$(get_field "$f" "id")

            # Calcular semanas desde última observação
            local days=$(days_since "$last_observed")
            local weeks=$((days / 7))

            if [ "$weeks" -ge 1 ]; then
                # Calcular semanas desde último decay para evitar dupla aplicação
                local decay_days=$(days_since "${last_decay:-$last_observed}")
                local decay_weeks=$((decay_days / 7))

                if [ "$decay_weeks" -ge 1 ]; then
                    local new_conf=$(float_sub "$confidence" "$WEEKLY_DECAY")
                    set_field "$f" "confidence" "$new_conf"
                    set_field "$f" "last_decay" "$today"
                    echo "[decay] $id: $confidence → $new_conf"
                    decayed=$((decayed + 1))
                fi
            fi
        done
    done

    echo ""
    echo "Decay aplicado a $decayed instintos."
}

# === COMANDO: prune ===
cmd_prune() {
    local pruned=0
    local dirs=("$ACTIVE_DIR" "$TENTATIVE_DIR")

    for dir in "${dirs[@]}"; do
        [ -d "$dir" ] || continue
        for f in "$dir"/*.md; do
            [ -f "$f" ] || continue
            [ "$(basename "$f")" = "README.md" ] && continue

            local confidence=$(get_field "$f" "confidence")
            local id=$(get_field "$f" "id")

            if float_lt "$confidence" "$AUTO_REMOVE_BELOW"; then
                echo "[prune] Removendo $id (confidence: $confidence < $AUTO_REMOVE_BELOW)"
                rm "$f"
                pruned=$((pruned + 1))
            fi
        done
    done

    echo ""
    echo "$pruned instintos removidos."
}

# === COMANDO: promote ===
cmd_promote() {
    local promoted=0

    [ -d "$TENTATIVE_DIR" ] || { echo "Nenhum instinto tentativo."; return; }
    mkdir -p "$ACTIVE_DIR" 2>/dev/null

    for f in "$TENTATIVE_DIR"/*.md; do
        [ -f "$f" ] || continue
        [ "$(basename "$f")" = "README.md" ] && continue

        local confidence=$(get_field "$f" "confidence")
        local id=$(get_field "$f" "id")

        if ! float_lt "$confidence" "$ACTIVE_MIN"; then
            echo "[promote] $id: confidence $confidence >= $ACTIVE_MIN → movendo para active/"
            mv "$f" "$ACTIVE_DIR/"
            promoted=$((promoted + 1))
        fi
    done

    echo ""
    echo "$promoted instintos promovidos para active/."
}

# === COMANDO: graduate ===
cmd_graduate() {
    local target_id="${1:-}"

    if [ -z "$target_id" ]; then
        echo "Uso: instinct-manager.sh graduate <id>"
        echo ""
        echo "Candidatos a graduação (confidence >= $GRADUATION_MIN):"

        [ -d "$ACTIVE_DIR" ] || { echo "  Nenhum."; return; }

        local found=0
        for f in "$ACTIVE_DIR"/*.md; do
            [ -f "$f" ] || continue
            [ "$(basename "$f")" = "README.md" ] && continue

            local confidence=$(get_field "$f" "confidence")
            local id=$(get_field "$f" "id")
            local sessions=$(get_field "$f" "sessions_observed")

            if ! float_lt "$confidence" "$GRADUATION_MIN"; then
                echo "  - $id (confidence: $confidence, sessões: ${sessions:-?})"
                found=$((found + 1))
            fi
        done

        [ "$found" -eq 0 ] && echo "  Nenhum candidato."
        return
    fi

    # Buscar o instinto pelo id
    local source_file=""
    for f in "$ACTIVE_DIR"/*.md; do
        [ -f "$f" ] || continue
        local fid=$(get_field "$f" "id")
        if [ "$fid" = "$target_id" ]; then
            source_file="$f"
            break
        fi
    done

    if [ -z "$source_file" ]; then
        echo "[ERRO] Instinto '$target_id' não encontrado em active/."
        exit 1
    fi

    mkdir -p "$GRADUATED_DIR" 2>/dev/null
    local today=$(date +%Y-%m-%d)
    set_field "$source_file" "graduated" "$today"
    mv "$source_file" "$GRADUATED_DIR/"
    echo "[graduate] $target_id movido para graduated/ em $today"
    echo "Próximo passo: criar rule, skill ou check de auditoria correspondente."
}

# === COMANDO: stats ===
cmd_stats() {
    local active_count=0
    local tentative_count=0
    local graduated_count=0
    local obs_count=0
    local obs_size="0"

    # Contar instintos
    if [ -d "$ACTIVE_DIR" ]; then
        active_count=$(find "$ACTIVE_DIR" -name "*.md" ! -name "README.md" 2>/dev/null | wc -l)
    fi
    if [ -d "$TENTATIVE_DIR" ]; then
        tentative_count=$(find "$TENTATIVE_DIR" -name "*.md" ! -name "README.md" 2>/dev/null | wc -l)
    fi
    if [ -d "$GRADUATED_DIR" ]; then
        graduated_count=$(find "$GRADUATED_DIR" -name "*.md" ! -name "README.md" 2>/dev/null | wc -l)
    fi

    # Contar observações
    if [ -f "$OBS_FILE" ]; then
        obs_count=$(wc -l < "$OBS_FILE" 2>/dev/null || echo 0)
        obs_size=$(du -h "$OBS_FILE" 2>/dev/null | cut -f1 || echo "0")
    fi

    printf "\n=== Estatísticas do Sistema de Aprendizado ===\n\n"
    printf "Instintos ativos:     %d\n" "$active_count"
    printf "Instintos tentativos: %d\n" "$tentative_count"
    printf "Instintos graduados:  %d\n" "$graduated_count"
    printf "Total de instintos:   %d\n" "$((active_count + tentative_count + graduated_count))"
    printf "\n"
    printf "Observações:          %d linhas\n" "$obs_count"
    printf "Tamanho observações:  %s\n" "$obs_size"
    printf "\n"
    printf "Thresholds ativos:\n"
    printf "  Ativo mínimo:       %s\n" "$ACTIVE_MIN"
    printf "  Graduação mínimo:   %s\n" "$GRADUATION_MIN"
    printf "  Remoção abaixo de:  %s\n" "$AUTO_REMOVE_BELOW"
    printf "  Decay semanal:      %s\n" "$WEEKLY_DECAY"
    printf "\n"
}

# === COMANDO: help ===
cmd_help() {
    cat <<'HELP'

instinct-manager.sh — CLI do sistema de aprendizado contínuo

Comandos:
  list       Lista todos os instintos (ativos + tentativos)
  decay      Aplica decay semanal a instintos sem observação recente
  prune      Remove instintos com confidence abaixo do threshold
  promote    Move instintos tentativos com confidence >= 0.5 para active/
  graduate   Lista candidatos a graduação ou gradua um instinto específico
  stats      Mostra estatísticas do sistema
  help       Esta mensagem

Uso:
  bash scripts/instinct-manager.sh list
  bash scripts/instinct-manager.sh decay
  bash scripts/instinct-manager.sh prune
  bash scripts/instinct-manager.sh promote
  bash scripts/instinct-manager.sh graduate [id]
  bash scripts/instinct-manager.sh stats

HELP
}

# === DISPATCHER ===
COMMAND="${1:-help}"
shift 2>/dev/null || true

case "$COMMAND" in
    list)     cmd_list ;;
    decay)    cmd_decay ;;
    prune)    cmd_prune ;;
    promote)  cmd_promote ;;
    graduate) cmd_graduate "$@" ;;
    stats)    cmd_stats ;;
    help)     cmd_help ;;
    *)
        echo "[ERRO] Comando desconhecido: $COMMAND"
        cmd_help
        exit 1
        ;;
esac
