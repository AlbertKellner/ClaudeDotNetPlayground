# Variáveis de Ambiente e Secrets — Registro de Configuração Externa

Este arquivo é o registro canônico de todas as variáveis de ambiente e secrets que devem ser cadastradas na ferramenta externa de configuração de container antes de iniciar uma sessão de trabalho neste repositório.

O script `scripts/setup-env.sh` assume que essas entradas já existem no ambiente. Se alguma estiver ausente, o script emite `[ERR]` ou `[WARN]` explícito indicando qual entrada está faltando e como obtê-la.

---

## Secrets

| Nome | Obrigatório | Como Obter | Impacto se Ausente |
|---|---|---|---|
| `DD_API_KEY` | **Sim** | Datadog → Organization Settings → API Keys | Datadog Agent não autentica. `/health` retorna `Unhealthy`. Build e run da aplicação funcionam, mas sem observabilidade. |
| `DD_APP_KEY` | **Sim** | Datadog → Organization Settings → Application Keys | Conexão MCP do Datadog não autentica. O servidor MCP fica inacessível para o Claude Code. |
| `GH_TOKEN_MCP` | **Sim** | Gerado na conta GitHub do usuário ClaudeCode-Bot → Settings → Developer Settings → Personal Access Tokens (Fine-grained) | Servidor MCP do GitHub fica inacessível. Assistente não consegue criar, atualizar ou consultar Pull Requests via MCP. Pipeline pré-commit (passo 10) falha. |
| `GITHUB_PAT` | Condicional² | GitHub → Settings → Developer Settings → Personal Access Tokens | Feature `/github-repo-search` retorna HTTP 500 ao consultar API do GitHub. Mapeada no `docker-compose.yml` para `ExternalApi__GitHub__HttpRequest__PersonalAccessToken`. |

---

## Variáveis de Ambiente

| Nome | Obrigatório | Valor Esperado | Impacto se Ausente |
|---|---|---|---|
| `EXTRA_CA_CERT` | Condicional¹ | `base64 -w 0 /usr/local/share/ca-certificates/swp-ca-production.crt` | Build Docker falha com `UntrustedRoot` durante `dotnet restore`. |
| `HTTP_PROXY` | Condicional¹ | `http://21.0.0.183:15004` (valor do ambiente) | Containers de build não acessam internet. `apt-get` e `dotnet restore` falham. |
| `HTTPS_PROXY` | Condicional¹ | Igual ao `HTTP_PROXY` neste ambiente | Idem acima para tráfego HTTPS. |
| `NO_PROXY` | Condicional¹ | `localhost,127.0.0.1` | Conexões locais podem ser roteadas incorretamente pelo proxy. |

> ¹ **Condicional**: obrigatório em ambientes com proxy de inspeção TLS (como este sandbox Claude Code). Em ambientes sem proxy intermediário, essas variáveis não são necessárias.
> ² **Condicional**: obrigatório quando a feature `/github-repo-search` será usada. `GH_TOKEN_MCP` é para o servidor MCP do GitHub (criar/atualizar PRs, monitorar Actions), `GITHUB_PAT` é para a aplicação .NET consultar a API GitHub via Refit. São tokens de usuários diferentes: `GH_TOKEN_MCP` pertence ao usuário ClaudeCode-Bot; `GITHUB_PAT` pertence ao desenvolvedor.

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

## Ciclo de Vida de Credenciais

### DD_API_KEY (Datadog API Key)

| Campo | Valor |
|---|---|
| **Validade** | Não expira automaticamente. Permanece válida até ser revogada manualmente no painel do Datadog. |
| **Como obter** | Datadog → Organization Settings → API Keys → copiar chave existente ou criar nova. Se não for admin da organização, solicitar ao administrador. |
| **Sintoma quando ausente** | Pipeline Docker prossegue sem Datadog Agent. `GET /health` retorna `Unhealthy`. Logs não fluem ao Datadog. |
| **Sintoma quando inválida** | Datadog Agent reporta: `Unexpected response code from the API Key validation endpoint`. DogStatsD mostra `TLSErrors`. |
| **Como renovar** | Revogar a chave antiga no painel do Datadog e criar nova. Atualizar o valor na ferramenta externa de configuração de container. |
| **Quem pode fornecer** | Administrador da organização no Datadog. |

### DD_APP_KEY (Datadog Application Key)

| Campo | Valor |
|---|---|
| **Validade** | Não expira automaticamente. Permanece válida até ser revogada manualmente. |
| **Como obter** | Datadog → Organization Settings → Application Keys → criar chave com nome descritivo (ex: `claude-code-mcp`). |
| **Sintoma quando ausente** | Servidor MCP do Datadog fica inacessível. Ferramentas MCP retornam erro de autenticação. |
| **Sintoma quando inválida** | MCP retorna HTTP 403 do Datadog. |
| **Como renovar** | Revogar a chave antiga e criar nova no painel do Datadog. Atualizar na ferramenta externa. |
| **Quem pode fornecer** | Qualquer membro da organização no Datadog (Application Keys são pessoais). |

### GH_TOKEN_MCP (GitHub Personal Access Token do ClaudeCode-Bot para MCP)

| Campo | Valor |
|---|---|
| **Usuário GitHub** | `ClaudeCode-Bot` — conta dedicada de serviço para operações automatizadas do assistente |
| **Validade** | Fine-grained: validade configurável (30, 60, 90 dias ou customizada). Recomendado: 90 dias com renovação periódica. |
| **Como obter** | Fazer login na conta GitHub `ClaudeCode-Bot` → Settings → Developer Settings → Personal Access Tokens → Fine-grained tokens → Generate new token. Repository access: Only select repositories → `AlbertKellner/ClaudeDotNetPlayground`. Permissões: `Contents` (Read and write), `Pull requests` (Read and write), `Actions` (Read-only), `Metadata` (Read-only). |
| **Sintoma quando ausente** | Servidor MCP do GitHub fica inacessível. Ferramentas MCP de GitHub não respondem. Pipeline pré-commit (passo 10) falha. |
| **Sintoma quando inválido/expirado** | MCP retorna HTTP 401 do GitHub. Ferramentas de PR e Actions não funcionam. |
| **Como renovar** | Login na conta `ClaudeCode-Bot` → Settings → Developer Settings → Personal Access Tokens → criar novo token com as mesmas permissões. Atualizar `GH_TOKEN_MCP` nos secrets do Claude Code. |
| **Quem pode fornecer** | O administrador da conta `ClaudeCode-Bot` (proprietário do repositório). |
| **Onde armazenar** | Secrets do Claude Code, variável `GH_TOKEN_MCP`. |

### GITHUB_PAT (GitHub Personal Access Token para a aplicação)

| Campo | Valor |
|---|---|
| **Validade** | Depende da configuração: tokens Classic podem não expirar; tokens Fine-grained têm validade configurável. |
| **Como obter** | GitHub → Settings → Developer Settings → Personal Access Tokens → criar token. Permissões mínimas: `public_repo` (para leitura de repositórios públicos). |
| **Sintoma quando ausente** | `GET /github-repo-search` retorna HTTP 500. Aplicação loga erro de autenticação ao consultar `api.github.com`. |
| **Sintoma quando inválido/expirado** | API GitHub retorna HTTP 401. Aplicação retorna HTTP 500. |
| **Como renovar** | Criar novo token no GitHub. Atualizar `GITHUB_PAT` na ferramenta externa de configuração de container. |
| **Quem pode fornecer** | O próprio desenvolvedor (tokens são pessoais). |

---

## Mapa de Erros por Variável

Esta tabela mapeia cada variável ao erro exato que aparece quando está ausente ou inválida, facilitando o diagnóstico rápido.

| Variável | Erro Quando Ausente | Erro Quando Inválida | Onde o Erro Aparece |
|---|---|---|---|
| `DD_API_KEY` | Pipeline Docker prossegue sem Datadog; `/health` retorna `Unhealthy` | `Unexpected response code from the API Key validation endpoint` | `docker compose up`, `GET /health` |
| `DD_APP_KEY` | MCP Datadog inacessível; ferramentas MCP não respondem | HTTP 403 do servidor MCP | Ferramentas MCP do Claude Code |
| `GH_TOKEN_MCP` | Servidor MCP do GitHub inacessível; ferramentas MCP não respondem | HTTP 401 do servidor MCP do GitHub | Ferramentas MCP do Claude Code (PRs, Actions) |
| `GITHUB_PAT` | `/github-repo-search` retorna HTTP 500 | HTTP 401 da API GitHub | `GET /github-repo-search` via Docker |
| `EXTRA_CA_CERT` | `UntrustedRoot` em `dotnet restore` dentro do Docker build | CA inválida; mesmo erro `UntrustedRoot` | `docker compose build` |
| `HTTP_PROXY` | `Temporary failure resolving 'archive.ubuntu.com'` em `apt-get` | Proxy inacessível; timeout de conexão | `docker compose build`, `apt-get`, `dotnet restore` |

> Para o histórico detalhado de cada erro, ver `bash-errors-log.md`.

---

## Referências

- `scripts/operational-runbook.md` — ponto de entrada unificado: portas, URLs, comandos, troubleshooting
- `scripts/setup-env.sh` — script que valida estas variáveis ao ser executado
- `scripts/container-setup.md` — dependências de sistema do container (separado das variáveis)
- `.claude/rules/environment-readiness.md` — protocolo do agente quando o ambiente não está pronto
- `bash-errors-log.md` — histórico de falhas causadas por ausência dessas configurações

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-19 | Estrutura inicial criada | Bootstrap de governança |
| 2026-03-19 | Adicionado: ciclo de vida de credenciais, mapa de erros por variável, instruções detalhadas para obtenção de secrets | Instrução do usuário |
| 2026-03-21 | Adicionado: GITHUB_PAT documentada como variável condicional para a aplicação .NET consultar API GitHub; diferenciação entre GH_TOKEN (CLI) e GITHUB_PAT (aplicação) | Auditoria de governança |
| 2026-03-21 | Migração: GH_TOKEN substituído por GH_TOKEN_MCP; acesso ao GitHub via MCP (usuário ClaudeCode-Bot) em vez de CLI gh; ciclo de vida atualizado com instruções de token Fine-grained | Migração API → MCP |
