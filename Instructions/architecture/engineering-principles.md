# Princípios de Engenharia

## Propósito

Este arquivo registra os princípios técnicos que devem guiar todas as decisões de implementação neste repositório. Princípios são regras duráveis de design que se aplicam transversalmente.

**Estes princípios são obrigatórios, não sugestivos.** Decisões que os violem devem ser justificadas com ADR.

---

## Princípios Ativos

> **Estado atual**: os princípios abaixo são genéricos e válidos para qualquer repositório.
> Princípios específicos do projeto serão adicionados à medida que as decisões forem tomadas.

### P001 — Separação de Responsabilidades
Cada módulo, componente ou função deve ter uma responsabilidade única e bem definida.
Responsabilidades de negócio não devem vazar para camadas de infraestrutura e vice-versa.

### P002 — Governança Antes de Implementação
Nenhuma definição durável deve ser implementada sem que a governança do repositório esteja atualizada.
A governança é a memória do projeto; a implementação é a expressão dessa memória.
*Referência: `Instructions/operating-model.md`*

### P003 — Consistência Terminológica
Os mesmos termos de domínio devem ser usados consistentemente em todo o repositório.
Código, contratos, BDD, documentação e mensagens de erro devem compartilhar o mesmo vocabulário.
*Referência: `Instructions/glossary/ubiquitous-language.md`*

### P004 — Decisões Explicadas
Decisões arquiteturais relevantes devem ser documentadas com contexto, alternativas e trade-offs.
"Funciona" não é justificativa suficiente. "Por que funciona desta forma" é o que deve estar registrado.
*Referência: `Instructions/decisions/`*

### P005 — Comportamento de Negócio Prevalece
Preferências arquiteturais não podem invalidar comportamento de negócio exigido.
Quando houver conflito, o comportamento de negócio vence e o trade-off deve ser documentado.
*Referência: `.claude/rules/source-of-truth-priority.md`*

---

## Restrições Técnicas

> **Pendente de definição.** Restrições específicas do projeto serão registradas aqui.

Exemplos de restrições a serem definidas:
- Sem dependências circulares entre módulos
- Sem lógica de negócio em controladores
- Sem acesso direto ao banco de dados fora da camada de repositório
- Limites de tamanho de função/método

---

## Práticas de Qualidade

> **Pendente de definição.** As práticas de qualidade adotadas serão registradas aqui.

Exemplos a serem definidos:
- Cobertura mínima de testes
- Lint obrigatório antes de commit
- Revisão de código obrigatória
- Tipos/contratos estáticos obrigatórios

---

## Premissas dos Princípios

| Id | Premissa | Risco | Status |
|---|---|---|---|
| PREM-001 | Princípios genéricos são válidos até a stack ser definida | Baixo | Ativo |

---

## Referências Cruzadas

- `Instructions/architecture/patterns.md` — padrões que implementam estes princípios
- `Instructions/architecture/architecture-decisions.md` — decisões que seguem estes princípios
- `Instructions/architecture/technical-overview.md` — visão geral do projeto

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| Bootstrap | Princípios genéricos criados | — |
