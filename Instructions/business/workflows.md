# Fluxos de Negócio

## Propósito

Este arquivo registra os fluxos de negócio deste repositório. Fluxos descrevem sequências de passos que atores executam para atingir objetivos de negócio.

---

## O Que É Um Fluxo de Negócio

Um fluxo de negócio descreve:
- **Quem** faz (ator ou sistema)
- **O que** faz (sequência de passos)
- **Quando** começa (pré-condições)
- **Quando** termina com sucesso (pós-condições)
- **O que pode dar errado** (exceções e fluxos alternativos)

---

## Como Ler Este Arquivo

Cada fluxo segue a estrutura:
- **Id**: identificador único (WF-NNN)
- **Nome**: nome do fluxo em linguagem de negócio
- **Ator principal**: quem inicia o fluxo
- **Atores secundários**: outros envolvidos
- **Pré-condições**: o que deve ser verdade para o fluxo começar
- **Passos principais**: sequência do fluxo feliz
- **Pós-condições**: o que é verdade quando o fluxo termina com sucesso
- **Fluxos alternativos**: variações do fluxo principal
- **Exceções**: situações que interrompem o fluxo
- **Regras relacionadas**: regras de negócio que governam este fluxo
- **BDD relacionado**: cenários que especificam este fluxo
- **Contratos relacionados**: contratos usados neste fluxo
- **Status**: Ativo | Substituído | Depreciado

---

## Fluxos Ativos

> **Estado atual**: nenhum fluxo de negócio específico foi definido ainda.
> Fluxos serão registrados à medida que o domínio for definido.

### Template de Fluxo

```markdown
### WF-[número] — [Nome do Fluxo]
**Ator principal**: [quem inicia]
**Atores secundários**: [outros envolvidos]
**Pré-condições**:
  - [condição 1]
  - [condição 2]
**Passos principais**:
  1. [passo 1]
  2. [passo 2]
  3. [passo 3]
**Pós-condições**:
  - [resultado 1]
  - [resultado 2]
**Fluxos alternativos**:
  - FA-01: [quando X acontece, fazer Y]
**Exceções**:
  - EX-01: [quando Z acontece, o fluxo termina com erro]
**Regras relacionadas**: [RN-NNN, ...]
**BDD relacionado**: [referência a cenários]
**Contratos relacionados**: [referência a contratos]
**Status**: Ativo
```

---

## Mapa de Fluxos (quando houver)

> **Pendente de definição.** Um mapa de como os fluxos se relacionam será criado quando os fluxos principais forem definidos.

---

## Referências Cruzadas

- `Instructions/business/business-rules.md` — regras que governam os fluxos
- `Instructions/business/domain-model.md` — entidades envolvidas nos fluxos
- `Instructions/bdd/` — cenários que especificam os fluxos

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| Bootstrap | Estrutura criada sem fluxos específicos | — |
