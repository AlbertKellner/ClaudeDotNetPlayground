#!/bin/bash
# Script: governance-audit.sh
# Propósito: Verificar automaticamente a consistência estrutural dos arquivos de governança.
# Execução: bash scripts/governance-audit.sh
# Saída: Relatório com [OK] / [FALHA] por verificação.
#
# Este script NÃO verifica conteúdo semântico (ex: "a regra está correta?").
# Verifica apenas consistência estrutural: imports, contagens, referências, alinhamento.

set -euo pipefail

REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
FAILURES=0
TOTAL=0

pass() {
  TOTAL=$((TOTAL + 1))
  echo "[OK]    $1"
}

fail() {
  TOTAL=$((TOTAL + 1))
  FAILURES=$((FAILURES + 1))
  echo "[FALHA] $1"
  if [ -n "${2:-}" ]; then
    echo "        Detalhe: $2"
  fi
}

echo "=== Auditoria de Governança ==="
echo "Repositório: $REPO_ROOT"
echo "Data: $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

# ---------------------------------------------------------------------------
# 1. Verificar que todos os arquivos .md em Instructions/ estão importados no CLAUDE.md
# ---------------------------------------------------------------------------
echo "--- 1. Imports do CLAUDE.md ---"

CLAUDE_MD="$REPO_ROOT/CLAUDE.md"
MISSING_IMPORTS=""

while IFS= read -r file; do
  relative="${file#$REPO_ROOT/}"
  if ! grep -qF "@$relative" "$CLAUDE_MD"; then
    MISSING_IMPORTS="$MISSING_IMPORTS $relative"
  fi
done < <(find "$REPO_ROOT/Instructions" -name "*.md" -type f | sort)

if [ -z "$MISSING_IMPORTS" ]; then
  pass "Todos os arquivos de Instructions/ estão importados no CLAUDE.md"
else
  fail "Arquivos de Instructions/ ausentes nos imports do CLAUDE.md" "$MISSING_IMPORTS"
fi

# Verificar rules
MISSING_RULES_IMPORTS=""
while IFS= read -r file; do
  relative="${file#$REPO_ROOT/}"
  if ! grep -qF "@$relative" "$CLAUDE_MD"; then
    MISSING_RULES_IMPORTS="$MISSING_RULES_IMPORTS $relative"
  fi
done < <(find "$REPO_ROOT/.claude/rules" -name "*.md" -type f | sort)

if [ -z "$MISSING_RULES_IMPORTS" ]; then
  pass "Todos os arquivos de .claude/rules/ estão importados no CLAUDE.md"
else
  fail "Arquivos de .claude/rules/ ausentes nos imports do CLAUDE.md" "$MISSING_RULES_IMPORTS"
fi

# ---------------------------------------------------------------------------
# 2. Contagem de rules no README.md
# ---------------------------------------------------------------------------
echo ""
echo "--- 2. Contagens no README.md ---"

README="$REPO_ROOT/README.md"
ACTUAL_RULES_COUNT=$(find "$REPO_ROOT/.claude/rules" -name "*.md" -type f | wc -l)
README_RULES_COUNT=$(grep -oP '\d+(?=\s*rules)' "$README" 2>/dev/null | head -1 || echo "0")

if [ "$ACTUAL_RULES_COUNT" = "$README_RULES_COUNT" ]; then
  pass "Contagem de rules no README.md ($README_RULES_COUNT) corresponde ao real ($ACTUAL_RULES_COUNT)"
else
  fail "Contagem de rules no README.md ($README_RULES_COUNT) não corresponde ao real ($ACTUAL_RULES_COUNT)"
fi

# Contagem de skills
ACTUAL_SKILLS_COUNT=$(find "$REPO_ROOT/.claude/skills" -mindepth 1 -maxdepth 1 -type d | wc -l)
README_SKILLS_COUNT=$(grep -oP '\d+(?=\s*skills)' "$README" 2>/dev/null | head -1 || echo "0")

if [ "$ACTUAL_SKILLS_COUNT" = "$README_SKILLS_COUNT" ]; then
  pass "Contagem de skills no README.md ($README_SKILLS_COUNT) corresponde ao real ($ACTUAL_SKILLS_COUNT)"
else
  fail "Contagem de skills no README.md ($README_SKILLS_COUNT) não corresponde ao real ($ACTUAL_SKILLS_COUNT)"
fi

# ---------------------------------------------------------------------------
# 3. Variáveis de ambiente no docker-compose.yml documentadas em required-vars.md
# ---------------------------------------------------------------------------
echo ""
echo "--- 3. Variáveis de ambiente ---"

COMPOSE="$REPO_ROOT/docker-compose.yml"
REQUIRED_VARS="$REPO_ROOT/scripts/required-vars.md"

if [ -f "$COMPOSE" ] && [ -f "$REQUIRED_VARS" ]; then
  UNDOCUMENTED_VARS=""
  while IFS= read -r var; do
    # Ignorar variáveis padrão do Docker/Datadog que são internas ao compose
    case "$var" in
      DD_SITE|DD_ENV|DD_HOSTNAME|DD_LOGS_ENABLED|DD_LOGS_CONFIG_CONTAINER_COLLECT_ALL|\
      DD_CONVERT_DD_SITE_FQDN_ENABLED|DD_DOGSTATSD_NON_LOCAL_TRAFFIC|\
      ASPNETCORE_URLS|Datadog__*|ExternalApi__OpenMeteo__*) continue ;;
    esac
    if ! grep -qF "$var" "$REQUIRED_VARS"; then
      UNDOCUMENTED_VARS="$UNDOCUMENTED_VARS $var"
    fi
  done < <(grep -oP '\$\{(\w+)' "$COMPOSE" | sed 's/\${//' | sort -u)

  if [ -z "$UNDOCUMENTED_VARS" ]; then
    pass "Todas as variáveis de ambiente do docker-compose.yml estão documentadas em required-vars.md"
  else
    fail "Variáveis do docker-compose.yml não documentadas em required-vars.md" "$UNDOCUMENTED_VARS"
  fi
else
  pass "docker-compose.yml ou required-vars.md não encontrado — verificação ignorada"
fi

# ---------------------------------------------------------------------------
# 4. Referências a regras de negócio removidas em arquivos ativos
# ---------------------------------------------------------------------------
echo ""
echo "--- 4. Referências a artefatos removidos ---"

# RN-006 e RN-007 foram removidas — não devem aparecer fora de seções de histórico
STALE_REFS=""
while IFS= read -r file; do
  # Ignorar bash-errors-log.md, assumptions-log.md, open-questions.md (logs históricos)
  basename=$(basename "$file")
  case "$basename" in
    bash-errors-log.md|assumptions-log.md) continue ;;
  esac
  # Procurar RN-006 ou RN-007 no arquivo
  if grep -qP 'RN-00[67]' "$file" 2>/dev/null; then
    # Verificar se TODAS as ocorrências estão em contexto histórico:
    # - Linhas de tabela de histórico (começam com | e contêm data YYYY-MM-DD)
    # - Linhas com palavras de remoção/depreciação
    # - Linhas em seções "Substituídas/Depreciadas/Removidas"
    has_active_ref=false
    while IFS= read -r line; do
      if echo "$line" | grep -qiP 'remov|substituíd|depreciad|histórico|revogad|^\|.*20[0-9]{2}-[0-9]{2}-[0-9]{2}'; then
        : # OK — contexto histórico na mesma linha
      else
        # Verificar contexto expandido: buscar keywords nas 3 linhas seguintes à ocorrência
        line_num=$(grep -nF "$line" "$file" 2>/dev/null | head -1 | cut -d: -f1)
        if [ -n "$line_num" ]; then
          context_after=$(sed -n "$((line_num)),$((line_num + 3))p" "$file" 2>/dev/null)
          if echo "$context_after" | grep -qiP 'remov|substituíd|depreciad|status.*remov'; then
            : # OK — contexto próximo contém keywords de remoção
          else
            has_active_ref=true
            break
          fi
        fi
      fi
    done < <(grep -P 'RN-00[67]' "$file" 2>/dev/null)
    if [ "$has_active_ref" = true ]; then
      STALE_REFS="$STALE_REFS $file"
    fi
  fi
done < <(find "$REPO_ROOT/Instructions" "$REPO_ROOT/.claude/rules" "$REPO_ROOT/wiki" -name "*.md" -type f 2>/dev/null | sort)

if [ -z "$STALE_REFS" ]; then
  pass "Nenhuma referência ativa a regras de negócio removidas (RN-006, RN-007)"
else
  fail "Referências ativas a RN-006/RN-007 encontradas em arquivos não-históricos" "$STALE_REFS"
fi

# ---------------------------------------------------------------------------
# 5. Wiki pages existem para features ativas
# ---------------------------------------------------------------------------
echo ""
echo "--- 5. Cobertura da Wiki ---"

WIKI_DIR="$REPO_ROOT/wiki"
if [ -d "$WIKI_DIR" ]; then
  MISSING_WIKI=""
  # Features ativas baseadas nas pastas em Features/
  FEATURES_DIR="$REPO_ROOT/src/Albert.Playground.ECS.AOT.Api/Features"
  if [ -d "$FEATURES_DIR" ]; then
    while IFS= read -r feature_dir; do
      feature_name=$(basename "$feature_dir")
      # Procurar wiki page com nome parcialmente correspondente
      if ! ls "$WIKI_DIR"/Feature-*"$feature_name"* 1>/dev/null 2>&1; then
        # Tentar busca case-insensitive
        if ! find "$WIKI_DIR" -iname "Feature-*${feature_name}*" -type f 2>/dev/null | grep -q .; then
          MISSING_WIKI="$MISSING_WIKI $feature_name"
        fi
      fi
    done < <(find "$FEATURES_DIR" -mindepth 2 -maxdepth 2 -type d | sort)
  fi

  if [ -z "$MISSING_WIKI" ]; then
    pass "Todas as features possuem página correspondente na Wiki"
  else
    fail "Features sem página na Wiki" "$MISSING_WIKI"
  fi

  # Verificar páginas estruturais obrigatórias
  REQUIRED_PAGES="Home.md _Sidebar.md Architecture.md Project-Setup.md Business-Rules.md CI-CD.md"
  MISSING_PAGES=""
  for page in $REQUIRED_PAGES; do
    if [ ! -f "$WIKI_DIR/$page" ]; then
      MISSING_PAGES="$MISSING_PAGES $page"
    fi
  done

  if [ -z "$MISSING_PAGES" ]; then
    pass "Todas as páginas estruturais obrigatórias existem na Wiki"
  else
    fail "Páginas estruturais ausentes na Wiki" "$MISSING_PAGES"
  fi
else
  fail "Diretório wiki/ não encontrado"
fi

# ---------------------------------------------------------------------------
# 6. Rules com workflows procedurais extensos (heurística)
# ---------------------------------------------------------------------------
echo ""
echo "--- 6. Separação Rules/Skills ---"

RULES_WITH_WORKFLOWS=""
while IFS= read -r rule_file; do
  rule_name=$(basename "$rule_file")
  # Contar seções de passos procedurais (### Passo, #### Passo, ### Step)
  step_count=$(grep -cP '###\s+(Passo|Step|Workflow)' "$rule_file" 2>/dev/null || true)
  step_count="${step_count:-0}"
  if [ "$step_count" -gt 5 ]; then
    RULES_WITH_WORKFLOWS="$RULES_WITH_WORKFLOWS $rule_name(${step_count}_passos)"
  fi
done < <(find "$REPO_ROOT/.claude/rules" -name "*.md" -type f | sort)

if [ -z "$RULES_WITH_WORKFLOWS" ]; then
  pass "Nenhuma rule contém workflows procedurais extensos (>5 passos)"
else
  fail "Rules com workflows procedurais extensos (deveriam ser skills)" "$RULES_WITH_WORKFLOWS"
fi

# ---------------------------------------------------------------------------
# 7. Referências cruzadas entre rules apontam para arquivos existentes
# ---------------------------------------------------------------------------
echo ""
echo "--- 7. Referências cruzadas ---"

BROKEN_REFS=""
while IFS= read -r rule_file; do
  # Extrair referências a .md que contenham / (caminhos reais, não nomes curtos)
  while IFS= read -r ref; do
    # Ignorar padrões glob (contêm * ou **)
    case "$ref" in *\**) continue ;; esac
    # Resolver caminho relativo ao repositório
    if [ ! -f "$REPO_ROOT/$ref" ]; then
      BROKEN_REFS="$BROKEN_REFS $(basename "$rule_file")->$ref"
    fi
  done < <(grep -oP '`([^`]*/[^`]*\.md)`' "$rule_file" 2>/dev/null | sed 's/`//g' | sort -u)
done < <(find "$REPO_ROOT/.claude/rules" -name "*.md" -type f | sort)

if [ -z "$BROKEN_REFS" ]; then
  pass "Todas as referências cruzadas entre rules apontam para arquivos existentes"
else
  fail "Referências cruzadas quebradas" "$BROKEN_REFS"
fi

# ---------------------------------------------------------------------------
# 8. README.md não referencia funcionalidades removidas
# ---------------------------------------------------------------------------
echo ""
echo "--- 8. README.md atualizado ---"

README_ISSUES=""
if grep -qi "WebMotors\|IntegrationRepos\|repositories/sync\|RepositoriesGetAll\|RepositoriesSyncAll" "$README" 2>/dev/null; then
  README_ISSUES="$README_ISSUES referências-a-funcionalidades-removidas"
fi

if [ -z "$README_ISSUES" ]; then
  pass "README.md não contém referências a funcionalidades removidas"
else
  fail "README.md contém referências a funcionalidades removidas" "$README_ISSUES"
fi

# ---------------------------------------------------------------------------
# 9. ADRs revogadas têm nota de redirecionamento
# ---------------------------------------------------------------------------
echo ""
echo "--- 9. ADRs revogadas ---"

ADR_FILE="$REPO_ROOT/Instructions/architecture/architecture-decisions.md"
if [ -f "$ADR_FILE" ]; then
  # Procurar entradas com "Revogad" que não tenham "Substituída por" ou "Ver DA-"
  REVOKED_WITHOUT_REDIRECT=$(grep -c 'Revogad' "$ADR_FILE" 2>/dev/null || echo "0")
  REVOKED_WITH_REDIRECT=$(grep -cP 'Revogad.*Substituíd|Substituíd.*DA-' "$ADR_FILE" 2>/dev/null || echo "0")

  if [ "$REVOKED_WITHOUT_REDIRECT" -le "$REVOKED_WITH_REDIRECT" ] || [ "$REVOKED_WITHOUT_REDIRECT" -eq 0 ]; then
    pass "Todas as ADRs revogadas possuem nota de redirecionamento"
  else
    fail "ADRs revogadas sem nota de redirecionamento ($REVOKED_WITHOUT_REDIRECT revogadas, $REVOKED_WITH_REDIRECT com redirecionamento)"
  fi
else
  pass "Arquivo de decisões arquiteturais não encontrado — verificação ignorada"
fi

# ---------------------------------------------------------------------------
# 10. Imports do CLAUDE.md apontam para arquivos existentes (sem imports quebrados)
# ---------------------------------------------------------------------------
echo ""
echo "--- 10. Integridade dos imports existentes ---"

BROKEN_IMPORTS=""
while IFS= read -r import_line; do
  import_path="${import_line#@}"
  if [ ! -f "$REPO_ROOT/$import_path" ]; then
    BROKEN_IMPORTS="$BROKEN_IMPORTS $import_path"
  fi
done < <(grep '^@' "$CLAUDE_MD")

if [ -z "$BROKEN_IMPORTS" ]; then
  pass "Todos os imports existentes no CLAUDE.md apontam para arquivos reais"
else
  fail "Imports quebrados no CLAUDE.md (arquivos inexistentes)" "$BROKEN_IMPORTS"
fi

# ---------------------------------------------------------------------------
# 11. Componentes de Infra/ documentados em technical-overview.md
# ---------------------------------------------------------------------------
echo ""
echo "--- 11. Cobertura de documentação: Infra/ ---"

TECH_OVERVIEW="$REPO_ROOT/Instructions/architecture/technical-overview.md"
INFRA_DIR="$REPO_ROOT/src/Albert.Playground.ECS.AOT.Api/Infra"

if [ -d "$INFRA_DIR" ] && [ -f "$TECH_OVERVIEW" ]; then
  UNDOCUMENTED_INFRA=""
  while IFS= read -r infra_subdir; do
    subdir_name=$(basename "$infra_subdir")
    # Verificar se a subpasta está mencionada em technical-overview.md
    if ! grep -qi "$subdir_name" "$TECH_OVERVIEW" 2>/dev/null; then
      UNDOCUMENTED_INFRA="$UNDOCUMENTED_INFRA Infra/$subdir_name"
    fi
  done < <(find "$INFRA_DIR" -mindepth 1 -maxdepth 1 -type d | sort)

  if [ -z "$UNDOCUMENTED_INFRA" ]; then
    pass "Todas as subpastas de Infra/ estão documentadas em technical-overview.md"
  else
    fail "Subpastas de Infra/ não documentadas em technical-overview.md" "$UNDOCUMENTED_INFRA"
  fi
else
  pass "Infra/ ou technical-overview.md não encontrado — verificação ignorada"
fi

# ---------------------------------------------------------------------------
# 12. Subpastas de Infra/ registradas em folder-structure.md
# ---------------------------------------------------------------------------
echo ""
echo "--- 12. Cobertura de documentação: Infra/ em folder-structure.md ---"

FOLDER_STRUCTURE="$REPO_ROOT/Instructions/architecture/folder-structure.md"

if [ -d "$INFRA_DIR" ] && [ -f "$FOLDER_STRUCTURE" ]; then
  UNDOCUMENTED_FOLDERS=""
  while IFS= read -r infra_subdir; do
    subdir_name=$(basename "$infra_subdir")
    if ! grep -qi "$subdir_name" "$FOLDER_STRUCTURE" 2>/dev/null; then
      UNDOCUMENTED_FOLDERS="$UNDOCUMENTED_FOLDERS Infra/$subdir_name"
    fi
  done < <(find "$INFRA_DIR" -mindepth 1 -maxdepth 1 -type d | sort)

  if [ -z "$UNDOCUMENTED_FOLDERS" ]; then
    pass "Todas as subpastas de Infra/ estão registradas em folder-structure.md"
  else
    fail "Subpastas de Infra/ ausentes em folder-structure.md" "$UNDOCUMENTED_FOLDERS"
  fi
else
  pass "Infra/ ou folder-structure.md não encontrado — verificação ignorada"
fi

# ---------------------------------------------------------------------------
# 13. Integrações em Shared/ExternalApi/ documentadas em technical-overview.md
# ---------------------------------------------------------------------------
echo ""
echo "--- 13. Cobertura de documentação: Shared/ExternalApi/ ---"

EXTERNAL_API_DIR="$REPO_ROOT/src/Albert.Playground.ECS.AOT.Api/Shared/ExternalApi"

if [ -d "$EXTERNAL_API_DIR" ] && [ -f "$TECH_OVERVIEW" ]; then
  UNDOCUMENTED_APIS=""
  while IFS= read -r api_subdir; do
    api_name=$(basename "$api_subdir")
    if ! grep -qi "$api_name" "$TECH_OVERVIEW" 2>/dev/null; then
      UNDOCUMENTED_APIS="$UNDOCUMENTED_APIS Shared/ExternalApi/$api_name"
    fi
  done < <(find "$EXTERNAL_API_DIR" -mindepth 1 -maxdepth 1 -type d | sort)

  if [ -z "$UNDOCUMENTED_APIS" ]; then
    pass "Todas as integrações em Shared/ExternalApi/ estão documentadas em technical-overview.md"
  else
    fail "Integrações em Shared/ExternalApi/ não documentadas em technical-overview.md" "$UNDOCUMENTED_APIS"
  fi
else
  pass "Shared/ExternalApi/ ou technical-overview.md não encontrado — verificação ignorada"
fi

# ---------------------------------------------------------------------------
# 14. Hooks configurados em settings.json existem como arquivos
# ---------------------------------------------------------------------------
echo ""
echo "--- 14. Integridade dos hooks ---"

SETTINGS="$REPO_ROOT/.claude/settings.json"
HOOKS_DIR="$REPO_ROOT/.claude/hooks"

if [ -f "$SETTINGS" ]; then
  MISSING_HOOKS=""
  while IFS= read -r hook_script; do
    # Extrair o nome do script do comando (ex: "bash .claude/hooks/branch-guard.sh ...")
    script_name=$(echo "$hook_script" | grep -oP '\.claude/hooks/\S+\.sh' | head -1)
    if [ -n "$script_name" ] && [ ! -f "$REPO_ROOT/$script_name" ]; then
      MISSING_HOOKS="$MISSING_HOOKS $script_name"
    fi
  done < <(grep -oP '"command":\s*"[^"]*"' "$SETTINGS" | sed 's/"command":\s*"//;s/"$//')

  if [ -z "$MISSING_HOOKS" ]; then
    pass "Todos os hooks configurados em settings.json existem como arquivos"
  else
    fail "Hooks configurados mas inexistentes" "$MISSING_HOOKS"
  fi
else
  pass "settings.json não encontrado — verificação ignorada"
fi

# ---------------------------------------------------------------------------
# 15. Contagem de skills em README.md (todas as ocorrências)
# ---------------------------------------------------------------------------
echo ""
echo "--- 15. Consistência interna do README.md ---"

if [ -f "$README" ]; then
  # Verificar se TODAS as menções de skills no README usam o número correto
  INCONSISTENT_COUNTS=""
  while IFS= read -r line_num; do
    line_content=$(sed -n "${line_num}p" "$README")
    mentioned_count=$(echo "$line_content" | grep -oP '\d+(?=\s*skills)' || echo "")
    if [ -n "$mentioned_count" ] && [ "$mentioned_count" != "$ACTUAL_SKILLS_COUNT" ]; then
      INCONSISTENT_COUNTS="$INCONSISTENT_COUNTS linha${line_num}(diz:${mentioned_count},real:${ACTUAL_SKILLS_COUNT})"
    fi
  done < <(grep -n 'skills' "$README" | grep -oP '^\d+')

  if [ -z "$INCONSISTENT_COUNTS" ]; then
    pass "README.md usa contagem de skills consistente em todas as ocorrências"
  else
    fail "README.md contém contagens inconsistentes de skills" "$INCONSISTENT_COUNTS"
  fi
fi

# ---------------------------------------------------------------------------
# Resumo
# ---------------------------------------------------------------------------
echo ""
echo "=== Resumo ==="
PASSED=$((TOTAL - FAILURES))
echo "Total: $TOTAL verificações | $PASSED aprovadas | $FAILURES falhas"

if [ "$FAILURES" -gt 0 ]; then
  echo ""
  echo "[ATENÇÃO] Existem $FAILURES falhas de consistência na governança."
  echo "Corrija as falhas antes de prosseguir com o commit."
  exit 1
else
  echo ""
  echo "[APROVADO] Governança estruturalmente consistente."
  exit 0
fi
