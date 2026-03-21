# Regra: Prontidão de Ambiente e Eficiência de Execução

## Propósito

Esta rule define a política de verificação de prontidão de ambiente e eficiência de execução. O workflow procedural está em `.claude/skills/verify-environment/SKILL.md`.

---

## Princípio Fundamental

> Falhas de ambiente previsíveis são responsabilidade do agente, não do usuário.
> Verificar antes de executar custa segundos. Recuperar de falhas em cascata custa minutos.
> O ambiente deve estar pronto antes que qualquer operação substantiva comece.
> Eficiência não é um objetivo secundário — é parte obrigatória de toda tarefa.

---

## Políticas

### Script de Bootstrap

`scripts/setup-env.sh` é um **modelo declarativo de configuração de ambiente**. O agente nunca deve executá-lo diretamente. O ambiente deve chegar já pronto. Se um pré-requisito estiver ausente, o agente atualiza o script e sinaliza ao usuário para sincronizar a ferramenta externa.

### Arquivos de Registro

| Arquivo | Propósito |
|---|---|
| `scripts/required-vars.md` | Variáveis de ambiente e secrets que a ferramenta externa deve prover |
| `scripts/container-setup.md` | Dependências de sistema, PATH, SDKs, permissões |
| `scripts/operational-runbook.md` | Portas, URLs, comandos, credenciais de teste, troubleshooting |

### Eficiência de Execução

Em toda tarefa, antes de iniciar qualquer sequência de operações, avaliar ativamente:
- Existe artefato já gerado que pode ser reutilizado?
- Existe etapa que pode ser antecipada para evitar falha custosa posterior?
- Existe etapa redundante que pode ser eliminada?
- Existe abordagem mais rápida e reversível que produz o mesmo resultado?

### Conversão de Problemas Recorrentes

Todo problema recorrente de ambiente deve ser convertido em pré-requisito verificável:
- Prevenível por checklist → adicionar ao checklist de pré-requisitos na skill
- Prevenível por configuração → atualizar `scripts/setup-env.sh`
- Prevenível por documentação → adicionar a `scripts/operational-runbook.md`

---

## Workflow

O workflow completo (checklist de pré-requisitos, protocolo de ambiente não pronto, avaliação de eficiência, evolução do script e conversão de problemas recorrentes) está definido em `.claude/skills/verify-environment/SKILL.md`.

---

## Relação com Outras Rules

- `bash-error-logging.md` — registra erros APÓS ocorrerem; esta rule previne que ocorram
- `governance-policies.md` — políticas de ambiguidade (§4) e contexto do repositório (§2) aplicáveis a ambiente
- `governance-audit.md` — a auditoria automatizada complementa a verificação de prontidão

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-18 | Criado: regra de prontidão de ambiente | Instrução do usuário |
| 2026-03-19 | Expandido: eficiência de execução, evolução do script, conversão de problemas recorrentes | Instrução do usuário |
| 2026-03-21 | Refatorado: workflows procedurais extraídos para skill verify-environment; rule simplificada para conter apenas política | Auditoria de governança |
