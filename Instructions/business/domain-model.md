# Modelo de Domínio

## Propósito

Este arquivo registra o modelo de domínio deste repositório — entidades, agregados, value objects, eventos de domínio e serviços de domínio. É a representação estruturada do conhecimento do negócio.

---

## Estrutura do Modelo

### Entidades
Objetos com identidade própria que persistem ao longo do tempo.
Uma entidade é identificável independentemente de seus atributos.

### Agregados
Grupos de entidades e value objects tratados como uma unidade para fins de consistência.
O agregado tem uma raiz (Aggregate Root) que é a única porta de entrada para modificações.

### Value Objects
Objetos definidos pelos seus atributos, sem identidade própria.
Dois value objects com os mesmos atributos são iguais.

### Eventos de Domínio
Fatos que ocorreram no domínio e têm relevância de negócio.
São imutáveis e descrevem algo que já aconteceu.

### Serviços de Domínio
Operações de negócio que não pertencem naturalmente a nenhuma entidade específica.

---

## Entidades Registradas

> **Estado atual**: nenhuma entidade foi definida ainda.
> Entidades serão registradas à medida que o domínio for definido.

### Template de Entidade

```markdown
### [Nome da Entidade]
**Definição**: [O que é esta entidade no contexto do negócio]
**Identidade**: [Como esta entidade é identificada]
**Atributos relevantes**:
  - [atributo]: [tipo e significado]
**Invariantes**: [INV-NNN que se aplicam]
**Comportamentos**: [operações que esta entidade expõe]
**Relacionamentos**: [outras entidades e como se relacionam]
**Eventos emitidos**: [eventos de domínio que esta entidade emite]
**Regras relacionadas**: [RN-NNN]
**Notas**: [observações importantes]
```

---

## Agregados Registrados

> **Pendente de definição.**

---

## Value Objects Registrados

> **Pendente de definição.**

---

## Eventos de Domínio Registrados

> **Pendente de definição.**

---

## Serviços de Domínio Registrados

> **Pendente de definição.**

---

## Contextos Delimitados (Bounded Contexts)

> **Pendente de definição.** Se o domínio tiver múltiplos bounded contexts, eles serão mapeados aqui.

---

## Linguagem Ubíqua no Modelo

Todos os termos usados neste modelo devem estar definidos em:
`Instructions/glossary/ubiquitous-language.md`

Se um novo termo for introduzido aqui, ele deve ser adicionado ao glossário.

---

## Ambiguidades sobre o Domínio

> Ver `open-questions.md` para dúvidas abertas sobre o modelo de domínio.

---

## Referências Cruzadas

- `Instructions/glossary/ubiquitous-language.md` — terminologia de todos os termos do modelo
- `Instructions/business/business-rules.md` — regras que governam as entidades
- `Instructions/business/invariants.md` — invariantes do modelo
- `Instructions/contracts/` — contratos que expõem o modelo

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| Bootstrap | Estrutura criada sem entidades específicas | — |
