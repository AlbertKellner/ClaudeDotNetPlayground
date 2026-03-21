# Regra: Governança de Metadados de Pull Request

## Propósito

Esta rule define as políticas de criação, atualização e governança de Pull Requests. Os workflows procedurais de criação/atualização de PR e acompanhamento de GitHub Actions estão em `.claude/skills/manage-pr-lifecycle/SKILL.md`.

---

## Princípio Fundamental

> Pull requests são a interface pública do trabalho realizado.
> Título e descrição devem ser sempre consistentes com o estado real do código.
> Um PR desatualizado gera confusão e dificulta revisão e governança.

---

## Obrigações do Assistente

### Quando criar um novo PR:
1. Definir título claro, objetivo e tecnicamente descritivo (máximo ~70 caracteres)
2. Preencher a descrição com as três seções obrigatórias (ver template)
3. Adicionar as labels correspondentes ao tipo de alteração

### Quando adicionar novos commits a um PR existente:
1. Revisar o título — atualizar se a nova mudança alterar o escopo ou foco
2. Revisar cada seção da descrição e incorporar o que foi adicionado
3. Remover qualquer referência a alterações descartadas

### Quando descartar alterações de um PR:
1. Remover da descrição qualquer referência ao que foi descartado
2. Verificar se o título ainda é preciso após a remoção
3. Atualizar labels se o escopo da mudança se alterou

---

## Formato Obrigatório do Título

O título deve ser um **Semantic Commit** no formato:

```
<tipo>(<escopo>): <descrição imperativa em português>
```

**Tipos aceitos:**
- `feat` — nova funcionalidade
- `fix` — correção de bug
- `docs` — documentação
- `refactor` — refatoração sem mudança de comportamento
- `test` — testes
- `chore` — manutenção, dependências, configuração
- `ci` — CI/CD e workflows

---

## Estrutura Obrigatória da Descrição

A descrição deve sempre seguir as três seções do template `.github/pull_request_template.md`:

### Seção 1 — Motivos da alteração
### Seção 2 — Plano de execução
### Seção 3 — O que foi realizado

**Esta seção deve estar sempre atualizada.** Toda vez que um novo commit for adicionado, ela deve ser revisada e atualizada.

---

## Labels Obrigatórias

### Labels de tipo:
| Label | Quando usar |
|---|---|
| `feature` | Nova funcionalidade implementada |
| `bugfix` | Correção de bug ou comportamento incorreto |
| `documentation` | Criação ou atualização de documentação |
| `refactoring` | Refatoração sem mudança de comportamento |
| `governance` | Atualização de regras, governança ou processo |
| `infrastructure` | Docker, CI/CD, scripts de ambiente |
| `testing` | Adição ou correção de testes |

### Labels de impacto:
| Label | Quando usar |
|---|---|
| `breaking-change` | Altera interface pública, contrato ou comportamento esperado |
| `non-breaking` | Mudança compatível com versão atual |

---

## Política de Verificação e Criação Automática de PR

> Todo trabalho commitado deve ter um PR associado. A verificação e criação de PR é a última etapa obrigatória da codificação de qualquer tarefa.

**Exceção — Análise de PR (skill pr-analysis)**: o PR já existe e não deve ser criado um novo.

O workflow de criação/atualização está em `.claude/skills/manage-pr-lifecycle/SKILL.md`.

---

## Política de Acompanhamento de GitHub Actions

> Código pushado não está validado até que o CI confirme. O acompanhamento das Actions é a etapa final obrigatória do pipeline e **condição de encerramento da tarefa**.

**Não se aplica a tarefas exclusivamente de governança** (sem código, sem build, sem Docker).

O workflow de acompanhamento está em `.claude/skills/manage-pr-lifecycle/SKILL.md`.

---

## Política de Merge

### Método obrigatório: Merge Commit

Todo merge deve utilizar **merge commit** (`merge_method: "merge"`).

**Proibido**: Squash merge e Rebase merge.

---

## Política de Merge e Fechamento — Restrição Absoluta

> O assistente **nunca** deve executar merge nem fechar Pull Requests, a menos que o usuário solicite explicitamente na mensagem atual.

- Merge proibido sem solicitação explícita
- Fechamento proibido sem solicitação explícita
- Esta restrição prevalece sobre qualquer outra condição
- Solicitação explícita significa: o usuário escrever na mensagem atual uma instrução clara como "faça o merge", "realize o merge", "feche o PR"

---

## Política de Branch durante Revisão de PR

> Solicitações de mudança em um PR são resolvidas no próprio branch de origem do PR. Criar um branch novo para tratar comentários de revisão é proibido.

- O branch atribuído pelo sistema externo é ignorado em pr-analysis
- O único branch válido é o `head.ref` do PR sendo analisado
- O passo 10 do pipeline pré-commit não se aplica durante análise de PR
- **Enforcement**: o hook `.claude/hooks/branch-guard.sh` detecta automaticamente operações de branch incorretas durante pr-analysis, usando o contexto armazenado em `.claude/.pr-analysis-context`

---

## Política de Consistência

- A descrição é a **fonte de verdade textual** do PR
- Discrepância entre descrição e commits deve ser corrigida automaticamente

---

## Relação com Outras Rules

- Skill `implement-request` — o PR é criado ao final do workflow de implementação
- `governance-policies.md` §3 — quando o escopo de mudança se expande, o PR deve ser atualizado
- `bash-error-logging.md` — erros encontrados durante validação devem ser refletidos no PR quando relevantes
- `endpoint-validation.md` — resultados de validação de endpoints devem constar na seção "O que foi realizado"

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-18 | Criado: governança de metadados de PR | Instrução do usuário |
| 2026-03-18 | Adicionado: política de verificação e criação automática de PR após último commit | Instrução do usuário |
| 2026-03-19 | Adicionado: política de acompanhamento de GitHub Actions pós-PR com análise de logs e correção de falhas | Instrução do usuário |
| 2026-03-19 | Reforço: acompanhamento de Actions e verificação de logs Datadog tornados condição de encerramento da tarefa | Falha observada em sessão |
| 2026-03-20 | Adicionado: Política de Merge — merge commit obrigatório; squash e rebase proibidos | Instrução do usuário |
| 2026-03-20 | Adicionado: Política de Branch durante Revisão de PR | Instrução do usuário |
| 2026-03-20 | Adicionado: Política de Merge e Fechamento — Restrição Absoluta | Instrução explícita do usuário |
| 2026-03-21 | Refatorado: workflows procedurais extraídos para skill manage-pr-lifecycle; rule simplificada para conter apenas políticas | Auditoria de governança |
| 2026-03-21 | Adicionado: referência explícita ao hook branch-guard.sh na Política de Branch | Análise estrutural de governança |
