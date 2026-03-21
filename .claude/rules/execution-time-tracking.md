# Regra: Rastreamento de Tempo de Execução Efetivo

## Propósito

Esta rule define a política de rastreamento e reporte de tempo de execução efetivo do assistente durante sessões de trabalho. O tempo efetivo exclui períodos de ociosidade (espera por resposta ou interação do usuário).

---

## Princípio Fundamental

> Visibilidade do tempo de execução permite ao usuário avaliar eficiência e planejar sessões.
> Tempo ocioso não é tempo de trabalho — apenas o tempo em que o assistente está ativamente processando é contabilizado.

---

## Políticas

### Inicialização

No início de cada sessão (primeiro processamento de mensagem do usuário), o assistente deve:
1. Executar `date +%s%3N` para obter o timestamp atual em milissegundos
2. Criar o arquivo `.claude/.session-timer` com os valores iniciais
3. Emitir uma linha informativa: `[Sessão iniciada: HH:MM:SS]`

Se o arquivo `.claude/.session-timer` já existir com idade superior a 4 horas, considerar como sessão anterior encerrada de forma anormal — resetar o arquivo.

### Segmentos de Trabalho

Cada mensagem do usuário processada constitui um **segmento de trabalho**:
- **Início do segmento**: registrar timestamp ao começar a processar a mensagem
- **Fim do segmento**: registrar timestamp e acumular a duração em `SEGMENTS_TOTAL_MS` ao concluir o processamento
- O tempo entre segmentos (período em que o assistente está inativo, aguardando o usuário) **não é contabilizado**

### Reporte Periódico

A cada ~60 segundos de tempo efetivo acumulado (verificado após cada chamada de ferramenta), o assistente deve emitir uma linha de status:

```
[Tempo efetivo: MM:SS]
```

O reporte ocorre em pontos de interrupção naturais (entre chamadas de ferramenta), não em intervalos de relógio. O hook `session-timer.sh` exibe o tempo acumulado após cada chamada Bash, tornando o tempo visível para o assistente decidir quando reportar.

### Reporte Final

Ao concluir a sessão ou a última tarefa, o assistente deve emitir um resumo:

```
[Tempo total efetivo da sessão: HH:MM:SS (N segmentos de trabalho)]
```

### Arquivo de Estado

| Campo | Descrição |
|---|---|
| `SESSION_START` | Epoch em milissegundos do início da sessão |
| `SEGMENTS_TOTAL_MS` | Tempo efetivo acumulado em milissegundos |
| `LAST_SEGMENT_START` | Epoch em milissegundos do início do segmento atual |
| `LAST_REPORT_AT_MS` | Valor de `SEGMENTS_TOTAL_MS` no momento do último reporte periódico |
| `SEGMENT_COUNT` | Número de segmentos de trabalho processados |

- **Localização**: `.claude/.session-timer`
- **Formato**: pares `CHAVE=VALOR` (compatível com `source` do bash)
- **Ciclo de vida**: criado no início da sessão, transiente — não versionado (adicionado ao `.gitignore`)

---

## Relação com Outras Rules

- `environment-readiness.md` — a inicialização do timer faz parte da preparação do ambiente de sessão
- `governance-policies.md` — eficiência de execução (§2) complementada por visibilidade de tempo

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-21 | Criado: regra de rastreamento de tempo de execução efetivo | Instrução do usuário |
