# Snippets Normativos Canônicos

## Propósito

Esta pasta armazena referências canônicas de snippets normativos — trechos técnicos fornecidos explicitamente pelo usuário para serem preservados na íntegra e respeitados em futuras implementações.

---

## Quando um Trecho Deve Ser Tratado como Snippet Normativo

Um trecho é normativo quando o usuário indica claramente que ele deve ser preservado exatamente como fornecido:
- "inclua exatamente assim"
- "copie isso"
- "use exatamente esse trecho"
- "mantenha esse código"
- "não altere"
- "preserve isso"
- "é pra ser literalmente esse"

---

## Quando um Trecho É Apenas Exemplo

Um trecho é exemplo ilustrativo quando o usuário indica que é referência ou inspiração:
- "algo assim"
- "tipo isso"
- "como exemplo"
- "baseado nisso"
- "nessa linha"

**Exemplos ilustrativos não são registrados nesta pasta como canônicos.**
Eles podem ser adaptados livremente ao contexto do projeto.

---

## Como Snippets Canônicos Influenciam Implementações Futuras

Quando um snippet é registrado em `canonical-snippets.md`:
1. Toda implementação futura relacionada ao escopo do snippet deve respeitá-lo
2. O assistente verifica este arquivo antes de implementar artefatos no escopo do snippet
3. O conteúdo do snippet é aplicado na íntegra nos locais de destino indicados
4. Qualquer divergência do snippet canônico deve ser justificada e reportada

---

## Como Mudanças em Snippets Canônicos Devem Ser Tratadas

Snippets canônicos **não podem ser reescritos livremente**.
Para alterar um snippet canônico:
1. O usuário deve fornecer nova instrução explícita com o novo conteúdo
2. Ou deve haver conflito técnico verificável que impeça a aplicação do snippet original
3. Em ambos os casos, a mudança deve ser justificada e reportada
4. O registro no `canonical-snippets.md` deve ser atualizado com a nova versão

---

## Estrutura da Pasta

```
snippets/
├── README.md               # (este arquivo)
└── canonical-snippets.md   # Registro de snippets canônicos
```

---

## Referências Cruzadas

- `.claude/rules/governance-policies.md` §5 — políticas de classificação e tratamento de snippets
- `.claude/skills/apply-user-snippet/SKILL.md` — skill que processa trechos do usuário
- `Instructions/operating-model.md` — modelo operacional que contextualiza snippets
