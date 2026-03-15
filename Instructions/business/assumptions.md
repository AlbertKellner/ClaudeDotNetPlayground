# Premissas de Negócio

## Propósito

Este arquivo registra premissas de negócio adotadas — suposições sobre o domínio que ainda não foram confirmadas formalmente pelo usuário, mas que foram necessárias para avançar com implementações ou definições.

**Diferença entre este arquivo e `assumptions-log.md`**:
- `Instructions/business/assumptions.md` → premissas **de negócio** com impacto em regras, comportamentos e domínio
- `assumptions-log.md` → todas as premissas operacionais, incluindo técnicas, de processo e de ambiguidade

---

## Regras sobre Premissas de Negócio

1. Toda premissa que afete comportamento de negócio, regras, invariantes ou fluxos deve ser registrada aqui.
2. Premissas de negócio devem ser confirmadas pelo usuário o quanto antes.
3. Quando uma premissa for confirmada → remover daqui, incorporar nas definições definitivas (`business-rules.md`, `invariants.md`, etc.).
4. Quando uma premissa for invalidada → remover daqui, rever as implementações que dependiam dela.
5. Este arquivo deve conter **apenas** premissas ainda ativas.

---

## Premissas Ativas

> **Estado atual**: nenhuma premissa de negócio foi adotada ainda.
> Premissas serão registradas aqui quando necessário durante as interações.

### Template de Premissa de Negócio

```markdown
### PNE-[número] — [Título]
**Premissa**: [O que foi assumido]
**Motivo**: [Por que foi necessário assumir sem confirmação]
**Escopo**: [Quais regras, fluxos ou entidades dependem desta premissa]
**Risco**: Baixo | Médio | Alto
**Impacto se invalidada**: [O que precisaria ser revisto]
**Precisa de confirmação**: Sim
**Registrada também em**: assumptions-log.md (id: [PREM-NNN])
```

---

## Histórico de Premissas Resolvidas

> Quando uma premissa for resolvida, registrar aqui como rastreabilidade histórica:

| Id | Premissa | Resolução | Data | Onde consolidado |
|---|---|---|---|---|
| — | — | — | — | — |

---

## Referências Cruzadas

- `assumptions-log.md` — registro central de todas as premissas operacionais
- `Instructions/business/business-rules.md` — onde premissas confirmadas se tornam regras
- `open-questions.md` — dúvidas relacionadas às premissas

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| Bootstrap | Estrutura criada sem premissas específicas | — |
