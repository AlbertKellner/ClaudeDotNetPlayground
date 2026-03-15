# CI/CD

A aplicação possui dois pipelines de GitHub Actions: um pipeline de integração contínua (`ci.yml`) e um de validação de pull requests (`pr-language-check.yml`).

---

## Pipeline de Integração Contínua

**Arquivo:** `.github/workflows/ci.yml`

**Gatilhos:**
- Push para as branches `main` ou `master`
- Pull requests abertos, sincronizados ou reabertos
- Execução manual via `workflow_dispatch`

O pipeline é composto por **três jobs encadeados**, onde cada job depende do anterior:

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
2. Execução do binário em background
3. Verificação de startup via polling em `GET /health` (até 30 tentativas a cada 2 segundos)
4. Falha se a aplicação não responder dentro de 60 segundos

### Job 3: `healthcheck`

Valida que o endpoint de saúde responde corretamente.

**Passos:**
1. Download do artefato `published-app`
2. Inicialização da aplicação e aguarda readiness via `GET /health`
3. Chamada explícita a `GET /health` e verificação do HTTP status
4. Sucesso se `HTTP 200`; falha caso contrário
5. Encerramento da aplicação ao final (mesmo em caso de falha)

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

## Relação com o Projeto

- O job `healthcheck` valida o endpoint documentado em [Feature: Health Check](Feature-Health)
- O build Native AOT é descrito em [Configuração do Projeto](Project-Setup)
