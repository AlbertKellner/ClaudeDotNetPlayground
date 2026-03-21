# Configuração do Container — Guia para Ferramenta Externa

Este arquivo documenta tudo que deve estar configurado no container criado pela ferramenta externa de configuração antes que o agente de codificação inicie uma sessão de trabalho neste repositório.

Para variáveis de ambiente e secrets, ver `scripts/required-vars.md`.

---

## Dependências de Sistema

Estas dependências devem estar pré-instaladas no container. O script `scripts/setup-env.sh` verifica sua presença e emite avisos se estiverem ausentes.

| Dependência | Versão | Como Instalar |
|---|---|---|
| Docker Engine | qualquer estável | `apt-get install docker.io` ou script oficial do Docker |
| .NET SDK | **10.x** | Via `scripts/setup-dotnet.sh` ou devcontainer feature `ghcr.io/devcontainers/features/dotnet` |
| CA do proxy TLS | — | Copiar certificado para `/usr/local/share/ca-certificates/` e executar `update-ca-certificates` |
| `base64` | qualquer | Pré-instalado na maioria das imagens (coreutils) |
| `curl` | qualquer | `apt-get install curl` |

---

## Certificado CA do Proxy TLS

Este ambiente usa proxy com inspeção TLS. O certificado CA do proxy deve ser instalado no sistema operacional do container para que `dotnet restore`, `apt-get` e outras ferramentas confiem no proxy.

**Caminho esperado no container:**
```
/usr/local/share/ca-certificates/swp-ca-production.crt
```

**Como instalar:**
```bash
# Copiar o certificado (obtido do ambiente de origem)
cp swp-ca-production.crt /usr/local/share/ca-certificates/
update-ca-certificates
```

**Verificação:**
```bash
ls /usr/local/share/ca-certificates/swp-ca-production.crt
```

Se o certificado não estiver presente, o script `setup-env.sh` emitirá `[WARN]` e o build Docker falhará ao tentar executar `dotnet restore` (erro: `UntrustedRoot`).

---

## Docker Daemon

O Docker daemon não inicia automaticamente em containers sem `systemd`. A ferramenta externa deve garantir que o Docker daemon esteja disponível, ou que o container tenha privilégios suficientes para iniciá-lo.

**Configuração recomendada na ferramenta externa:**
- Montar o socket Docker do host: `/var/run/docker.sock:/var/run/docker.sock`
- Ou conceder `--privileged` ao container para permitir inicialização do daemon internamente

**Verificação:**
```bash
docker info
```

---

## Proxy HTTP

O proxy HTTP do ambiente deve estar disponível como variável de ambiente no container (ver `scripts/required-vars.md`). Adicionalmente, o script `setup-env.sh` configura o proxy em `~/.docker/config.json` para que containers de build herdem a configuração.

**Verificação:**
```bash
echo $HTTP_PROXY
cat ~/.docker/config.json
```

---

## PATH e Runtime

O .NET SDK deve estar acessível no `PATH` do container. Após instalação via `scripts/setup-dotnet.sh`, adicionar ao `PATH`:

```bash
export DOTNET_ROOT="${HOME}/.dotnet"
export PATH="${PATH}:${DOTNET_ROOT}:${DOTNET_ROOT}/tools"
```

Incluir essas exportações no perfil do shell do container (`.bashrc`, `.profile` ou equivalente da ferramenta externa) para que estejam disponíveis em todas as sessões.

---

## Portas e Serviços

Referência rápida de portas utilizadas pelo projeto. Para detalhes completos (URLs, endpoints, credenciais), ver `scripts/operational-runbook.md`.

| Serviço | Porta | Contexto |
|---|---|---|
| Aplicação (Docker) | `8080` | `docker compose up` — acesso via `http://localhost:8080` |
| Aplicação (Debug) | `5000` | `dotnet run` — acesso via `http://localhost:5000` |
| Datadog Agent (traces) | `8126` | Rede interna Docker; referenciado em `Program.cs` |

---

## Checklist de Verificação Final

Execute após configurar o container na ferramenta externa:

```bash
# 1. .NET SDK 10
dotnet --list-sdks | grep '^10\.'

# 2. Docker daemon
docker info >/dev/null 2>&1 && echo "OK" || echo "FALHOU"

# 3. CA do proxy
ls /usr/local/share/ca-certificates/swp-ca-production.crt

# 4. Variáveis obrigatórias (ver required-vars.md)
echo "DD_API_KEY=${DD_API_KEY:-(AUSENTE)}"
echo "DD_APP_KEY=${DD_APP_KEY:-(AUSENTE — necessário para MCP Datadog)}"
echo "GH_TOKEN_MCP=${GH_TOKEN_MCP:-(AUSENTE — necessário para MCP GitHub)}"
echo "HTTP_PROXY=${HTTP_PROXY:-(não definido)}"
```

---

## Verificação Pós-Setup

Após o checklist acima passar, execute esta sequência para confirmar que o ambiente funciona de ponta a ponta:

```bash
# 1. Build — deve compilar sem erros
dotnet build src/Albert.Playground.ECS.AOT.Api/Albert.Playground.ECS.AOT.Api.csproj

# 2. Execução debug — deve iniciar e responder
dotnet run --project src/Albert.Playground.ECS.AOT.Api/Albert.Playground.ECS.AOT.Api.csproj &
sleep 5
curl -sf http://localhost:5000/health && echo " → OK" || echo " → FALHOU"
kill %1 2>/dev/null

# 3. Docker — deve subir containers e responder
docker compose up -d --build
sleep 10
curl -sf http://localhost:8080/health && echo " → OK" || echo " → FALHOU"
docker compose down
```

Se todos os passos retornarem OK, o ambiente está pronto para desenvolvimento.

> **Erro comum**: `dotnet: command not found` — o .NET SDK está instalado em `/root/.dotnet` mas não está no `PATH`. Adicionar `export PATH="${PATH}:/root/.dotnet"` ao perfil do shell. Ver seção "PATH e Runtime" acima.

---

## Referências

- `scripts/operational-runbook.md` — ponto de entrada unificado: portas, URLs, comandos, troubleshooting
- `scripts/required-vars.md` — variáveis de ambiente e secrets a cadastrar (arquivo separado)
- `scripts/setup-env.sh` — script de bootstrap que valida este checklist automaticamente
- `.claude/rules/environment-readiness.md` — protocolo do agente quando o ambiente não está pronto
- `bash-errors-log.md` — histórico de falhas de ambiente e soluções adotadas

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-19 | Estrutura inicial criada | Bootstrap de governança |
| 2026-03-19 | Adicionado: seção Portas e Serviços, Verificação Pós-Setup com sequência de validação ponta a ponta | Instrução do usuário |
| 2026-03-21 | Migração: `gh` CLI removido das dependências (GitHub agora via MCP); checklist atualizado de GH_TOKEN para GH_TOKEN_MCP | Migração API → MCP |
