# Claude — Visão Geral

## Descrição

Explica como o Claude opera neste repositório através de um sistema de governança persistente. Esta página é o ponto de entrada para entender o modelo operacional, o sistema de governança e como o assistente processa tarefas. Deve ser consultada para compreender o comportamento do assistente, o pipeline de validação ou a estrutura de governança.

## Contexto

Este repositório opera com um sistema de governança persistente em linguagem natural. O `CLAUDE.md` é o ponto de entrada. O usuário não precisa reexplicar processo nem contexto acumulado — tudo está registrado nos arquivos de governança.

---

## Sistema de Governança Operacional

O repositório utiliza um sistema de governança persistente que serve como sistema operacional de todas as interações. Os arquivos de governança estão organizados em três locais principais:

| Local | Conteúdo |
|---|---|
| `Instructions/` | Domínio, arquitetura, regras de negócio, padrões, decisões, glossário, BDD, contratos |
| `.claude/rules/` | Políticas operacionais — definem **o quê** deve ser feito ou respeitado |
| `.claude/skills/` | Workflows executáveis — definem **como** executar processos |

O `CLAUDE.md` na raiz do repositório importa todos os arquivos de governança e define os comportamentos obrigatórios e o pipeline de validação pré-commit.

---

## Como Mensagens São Processadas

Toda mensagem do usuário é classificada semanticamente antes de qualquer ação. A classificação determina quais skills são ativadas:

| Tipo de Mensagem | Skill Ativada | Propósito |
|---|---|---|
| Nova definição durável | `ingest-definition` | Persistir na governança |
| Solicitação de implementação | `implement-request` | Criar, alterar ou remover artefatos |
| Revisão de alinhamento | `review-alignment` | Verificar consistência entre artefatos |
| Evolução de governança | `evolve-governance` | Reorganizar ou melhorar a governança |
| Resolução de ambiguidade | `resolve-ambiguity` | Responder dúvidas registradas |
| Trecho técnico | `apply-user-snippet` | Classificar e aplicar código/configuração |
| Alteração de instrução | `review-instructions` | Executar checklist REVIEW.md |
| Análise de PR | `pr-analysis` | Analisar solicitações de mudança em PR aberto |

Uma mensagem pode ativar múltiplos tipos simultaneamente.

---

## Pipeline de Validação Pré-Commit

Antes de qualquer commit, o assistente deve classificar o escopo da tarefa e executar os passos correspondentes.

### Classificação de Escopo

| Escopo | Critério | Passos Aplicáveis |
|---|---|---|
| **Código** | Altera `.cs`, `.csproj`, `Dockerfile`, `docker-compose.yml`, `appsettings.json`, workflows de CI ou qualquer artefato que afete build/execução | Todos: 0 a 11 |
| **Governança** | Altera exclusivamente `.md`, `.sh`, scripts de governança, hooks ou documentação | Apenas: 0.1, 9, 10 |
| **Análise de PR** | Análise de solicitações de mudança em PR existente | Conforme skill pr-analysis |

### Passos do Pipeline (Escopo Código)

| Passo | Descrição |
|---|---|
| 0 | Verificar pré-requisitos de ambiente |
| 0.1 | Executar auditoria automatizada de governança (`governance-audit.sh`) |
| 1 | `dotnet build` — compilação sem erros |
| 2 | `dotnet run` — iniciar em modo debug, verificar `/health` |
| 3 | `dotnet test` — todos os testes devem passar (gate obrigatório) |
| 4 | `docker compose up -d` — publicar Release/Native AOT e iniciar containers |
| 5 | Aguardar `/health` responder HTTP 200 |
| 6 | Validar endpoints via chamada HTTP real |
| 7 | Exibir logs do container da aplicação |
| 8 | `docker compose down` — parar containers |
| 9 | Realizar o commit |
| 10 | Criar ou atualizar Pull Request |
| 11 | Acompanhar GitHub Actions até conclusão |

---

## Fontes de Verdade

Quando houver conflito entre fontes, a seguinte hierarquia de prioridade é respeitada:

1. **Contratos executáveis, artefatos formais e snippets normativos canônicos**
2. **BDD** — cenários comportamentais formalizados
3. **Regras de negócio estruturadas**
4. **Arquitetura e padrões técnicos**
5. **Convenções de nomenclatura, estilo e organização**

Comportamento de negócio sempre prevalece sobre preferência arquitetural quando há conflito.

---

## Referências

- [Claude — Skills](Claude-Skills)
- [Claude — Hooks](Claude-Hooks)
- [Claude — Convenções e Restrições](Claude-Conventions)
