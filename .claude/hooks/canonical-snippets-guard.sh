#!/bin/bash
# =============================================================================
# Hook: canonical-snippets-guard.sh
# =============================================================================
# PROPÓSITO:
#   Verifica se snippets normativos registrados como canônicos em
#   Instructions/snippets/canonical-snippets.md ainda estão presentes
#   e preservados nos arquivos de destino correspondentes.
#
# GATILHO PRETENDIDO: Pre-commit
#
# COMPORTAMENTO QUE REFORÇA:
#   "Snippets normativos explicitamente declarados pelo usuário não devem
#    ser reescritos silenciosamente."
#   Referência: .claude/rules/snippet-handling.md
#
# POR QUE É PLACEHOLDER:
#   A verificação real depende de:
#   - Quais snippets canônicos foram registrados (arquivo ainda vazio no bootstrap)
#   - Onde cada snippet está localizado no repositório
#   - Como verificar que o conteúdo foi preservado (hash, substring match, etc.)
#
# COMO ADAPTAR:
#   1. Após registrar snippets em canonical-snippets.md, implementar
#      verificação de hash ou substring dos trechos canônicos
#   2. Mapear cada snippet ao(s) arquivo(s) de destino
#   3. Executar verificação de presença e integridade
# =============================================================================

CANONICAL_SNIPPETS="Instructions/snippets/canonical-snippets.md"

echo "[canonical-snippets-guard] Verificando integridade de snippets normativos..."

if [ ! -f "$CANONICAL_SNIPPETS" ]; then
    echo "   canonical-snippets.md não encontrado. Ignorando verificação."
    exit 0
fi

# PLACEHOLDER: verificar se há snippets registrados
SNIPPET_COUNT=$(grep -c "^## Snippet" "$CANONICAL_SNIPPETS" 2>/dev/null || echo "0")

if [ "$SNIPPET_COUNT" -eq 0 ]; then
    echo "   Nenhum snippet canônico registrado ainda. Verificação ignorada."
    exit 0
fi

echo "   $SNIPPET_COUNT snippet(s) canônico(s) registrado(s)."
echo "   [PLACEHOLDER] Verificação de integridade não implementada."
echo "   Adapte este hook após registrar snippets com seus arquivos de destino."
echo "   Consulte: Instructions/snippets/README.md"

exit 0
