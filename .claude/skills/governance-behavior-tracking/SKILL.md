# Skill: governance-behavior-tracking

## Nome

Rastreamento de Comportamentos Esperados

## Descrição

Coleta, apresenta, rastreia e verifica todos os comportamentos esperados durante uma sessão de trabalho, conforme definidos nos arquivos de governança do repositório.

---

## Quando Usar

Esta skill é ativada automaticamente no início de qualquer tarefa de implementação, governança ou análise de PR. É o primeiro ato operacional após a classificação de escopo.

---

## Workflow Interno

```
FASE 1 — COLETAR COMPORTAMENTOS ESPERADOS

  1. Classificar o escopo da tarefa:
     - Código → todos os passos do pipeline (0 → 11)
     - Governança → apenas passos 0.1, 9, 10
     - Análise de PR → conforme skill pr-analysis

  2. Derivar passos do pipeline pré-commit (CLAUDE.md):
     - Escopo código:
       [0]  Verificar pré-requisitos de ambiente
       [0.1] Executar auditoria de governança
       [1]  dotnet build
       [2]  dotnet run + health check
       [3]  dotnet test (gate obrigatório)
       [4]  docker compose up -d
       [5]  Health check HTTP 200
       [6]  Validação de endpoints via HTTP
       [7]  Exibir logs do container
       [8]  docker compose down
       [9]  Commit
       [10] Criar/atualizar PR
       [11] Acompanhar CI + verificar logs Datadog
     - Escopo governança:
       [0.1] Executar auditoria de governança
       [9]  Commit
       [10] Criar/atualizar PR

  3. Derivar comportamentos obrigatórios (CLAUDE.md seção "Comportamento Obrigatório"):
     - Interpretar antes de agir
     - Ler governança relevante antes de implementar
     - Verificar ambiguidades antes de implementar
     - Classificar trechos técnicos enviados pelo usuário (quando aplicável)
     - Atualizar governança primeiro (quando aplicável)
     - Seguir prioridade entre fontes de verdade
     - Usar contexto acumulado do repositório
     - Não depender de repetição de instruções
     - Avaliar eficiência em toda tarefa
     - Proteção de branch em análise de PR (quando aplicável)
     - Rastrear comportamentos esperados (esta skill)

  4. Derivar comportamentos de skills ativados:
     - implement-request: normalizar, classificar, ler governança, verificar ambiguidades,
       classificar trechos, registrar dúvidas, atualizar governança, avaliar propagação,
       implementar, verificar cobertura, relatar
     - validate-endpoints: identificar endpoints, obter token, consumir, validar cache,
       exibir logs storytelling
     - manage-pr-lifecycle: verificar PR existente, criar/atualizar, acompanhar CI
     - verify-environment: checklist de pré-requisitos
     - Outros skills conforme ativação pela tarefa

  5. Eliminar duplicatas (comportamentos que aparecem em múltiplas fontes)

  6. Remover comportamentos inaplicáveis ao escopo:
     - Sem trechos técnicos → remover "Classificar trechos técnicos"
     - Sem definição durável nova → remover "Atualizar governança primeiro"
     - Sem endpoint novo/alterado → remover "Validação de endpoints"
     - Escopo governança → remover passos de build/docker/test

FASE 2 — APRESENTAR VIA TODOWRITE

  1. Criar lista TodoWrite com todos os comportamentos, organizados em grupos:

     GRUPO: Governança e Interpretação
       - Interpretar intenção do usuário
       - Ler governança relevante
       - Verificar ambiguidades
       - [outros aplicáveis]

     GRUPO: Pipeline de Validação
       - [Passo 0] Verificar ambiente
       - [Passo 0.1] Auditoria de governança
       - [Passo 1] Build
       - [outros passos aplicáveis]

     GRUPO: Encerramento
       - [Passo 9] Commit
       - [Passo 10] PR
       - [Passo 11] CI + Datadog
       - Verificação final de comportamentos (esta skill, Fase 4)

  2. Todos marcados como pending inicialmente

FASE 3 — ATUALIZAR DURANTE EXECUÇÃO

  1. Marcar cada comportamento como in_progress ao iniciá-lo
  2. Marcar como completed ao concluí-lo com sucesso
  3. Se a tarefa revelar novos comportamentos necessários:
     - Adicionar ao TodoWrite como pending
  4. Se um comportamento se tornar inaplicável durante a execução:
     - Remover do TodoWrite (não manter como pendente)

FASE 4 — VERIFICAÇÃO FINAL

  1. Revisar o TodoWrite inteiro
  2. Para cada comportamento NÃO marcado como completed:
     a. Classificar o motivo:
        (i)   Inaplicável ao escopo — justificar
        (ii)  Omitido/esquecido — executar imediatamente
        (iii) Bloqueado por erro — registrar em bash-errors-log.md
     b. Para (ii) e (iii), investigar causa raiz:
        - Por que o comportamento não foi executado?
        - O que na governança falhou em garantir a execução?
        - Qual correção previne recorrência?
     c. Implementar correção da causa raiz (atualizar skill, rule, hook ou pipeline)

  3. Emitir relatório de comportamentos:
     - Total esperados
     - Total executados
     - Total inaplicáveis (com justificativa)
     - Total omitidos e corrigidos (com causa raiz)
     - Ações de saneamento (se houver)
```

---

## Arquivos de Governança Relacionados

- `.claude/rules/governance-behavior-tracking.md` — política que este workflow implementa
- `CLAUDE.md` — fonte dos passos do pipeline e comportamentos obrigatórios
- `.claude/rules/execution-time-tracking.md` — complementar (tempo + comportamentos)
- `.claude/rules/governance-audit.md` — auditoria de artefatos complementa auditoria de comportamentos
- `Instructions/operating-model.md` — classificação de tipos de mensagem e skills ativados

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-21 | Criado: workflow de rastreamento de comportamentos esperados | Instrução do usuário |
