#!/bin/bash
# =============================================================================
# Hook: governance-change-detector.sh
# =============================================================================
# PROPÓSITO:
#   Detecta mudanças em arquivos de governança (Instructions/, .claude/rules/)
#   e lembra o assistente/desenvolvedor de verificar se a implementação
#   permanece alinhada com as definições atualizadas.
#
# GATILHO PRETENDIDO: Pre-commit
#
# COMPORTAMENTO QUE REFORÇA:
#   "Quando a governança muda, a implementação deve ser revisada para
#    garantir que ainda está alinhada."
#
# POR QUE É PLACEHOLDER:
#   O enforcement real depende de como o repositório será estruturado,
#   qual sistema de CI/CD será usado e quais ferramentas de lint/test
#   estão disponíveis. Substitua pela implementação real quando a
#   stack do repositório for conhecida.
#
# COMO ADAPTAR:
#   1. Definir quais arquivos de implementação correspondem a quais
#      arquivos de governança (ex: business-rules.md → src/domain/)
#   2. Implementar verificação de data de modificação ou hash
#   3. Integrar com o pipeline de CI/CD da stack escolhida
#   4. Registrar a decisão de tooling em Instructions/decisions/
# =============================================================================

GOVERNANCE_DIRS=("Instructions/" ".claude/rules/")
CHANGED_GOVERNANCE=()

echo "[governance-change-detector] Verificando mudanças em arquivos de governança..."

for dir in "${GOVERNANCE_DIRS[@]}"; do
    # PLACEHOLDER: substituir por verificação real de git diff ou hash
    if git diff --cached --name-only 2>/dev/null | grep -q "^${dir}"; then
        CHANGED_GOVERNANCE+=("$dir")
    fi
done

if [ ${#CHANGED_GOVERNANCE[@]} -gt 0 ]; then
    echo ""
    echo "⚠️  GOVERNANÇA MODIFICADA:"
    for changed in "${CHANGED_GOVERNANCE[@]}"; do
        echo "   - $changed"
    done
    echo ""
    echo "   Lembre-se: verifique se a implementação ainda está alinhada"
    echo "   com as definições de governança atualizadas."
    echo ""
    echo "   Consulte: .claude/skills/review-alignment/SKILL.md"
    echo ""
fi

# PLACEHOLDER: adicionar validação real aqui quando a stack for conhecida
# Exemplos:
#   - Executar testes de contrato (Pact, Dredd, etc.)
#   - Executar testes de BDD (Cucumber, Behave, etc.)
#   - Executar lint de schemas (ajv, openapi-schema-validator, etc.)

exit 0
