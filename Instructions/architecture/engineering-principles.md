# Princípios de Engenharia

## Propósito

Este arquivo registra os princípios técnicos que devem guiar todas as decisões de implementação neste repositório. Princípios são regras duráveis de design que se aplicam transversalmente.

**Estes princípios são obrigatórios, não sugestivos.** Decisões que os violem devem ser justificadas com ADR.

---

## Princípios Ativos

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

### P006 — Linguagem e Comunicação
- **Código**: sempre em inglês — nomes de classes, métodos, variáveis, arquivos, pastas, contratos, comentários técnicos.
- **Respostas do agente**: sempre em português — toda resposta ao usuário no prompt deve ser em português.
- **Resumo de mudanças**: ao executar qualquer tarefa, incluir na resposta um resumo em português do que foi alterado e a justificativa técnica da decisão tomada.
- **Pull Requests**: todo pull request (título, descrição, comentários e seções do corpo) deve ser escrito em português brasileiro.

### P007 — File-Scoped Namespace
A declaração de namespace em todas as classes C# deve seguir o padrão de **file-scoped namespace**.

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

### P008 — Declaração Implícita de Variáveis (var)
Usar `var` sempre que possível. O nome da variável deve ser autoexplicativo, tornando o tipo inferível pelo contexto.

```csharp
// Correto
var todoItems = await repository.GetAllAsync();
var input = context.Request.ReadFromJsonAsync<TodoItemInsertInput>();
```

```csharp
// Evitar (quando o tipo é inferível pelo nome e contexto)
List<TodoItemEntity> todoItems = await repository.GetAllAsync();
```

### P009 — Princípio da Responsabilidade Única (SRP)
Todo código novo ou alterado deve seguir o princípio de **Responsabilidade Única**: cada classe, método e componente deve ter **um único motivo para mudar**.

**Regras práticas:**

#### Separação de camadas obrigatória:
- **Endpoints/Controllers**: orquestração de request/response, definição de status codes e **escrita de logs relevantes** para elucidar o que está sendo feito durante a request (início/fim, parâmetros relevantes, decisões de fluxo, erros e resultados). Sem lógica de negócio.
- **Use Cases** (dentro das Slices em `Features/`): orquestração da lógica de negócio da Slice. Não acessa infraestrutura diretamente — depende de interfaces.
- **Repositories**: apenas acesso a dados. Responsável por materializar e trabalhar com objetos de domínio tipados a partir de dados brutos (banco, API externa, etc.).
- **Models/DTOs** (`<Feature>Models/`): modelagem de Input e Output especializada por Slice. Validação de payload fica no objeto Input — não em repositórios nem em outros componentes.
- **Shared**: abstrações, utilitários, clientes, helpers e persistência genérica reutilizáveis entre Slices. Sem lógica especializada para uma única Slice. Models de Input e Output de Features não podem residir em `Shared/` — pertencem exclusivamente a `<Feature>Models/` (DA-020). Features também não devem usar models de `Shared/` (incluindo `Shared/ExternalApi/*/Models/`) como tipo de retorno de seus Use Cases ou Endpoints — se a Feature consome uma API externa via Shared, deve possuir seu próprio Output model e mapear os dados internamente (DA-020).

#### Isolamento entre Slices:
- Nenhuma Slice pode se comunicar diretamente com outra Slice.
- Lógica compartilhada entre Slices deve estar em `Shared/`.
- `Shared/` não depende de Features.

#### Anti-padrões a evitar:
- Classes "faz-tudo" que validam, acessam banco, montam DTO, chamam APIs externas e escrevem logs.
- Métodos com múltiplas responsabilidades (validação + persistência + mapeamento + regras de negócio no mesmo método).
- Lógica de negócio misturada com detalhes de framework (ASP.NET, EF Core, serialização).

### P010 — Tratamento de Erros com Responsabilidade Definida
- **Proibido**: blocos `try-catch` genéricos espalhados pela aplicação.
- **Obrigatório**: usar um handler centralizado de captura de erros para exceções genéricas.
- **Permitido**: blocos `try-catch` para exceções bem definidas e específicas, geralmente nas camadas de infraestrutura ou repositórios.

```csharp
// Permitido — exceção específica, em repositório
try
{
    await connection.ExecuteAsync(sql, parameters);
}
catch (SqlException ex) when (ex.Number == 2627)
{
    throw new DuplicateKeyException("Todo item already exists.", ex);
}
```

```csharp
// Proibido — try-catch genérico em use case ou endpoint
try
{
    var result = await useCase.ExecuteAsync(input);
    return Results.Ok(result);
}
catch (Exception ex)
{
    logger.LogError(ex, "Error");
    return Results.StatusCode(500);
}
```

### P011 — Preservação de Formatação e Indentação
- Manter a indentação dos arquivos de acordo com as convenções já existentes neles.
- Não alterar indentação ou espaçamento de código que não foi modificado.
- Ao formatar arquivos `.csproj`, respeitar os níveis de indentação existentes dos itens e subitens.

### P012 — Workflow Obrigatório de Validação Pré-Commit
Antes de qualquer commit, executar obrigatoriamente esta sequência e só prosseguir quando todos os critérios forem satisfeitos:

1. **Build limpo**
```bash
dotnet build
```
Critério: zero erros de compilação.

2. **Execução e HealthCheck**
```bash
dotnet run &
curl http://localhost:5000/health
```
Critério: aplicação sobe sem erro e `/health` retorna `Healthy`.

Se qualquer critério falhar, corrigir o código e repetir a sequência integralmente antes de commitar.

### P014 — Compilação AOT (Ahead-of-Time)
Todo projeto deve ser configurado para publicação com Native AOT (`<PublishAot>true</PublishAot>`).

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
  <InvariantGlobalization>true</InvariantGlobalization>
</PropertyGroup>
```

**Implicações obrigatórias**:
- Todo código novo deve ser AOT-compatível: sem reflection dinâmica não anotada, sem `Assembly.Load`, sem `dynamic`.
- `dotnet build` e `dotnet run` continuam usando JIT — AOT só é ativado em `dotnet publish`.
- Controllers MVC (DA-008) usam reflection e **não são totalmente compatíveis com Native AOT**; essa limitação está registrada em DA-009.

*Referência: DA-009*

### P013 — HealthCheck Obrigatório
Toda aplicação deve expor um endpoint de HealthCheck em `/health`.
O HealthCheck deve responder `Healthy` com HTTP 200 quando a aplicação estiver em operação normal.

```csharp
// Program.cs
builder.Services.AddHealthChecks();
// ...
app.MapHealthChecks("/health");
```

O endpoint `/health` é a verificação canônica de disponibilidade da aplicação e deve ser o critério de aceitação antes de qualquer commit (ver P012).

---

## Restrições Técnicas

- Sem dependências circulares entre módulos.
- Sem lógica de negócio em Endpoints ou Repositories.
- Sem acesso direto ao banco de dados fora da camada de Repository.
- Slices não se comunicam diretamente entre si.
- `Shared/` não depende de Features.
- Validação de payload deve estar no objeto `Input` de cada Slice.
- Namespaces devem seguir o padrão file-scoped.
- Variáveis devem usar `var` sempre que possível.
- Sem blocos `try-catch` genéricos fora de handlers centralizados.

---

## Práticas de Qualidade

- Compilação limpa (`dotnet build`) obrigatória antes de qualquer commit.
- `/health` retornando `Healthy` obrigatório antes de qualquer commit.
- Testes passando obrigatoriamente antes de concluir qualquer tarefa.
- Código sempre em inglês.
- Respostas ao usuário sempre em português com resumo e justificativa técnica.
- Pull requests sempre em português brasileiro (título, descrição e corpo).

---

## Premissas dos Princípios

| Id | Premissa | Risco | Status |
|---|---|---|---|
| PREM-001 | Princípios genéricos são válidos até a stack ser definida | Baixo | Superado — stack definida |

---

## Referências Cruzadas

- `Instructions/architecture/patterns.md` — padrões que implementam estes princípios
- `Instructions/architecture/architecture-decisions.md` — decisões que seguem estes princípios
- `Instructions/architecture/technical-overview.md` — visão geral do projeto

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| Bootstrap | Princípios genéricos criados (P001–P005) | — |
| 2026-03-15 | Princípios P006–P012 adicionados: linguagem, namespace, var, SRP, tratamento de erros, formatação, workflow | Instruções do usuário |
| 2026-03-15 | P012 atualizado: pré-commit inclui execução e verificação de HealthCheck; P013 adicionado: HealthCheck obrigatório em /health | Instrução do usuário |
| 2026-03-15 | P014 adicionado: compilação AOT obrigatória; trade-off com Controllers registrado em DA-009 | Instrução do usuário |
| 2026-03-15 | P006 atualizado: pull requests devem ser escritos em português brasileiro | Instrução do usuário |
| 2026-03-19 | P009 atualizado: restrição explícita — models de Input/Output de Features não podem residir em Shared | DA-020 |
