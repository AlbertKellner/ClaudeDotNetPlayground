#!/usr/bin/env bash
# =============================================================================
# setup-env.sh — Bootstrap de Ambiente para Albert.Playground.ECS.AOT
# =============================================================================
# MODELO DECLARATIVO — não execute diretamente.
# Copie este script para a ferramenta externa de configuração de container.
# O ambiente resultante deve chegar ao agente já pronto para uso.
#
# Todas as secrets e variáveis listadas em scripts/required-vars.md devem
# estar disponíveis na ferramenta externa ANTES de aplicar este script.
# Se alguma entrada obrigatória estiver ausente, o script emite [ERR] e
# encerra com código não-zero.
#
# Pré-requisitos cobertos:
#   1. Arquivo .env com DD_API_KEY e EXTRA_CA_CERT
#   2. Docker daemon em execução
#   3. ~/.docker/config.json com proxy HTTP configurado
#   4. Certificado CA do proxy disponível
#
# Ver: scripts/required-vars.md   — variáveis e secrets a cadastrar na ferramenta externa
#      scripts/container-setup.md — dependências de sistema do container
# =============================================================================

set -uo pipefail

ERRORS=0

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ENV_FILE="${REPO_ROOT}/.env"
CA_CERT_PATH="/usr/local/share/ca-certificates/swp-ca-production.crt"
DOCKER_SOCK="/var/run/docker.sock"
DOCKER_CONFIG="${HOME}/.docker/config.json"

# Contadores para o sumário final
READY=0
PREPARED=0
WARNINGS=0
ERRORS=0

print_header() {
  echo ""
  echo "========================================"
  echo "  Bootstrap de Ambiente"
  echo "  $(date '+%Y-%m-%d %H:%M:%S')"
  echo "========================================"
  echo ""
}

print_item() {
  local status="$1"   # OK | PREP | WARN | ERR
  local label="$2"
  local detail="${3:-}"
  case "$status" in
    OK)   echo "  [OK]   ${label}${detail:+ — ${detail}}" ; READY=$((READY+1)) ;;
    PREP) echo "  [PREP] ${label}${detail:+ — ${detail}}" ; PREPARED=$((PREPARED+1)) ;;
    WARN) echo "  [WARN] ${label}${detail:+ — ${detail}}" ; WARNINGS=$((WARNINGS+1)) ;;
    ERR)  echo "  [ERR]  ${label}${detail:+ — ${detail}}" ; ERRORS=$((ERRORS+1)) ;;
  esac
}

print_summary() {
  echo ""
  echo "----------------------------------------"
  echo "  Sumário: ${READY} prontos | ${PREPARED} preparados | ${WARNINGS} avisos | ${ERRORS} erros"
  echo "----------------------------------------"
  echo ""
}

# =============================================================================
# 1. Verificar e gerar .env
# =============================================================================
setup_env_file() {
  local dd_api_key
  dd_api_key="$(printenv DD_API_KEY 2>/dev/null || true)"

  local extra_ca_cert=""
  if [ -f "$CA_CERT_PATH" ]; then
    extra_ca_cert="$(base64 -w 0 "$CA_CERT_PATH" 2>/dev/null || base64 "$CA_CERT_PATH" 2>/dev/null || true)"
  fi

  # Verificar se .env já está correto
  if [ -f "$ENV_FILE" ]; then
    local current_key
    current_key="$(grep '^DD_API_KEY=' "$ENV_FILE" | cut -d= -f2- | tr -d '\n' || true)"
    if [ -n "$current_key" ] && [ "$current_key" != "" ]; then
      print_item "OK" ".env" "DD_API_KEY presente"
      return 0
    fi
  fi

  # Criar ou atualizar .env
  {
    if [ -n "$dd_api_key" ]; then
      echo "DD_API_KEY=${dd_api_key}"
    else
      echo "DD_API_KEY="
    fi
    if [ -n "$extra_ca_cert" ]; then
      echo "EXTRA_CA_CERT=${extra_ca_cert}"
    fi
  } > "$ENV_FILE"

  if [ -n "$dd_api_key" ]; then
    print_item "PREP" ".env" "criado com DD_API_KEY e EXTRA_CA_CERT"
  else
    print_item "ERR" ".env" "DD_API_KEY ausente no ambiente — cadastre-a na ferramenta externa (ver scripts/required-vars.md)"
  fi
}

# =============================================================================
# 2. Verificar e iniciar Docker daemon
# =============================================================================
setup_docker_daemon() {
  if [ -S "$DOCKER_SOCK" ]; then
    if docker info &>/dev/null 2>&1; then
      print_item "OK" "Docker daemon" "socket ativo"
      return 0
    fi
  fi

  echo "  [PREP] Docker daemon — iniciando (dockerd)..."
  dockerd --host=unix://"$DOCKER_SOCK" &>/tmp/dockerd.log &
  local daemon_pid=$!

  local attempts=0
  while [ $attempts -lt 12 ]; do
    sleep 2
    if [ -S "$DOCKER_SOCK" ] && docker info &>/dev/null 2>&1; then
      print_item "PREP" "Docker daemon" "iniciado (PID ${daemon_pid})"
      return 0
    fi
    attempts=$((attempts+1))
  done

  print_item "WARN" "Docker daemon" "socket não apareceu após 24s — verificar /tmp/dockerd.log"
  return 1
}

# =============================================================================
# 3. Configurar proxy HTTP no Docker config
# =============================================================================
setup_docker_proxy() {
  if [ -f "$DOCKER_CONFIG" ] && grep -q '"proxies"' "$DOCKER_CONFIG" 2>/dev/null; then
    print_item "OK" "Docker proxy" "~/.docker/config.json já configurado"
    return 0
  fi

  local http_proxy="${HTTP_PROXY:-${http_proxy:-}}"
  local https_proxy="${HTTPS_PROXY:-${https_proxy:-}}"
  local no_proxy="${NO_PROXY:-${no_proxy:-}}"

  if [ -z "$http_proxy" ] && [ -z "$https_proxy" ]; then
    print_item "WARN" "Docker proxy" "HTTP_PROXY não encontrado no ambiente — containers podem ter problemas de DNS/TLS"
    return 0
  fi

  mkdir -p "$(dirname "$DOCKER_CONFIG")"
  cat > "$DOCKER_CONFIG" <<EOF
{
  "proxies": {
    "default": {
      "httpProxy": "${http_proxy}",
      "httpsProxy": "${https_proxy}",
      "noProxy": "${no_proxy}"
    }
  }
}
EOF
  print_item "PREP" "Docker proxy" "~/.docker/config.json criado com HTTP_PROXY=${http_proxy}"
}

# =============================================================================
# 4. Verificar certificado CA do proxy
# =============================================================================
check_ca_cert() {
  if [ -f "$CA_CERT_PATH" ]; then
    print_item "OK" "CA do proxy" "${CA_CERT_PATH}"
  else
    print_item "WARN" "CA do proxy" "${CA_CERT_PATH} não encontrado — builds com dotnet restore podem falhar por TLS"
  fi
}

# =============================================================================
# 5. Verificar .NET SDK (delegar ao setup-dotnet.sh se necessário)
# =============================================================================
check_dotnet() {
  if command -v dotnet >/dev/null 2>&1 && dotnet --list-sdks 2>/dev/null | grep -q "^10\."; then
    local sdk_version
    sdk_version="$(dotnet --list-sdks 2>/dev/null | grep '^10\.' | tail -1 | cut -d' ' -f1)"
    print_item "OK" ".NET SDK" "versão ${sdk_version}"
  else
    print_item "WARN" ".NET SDK 10" "não encontrado — executar scripts/setup-dotnet.sh antes do build"
  fi
}

# =============================================================================
# Execução principal
# =============================================================================
main() {
  cd "$REPO_ROOT"

  print_header

  echo "  Verificando pré-requisitos..."
  echo ""

  setup_env_file
  setup_docker_daemon
  setup_docker_proxy
  check_ca_cert
  check_dotnet

  print_summary

  if [ $ERRORS -gt 0 ]; then
    echo "  ERROS CRÍTICOS: ${ERRORS} entrada(s) obrigatória(s) ausente(s)."
    echo "  Consulte scripts/required-vars.md e cadastre as entradas faltantes"
    echo "  na ferramenta externa de configuração de container antes de continuar."
    echo ""
  fi

  if [ $WARNINGS -gt 0 ]; then
    echo "  Avisos presentes. Verifique os itens marcados com [WARN] acima."
    echo "  O pipeline pode falhar se os avisos não forem resolvidos."
    echo ""
  fi

  if [ $ERRORS -eq 0 ] && [ $PREPARED -gt 0 ]; then
    echo "  Ambiente preparado. Pronto para executar o pipeline."
  elif [ $ERRORS -eq 0 ]; then
    echo "  Ambiente já estava pronto. Nenhuma ação necessária."
  fi
  echo ""

  return $ERRORS
}

main "$@"
exit $?
