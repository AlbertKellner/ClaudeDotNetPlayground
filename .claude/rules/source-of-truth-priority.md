# Regra: Prioridade entre Fontes de Verdade

## Propósito

Esta rule define a ordem de prioridade entre fontes de verdade do repositório e como conflitos entre elas devem ser detectados, documentados e resolvidos.

---

## Ordem de Prioridade

Quando houver conflito entre fontes, a seguinte hierarquia deve ser respeitada:

```
1. Contratos executáveis, artefatos formais equivalentes e snippets normativos
   explicitamente declarados como canônicos pelo usuário

2. BDD (cenários comportamentais formalizados)

3. Regras de negócio estruturadas

4. Arquitetura e padrões técnicos

5. Convenções de nomenclatura, estilo e organização
```

---

## Regras de Resolução de Conflito

### Princípios fundamentais

- **Contrato prevalece sobre texto narrativo** — se um contrato formal define um comportamento e um texto narrativo define outro, o contrato é a fonte prevalente.
- **BDD prevalece sobre texto narrativo** — se um cenário BDD formalizado define um comportamento, ele prevalece sobre descrições informais.
- **Comportamento de negócio prevalece sobre preferência arquitetural** — quando uma preferência arquitetural invalidaria o comportamento esperado do sistema, o comportamento de negócio vence.
- **Convenções de nomenclatura, estilo e organização não podem prevalecer** sobre comportamento de negócio, BDD ou contratos.
- **Snippets normativos explicitamente declarados pelo usuário não devem ser reescritos silenciosamente** — mesmo que o assistente considere que há uma "versão melhor".

### Conflitos nunca devem ser ignorados

Todo conflito detectado deve ser:
1. Reportado no prompt de resposta
2. Registrado nos arquivos apropriados do repositório
3. Resolvido seguindo a ordem de prioridade acima
4. Documentado com a interpretação escolhida e o motivo

---

## Workflow de Resolução de Conflito

```
1. Detectar o conflito (entre quais artefatos e qual é a contradição)

2. Identificar as fontes envolvidas

3. Aplicar a hierarquia de prioridade:
   - Fonte de maior prioridade prevalece
   - Se as fontes têm a mesma prioridade, registrar ambiguidade e pedir confirmação

4. Documentar o conflito:
   - Se o conflito for entre artefatos formais (contrato vs. BDD): registrar em open-questions.md
   - Se o conflito for entre artefato formal e texto narrativo: resolver pela prioridade, documentar a decisão
   - Se o conflito for entre preferência arquitetural e regra de negócio: resolver pela prioridade, documentar

5. Reportar no prompt:
   - Qual foi o conflito
   - Qual fonte prevaleceu e por quê
   - O que precisou ser atualizado para refletir a resolução
   - Se houver impacto em implementação, BDD, contratos ou glossário

6. Propagar a resolução:
   - Atualizar os artefatos de menor prioridade para ficarem consistentes com a fonte prevalente
   - Ou registrar o conflito como dúvida aberta se não houver clareza suficiente para resolver
```

---

## Documentação de Conflito

Todo conflito relevante deve ser refletido em um dos seguintes locais:

- **`open-questions.md`**: quando o conflito não pode ser resolvido sem confirmação do usuário
- **`assumptions-log.md`**: quando foi adotada uma premissa para resolver o conflito sem confirmação
- **`Instructions/decisions/`**: quando o conflito foi resolvido e a decisão deve ser preservada como ADR
- **Arquivos definitivos do repositório**: a resolução consolidada deve estar nos artefatos finais, não apenas nos logs de dúvidas

---

## Casos Especiais

### Snippet normativo vs. arquitetura existente
Se um snippet normativo explicitamente declarado pelo usuário conflitar com a arquitetura existente:
- Não substituir silenciosamente a arquitetura
- Registrar o conflito em `open-questions.md`
- Reportar o impacto e as opções possíveis
- Aguardar confirmação do usuário

### Regra de negócio nova vs. contrato existente
Se uma nova regra de negócio introduzida pelo usuário conflitar com um contrato já existente:
- A regra de negócio não automaticamente prevalece — o contrato pode ter dependentes externos
- Registrar o conflito em `open-questions.md`
- Reportar o impacto da mudança no contrato existente
- Aguardar confirmação antes de alterar o contrato

### Convenção de nomenclatura vs. terminologia de negócio
Se uma convenção técnica de nomenclatura conflitar com a terminologia de negócio do domínio:
- A terminologia de negócio (glossário, ubiquitous language) prevalece na camada de domínio
- A convenção técnica pode ser aplicada na camada de infraestrutura/implementação, se devidamente justificada
- Documentar a decisão em `Instructions/decisions/`

---

## Hierarquia entre Rules

A ordem de prioridade acima governa conflitos entre **artefatos de domínio** (contratos, BDD, regras, arquitetura, convenções). Quando duas **rules de governança** conflitam entre si, aplicar os seguintes princípios:

1. **`source-of-truth-priority.md` prevalece sobre todas as demais rules** — esta rule é a autoridade final para resolução de conflitos.
2. **Rule mais específica prevalece sobre rule mais genérica** — ex: `endpoint-validation.md` (específica para endpoints) prevalece sobre `governance-policies.md` (genérica) em matéria de validação de endpoints.
3. **Rule que define política prevalece sobre rule que apenas referencia** — quando rule A define uma política e rule B a referencia, a definição em A é a fonte de verdade.
4. **Em caso de ambiguidade não resolvível**: registrar em `open-questions.md` e aplicar premissa conservadora (menor impacto, mais reversível).

### Subordinações explícitas

| Rule | Subordinada a |
|---|---|
| `governance-policies.md` | `source-of-truth-priority.md` (para resolução de conflitos) |
| `naming-governance.md` | `source-of-truth-priority.md` (quando conflita com negócio ou contratos) |
| `folder-governance.md` | `architecture-governance.md` (para decisões estruturais) |
| `endpoint-validation.md` | `pr-metadata-governance.md` (para políticas de pipeline) |

---

## Relação com Outras Rules

- `governance-policies.md` — políticas de ambiguidade (§4) e propagação (§3) usam esta rule para resolver conflitos; subordinada a esta rule
- `naming-governance.md` — subordinada a esta rule nos casos de conflito com negócio ou contratos
- `architecture-governance.md` — decisões arquiteturais são priorizadas conforme a hierarquia de artefatos definida aqui
