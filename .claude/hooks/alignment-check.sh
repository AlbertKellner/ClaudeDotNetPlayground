#!/bin/bash
# =============================================================================
# Hook: alignment-check.sh
# =============================================================================
# PROPÓSITO:
#   Sinaliza inconsistências entre artefatos de negócio, BDD, contratos
#   e implementação. Reforça que mudanças em um artefato devem ser
#   propagadas para os demais quando relevante.
#
# GATILHO PRETENDIDO: Pre-commit / CI
#
# COMPORTAMENTO QUE REFORÇA:
#   "Se negócio muda, avalie BDD, contratos, glossário e implementação.
#    Se contratos mudam, avalie negócio, BDD, glossário e implementação."
#   Referência: .claude/rules/change-propagation.md
#
# POR QUE É PLACEHOLDER:
#   Verificações reais de alinhamento dependem de:
#   - Formato dos contratos (OpenAPI, AsyncAPI, Avro, Protobuf, etc.)
#   - Formato dos cenários BDD (Cucumber .feature files, etc.)
#   - Estrutura do código de domínio
#   - Ferramentas disponíveis (contract testing, BDD runners, etc.)
#
# COMO ADAPTAR:
#   1. Implementar validação de contrato com a ferramenta escolhida
#   2. Implementar execução de cenários BDD
#   3. Implementar verificação de consistência de terminologia
#   4. Definir se é bloqueante ou apenas sinalizador
# =============================================================================

echo "[alignment-check] Verificando alinhamento entre artefatos..."

ERRORS=0
WARNINGS=0

# PLACEHOLDER: verificação 1 — contratos válidos
if ls Instructions/contracts/*.yaml 2>/dev/null | head -1 > /dev/null 2>&1; then
    echo "   [PLACEHOLDER] Validação de contratos YAML: não implementada"
    echo "   Adaptar com: openapi-schema-validator, spectral, asyncapi-validator, etc."
    WARNINGS=$((WARNINGS + 1))
fi

# PLACEHOLDER: verificação 2 — cenários BDD existem para comportamentos chave
if ls Instructions/bdd/*.feature 2>/dev/null | head -1 > /dev/null 2>&1; then
    echo "   [PLACEHOLDER] Execução de cenários BDD: não implementada"
    echo "   Adaptar com: Cucumber, Behave, SpecFlow, etc."
    WARNINGS=$((WARNINGS + 1))
fi

# PLACEHOLDER: verificação 3 — terminologia consistente
if [ -f "Instructions/glossary/ubiquitous-language.md" ]; then
    echo "   [PLACEHOLDER] Verificação de terminologia: não implementada"
    echo "   Adaptar com: grep de termos proibidos em arquivos de código/contratos"
    WARNINGS=$((WARNINGS + 1))
fi

if [ "$ERRORS" -gt 0 ]; then
    echo ""
    echo "❌ $ERRORS erro(s) de alinhamento encontrados. Resolva antes de commitar."
    exit 1
fi

if [ "$WARNINGS" -gt 0 ]; then
    echo ""
    echo "⚠️  $WARNINGS verificação(ões) placeholder. Adapte este hook quando a stack for definida."
    echo "   Consulte: .claude/hooks/README.md"
fi

echo "   [alignment-check] Verificação concluída."
exit 0
