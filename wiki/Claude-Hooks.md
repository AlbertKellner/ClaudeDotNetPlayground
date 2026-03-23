# Claude — Hooks

## Descrição

Documenta os hooks configurados em `.claude/settings.json` que aplicam regras de governança automaticamente. Deve ser consultada para entender quais verificações automatizadas são executadas durante o desenvolvimento.

## Contexto

Hooks são scripts shell que executam automaticamente em resposta a eventos de uso de ferramentas pelo assistente. São o mecanismo de enforcement — forçam o cumprimento das rules sem depender de julgamento humano. Não contêm lógica de negócio ou governança — apenas enforcement.

---

## O Que São Hooks

- Scripts shell que executam automaticamente em resposta a eventos de uso de ferramentas
- Configurados em `.claude/settings.json` na chave `"hooks"`
- Dois tipos de gatilho:
  - **PostToolUse** em `Write|Edit` — executado após escrita ou edição de arquivos
  - **PostToolUse** em `Bash` — executado após cada chamada Bash

---

## Hooks Ativos

### instruction-change-detector.sh

| Campo | Valor |
|---|---|
| **Gatilho** | PostToolUse em Write\|Edit |
| **Propósito** | Detecta quando arquivos de governança são modificados, emite lembrete e executa auditoria automatizada |
| **Arquivos monitorados** | `CLAUDE.md`, `REVIEW.md`, `Instructions/**`, `.claude/rules/**`, `.claude/skills/**`, `.claude/settings.json`, `.claude/hooks/**` |
| **Comportamento** | Quando um arquivo de governança é alterado, o hook emite um lembrete para que o assistente execute o checklist de `REVIEW.md` e dispara a auditoria `governance-audit.sh` |

### branch-guard.sh

| Campo | Valor |
|---|---|
| **Gatilho** | PostToolUse em Bash |
| **Propósito** | Previne operações de branch incorretas durante análise de PR |
| **Comportamento** | Garante que commits durante pr-analysis vão para o `head.ref` do PR sendo analisado, não para um branch novo. Utiliza o arquivo de contexto `.claude/.pr-analysis-context` para determinar se a tarefa atual é uma análise de PR |
| **Proteção** | Criar um branch novo durante pr-analysis é considerado erro — todos os commits devem ser feitos no branch de origem do PR |

### session-timer.sh

| Campo | Valor |
|---|---|
| **Gatilho** | PostToolUse em Bash |
| **Propósito** | Rastreia o tempo de execução efetivo da sessão |
| **Comportamento** | Exibe o tempo efetivo acumulado após cada chamada Bash, permitindo ao assistente e ao usuário acompanhar a duração do trabalho. Utiliza o arquivo de estado `.claude/.session-timer` |
| **Cálculo** | Contabiliza apenas tempo em que o assistente está ativamente processando; períodos de espera por resposta do usuário são excluídos |

### pre-commit-gate.sh

| Campo | Valor |
|---|---|
| **Tipo** | Script de governança (não é um hook de gatilho automático) |
| **Propósito** | Executa o ciclo verificação → correção → re-verificação da auditoria de governança antes de commits |
| **Comportamento** | Executa `governance-audit.sh`, aplica `--fix` em caso de falhas, e re-executa para confirmar que todas as falhas foram resolvidas |
| **Invocação** | Manual — invocado pelo assistente como passo 0.1 do pipeline pré-commit, ou chamado por `instruction-change-detector.sh` após mudanças de governança |

---

## Localização

| Artefato | Caminho |
|---|---|
| Scripts de hooks | `.claude/hooks/` |
| Configuração dos hooks | `.claude/settings.json` |
| Arquivo de contexto (pr-analysis) | `.claude/.pr-analysis-context` |
| Arquivo de estado (timer) | `.claude/.session-timer` |

---

## Referências

- [Claude — Visão Geral](Claude-Overview)
- [Claude — Skills](Claude-Skills)
- [Claude — Convenções e Restrições](Claude-Conventions)
