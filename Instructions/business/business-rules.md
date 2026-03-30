# Regras de Negócio

## Propósito

Este arquivo registra as regras de negócio formalizadas deste repositório. Regras de negócio definem o que o sistema deve ou não deve fazer — são a fonte de verdade para comportamento esperado.

**Prioridade**: Regras de negócio prevalecem sobre preferências arquiteturais quando houver conflito.

---

## Como Ler Este Arquivo

Cada regra segue a estrutura:
- **Id**: identificador único (RN-NNN)
- **Título**: nome curto e descritivo
- **Enunciado**: a regra em linguagem de negócio clara
- **Condição**: quando a regra se aplica
- **Ação**: o que deve acontecer quando a condição é satisfeita
- **Exceções**: casos em que a regra não se aplica
- **Dependências**: outras regras ou invariantes relacionados
- **BDD relacionado**: cenários que especificam comportamento desta regra
- **Contrato relacionado**: contratos que formalizam esta regra
- **Workflows relacionados**: fluxos que implementam esta regra
- **Status**: Ativo | Substituído | Depreciado

---

## Regras Ativas

> **Estado atual**: nenhuma regra de negócio foi definida ainda. Regras serão registradas à medida que o domínio for definido.

### Template de Regra de Negócio

```markdown
### RN-[número] — [Título]
**Enunciado**: [A regra em linguagem de negócio clara]
**Condição**: Quando [condição]
**Ação**: [O que deve acontecer]
**Exceções**: [Casos que não seguem a regra, se houver]
**Dependências**: [Outras regras ou invariantes]
**BDD relacionado**: [Referência a cenários BDD]
**Contrato relacionado**: [Referência a contratos]
**Workflows relacionados**: [Referência a fluxos]
**Status**: Ativo
```

---

## Regras Substituídas ou Depreciadas

> Nenhuma regra substituída ou depreciada no momento.

---

## Dúvidas Abertas sobre Regras de Negócio

> Ver `open-questions.md` para dúvidas abertas relacionadas a regras de negócio.

---

## Referências Cruzadas

- `Instructions/business/invariants.md` — condições que nunca podem ser violadas
- `Instructions/business/workflows.md` — fluxos que implementam as regras
- `Instructions/business/domain-model.md` — entidades às quais as regras se aplicam
- `Instructions/bdd/` — cenários que especificam o comportamento das regras
- `Instructions/contracts/` — contratos que formalizam as regras

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| Bootstrap | Estrutura criada sem regras específicas | — |
