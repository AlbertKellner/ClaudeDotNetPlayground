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
