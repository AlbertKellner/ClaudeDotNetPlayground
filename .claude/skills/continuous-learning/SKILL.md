# Skill: continuous-learning

## Propósito

Workflow de análise de observações, criação e evolução de instintos, e proposição de graduações para a governança formal do repositório.

---

## Nome da Skill

`continuous-learning`

---

## Quando Usar

Esta skill é invocada:
1. **Sob demanda** — quando o usuário solicitar análise de padrões ou revisão de instintos
2. **Ao final de uma sessão** — para consolidar aprendizados da sessão
3. **Quando o hook de observação acumular >= 50 observações** sem análise recente
4. **Quando um erro recorrente for detectado** em `bash-errors-log.md`

---

## Workflow

### Fase 1 — Coleta de Observações

1. Ler `.claude/learning/observations.jsonl` (últimas 500 linhas, se existir)
2. Ler `.claude/learning/config.json` para obter thresholds ativos
3. Ler instintos existentes em `instincts/active/` e `instincts/tentative/`
4. Ler `bash-errors-log.md` para identificar erros recorrentes

### Fase 2 — Detecção de Padrões

Analisar as observações buscando:

| Padrão | Sinal | Confidence inicial |
|---|---|---|
| **Erro recorrente** | Mesmo tipo de erro 3+ vezes | 0.5 |
| **Correção de governança repetida** | `audit --fix` no mesmo check 3+ vezes | 0.5 |
| **Sequência de ferramentas otimizável** | Mesma sequência de 3+ tools repetida 3+ vezes | 0.3 |
| **Desvio de pipeline corrigido** | Passo do pipeline executado fora de ordem e depois corrigido | 0.3 |
| **Preferência de ferramenta** | Escolha consistente de ferramenta para um tipo de tarefa | 0.3 |
| **Correção do usuário** | Usuário corrige ação do assistente ("não, use X em vez de Y") | 0.7 |

### Fase 3 — Criação/Atualização de Instintos

Para cada padrão detectado:

1. **Verificar se já existe instinto similar** — buscar por domínio + trigger em instintos existentes
2. **Se existe**: incrementar `evidence_count`, ajustar confidence (+0.05), atualizar `last_observed`
3. **Se não existe**: criar novo instinto usando o template em `.claude/learning/instinct-template.md`
4. **Posicionar o instinto**:
   - confidence < 0.5 → `instincts/tentative/`
   - confidence >= 0.5 → `instincts/active/`
   - Se um instinto tentativo subir para >= 0.5 → mover para `active/`

### Fase 4 — Decay e Limpeza

1. Para cada instinto existente sem observação na última semana:
   - Reduzir confidence em 0.02
   - Se confidence < 0.2: remover o arquivo e registrar no log
2. Atualizar campo `last_decay` no instinto

### Fase 5 — Proposição de Graduações

Para cada instinto com confidence >= 0.85 e `sessions_observed` >= 3:

1. Classificar o destino sugerido:
   - Instinto de **ambiente** → check no `environment-readiness.md` ou `governance-audit.sh`
   - Instinto de **pipeline** → ajuste no pipeline pré-commit (`CLAUDE.md`)
   - Instinto de **governança** → nova rule ou atualização de rule existente
   - Instinto de **code-pattern** → snippet canônico ou atualização de `engineering-principles.md`
   - Instinto de **tooling** → nova skill ou atualização de skill existente

2. **Apresentar ao usuário** com:
   - O instinto completo (trigger, ação, evidência)
   - Destino sugerido e justificativa
   - Impacto na governança se aprovado
   - Pergunta: "Deseja graduar este instinto?"

3. **Se aprovado**: executar a graduação via skill correspondente (ingest-definition, evolve-governance)
4. **Se rejeitado**: manter instinto como está, sem degradação

### Fase 6 — Relatório

Ao final da análise, apresentar:

```
## Relatório de Aprendizado Contínuo

### Observações analisadas: N
### Instintos criados: N (novos)
### Instintos atualizados: N (confidence ajustado)
### Instintos removidos: N (decay < 0.2)
### Candidatos a graduação: N

### Instintos ativos (por domínio):
- environment: N instintos
- pipeline: N instintos
- governance: N instintos
- code-pattern: N instintos
- endpoint: N instintos
- tooling: N instintos
```

---

## Formato de Observação (JSONL)

```json
{
  "timestamp": "2026-03-22T10:30:00Z",
  "tool": "Bash",
  "input_summary": "docker compose up -d --build",
  "output_summary": "success: containers started",
  "result": "success",
  "pipeline_step": "4",
  "session_id": "abc123"
}
```

---

## Arquivos de Governança Relacionados

- `.claude/rules/continuous-learning.md` — políticas de captura, scoring e graduação
- `.claude/rules/bash-error-logging.md` — erros logados alimentam detecção de instintos
- `.claude/rules/governance-audit.md` — futuros checks de saúde do sistema de instintos
- `.claude/rules/governance-behavior-tracking.md` — complementaridade com tracking de comportamentos

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-22 | Criado: skill de aprendizado contínuo com workflow de 6 fases | Adaptação do ECC ao sistema de governança do repositório |
