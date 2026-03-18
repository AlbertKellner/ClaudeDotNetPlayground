#!/bin/bash
# Hook: pre-commit-gate.sh
# Propósito: Verificar pré-requisitos antes de permitir commit
# Uso: Executar manualmente ou via hook antes de git commit
#
# Verifica:
# 1. dotnet build passou sem erros
# 2. dotnet test passou sem erros
# 3. Docker daemon disponível (quando necessário)

set -e

echo "[Pre-commit gate] Verificando pré-requisitos..."

# Verificar se estamos no diretório do projeto
if [ ! -f "src/ClaudeDotNetPlayground/ClaudeDotNetPlayground.csproj" ]; then
  echo "[ERRO] Não encontrado o arquivo de projeto. Execute a partir da raiz do repositório."
  exit 1
fi

# Verificar compilação
echo "[Pre-commit gate] Verificando compilação..."
if ! dotnet build src/ClaudeDotNetPlayground/ClaudeDotNetPlayground.csproj --verbosity quiet 2>/dev/null; then
  echo "[BLOQUEADO] dotnet build falhou. Corrija os erros antes de fazer commit."
  exit 1
fi

echo "[Pre-commit gate] Compilação OK."

# Verificar testes
echo "[Pre-commit gate] Verificando testes..."
if ! dotnet test tests/ClaudeDotNetPlayground.Tests/ClaudeDotNetPlayground.Tests.csproj --verbosity quiet 2>/dev/null; then
  echo "[BLOQUEADO] dotnet test falhou. Corrija os testes antes de fazer commit."
  exit 1
fi

echo "[Pre-commit gate] Testes OK."
echo "[Pre-commit gate] Todos os pré-requisitos verificados. Commit permitido."
exit 0
