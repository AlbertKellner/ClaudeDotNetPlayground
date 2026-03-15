# Regra: Ingestão de Definições de Negócio

## Propósito

Esta rule define o workflow que o assistente deve seguir quando o usuário introduz ou altera qualquer definição de negócio durável — seja uma regra, invariante, fluxo, conceito de domínio, comportamento esperado ou cenário comportamental.

---

## O Que É Uma Entrada de Negócio

Entrada de negócio é qualquer mensagem do usuário que introduza ou altere:
- Regras de negócio (o que o sistema deve fazer ou não fazer)
- Invariantes (condições que nunca podem ser violadas)
- Fluxos de negócio (sequência de passos para atingir um objetivo)
- Conceitos de domínio (entidades, agregados, eventos de domínio, value objects)
- Comportamentos esperados do sistema
- Restrições de negócio
- Exceções a regras existentes
- Terminologia de domínio
- Relacionamentos entre entidades de domínio

---

## Workflow de Classificação de Entrada de Negócio

```
1. Identificar o tipo de entrada de negócio:
   ├── Regra de negócio → Instructions/business/business-rules.md
   ├── Invariante → Instructions/business/invariants.md
   ├── Fluxo → Instructions/business/workflows.md
   ├── Conceito de domínio → Instructions/business/domain-model.md
   ├── Premissa de negócio → Instructions/business/assumptions.md
   ├── Cenário comportamental → Instructions/bdd/ (quando BDD for relevante)
   └── Contrato de interface → Instructions/contracts/ (quando formalização for necessária)

2. Normalizar a linguagem solta do usuário para linguagem técnica de negócio clara:
   ├── Identificar entidades envolvidas
   ├── Identificar ações e verbos de domínio
   ├── Identificar condições (pré-condições, pós-condições, exceções)
   └── Usar terminologia do glossário existente quando disponível

3. Verificar se o conteúdo conflita com definições de negócio existentes:
   ├── Sem conflito → registrar e processar
   ├── Complementa existente → registrar como adição/exceção com referência cruzada
   └── Conflita diretamente → registrar em open-questions.md, reportar e aguardar confirmação

4. Verificar dependências:
   ├── Esta regra depende de outras regras?
   ├── Outras regras dependem desta?
   ├── Há contratos que devem ser criados ou atualizados?
   └── Há cenários BDD que devem ser criados ou atualizados?

5. Atualizar a governança de negócio antes de implementar
6. Propagar impactos para artefatos relacionados quando necessário
```

---

## Normalização de Linguagem Solta para Linguagem de Negócio

Quando o usuário descrever comportamento em linguagem informal:

| Linguagem do Usuário | Forma Normalizada |
|---|---|
| "se o cara não pagou" | "quando o pagamento do cliente estiver com status vencido" |
| "não pode salvar sem nome" | "o campo nome é obrigatório para persistência da entidade" |
| "atualiza o status pra confirmado quando chegar" | "o status da entidade é atualizado para CONFIRMADO quando o evento de chegada é processado" |
| "não deixa deletar se tiver filho" | "a exclusão da entidade pai é proibida quando existirem entidades filhas associadas" |

---

## Regras de Registro por Arquivo

### `business-rules.md`
- Regras que descrevem o que o sistema deve ou não deve fazer
- Cada regra deve ter: id, descrição, condição, ação, exceções, dependências
- Regras devem ser expressas de forma condicional clara: "Quando X, então Y"

### `invariants.md`
- Condições que **nunca podem ser violadas**, independentemente da operação
- Cada invariante deve ter: id, enunciado, justificativa, escopo, consequência de violação

### `workflows.md`
- Sequências de passos para atingir objetivos de negócio
- Cada fluxo deve ter: nome, ator, pré-condições, passos, pós-condições, exceções e fluxos alternativos

### `domain-model.md`
- Entidades, agregados, value objects, serviços de domínio e eventos de domínio
- Cada conceito deve ter: nome canônico, definição, atributos relevantes, relacionamentos, invariantes associados

### `assumptions.md`
- Premissas de negócio adotadas que ainda não foram confirmadas formalmente
- Cada premissa deve ter: descrição, motivo, escopo, risco e status de confirmação

---

## Política para BDD e Contratos

- BDD **não é obrigatório** para toda definição de negócio
- Usar BDD quando o comportamento se beneficiar de especificação por cenários formais
- Usar contratos quando interfaces, payloads, mensagens ou integrações exigirem formalização
- **Não forçar** toda definição de negócio a gerar artefatos de BDD ou contrato
- Só criar ou atualizar esses artefatos quando forem relevantes para a mudança ou necessários para consistência

---

## Propagação de Mudanças de Negócio

Quando uma regra de negócio for criada ou alterada, verificar:
- Há cenários BDD relacionados que devem ser atualizados?
- Há contratos que dependem dessa regra?
- Há termos de domínio que devem ser adicionados ao glossário?
- Há workflows que mencionam essa regra?
- Há invariantes relacionados?
- Há implementação que precisa ser atualizada para refletir a nova regra?

---

## Relação com Outras Rules

- `natural-language-normalization.md` vem antes desta rule
- `architecture-governance.md` é separada — não misturar governança de negócio com técnica
- `source-of-truth-priority.md` define que regras de negócio prevalecem sobre preferências arquiteturais
- `change-propagation.md` é ativada após atualizar definições de negócio
- `ambiguity-handling.md` é ativada quando a entrada de negócio for ambígua ou incompleta
