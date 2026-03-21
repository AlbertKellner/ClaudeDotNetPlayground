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

| Item | Valor | Onde Está Definido |
|---|---|---|
| Usuário de teste | `Albert` | `Features/Command/UserLogin/UserLoginUseCase/UserLoginUseCase.cs` |
| Senha de teste | `albert123` | Idem |
| ID do usuário | `123` | Idem |
| JWT Secret (dev) | `super-secret-key-for-development-only-change-in-production` | `appsettings.json` → `Jwt:Secret` |

> **Aviso**: Estas credenciais são hardcoded para desenvolvimento. Em produção, devem ser substituídas por mecanismo seguro.

---

## Comandos Essenciais por Etapa

### 1. Build
```bash
dotnet build src/Albert.Playground.ECS.AOT.Api/Albert.Playground.ECS.AOT.Api.csproj
```

### 2. Execução em modo debug
```bash
dotnet run --project src/Albert.Playground.ECS.AOT.Api/Albert.Playground.ECS.AOT.Api.csproj
# Verificar: curl http://localhost:5000/health
```

### 3. Testes unitários
```bash
dotnet test src/Albert.Playground.ECS.AOT.UnitTest/Albert.Playground.ECS.AOT.UnitTest.csproj
```

### 4. Docker (Release/Native AOT)
```bash
docker compose up -d --build    # build + start
# Verificar: curl http://localhost:8080/health
```

### 5. Login (obter Bearer Token)
```bash
TOKEN=$(curl -s -X POST http://localhost:8080/login \
  -H "Content-Type: application/json" \
  -d '{"userName":"Albert","password":"albert123"}' \
  | grep -o '"token":"[^"]*"' | cut -d'"' -f4)
echo $TOKEN
```

### 6. Chamar endpoint autenticado
```bash
curl -H "Authorization: Bearer $TOKEN" http://localhost:8080/weather-conditions
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
| `POST` | `/login` | Não | Login com credenciais; retorna JWT Bearer Token | RN-002 |
| `GET` | `/test` | Sim | Endpoint de teste; retorna `"funcionando"` | RN-001 |
| `GET` | `/weather-conditions` | Sim | Condições climáticas atuais de São Paulo via Open-Meteo | RN-004 |
| `GET` | `/github-repo-search` | Sim | Pesquisa de repositórios GitHub da conta AlbertKellner | RN-008 |
| `GET` | `/pokemon` | Sim | Consulta de Pokémon por ID (Pikachu hardcoded) via PokéAPI | RN-009 |

---

## Problemas Recorrentes e Soluções

Esta tabela consolida os problemas de ambiente mais frequentes, extraídos de `bash-errors-log.md`. Para cada sintoma, a causa e a solução já são conhecidas.

| # | Sintoma | Causa | Solução | Ref |
|---|---|---|---|---|
| 1 | `Cannot connect to the Docker daemon` | Docker daemon não está em execução (sem systemd) | Iniciar manualmente: `dockerd --host=unix:///var/run/docker.sock &>/tmp/dockerd.log &` — aguardar ~5s | Erro 1 |
| 2 | `Temporary failure resolving 'archive.ubuntu.com'` no Docker build | Proxy HTTP não configurado no Docker | Criar `~/.docker/config.json` com seção `proxies` contendo `HTTP_PROXY`, `HTTPS_PROXY`, `NO_PROXY` | Erro 2 |
| 3 | `UntrustedRoot` em `dotnet restore` no Docker | CA do proxy TLS ausente no container de build | Configurar `EXTRA_CA_CERT` no `.env` (base64 do certificado) | Erro 3 |
| 4 | `unable to reliably determine the host name` no Datadog Agent | Hostname não configurado no container | Definir `DD_HOSTNAME` no `docker-compose.yml` | Erro 4 |
| 5 | `TLSErrors` no Datadog Agent | CA do proxy não está no trust store do container | Montar CA do host via volumes e adicionar ao bundle `/etc/ssl/certs/cacert.pem` | Erro 6 |
| 6 | `FQDN trailing dot` no Datadog Agent | Agent usa FQDN com ponto final; proxy não suporta | Definir `DD_CONVERT_DD_SITE_FQDN_ENABLED=false` no `docker-compose.yml` | Erro 7 |
| 7 | `gh: command not found` | GitHub CLI não instalado | Instalar via repositório oficial: `apt-get install gh` | Erro 11 |
| 8 | HTTP 404 em todos os endpoints | Controllers removidos pelo linker AOT | Verificar `AotControllerPreservation.PreserveControllers()` chamado em `Program.cs` | Erro 8 |
| 9 | HTTP 500 em `/login` — `Jwt:Secret is not configured` | `appsettings.json` não copiado para runtime no Dockerfile | Usar `COPY --from=build /app/publish/ .` (com barra) no Dockerfile | Erro 9 |
| 10 | HTTP 500 em `/weather-conditions` — `UntrustedRoot` em runtime | CA do proxy não propagada do build para o runtime stage | Copiar bundle CA: `COPY --from=build /etc/ssl/certs/ca-certificates.crt /etc/ssl/certs/` | Erro 10 |
| 11 | HTTP 401 inesperado em endpoint autenticado | Token expirado, inválido ou ausente | Regenerar via `POST /login` com credenciais válidas (Albert/albert123) | — |
| 12 | ~~`remote "origin"` não reconhecido pelo `gh`~~ | ~~Remote aponta para proxy local~~ | **Obsoleto**: migração para MCP eliminou dependência do `gh` CLI | Erro 12 |
| 13 | HTTP 500 em `/weather-conditions` no CI — `ArgumentNullException` | CWD incorreto no CI; `appsettings.json` não encontrado | Executar `cd ./app` antes de iniciar o binário no CI | Erro 13 |

---

## Serviços Externos e Dependências

| Serviço | URL Base | Autenticação | Variável de Configuração | Impacto se Indisponível |
|---|---|---|---|---|
| Open-Meteo API | `api.open-meteo.com` | Nenhuma (API pública) | `ExternalApi:OpenMeteo:BaseUrl` em `appsettings.json` | `/weather-conditions` retorna HTTP 500 |
| GitHub API | `api.github.com` | PAT (opcional) | `ExternalApi:GitHub:HttpRequest:PersonalAccessToken` + `ExternalApi:GitHub:HttpRequest:BaseUrl` em `appsettings.json` | `/github-repo-search` retorna HTTP 500 |
| PokéAPI | `pokeapi.co` | Nenhuma (API pública) | `ExternalApi:Pokemon:HttpRequest:BaseUrl` em `appsettings.json` | `/pokemon` retorna HTTP 500 |
| Datadog | `app.datadoghq.com` | API Key | `DD_API_KEY` no ambiente/`.env` | `/health` retorna `Degraded` ou `Unhealthy`; logs não fluem ao Datadog |
| Datadog MCP | `mcp.datadoghq.com` | API Key + App Key | `DD_API_KEY` + `DD_APP_KEY` no ambiente | Ferramentas MCP do Datadog ficam indisponíveis para o assistente |
| GitHub MCP | `api.githubcopilot.com` | Bearer Token (PAT) | `GH_TOKEN_MCP` no ambiente | Ferramentas MCP do GitHub ficam indisponíveis para o assistente (criação/atualização de PRs, monitoramento de Actions) |

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
| `DD_HOSTNAME` | `albert-playground-ecs-aot-local` | Hostname fixo para evitar erro de detecção |
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
- `src/Albert.Playground.ECS.AOT.Api/Dockerfile` — build multi-stage com Native AOT

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-19 | Criado: runbook operacional unificado com portas, URLs, comandos, troubleshooting e dependências externas | Instrução do usuário |
