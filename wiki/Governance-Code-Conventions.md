# Convenções de Código

## Descrição

Documenta todas as convenções de código obrigatórias: nomenclatura (PascalCase para classes, camelCase para variáveis, kebab-case para rotas), file-scoped namespaces (P007), declaração implícita com var (P008), padrão de logging estruturado SNP-001, código sempre em inglês. Deve ser consultado ao escrever qualquer código novo.

## Contexto

As convenções aqui registradas são **obrigatórias** e devem ser aplicadas consistentemente em código, contratos, BDD, documentação e qualquer outro artefato. Convenções técnicas são subordinadas à terminologia de negócio quando há conflito na camada de domínio.

---

## Regra de Idioma

- **Código**: sempre em inglês — nomes de classes, métodos, variáveis, arquivos, pastas, contratos, comentários técnicos
- **Respostas do agente**: sempre em português
- **Pull Requests**: sempre em português brasileiro (título, descrição e corpo)

---

## Convenções de Nomenclatura por Tipo de Artefato

| Tipo de Artefato | Convenção | Exemplo |
|---|---|---|
| Classes / tipos / records | PascalCase | `TodoItemEntity`, `TodoItemsGetAllOutput` |
| Interfaces | PascalCase com prefixo `I` | `ITodoItemsGetAllRepository` |
| Métodos | PascalCase | `GetAllAsync`, `ExecuteAsync` |
| Variáveis locais | camelCase com `var`; autoexplicativas | `var todoItems`, `var input` |
| Parâmetros de método | camelCase; autoexplicativos | `useCase`, `logger`, `repository` |
| Constantes | PascalCase ou SCREAMING_SNAKE_CASE | `MaxRetryCount`, `MAX_RETRY_COUNT` |
| Arquivos de código C# | PascalCase; nome igual ao tipo principal | `TodoItemsGetAllUseCase.cs` |
| Pastas de Feature | PascalCase; nome da Feature | `TodoItemsGetAll/` |
| Pastas de subcomponente | PascalCase; `<Feature><Tipo>` | `TodoItemsGetAllUseCase/` |
| Rotas de API | kebab-case; plural para coleções | `/todo-items`, `/todo-items/{id}` |
| Campos de contrato (JSON) | camelCase | `"todoItemId"`, `"createdAt"` |
| Variáveis de ambiente | SCREAMING_SNAKE_CASE | `DATABASE_CONNECTION_STRING` |

---

## Nomenclatura de Features (Slices)

O nome da Feature segue a estrutura:

```
<Entidade>[<Atributo>]<Ação>
```

### Regras:
1. Usar PascalCase — sem separadores, hífens ou underscores
2. Entidade no singular para operações sobre um único item
3. Entidade no plural para operações sobre coleções
4. Ação explícita refletindo a intenção funcional
5. O nome deve permitir inferir: **entidade + ação + cardinalidade**

### Exemplos válidos:

| Feature | Entidade | Ação | Cardinalidade |
|---|---|---|---|
| `TodoItemsGetAll` | TodoItem | Get | Plural (coleção) |
| `TodoItemGetById` | TodoItem | Get | Singular (por id) |
| `TodoItemInsert` | TodoItem | Insert | Singular |
| `PokemonGet` | Pokemon | Get | Singular |

### Nomes proibidos:

| Nome Proibido | Motivo |
|---|---|
| `GetTodoItems` | Ação antes da entidade |
| `TodoItemService` | Genérico; sem ação e cardinalidade |
| `TodoController` | Terminologia de camada |
| `todo-items-get-all` | kebab-case; usar PascalCase |

---

## Nomenclatura de Subcomponentes de Slice

| Componente | Padrão | Exemplo |
|---|---|---|
| Pasta do componente | `<Feature><Tipo>/` | `TodoItemsGetAllUseCase/` |
| Arquivo de classe | `<Feature><Tipo>.cs` | `TodoItemsGetAllUseCase.cs` |
| Interface de repositório | `I<Feature>Repository` | `ITodoItemsGetAllRepository` |
| Input | `<Feature>Input` | `TodoItemInsertInput` |
| Output | `<Feature>Output` | `TodoItemsGetAllOutput` |
| Entity | `<Feature>Entity` | `TodoItemEntity` |

---

## File-Scoped Namespace (P007)

Toda declaração de namespace em classes C# deve seguir o padrão de file-scoped namespace:

```csharp
// Correto
namespace DotNetPlayground.Features.Query.TodoItemsGetAll;

public class TodoItemsGetAllUseCase { }
```

```csharp
// Proibido
namespace DotNetPlayground.Features.Query.TodoItemsGetAll
{
    public class TodoItemsGetAllUseCase { }
}
```

---

## Declaração Implícita de Variáveis — var (P008)

Usar `var` sempre que possível. O nome da variável deve ser autoexplicativo:

```csharp
// Correto
var todoItems = await repository.GetAllAsync();
var input = context.Request.ReadFromJsonAsync<TodoItemInsertInput>();
```

```csharp
// Evitar (quando o tipo é inferível)
List<TodoItemEntity> todoItems = await repository.GetAllAsync();
```

---

## Convenção de Namespace

O namespace de todos os componentes de uma Slice para no nível da **Feature**, não na subpasta do componente:

```
Pasta física:   Features/Query/TestGet/TestGetUseCase/TestGetUseCase.cs
Namespace:      Albert.Playground.ECS.AOT.Api.Features.Query.TestGet   ← para na Feature
Proibido:       Albert.Playground.ECS.AOT.Api.Features.Query.TestGet.TestGetUseCase
```

Componentes da mesma Slice compartilham o namespace da Feature e se enxergam diretamente, sem necessidade de `using` adicional.

**Restrição adicional**: proibido usar `using` alias para tipos (ex: `using UseCase = Some.Namespace.MyClass;`).

---

## Padrão de Logging Estruturado — SNP-001

### Formato de prefixo (obrigatório)

```
[NomeDaClasse][NomeDoMétodo] DescriçãoBreveFrase
```

### Regras de escrita de logs

1. **Prefixo obrigatório**: todo log deve começar com `[NomeDaClasse][NomeDoMétodo]`
2. **Linguagem imperativa**: descrição breve, objetiva e no imperativo (ex: "Processar requisição", "Retornar resposta")
3. **Log de entrada do método**: primeiro log informa o que será executado e registra os objetos recebidos
4. **Log de saída do método**: último log antes do `return` informa o que está sendo retornado
5. **Log antes de loops**: antes de qualquer `for`, `foreach` ou iteração, log informando o que será iterado
6. **Log após loops**: na linha após o encerramento do loop, log informando que a iteração foi concluída
7. **Isolamento visual**: toda instrução `logger.Log*()` deve ter uma linha em branco acima e uma abaixo no código

### Template de console colorido

```csharp
.WriteTo.Console(
    theme: AnsiConsoleTheme.Code,
    outputTemplate: "[{Timestamp:dd/MM/yyyy HH:mm:ss.fffffff}] [{CorrelationId}] [{UserName}] {Message:lj}{NewLine}{Exception}")
```

### Padrão de logs em Program.cs

- Log inicial informando que a aplicação está sendo iniciada
- Um log por bloco lógico (Serilog, DI de infraestrutura, DI de features, DI de segurança, pipeline de middlewares)
- Não criar log por instrução individual dentro do bloco

---

## Termos Proibidos

| Termo | Motivo | Alternativa |
|---|---|---|
| `manager` | Genérico sem contexto | Nome da ação específica |
| `helper` | Genérico sem contexto | Nome da responsabilidade específica |
| `util` / `utils` | Genérico sem contexto | Nome da responsabilidade específica |
| `data` | Ambíguo | Nome do conceito de domínio |
| `info` | Ambíguo | Nome do conceito de domínio |
| `Service` como sufixo de Slice | Não reflete a intenção da Slice | `UseCase` para orquestração |

---

## Referências

- [Padrões de Desenvolvimento](Governance-Development-Patterns) — padrões que usam estas convenções
- [Arquitetura](Governance-Architecture) — visão geral da estrutura do projeto
