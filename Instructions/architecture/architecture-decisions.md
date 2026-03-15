# Decisões Arquiteturais

## Propósito

Este arquivo mantém um registro de alto nível das decisões arquiteturais mais importantes deste repositório. Cada decisão significativa deve ter um ADR completo em `Instructions/decisions/`.

---

## Decisões Ativas

> **Estado atual**: as decisões abaixo são as do bootstrap do sistema de governança.

### DA-001 — Adoção de Sistema de Governança Persistente
**Data**: Bootstrap
**Status**: Ativo
**Decisão**: Este repositório usa um sistema de governança persistente em linguagem natural que serve como sistema operacional de todas as interações futuras.
**Motivação**: Garantir consistência, preservar conhecimento durável e eliminar a necessidade de reapresentar contexto a cada sessão.
**ADR**: Ver `Instructions/decisions/` quando criado.

### DA-002 — Separação entre Governança Técnica e de Negócio
**Data**: Bootstrap
**Status**: Ativo
**Decisão**: Instruções técnicas ficam em `Instructions/architecture/` e instruções de negócio ficam em `Instructions/business/`. Os dois domínios não se mesclam nos mesmos arquivos.
**Motivação**: Separar responsabilidades facilita navegação, manutenção e propagação de mudanças sem contaminação entre domínios.

### DA-003 — Governança Antes de Implementação
**Data**: Bootstrap
**Status**: Ativo
**Decisão**: Toda definição durável deve ser persistida na governança antes de qualquer mudança de código ou artefato.
**Motivação**: Implementação sem governança cria código sem memória de intenção.

---

## Decisões Pendentes

> Decisões que devem ser tomadas nas próximas interações.

| Id | Decisão Necessária | Impacto |
|---|---|---|
| DP-001 | Escolha da stack tecnológica principal | Alto |
| DP-002 | Estilo arquitetural principal | Alto |
| DP-003 | Estratégia de persistência | Médio-Alto |
| DP-004 | Estratégia de mensageria (se aplicável) | Médio |
| DP-005 | Estratégia de testes | Médio |

---

## Template para Nova Decisão

Ao adicionar uma nova decisão:
```
### DA-[número] — [Título da Decisão]
**Data**: [data]
**Status**: Ativo | Substituído por DA-[n] | Depreciado
**Decisão**: [o que foi decidido]
**Motivação**: [por que foi decidido assim]
**Alternativas consideradas**: [outras opções avaliadas]
**Trade-offs**: [o que se perde com esta decisão]
**ADR completo**: Instructions/decisions/[nome-do-adr].md
```

---

## Referências Cruzadas

- `Instructions/decisions/` — ADRs completos
- `Instructions/architecture/engineering-principles.md` — princípios que motivam as decisões
- `Instructions/architecture/technical-overview.md` — visão geral que reflete estas decisões

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| Bootstrap | DA-001, DA-002, DA-003 criadas | — |
