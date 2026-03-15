# Invariantes de Negócio

## Propósito

Este arquivo registra os invariantes de negócio deste repositório. Invariantes são condições que **nunca podem ser violadas**, independentemente da operação, estado ou fluxo em execução.

**Invariantes são mais fortes que regras de negócio.** Uma regra de negócio pode ter exceções. Um invariante não tem exceções — é uma pré-condição absoluta do sistema.

---

## O Que É Um Invariante

Um invariante é uma afirmação que deve ser **sempre verdadeira** antes e depois de qualquer operação que afete o objeto ou contexto relacionado.

**Exemplos genéricos**:
- "Um pedido nunca pode ter valor total negativo"
- "Uma conta de usuário nunca pode existir sem identificador único"
- "Um item de estoque nunca pode ter quantidade menor que zero"

---

## Como Ler Este Arquivo

Cada invariante segue a estrutura:
- **Id**: identificador único (INV-NNN)
- **Enunciado**: a condição que nunca pode ser violada
- **Justificativa**: por que esta condição é absoluta
- **Escopo**: qual entidade, agregado ou contexto este invariante governa
- **Consequência de violação**: o que deve acontecer se a violação for detectada
- **Verificação**: como o sistema deve verificar este invariante
- **Regras relacionadas**: regras de negócio que dependem deste invariante
- **Status**: Ativo | Substituído | Depreciado

---

## Invariantes Ativos

> **Estado atual**: nenhum invariante específico do domínio foi definido ainda.
> Invariantes serão adicionados aqui à medida que o domínio for definido.

### Template de Invariante

```markdown
### INV-[número] — [Título]
**Enunciado**: [A condição que nunca pode ser violada]
**Justificativa**: [Por que é uma condição absoluta]
**Escopo**: [Entidade, agregado ou contexto]
**Consequência de violação**: [O que acontece se for violado]
**Verificação**: [Como verificar que o invariante é mantido]
**Regras relacionadas**: [RN-NNN, ...]
**Status**: Ativo
```

---

## Invariantes Substituídos ou Depreciados

> Nenhum invariante substituído no momento.

---

## Relação com Regras de Negócio

| Invariante | Regras que dependem dele |
|---|---|
| — | — |

---

## Ambiguidades sobre Invariantes

> Ver `open-questions.md` para dúvidas abertas relacionadas a invariantes.

---

## Referências Cruzadas

- `Instructions/business/business-rules.md` — regras que dependem dos invariantes
- `Instructions/business/domain-model.md` — entidades que têm estes invariantes
- `Instructions/bdd/` — cenários que testam violações de invariantes

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| Bootstrap | Estrutura criada sem invariantes específicos | — |
