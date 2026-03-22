# Instintos — Conhecimento Operacional Aprendido

## Propósito

Esta pasta contém instintos — padrões operacionais detectados automaticamente a partir de observações de uso de ferramentas ao longo das sessões de trabalho.

---

## Estrutura

```
instincts/
├── active/       # Instintos com confidence >= 0.5 (conhecimento confirmado)
├── tentative/    # Instintos com confidence < 0.5 (padrão emergente)
└── README.md     # Este arquivo
```

---

## Formato de Instinto

Cada instinto é um arquivo `.md` com frontmatter YAML:

```markdown
---
id: nome-kebab-case-descritivo
trigger: "Quando [condição específica]"
action: "Fazer [ação específica]"
confidence: 0.7
domain: environment | pipeline | governance | code-pattern | endpoint | tooling
source: session-observation
evidence_count: 5
sessions_observed: 3
last_observed: 2026-03-22
last_decay: 2026-03-22
created: 2026-03-22
---

# Título Descritivo do Instinto

## Evidência
- Observado N vezes em M sessões distintas
- Padrão: [descrição do padrão detectado]
- Última ocorrência: [data]

## Contexto
[Contexto adicional sobre quando e por que este padrão ocorre]
```

---

## Ciclo de Vida

1. **Criação**: padrão detectado com >= 3 observações → instinto tentativo (confidence 0.3-0.5)
2. **Confirmação**: observações adicionais → confidence sobe → promovido para `active/`
3. **Maturação**: confidence >= 0.85 + 3 sessões → candidato a graduação
4. **Graduação**: aprovado pelo usuário → vira rule, skill ou check de auditoria → movido para `../graduated/`
5. **Decay**: sem observações por semana → confidence diminui → abaixo de 0.2 = removido

---

## Convenções

- **Nomes de arquivo**: `<id>.md` onde id é o campo `id` do frontmatter (kebab-case)
- **Um instinto por arquivo**
- **Não editar manualmente** a menos que seja para corrigir um instinto incorreto
- **Não mover entre pastas manualmente** — o workflow de análise gerencia a posição baseado no confidence

---

## Referências

- `.claude/rules/continuous-learning.md` — políticas do sistema
- `.claude/skills/continuous-learning/SKILL.md` — workflow de análise e evolução
- `.claude/learning/config.json` — configuração de thresholds e domínios
