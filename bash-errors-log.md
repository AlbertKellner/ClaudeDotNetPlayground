# Log de Erros de Bash

Este arquivo documenta todos os erros de Bash encontrados durante sessões de trabalho neste repositório, incluindo causa raiz e solução adotada. É um log acumulativo — erros não são removidos após resolvidos.

## Template de Registro

```markdown
## Erro [N] — [Título descritivo do problema]

| Campo | Valor |
|---|---|
| **Número** | [N] |
| **Data** | [YYYY-MM-DD] |
| **Comando executado** | `[comando exato que falhou]` |
| **Erro retornado** | `[mensagem de erro exata]` |
| **Causa** | [Explicação técnica objetiva da causa raiz] |
| **Novo comando / solução** | `[comando ou sequência que resolveu]` |
```

---

---

## Erro 1 — Docker daemon não estava em execução

| Campo | Valor |
|---|---|
| **Número** | 1 |
| **Data** | 2026-03-17 |
| **Comando executado** | `docker compose down --remove-orphans && docker compose up -d --build` |
| **Erro retornado** | `Cannot connect to the Docker daemon at unix:///var/run/docker.sock. Is the docker daemon running?` |
| **Causa** | O Docker daemon não é iniciado automaticamente neste ambiente (sem systemd). O socket `/var/run/docker.sock` não existe até que o daemon seja iniciado manualmente. |
| **Novo comando / solução** | Iniciar o daemon manualmente: `dockerd --host=unix:///var/run/docker.sock &>/tmp/dockerd.log &` e aguardar 5 segundos antes de prosseguir. |

---

## Erro 2 — apt-get falha por ausência de resolução DNS no container Docker

| Campo | Valor |
|---|---|
| **Número** | 2 |
| **Data** | 2026-03-17 |
| **Comando executado** | `docker compose up -d --build` (step `RUN apt-get update && apt-get install -y clang zlib1g-dev`) |
| **Erro retornado** | `Temporary failure resolving 'archive.ubuntu.com'` / `E: Unable to locate package clang` |
| **Causa** | O ambiente usa um proxy HTTP autenticado (`21.0.0.183:15004`) para acesso à internet. Docker containers não herdam as variáveis de proxy do host por padrão. Sem proxy, `apt-get` não consegue resolver DNS nem acessar os repositórios Ubuntu. |
| **Novo comando / solução** | Configurar o proxy no Docker via `~/.docker/config.json` com a seção `proxies`, passando `HTTP_PROXY`, `HTTPS_PROXY` e `NO_PROXY` do ambiente host. Após a configuração, os containers de build herdam automaticamente as configurações de proxy. |

---

## Erro 3 — dotnet restore falha por certificado TLS inválido (UntrustedRoot)

| Campo | Valor |
|---|---|
| **Número** | 3 |
| **Data** | 2026-03-17 |
| **Comando executado** | `docker compose up -d --build` (step `RUN dotnet restore ClaudeDotNetPlayground.csproj`) |
| **Erro retornado** | `NU1301: Unable to load the service index for source https://api.nuget.org/v3/index.json. The SSL connection could not be established. The remote certificate is invalid because of errors in the certificate chain: UntrustedRoot` |
| **Causa** | O proxy intercepta tráfego HTTPS (SSL inspection) e apresenta seu próprio certificado assinado por uma CA interna da Anthropic (`sandbox-egress-production TLS Inspection CA`). O container de build não confia nessa CA, causando falha de TLS. |
| **Novo comando / solução** | Modificar o `Dockerfile` para aceitar um `ARG EXTRA_CA_CERT` (certificado CA em base64). Quando fornecido, decodifica e instala o certificado via `update-ca-certificates` antes do `dotnet restore`. Adicionar `EXTRA_CA_CERT` ao `.env` (base64 do arquivo `/usr/local/share/ca-certificates/swp-ca-production.crt`). Atualizar `docker-compose.yml` para passar o `args: EXTRA_CA_CERT: ${EXTRA_CA_CERT:-}`. |

---

## Erro 4 — Datadog Agent encerra por não conseguir determinar hostname

| Campo | Valor |
|---|---|
| **Número** | 4 |
| **Data** | 2026-03-17 |
| **Comando executado** | `docker compose up -d` (container `datadog-agent`) |
| **Erro retornado** | `CORE | ERROR | unable to reliably determine the host name. You can define one in the agent config file or in your hosts file` / `AGENT EXITED WITH CODE 255` |
| **Causa** | O Datadog Agent v7 falha ao iniciar quando não consegue determinar o hostname do host automaticamente. Neste ambiente sandbox, o container não tem acesso ao hostname do host via mecanismos padrão. |
| **Novo comando / solução** | Adicionar `DD_HOSTNAME=claudedotnetplayground-local` nas variáveis de ambiente do serviço `datadog-agent` no `docker-compose.yml`. |

---

## Erro 5 — Datadog Agent falha ao copiar CA cert (caminho inexistente)

| Campo | Valor |
|---|---|
| **Número** | 5 |
| **Data** | 2026-03-17 |
| **Comando executado** | `docker compose up -d` (entrypoint override: `cp /custom-certs/proxy-ca.crt /usr/local/share/ca-certificates/proxy-ca.crt`) |
| **Erro retornado** | `cp: cannot create regular file '/usr/local/share/ca-certificates/proxy-ca.crt': No such file or directory` |
| **Causa** | A imagem `registry.datadoghq.com/agent:7` não possui o diretório `/usr/local/share/ca-certificates/` e não tem `update-ca-certificates`. O bundle CA está em `/etc/ssl/certs/cacert.pem`. |
| **Novo comando / solução** | Modificar o entrypoint para usar `cat /custom-certs/proxy-ca.crt >> /etc/ssl/certs/cacert.pem` (append ao bundle existente) antes de executar `/bin/entrypoint.sh`. |

---

## Erro 6 — Datadog Agent reporta TLSErrors por CA do proxy não confiada

| Campo | Valor |
|---|---|
| **Número** | 6 |
| **Data** | 2026-03-17 |
| **Comando executado** | `docker exec dd-agent agent status` |
| **Erro retornado** | `TLSErrors: 12` / `Unexpected response code from the API Key validation endpoint` |
| **Causa** | O Datadog Agent utiliza o proxy HTTP do ambiente (detectado automaticamente via `HTTP_PROXY`/`HTTPS_PROXY`), mas a CA do proxy não estava no trust store do container, causando falhas TLS ao conectar ao `app.datadoghq.com`. |
| **Novo comando / solução** | Montar o arquivo de CA do host (`/usr/local/share/ca-certificates/swp-ca-production.crt`) no container via `volumes` e adicionar ao bundle `/etc/ssl/certs/cacert.pem` no entrypoint customizado. |

---

## Erro 7 — Datadog Agent falha em endpoints FQDN com ponto final (trailing dot)

| Campo | Valor |
|---|---|
| **Número** | 7 |
| **Data** | 2026-03-17 |
| **Comando executado** | `docker exec dd-agent agent status` |
| **Erro retornado** | `Connection to 'https://app.datadoghq.com./api/v1/series' failed. The connection to the fully qualified domain name (FQDN) "app.datadoghq.com." failed, but the connection to "app.datadoghq.com" (without trailing dot) succeeded.` |
| **Causa** | O Datadog Agent v7 usa FQDNs com ponto final (ex: `app.datadoghq.com.`) por padrão. O proxy HTTP intermediário não suporta FQDNs com ponto final no hostname. |
| **Novo comando / solução** | Adicionar `DD_CONVERT_DD_SITE_FQDN_ENABLED=false` nas variáveis de ambiente do `datadog-agent` no `docker-compose.yml`. Com isso, o agente usa hostnames sem ponto final, compatíveis com o proxy. |

---

---

## Erro 8 — Controllers não encontrados no modo AOT (404 em todos os endpoints de controller)

| Campo | Valor |
|---|---|
| **Número** | 8 |
| **Data** | 2026-03-18 |
| **Comando executado** | `curl -s -X POST http://localhost:8080/login ...` |
| **Erro retornado** | `HTTP 404` + log: `Request reached the end of the middleware pipeline without being handled by application code` |
| **Causa** | `AotControllerPreservation.PreserveControllers()` era `private` e nunca chamado. Em Native AOT, métodos privados não chamados são removidos pelo linker junto com seus atributos `[DynamicDependency]`. Sem as `DynamicDependency` ativas, os tipos de Controller eram trimados e `app.MapControllers()` não encontrava nenhuma rota. |
| **Novo comando / solução** | Tornar `PreserveControllers()` `internal` e chamar `AotControllerPreservation.PreserveControllers()` explicitamente em `Program.cs` antes de `app.Run()`. Isso garante que o método seja reachable e que as `DynamicDependency` sejam respeitadas pelo linker AOT. |

---

## Erro 9 — Jwt:Secret não encontrada no container de runtime (HTTP 500 no /login)

| Campo | Valor |
|---|---|
| **Número** | 9 |
| **Data** | 2026-03-18 |
| **Comando executado** | `curl -s -X POST http://localhost:8080/login ...` |
| **Erro retornado** | `HTTP 500` + exceção: `System.InvalidOperationException: Jwt:Secret is not configured.` |
| **Causa** | O `Dockerfile` copiava apenas o binário nativo: `COPY --from=build /app/publish/ClaudeDotNetPlayground .`. O arquivo `appsettings.json` (que contém `Jwt:Secret`, `OpenMeteo:BaseAddress`, etc.) estava na pasta `/app/publish/` do estágio de build mas não era copiado para o estágio de runtime. |
| **Novo comando / solução** | Alterar o `Dockerfile` para copiar todo o diretório de publicação: `COPY --from=build /app/publish/ .` (barra no final inclui todos os arquivos do diretório, incluindo `appsettings.json`). |

---

## Erro 10 — UntrustedRoot SSL no container de runtime ao chamar Open-Meteo (HTTP 500 no /weather-conditions)

| Campo | Valor |
|---|---|
| **Número** | 10 |
| **Data** | 2026-03-18 |
| **Comando executado** | `curl -s http://localhost:8080/weather-conditions -H "Authorization: Bearer ..."` |
| **Erro retornado** | `HTTP 500` + exceção: `System.Security.Authentication.AuthenticationException: The remote certificate is invalid because of errors in the certificate chain: UntrustedRoot` |
| **Causa** | O estágio `build` do Dockerfile instala a CA customizada do proxy (via `EXTRA_CA_CERT`) usando `update-ca-certificates`. O estágio `runtime` (`mcr.microsoft.com/dotnet/runtime-deps:10.0`) é uma imagem separada que não herda as CAs instaladas no estágio de build. Ao fazer chamadas HTTPS para `api.open-meteo.com` via proxy com inspeção TLS, o runtime não confiava na CA do proxy. |
| **Novo comando / solução** | Adicionar ao estágio `runtime` do `Dockerfile`: `COPY --from=build /etc/ssl/certs/ca-certificates.crt /etc/ssl/certs/ca-certificates.crt`. Isso copia o bundle de certificados completo (incluindo a CA customizada instalada no build) para o container de runtime, sem necessitar de acesso à internet ou `apt-get` no estágio de runtime. |

---

## Resultado Final (Sessão 2026-03-17 — Infrastructure)

Após a aplicação de todas as correções (Erros 1–7):

- `GET http://localhost:8080/health` → `200 Healthy`
- Datadog Agent: `API key ending with 5a286: API Key valid`
- `Dropped: 0`, `Retried: 0`
- `Series Flushed: 466+`
- Todos os checks do agente: `[OK]`

---

## Resultado Final (Sessão 2026-03-18 — AOT Publish Validation)

Após a aplicação das correções dos Erros 8–10:

- `GET http://localhost:8080/health` → `200 Degraded` (Datadog 403 é comportamento esperado neste ambiente)
- `POST http://localhost:8080/login` (válido) → `200` com JWT
- `POST http://localhost:8080/login` (inválido) → `401`
- `GET http://localhost:8080/test` (com auth) → `200 "funcionando"`
- `GET http://localhost:8080/test` (sem auth) → `401`
- `GET http://localhost:8080/weather-conditions` (com auth) → `200` com payload Open-Meteo
- `GET http://localhost:8080/weather-conditions` (sem auth) → `401`
- 54/54 testes passando em modo debug
- Aplicação em modo Native AOT totalmente funcional

---

## Erro 11 — gh CLI não encontrado ao tentar criar PR

| Campo | Valor |
|---|---|
| **Número** | 11 |
| **Data** | 2026-03-18 |
| **Comando executado** | `gh pr create --title "feat(mcp): ..." --body "..."` |
| **Erro retornado** | `/bin/bash: line 23: gh: command not found` |
| **Causa** | O GitHub CLI (`gh`) não estava instalado no container. O pipeline de validação pré-commit (passo 10) depende de `gh` para criar PRs automaticamente, mas essa dependência não estava listada nos pré-requisitos de ambiente (`scripts/container-setup.md`, `scripts/setup-env.sh`). |
| **Novo comando / solução** | Instalar `gh` via repositório oficial: `curl -fsSL https://cli.github.com/packages/githubcli-archive-keyring.gpg \| dd of=/usr/share/keyrings/githubcli-archive-keyring.gpg && echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages stable main" \| tee /etc/apt/sources.list.d/github-cli.list && apt-get update && apt-get install gh`. Scripts de ambiente (`setup-env.sh`, `container-setup.md`) atualizados para incluir `gh` como dependência. |

---

## Erro 12 — gh pr create falha por remote apontar para proxy local

| Campo | Valor |
|---|---|
| **Número** | 12 |
| **Data** | 2026-03-18 |
| **Comando executado** | `gh pr create --repo AlbertKellner/ClaudeDotNetPlayground --title "..." --body "..."` |
| **Erro retornado** | `could not resolve remote "origin": none of the git remotes configured for this repository point to a known GitHub host. To tell gh about a new GitHub host, please use 'gh auth login'` |
| **Causa** | O remote `origin` aponta para um proxy local (`http://127.0.0.1:40021/git/...`), não para `github.com`. O `gh pr create` precisa resolver o remote para um host GitHub conhecido. |
| **Novo comando / solução** | Usar `gh api` diretamente (que sempre fala com `api.github.com`) em vez de `gh pr create`: `gh api repos/AlbertKellner/ClaudeDotNetPlayground/pulls -f title="..." -f head="..." -f base="main" -f body="..."`. Autenticação via variável de ambiente `GH_TOKEN` já disponível. |

---

## Erro 13 — weather-conditions retorna 500 no CI (job Publish) — ArgumentNullException em OpenMeteo BaseUrl

| Campo | Valor |
|---|---|
| **Número** | 13 |
| **Data** | 2026-03-19 |
| **Comando executado** | `curl -H "Authorization: Bearer $TOKEN" http://localhost:5000/weather-conditions` (no job `healthcheck-publish` do CI) |
| **Erro retornado** | `HTTP 500` — `System.ArgumentNullException: Value cannot be null. (Parameter 'uriString')` na linha `c.BaseAddress = new Uri(builder.Configuration["ExternalApi:OpenMeteo:BaseUrl"]!)` |
| **Causa** | Dois problemas combinados: (1) O binário publicado era executado com `./app/Albert.Playground.ECS.AOT.Api` mas o CWD era a raiz do workspace do GitHub Actions, não `./app/`. O .NET busca `appsettings.json` no CWD (`Directory.GetCurrentDirectory()`), logo o arquivo não era encontrado. (2) A env var `OpenMeteo__BaseAddress` passada no CI mapeia para `OpenMeteo:BaseAddress`, mas o código lê `ExternalApi:OpenMeteo:BaseUrl` — chaves completamente diferentes. Nenhuma das duas fontes fornecia o valor correto. |
| **Novo comando / solução** | Adicionar `cd ./app` antes de executar o binário nos jobs `run` e `healthcheck-publish` do CI, garantindo que `appsettings.json` (presente no artefato de publish) seja encontrado pelo .NET. Removida a env var `OpenMeteo__BaseAddress` (incorreta e desnecessária com o appsettings.json disponível). |

---

## Erro 14 — SSL PartialChain no primeiro request HTTPS dentro do container Docker

| Campo | Valor |
|---|---|
| **Número** | 14 |
| **Data** | 2026-03-19 |
| **Comando executado** | `curl -H "Authorization: Bearer $TOKEN" http://localhost:8080/weather-conditions` (via docker compose) |
| **Erro retornado** | Primeiro request à Open-Meteo falha com `PartialChain` — `System.Security.Authentication.AuthenticationException: The remote certificate is invalid because of errors in the certificate chain: PartialChain`. Retry do Polly (Attempt 1) retorna HTTP 200. |
| **Causa** | O Dockerfile copiava o bundle `ca-certificates.crt` do build stage para o runtime stage, incluindo a CA do proxy. Porém os hash symlinks em `/etc/ssl/certs/` não incluíam a CA do proxy (hash `1a2ed160`). O OpenSSL no .NET usa hash symlinks para lookup de CA durante verificação de cadeia TLS. Sem o symlink, a primeira conexão falhava com `PartialChain`; no retry, caching interno de sessão SSL permitia sucesso. |
| **Novo comando / solução** | Alterar o Dockerfile para: (1) no build stage, criar arquivo `proxy-ca.crt` vazio quando `EXTRA_CA_CERT` não é fornecido (`touch`), garantindo que `COPY --from=build` não falhe; (2) no runtime stage, copiar o cert individual do proxy para `/etc/ssl/certs/` e executar `c_rehash /etc/ssl/certs/` para criar os hash symlinks. Resultado: hash symlink `1a2ed160.0 -> proxy-ca.crt` criado, Attempt 0 retorna HTTP 200 sem retry. |

---

## Resultado Final (Sessão 2026-03-19 — SSL PartialChain Fix)

Após a aplicação da correção do Erro 14:

- `GET http://localhost:8080/health` → `200 Degraded` (Datadog 403 é comportamento esperado neste ambiente)
- `POST http://localhost:8080/login` → `200` com JWT
- `GET http://localhost:8080/weather-conditions` → `200` com payload Open-Meteo, **Attempt 0, sem retry**
- Hash symlink `1a2ed160.0 -> proxy-ca.crt` presente no container
- 94/94 testes passando em modo debug

---

## Referências

- `docker-compose.yml` — arquivo principal afetado pelas correções
- `src/Albert.Playground.ECS.AOT.Api/Dockerfile` — modificado para suporte a CA customizada e hash symlinks
- `assumptions-log.md` — premissas de ambiente registradas
