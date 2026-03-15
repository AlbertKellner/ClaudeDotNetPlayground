#!/bin/bash
# =============================================================================
# Hook: ambiguity-guard.sh
# =============================================================================
# PROPÓSITO:
#   Verifica se há dúvidas abertas marcadas como bloqueantes em
#   open-questions.md. Se houver, impede o commit ou emite aviso,
#   dependendo da configuração.
#
# GATILHO PRETENDIDO: Pre-commit
#
# COMPORTAMENTO QUE REFORÇA:
#   "Se houver dúvida material bloqueante, o assistente não deve implementar
#    antes de obter confirmação."
#   Referência: .claude/rules/ambiguity-handling.md
#
# POR QUE É PLACEHOLDER:
#   A verificação de "bloqueante" depende do formato específico adotado
#   em open-questions.md. O formato atual usa markdown — a verificação
#   pode ser feita via grep, mas precisa ser calibrada conforme o
#   repositório evolui.
#
# COMO ADAPTAR:
#   1. Ajustar o padrão de grep para corresponder ao formato real usado
#   2. Decidir se é bloqueante (exit 1) ou apenas aviso (exit 0)
#   3. Considerar integrar com sistema de tracking de issues se disponível
# =============================================================================

OPEN_QUESTIONS_FILE="open-questions.md"

echo "[ambiguity-guard] Verificando dúvidas abertas bloqueantes..."

if [ ! -f "$OPEN_QUESTIONS_FILE" ]; then
    echo "   open-questions.md não encontrado. Ignorando verificação."
    exit 0
fi

# Verificar se há itens marcados como bloqueantes ainda abertos
# PLACEHOLDER: ajustar padrão conforme formato real do arquivo
BLOCKING_COUNT=$(grep -c "bloqueante.*sim\|bloqueante: sim\|Bloqueante.*Sim\|⛔" "$OPEN_QUESTIONS_FILE" 2>/dev/null || echo "0")

if [ "$BLOCKING_COUNT" -gt 0 ]; then
    echo ""
    echo "⚠️  DÚVIDAS BLOQUEANTES ABERTAS: $BLOCKING_COUNT item(ns)"
    echo "   Consulte open-questions.md para detalhes."
    echo ""
    echo "   Dúvidas bloqueantes devem ser resolvidas antes de implementar"
    echo "   a parte afetada. Verifique se os artefatos commitados dependem"
    echo "   dessas dúvidas."
    echo ""
    # PLACEHOLDER: mudar para exit 1 se quiser bloquear o commit
    exit 0
fi

echo "   Nenhuma dúvida bloqueante encontrada."
exit 0
