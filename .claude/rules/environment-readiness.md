# Regra: Prontidão de Ambiente e Eficiência de Execução

## Propósito

Esta rule define dois comportamentos obrigatórios e complementares:

1. **Verificação de prontidão de ambiente**: antes de qualquer pipeline de build, execução Docker ou deploy, verificar os pré-requisitos do ambiente. O ambiente deve chegar já configurado — se não estiver, atualizar o script canônico e sinalizar ao usuário.
2. **Eficiência de execução**: em toda tarefa, identificar e eliminar etapas desnecessárias, antecipar validações e reaproveitar artefatos já disponíveis.

Estes comportamentos eliminam o padrão de falha documentado em `bash-errors-log.md`: o agente executa etapas custosas, encontra um erro previsível apenas no final, corrige o ambiente e reinicia tudo do zero. Esse ciclo é inaceitável e prevenível.

---

## Princípio Fundamental

> Falhas de ambiente previsíveis são responsabilidade do agente, não do usuário.
> Verificar antes de executar custa segundos. Recuperar de falhas em cascata custa minutos.
> O ambiente deve estar pronto antes que qualquer operação substantiva comece.
> Eficiência não é um objetivo secundário — é parte obrigatória de toda tarefa.

---

## Parte 1: Script de Bootstrap de Ambiente

### Natureza do Script

`scripts/setup-env.sh` é um **modelo declarativo de configuração de ambiente**. Ele não é executado pelo agente — é copiado manualmente pelo usuário em uma ferramenta externa de configuração de container (ex: devcontainer, ambiente de CI, configurador de sandbox). O ambiente resultante deve chegar ao agente já pronto para uso.

**O agente nunca deve executar `scripts/setup-env.sh` diretamente** nem tentar replicar seu conteúdo manualmente como workaround de ambiente não configurado.

### Responsabilidade do Script

O script `scripts/setup-env.sh` deve conter **tudo** o que for necessário para preparar o ambiente do zero, incluindo:

- Verificação e inicialização do Docker daemon
- Criação do arquivo `.env` com todas as variáveis obrigatórias (`DD_API_KEY`, `EXTRA_CA_CERT`)
- Configuração do proxy HTTP no `~/.docker/config.json`
- Exportação de variáveis de ambiente necessárias para build e runtime
- Verificação de disponibilidade de certificados TLS e CA do proxy
- Qualquer outro pré-requisito recorrente identificado em sessões anteriores

**Princípio de concentração**: toda dependência conhecida, sensível ou recorrente pertence ao script — não deve ser tratada de forma reativa durante build, run ou debug.

### Arquivos de Registro da Ferramenta Externa

Dois arquivos documentam o que deve ser configurado na ferramenta externa:

| Arquivo | Propósito |
|---|---|
| `scripts/required-vars.md` | Registro de **variáveis de ambiente e secrets** que a ferramenta externa deve prover ao container |
| `scripts/container-setup.md` | Guia de **configuração do container** na ferramenta externa: dependências de sistema, PATH, SDKs, permissões |

O agente consulta esses arquivos para saber o que validar e o que referenciar nas mensagens de erro quando o ambiente não estiver pronto.

### Quando o Ambiente Não Está Pronto

Se o agente detectar que um pré-requisito do ambiente está ausente (ex: `.env` inexistente, Docker daemon parado, proxy não configurado), isso significa que a ferramenta externa não foi atualizada com a versão mais recente do script.

**Comportamento obrigatório do agente nesse caso:**

1. **Identificar** qual pré-requisito está ausente e qual trecho do script o cobre
2. **Verificar** se o trecho correspondente já existe em `scripts/setup-env.sh`
3. **Se não existir:** atualizar `scripts/setup-env.sh` com o trecho que resolve o pré-requisito ausente
4. **Sinalizar ao usuário** com a seguinte mensagem padronizada:

```
[Ambiente não pronto — ferramenta externa desatualizada]

Pré-requisito ausente: [descrição do que está faltando]
Causa provável: scripts/setup-env.sh foi atualizado mas a ferramenta externa de configuração de container ainda não foi sincronizada.

O script foi atualizado nesta sessão. Copie o conteúdo atualizado de scripts/setup-env.sh para a ferramenta de configuração de container e reaplique o ambiente antes de continuar.
```

5. **Não prosseguir** com operações que dependem do pré-requisito ausente até que o usuário confirme que o ambiente foi reconfigurado.

---

## Parte 2: Checklist de Pré-requisitos

Aplicar este checklist antes de qualquer operação Docker para verificar se o ambiente está pronto:

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

**Os pré-requisitos 1–5 devem estar satisfeitos antes de executar `docker compose up`.** Os pré-requisitos 6–8 são necessários para recursos operacionais do assistente (MCP e GitHub) e devem ser validados antes de usar os recursos correspondentes.

Se algum pré-requisito estiver ausente → seguir o protocolo da Parte 1 (atualizar script + sinalizar usuário). O agente não deve tentar corrigir o ambiente manualmente.

---

## Parte 3: Eficiência de Execução (Instrução Permanente)

### Escopo

Esta instrução aplica-se a **toda tarefa**, não apenas ao pipeline de build/run.

### Comportamento Obrigatório

Em toda tarefa, antes de iniciar qualquer sequência de operações, o agente deve avaliar ativamente:

1. **Reutilização de artefatos já gerados**
   - Imagem Docker já construída nesta sessão sem alteração de código → usar `docker compose up -d` sem `--build`
   - `.env` já presente e com `DD_API_KEY` válida → não recriar
   - `dotnet build` já passou sem erros e nenhum arquivo foi alterado → não re-executar antes do `docker compose up`
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
   - Operações que afetam estado persistente (arquivos, imagens Docker, branches) devem ser feitas somente quando necessário

5. **Simplificação do fluxo**
   - Se uma tarefa pode ser concluída em 2 passos em vez de 5, usar 2
   - Etapas de verificação que nunca falham no contexto atual podem ser omitidas

### Avaliação Obrigatória

Antes de iniciar qualquer sequência de mais de 2 operações, formular internamente:
> "Existe alguma etapa desta sequência que posso eliminar, antecipar ou substituir por algo mais rápido, mantendo o resultado correto?"

Se a resposta for sim, aplicar a otimização. Se a resposta for não, prosseguir.

---

## Parte 4: Evolução do Script de Bootstrap

### Quando Atualizar o Script

O agente **pode e deve** atualizar `scripts/setup-env.sh` autonomamente quando:
- Um pré-requisito ausente no ambiente não está coberto pelo script
- Uma nova dependência de ambiente for identificada durante uma sessão
- Um comando manual que resolve um problema recorrente não está no script

Nesses casos, o agente atualiza o script e emite o sinal padronizado (ver Parte 1) para que o usuário sincronize a ferramenta externa.

### Exemplos de Situações que Disparam Atualização

- Novo pré-requisito identificado que o script ainda não verifica
- Nova variável de ambiente necessária que o script não exporta
- Nova dependência de certificado ou proxy não coberta
- Novo diretório, arquivo de configuração ou permissão necessária ao ambiente

---

## Relação com Outras Rules

- `bash-error-logging.md` — registra erros APÓS ocorrerem; esta rule previne que ocorram
- `governance-policies.md` — políticas de ambiguidade (§4) e contexto do repositório (§2) aplicáveis a ambiente
- `Instructions/architecture/technical-overview.md` — seção "Recursos Operacionais do Assistente" lista os recursos que dependem dos pré-requisitos 6–8
