# Padrões Técnicos

## Propósito

Este arquivo registra os padrões arquiteturais e de design adotados neste repositório. Padrões são abordagens específicas e recorrentes que devem ser seguidas consistentemente.

**Padrões registrados aqui são obrigatórios** para novos artefatos, salvo exceção explicitamente justificada.

---

## Padrões Ativos

---

### PAD-001 — Vertical Slice Architecture

**Contexto**: Toda nova funcionalidade a ser implementada no projeto.

**Problema**: Organização por camadas horizontais globais (Controllers/, Services/, Repositories/) cria acoplamento entre funcionalidades distintas e dificulta o isolamento de mudanças.

**Solução**: Cada funcionalidade é implementada como uma Slice vertical isolada, que contém todos os artefatos necessários para o seu funcionamento: endpoint, use case, interfaces, models, repository e scripts SQL. Slices residem em `Features/Query/` ou `Features/Command/` conforme o tipo de operação.

**Estrutura de uma Slice:**
```
Features
 ├── Query
 │    └── <NomeDaFeature>
 │         ├── <NomeDaFeature>Endpoint/
 │         │    └── <NomeDaFeature>Endpoint.cs
 │         ├── <NomeDaFeature>UseCase/
 │         │    └── <NomeDaFeature>UseCase.cs
 │         ├── <NomeDaFeature>Interfaces/
 │         │    └── I<NomeDaFeature>Repository.cs
 │         ├── <NomeDaFeature>Models/
 │         │    ├── <NomeDaFeature>Input.cs      (quando aplicável)
 │         │    ├── <NomeDaFeature>Output.cs
 │         │    └── <NomeDaFeature>Entity.cs     (quando aplicável)
 │         └── <NomeDaFeature>Repository/
 │              ├── <NomeDaFeature>Repository.cs
 │              └── Scripts/
 │                   └── <NomeDaFeature>.sql
 └── Command
      └── <NomeDaFeature>
           └── (mesma estrutura)
```

**Consequências**:
- Mudanças em uma Slice não afetam outras Slices.
- Toda lógica de uma funcionalidade está em um único lugar.
- Facilita a deleção e teste isolado de uma funcionalidade.
- Maior verbosidade inicial de estrutura de pastas.

**Exceções permitidas**: nenhuma — toda funcionalidade nova deve seguir este padrão.

*Referência: DA-004*

---

### PAD-002 — Segregação Command/Query (CQRS leve)

**Contexto**: Classificação e organização de toda funcionalidade antes da implementação.

**Problema**: Misturar operações de leitura e escrita no mesmo nível dificulta raciocinar sobre side effects, torna mais difícil otimizar leituras independentemente das escritas e obscurece a intenção da operação.

**Solução**: Toda funcionalidade é classificada **antes de ser implementada** como:
- **Query**: operação de leitura — não altera estado. Reside em `Features/Query/`.
- **Command**: operação de escrita — altera estado. Reside em `Features/Command/`.

A classificação deve ser feita com base na **intenção da operação**, não no verbo HTTP.

**Consequências**:
- Intenção da operação é explícita pela localização.
- Leituras e escritas evoluem de forma independente.
- Facilita futuras otimizações (ex.: read replicas, caching de queries).

**Exceções permitidas**: nenhuma — toda Slice deve estar em `Query/` ou `Command/`.

*Referência: DA-005*

---

### PAD-003 — Endpoint como Controller com Action

**Contexto**: Toda exposição de funcionalidade via HTTP.

**Problema**: Lógica de request/response espalhada ou acoplada a outros componentes da Slice.

**Solução**: Cada Slice tem seu próprio Controller localizado na pasta `<Feature>Endpoint/`. O Controller:
- Define a rota via atributos (`[Route]`, `[HttpGet]`, `[HttpPost]`, etc.).
- Contém uma ou mais Actions bem definidas, cada uma correspondendo a uma operação da Slice.
- Orquestra request/response (leitura de body/params/route, retorno de status codes e resultados).
- Escreve logs relevantes: início/fim da request, parâmetros relevantes, decisões de fluxo, erros, resultados.
- Delega toda lógica ao UseCase.
- Não contém lógica de negócio.

```csharp
// Exemplo de estrutura de Controller na pasta Endpoint
[ApiController]
[Route("todo-items")]
public class TodoItemsGetAllEndpoint(ITodoItemsGetAllUseCase useCase, ILogger<TodoItemsGetAllEndpoint> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        logger.LogInformation("Getting all todo items");
        var output = await useCase.ExecuteAsync();
        logger.LogInformation("Returning {Count} todo items", output.Items.Count);
        return Ok(output);
    }
}
```

**Exceções permitidas**: validação básica de formato de rota (ex.: id como Guid) pode ficar na Action do Controller.

*Referência: DA-008*

---

### PAD-004 — UseCase como Orquestrador da Slice

**Contexto**: Toda lógica de negócio de uma Slice.

**Problema**: Lógica de negócio espalhada entre endpoints, repositories e outros componentes.

**Solução**: O UseCase é o único componente que contém a lógica de orquestração da Slice. Ele:
- Recebe o input vindo do Endpoint.
- Coordena chamadas a Repositories (via interfaces).
- Aplica regras de negócio da Slice.
- Retorna o output.
- Não acessa infraestrutura diretamente — depende apenas de interfaces.

**Exceções permitidas**: nenhuma — toda lógica de negócio da Slice deve estar no UseCase.

*Referência: DA-004*

---

### PAD-005 — Repository como Materializador de Domínio

**Contexto**: Toda operação de acesso a dados de uma Slice.

**Problema**: Lógica de mapeamento entre dados brutos e objetos de domínio espalhada por múltiplos componentes.

**Solução**: O Repository é responsável por:
- Executar queries e comandos de banco de dados (via scripts SQL em `Scripts/`).
- Materializar objetos tipados de domínio (`Entity`) a partir dos dados brutos retornados.
- Não conter lógica de negócio.
- Implementar a interface definida em `<Feature>Interfaces/`.

Scripts SQL ficam em arquivos `.sql` dentro de `<Feature>Repository/Scripts/`, versionados por Feature.

**Exceções permitidas**: blocos `try-catch` para exceções específicas de infraestrutura (ex.: violação de constraint única, timeout de conexão) são permitidos e esperados nos repositories.

*Referência: DA-004*

---

### PAD-006 — Validação de Input na Camada de Model

**Contexto**: Validação de payloads de entrada de qualquer Endpoint.

**Problema**: Validação espalhada em repositories, use cases ou endpoints mistura responsabilidades.

**Solução**: A validação de payload deve ficar no objeto `Input` de cada Slice, dentro de `<Feature>Models/`. O Input é o ponto de entrada do contrato da Slice e deve garantir que os dados chegam válidos ao UseCase.

```csharp
// Exemplo
public record TodoItemInsertInput
{
    [Required]
    [MaxLength(200)]
    public string Title { get; init; } = string.Empty;
}
```

**Exceções permitidas**: validação de formato de rota (ex.: id como Guid) pode ficar diretamente no handler do endpoint.

*Referência: P009*

---

### PAD-007 — Shared como Biblioteca de Infraestrutura Compartilhada

**Contexto**: Qualquer código reutilizável entre duas ou mais Slices.

**Problema**: Duplicação de código entre Slices ou acoplamento direto entre Slices para compartilhar lógica.

**Solução**: Todo código que precisa ser compartilhado entre Slices reside em `Shared/`. Shared contém:
- Abstrações e interfaces genéricas.
- Utilitários e helpers.
- Clientes de serviços externos.
- Configurações de infraestrutura compartilhada.
- Persistência comum (ex.: configuração de connection string).

`Shared/` **não deve**:
- Conter lógica especializada para uma única Slice.
- Depender de Features.
- Conter regras de negócio.

*Referência: DA-004*

---

### PAD-008 — Tratamento Centralizado de Exceções via IExceptionHandler

**Contexto**: Tratamento de exceções não tratadas em qualquer ponto da aplicação.

**Problema**: `try-catch` genérico espalhado em Endpoints e UseCases viola SRP (P010, DA-006) e esconde a origem real dos erros, dificultando diagnóstico e manutenção.

**Solução**: Implementar `IExceptionHandler` (ASP.NET Core 8) como handler centralizado, registrado em `Shared/Middleware/`. O handler:
- Captura toda exceção não tratada que escape da pipeline.
- Loga o erro com contexto completo via `ILogger`.
- Retorna resposta padronizada em formato **Problem Details** (RFC 7807 / RFC 9110).
- Não contém lógica de negócio — responsabilidade exclusiva de captura e formatação de erro.

**Estrutura**:
```
Shared/
└── Middleware/
    └── GlobalExceptionHandler.cs
```

**Registro em `Program.cs`**:
```csharp
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
// ...
app.UseExceptionHandler();
```

**Exceções permitidas**: `try-catch` específicos em Repositories para exceções de infraestrutura (ex: violação de constraint, timeout de conexão) continuam permitidos conforme PAD-005.

*Referência: P010, DA-006, DA-010*

---

## Anti-Padrões Conhecidos

| Anti-Padrão | Por Que Evitar | Alternativa |
|---|---|---|
| Pastas globais `Services/`, `Repositories/` | Acoplamento entre Features; dificulta isolamento de mudanças | Organizar por Slice em `Features/Query/` ou `Features/Command/` |
| Lógica de negócio no Controller/Action | Viola SRP; dificulta testes | Mover para o UseCase da Slice |
| Lógica de negócio no Repository | Viola SRP; acopla infraestrutura ao negócio | Mover para o UseCase da Slice |
| Comunicação direta entre Slices | Cria acoplamento entre Features | Mover lógica compartilhada para `Shared/` |
| `try-catch` genérico espalhado pela aplicação | Oculta erros reais; viola SRP | Usar handler centralizado de erros |
| Validação de payload fora do Input | Viola SRP; dificulta rastreamento | Mover validação para o objeto Input da Slice |
| Namespace com chaves em C# | Inconsistência com o padrão do projeto | Usar file-scoped namespace |
| Tipos explícitos onde `var` resolve | Verbosidade desnecessária | Usar `var` com nome de variável autoexplicativo |
| Controller com múltiplas responsabilidades de Slices distintas | Quebra o isolamento da Vertical Slice | Um Controller por Slice, na pasta `<Feature>Endpoint/` |

---

## Referências Cruzadas

- `Instructions/architecture/engineering-principles.md` — princípios que motivam os padrões
- `Instructions/architecture/naming-conventions.md` — nomenclatura usada nos padrões
- `Instructions/architecture/folder-structure.md` — organização que reflete os padrões
- `Instructions/decisions/` — ADRs que justificam a escolha dos padrões

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| Bootstrap | Estrutura criada sem padrões específicos | — |
| 2026-03-15 | PAD-001 a PAD-007 criados: Vertical Slice, Command/Query, Minimal API, UseCase, Repository, Validação, Shared | Instruções do usuário |
| 2026-03-15 | PAD-003 atualizado: Minimal API substituída por Controller com Action; anti-padrão adicionado | DA-008 |
| 2026-03-15 | PAD-008 criado: tratamento centralizado de exceções via IExceptionHandler em Shared/Middleware/ | DA-010, P010 |
