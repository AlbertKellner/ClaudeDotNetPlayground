#!/bin/bash
# Hook: pre-commit-gate.sh
# Propósito: Verificar pré-requisitos antes de permitir commit
# Uso: Executar manualmente ou via hook antes de git commit
#
# Verifica:
# 1. dotnet build passou sem erros
# 2. dotnet test passou sem erros
#
# Nota: Os paths são resolvidos dinamicamente a partir da raiz do repositório,
# eliminando dependência de nomes hardcoded que podem ficar obsoletos.

set -e

REPO_ROOT="$(cd "$(dirname "$0")/../.." && pwd)"

echo "[Pre-commit gate] Verificando pré-requisitos..."

# Descobrir projeto API dinamicamente (único .csproj em src/ que não é UnitTest/IntegrationTest)
API_PROJECT=$(find "$REPO_ROOT/src" -maxdepth 2 -name "*.csproj" -type f \
  | grep -v -i "test" \
  | head -1)

if [ -z "$API_PROJECT" ]; then
  echo "[ERRO] Nenhum projeto API encontrado em src/. Execute a partir da raiz do repositório."
  exit 1
fi

echo "[Pre-commit gate] Projeto API: $API_PROJECT"

# Verificar compilação
echo "[Pre-commit gate] Verificando compilação..."
if ! dotnet build "$API_PROJECT" --verbosity quiet 2>/dev/null; then
  echo "[BLOQUEADO] dotnet build falhou. Corrija os erros antes de fazer commit."
  exit 1
fi

echo "[Pre-commit gate] Compilação OK."

# Descobrir projeto de testes dinamicamente
TEST_PROJECT=$(find "$REPO_ROOT/src" -maxdepth 2 -name "*.csproj" -type f \
  | grep -i "test" \
  | head -1)

if [ -n "$TEST_PROJECT" ]; then
  echo "[Pre-commit gate] Verificando testes ($TEST_PROJECT)..."
  if ! dotnet test "$TEST_PROJECT" --verbosity quiet 2>/dev/null; then
    echo "[BLOQUEADO] dotnet test falhou. Corrija os testes antes de fazer commit."
    exit 1
  fi
  echo "[Pre-commit gate] Testes OK."
else
  echo "[Pre-commit gate] Nenhum projeto de testes encontrado — verificação ignorada."
fi

echo "[Pre-commit gate] Todos os pré-requisitos verificados. Commit permitido."
exit 0
