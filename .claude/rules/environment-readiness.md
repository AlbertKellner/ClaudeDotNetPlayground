# Regra: Prontidão de Ambiente e Eficiência de Execução

## Propósito

Esta rule define dois comportamentos obrigatórios e complementares:

1. **Verificação de prontidão de ambiente**: antes de qualquer pipeline de build, execução Docker ou deploy, verificar e preparar proativamente todas as dependências do ambiente deste repositório.
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

### O Script Canônico

Este repositório mantém `scripts/setup-env.sh` como o script de bootstrap canônico de ambiente. Ele consolida em um único lugar executável toda preparação necessária para que o pipeline de build e run funcione na primeira tentativa.

**O script deve ser executado:**
- No início de toda sessão de trabalho que envolverá operações Docker
- Antes de qualquer `docker compose up`, `docker build` ou operação que dependa do Docker daemon
- Sempre que houver dúvida sobre o estado do ambiente

**O script é idempotente:** executá-lo quando o ambiente já está corretamente configurado é uma no-op segura — nenhum estado existente é destruído.

### Responsabilidade do Script de Bootstrap

O script `scripts/setup-env.sh` deve conter **tudo** o que for necessário para preparar o ambiente do zero, incluindo:

- Verificação e inicialização do Docker daemon
- Criação do arquivo `.env` com todas as variáveis obrigatórias (`DD_API_KEY`, `EXTRA_CA_CERT`)
- Configuração do proxy HTTP no `~/.docker/config.json`
- Exportação de variáveis de ambiente necessárias para build e runtime
- Verificação de disponibilidade de certificados TLS e CA do proxy
- Qualquer outro pré-requisito recorrente identificado em sessões anteriores

**Princípio de concentração**: dependências conhecidas, sensíveis ou recorrentes devem ser resolvidas no script de bootstrap, não tratadas de forma reativa durante tentativas de build, run ou debug. Se uma dependência é conhecida antes da execução, sua verificação e preparação pertencem ao script.

### Quando NÃO Executar o Script

- Quando o ambiente já foi verificado nesta sessão e nenhuma alteração estrutural ocorreu
- Quando a tarefa não envolve Docker (ex: somente `dotnet build` sem run)
- Quando o agente verificou manualmente que todos os pré-requisitos estão satisfeitos (ver checklist abaixo)

---

## Parte 2: Checklist de Pré-requisitos

Quando `scripts/setup-env.sh` não estiver disponível ou quando for necessária verificação manual, aplicar este checklist antes de qualquer operação Docker:

| # | Pré-requisito | Comando de Verificação | Estado Esperado |
|---|---|---|---|
| 1 | `DD_API_KEY` no host | `env \| grep DD_API_KEY` | Linha não vazia retornada |
| 2 | Arquivo `.env` presente e preenchido | `ls .env && grep -c DD_API_KEY .env` | Arquivo existe, contém `DD_API_KEY` |
| 3 | Docker daemon em execução | `ls /var/run/docker.sock` | Socket presente |
| 4 | `~/.docker/config.json` com proxy | `grep -q proxies ~/.docker/config.json && echo ok` | Saída `ok` |
| 5 | CA do proxy disponível no host | `ls /usr/local/share/ca-certificates/swp-ca-production.crt` | Arquivo presente |

**Todos os 5 pré-requisitos devem estar satisfeitos antes de executar `docker compose up`.**

### Preparação Manual por Pré-requisito

Se o script `setup-env.sh` não puder ser executado, preparar cada dependência ausente individualmente:

#### Pré-requisito 2 ausente — Arquivo `.env` ausente ou incompleto

```bash
DD_API_KEY=$(env | grep DD_API_KEY | cut -d= -f2)
EXTRA_CA_CERT=$(base64 -w 0 /usr/local/share/ca-certificates/swp-ca-production.crt 2>/dev/null || echo "")
echo "DD_API_KEY=${DD_API_KEY}" > .env
[ -n "$EXTRA_CA_CERT" ] && echo "EXTRA_CA_CERT=${EXTRA_CA_CERT}" >> .env
```

Se `DD_API_KEY` estiver vazia no host: registrar premissa em `assumptions-log.md` — execução continuará sem Datadog, apenas com build e health check local.

#### Pré-requisito 3 ausente — Docker daemon não em execução

```bash
dockerd --host=unix:///var/run/docker.sock &>/tmp/dockerd.log &
sleep 6
ls /var/run/docker.sock
```

O daemon não inicia automaticamente neste ambiente (sem systemd). Esse comportamento é esperado — não registrar como erro. Ver PREM-004 em `assumptions-log.md`.

#### Pré-requisito 4 ausente — `~/.docker/config.json` sem proxy

```bash
HTTP_PROXY_VAL=$(env | grep -i "^HTTP_PROXY\|^http_proxy" | head -1 | cut -d= -f2-)
HTTPS_PROXY_VAL=$(env | grep -i "^HTTPS_PROXY\|^https_proxy" | head -1 | cut -d= -f2-)
NO_PROXY_VAL=$(env | grep -i "^NO_PROXY\|^no_proxy" | head -1 | cut -d= -f2-)
mkdir -p ~/.docker
cat > ~/.docker/config.json <<EOF
{
  "proxies": {
    "default": {
      "httpProxy": "${HTTP_PROXY_VAL}",
      "httpsProxy": "${HTTPS_PROXY_VAL}",
      "noProxy": "${NO_PROXY_VAL}"
    }
  }
}
EOF
```

Esta configuração é necessária para que containers de build herdem o proxy do host. Ver Erros 2 e 3 em `bash-errors-log.md`.

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

### Identificação de Melhorias

Sempre que o agente executar uma configuração manual de ambiente que não estava no `scripts/setup-env.sh`, deve:

1. **Identificar** que essa configuração poderia ter sido feita pelo script
2. **Reportar ao usuário** ao final da tarefa, descrevendo claramente:
   - O que foi feito manualmente
   - Por que isso poderia estar no script
   - O trecho exato que poderia ser adicionado

3. **Não alterar o script autonomamente** — o script de bootstrap é um artefato de infraestrutura relevante e sua evolução requer instrução explícita do usuário

4. **Formatar a sugestão como snippet pronto para copiar**

### Formato Obrigatório de Feedback

```
[Sugestão de melhoria para scripts/setup-env.sh]

O que foi feito manualmente nesta sessão: [descrição]
Por que pertence ao script: [justificativa]

Trecho sugerido:

---
[código exato]
---

Adicionar ao scripts/setup-env.sh eliminará esse passo manual nas próximas sessões.
```

### Exemplos de Situações que Geram Feedback

- Docker proxy foi configurado manualmente em `~/.docker/config.json`
- Arquivo `.env` foi criado manualmente após o pipeline falhar
- Docker daemon foi iniciado manualmente com `dockerd &`
- Certificado CA foi copiado manualmente para o container
- Qualquer variável de ambiente foi exportada manualmente para corrigir uma falha
- Qualquer diretório ou arquivo de configuração foi criado manualmente para desbloquear uma operação

---

## Relação com Outras Rules

- `bash-error-logging.md` — registra erros APÓS ocorrerem; esta rule previne que ocorram
- `implementation-alignment.md` — esta rule complementa o Passo 0 do pipeline de validação pré-commit
- `ambiguity-handling.md` — se um pré-requisito de ambiente for ambíguo, aplicar lógica de premissa conservadora e registrar
- `repository-context-evolution.md` — o script `setup-env.sh` e os pré-requisitos identificados são contexto acumulado e durável deste repositório
