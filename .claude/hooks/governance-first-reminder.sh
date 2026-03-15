#!/bin/bash
# =============================================================================
# Hook: governance-first-reminder.sh
# =============================================================================
# PROPÓSITO:
#   Reforça o modelo operacional "governança antes de implementação".
#   Detecta quando arquivos de implementação foram modificados sem
#   que arquivos de governança correspondentes tenham sido atualizados,
#   e emite um lembrete.
#
# GATILHO PRETENDIDO: Pre-commit
#
# COMPORTAMENTO QUE REFORÇA:
#   "Se a mensagem do usuário introduzir ou alterar uma definição durável,
#    a governança deve ser atualizada antes de qualquer mudança de código."
#
# POR QUE É PLACEHOLDER:
#   Para determinar quando governança "deveria ter sido atualizada",
#   é necessário conhecer a estrutura de pastas da implementação,
#   as convenções de nomenclatura e o mapeamento entre módulos de
#   implementação e artefatos de governança correspondentes.
#   Isso só é possível após a stack e arquitetura serem definidas.
#
# COMO ADAPTAR:
#   1. Mapear pastas de implementação para arquivos de governança
#      (ex: src/domain/ → Instructions/business/domain-model.md)
#   2. Definir heurísticas para detectar quando governança deveria ter mudado
#   3. Decidir se o hook é bloqueante (exit 1) ou apenas aviso (exit 0)
#   4. Registrar a decisão em Instructions/decisions/
# =============================================================================

echo "[governance-first-reminder] Verificando política de governança primeiro..."

# PLACEHOLDER: detectar mudanças em código sem mudança correspondente em governança
# Implementação real dependente da estrutura de pastas do projeto

# Verificação mínima: avisar se arquivos de Instructions/ não foram tocados
# mas arquivos fora de .claude/ e Instructions/ foram modificados
IMPL_CHANGES=$(git diff --cached --name-only 2>/dev/null | grep -v "^Instructions/" | grep -v "^\\.claude/" | grep -v "^README\\.md" | grep -v "^CLAUDE\\.md" | grep -v "^open-questions\\.md" | grep -v "^assumptions-log\\.md" | wc -l)
GOV_CHANGES=$(git diff --cached --name-only 2>/dev/null | grep -E "^Instructions/|^\\.claude/rules/" | wc -l)

if [ "$IMPL_CHANGES" -gt 0 ] && [ "$GOV_CHANGES" -eq 0 ]; then
    echo ""
    echo "ℹ️  LEMBRETE DE GOVERNANÇA:"
    echo "   Arquivos de implementação foram modificados sem alteração na governança."
    echo ""
    echo "   Isso pode ser normal se a implementação já estava coberta pela"
    echo "   governança existente. Se introduziu nova definição durável, verifique:"
    echo "   - Instructions/business/ (regras, invariantes, workflows)"
    echo "   - Instructions/architecture/ (princípios, padrões, decisões)"
    echo "   - Instructions/glossary/ (terminologia nova)"
    echo ""
    echo "   Consulte: .claude/rules/implementation-alignment.md"
    echo ""
fi

# PLACEHOLDER: adicionar verificação real aqui quando a stack for conhecida

exit 0
