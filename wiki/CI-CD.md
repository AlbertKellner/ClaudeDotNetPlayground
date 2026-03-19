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

O job utiliza o GitHub Environment **`ClaudeCode`**, onde o secret `DD_API_KEY` é armazenado para integração com o Datadog.

O pipeline é composto por **um único job** com steps sequenciais. Cada step só executa se o anterior for concluído com sucesso.

```
Compilação (Native AOT) → Testes Unitários → Execução (Release) → Validação de Endpoints
```

### Step 1: Compilação (Native AOT publish)

Compila a aplicação com **Native AOT** para a plataforma `linux-x64`.

**Passos:**
1. Checkout do repositório
2. Instalação das dependências de Native AOT (`clang`, `zlib1g-dev`)
3. Configuração do .NET 10
4. Restore de dependências (API e projeto de testes)
5. Publicação com `dotnet publish -c Release -r linux-x64 --self-contained`

### Step 2: Testes Unitários

Executa os testes unitários do projeto.

**Passos:**
1. Execução dos testes com `dotnet test --verbosity normal`

Os testes utilizam o código-fonte já disponível no mesmo runner, sem necessidade de checkout ou restore adicionais.

### Step 3: Execução (Release)

Inicia a aplicação a partir do binário Native AOT compilado no Step 1 e verifica startup.

**Passos:**
1. Inicialização do Datadog Agent container (`DD_ENV=ci`) se `DD_API_KEY` estiver disponível
2. Execução do binário Native AOT em background a partir da pasta de publicação
3. Verificação de startup via polling em `GET /health` (até 30 tentativas a cada 2 segundos)
4. Falha se a aplicação não responder dentro de 60 segundos

### Step 4: Validação de Endpoints

Valida os endpoints principais com a aplicação em execução (binário Release do Step 3).

**Passos:**
1. Chamada explícita a `GET /health` e verificação do HTTP status (esperado: 200)
2. Obtenção de Bearer Token via `POST /login` (credenciais: Albert/albert123)
3. Chamada a `GET /weather-conditions` com o token obtido e verificação do HTTP status (esperado: 200)
4. Encerramento da aplicação e do Datadog Agent ao final (mesmo em caso de falha)

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

O job declara `environment: ClaudeCode`. O secret `DD_API_KEY` deve ser cadastrado neste environment no GitHub (Settings → Environments → ClaudeCode → Secrets).

Se o secret não estiver disponível (ex: PR de fork), o Datadog Agent é simplesmente ignorado e o job continua normalmente.

---

## Relação com o Projeto

- O step de validação de endpoints valida o endpoint documentado em [Feature: Health Check](Feature-Health)
- O build Native AOT é descrito em [Configuração do Projeto](Project-Setup)
- A integração Docker e Datadog é descrita em [Arquitetura](Architecture)
