# CI/CD

A aplicação possui dois pipelines de GitHub Actions: um pipeline de integração contínua (`ci.yml`) e um de validação de pull requests (`pr-language-check.yml`).

---

## Pipeline de Integração Contínua

**Arquivo:** `.github/workflows/ci.yml`

**Gatilhos:**
- Push para as branches `main` ou `master`
- Pull requests abertos, sincronizados ou reabertos
- Execução manual via `workflow_dispatch`

Todos os jobs utilizam o GitHub Environment **`ClaudeCode`**, onde o secret `DD_API_KEY` é armazenado para integração com o Datadog.

O pipeline é composto por **cinco jobs** encadeados: `build` → `run` → `healthcheck` → `validate-weather`, mais `docker-build` que executa em paralelo de forma independente:

### Job 1: `build`

Compila a aplicação com **Native AOT** para a plataforma `linux-x64`.

**Passos:**
1. Checkout do repositório
2. Instalação das dependências de Native AOT (`clang`, `zlib1g-dev`)
3. Configuração do .NET 10
4. Restore de dependências
5. Publicação com `dotnet publish -c Release -r linux-x64 --self-contained`
6. Upload do binário publicado como artefato (`published-app`)

### Job 2: `run`

Verifica se a aplicação inicia corretamente a partir do binário compilado.

**Passos:**
1. Download do artefato `published-app`
2. Inicialização do Datadog Agent container (`DD_ENV=ci`) se `DD_API_KEY` estiver disponível
3. Execução do binário em background
4. Verificação de startup via polling em `GET /health` (até 30 tentativas a cada 2 segundos)
5. Falha se a aplicação não responder dentro de 60 segundos
6. Encerramento do Datadog Agent ao final

### Job 3: `healthcheck`

Valida que o endpoint de saúde responde corretamente. Depende de `run`.

**Passos:**
1. Download do artefato `published-app`
2. Inicialização do Datadog Agent container (`DD_ENV=ci`) se `DD_API_KEY` estiver disponível
3. Inicialização da aplicação e aguarda readiness via `GET /health`
4. Chamada explícita a `GET /health` e verificação do HTTP status
5. Sucesso se `HTTP 200`; falha caso contrário
6. Encerramento da aplicação e do Datadog Agent ao final (mesmo em caso de falha)

### Job 4: `validate-weather`

Valida o endpoint autenticado de condições climáticas. **Depende de `healthcheck`** — só executa se o healthcheck retornar sucesso.

**Passos:**
1. Download do artefato `published-app`
2. Inicialização do Datadog Agent container (`DD_ENV=ci`) se `DD_API_KEY` estiver disponível
3. Inicialização da aplicação e aguarda readiness via `GET /health`
4. **Gerar token JWT** — `POST /login` com as credenciais do usuário registrado; extrai o `token` da resposta e exporta como `$JWT_TOKEN`
5. **Validar endpoint de tempo** — `GET /weather-conditions` com `Authorization: Bearer $JWT_TOKEN`; verifica `HTTP 200` e exibe o payload retornado
6. Encerramento da aplicação e do Datadog Agent ao final (mesmo em caso de falha)

### Job 5: `docker-build`

Valida que o Dockerfile compila e que a imagem Docker sobe corretamente. Executa em paralelo com `build`.

**Passos:**
1. Checkout do repositório
2. Build da imagem Docker com `docker build`
3. Execução do container em modo de teste na porta `8080`
4. Verificação de startup via polling em `GET /health` (até 30 tentativas a cada 2 segundos)
5. Remoção do container de teste ao final

---

## Pipeline de Validação de Pull Requests

**Arquivo:** `.github/workflows/pr-language-check.yml`

**Gatilhos:**
- Pull requests abertos, editados, sincronizados ou reabertos

**Validações realizadas:**
- O **título** do PR deve ter no mínimo 5 caracteres
- O **corpo (descrição)** do PR deve ter no mínimo 20 caracteres

Se qualquer uma das validações falhar, o job é marcado como falho e o PR não pode ser mesclado sem correção.

---

## Template de Pull Request

**Arquivo:** `.github/pull_request_template.md`

Todo PR criado no repositório utiliza automaticamente o template padrão, que inclui:

- **O que foi alterado** — descrição objetiva das mudanças
- **Motivação** — por que a mudança é necessária
- **Como testar** — passos para verificar as mudanças localmente
- **Checklist:**
  - Build limpo (`dotnet build` sem erros)
  - HealthCheck passando (`/health` retorna `Healthy`)
  - Código escrito em inglês
  - Governança atualizada antes da implementação (quando aplicável)
  - Título e descrição do PR em português brasileiro

---

---

## Datadog e GitHub Environment

Todos os jobs declaram `environment: ClaudeCode`. O secret `DD_API_KEY` deve ser cadastrado neste environment no GitHub (Settings → Environments → ClaudeCode → Secrets).

Se o secret não estiver disponível (ex: PR de fork), o Datadog Agent é simplesmente ignorado e o job continua normalmente.

---

## Relação com o Projeto

- O job `healthcheck` valida o endpoint documentado em [Feature: Health Check](Feature-Health)
- O workflow `validate-weather` valida o endpoint documentado em [Feature: Condições Climáticas](Feature-WeatherConditionsGet)
- O build Native AOT é descrito em [Configuração do Projeto](Project-Setup)
- A integração Docker e Datadog é descrita em [Arquitetura](Architecture)
