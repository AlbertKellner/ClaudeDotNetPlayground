# Regra: Governança de Estrutura de Pastas

## Propósito

Esta rule define como a estrutura de pastas e módulos deste repositório é definida, documentada, alterada e mantida consistente.

---

## Princípio Fundamental

> A estrutura de pastas é uma decisão arquitetural.
> Toda mudança estrutural relevante deve ser registrada e justificada.
> A estrutura existente reflete decisões tomadas — não deve ser alterada sem intenção explícita.

---

## Como Regras de Estrutura de Pastas São Registradas

A estrutura canônica do repositório é definida em `Instructions/architecture/folder-structure.md`.

Esse arquivo deve conter:
- Mapa da estrutura de pastas e módulos do projeto
- Propósito de cada pasta e módulo
- Regras sobre o que pertence a cada área
- O que **não deve** ser colocado em cada área
- Convenções de nomenclatura de pastas
- Referências a decisões arquiteturais relevantes

---

## Como Mudanças Estruturais São Decididas

Quando o usuário propõe ou implica uma mudança na estrutura de pastas:

1. **Identificar o impacto**:
   - É criação de nova pasta/módulo?
   - É renomeação de pasta/módulo existente?
   - É reorganização de artefatos entre pastas?
   - É remoção de pasta/módulo?

2. **Verificar consistência com a arquitetura**:
   - A mudança é compatível com os princípios registrados?
   - A mudança conflita com padrões existentes?
   - A mudança reflete uma decisão consciente ou é incidental?

3. **Registrar a mudança**:
   - Atualizar `Instructions/architecture/folder-structure.md`
   - Criar ADR em `Instructions/decisions/` se for mudança significativa
   - Atualizar `Instructions/architecture/technical-overview.md` se necessário

4. **Propagar a mudança**:
   - Atualizar referências em outros arquivos de governança
   - Atualizar imports e referências no código se aplicável

---

## Como Evitar Proliferação de Artefatos

### Regras obrigatórias:
- Não criar novas pastas sem justificativa explícita na governança
- Não criar arquivos de governança vazios ou apenas com placeholders sem valor operacional
- Não duplicar estrutura quando o repositório já tem local apropriado
- Não criar subpastas para conter apenas um arquivo (a menos que sejam esperados múltiplos arquivos)
- Não criar arquivos `index` ou `README` vazios apenas para "marcar" uma pasta como existente

### Quando criar nova pasta é justificado:
- O escopo de conteúdo é claramente distinto do que já existe
- Existem ou serão criados múltiplos artefatos desse tipo
- A separação é necessária para clareza arquitetural ou organizacional
- A decisão foi explicitamente tomada pelo usuário

---

## Impacto em Instruções e Implementação

Quando a estrutura de pastas mudar:

| Tipo de Mudança | Impacto Potencial |
|---|---|
| Nova pasta de módulo | `technical-overview.md`, `folder-structure.md`, possivelmente ADR |
| Renomeação de pasta | `folder-structure.md`, `naming-conventions.md`, referências no código, contratos se aplicável |
| Reorganização de módulos | `folder-structure.md`, `technical-overview.md`, possivelmente ADR |
| Remoção de pasta | `folder-structure.md`, verificar se conteúdo foi migrado |
| Nova pasta de governança | `CLAUDE.md` imports, `README.md`, `operating-model.md` |

---

## Estrutura de Governança (Imutável sem Instrução Explícita)

A estrutura de pastas de governança definida neste bootstrap não deve ser alterada sem instrução explícita do usuário:

```
.claude/
  rules/
  skills/
  hooks/
Instructions/
  architecture/
  business/
  bdd/
  contracts/
  glossary/
  decisions/
  snippets/
```

Qualquer adição ou reorganização desta estrutura deve ser:
- Solicitada explicitamente pelo usuário
- Registrada em `Instructions/architecture/folder-structure.md`
- Refletida nos imports de `CLAUDE.md`

---

## Relação com Outras Rules

- `architecture-governance.md` — mudanças estruturais são decisões arquiteturais
- `naming-governance.md` — nomes de pastas seguem as convenções de nomenclatura registradas
- `governance-policies.md` §3 — mudanças estruturais devem ser propagadas para artefatos relacionados
