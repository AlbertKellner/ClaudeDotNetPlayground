---
description: Exportar instintos do sistema de aprendizado contínuo para formato compartilhável.
---

# Comando Instinct Export

Exporta instintos para formato compartilhável — útil para transferência entre projetos ou backup.

## Uso

```
/instinct-export                           # Exportar todos os instintos
/instinct-export --domain environment      # Apenas domínio específico
/instinct-export --min-confidence 0.7      # Apenas alta confiança
```

## Processo

1. Ler instintos de `.claude/learning/instincts/active/` e `tentative/`
2. Aplicar filtros se especificados
3. Gerar arquivo de exportação em `.claude/learning/exports/`

## Formato de Exportação

Cria arquivo YAML-style:

```yaml
# Exportação de Instintos
# Gerado: 2026-03-22
# Projeto: Albert.Playground.ECS.AOT
# Total: N instintos

---
id: nome-kebab-case
trigger: "quando [condição]"
action: "fazer [ação]"
confidence: 0.8
domain: environment
evidence_count: 5
sessions_observed: 3
---

# Título do Instinto

## Evidência
- Observado N vezes em M sessões
- Padrão: [descrição]
```

## Filtros Disponíveis

- `--domain <nome>` — Exportar apenas domínio específico (environment, pipeline, governance, code-pattern, endpoint, tooling)
- `--min-confidence <n>` — Threshold mínimo de confidence (0.0-1.0)
- `--active-only` — Apenas instintos ativos (>= 0.5)

## Importação

Para importar instintos exportados de outro projeto:
1. Copiar os arquivos `.md` para `.claude/learning/instincts/tentative/`
2. Ajustar confidence para 0.3 (recomeçar validação no novo contexto)
3. O sistema de aprendizado irá confirmar ou rejeitar com base nas observações
