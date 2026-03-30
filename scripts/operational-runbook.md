# Runbook Operacional do Projeto

## Propósito

Este arquivo é o ponto de entrada único para qualquer pessoa que precise executar, depurar ou validar este projeto. Concentra as informações operacionais mais consultadas — portas, URLs, comandos, credenciais de teste, problemas recorrentes e dependências externas — em um único local, eliminando a necessidade de consultar múltiplos arquivos para responder perguntas operacionais básicas.

---

## Referência Rápida — Portas, URLs e Serviços

| Serviço | Contexto | Host:Porta | URL de Verificação | Propósito |
|---|---|---|---|---|
| Aplicação | Docker (`docker compose`) | `localhost:8080` | `GET http://localhost:8080/health` | Aplicação em modo Release/Native AOT via container |
| Aplicação | Debug (`dotnet run`) | `localhost:5000` | `GET http://localhost:5000/health` | Aplicação em modo debug local |
| Aplicação | CI (GitHub Actions) | `localhost:5000` | `GET http://localhost:5000/health` | Execução no pipeline de CI |
| Datadog Agent | Docker (interno) | `datadog-agent:8126` | — | Recebe traces e logs da aplicação via rede Docker |
| Datadog Agent | CI | `localhost:8126` | — | Recebe traces e logs no pipeline de CI |

> **Nota**: A porta `8080` é usada apenas via Docker. Em modo debug e no CI, a porta padrão é `5000` (configurável via `ASPNETCORE_URLS`).

---

## Credenciais de Teste

> **Nota**: As credenciais abaixo são exemplos do template. Devem ser substituídas conforme o domínio do projeto.

| Item | Valor | Onde Está Definido |
|---|---|---|
| Usuário de teste | Definido no UseCase de login | UseCase da Feature de login em `Features/Command/` |
| Senha de teste | Definida no UseCase de login | Idem |
| ID do usuário | Definido no UseCase de login | Idem |
| JWT Secret (dev) | `super-secret-key-for-development-only-change-in-production` | `appsettings.json` → `Jwt:Secret` |

> **Aviso**: Estas credenciais são hardcoded para desenvolvimento. Em produção, devem ser substituídas por mecanismo seguro.

---

## Comandos Essenciais por Etapa

### 1. Build
```bash
dotnet build src/Starter.Template.AOT.Api/Starter.Template.AOT.Api.csproj
```

### 2. Execução em modo debug
```bash
dotnet run --project src/Starter.Template.AOT.Api/Starter.Template.AOT.Api.csproj
# Verificar: curl http://localhost:5000/health
```

### 3. Testes unitários
```bash
dotnet test src/Starter.Template.AOT.UnitTest/Starter.Template.AOT.UnitTest.csproj
```

### 4. Docker (Release/Native AOT)
```bash
docker compose up -d --build    # build + start
# Verificar: curl http://localhost:8080/health
```

### 5. Login (obter Bearer Token)
```bash
# Substituir userName e password pelas credenciais definidas no UseCase de login
TOKEN=$(curl -s -X POST http://localhost:8080/login \
  -H "Content-Type: application/json" \
  -d '{"userName":"<usuario>","password":"<senha>"}' \
  | grep -o '"token":"[^"]*"' | cut -d'"' -f4)
echo $TOKEN
```

### 6. Chamar endpoint autenticado
```bash
# Substituir pela rota do endpoint implementado
curl -H "Authorization: Bearer $TOKEN" "http://localhost:8080/<rota-do-endpoint>"
```

### 7. Logs do container da aplicação
```bash
docker logs $(docker compose ps -q app) --tail 50
```

### 8. Parar containers
```bash
docker compose down
```

---

## Endpoints Disponíveis

| Método | Rota | Autenticação | Descrição | Regra de Negócio |
|---|---|---|---|---|
| `GET` | `/health` | Não | Verificação de disponibilidade (app + Datadog Agent) | RN-005 |

> **Nota**: Adicionar endpoints à medida que features forem implementadas. O endpoint `/health` é o único presente no template base.

---

## Problemas Recorrentes e Soluções

Esta tabela consolida os problemas de ambiente mais frequentes, extraídos de `bash-errors-log.md`. Para cada sintoma, a causa e a solução já são conhecidas.

| # | Sintoma | Causa | Solução | Ref |
|---|---|---|---|---|

> **Nota**: Esta tabela é preenchida à medida que problemas são encontrados e documentados em `bash-errors-log.md`. No template base, não há problemas recorrentes registrados.

---

## Serviços Externos e Dependências

| Serviço | URL Base | Autenticação | Variável de Configuração | Impacto se Indisponível |
|---|---|---|---|---|
| Datadog | `app.datadoghq.com` | API Key | `DD_API_KEY` no ambiente/`.env` | `/health` retorna `Degraded` ou `Unhealthy`; logs não fluem ao Datadog |
| Datadog MCP | `mcp.datadoghq.com` | API Key + App Key | `DD_API_KEY` + `DD_APP_KEY` no ambiente | Ferramentas MCP do Datadog ficam indisponíveis para o assistente |
| GitHub MCP | `api.githubcopilot.com` | Bearer Token (PAT) | `GH_CLAUDE_CODE_MCP` no ambiente | Ferramentas MCP do GitHub ficam indisponíveis para o assistente (criação/atualização de PRs, monitoramento de Actions) |

---

## Containers Docker

| Serviço (docker-compose) | Container Name | Imagem | Portas Expostas |
|---|---|---|---|
| `app` | (gerado pelo compose) | Build local via Dockerfile | `8080:8080` |
| `datadog-agent` | `dd-agent` | `registry.datadoghq.com/agent:7` | Nenhuma exposta ao host (rede interna) |

### Variáveis de Ambiente do Datadog Agent (docker-compose)

| Variável | Valor | Propósito |
|---|---|---|
| `DD_API_KEY` | Secret do ambiente | Autenticação com Datadog |
| `DD_SITE` | `datadoghq.com` | Região do Datadog |
| `DD_ENV` | `${DD_ENV:-local}` | Ambiente para filtragem (local, ci, build) |
| `DD_HOSTNAME` | `starter-template-aot-local` | Hostname fixo para evitar erro de detecção |
| `DD_LOGS_ENABLED` | `true` | Coleta de logs ativa |
| `DD_LOGS_CONFIG_CONTAINER_COLLECT_ALL` | `true` | Coleta de todos os containers |
| `DD_CONVERT_DD_SITE_FQDN_ENABLED` | `false` | Desabilita FQDN com trailing dot (incompatível com proxy) |
| `DD_DOGSTATSD_NON_LOCAL_TRAFFIC` | `true` | Aceita métricas de outros containers |

---

## Referências Cruzadas

- `scripts/required-vars.md` — detalhes de cada secret e variável de ambiente, ciclo de vida de credenciais
- `scripts/container-setup.md` — dependências de sistema e configuração do container
- `scripts/setup-env.sh` — script de bootstrap (modelo declarativo)
- `.claude/rules/environment-readiness.md` — política de validação de ambiente do agente
- `bash-errors-log.md` — histórico completo e detalhado de todos os erros encontrados
- `docker-compose.yml` — definição dos containers
- `src/Starter.Template.AOT.Api/Dockerfile` — build multi-stage com Native AOT

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-19 | Criado: runbook operacional unificado com portas, URLs, comandos, troubleshooting e dependências externas | Instrução do usuário |
