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
| 5 | Variáveis de ambiente do `docker-compose.yml` estão documentadas em `required-vars.md` | Configuração |
| 6 | Nenhuma referência ativa a artefatos removidos em arquivos não-históricos | Higiene |
| 7 | Todas as features possuem página correspondente na Wiki | Cobertura documental |
| 8 | Páginas estruturais obrigatórias existem na Wiki | Cobertura documental |
| 9 | Rules não contêm workflows procedurais extensos (headings procedurais + maior sequência contígua > 8) | Separação rules/skills |
| 10 | Referências cruzadas entre rules apontam para arquivos existentes | Integridade referencial |
| 11 | `README.md` não referencia funcionalidades removidas | Higiene |
| 12 | ADRs revogadas possuem justificativa ou redirecionamento (substituição por outra DA ou razão da remoção) | Rastreabilidade |
| 13 | Imports existentes no CLAUDE.md apontam para arquivos reais | Integridade referencial |
| 14 | Subpastas de `Infra/` estão documentadas em `technical-overview.md` | Código vs. Governança |
| 15 | Subpastas de `Infra/` estão registradas em `folder-structure.md` | Código vs. Governança |
| 16 | Integrações em `Shared/ExternalApi/` estão documentadas em `technical-overview.md` | Código vs. Governança |
| 17 | Hooks configurados em `settings.json` existem como arquivos | Integridade de hooks |
| 18 | Contagens de skills são consistentes em todas as ocorrências do `README.md` | Consistência documental |
| 19 | Todos os diretórios de skills contêm `SKILL.md` | Integridade estrutural |
| 20 | `wiki/Architecture.md` lista todas as features implementadas | Completude wiki |
| 21 | `wiki/Architecture.md` lista todas as subpastas de `Infra/` | Completude wiki |
| 22 | `wiki/Architecture.md` lista todas as integrações de `Shared/ExternalApi/` | Completude wiki |
| 23 | Todas as rules possuem seção "Propósito" | Estrutura mínima |
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
| 31 | `wiki/Business-Rules.md` lista todas as RNs ativas | Completude wiki |
| 32 | Quantidade de checks no script corresponde à documentação na rule | Meta-consistência |

### Sobre a lista de artefatos removidos (check #6)

O check #6 deriva automaticamente a lista de IDs removidos das fontes de governança: `architecture-decisions.md` (status "Revogad") e `business-rules.md` (status "Removida"). Uma lista manual de fallback (`REMOVED_ARTIFACTS_MANUAL`) garante cobertura caso o padrão de texto mude. Ao remover qualquer artefato, verificar que o padrão de status está presente na fonte primária.

### Sobre verificações de completude semântica (checks #27-28)

Estes checks emitem avisos, não falhas. A decisão de manter contratos e BDD como futuros está registrada em DA-022. Quando o projeto evoluir para domínio real, estes avisos devem ser promovidos a falhas.

---

## Modo de Auto-Correção (--fix)

O script suporta o modo `--fix` que corrige automaticamente problemas triviais:

```bash
bash scripts/governance-audit.sh --fix
```

### Correções automáticas suportadas:
- Checks #1 e #2: adiciona imports faltantes ao `CLAUDE.md`
- Checks #3 e #4: atualiza contagens de rules e skills no `README.md`

### O que NÃO é corrigido automaticamente:
- Problemas semânticos (categorização incorreta, referências a artefatos removidos em contexto ativo)
- Problemas que requerem julgamento (separação rules/skills, conteúdo de wiki)
- Referências cruzadas quebradas (requer decisão sobre qual arquivo corrigir)

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
