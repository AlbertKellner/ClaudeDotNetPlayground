# Operação

## Descrição

Documenta os pré-requisitos, configuração, build, execução e setup Docker do projeto. Deve ser consultado ao configurar o ambiente de desenvolvimento ou executar a aplicação.

## Contexto

A aplicação pode ser executada de duas formas: em modo debug via `dotnet run` (porta 5000) ou em modo Release/Native AOT via `docker compose` (porta 8080). O modo Docker é o recomendado para validação completa, pois inclui o Datadog Agent para observabilidade. O modo debug é utilizado como validação intermediária local antes da publicação.

---

## Pré-requisitos

| Requisito | Versão | Propósito |
|---|---|---|
| .NET SDK | 10.0 | Build, execução e testes |
| clang | latest | Compilação Native AOT (linker nativo) |
| zlib1g-dev | latest | Dependência de compressão para Native AOT |
| Docker | latest | Containerização da aplicação |
| Docker Compose | latest | Orquestração de containers (app + Datadog Agent) |

---

## Configuração (`appsettings.json`)

### Configurações obrigatórias

| Chave | Descrição | Observação |
|---|---|---|
| `Jwt:Secret` | Chave secreta para geração de JWT HS256 | **Alterar em produção** — valor padrão é apenas para desenvolvimento |

### Configurações de Datadog

| Chave | Descrição | Valor padrão |
|---|---|---|
| `Datadog:AgentUrl` | URL do Datadog Agent para traces | `http://datadog-agent:8126` |
| `Datadog:DirectLogs` | Ativa envio de logs diretamente ao Datadog via HTTP Sink | `false` |

### Configurações de Serilog

| Chave | Descrição | Valor padrão |
|---|---|---|
| `Serilog:MinimumLevel:Default` | Nível mínimo de log padrão | `Information` |
| `Serilog:MinimumLevel:Override:Microsoft` | Nível mínimo para namespaces Microsoft | `Warning` |
| `Serilog:MinimumLevel:Override:Microsoft.AspNetCore` | Nível mínimo para ASP.NET Core | `Warning` |
| `Serilog:MinimumLevel:Override:System` | Nível mínimo para namespaces System | `Warning` |

### Configurações de APIs externas

> **Estado atual**: nenhuma integração com API externa configurada. Configurações serão adicionadas aqui conforme integrações forem implementadas, seguindo o padrão `ExternalApi:<Servico>:HttpRequest/CircuitBreaker/EndpointCache`.

---

## Build

### Modo Debug
```bash
dotnet build src/Starter.Template.AOT.Api/Starter.Template.AOT.Api.csproj
```

### Publicação Native AOT
```bash
dotnet publish src/Starter.Template.AOT.Api/Starter.Template.AOT.Api.csproj -c Release -r linux-x64
```

---

## Execução

### Modo Debug (porta 5000)
```bash
dotnet run --project src/Starter.Template.AOT.Api/Starter.Template.AOT.Api.csproj
```

Verificação:
```bash
curl http://localhost:5000/health
```

### Modo AOT (binário publicado)
```bash
./src/Starter.Template.AOT.Api/bin/Release/net10.0/linux-x64/publish/Starter.Template.AOT.Api
```

---

## Docker Compose

### Pré-requisitos

- Docker e Docker Compose instalados
- `DD_API_KEY` disponível (secret do Datadog)

### Setup

1. Copiar `.env.example` para `.env`:
   ```bash
   cp .env.example .env
   ```
2. Preencher `DD_API_KEY` no arquivo `.env`

> **Nota**: O arquivo `.env` está no `.gitignore` e nunca deve ser commitado.

### Iniciar

```bash
docker compose up -d --build
```

A aplicação estará disponível em `http://localhost:8080`.

### Verificar

```bash
curl http://localhost:8080/health
```

### Parar

```bash
docker compose down
```

---

## Endpoints Disponíveis

| Método | Rota | Autenticação | Descrição |
|---|---|---|---|
| `GET` | `/health` | Não | Verificação de disponibilidade (app + Datadog Agent) |
| `POST` | `/login` | Não | Login com credenciais; retorna JWT Bearer Token |
| `GET` | `/test` | Sim | Endpoint de teste; retorna `"funcionando"` |

---

## Credenciais de Teste

| Item | Valor |
|---|---|
| Usuário | Definido no UseCase de login |
| Senha | Definida no UseCase de login |
| ID do usuário | Definido no UseCase de login |

> **Aviso**: Estas credenciais são hardcoded no código para fins de desenvolvimento. Em produção, devem ser substituídas por mecanismo seguro de autenticação.

### Exemplo de Login

```bash
TOKEN=$(curl -s -X POST http://localhost:8080/login \
  -H "Content-Type: application/json" \
  -d '{"userName":"<usuario>","password":"<senha>"}' \
  | grep -o '"token":"[^"]*"' | cut -d'"' -f4)
```

### Exemplo de Chamada Autenticada

```bash
curl -H "Authorization: Bearer $TOKEN" http://localhost:8080/test
```

---

## Referências

- [Health Check](Feature-Health) — detalhes do endpoint de verificação de disponibilidade
- [CI/CD e Deploy](Governance-CI-CD) — pipelines de integração contínua
- [Observabilidade](Governance-Observability) — Datadog Agent e configuração de métricas
