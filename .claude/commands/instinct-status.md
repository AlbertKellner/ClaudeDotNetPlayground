---
description: Mostrar instintos aprendidos com confidence, agrupados por domínio. Executa o script instinct-manager.sh.
---

# Comando Instinct Status

Mostra o estado atual dos instintos do sistema de aprendizado contínuo.

## Uso

```
/instinct-status
```

## Processo

1. Executar o CLI de instintos:

```bash
bash scripts/instinct-manager.sh list
```

2. Complementar com estatísticas:

```bash
bash scripts/instinct-manager.sh stats
```

3. Apresentar resultado formatado ao usuário.

## Formato de Saída

```
════════════════════════════════════════
  ESTADO DOS INSTINTOS — N total
════════════════════════════════════════

  Ativos (confidence >= 0.5): X
  Tentativos (confidence < 0.5): Y

## ATIVOS (por domínio)
  ### ENVIRONMENT (N)
    ████████░░  80%  nome-do-instinto
              gatilho: quando [condição]

  ### PIPELINE (N)
    ██████░░░░  60%  nome-do-instinto
              gatilho: quando [condição]

## TENTATIVOS
  ### TOOLING (N)
    ███░░░░░░░  30%  nome-do-instinto
              gatilho: quando [condição]

## ESTATÍSTICAS
  Observações acumuladas: N
  Última análise: [data]
  Candidatos a graduação: N
════════════════════════════════════════
```

## Ações Disponíveis Após Visualização

- Para forçar decay: `bash scripts/instinct-manager.sh decay`
- Para limpar expirados: `bash scripts/instinct-manager.sh prune`
- Para promover um instinto: `bash scripts/instinct-manager.sh promote <id>`
- Para exportar: `/instinct-export`
