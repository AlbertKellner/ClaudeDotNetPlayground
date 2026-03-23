# Regra: Auditoria Automatizada de Governança

## Propósito

Esta rule define a política de auditoria automatizada da consistência estrutural dos arquivos de governança. O objetivo é detectar e bloquear inconsistências antes que entrem no repositório, eliminando a degradação silenciosa da governança.

---

## Princípio Fundamental

> Tudo que pode ser verificado por script não deve depender de julgamento humano.
> Verificações estruturais (imports, contagens, referências, alinhamento) são automáticas.
> Verificações semânticas (duplicação de conteúdo, localização correta) permanecem com o assistente.

---

## Quando a Auditoria É Executada

| Gatilho | Obrigatório | Bloqueante |
|---|---|---|
| Após qualquer mudança em arquivo de governança (via hook) | Sim | Não (informativo) |
| Antes de qualquer commit (passo 0.1 do pipeline pré-commit) | Sim | Sim — falhas bloqueiam o commit |
| No início de uma sessão de trabalho (primeira ação) | Recomendado | Não |

---

## O Que a Auditoria Verifica

O script `scripts/governance-audit.sh` verifica automaticamente.

**IMPORTANTE**: A numeração abaixo corresponde 1:1 às seções do script. Ao adicionar ou remover verificações, atualizar ambos os arquivos simultaneamente.

### Verificações bloqueantes (falha bloqueia commit)

| # | Verificação | Categoria |
|---|---|---|
| 1 | Todos os arquivos `.md` de `Instructions/` estão importados no `CLAUDE.md` | Estrutura de governança |
| 2 | Todos os arquivos `.md` de `.claude/rules/` estão importados no `CLAUDE.md` | Estrutura de governança |
| 3 | Contagem de rules no `README.md` corresponde ao número real | Consistência documental |
| 4 | Contagem de skills no `README.md` corresponde ao número real | Consistência documental |
| 5 | Variáveis de ambiente do `docker-compose.yml` (via `${...}`) estão documentadas em `required-vars.md` — variáveis com valor literal são excluídas dinamicamente; prefixos de configuração inline (`__`) são excluídos via lista configurável (`INLINE_CONFIG_PREFIXES` no topo do script) | Configuração |
| 6 | Nenhuma referência ativa a artefatos removidos em arquivos não-históricos — IDs derivados do campo `**Status**` em `architecture-decisions.md` e `business-rules.md` | Higiene |
| 7 | Todas as features possuem página correspondente na Wiki | Cobertura documental |
| 8 | Páginas estruturais obrigatórias existem na Wiki | Cobertura documental |
| 9 | Rules não contêm workflows procedurais extensos (headings procedurais + maior sequência contígua > `MAX_POLICY_STEPS`, configurável no topo do script, default 8) | Separação rules/skills |
| 10 | Referências cruzadas entre rules apontam para arquivos existentes | Integridade referencial |
| 11 | `README.md` não referencia artefatos removidos — derivação dinâmica dos mesmos IDs do check #6 | Higiene |
| 12 | ADRs revogadas possuem justificativa ou redirecionamento (substituição por outra DA ou razão da remoção) | Rastreabilidade |
| 13 | Imports existentes no CLAUDE.md apontam para arquivos reais | Integridade referencial |
| 14 | Subpastas de `Infra/` estão documentadas em `technical-overview.md` | Código vs. Governança |
| 15 | Subpastas de `Infra/` estão registradas em `folder-structure.md` | Código vs. Governança |
| 16 | Integrações em `Shared/ExternalApi/` estão documentadas em `technical-overview.md` | Código vs. Governança |
| 17 | Hooks configurados em `settings.json` existem como arquivos e têm sintaxe bash válida | Integridade de hooks |
| 18 | Contagens de skills são consistentes em todas as ocorrências do `README.md` | Consistência documental |
| 19 | Todos os diretórios de skills contêm `SKILL.md` com estrutura mínima (Propósito/Nome, Quando Usar, Workflow) | Integridade estrutural |
| 20 | `wiki/Governance-Architecture.md` lista todas as features implementadas | Completude wiki |
| 21 | `wiki/Governance-Architecture.md` lista todas as subpastas de `Infra/` | Completude wiki |
| 22 | `wiki/Governance-Architecture.md` lista todas as integrações de `Shared/ExternalApi/` | Completude wiki |
| 23 | Todas as rules possuem estrutura mínima (Propósito + Histórico ou Relação com Outras Rules) | Estrutura mínima |
| 24 | Tabela "Features Implementadas" na wiki lista todas as features | Completude wiki |

### Verificações não-bloqueantes (aviso, não bloqueia commit)

| # | Verificação | Categoria |
|---|---|---|
| 25 | Nenhuma página wiki `Feature-*.md` órfã (sem feature correspondente no código) | Higiene bidirecional |
| 26 | Endpoints no runbook correspondem a rotas nos Controllers | Alinhamento operacional |
| 27 | Regras de negócio ativas possuem cenários BDD correspondentes | Completude semântica |
| 28 | Contratos OpenAPI refletem endpoints implementados (não são placeholders) | Completude semântica |
| 29 | Referências em skills apontam para arquivos existentes | Integridade referencial |
| 30 | Skills reais estão referenciadas em `operating-model.md` | Alinhamento operacional |
| 31 | `wiki/Domain-Business-Rules.md` lista todas as RNs ativas | Completude wiki |
| 32 | Quantidade e IDs de checks no script correspondem 1:1 à documentação na rule | Meta-consistência |
| 33 | Grafo de rules conectado — nenhuma rule isolada (sem referências a outras rules); detecta pares bidirecionais sem hierarquia explícita | Integridade referencial |
| 34 | Todas as skills referenciam pelo menos uma rule | Integridade referencial |
| 35 | Auto-fix usa backup (`safe_fix`) antes de alterações destrutivas (`sed -i`) — meta-análise do próprio script | Meta-segurança |
| 36 | Conceitos usados no mapa de propagação de `governance-policies.md` existem no glossário | Completude semântica |

### Sobre a lista de artefatos removidos (checks #6 e #11)

Os checks #6 e #11 derivam automaticamente a lista de IDs removidos do campo `**Status**` das fontes de governança: `architecture-decisions.md` e `business-rules.md`. O padrão robusto aceita variações de gênero e conjugação (`Revogado/Revogada/Removido/Removida/Depreciado/Depreciada/Obsoleto/Obsoleta`). O padrão é definido como constante `REMOVED_STATUS_PATTERN` no topo do script. Não há lista manual de fallback — toda remoção deve ser rastreável via campo Status nos arquivos-fonte. Ao remover qualquer artefato, garantir que o campo `**Status**` contém uma dessas palavras-chave.

### Sobre verificações de completude semântica (checks #27-28)

Estes checks emitem avisos, não falhas. A decisão de manter contratos e BDD como futuros está registrada em DA-022. Quando o projeto evoluir para domínio real, estes avisos devem ser promovidos a falhas.

---

## Modo de Auto-Correção (--fix)

O script suporta o modo `--fix` que corrige automaticamente problemas triviais:

```bash
bash scripts/governance-audit.sh --fix
```

### Segurança do modo --fix

Toda operação de escrita (`sed -i`, `cat >`) é precedida por `safe_fix <arquivo>`, que cria backup (`<arquivo>.bak`) antes da modificação. Isso garante reversibilidade. O check #35 verifica automaticamente que esta prática é seguida em todo o script.

**Ciclo recomendado** (documentado no pipeline passo 0.1 do CLAUDE.md):
1. Executar `bash scripts/governance-audit.sh` (verificação)
2. Se falhar: executar `bash scripts/governance-audit.sh --fix` (correção)
3. Re-executar `bash scripts/governance-audit.sh` (re-verificação)
4. Se ainda falhar: corrigir manualmente

### Correções automáticas suportadas:
- Checks #1 e #2: adiciona imports faltantes ao `CLAUDE.md`
- Checks #3 e #4: atualiza contagens de rules e skills no `README.md`
- Check #7: cria stub de página wiki `Feature-<Nome>.md` com template obrigatório
- Check #13: remove imports quebrados (arquivos inexistentes) do `CLAUDE.md`

### O que NÃO é corrigido automaticamente:
- Problemas semânticos (categorização incorreta, referências a artefatos removidos em contexto ativo)
- Problemas que requerem julgamento (separação rules/skills, conteúdo de wiki, conflitos de referência)
- Estrutura de SKILL.md (check #19) — requer conteúdo contextual
- Referências cruzadas quebradas (requer decisão sobre qual arquivo corrigir)

### Mensagens de diagnóstico enriquecidas

A partir da quarta rodada, toda falha e aviso inclui campos de diagnóstico opcionais:
- **CAUSA**: explica por que o problema ocorreu (não apenas o sintoma)
- **AÇÃO**: indica a ação corretiva específica para resolver o problema

Isso transforma o script de detector de sintomas em diagnosticador de causas-raiz.

---

## O Que Fazer com Falhas

### Falhas detectadas durante o pipeline pré-commit (bloqueantes):
1. Executar `bash scripts/governance-audit.sh --fix` para tentar correção automática
2. Corrigir manualmente falhas remanescentes
3. Se a correção envolver mudança de governança, executar o checklist de `REVIEW.md` após a correção
4. Re-executar `scripts/governance-audit.sh` para confirmar que a falha foi resolvida

### Falhas detectadas via hook (informativas):
1. Registrar mentalmente a falha
2. Corrigir durante a tarefa atual se possível
3. Se não for possível corrigir imediatamente, a auditoria pré-commit bloqueará antes do commit

---

## Evolução da Auditoria

Quando uma nova categoria de inconsistência for identificada (manualmente ou por erro em sessão):
1. Avaliar se a verificação pode ser automatizada em script
2. Se sim: adicionar nova verificação ao `scripts/governance-audit.sh`
3. Atualizar a tabela "O Que a Auditoria Verifica" nesta rule
4. Testar o script com a nova verificação

---

## Relação com Outras Rules

- `instruction-review.md` — ativada pelo mesmo gatilho (mudança de governança); esta rule complementa com verificação automatizada
- `governance-policies.md` §3 — a auditoria verifica o resultado da propagação
- `bash-error-logging.md` — falhas do script devem ser registradas se forem erros de bash

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-21 | Criado: regra de auditoria automatizada de governança | Análise de inconsistências do repositório |
| 2026-03-21 | Expandido: verificações 14–18 adicionadas (código vs. governança, integridade de hooks, consistência interna do README) | Análise de causas-raiz |
| 2026-03-21 | Expandido: verificações 19–24 adicionadas (integridade de skills, completude da wiki Architecture.md, estrutura mínima de rules) | Análise de governança — completude vs. existência |
| 2026-03-21 | Reestruturado: numeração 1:1 entre rule e script; heurística de workflows melhorada (listas numeradas); check #6 generalizado para artefatos removidos; checks 25–28 adicionados (wiki órfãs, runbook↔endpoints, completude BDD, completude contratos); nível AVISO adicionado | Segunda análise de causas-raiz |
| 2026-03-21 | Terceira rodada: check #6 com derivação automática de REMOVED_ARTIFACTS; check #12 com heurística corrigida para revogações sem substituição; checks 29–32 adicionados (skills→rules, operating-model↔skills, wiki Business-Rules↔RNs, meta-consistência script↔rule); modo --fix para correções triviais automáticas | Análise estrutural de governança |
| 2026-03-21 | Quarta rodada — maturidade de auto-diagnóstico: (1) valores hardcoded eliminados nos checks #5, #6, #11 — derivação dinâmica de variáveis e artefatos removidos; (2) check #6 restrito ao campo `**Status**` para evitar falsos positivos; (3) mensagens de diagnóstico enriquecidas (CAUSA + AÇÃO) em todos os fails/warns; (4) auto-fix expandido: check #7 cria stubs wiki, check #13 remove imports quebrados; (5) check #17 expandido com validação de sintaxe bash; (6) check #19 expandido com validação de estrutura mínima do SKILL.md; (7) check #23 expandido com validação de Histórico/Relação; (8) check #32 expandido com correspondência individual de IDs (não apenas contagem); (9) checks 33–34 adicionados (conectividade do grafo de rules, skills→rules) | Análise de maturidade de governança |
| 2026-03-21 | Quinta rodada — auto-diagnóstico e prevenção: (1) safe_fix com backup obrigatório antes de sed -i no modo --fix; (2) constantes configuráveis extraídas para topo do script (MAX_POLICY_STEPS, INLINE_CONFIG_PREFIXES, REMOVED_STATUS_PATTERN); (3) "Obsoleto/Obsoleta" adicionado ao padrão de status removido; (4) heurística do check #6 melhorada — verificação estrutural de seção Histórico em vez de keyword matching; (5) check #26 regex expandido para rotas parametrizadas e case-insensitive; (6) check #31 padrão de status expandido; (7) check #33 expandido com detecção de circularidade bidirecional; (8) checks 35–36 adicionados (meta-segurança do --fix, conceitos de rules no glossário); (9) ciclo verify→fix→re-verify documentado no pipeline CLAUDE.md e implementado em pre-commit-gate.sh | Análise de capacidade de auto-diagnóstico |
