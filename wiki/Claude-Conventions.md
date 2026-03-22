# Claude — Convenções e Restrições

## Descrição

Documenta as convenções, restrições e comportamentos esperados que governam como o Claude opera neste repositório. Deve ser consultada para entender o que o assistente deve sempre fazer ou nunca fazer.

## Contexto

O comportamento do assistente é governado por regras persistentes registradas em `CLAUDE.md`, `.claude/rules/` e `.claude/skills/`. Estas convenções são obrigatórias e aplicadas automaticamente — o usuário não precisa repeti-las a cada interação.

---

## Comportamentos Obrigatórios

Os seguintes comportamentos são executados automaticamente pelo assistente em toda interação, conforme definido em `CLAUDE.md`:

1. **Interpretar antes de agir** — toda mensagem deve ser interpretada semanticamente antes de qualquer ação; normalizar a intenção do usuário
2. **Ler a governança relevante antes de implementar** — consultar os arquivos de governança pertinentes antes de qualquer implementação
3. **Verificar ambiguidades antes de implementar** — se houver dúvida material, registrar antes de agir
4. **Classificar trechos técnicos enviados pelo usuário** — normativo, ilustrativo, preferencial ou contextual
5. **Atualizar a governança primeiro** — se a mensagem introduzir definição durável, atualizar governança antes do código
6. **Seguir a prioridade entre fontes de verdade** — contratos > BDD > regras de negócio > arquitetura > convenções
7. **Usar o contexto acumulado do repositório** — preferir governança acumulada a suposições genéricas
8. **Não depender de repetição de instruções** — comportamentos escritos na governança são executados automaticamente
9. **Avaliar eficiência em toda tarefa** — reutilizar artefatos, antecipar falhas, eliminar redundâncias
10. **Proteção de branch em análise de PR** — usar exclusivamente o `head.ref` do PR, nunca criar branch novo
11. **Rastrear comportamentos esperados durante toda a sessão** — coletar, apresentar e verificar ao final

---

## Linguagem e Comunicação

| Contexto | Idioma |
|---|---|
| Código (classes, métodos, variáveis, arquivos, pastas, contratos, comentários técnicos) | Sempre em **inglês** |
| Respostas ao usuário | Sempre em **português** |
| Pull Requests (título, descrição, comentários e corpo) | Sempre em **português brasileiro** |
| Resumo de mudanças | Incluído em toda resposta de tarefa, em **português**, com justificativa técnica |

---

## Restrições

| Restrição | Descrição |
|---|---|
| Merge e fechamento de PR | **Nunca** executar sem solicitação explícita do usuário na mensagem atual |
| Push para branches incorretos | **Nunca** fazer push para branch diferente do atribuído (exceto em pr-analysis, onde o `head.ref` prevalece) |
| Pipeline pré-commit | **Nunca** pular — build, testes, Docker e validação de endpoints são obrigatórios antes de qualquer commit |
| Governança antes de implementação | Toda definição durável deve ser persistida na governança **antes** de qualquer mudança de código |
| Comportamento de negócio | **Sempre** prevalece sobre preferências arquiteturais quando houver conflito |
| Snippets normativos | **Nunca** reescrever silenciosamente — alteração exige instrução explícita do usuário |

---

## Classificação de Escopo

A classificação de escopo é o primeiro ato obrigatório antes de iniciar qualquer tarefa:

| Escopo | Critério | Passos Aplicáveis | Passos Não Aplicáveis |
|---|---|---|---|
| **Código** | Altera `.cs`, `.csproj`, `Dockerfile`, `docker-compose.yml`, `appsettings.json`, workflows de CI | Todos: 0 a 11 | Nenhum — todos obrigatórios |
| **Governança** | Altera exclusivamente `.md`, `.sh`, scripts, hooks ou documentação | Apenas: 0.1, 9, 10 | Passos 0, 1-8 e 11 |
| **Análise de PR** | Análise de solicitações de mudança em PR existente | Conforme skill pr-analysis | Passo 10 (criação de PR) |

Executar passos inaplicáveis ao escopo é um erro. Omitir passos aplicáveis ao escopo também é um erro.

---

## Pipeline de Validação Pré-Commit — Passos 0 a 11

O pipeline é a sequência obrigatória de validação antes de qualquer commit. Cada passo tem aplicabilidade definida pelo escopo da tarefa.

### Passo 0 — Verificar pré-requisitos de ambiente

| Campo | Valor |
|---|---|
| **O que faz** | Verifica se o ambiente está pronto: .NET SDK, Docker, Docker Compose, clang, variáveis de ambiente, Docker daemon em execução |
| **Gate** | Sim — ambiente não pronto bloqueia todos os passos seguintes |
| **Escopo Código** | Obrigatório |
| **Escopo Governança** | Não aplicável |
| **Escopo Análise de PR** | Conforme skill pr-analysis |
| **Skill** | `verify-environment` |

### Passo 0.1 — Auditoria automatizada de governança

| Campo | Valor |
|---|---|
| **O que faz** | Executa `bash scripts/governance-audit.sh` — 36 verificações automatizadas de consistência estrutural dos arquivos de governança |
| **Gate** | Sim — falhas bloqueiam o commit. Se houver falhas, executar `--fix` para correções automáticas e re-executar |
| **Escopo Código** | Obrigatório |
| **Escopo Governança** | Obrigatório — gate principal antes do commit |
| **Escopo Análise de PR** | Conforme skill pr-analysis |

### Passo 1 — Build (`dotnet build`)

| Campo | Valor |
|---|---|
| **O que faz** | Compila o projeto em modo Debug; verifica que não há erros de compilação |
| **Gate** | Sim — erro de compilação bloqueia o avanço |
| **Escopo Código** | Obrigatório |
| **Escopo Governança** | Não aplicável |
| **Escopo Análise de PR** | Conforme skill pr-analysis |

### Passo 2 — Execução em modo debug (`dotnet run`)

| Campo | Valor |
|---|---|
| **O que faz** | Inicia a aplicação localmente na porta 5000; aguarda `/health` responder (qualquer código HTTP confirma inicialização); encerra o processo |
| **Gate** | Sim — aplicação que não inicia bloqueia o avanço |
| **Escopo Código** | Obrigatório — primeira validação antes dos testes |
| **Escopo Governança** | Não aplicável |
| **Escopo Análise de PR** | Conforme skill pr-analysis |

### Passo 3 — Testes unitários (`dotnet test`)

| Campo | Valor |
|---|---|
| **O que faz** | Executa todos os testes unitários em modo Debug |
| **Gate** | **Sim — gate obrigatório**: falha em qualquer teste bloqueia os passos 4 a 8. Testes falhando bloqueiam o commit |
| **Escopo Código** | Obrigatório |
| **Escopo Governança** | Não aplicável |
| **Escopo Análise de PR** | Conforme skill pr-analysis |

### Passo 4 — Publicar e iniciar via Docker (`docker compose up -d`)

| Campo | Valor |
|---|---|
| **O que faz** | Publica em modo Release/Native AOT e inicia a aplicação + Datadog Agent via Docker Compose na porta 8080 |
| **Gate** | Sim — falha de build Docker ou startup bloqueia o avanço |
| **Escopo Código** | Obrigatório — executado **somente** após aprovação no gate de testes (passo 3) |
| **Escopo Governança** | Não aplicável |
| **Escopo Análise de PR** | Conforme skill pr-analysis |
| **Pré-requisito** | Docker daemon em execução; `DD_API_KEY` disponível (sem ela, prossegue sem Datadog) |

### Passo 5 — Health Check pós-Docker

| Campo | Valor |
|---|---|
| **O que faz** | Aguarda `/health` na porta 8080 responder HTTP 200 (polling até 30 tentativas) |
| **Gate** | Sim — aplicação que não responde HTTP 200 bloqueia o avanço |
| **Escopo Código** | Obrigatório |
| **Escopo Governança** | Não aplicável |
| **Escopo Análise de PR** | Conforme skill pr-analysis |

### Passo 6 — Validação de endpoints via HTTP real

| Campo | Valor |
|---|---|
| **O que faz** | Para cada endpoint criado ou alterado na tarefa: obtém Bearer Token via `POST /login` (se necessário), consome o endpoint e valida o status code. Inclui validação de cache (segunda requisição consecutiva) e captura de logs de storytelling (SNP-001) |
| **Gate** | Sim — status code inesperado bloqueia o commit |
| **Escopo Código** | Obrigatório quando a tarefa criou ou alterou features com endpoint |
| **Escopo Governança** | Não aplicável |
| **Escopo Análise de PR** | Conforme skill pr-analysis |
| **Skill** | `validate-endpoints` |

### Passo 7 — Exibir logs do container

| Campo | Valor |
|---|---|
| **O que faz** | Exibe os logs de storytelling de cada requisição validada no passo 6. Se o passo 6 não foi aplicável, exibe os logs gerais do container via `docker logs` |
| **Gate** | Não — informativo |
| **Escopo Código** | Obrigatório |
| **Escopo Governança** | Não aplicável |
| **Escopo Análise de PR** | Conforme skill pr-analysis |

### Passo 8 — Parar containers (`docker compose down`)

| Campo | Valor |
|---|---|
| **O que faz** | Para todos os containers Docker (aplicação + Datadog Agent) |
| **Gate** | Não |
| **Escopo Código** | Obrigatório |
| **Escopo Governança** | Não aplicável |
| **Escopo Análise de PR** | Conforme skill pr-analysis |

### Passo 9 — Commit

| Campo | Valor |
|---|---|
| **O que faz** | Realiza o commit com mensagem descritiva. Somente executado após todos os gates anteriores passarem |
| **Gate** | — |
| **Escopo Código** | Obrigatório |
| **Escopo Governança** | Obrigatório |
| **Escopo Análise de PR** | Obrigatório (no branch do PR) |

### Passo 10 — Criar ou atualizar Pull Request

| Campo | Valor |
|---|---|
| **O que faz** | Verifica se já existe PR aberto para o branch; se não, cria seguindo a governança de PR. Se já existir, atualiza título e descrição para refletir o estado atual |
| **Gate** | — |
| **Escopo Código** | Obrigatório |
| **Escopo Governança** | Obrigatório (quando houver commits a integrar) |
| **Escopo Análise de PR** | **Não aplicável** — o PR já existe. Em vez disso, atualizar título e descrição do PR existente via MCP `update_pull_request` |
| **Skill** | `manage-pr-lifecycle` |
| **Exceção** | Em análise de PR, o branch atribuído pelo sistema externo é ignorado; usar exclusivamente o `head.ref` do PR |

### Passo 11 — Checkpoint de encerramento (CI + Datadog)

| Campo | Valor |
|---|---|
| **O que faz** | Acompanha a execução das GitHub Actions até término de todos os jobs; verifica logs no Datadog; procura falhas ou erros; se tudo passar, reporta e encerra. Se falhar, diagnostica, corrige e reinicia o ciclo |
| **Gate** | Sim — a tarefa **não se encerra** até que todos os jobs do CI passem e os logs no Datadog sejam verificados |
| **Escopo Código** | Obrigatório — condição de encerramento da tarefa |
| **Escopo Governança** | Não aplicável (sem build, sem Docker, sem CI de código) |
| **Escopo Análise de PR** | Conforme skill pr-analysis |
| **Skill** | `manage-pr-lifecycle` |

---

## Notas sobre o Pipeline

- **Passo 0 previne falhas em cascata** — verificar ambiente antes de executar evita erros custosos documentados em `bash-errors-log.md`
- **Passo 3 é gate obrigatório** — o Docker publish (passo 4) só executa após todos os testes passarem
- **Passo 11 é condição de encerramento** — a tarefa não está concluída enquanto houver jobs em execução ou logs não verificados
- **`scripts/setup-env.sh` é modelo declarativo** — o agente não executa esse script; o ambiente deve chegar pronto
- **`DD_API_KEY` ausente** — o pipeline prossegue sem Datadog; logs aparecerão quando o CI executar com a chave

---

## Auditoria Automatizada

O script `governance-audit.sh` executa **36 verificações automatizadas** da consistência estrutural dos arquivos de governança:

### Verificações Bloqueantes

Falhas bloqueiam o commit. Incluem:
- Imports faltantes ou quebrados no `CLAUDE.md`
- Contagens inconsistentes de rules e skills
- Referências a artefatos removidos em contexto ativo
- Features sem página correspondente na Wiki
- Páginas estruturais obrigatórias ausentes na Wiki
- Rules com workflows procedurais extensos (separação rules/skills)
- Referências cruzadas para arquivos inexistentes
- Subpastas de `Infra/` e `Shared/ExternalApi/` não documentadas
- Hooks configurados mas inexistentes ou com sintaxe inválida
- Skills sem estrutura mínima obrigatória

### Verificações Não-Bloqueantes

Emitem avisos sem bloquear o commit. Incluem:
- Páginas wiki órfãs (sem feature correspondente)
- Endpoints no runbook sem rota correspondente nos Controllers
- Regras de negócio sem cenários BDD
- Contratos OpenAPI como placeholders
- Skills sem referência a rules
- Conceitos de rules ausentes no glossário

### Modo de Auto-Correção

```bash
bash scripts/governance-audit.sh --fix
```

Corrige automaticamente problemas triviais: imports faltantes, contagens incorretas, stubs de páginas wiki. Toda operação de escrita é precedida por backup (`safe_fix`).

---

## Referências

- [Claude — Visão Geral](Claude-Overview)
- [Claude — Skills](Claude-Skills)
- [Claude — Hooks](Claude-Hooks)
