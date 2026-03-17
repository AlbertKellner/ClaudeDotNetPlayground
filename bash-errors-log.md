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

## Resultado Final

Após a aplicação de todas as correções:

- `GET http://localhost:8080/health` → `200 Healthy`
- Datadog Agent: `API key ending with 5a286: API Key valid`
- `Dropped: 0`, `Retried: 0`
- `Series Flushed: 466+`
- Todos os checks do agente: `[OK]`

---

## Referências

- `docker-compose.yml` — arquivo principal afetado pelas correções
- `src/ClaudeDotNetPlayground/Dockerfile` — modificado para suporte a CA customizada
- `assumptions-log.md` — premissas de ambiente registradas
