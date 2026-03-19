# Sistema de Governança para Claude Code

## Árvore de Arquivos

```
.claude/
  hooks/         (6 scripts + settings.json)
  rules/         (12 rules operacionais)
  skills/        (6 skills com SKILL.md cada)
CLAUDE.md
Instructions/
  architecture/  (6 arquivos)
  bdd/           (3 arquivos + example.feature)
  business/      (5 arquivos)
  contracts/     (openapi.yaml, asyncapi.yaml, schema, exemplos)
  decisions/     (README + template ADR)
  glossary/      (ubiquitous-language.md)
  operating-model.md
  snippets/      (README + canonical-snippets.md)
README.md
assumptions-log.md
open-questions.md
```

---

## Propósito Operacional por Grupo

| Grupo | Propósito |
|---|---|
| `CLAUDE.md` | Ponto de entrada: pipeline obrigatório de execução e imports de governança |
| `.claude/rules/` | **Comportamento operacional persistente do assistente** — 12 rules que definem como interpretar, classificar, priorizar, propagar e implementar |
| `.claude/skills/` | 6 skills que mapeiam tipos de solicitação a workflows completos de execução |
| `.claude/hooks/` | Scripts placeholder de enforcement — prontos para especialização quando a stack for definida |
| `Instructions/operating-model.md` | Documento central: explica como tudo funciona junto em runtime |
| `Instructions/architecture/` | Memória arquitetural: princípios, padrões, decisões, nomenclatura, estrutura |
| `Instructions/business/` | Memória de negócio: regras, invariantes, fluxos, modelo de domínio, premissas |
| `Instructions/bdd/` | Especificação por comportamento: convenções e exemplo Gherkin agnóstico |
| `Instructions/contracts/` | Contratos formais: OpenAPI, AsyncAPI, JSON Schema, exemplos válido/inválido |
| `Instructions/glossary/` | Linguagem ubíqua: termos canônicos, sinônimos permitidos, termos proibidos |
| `Instructions/decisions/` | ADRs: template e README com política de quando e como registrar decisões |
| `Instructions/snippets/` | Snippets canônicos: registro de trechos normativos para preservação literal futura |
| `open-questions.md` | Dúvidas e ambiguidades abertas — apenas itens ainda pendentes |
| `assumptions-log.md` | Premissas ativas — apenas itens ainda válidos (3 premissas de bootstrap registradas) |

---

## O Que os Arquivos Gerados Contêm

- **Pipeline obrigatório** de validação pré-commit com 10 passos (`CLAUDE.md`)
- **Políticas consolidadas** de normalização, contexto, propagação, ambiguidade e snippets (`governance-policies.md`)
- **Hierarquia de prioridade** entre fontes de verdade com regras de conflito (`source-of-truth-priority.md`)
- **Meta-governança** com checklist de revisão obrigatória para instruções (`REVIEW.md`, `instruction-review.md`)
- **7 skills operacionais** com workflows internos completos para cada tipo de solicitação
- **Estrutura durável** em todos os arquivos de `Instructions/` — prontos para receber conteúdo específico do domínio

---

## Configuração Manual

### 1. GitHub Personal Access Token (PAT)

A feature de busca de repositórios requer um **Personal Access Token (classic)** do GitHub com as seguintes permissões:

- **Scope obrigatório**: `read:org` — permite listar repositórios de teams dentro da organização WebMotors

**Como gerar o token**:
1. Acesse [GitHub Settings > Developer settings > Personal access tokens > Tokens (classic)](https://github.com/settings/tokens)
2. Clique em "Generate new token (classic)"
3. Selecione o scope `read:org`
4. Gere o token e copie o valor

**Onde configurar o token**:

O token deve ser informado na configuração `ExternalApi:GitHub:HttpRequest:Token`. Existem duas formas:

- **Via `appsettings.json`** (desenvolvimento local):
  ```json
  {
    "ExternalApi": {
      "GitHub": {
        "HttpRequest": {
          "Token": "ghp_seu_token_aqui"
        }
      }
    }
  }
  ```

- **Via variável de ambiente** (recomendado para CI/produção):
  ```
  ExternalApi__GitHub__HttpRequest__Token=ghp_seu_token_aqui
  ```

### 2. Permissões no GitHub

O token deve pertencer a um usuário que tenha **acesso ao team IntegrationRepos** dentro da organização **WebMotors**. Sem essa permissão, a API retornará `404 Not Found`.

### 3. Git

A feature de sincronização de repositórios executa comandos `git clone` e `git pull` via `System.Diagnostics.Process`. É obrigatório que o binário `git` esteja instalado e acessível no `PATH` da máquina onde a aplicação será executada.

### 4. Caminhos configuráveis

| Configuração | Descrição | Valor padrão |
|---|---|---|
| `Repositories:JsonFilePath` | Caminho do arquivo JSON com a lista de repositórios | `data/repositories.json` |
| `Repositories:SyncRootPath` | Pasta raiz onde os repositórios serão clonados | `c:/usuarios/albert/git` |

Ambos podem ser sobrescritos via variáveis de ambiente:
```
Repositories__JsonFilePath=caminho/repositorios.json
Repositories__SyncRootPath=/home/user/repos
```

### 5. Autenticação JWT

Todos os endpoints da aplicação (exceto `/login` e `/health`) exigem autenticação via Bearer Token JWT.

Para obter o token:
```bash
curl -X POST http://localhost:8080/login \
  -H "Content-Type: application/json" \
  -d '{"userName": "usuario", "password": "senha"}'
```

Para consumir os endpoints protegidos:
```bash
curl http://localhost:8080/repositories \
  -H "Authorization: Bearer <token>"

curl -X POST http://localhost:8080/repositories/sync \
  -H "Authorization: Bearer <token>"
```

---

## Começando

A partir de agora, qualquer mensagem em linguagem natural é entrada operacional suficiente.
