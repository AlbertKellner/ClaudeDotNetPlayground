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

O pipeline é composto por **quatro jobs** encadeados sequencialmente:

```
Compilação → Execução → unit-tests → Validar Health Check
```

### Job 1: Compilação (`build`)

Compila a aplicação com **Native AOT** para a plataforma `linux-x64`.

**Passos:**
1. Checkout do repositório
2. Instalação das dependências de Native AOT (`clang`, `zlib1g-dev`)
3. Configuração do .NET 10
4. Restore de dependências
5. Publicação com `dotnet publish -c Release -r linux-x64 --self-contained`
6. Upload do binário publicado como artefato (`published-app`)

### Job 2: Execução (`run`)

Verifica se a aplicação inicia corretamente a partir do binário compilado.

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

### Job 4: Validar Health Check (`healthcheck`)

Valida que os endpoints principais da aplicação respondem corretamente.

**Depende de:** unit-tests

**Passos:**
1. Download do artefato `published-app`
2. Inicialização do Datadog Agent container (`DD_ENV=ci`) se `DD_API_KEY` estiver disponível
3. Inicialização da aplicação e aguarda readiness via `GET /health`
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

- O job `Validar Health Check` valida o endpoint documentado em [Feature: Health Check](Feature-Health)
- O build Native AOT é descrito em [Configuração do Projeto](Project-Setup)
- A integração Docker e Datadog é descrita em [Arquitetura](Architecture)
