# Variáveis de Ambiente e Secrets — Registro de Configuração Externa

Este arquivo é o registro canônico de todas as variáveis de ambiente e secrets que devem ser cadastradas na ferramenta externa de configuração de container antes de iniciar uma sessão de trabalho neste repositório.

O script `scripts/setup-env.sh` assume que essas entradas já existem no ambiente. Se alguma estiver ausente, o script emite `[ERR]` ou `[WARN]` explícito indicando qual entrada está faltando e como obtê-la.

---

## Secrets

| Nome | Obrigatório | Como Obter | Impacto se Ausente |
|---|---|---|---|
| `DD_API_KEY` | **Sim** | Datadog → Organization Settings → API Keys | Datadog Agent não autentica. `/health` retorna `Unhealthy`. Build e run da aplicação funcionam, mas sem observabilidade. |
| `DD_APP_KEY` | **Sim** | Datadog → Organization Settings → Application Keys | Conexão MCP do Datadog não autentica. O servidor MCP fica inacessível para o Claude Code. |

---

## Variáveis de Ambiente

| Nome | Obrigatório | Valor Esperado | Impacto se Ausente |
|---|---|---|---|
| `EXTRA_CA_CERT` | Condicional¹ | `base64 -w 0 /usr/local/share/ca-certificates/swp-ca-production.crt` | Build Docker falha com `UntrustedRoot` durante `dotnet restore`. |
| `HTTP_PROXY` | Condicional¹ | `http://21.0.0.183:15004` (valor do ambiente) | Containers de build não acessam internet. `apt-get` e `dotnet restore` falham. |
| `HTTPS_PROXY` | Condicional¹ | Igual ao `HTTP_PROXY` neste ambiente | Idem acima para tráfego HTTPS. |
| `NO_PROXY` | Condicional¹ | `localhost,127.0.0.1` | Conexões locais podem ser roteadas incorretamente pelo proxy. |

> ¹ **Condicional**: obrigatório em ambientes com proxy de inspeção TLS (como este sandbox Claude Code). Em ambientes sem proxy intermediário, essas variáveis não são necessárias.

---

## Como Verificar se Estão Disponíveis

Após aplicar a ferramenta externa de configuração, execute no container:

```bash
echo "DD_API_KEY=${DD_API_KEY:-(AUSENTE — cadastrar na ferramenta externa)}"
echo "HTTP_PROXY=${HTTP_PROXY:-(não definido)}"
echo "EXTRA_CA_CERT=${EXTRA_CA_CERT:+(presente)}"
```

Ou execute `scripts/setup-env.sh` — ele valida todas as entradas e emite erros explícitos para o que estiver faltando.

---

## Referências

- `scripts/setup-env.sh` — script que valida estas variáveis ao ser executado
- `scripts/container-setup.md` — dependências de sistema do container (separado das variáveis)
- `.claude/rules/environment-readiness.md` — protocolo do agente quando o ambiente não está pronto
- `bash-errors-log.md` — histórico de falhas causadas por ausência dessas configurações
