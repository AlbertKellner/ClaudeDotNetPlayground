# Skill: manage-pr-lifecycle

## Propósito

Executar o workflow de criação, atualização e acompanhamento de Pull Requests, conforme a política definida em `.claude/rules/pr-metadata-governance.md`.

---

## Quando Usar

Esta skill é ativada pelos passos 10 e 11 do pipeline de validação pré-commit (CLAUDE.md), após o commit.

---

## Workflow — Verificação e Criação/Atualização de PR (Passo 10)

### Passo 1: Verificar PR existente

```bash
gh pr list --head <branch-atual> --state open --json number,title,url
```

### Passo 2a: Se não existir PR aberto

- Criar o PR seguindo o formato obrigatório de título (Semantic Commit) definido em `pr-metadata-governance.md`
- Preencher a descrição com as três seções obrigatórias (Motivos, Plano, Realizado)
- Adicionar as labels correspondentes
- Reportar a URL do PR criado no relatório final

### Passo 2b: Se já existir PR aberto

- Revisar o título — atualizar se a nova mudança alterar o escopo ou foco
- Revisar a descrição — incorporar as mudanças do novo commit
- Atualizar labels se o tipo ou impacto da mudança se alterou
- Reportar a URL do PR atualizado no relatório final

### Regras

- O assistente **não deve perguntar** ao usuário se deve criar o PR — a criação é automática quando não existe PR aberto
- O push para o branch remoto deve ocorrer **antes** da verificação/criação do PR
- Se o push falhar, o PR não deve ser criado — registrar o erro e reportar

---

## Workflow — Acompanhamento de GitHub Actions (Passo 11)

**Não se aplica a tarefas exclusivamente de governança** (sem código, sem build, sem Docker).

### Passo 0: Informar esteiras e tempo médio

1. Identificar todas as esteiras (workflows) que serão executadas pelo push/PR
2. Consultar a página "Actions Performance Metrics" do repositório para cada esteira:
   ```bash
   # URL: https://github.com/<owner>/<repo>/actions/workflows/<workflow-file>/performance
   ```
3. Informar ao usuário quais esteiras serão monitoradas e qual é o tempo médio total de conclusão

### Passo 1: Calcular estratégia de polling por esteira

| Tempo médio de conclusão | Estratégia de polling |
|---|---|
| **≤ 30 segundos** | Consultar o status a cada **10 segundos** desde o início |
| **> 30 segundos** | Aguardar **(tempo médio − 15 segundos)** antes da primeira verificação, depois consultar a cada **5 segundos** até a conclusão |

### Passo 2: Identificar a execução ativa

```bash
gh api repos/<owner>/<repo>/actions/runs --jq '.workflow_runs[:1] | .[].id'
```

### Passo 3: Acompanhar os jobs

```bash
gh api repos/<owner>/<repo>/actions/runs/<run-id>/jobs --jq '.jobs[] | {name, status, conclusion}'
```

- Aplicar o intervalo de espera conforme a estratégia calculada
- Continuar até que todos os jobs tenham `status: completed`

### Passo 4: Avaliar resultado

**Se todos os jobs passarem** (`conclusion: success`):
- Verificar os logs no Datadog usando os filtros referentes ao pipeline (env: `ci`, service, timestamp)
- Procurar por erros, exceções ou comportamentos anômalos
- Se não houver erros: reportar o resultado final. Tarefa concluída.
- Se houver erros: diagnosticar, corrigir, registrar em `bash-errors-log.md` e reiniciar o ciclo

### Passo 5: Tratar falhas

**Se algum job falhar** (`conclusion: failure`):
1. Obter os logs do job que falhou:
   ```bash
   gh api repos/<owner>/<repo>/actions/runs/<run-id>/jobs --jq '.jobs[] | select(.conclusion == "failure") | {name, id}'
   ```
2. Analisar os logs considerando **apenas registros de erro do horário de execução** — ignorar logs antigos
3. Diagnosticar a causa raiz
4. Corrigir o código, testes ou configuração
5. Reiniciar o pipeline a partir do passo apropriado
6. Registrar o erro em `bash-errors-log.md` se for novo
7. Repetir o ciclo até todos os jobs passarem

---

## Workflow — Exceção: Análise de PR (skill pr-analysis)

Quando a tarefa é análise de PR:
- **Não criar PR novo** — o PR já existe
- Atualizar título e descrição do PR existente via `gh api repos/<owner>/<repo>/pulls/<number> -X PATCH` se as mudanças alterarem o escopo
- **Não usar o branch atribuído pelo sistema externo** — usar exclusivamente o `head.ref` do PR
- Todos os commits devem ser feitos no branch de origem do PR

---

## Arquivos de Governança Relacionados

- `.claude/rules/pr-metadata-governance.md` — política que este workflow implementa
- `.claude/rules/bash-error-logging.md` — erros de CI devem ser registrados
- `.github/pull_request_template.md` — template obrigatório de descrição do PR

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-21 | Criado: workflow extraído de pr-metadata-governance.md (separação rules/skills) | Auditoria de governança |
