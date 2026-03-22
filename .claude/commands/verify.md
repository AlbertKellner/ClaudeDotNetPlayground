---
description: Verificação abrangente do estado atual do codebase. Executa build, testes, auditoria de governança e status git.
---

# Comando Verify

Executa verificação completa do estado do codebase.

## Processo

Executar verificação nesta ordem exata:

1. **Build Check**
   ```bash
   dotnet build src/Albert.Playground.ECS.AOT.Api/Albert.Playground.ECS.AOT.Api.csproj
   ```
   Se falhar, reportar erros e PARAR.

2. **Testes**
   ```bash
   dotnet test src/Albert.Playground.ECS.AOT.UnitTest/Albert.Playground.ECS.AOT.UnitTest.csproj
   ```
   Reportar contagem de pass/fail.

3. **Auditoria de Governança**
   ```bash
   bash scripts/governance-audit.sh
   ```
   Reportar falhas e avisos.

4. **Status Git**
   - Mostrar mudanças não commitadas
   - Mostrar arquivos modificados desde último commit

5. **Verificação de Segurança**
   - Buscar por credenciais hardcoded em arquivos `.cs`
   - Buscar por `Console.WriteLine` em código de produção

## Formato de Saída

```
VERIFICAÇÃO: [APROVADO/REPROVADO]
==================================

Build:        [OK/FALHA]
Testes:       [X/Y passando]
Governança:   [OK/X falhas, Y avisos]
Segurança:    [OK/X encontrados]
Git:          [X arquivos modificados]

Pronto para commit: [SIM/NÃO]
```

Se houver problemas críticos, listar com sugestões de correção.

## Argumentos

- `quick` — Apenas build + testes
- `full` — Todas as verificações (padrão)
- `pre-commit` — Verificações relevantes para commit (equivale ao pipeline pré-commit simplificado)
- `governance` — Apenas auditoria de governança
