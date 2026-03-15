# Convenções de Nomenclatura

## Propósito

Este arquivo registra todas as convenções de nomenclatura deste repositório — técnicas e de domínio. Convenções registradas aqui são **obrigatórias** e devem ser aplicadas consistentemente em código, contratos, BDD, documentação e qualquer outro artefato.

---

## Princípio Fundamental

A **terminologia de domínio** (ubiquitous language) prevalece sobre convenções técnicas na camada de domínio.
Convenções técnicas de casing (PascalCase, snake_case, etc.) são aplicadas respeitando a terminologia de domínio.

Todo código deve ser escrito em **inglês** — nomes de classes, métodos, variáveis, arquivos, pastas, contratos e comentários técnicos.

*Ver conflitos em: `.claude/rules/naming-governance.md` e `.claude/rules/source-of-truth-priority.md`*

---

## Convenções por Tipo de Artefato

| Tipo de Artefato | Convenção | Exemplo |
|---|---|---|
| Classes/tipos/records | PascalCase | `TodoItemEntity`, `TodoItemsGetAllOutput` |
| Interfaces | PascalCase com prefixo `I` | `ITodoItemsGetAllRepository` |
| Métodos | PascalCase | `GetAllAsync`, `ExecuteAsync` |
| Variáveis locais | camelCase com `var`; autoexplicativas | `var todoItems`, `var input` |
| Parâmetros de método | camelCase; autoexplicativos | `useCase`, `logger`, `repository` |
| Constantes | PascalCase ou SCREAMING_SNAKE_CASE (conforme convenção .NET) | `MaxRetryCount`, `MAX_RETRY_COUNT` |
| Arquivos de código C# | PascalCase; nome igual ao tipo principal do arquivo | `TodoItemsGetAllUseCase.cs` |
| Pastas de Feature | PascalCase; nome da Feature (ver regras abaixo) | `TodoItemsGetAll/` |
| Pastas de subcomponente de Slice | PascalCase; `<NomeDaFeature><Tipo>` | `TodoItemsGetAllUseCase/`, `TodoItemsGetAllRepository/` |
| Scripts SQL | PascalCase; nome da Feature | `TodoItemsGetAll.sql` |
| Rotas de API | kebab-case; plural para coleções | `/todo-items`, `/todo-items/{id}` |
| Campos de contrato (JSON) | camelCase | `"todoItemId"`, `"createdAt"` |
| Variáveis de ambiente | SCREAMING_SNAKE_CASE | `DATABASE_CONNECTION_STRING` |
| Tabelas de banco | A definir | — |
| Colunas de banco | A definir | — |
| Tópicos de mensageria | A definir | — |
| Filas | A definir | — |

---

## Nomenclatura de Features (Slices)

O nome da Feature (Slice) é o contrato semântico do caso de uso e deve ser tratado como parte da API interna do sistema. Deve ser **explícito, semântico e orientado à intenção do caso de uso**.

### Regras de nomenclatura de Feature:

1. **Usar PascalCase** — sem separadores, hífens ou underscores.
2. **Entidade no singular** para operações sobre um único item.
3. **Entidade no plural** para operações sobre coleções.
4. **Ação** deve ser explícita e refletir a intenção funcional.
5. O nome deve permitir inferir: **entidade + ação + cardinalidade**.

### Estrutura do nome:

```
<Entidade>[<Atributo>]<Ação>
```

Onde `<Atributo>` é opcional e representa uma qualificação ou sub-recurso.

### Exemplos válidos:

| Feature | Entidade | Ação | Cardinalidade |
|---|---|---|---|
| `TodoItemsGetAll` | TodoItem | Get | Plural (coleção) |
| `TodoItemGetById` | TodoItem | Get | Singular (por id) |
| `TodoItemInsert` | TodoItem | Insert | Singular |
| `TodoItemUpdate` | TodoItem | Update | Singular |
| `TodoItemDelete` | TodoItem | Delete | Singular |
| `CustomerAddressUpdate` | CustomerAddress | Update | Singular |
| `ProductsByCategoriGetAll` | ProductsByCategory | Get | Plural |

### Anti-exemplos (proibidos):

| Nome Proibido | Motivo |
|---|---|
| `GetTodoItems` | Ação antes da entidade — inverter para `TodoItemsGetAll` |
| `TodoItemService` | Genérico; sem ação e cardinalidade explícitas |
| `TodoController` | Não reflete a Slice; usa terminologia de camada |
| `todo-items-get-all` | kebab-case; usar PascalCase |
| `TodoItemsGet` | Ação incompleta; especificar: `GetAll`, `GetById` |

---

## Nomenclatura de Subcomponentes de Slice

Cada subcomponente de uma Slice usa o nome da Feature como prefixo, seguido pelo tipo do componente:

| Componente | Padrão | Exemplo |
|---|---|---|
| Pasta do componente | `<NomeDaFeature><Tipo>/` | `TodoItemsGetAllUseCase/` |
| Arquivo de classe | `<NomeDaFeature><Tipo>.cs` | `TodoItemsGetAllUseCase.cs` |
| Classe | `<NomeDaFeature><Tipo>` | `TodoItemsGetAllUseCase` |
| Interface de repositório | `I<NomeDaFeature>Repository` | `ITodoItemsGetAllRepository` |
| Input | `<NomeDaFeature>Input` | `TodoItemInsertInput` |
| Output | `<NomeDaFeature>Output` | `TodoItemsGetAllOutput` |
| Entity | `<NomeDaFeature>Entity` | `TodoItemEntity` |
| Script SQL | `<NomeDaFeature>.sql` | `TodoItemsGetAll.sql` |

---

## Terminologia de Domínio

A terminologia de domínio é definida no glossário:
`Instructions/glossary/ubiquitous-language.md`

**Regra**: nenhum nome de conceito de domínio pode divergir da definição do glossário sem alteração explícita do glossário primeiro.

---

## Prefixos e Sufixos Padronizados

| Tipo | Sufixo/Prefixo | Exemplo |
|---|---|---|
| Interface | Prefixo `I` | `ITodoItemsGetAllRepository` |
| Use Case | Sufixo `UseCase` | `TodoItemsGetAllUseCase` |
| Repository | Sufixo `Repository` | `TodoItemsGetAllRepository` |
| Endpoint | Sufixo `Endpoint` | `TodoItemsGetAllEndpoint` |
| Input model | Sufixo `Input` | `TodoItemInsertInput` |
| Output model | Sufixo `Output` | `TodoItemsGetAllOutput` |
| Entity | Sufixo `Entity` | `TodoItemEntity` |

---

## Namespaces de Slice

O namespace de todos os componentes de uma Slice (Endpoint, UseCase, Repository, Interfaces, Models) é o namespace da **Feature**, não o da subpasta do componente.

```
Pasta física:   Features/Query/TestGet/TestGetUseCase/TestGetUseCase.cs
Namespace:      ClaudeDotNetPlayground.Features.Query.TestGet   ← para na Feature
Proibido:       ClaudeDotNetPlayground.Features.Query.TestGet.TestGetUseCase
```

**Motivação**: o sufixo da subpasta (ex: `TestGetUseCase`) coincide com o nome da classe, criando colisão `Namespace.Tipo`. A convenção de namespace parar na Feature elimina essa colisão estruturalmente.

**Consequência**: componentes da mesma Slice compartilham o namespace da Feature e se enxergam diretamente, sem necessidade de `using` adicional.

---

## Restrições de Código

- **Proibido usar `using` alias para tipos** — aliases mascaram a origem dos tipos e dificultam rastreabilidade. Ex: `using UseCase = Some.Namespace.MyClass;` é proibido.
- Se uma colisão de nomes exigir alias, a causa raiz deve ser corrigida (renomear, reorganizar namespace ou reorganizar estrutura).

---

## Abreviações Permitidas

> **Pendente de definição.** Abreviações aceitas no repositório serão listadas aqui quando estabelecidas.

Regra: só usar abreviações registradas aqui. Sem abreviações não documentadas.

---

## Termos Proibidos

| Termo | Motivo | Alternativa |
|---|---|---|
| `manager` | Genérico sem contexto | Usar nome da ação específica |
| `helper` | Genérico sem contexto | Usar nome da responsabilidade específica |
| `util` / `utils` | Genérico sem contexto | Usar nome da responsabilidade específica |
| `data` | Ambíguo | Usar nome do conceito de domínio |
| `info` | Ambíguo | Usar nome do conceito de domínio |
| `Service` como sufixo de Slice | Não reflete a intenção da Slice | Usar `UseCase` para orquestração |

---

## Referências Cruzadas

- `Instructions/glossary/ubiquitous-language.md` — fonte de terminologia de domínio
- `Instructions/architecture/patterns.md` — padrões que usam estas convenções
- `Instructions/architecture/folder-structure.md` — nomes de pastas e módulos

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| Bootstrap | Estrutura criada sem convenções específicas | — |
| 2026-03-15 | Convenções definidas: linguagem (inglês), Feature naming, subcomponentes de Slice, prefixos/sufixos, termos proibidos | Instruções do usuário |
| 2026-03-15 | Adicionado: namespace de Slice para na Feature (não na subpasta); proibido `using` alias para tipos | Instrução do usuário |
