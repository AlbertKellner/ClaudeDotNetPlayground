---
description: Revisão abrangente de segurança e qualidade das mudanças não commitadas. Bloqueia commit se encontrar problemas críticos.
---

# Comando Code Review

Revisão de segurança e qualidade do código alterado.

## Processo

1. Obter arquivos alterados: `git diff --name-only HEAD`

2. Para cada arquivo alterado, verificar:

### Problemas de Segurança (CRÍTICO)
- Credenciais hardcoded, API keys, tokens
- Vulnerabilidades de SQL injection
- Vulnerabilidades de XSS
- Validação de input ausente
- Dependências inseguras
- Riscos de path traversal

### Qualidade de Código (ALTO)
- Métodos > 50 linhas
- Arquivos > 800 linhas
- Profundidade de aninhamento > 4 níveis
- Tratamento de erro ausente
- Logs de debug esquecidos (`Console.WriteLine`)
- Comentários TODO/FIXME pendentes

### Boas Práticas — C# / .NET (MÉDIO)
- Padrão SNP-001 de logging seguido?
- File-scoped namespace (P007)?
- `var` usado onde possível (P008)?
- SRP respeitado (P009)?
- `try-catch` genérico fora do handler centralizado (P010)?
- Models de Feature em `Shared/` em vez de `<Feature>Models/` (DA-020)?
- AOT-compatibilidade (sem reflection dinâmica não anotada)?

### Alinhamento com Governança (MÉDIO)
- Convenções de nomenclatura seguidas (`naming-conventions.md`)?
- Estrutura de Vertical Slice correta (`folder-structure.md`)?
- Regras de negócio implementadas conforme `business-rules.md`?

## Formato do Relatório

```
REVISÃO DE CÓDIGO: [APROVADO/REPROVADO]
========================================

Segurança:    [OK/X problemas]
Qualidade:    [OK/X problemas]
Boas Práticas: [OK/X problemas]
Governança:   [OK/X problemas]

[Lista de problemas com severidade, arquivo:linha e sugestão de correção]

Pronto para commit: [SIM/NÃO]
```

3. Bloquear commit se problemas CRÍTICOS ou ALTOS encontrados

Nunca aprovar código com vulnerabilidades de segurança!
