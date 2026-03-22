---
description: Criar ou verificar um checkpoint no workflow de desenvolvimento. Permite salvar estado e comparar progresso.
---

# Comando Checkpoint

Cria ou verifica checkpoints durante o desenvolvimento.

## Uso

`/checkpoint [create|verify|list] [nome]`

## Criar Checkpoint

1. Executar verificação rápida do estado atual (build limpo?)
2. Criar commit ou stash com nome do checkpoint
3. Registrar em `.claude/checkpoints.log`:

```bash
echo "$(date +%Y-%m-%d-%H:%M) | $NOME | $(git rev-parse --short HEAD)" >> .claude/checkpoints.log
```

4. Reportar checkpoint criado

## Verificar Checkpoint

1. Ler checkpoint do log
2. Comparar estado atual com o checkpoint:
   - Arquivos adicionados desde o checkpoint
   - Arquivos modificados desde o checkpoint
   - Taxa de testes agora vs então
3. Reportar:

```
COMPARAÇÃO COM CHECKPOINT: $NOME
================================
Arquivos alterados: X
Testes: +Y passando / -Z falhando
Build: [OK/FALHA]
```

## Listar Checkpoints

Mostrar todos os checkpoints com:
- Nome
- Timestamp
- Git SHA
- Status (atual, atrás, à frente)

## Workflow Típico

```
[Início] --> /checkpoint create "inicio-feature"
   |
[Implementar] --> /checkpoint create "core-pronto"
   |
[Testar] --> /checkpoint verify "core-pronto"
   |
[Refatorar] --> /checkpoint create "refatoracao-pronta"
   |
[PR] --> /checkpoint verify "inicio-feature"
```

## Argumentos

- `create <nome>` — Criar checkpoint nomeado
- `verify <nome>` — Verificar contra checkpoint
- `list` — Mostrar todos os checkpoints
- `clear` — Remover checkpoints antigos (mantém últimos 5)
