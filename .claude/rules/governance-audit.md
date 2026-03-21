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

O script `scripts/governance-audit.sh` verifica automaticamente:

| # | Verificação | Categoria |
|---|---|---|
| 1 | Todos os arquivos `.md` de `Instructions/` estão importados no `CLAUDE.md` | Estrutura de governança |
| 2 | Todos os arquivos `.md` de `.claude/rules/` estão importados no `CLAUDE.md` | Estrutura de governança |
| 3 | Contagem de rules no `README.md` corresponde ao número real | Consistência documental |
| 4 | Contagem de skills no `README.md` corresponde ao número real | Consistência documental |
| 5 | Variáveis de ambiente do `docker-compose.yml` estão documentadas em `required-vars.md` | Configuração |
| 6 | Nenhuma referência ativa a regras de negócio removidas em arquivos não-históricos | Higiene |
| 7 | Todas as features possuem página correspondente na Wiki | Cobertura documental |
| 8 | Pages estruturais obrigatórias existem na Wiki | Cobertura documental |
| 9 | Rules não contêm workflows procedurais extensos (>5 passos) | Separação rules/skills |
| 10 | Referências cruzadas entre rules apontam para arquivos existentes | Integridade referencial |
| 11 | `README.md` não referencia funcionalidades removidas | Higiene |
| 12 | ADRs revogadas possuem nota de redirecionamento | Rastreabilidade |
| 13 | Imports existentes no CLAUDE.md apontam para arquivos reais | Integridade referencial |
| 14 | Subpastas de `Infra/` estão documentadas em `technical-overview.md` | Código vs. Governança |
| 15 | Subpastas de `Infra/` estão registradas em `folder-structure.md` | Código vs. Governança |
| 16 | Integrações em `Shared/ExternalApi/` estão documentadas em `technical-overview.md` | Código vs. Governança |
| 17 | Hooks configurados em `settings.json` existem como arquivos | Integridade de hooks |
| 18 | Contagens de skills são consistentes em todas as ocorrências do `README.md` | Consistência documental |

---

## O Que Fazer com Falhas

### Falhas detectadas durante o pipeline pré-commit (bloqueantes):
1. Corrigir a falha antes de prosseguir
2. Se a correção envolver mudança de governança, executar o checklist de `REVIEW.md` após a correção
3. Re-executar `scripts/governance-audit.sh` para confirmar que a falha foi resolvida

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
