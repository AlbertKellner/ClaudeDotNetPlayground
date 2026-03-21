#!/bin/bash
# Script: governance-audit.sh
# Propósito: Verificar automaticamente a consistência estrutural dos arquivos de governança.
# Execução: bash scripts/governance-audit.sh
# Saída: Relatório com [OK] / [FALHA] / [AVISO] por verificação.
#
# Este script verifica consistência estrutural e completude documental.
# Verificações semânticas profundas (ex: "a regra está correta?") permanecem com o assistente.
#
# IMPORTANTE: A numeração das verificações neste script DEVE corresponder 1:1
# à tabela em .claude/rules/governance-audit.md. Ao adicionar ou remover verificações,
# atualizar ambos os arquivos simultaneamente.

set -euo pipefail

REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
FAILURES=0
WARNINGS=0
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

warn() {
  TOTAL=$((TOTAL + 1))
  WARNINGS=$((WARNINGS + 1))
  echo "[AVISO] $1"
  if [ -n "${2:-}" ]; then
    echo "        Detalhe: $2"
  fi
}

echo "=== Auditoria de Governança ==="
echo "Repositório: $REPO_ROOT"
echo "Data: $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

CLAUDE_MD="$REPO_ROOT/CLAUDE.md"
README="$REPO_ROOT/README.md"
TECH_OVERVIEW="$REPO_ROOT/Instructions/architecture/technical-overview.md"
FOLDER_STRUCTURE="$REPO_ROOT/Instructions/architecture/folder-structure.md"
ADR_FILE="$REPO_ROOT/Instructions/architecture/architecture-decisions.md"
COMPOSE="$REPO_ROOT/docker-compose.yml"
REQUIRED_VARS="$REPO_ROOT/scripts/required-vars.md"
SETTINGS="$REPO_ROOT/.claude/settings.json"
WIKI_DIR="$REPO_ROOT/wiki"
WIKI_ARCH="$REPO_ROOT/wiki/Architecture.md"
FEATURES_DIR="$REPO_ROOT/src/Albert.Playground.ECS.AOT.Api/Features"
INFRA_DIR="$REPO_ROOT/src/Albert.Playground.ECS.AOT.Api/Infra"
EXTERNAL_API_DIR="$REPO_ROOT/src/Albert.Playground.ECS.AOT.Api/Shared/ExternalApi"
RUNBOOK="$REPO_ROOT/scripts/operational-runbook.md"

# ---------------------------------------------------------------------------
# 1. Todos os arquivos .md de Instructions/ estão importados no CLAUDE.md
# ---------------------------------------------------------------------------
echo "--- 1. Imports de Instructions/ no CLAUDE.md ---"

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

# ---------------------------------------------------------------------------
# 2. Todos os arquivos .md de .claude/rules/ estão importados no CLAUDE.md
# ---------------------------------------------------------------------------
echo ""
echo "--- 2. Imports de .claude/rules/ no CLAUDE.md ---"

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
# 3. Contagem de rules no README.md corresponde ao número real
# ---------------------------------------------------------------------------
echo ""
echo "--- 3. Contagem de rules no README.md ---"

ACTUAL_RULES_COUNT=$(find "$REPO_ROOT/.claude/rules" -name "*.md" -type f | wc -l)
README_RULES_COUNT=$(grep -oP '\d+(?=\s*rules)' "$README" 2>/dev/null | head -1 || echo "0")

if [ "$ACTUAL_RULES_COUNT" = "$README_RULES_COUNT" ]; then
  pass "Contagem de rules no README.md ($README_RULES_COUNT) corresponde ao real ($ACTUAL_RULES_COUNT)"
else
  fail "Contagem de rules no README.md ($README_RULES_COUNT) não corresponde ao real ($ACTUAL_RULES_COUNT)"
fi

# ---------------------------------------------------------------------------
# 4. Contagem de skills no README.md corresponde ao número real
# ---------------------------------------------------------------------------
echo ""
echo "--- 4. Contagem de skills no README.md ---"

ACTUAL_SKILLS_COUNT=$(find "$REPO_ROOT/.claude/skills" -mindepth 1 -maxdepth 1 -type d | wc -l)
README_SKILLS_COUNT=$(grep -oP '\d+(?=\s*skills)' "$README" 2>/dev/null | head -1 || echo "0")

if [ "$ACTUAL_SKILLS_COUNT" = "$README_SKILLS_COUNT" ]; then
  pass "Contagem de skills no README.md ($README_SKILLS_COUNT) corresponde ao real ($ACTUAL_SKILLS_COUNT)"
else
  fail "Contagem de skills no README.md ($README_SKILLS_COUNT) não corresponde ao real ($ACTUAL_SKILLS_COUNT)"
fi

# ---------------------------------------------------------------------------
# 5. Variáveis de ambiente do docker-compose.yml documentadas em required-vars.md
# ---------------------------------------------------------------------------
echo ""
echo "--- 5. Variáveis de ambiente documentadas ---"

if [ -f "$COMPOSE" ] && [ -f "$REQUIRED_VARS" ]; then
  UNDOCUMENTED_VARS=""
  while IFS= read -r var; do
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
# 6. Nenhuma referência ativa a artefatos removidos em arquivos não-históricos
# ---------------------------------------------------------------------------
echo ""
echo "--- 6. Referências a artefatos removidos ---"

# Lista de IDs de artefatos removidos — ADICIONAR NOVOS IDs AQUI ao remover artefatos
REMOVED_ARTIFACTS="RN-006 RN-007"

STALE_REFS=""
for artifact_id in $REMOVED_ARTIFACTS; do
  while IFS= read -r file; do
    basename=$(basename "$file")
    case "$basename" in
      bash-errors-log.md|assumptions-log.md) continue ;;
    esac
    if grep -qF "$artifact_id" "$file" 2>/dev/null; then
      has_active_ref=false
      while IFS= read -r line; do
        if echo "$line" | grep -qiP 'remov|substituíd|depreciad|histórico|revogad|^\|.*20[0-9]{2}-[0-9]{2}-[0-9]{2}'; then
          :
        else
          line_num=$(grep -nF "$line" "$file" 2>/dev/null | head -1 | cut -d: -f1)
          if [ -n "$line_num" ]; then
            context_after=$(sed -n "$((line_num)),$((line_num + 3))p" "$file" 2>/dev/null)
            if echo "$context_after" | grep -qiP 'remov|substituíd|depreciad|status.*remov'; then
              :
            else
              has_active_ref=true
              break
            fi
          fi
        fi
      done < <(grep -F "$artifact_id" "$file" 2>/dev/null)
      if [ "$has_active_ref" = true ]; then
        STALE_REFS="$STALE_REFS $file($artifact_id)"
      fi
    fi
  done < <(find "$REPO_ROOT/Instructions" "$REPO_ROOT/.claude/rules" "$REPO_ROOT/wiki" -name "*.md" -type f 2>/dev/null | sort)
done

if [ -z "$STALE_REFS" ]; then
  pass "Nenhuma referência ativa a artefatos removidos ($REMOVED_ARTIFACTS)"
else
  fail "Referências ativas a artefatos removidos encontradas" "$STALE_REFS"
fi

# ---------------------------------------------------------------------------
# 7. Todas as features possuem página correspondente na Wiki
# ---------------------------------------------------------------------------
echo ""
echo "--- 7. Features com página na Wiki ---"

if [ -d "$WIKI_DIR" ] && [ -d "$FEATURES_DIR" ]; then
  MISSING_WIKI=""
  while IFS= read -r feature_dir; do
    feature_name=$(basename "$feature_dir")
    if ! find "$WIKI_DIR" -iname "Feature-*${feature_name}*" -type f 2>/dev/null | grep -q .; then
      MISSING_WIKI="$MISSING_WIKI $feature_name"
    fi
  done < <(find "$FEATURES_DIR" -mindepth 2 -maxdepth 2 -type d | sort)

  if [ -z "$MISSING_WIKI" ]; then
    pass "Todas as features possuem página correspondente na Wiki"
  else
    fail "Features sem página na Wiki" "$MISSING_WIKI"
  fi
else
  pass "wiki/ ou Features/ não encontrado — verificação ignorada"
fi

# ---------------------------------------------------------------------------
# 8. Páginas estruturais obrigatórias existem na Wiki
# ---------------------------------------------------------------------------
echo ""
echo "--- 8. Páginas estruturais obrigatórias na Wiki ---"

if [ -d "$WIKI_DIR" ]; then
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
# 9. Rules não contêm workflows procedurais extensos (>5 passos)
# ---------------------------------------------------------------------------
echo ""
echo "--- 9. Separação Rules/Skills (workflows extensos) ---"

RULES_WITH_WORKFLOWS=""
while IFS= read -r rule_file; do
  rule_name=$(basename "$rule_file")
  # Heurística 1: headings procedurais (### Passo, ### Step, ### Workflow)
  step_count=$(grep -cP '###\s+(Passo|Step|Workflow)' "$rule_file" 2>/dev/null || true)
  step_count="${step_count:-0}"
  # Heurística 2: sequências numeradas longas contíguas (>6 passos seguidos sem reiniciar)
  # Rules podem ter múltiplos blocos curtos (1-4 passos cada) como parte da política.
  # Um workflow extenso se manifesta como sequência longa contígua (1. 2. 3. ... 7. 8.).
  # Extrair conteúdo excluindo seções de histórico e referências
  max_sequence=0
  current_sequence=0
  last_num=0
  while IFS= read -r num; do
    if [ "$num" -eq $((last_num + 1)) ]; then
      # Continuação da sequência (2 após 1, 3 após 2, etc.)
      current_sequence=$((current_sequence + 1))
    elif [ "$num" -eq 1 ]; then
      # Início de nova lista — reiniciar contador
      current_sequence=1
    else
      current_sequence=1
    fi
    last_num=$num
    if [ "$current_sequence" -gt "$max_sequence" ]; then
      max_sequence=$current_sequence
    fi
  done < <(sed '/## Histórico/,$ d; /## Relação com/,/^##/d; /## Referências/,/^##/d' "$rule_file" 2>/dev/null \
    | grep -oP '^\d+(?=\.\s+\S)' 2>/dev/null || true)
  total_steps=$((step_count + max_sequence))
  # Limiar: headings procedurais + maior sequência contígua > 8 indica workflow extenso.
  # Rules podem ter listas de até ~7-8 ações como parte da definição de política.
  # Sequências >8 passos contíguos indicam workflow detalhado que pertence a uma skill.
  if [ "$total_steps" -gt 8 ]; then
    RULES_WITH_WORKFLOWS="$RULES_WITH_WORKFLOWS $rule_name(${total_steps}_passos)"
  fi
done < <(find "$REPO_ROOT/.claude/rules" -name "*.md" -type f | sort)

if [ -z "$RULES_WITH_WORKFLOWS" ]; then
  pass "Nenhuma rule contém workflows procedurais extensos (>10 passos)"
else
  fail "Rules com workflows procedurais extensos (deveriam ser skills)" "$RULES_WITH_WORKFLOWS"
fi

# ---------------------------------------------------------------------------
# 10. Referências cruzadas entre rules apontam para arquivos existentes
# ---------------------------------------------------------------------------
echo ""
echo "--- 10. Referências cruzadas entre rules ---"

BROKEN_REFS=""
while IFS= read -r rule_file; do
  while IFS= read -r ref; do
    case "$ref" in *\**) continue ;; esac
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
# 11. README.md não referencia funcionalidades removidas
# ---------------------------------------------------------------------------
echo ""
echo "--- 11. README.md sem funcionalidades removidas ---"

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
# 12. ADRs revogadas possuem nota de redirecionamento
# ---------------------------------------------------------------------------
echo ""
echo "--- 12. ADRs revogadas com redirecionamento ---"

if [ -f "$ADR_FILE" ]; then
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
# 13. Imports existentes no CLAUDE.md apontam para arquivos reais
# ---------------------------------------------------------------------------
echo ""
echo "--- 13. Integridade dos imports existentes ---"

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
# 14. Subpastas de Infra/ documentadas em technical-overview.md
# ---------------------------------------------------------------------------
echo ""
echo "--- 14. Infra/ documentada em technical-overview.md ---"

if [ -d "$INFRA_DIR" ] && [ -f "$TECH_OVERVIEW" ]; then
  UNDOCUMENTED_INFRA=""
  while IFS= read -r infra_subdir; do
    subdir_name=$(basename "$infra_subdir")
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
# 15. Subpastas de Infra/ registradas em folder-structure.md
# ---------------------------------------------------------------------------
echo ""
echo "--- 15. Infra/ registrada em folder-structure.md ---"

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
# 16. Integrações Shared/ExternalApi/ documentadas em technical-overview.md
# ---------------------------------------------------------------------------
echo ""
echo "--- 16. Shared/ExternalApi/ documentada em technical-overview.md ---"

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
# 17. Hooks configurados em settings.json existem como arquivos
# ---------------------------------------------------------------------------
echo ""
echo "--- 17. Integridade dos hooks ---"

if [ -f "$SETTINGS" ]; then
  MISSING_HOOKS=""
  while IFS= read -r hook_script; do
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
# 18. Contagens de skills consistentes em todas as ocorrências do README.md
# ---------------------------------------------------------------------------
echo ""
echo "--- 18. Consistência interna do README.md ---"

if [ -f "$README" ]; then
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
# 19. Todos os diretórios de skills contêm SKILL.md
# ---------------------------------------------------------------------------
echo ""
echo "--- 19. Integridade das skills ---"

SKILLS_WITHOUT_MD=""
while IFS= read -r skill_dir; do
  skill_name=$(basename "$skill_dir")
  if [ ! -f "$skill_dir/SKILL.md" ]; then
    SKILLS_WITHOUT_MD="$SKILLS_WITHOUT_MD $skill_name"
  fi
done < <(find "$REPO_ROOT/.claude/skills" -mindepth 1 -maxdepth 1 -type d | sort)

if [ -z "$SKILLS_WITHOUT_MD" ]; then
  pass "Todos os diretórios de skills contêm SKILL.md"
else
  fail "Diretórios de skills sem SKILL.md" "$SKILLS_WITHOUT_MD"
fi

# ---------------------------------------------------------------------------
# 20. wiki/Architecture.md lista todas as features implementadas
# ---------------------------------------------------------------------------
echo ""
echo "--- 20. Architecture.md lista todas as features ---"

if [ -f "$WIKI_ARCH" ] && [ -d "$FEATURES_DIR" ]; then
  MISSING_IN_ARCH=""
  while IFS= read -r feature_dir; do
    feature_name=$(basename "$feature_dir")
    if ! grep -qi "$feature_name" "$WIKI_ARCH" 2>/dev/null; then
      MISSING_IN_ARCH="$MISSING_IN_ARCH $feature_name"
    fi
  done < <(find "$FEATURES_DIR" -mindepth 2 -maxdepth 2 -type d | sort)

  if [ -z "$MISSING_IN_ARCH" ]; then
    pass "wiki/Architecture.md lista todas as features implementadas"
  else
    fail "Features ausentes em wiki/Architecture.md" "$MISSING_IN_ARCH"
  fi
else
  pass "wiki/Architecture.md ou Features/ não encontrado — verificação ignorada"
fi

# ---------------------------------------------------------------------------
# 21. wiki/Architecture.md lista todas as subpastas de Infra/
# ---------------------------------------------------------------------------
echo ""
echo "--- 21. Architecture.md lista todas as subpastas de Infra/ ---"

if [ -f "$WIKI_ARCH" ] && [ -d "$INFRA_DIR" ]; then
  MISSING_INFRA_WIKI=""
  while IFS= read -r infra_subdir; do
    subdir_name=$(basename "$infra_subdir")
    if ! grep -qi "$subdir_name" "$WIKI_ARCH" 2>/dev/null; then
      MISSING_INFRA_WIKI="$MISSING_INFRA_WIKI Infra/$subdir_name"
    fi
  done < <(find "$INFRA_DIR" -mindepth 1 -maxdepth 1 -type d | sort)

  if [ -z "$MISSING_INFRA_WIKI" ]; then
    pass "wiki/Architecture.md lista todas as subpastas de Infra/"
  else
    fail "Subpastas de Infra/ ausentes em wiki/Architecture.md" "$MISSING_INFRA_WIKI"
  fi
else
  pass "wiki/Architecture.md ou Infra/ não encontrado — verificação ignorada"
fi

# ---------------------------------------------------------------------------
# 22. wiki/Architecture.md lista todas as integrações Shared/ExternalApi/
# ---------------------------------------------------------------------------
echo ""
echo "--- 22. Architecture.md lista Shared/ExternalApi/ ---"

if [ -f "$WIKI_ARCH" ] && [ -d "$EXTERNAL_API_DIR" ]; then
  MISSING_API_WIKI=""
  while IFS= read -r api_subdir; do
    api_name=$(basename "$api_subdir")
    if ! grep -qi "$api_name" "$WIKI_ARCH" 2>/dev/null; then
      MISSING_API_WIKI="$MISSING_API_WIKI Shared/ExternalApi/$api_name"
    fi
  done < <(find "$EXTERNAL_API_DIR" -mindepth 1 -maxdepth 1 -type d | sort)

  if [ -z "$MISSING_API_WIKI" ]; then
    pass "wiki/Architecture.md lista todas as integrações de Shared/ExternalApi/"
  else
    fail "Integrações ausentes em wiki/Architecture.md" "$MISSING_API_WIKI"
  fi
else
  pass "wiki/Architecture.md ou Shared/ExternalApi/ não encontrado — verificação ignorada"
fi

# ---------------------------------------------------------------------------
# 23. Todas as rules possuem seção "Propósito"
# ---------------------------------------------------------------------------
echo ""
echo "--- 23. Estrutura mínima das rules ---"

RULES_WITHOUT_PURPOSE=""
while IFS= read -r rule_file; do
  rule_name=$(basename "$rule_file")
  if ! grep -q '## Propósito' "$rule_file" 2>/dev/null; then
    RULES_WITHOUT_PURPOSE="$RULES_WITHOUT_PURPOSE $rule_name"
  fi
done < <(find "$REPO_ROOT/.claude/rules" -name "*.md" -type f | sort)

if [ -z "$RULES_WITHOUT_PURPOSE" ]; then
  pass "Todas as rules possuem seção 'Propósito'"
else
  fail "Rules sem seção 'Propósito'" "$RULES_WITHOUT_PURPOSE"
fi

# ---------------------------------------------------------------------------
# 24. Tabela "Features Implementadas" na wiki lista todas as features
# ---------------------------------------------------------------------------
echo ""
echo "--- 24. Tabela de features na wiki ---"

if [ -f "$WIKI_ARCH" ] && [ -d "$FEATURES_DIR" ]; then
  FEATURES_TABLE=$(sed -n '/## Features Implementadas/,/^##\s/p' "$WIKI_ARCH" 2>/dev/null)
  MISSING_IN_TABLE=""
  while IFS= read -r feature_dir; do
    feature_name=$(basename "$feature_dir")
    if ! echo "$FEATURES_TABLE" | grep -qi "$feature_name" 2>/dev/null; then
      MISSING_IN_TABLE="$MISSING_IN_TABLE $feature_name"
    fi
  done < <(find "$FEATURES_DIR" -mindepth 2 -maxdepth 2 -type d | sort)

  if [ -z "$MISSING_IN_TABLE" ]; then
    pass "Tabela 'Features Implementadas' na wiki lista todas as features"
  else
    fail "Features ausentes na tabela 'Features Implementadas' da wiki" "$MISSING_IN_TABLE"
  fi
else
  pass "wiki/Architecture.md ou Features/ não encontrado — verificação ignorada"
fi

# ---------------------------------------------------------------------------
# 25. Nenhuma página wiki Feature-* órfã (sem feature correspondente no código)
# ---------------------------------------------------------------------------
echo ""
echo "--- 25. Wiki pages órfãs ---"

if [ -d "$WIKI_DIR" ] && [ -d "$FEATURES_DIR" ]; then
  ORPHAN_PAGES=""
  while IFS= read -r wiki_page; do
    page_name=$(basename "$wiki_page" .md)
    # Extrair nome da feature do padrão Feature-NomeDaFeature
    feature_part="${page_name#Feature-}"
    # Procurar feature correspondente em Features/ ou em Infra/ (ex: Health é HealthChecks)
    found=false
    if find "$FEATURES_DIR" -mindepth 2 -maxdepth 2 -type d -iname "*${feature_part}*" 2>/dev/null | grep -q .; then
      found=true
    elif [ -d "$INFRA_DIR" ] && find "$INFRA_DIR" -mindepth 1 -maxdepth 1 -type d -iname "*${feature_part}*" 2>/dev/null | grep -q .; then
      found=true
    fi
    if [ "$found" = false ]; then
      ORPHAN_PAGES="$ORPHAN_PAGES $page_name"
    fi
  done < <(find "$WIKI_DIR" -name "Feature-*.md" -type f | sort)

  if [ -z "$ORPHAN_PAGES" ]; then
    pass "Nenhuma página wiki Feature-* órfã encontrada"
  else
    warn "Páginas wiki Feature-* sem feature correspondente no código" "$ORPHAN_PAGES"
  fi
else
  pass "wiki/ ou Features/ não encontrado — verificação ignorada"
fi

# ---------------------------------------------------------------------------
# 26. Endpoints no runbook correspondem a rotas nos Controllers
# ---------------------------------------------------------------------------
echo ""
echo "--- 26. Alinhamento runbook ↔ endpoints reais ---"

if [ -f "$RUNBOOK" ] && [ -d "$FEATURES_DIR" ]; then
  # Extrair rotas do runbook (coluna "Rota" da tabela de endpoints)
  RUNBOOK_ROUTES=$(grep -oP '`/[a-z][a-z0-9-]*`' "$RUNBOOK" 2>/dev/null | sed 's/`//g' | sort -u)
  # Extrair rotas dos Controllers (atributos [Route("...")] e [Http*("...")])
  CODE_ROUTES=$(grep -rhoP '\[Route\("([^"]+)"\)\]|\[Http(Get|Post|Put|Delete|Patch)\("([^"]+)"\)\]' "$FEATURES_DIR" 2>/dev/null \
    | grep -oP '"[^"]+"' | sed 's/"//g' | sort -u)
  # Adicionar rotas implícitas conhecidas (/health e /login são especiais)
  CODE_ROUTES=$(echo -e "$CODE_ROUTES\nhealth\nlogin" | sort -u)

  MISSING_IN_RUNBOOK=""
  for route in $CODE_ROUTES; do
    clean_route="/$route"
    if ! echo "$RUNBOOK_ROUTES" | grep -qF "$clean_route"; then
      MISSING_IN_RUNBOOK="$MISSING_IN_RUNBOOK $clean_route"
    fi
  done

  STALE_IN_RUNBOOK=""
  for route in $RUNBOOK_ROUTES; do
    clean_route="${route#/}"
    if ! echo "$CODE_ROUTES" | grep -qF "$clean_route"; then
      STALE_IN_RUNBOOK="$STALE_IN_RUNBOOK $route"
    fi
  done

  if [ -z "$MISSING_IN_RUNBOOK" ] && [ -z "$STALE_IN_RUNBOOK" ]; then
    pass "Endpoints no runbook estão alinhados com rotas nos Controllers"
  else
    [ -n "$MISSING_IN_RUNBOOK" ] && warn "Rotas no código ausentes no runbook" "$MISSING_IN_RUNBOOK"
    [ -n "$STALE_IN_RUNBOOK" ] && warn "Rotas no runbook sem Controller correspondente" "$STALE_IN_RUNBOOK"
  fi
else
  pass "Runbook ou Features/ não encontrado — verificação ignorada"
fi

# ---------------------------------------------------------------------------
# 27. Features com RN possuem cenários BDD (aviso, não bloqueante)
# ---------------------------------------------------------------------------
echo ""
echo "--- 27. Completude semântica: BDD para regras de negócio ---"

BDD_DIR="$REPO_ROOT/Instructions/bdd"
BUSINESS_RULES="$REPO_ROOT/Instructions/business/business-rules.md"

if [ -f "$BUSINESS_RULES" ] && [ -d "$BDD_DIR" ]; then
  # Contar RNs ativas (excluindo removidas/depreciadas)
  ACTIVE_RN_COUNT=$(grep -cP '^### RN-\d+ —' "$BUSINESS_RULES" 2>/dev/null || echo "0")
  # Contar RNs com "BDD relacionado: Nenhum"
  NO_BDD_COUNT=$(grep -cP 'BDD relacionado.*Nenhum' "$BUSINESS_RULES" 2>/dev/null || echo "0")
  # Contar arquivos .feature reais (excluindo example.feature)
  REAL_FEATURE_COUNT=$(find "$BDD_DIR" -name "*.feature" -type f ! -name "example.feature" 2>/dev/null | wc -l)

  if [ "$NO_BDD_COUNT" -gt 0 ] || [ "$REAL_FEATURE_COUNT" -eq 0 ]; then
    warn "Regras de negócio sem cenários BDD: $NO_BDD_COUNT de $ACTIVE_RN_COUNT RNs ativas não possuem BDD; $REAL_FEATURE_COUNT arquivos .feature reais existem"
  else
    pass "Todas as regras de negócio possuem cenários BDD correspondentes"
  fi
else
  pass "business-rules.md ou bdd/ não encontrado — verificação ignorada"
fi

# ---------------------------------------------------------------------------
# 28. Contratos OpenAPI refletem endpoints implementados (aviso, não bloqueante)
# ---------------------------------------------------------------------------
echo ""
echo "--- 28. Completude semântica: contratos OpenAPI ---"

OPENAPI="$REPO_ROOT/Instructions/contracts/openapi.yaml"

if [ -f "$OPENAPI" ]; then
  if grep -qi "placeholder\|a definir\|substituir" "$OPENAPI" 2>/dev/null; then
    warn "Contratos OpenAPI são placeholders — nenhum contrato real para os endpoints implementados"
  else
    pass "Contratos OpenAPI parecem conter conteúdo real"
  fi
else
  pass "openapi.yaml não encontrado — verificação ignorada"
fi

# ---------------------------------------------------------------------------
# Resumo
# ---------------------------------------------------------------------------
echo ""
echo "=== Resumo ==="
PASSED=$((TOTAL - FAILURES - WARNINGS))
echo "Total: $TOTAL verificações | $PASSED aprovadas | $FAILURES falhas | $WARNINGS avisos"

if [ "$FAILURES" -gt 0 ]; then
  echo ""
  echo "[ATENÇÃO] Existem $FAILURES falhas de consistência na governança."
  echo "Corrija as falhas antes de prosseguir com o commit."
  exit 1
elif [ "$WARNINGS" -gt 0 ]; then
  echo ""
  echo "[APROVADO COM AVISOS] Governança estruturalmente consistente, mas existem $WARNINGS avisos de completude."
  echo "Avisos não bloqueiam o commit, mas indicam áreas de melhoria."
  exit 0
else
  echo ""
  echo "[APROVADO] Governança estruturalmente consistente."
  exit 0
fi
