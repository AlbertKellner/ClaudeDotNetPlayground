---
description: Corrigir erros de build e compilação incrementalmente com mudanças mínimas e seguras.
---

# Comando Build Fix

Corrige erros de build incrementalmente, um erro por vez.

## Passo 1: Executar Build

```bash
dotnet build src/Albert.Playground.ECS.AOT.Api/Albert.Playground.ECS.AOT.Api.csproj 2>&1
```

Se o build passar sem erros, reportar sucesso e parar.

## Passo 2: Agrupar Erros

1. Capturar saída de erro do build
2. Agrupar erros por arquivo
3. Ordenar por ordem de dependência (corrigir imports/tipos antes de erros de lógica)
4. Contar total de erros para acompanhar progresso

## Passo 3: Loop de Correção (Um Erro por Vez)

Para cada erro:

1. **Ler o arquivo** — Ver contexto do erro (10 linhas ao redor)
2. **Diagnosticar** — Identificar causa raiz (import faltante, tipo errado, erro de sintaxe)
3. **Corrigir minimamente** — Menor mudança que resolve o erro
4. **Re-executar build** — Verificar que o erro sumiu e nenhum novo foi introduzido
5. **Próximo** — Continuar com erros restantes

## Passo 4: Guardrails

Parar e perguntar ao usuário se:
- Uma correção introduz **mais erros do que resolve**
- O **mesmo erro persiste após 3 tentativas** (provavelmente problema mais profundo)
- A correção requer **mudanças arquiteturais** (não é só um fix de build)
- Erros de build vêm de **dependências faltantes** (precisa de `dotnet add package`)

## Passo 5: Resumo

```
RESULTADO: [X erros corrigidos]
================================
Corrigidos:
- [arquivo:linha] — [descrição da correção]

Restantes: [Y erros]
Novos introduzidos: [deve ser zero]

Próximos passos: [sugestões para erros não resolvidos]
```

## Estratégias de Recuperação

| Situação | Ação |
|----------|------|
| Namespace/using faltante | Adicionar `using` correto |
| Tipo incompatível | Verificar ambas as definições; corrigir o mais específico |
| Pacote NuGet faltante | Sugerir `dotnet add package` |
| Conflito de versão | Verificar `.csproj` |
| Incompatibilidade AOT | Verificar se há reflection não anotada; consultar DA-009 |

Corrigir um erro por vez para segurança. Preferir diffs mínimos.
