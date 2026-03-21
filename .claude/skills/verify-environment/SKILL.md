# Skill: verify-environment

## Propósito

Executar o workflow de verificação de prontidão de ambiente e otimização de eficiência, conforme a política definida em `.claude/rules/environment-readiness.md`.

---

## Quando Usar

Esta skill é ativada pelo passo 0 do pipeline de validação pré-commit (CLAUDE.md), antes de qualquer operação de build, Docker ou deploy.

---

## Workflow

### Passo 1: Executar checklist de pré-requisitos

Aplicar o checklist antes de qualquer operação Docker:

| # | Pré-requisito | Comando de Verificação | Estado Esperado |
|---|---|---|---|
| 1 | `DD_API_KEY` no host | `env \| grep DD_API_KEY` | Linha não vazia retornada |
| 2 | Arquivo `.env` presente e preenchido | `ls .env && grep -c DD_API_KEY .env` | Arquivo existe, contém `DD_API_KEY` |
| 3 | Docker daemon em execução | `ls /var/run/docker.sock` | Socket presente |
| 4 | `~/.docker/config.json` com proxy | `grep -q proxies ~/.docker/config.json && echo ok` | Saída `ok` |
| 5 | CA do proxy disponível no host | `ls /usr/local/share/ca-certificates/swp-ca-production.crt` | Arquivo presente |
| 6 | `DD_APP_KEY` no host (MCP Datadog) | `env \| grep DD_APP_KEY` | Linha não vazia retornada |
| 7 | `.mcp.json` presente e configurado | `ls .mcp.json && grep -q mcpServers .mcp.json && echo ok` | Saída `ok` |
| 8 | `GH_TOKEN` no host (GitHub API) | `env \| grep GH_TOKEN` | Linha não vazia retornada |
| 9 | .NET SDK disponível no PATH | `dotnet --version` | Versão 10.x retornada |

**Os pré-requisitos 1–5 devem estar satisfeitos antes de executar `docker compose up`.** Os pré-requisitos 6–8 são necessários para recursos operacionais do assistente (MCP e GitHub). O pré-requisito 9 deve estar satisfeito antes de qualquer operação `dotnet`.

### Passo 2: Tratar ambiente não pronto

Se algum pré-requisito estiver ausente:

1. **Identificar** qual pré-requisito está ausente e qual trecho do script o cobre
2. **Verificar** se o trecho correspondente já existe em `scripts/setup-env.sh`
3. **Se não existir:** atualizar `scripts/setup-env.sh` com o trecho que resolve o pré-requisito ausente
4. **Sinalizar ao usuário** com a mensagem padronizada:

```
[Ambiente não pronto — ferramenta externa desatualizada]

Pré-requisito ausente: [descrição do que está faltando]
Causa provável: scripts/setup-env.sh foi atualizado mas a ferramenta externa de configuração de container ainda não foi sincronizada.

O script foi atualizado nesta sessão. Copie o conteúdo atualizado de scripts/setup-env.sh para a ferramenta de configuração de container e reaplique o ambiente antes de continuar.
```

5. **Não prosseguir** com operações que dependem do pré-requisito ausente até confirmação do usuário.

### Passo 3: Avaliar eficiência de execução

Antes de iniciar qualquer sequência de operações, avaliar ativamente:

1. **Reutilização de artefatos já gerados**
   - Imagem Docker já construída nesta sessão sem alteração de código → usar `docker compose up -d` sem `--build`
   - `.env` já presente e com `DD_API_KEY` válida → não recriar
   - `dotnet build` já passou sem erros e nenhum arquivo foi alterado → não re-executar
   - `/health` já retornou 200 e não houve alterações de código → o pipeline de validação não precisa ser re-executado

2. **Antecipação de validações**
   - Verificar pré-requisitos de ambiente ANTES de iniciar operações custosas
   - Verificar que o branch e o estado do repositório estão corretos ANTES de um commit
   - Verificar que o `dotnet build` passa ANTES de tentar o `docker compose up`

3. **Eliminação de etapas redundantes**
   - Não executar `docker compose down` + `docker compose up` quando apenas `docker compose restart` é necessário
   - Não re-instalar dependências já presentes
   - Não re-verificar condições já verificadas no mesmo fluxo

4. **Preferência por operações reversíveis e mais rápidas**
   - Quando duas abordagens produzem o mesmo resultado, preferir a mais rápida e menos destrutiva

### Passo 4: Evolução do script de bootstrap

Quando um pré-requisito ausente não estiver coberto pelo script `scripts/setup-env.sh`:
1. Atualizar `scripts/setup-env.sh` com o trecho que resolve o pré-requisito
2. Atualizar `scripts/required-vars.md` se for variável de ambiente
3. Atualizar `scripts/container-setup.md` se for dependência de sistema
4. Emitir sinal padronizado ao usuário (Passo 2)

### Passo 5: Conversão de problemas recorrentes

Quando um problema de ambiente ocorrer mais de uma vez em sessões distintas:

1. **Registrar** o erro em `bash-errors-log.md`
2. **Classificar** o problema:
   - **Prevenível por checklist**: adicionar novo item ao checklist (Passo 1)
   - **Prevenível por configuração**: atualizar `scripts/setup-env.sh`
   - **Prevenível por documentação**: adicionar a `scripts/operational-runbook.md`
3. **Atualizar** o artefato correspondente na mesma sessão
4. **Sinalizar** ao usuário se a correção exigir reconfiguração

---

## Arquivos de Governança Relacionados

- `.claude/rules/environment-readiness.md` — política que este workflow implementa
- `.claude/rules/bash-error-logging.md` — erros de ambiente devem ser registrados
- `scripts/setup-env.sh` — script de bootstrap (modelo declarativo)
- `scripts/required-vars.md` — variáveis de ambiente necessárias
- `scripts/container-setup.md` — dependências de sistema
- `scripts/operational-runbook.md` — referência operacional unificada

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-21 | Criado: workflow extraído de environment-readiness.md (separação rules/skills) | Auditoria de governança |
