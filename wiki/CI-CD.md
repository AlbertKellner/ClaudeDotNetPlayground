# CI/CD

A aplicação possui um pipeline de GitHub Actions de integração contínua (`ci.yml`).

---

## Pipeline de Integração Contínua

**Arquivo:** `.github/workflows/ci.yml`
**Nome:** Validar Execução

**Gatilhos:**
- Push para as branches `main` ou `master`
- Pull requests abertos, sincronizados ou reabertos
- Execução manual via `workflow_dispatch`

Todos os jobs utilizam o GitHub Environment **`ClaudeCode`**, onde o secret `DD_API_KEY` é armazenado para integração com o Datadog.

O pipeline é composto por **cinco jobs**: quatro sequenciais seguidos de dois paralelos.

```
Compilação → Execução → unit-tests → ┬─ Validar Health Check (Debug)
                                      └─ Validar Health Check (Publish)
```

### Job 1: Compilação (`build`)

Compila a aplicação com **Native AOT** para a plataforma `linux-x64`.

**Passos:**
1. Checkout do repositório
2. Instalação das dependências de Native AOT (`clang`, `zlib1g-dev`)
3. Configuração do .NET 10
4. Restore de dependências
5. Publicação com `dotnet publish -c Release -r linux-x64 --self-contained`
6. Upload de toda a pasta de publicação como artefato (`published-app`), excluindo arquivos `.dbg`

### Job 2: Execução (`run`)

Verifica se a aplicação inicia corretamente a partir do binário compilado (modo AOT).

**Depende de:** Compilação

**Passos:**
1. Download do artefato `published-app`
2. Inicialização do Datadog Agent container (`DD_ENV=ci`) se `DD_API_KEY` estiver disponível
3. Execução do binário em background
4. Verificação de startup via polling em `GET /health` (até 30 tentativas a cada 2 segundos)
5. Falha se a aplicação não responder dentro de 60 segundos
6. Encerramento do Datadog Agent ao final

### Job 3: `unit-tests`

Executa os testes unitários do projeto.

**Depende de:** Execução

**Passos:**
1. Checkout do repositório
2. Configuração do .NET 10
3. Restore de dependências do projeto de testes
4. Execução dos testes com `dotnet test --verbosity normal`

### Job 4: Validar Health Check — Debug (`healthcheck-debug`)

Valida os endpoints principais com a aplicação rodando em **modo Debug** (`dotnet run`).

**Depende de:** unit-tests
**Roda em paralelo com:** `healthcheck-publish`

**Passos:**
1. Checkout do repositório
2. Configuração do .NET 10 e restore de dependências
3. Inicialização do Datadog Agent container (`DD_ENV=ci`) se `DD_API_KEY` estiver disponível
4. Inicialização via `dotnet run` e aguarda readiness via `GET /health`
5. Chamada explícita a `GET /health` e verificação do HTTP status (esperado: 200)
6. Obtenção de Bearer Token via `POST /login` (credenciais: Albert/albert123)
7. Chamada a `GET /weather-conditions` com o token obtido e verificação do HTTP status (esperado: 200)
8. Encerramento da aplicação e do Datadog Agent ao final (mesmo em caso de falha)

### Job 5: Validar Health Check — Publish (`healthcheck-publish`)

Valida os endpoints principais com a aplicação rodando como **binário Native AOT** (artefato publicado).

**Depende de:** unit-tests
**Roda em paralelo com:** `healthcheck-debug`

**Passos:**
1. Download do artefato `published-app`
2. Inicialização do Datadog Agent container (`DD_ENV=ci`) se `DD_API_KEY` estiver disponível
3. Inicialização do binário `Albert.Playground.ECS.AOT.Api` e aguarda readiness via `GET /health`
4. Chamada explícita a `GET /health` e verificação do HTTP status (esperado: 200)
5. Obtenção de Bearer Token via `POST /login` (credenciais: Albert/albert123)
6. Chamada a `GET /weather-conditions` com o token obtido e verificação do HTTP status (esperado: 200)
7. Encerramento da aplicação e do Datadog Agent ao final (mesmo em caso de falha)

---

## Pipeline de Publicação da Wiki

**Arquivo:** `.github/workflows/wiki-publish.yml`
**Nome:** Publicar Wiki

**Gatilhos:**
- Push para `main` ou `master` com alterações em `wiki/**`
- Execução manual via `workflow_dispatch`

Copia automaticamente os arquivos `.md` da pasta `wiki/` para o repositório GitHub Wiki.

---

## Datadog e GitHub Environment

Todos os jobs declaram `environment: ClaudeCode`. O secret `DD_API_KEY` deve ser cadastrado neste environment no GitHub (Settings → Environments → ClaudeCode → Secrets).

Se o secret não estiver disponível (ex: PR de fork), o Datadog Agent é simplesmente ignorado e o job continua normalmente.

---

## Relação com o Projeto

- Os jobs `Validar Health Check (Debug)` e `Validar Health Check (Publish)` validam o endpoint documentado em [Feature: Health Check](Feature-Health)
- O build Native AOT é descrito em [Configuração do Projeto](Project-Setup)
- A integração Docker e Datadog é descrita em [Arquitetura](Architecture)
