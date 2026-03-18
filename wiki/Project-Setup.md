# Configuração do Projeto

## Pré-requisitos

- **.NET 10 SDK** — necessário para build e execução
- **clang** e **zlib1g-dev** — necessários para compilação Native AOT

Para instalar as dependências de Native AOT no Ubuntu/Debian:

```bash
sudo apt-get install -y clang zlib1g-dev
```

---

## Configuração

A aplicação requer as seguintes configurações em `appsettings.json`:

### `Jwt:Secret` (obrigatório)

Chave de assinatura utilizada para gerar e validar os tokens JWT. Deve ser alterada em produção.

```json
{
  "Jwt": {
    "Secret": "sua-chave-secreta-aqui"
  }
}
```

> O arquivo `appsettings.json` inclui um valor padrão para desenvolvimento. **Altere este valor antes de qualquer uso em ambiente não local.**

### `Serilog:MinimumLevel` (opcional)

Controla o nível mínimo de log emitido pela aplicação.

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning",
        "System": "Warning"
      }
    }
  }
}
```

---

## Build

### Build padrão

```bash
dotnet build src/Albert.Playground.ECS.AOT.Api/Albert.Playground.ECS.AOT.Api.csproj
```

### Publicação com Native AOT (modo de produção)

```bash
dotnet publish src/Albert.Playground.ECS.AOT.Api/Albert.Playground.ECS.AOT.Api.csproj \
  -c Release \
  -r linux-x64 \
  --self-contained
```

O binário publicado será gerado em:
```
src/Albert.Playground.ECS.AOT.Api/bin/Release/net10.0/linux-x64/publish/Albert.Playground.ECS.AOT.Api
```

---

## Execução

### Via .NET CLI

```bash
dotnet run --project src/Albert.Playground.ECS.AOT.Api/Albert.Playground.ECS.AOT.Api.csproj
```

### Via binário publicado (Native AOT)

```bash
ASPNETCORE_URLS=http://localhost:5000 ./Albert.Playground.ECS.AOT.Api
```

---

## Verificação

Após iniciar a aplicação, verifique se está funcionando corretamente chamando o endpoint de saúde:

```bash
curl http://localhost:5000/health
```

Resposta esperada: `HTTP 200` com corpo `Healthy`.

Consulte [Health Check](Feature-Health) para mais detalhes sobre este endpoint.

---

## Execução via Docker Compose

Para executar a aplicação junto com o Datadog Agent localmente:

### Pré-requisitos

- **Docker** e **Docker Compose**
- API key do Datadog

### Configuração

1. Copie o template de variáveis de ambiente:

```bash
cp .env.example .env
```

2. Preencha `DD_API_KEY` no arquivo `.env`:

```
DD_API_KEY=sua-api-key-aqui
DD_SITE=datadoghq.com
DD_ENV=local
```

### Inicialização

```bash
docker compose up
```

A aplicação ficará disponível em `http://localhost:8080`.

```bash
curl http://localhost:8080/health
```

> O arquivo `.env` é ignorado pelo git (`.gitignore`). Nunca comite a API key no repositório.

---

## Funcionalidades disponíveis após inicialização

| Endpoint | Descrição |
|----------|-----------|
| `GET /health` | [Verificação de disponibilidade](Feature-Health) |
| `POST /login` | [Autenticação de usuário](Feature-UserLogin) |
| `GET /test` | [Endpoint protegido](Feature-TestGet) |
| `GET /weather-conditions` | [Condições climáticas de São Paulo](Feature-WeatherConditionsGet) |
