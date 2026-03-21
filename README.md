# Sistema de Governança para Claude Code

## Árvore de Arquivos

```
.claude/
  hooks/         (3 scripts + settings.json)
  rules/         (13 rules operacionais)
  skills/        (12 skills com SKILL.md cada)
CLAUDE.md
Instructions/
  architecture/  (6 arquivos)
  bdd/           (2 arquivos)
  business/      (5 arquivos)
  contracts/     (README + placeholders)
  decisions/     (README + template ADR)
  glossary/      (ubiquitous-language.md)
  operating-model.md
  snippets/      (README + canonical-snippets.md)
  wiki/          (wiki-governance.md)
README.md
assumptions-log.md
open-questions.md
```

---

## Propósito Operacional por Grupo

| Grupo | Propósito |
|---|---|
| `CLAUDE.md` | Ponto de entrada: pipeline obrigatório de execução e imports de governança |
| `.claude/rules/` | **Comportamento operacional persistente do assistente** — 13 rules que definem como interpretar, classificar, priorizar, propagar, auditar e implementar |
| `.claude/skills/` | 12 skills que mapeiam tipos de solicitação e passos de pipeline a workflows completos de execução |
| `.claude/hooks/` | Scripts de enforcement automatizado — detecção de mudanças em governança, proteção de branch e auditoria |
| `Instructions/operating-model.md` | Documento central: explica como tudo funciona junto em runtime |
| `Instructions/architecture/` | Memória arquitetural: princípios, padrões, decisões, nomenclatura, estrutura |
| `Instructions/business/` | Memória de negócio: regras, invariantes, fluxos, modelo de domínio, premissas |
| `Instructions/bdd/` | Especificação por comportamento: convenções Gherkin |
| `Instructions/contracts/` | Contratos formais: OpenAPI, AsyncAPI, JSON Schema, exemplos válido/inválido |
| `Instructions/glossary/` | Linguagem ubíqua: termos canônicos, sinônimos permitidos, termos proibidos |
| `Instructions/decisions/` | ADRs: template e README com política de quando e como registrar decisões |
| `Instructions/snippets/` | Snippets canônicos: registro de trechos normativos para preservação literal futura |
| `open-questions.md` | Dúvidas e ambiguidades abertas — apenas itens ainda pendentes |
| `assumptions-log.md` | Premissas ativas — apenas itens ainda válidos |
| `scripts/governance-audit.sh` | Auditoria automatizada de consistência estrutural da governança |

---

## O Que os Arquivos Gerados Contêm

- **Pipeline obrigatório** de validação pré-commit com 12 passos incluindo auditoria automatizada (`CLAUDE.md`)
- **Políticas consolidadas** de normalização, contexto, propagação, ambiguidade e snippets (`governance-policies.md`)
- **Hierarquia de prioridade** entre fontes de verdade com regras de conflito (`source-of-truth-priority.md`)
- **Meta-governança** com checklist de revisão obrigatória para instruções (`REVIEW.md`, `instruction-review.md`)
- **Auditoria automatizada** de consistência estrutural via script (`governance-audit.md`, `governance-audit.sh`)
- **12 skills operacionais** com workflows internos completos para cada tipo de solicitação
- **Estrutura durável** em todos os arquivos de `Instructions/` — prontos para receber conteúdo específico do domínio

---

## Configuração Manual

### 1. GitHub Personal Access Token (PAT)

A feature de pesquisa de repositórios GitHub (`GET /github-repo-search`) requer um **Personal Access Token** do GitHub.

**Onde configurar o token**:

- **Via `appsettings.json`** (desenvolvimento local):
  ```json
  {
    "ExternalApi": {
      "GitHub": {
        "HttpRequest": {
          "PersonalAccessToken": "ghp_seu_token_aqui"
        }
      }
    }
  }
  ```

- **Via variável de ambiente** (recomendado para CI/Docker):
  ```
  GITHUB_PAT=ghp_seu_token_aqui
  ```
  (Mapeada no `docker-compose.yml` para `ExternalApi__GitHub__HttpRequest__PersonalAccessToken`)

### 2. Autenticação JWT

Todos os endpoints da aplicação (exceto `/login` e `/health`) exigem autenticação via Bearer Token JWT.

Para obter o token:
```bash
curl -X POST http://localhost:8080/login \
  -H "Content-Type: application/json" \
  -d '{"userName": "Albert", "password": "albert123"}'
```

Para consumir os endpoints protegidos:
```bash
curl http://localhost:8080/github-repo-search \
  -H "Authorization: Bearer <token>"
```

---

## Começando

A partir de agora, qualquer mensagem em linguagem natural é entrada operacional suficiente.
