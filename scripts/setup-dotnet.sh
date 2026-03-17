#!/usr/bin/env bash
set -euo pipefail

REQUIRED_MAJOR="10"
REQUIRED_VERSION="${DOTNET_VERSION:-10.0.100}"
INSTALL_DIR="${DOTNET_INSTALL_DIR:-$HOME/.dotnet}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

export DOTNET_ROOT="$INSTALL_DIR"
export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH"

has_required_sdk() {
  command -v dotnet >/dev/null 2>&1 && dotnet --list-sdks 2>/dev/null | grep -q "^${REQUIRED_MAJOR}\."
}

if has_required_sdk; then
  echo ".NET ${REQUIRED_MAJOR} já está disponível."
else
  echo ".NET ${REQUIRED_MAJOR} não encontrado. Instalando versão $REQUIRED_VERSION em: $INSTALL_DIR"
  mkdir -p "$INSTALL_DIR"
  curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
  chmod +x /tmp/dotnet-install.sh
  /tmp/dotnet-install.sh --version "$REQUIRED_VERSION" --install-dir "$INSTALL_DIR"
  export DOTNET_ROOT="$INSTALL_DIR"
  export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH"
fi

echo ""
echo "SDKs instalados:"
dotnet --list-sdks

if ! has_required_sdk; then
  echo "Falha: .NET ${REQUIRED_MAJOR} continua indisponível após instalação."
  exit 1
fi

echo ""
echo "Criando/atualizando global.json em $REPO_ROOT"
cat > "$REPO_ROOT/global.json" <<EOF
{
  "sdk": {
    "version": "$REQUIRED_VERSION",
    "rollForward": "latestFeature",
    "errorMessage": "O SDK do .NET 10 não foi encontrado. Execute ./scripts/setup-dotnet.sh"
  }
}
EOF

echo ""
echo "Ambiente configurado com sucesso."
dotnet --info
