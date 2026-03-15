# Decisões Arquiteturais (ADRs)

## Propósito

Esta pasta contém os registros de decisão arquitetural (Architecture Decision Records — ADRs) deste repositório.

ADRs preservam o raciocínio por trás de decisões importantes. Eles respondem não apenas "o que foi decidido" mas "por que foi decidido assim" e "quais alternativas foram consideradas".

---

## Quando Registrar uma Decisão

Registre uma ADR quando:
- Uma decisão tecnológica relevante for tomada (linguagem, framework, banco, broker, etc.)
- Uma decisão arquitetural estruturante for tomada (estilo arquitetural, padrão de integração, etc.)
- Uma restrição técnica importante for estabelecida
- Uma alternativa tiver sido descartada e a razão dever ser preservada
- Uma mudança significativa de direção técnica for feita
- Um conflito entre abordagens tiver sido resolvido com impacto futuro

**Não registre ADR para**:
- Escolhas de implementação triviais e reversíveis
- Preferências pessoais de estilo sem impacto arquitetural
- Decisões totalmente reversíveis sem custo significativo

---

## Como Registrar Contexto, Decisão, Trade-off e Consequência

### Contexto
Descreva a situação que levou à necessidade da decisão:
- Qual problema precisava ser resolvido?
- Quais eram as restrições e forças em jogo?
- O que tornava este problema difícil?

### Decisão
Descreva o que foi decidido:
- Formulação clara e afirmativa: "Decidimos usar X para Y"
- Sem ambiguidade sobre o que está sendo decidido

### Alternativas Consideradas
Liste as outras opções que foram avaliadas:
- O que foram as alternativas reais consideradas?
- Por que cada uma foi descartada?

### Trade-offs
Seja honesto sobre os custos da decisão:
- O que esta decisão compromete ou sacrifica?
- Quais são as desvantagens conhecidas?
- O que seria necessário rever se o contexto mudar?

### Consequências
Descreva o impacto da decisão:
- Quais outros artefatos são afetados?
- Quais restrições isso impõe no futuro?
- O que fica mais fácil ou mais difícil com esta decisão?

---

## Como Decisões se Relacionam com Arquitetura e Negócio

- Decisões de **arquitetura técnica** → impacto em `Instructions/architecture/`
- Decisões motivadas por **negócio** → documentar a motivação de negócio no ADR
- Decisões que **resolvem conflito** entre fontes de verdade → documentar a resolução
- Decisões de **nomenclatura ou estrutura** → atualizar arquivos correspondentes

---

## Status de ADRs

Cada ADR deve ter um status:
- **Proposto**: em análise, ainda não decidido
- **Ativo**: decisão em vigor
- **Substituído**: superado por ADR mais recente (referenciar o substituto)
- **Depreciado**: não mais aplicável mas não substituído por outro

**ADRs substituídos não devem ser deletados** — eles documentam por que mudamos de direção.

---

## Estrutura de Arquivos

```
decisions/
├── README.md               # (este arquivo)
├── adr-template.md         # Template para novos ADRs
└── adr-[número]-[título].md  # ADRs individuais (a criar)
```

---

## Referências Cruzadas

- `Instructions/architecture/architecture-decisions.md` — sumário de decisões de alto nível
- `Instructions/architecture/engineering-principles.md` — princípios que motivam decisões
- `.claude/rules/architecture-governance.md` — rule que governa o registro de decisões
