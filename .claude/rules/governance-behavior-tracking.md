# Regra: Rastreamento de Comportamentos Esperados

## Propósito

Esta rule define a política de rastreamento, visibilidade e verificação de todos os comportamentos esperados ao longo de uma sessão de trabalho. Garante que nenhum comportamento definido na governança seja silenciosamente omitido.

---

## Princípio Fundamental

> Todo comportamento definido na governança deve ser rastreado, executado e verificado.
> Omissão silenciosa de comportamentos é uma falha de governança.
> Visibilidade contínua permite ao usuário acompanhar o cumprimento dos processos em tempo real.

---

## Políticas

### Inicialização Obrigatória

No início de qualquer tarefa, antes do primeiro passo de implementação, o assistente deve:
1. Classificar o escopo da tarefa (código / governança / pr-analysis)
2. Coletar todos os comportamentos esperados dos arquivos de governança
3. Apresentá-los ao usuário via TodoWrite, agrupados por fase

### Fontes de Derivação de Comportamentos

Os comportamentos esperados são derivados de três fontes complementares:

| Fonte | O que contém | Onde está |
|---|---|---|
| Pipeline pré-commit | Passos obrigatórios por escopo (0 a 11) | `CLAUDE.md` seção "Pipeline de Validação Pré-Commit" |
| Comportamentos obrigatórios | Regras transversais de toda interação (itens 1–11) | `CLAUDE.md` seção "Comportamento Obrigatório" |
| Skills ativados | Workflows procedurais específicos da tarefa | `.claude/skills/*/SKILL.md` |

A combinação das três fontes forma a lista completa de comportamentos esperados para a tarefa.

### Filtragem por Escopo

Nem todos os comportamentos se aplicam a todos os escopos. A filtragem é obrigatória:

| Escopo | Passos do pipeline | Comportamentos obrigatórios |
|---|---|---|
| **Código** | Todos: 0 → 11 | Todos aplicáveis |
| **Governança** | Apenas: 0.1, 9, 10 | Todos exceto os vinculados a build/execução |
| **Análise de PR** | Conforme skill pr-analysis | Todos exceto criação de PR (passo 10) |

### Visibilidade Contínua

- A lista de comportamentos deve ser mantida atualizada durante toda a execução via TodoWrite
- Cada comportamento deve ser marcado como `pending` → `in_progress` → `completed` à medida que é executado
- Novos comportamentos descobertos durante a execução (ex: endpoint validation necessário) devem ser adicionados
- Comportamentos que se tornarem inaplicáveis (ex: sem trechos técnicos para classificar) devem ser removidos da lista

### Verificação Final Obrigatória

Ao final da tarefa, antes de considerar o trabalho concluído, o assistente deve:
1. Revisar a lista completa de comportamentos no TodoWrite
2. Para cada comportamento não marcado como `completed`, classificar o motivo:
   - **(i) Inaplicável ao escopo**: documentar justificativa no relatório final
   - **(ii) Omitido/esquecido**: executar o comportamento pendente imediatamente
   - **(iii) Bloqueado por erro**: registrar em `bash-errors-log.md`
3. Para omissões classificadas como (ii) ou (iii), investigar e sanear a causa raiz:
   - Por que o comportamento não foi executado?
   - Por que a governança existente não assegurou a execução?
   - O que deve ser corrigido (skill, rule, hook, pipeline) para evitar recorrência?
4. Implementar a correção da causa raiz quando aplicável
5. Emitir relatório de comportamentos no relatório final da tarefa

### Formato do Relatório de Comportamentos

O relatório deve conter:
- Total de comportamentos esperados
- Total executados com sucesso
- Total inaplicáveis (com justificativa por item)
- Total omitidos e corrigidos (com causa raiz e correção por item)
- Ações de saneamento realizadas (se houver)

---

## Relação com Outras Rules

- `execution-time-tracking.md` — complementar: tempo + comportamentos dão visibilidade completa da sessão
- `governance-audit.md` — a auditoria verifica consistência de artefatos; esta rule verifica execução de comportamentos
- `governance-policies.md` — comportamentos derivados das políticas aqui definidas
- `pr-metadata-governance.md` — o relatório de comportamentos pode ser incluído na seção "O que foi realizado" do PR

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-21 | Criado: regra de rastreamento de comportamentos esperados | Instrução do usuário |
