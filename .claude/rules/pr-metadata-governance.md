# Regra: Governança de Metadados de Pull Request

## Propósito

Esta rule define como o assistente deve criar, manter e atualizar os metadados de pull requests neste repositório. Título, descrição e labels de todo PR devem refletir com precisão o estado atual da implementação.

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

**Exemplos de títulos válidos:**
- `feat(auth): adicionar endpoint de login com JWT Bearer Token`
- `fix(health-check): corrigir retorno Healthy quando Datadog bloqueia IPs privados`
- `docs(wiki): documentar arquitetura e features implementadas`
- `refactor(governance): redefinir setup-env.sh como modelo declarativo`

---

## Estrutura Obrigatória da Descrição

A descrição deve sempre seguir as três seções do template `.github/pull_request_template.md`:

### Seção 1 — Motivos da alteração
- Por que esta mudança é necessária
- Qual problema foi identificado
- Qual regra de negócio é implementada
- Qual comportamento incorreto é corrigido

### Seção 2 — Plano de execução
- Quais etapas foram planejadas
- Qual sequência de implementação foi adotada
- Quais decisões técnicas relevantes foram tomadas

### Seção 3 — O que foi realizado
- Descrição completa e técnica de tudo que foi feito
- Arquivos criados ou modificados (com breve descrição da mudança)
- Mudanças de comportamento observáveis
- Endpoints adicionados ou alterados
- Regras de negócio implementadas
- Qualquer outro detalhe relevante para revisão

**Esta seção deve estar sempre atualizada.** Toda vez que um novo commit for adicionado, ela deve ser revisada e atualizada para refletir o estado atual.

---

## Labels Obrigatórias

Todo PR deve ter pelo menos uma label de tipo e uma de impacto:

### Labels de tipo (escolher a mais representativa):
| Label | Quando usar |
|---|---|
| `feature` | Nova funcionalidade implementada |
| `bugfix` | Correção de bug ou comportamento incorreto |
| `documentation` | Criação ou atualização de documentação |
| `refactoring` | Refatoração sem mudança de comportamento |
| `governance` | Atualização de regras, governança ou processo |
| `infrastructure` | Docker, CI/CD, scripts de ambiente |
| `testing` | Adição ou correção de testes |

### Labels de impacto (escolher uma):
| Label | Quando usar |
|---|---|
| `breaking-change` | Altera interface pública, contrato ou comportamento esperado |
| `non-breaking` | Mudança compatível com versão atual |

### Labels opcionais:
| Label | Quando usar |
|---|---|
| `wip` | Trabalho em andamento, não pronto para merge |
| `needs-review` | Aguardando revisão |

---

## Política de Verificação e Criação Automática de PR

### Princípio:
> Todo trabalho commitado deve ter um PR associado. A verificação e criação de PR é a última etapa obrigatória da codificação de qualquer tarefa.

### Quando se aplica:
Esta política é ativada automaticamente **após a criação do último commit** de qualquer tarefa, como passo 10 do pipeline de validação pré-commit definido em `CLAUDE.md`.

### Workflow obrigatório:

1. **Verificar se já existe PR aberto** para o branch atual:
   ```bash
   gh pr list --head <branch-atual> --state open --json number,title,url
   ```

2. **Se não existir PR aberto**:
   - Criar o PR seguindo o formato obrigatório de título (Semantic Commit)
   - Preencher a descrição com as três seções obrigatórias
   - Adicionar as labels correspondentes
   - Reportar a URL do PR criado no relatório final

3. **Se já existir PR aberto**:
   - Revisar o título — atualizar se a nova mudança alterar o escopo ou foco
   - Revisar a descrição — incorporar as mudanças do novo commit
   - Atualizar labels se o tipo ou impacto da mudança se alterou
   - Reportar a URL do PR atualizado no relatório final

### Regras:
- O assistente **não deve perguntar** ao usuário se deve criar o PR — a criação é automática quando não existe PR aberto
- O push para o branch remoto deve ocorrer **antes** da verificação/criação do PR (o PR depende do branch remoto estar atualizado)
- Se o push falhar, o PR não deve ser criado — registrar o erro e reportar

---

## Política de Acompanhamento de GitHub Actions

### Princípio:
> Código pushado não está validado até que o CI confirme. O acompanhamento das Actions é a etapa final obrigatória do pipeline e **condição de encerramento da tarefa**.

### Quando se aplica:
Esta política é ativada automaticamente **após o push e a criação/atualização do PR**, como passo 11 do pipeline de validação pré-commit definido em `CLAUDE.md`. **Aplica-se a toda tarefa que resulte em push — incluindo tarefas exclusivamente de governança** (sem código, sem build, sem Docker). A ausência de passos 0–8 não dispensa este passo.

### Workflow obrigatório:

1. **Identificar a execução ativa** vinculada ao PR/branch:
   ```bash
   gh api repos/<owner>/<repo>/actions/runs --jq '.workflow_runs[:1] | .[].id'
   ```

2. **Acompanhar os jobs** até a conclusão de todos:
   ```bash
   gh api repos/<owner>/<repo>/actions/runs/<run-id>/jobs --jq '.jobs[] | {name, status, conclusion}'
   ```
   - Polling a cada 30–90 segundos até que todos os jobs tenham `status: completed`

3. **Se todos os jobs passarem** (`conclusion: success`):
   - Verificar os logs no Datadog usando os filtros referentes ao pipeline:
     - Filtrar por `env` correspondente ao contexto de execução (`ci`)
     - Filtrar por `service` correspondente à aplicação
     - Filtrar pelo intervalo de tempo da execução do pipeline
   - Procurar por erros, exceções ou comportamentos anômalos nos logs
   - Se não houver erros nos logs: reportar o resultado final com a lista de jobs, seus status e a confirmação de logs limpos no Datadog. A tarefa está concluída.
   - Se houver erros nos logs: diagnosticar, corrigir, registrar em `bash-errors-log.md` e reiniciar o ciclo

4. **Se algum job falhar** (`conclusion: failure`):
   - Obter os logs do job que falhou:
     ```bash
     gh api repos/<owner>/<repo>/actions/runs/<run-id>/jobs --jq '.jobs[] | select(.conclusion == "failure") | {name, id}'
     ```
   - Analisar os logs considerando **apenas os registros de erro emitidos no horário de execução da pipeline** — ignorar logs antigos ou de execuções anteriores
   - Diagnosticar a causa raiz
   - Corrigir o código, testes ou configuração de CI conforme necessário
   - Reiniciar o pipeline de validação pré-commit a partir do passo apropriado (build, test ou commit dependendo do que foi alterado)
   - Registrar o erro em `bash-errors-log.md` se for um erro novo
   - Repetir o ciclo de acompanhamento até que todos os jobs passem

### Regras:
- **A tarefa NÃO se encerra com a abertura ou atualização do PR.** O PR é um passo intermediário — a tarefa só está concluída após a validação do pipeline e a conferência dos logs no Datadog.
- O assistente **não deve encerrar a tarefa** enquanto houver jobs em execução, jobs falhando ou logs no Datadog não verificados.
- Erros de CI devem ser tratados com a mesma diligência que erros locais
- Se o erro for intermitente (flaky test, timeout de rede), documentar e tentar novamente antes de investigar profundamente
- Se o erro exigir mudança em arquivo de governança, ativar a meta-regra de revisão de instruções (`instruction-review.md`)

---

## Política de Merge

### Método obrigatório: Merge Commit

Todo merge de PR neste repositório deve utilizar o método **merge commit** (`merge_method: "merge"`).

**Justificativa**: O merge commit preserva todos os commits individuais do branch de feature no histórico do branch principal, mantendo rastreabilidade completa de cada mudança realizada. Isso garante que o histórico de commits reflita fielmente a sequência de trabalho, facilitando auditoria, diagnóstico de regressões e navegação temporal do código.

**Proibido**:
- Squash merge (`merge_method: "squash"`) — consolida commits e perde granularidade do histórico
- Rebase merge (`merge_method: "rebase"`) — reescreve a árvore de commits e perde a referência ao branch original

**Aplicação**: Esta política se aplica a todo merge realizado pelo assistente, seja via skill `pr-analysis` ou via qualquer outro fluxo que resulte em merge de PR.

---

## Política de Consistência

- A descrição é a **fonte de verdade textual** do PR
- Se há discrepância entre a descrição e os commits, a descrição deve ser corrigida
- Se há alterações no código que não estão na descrição, a descrição está incompleta
- Se há referências na descrição a código que não existe mais, a descrição está desatualizada

**O assistente é responsável por manter essa consistência automaticamente**, sem necessidade de instrução explícita do usuário a cada atualização.

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
| 2026-03-19 | Reforço: acompanhamento de Actions e verificação de logs Datadog tornados condição de encerramento da tarefa; aplicabilidade em tarefas de governança explicitada | Falha observada em sessão |
| 2026-03-20 | Adicionado: Política de Merge — merge commit definido como método obrigatório; squash e rebase proibidos | Instrução do usuário |
