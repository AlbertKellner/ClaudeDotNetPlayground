# Padrões de Desenvolvimento

## Descrição

Documenta os padrões de desenvolvimento adotados neste repositório — Vertical Slice, CQRS leve, UseCase como orquestrador, Repository como materializador de domínio, validação no Input e Decorator para cache. Deve ser consultado ao implementar novas features ou revisar código existente.

## Contexto

Todos os padrões listados aqui são **obrigatórios** para novos artefatos. Exceções devem ser justificadas com ADR. Esses padrões garantem isolamento entre funcionalidades, clareza de responsabilidades e previsibilidade na organização do código.

---

## PAD-001 — Vertical Slice Architecture

Cada funcionalidade é implementada como uma Slice vertical isolada, contendo todos os artefatos necessários para seu funcionamento: endpoint, use case, interfaces, models, repository e scripts SQL.

Slices residem em `Features/Query/` ou `Features/Command/` conforme o tipo de operação.

**Consequências**:
- Mudanças em uma Slice não afetam outras Slices
- Toda lógica de uma funcionalidade está em um único lugar
- Facilita a deleção e teste isolado de uma funcionalidade
- Maior verbosidade inicial de estrutura de pastas

---

## PAD-002 — Segregação Command/Query (CQRS leve)

Toda funcionalidade é classificada **antes de ser implementada** como:

| Tipo | Descrição | Localização |
|---|---|---|
| **Query** | Operação de leitura — não altera estado | `Features/Query/` |
| **Command** | Operação de escrita — altera estado | `Features/Command/` |

A classificação é baseada na **intenção da operação**, não no verbo HTTP.

---

## PAD-003 — Controller com Actions

Cada Slice tem seu próprio Controller localizado na pasta `<Feature>Endpoint/`. O Controller:

- Define a rota via atributos (`[Route]`, `[HttpGet]`, `[HttpPost]`, etc.)
- Contém uma ou mais Actions bem definidas
- Orquestra request/response (leitura de body/params/route, retorno de status codes)
- Escreve logs relevantes seguindo o padrão SNP-001
- Delega toda lógica ao UseCase
- **Não contém lógica de negócio**

```csharp
[ApiController]
[Route("todo-items")]
public class TodoItemsGetAllEndpoint(ITodoItemsGetAllUseCase useCase, ILogger<TodoItemsGetAllEndpoint> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        logger.LogInformation("[TodoItemsGetAllEndpoint][GetAll] Processar requisição de todos os itens");

        var output = await useCase.ExecuteAsync();

        logger.LogInformation("[TodoItemsGetAllEndpoint][GetAll] Retornar {Count} itens", output.Items.Count);
        return Ok(output);
    }
}
```

---

## PAD-004 — UseCase como Orquestrador da Slice

O UseCase é o único componente que contém a lógica de orquestração da Slice:

- Recebe o input vindo do Endpoint
- Coordena chamadas a Repositories e clientes externos (via interfaces)
- Aplica regras de negócio da Slice
- Retorna o output
- **Não acessa infraestrutura diretamente** — depende apenas de interfaces

---

## PAD-005 — Repository como Materializador de Domínio

O Repository é responsável por:

- Executar queries e comandos de banco de dados (via scripts SQL em `Scripts/`)
- Materializar objetos tipados de domínio (`Entity`) a partir dos dados brutos
- Implementar a interface definida em `<Feature>Interfaces/`
- **Não contém lógica de negócio**

Blocos `try-catch` para exceções específicas de infraestrutura (violação de constraint, timeout) são permitidos e esperados nos repositories.

---

## PAD-006 — Validação de Input na Camada de Model

A validação de payload deve ficar no objeto `Input` de cada Slice, dentro de `<Feature>Models/`:

```csharp
public record TodoItemInsertInput
{
    [Required]
    [MaxLength(200)]
    public string Title { get; init; } = string.Empty;
}
```

Validação fora do objeto Input (em repositories, use cases ou endpoints) é proibida.

---

## PAD-007 — Shared como Biblioteca de Infraestrutura Compartilhada

Todo código que precisa ser compartilhado entre Slices reside em `Shared/`:

| Contém | Não contém |
|---|---|
| Abstrações e interfaces genéricas | Lógica especializada para uma única Slice |
| Utilitários e helpers | Regras de negócio |
| Clientes de serviços externos (Refit + Polly) | Models de Input ou Output de Features |
| Configurações de infraestrutura compartilhada | Dependências de Features |

`Shared/` não depende de `Features/`.

---

## PAD-008 — Tratamento Centralizado de Exceções via IExceptionHandler

O tratamento de exceções não tratadas é centralizado em `GlobalExceptionHandler` (`Infra/ExceptionHandling/`), que implementa `IExceptionHandler` do ASP.NET Core:

- Captura toda exceção não tratada que escape da pipeline
- Loga o erro com contexto completo via `ILogger` (inclui CorrelationId automaticamente)
- Retorna HTTP 500 com corpo no formato **Problem Details** (RFC 7807 / RFC 9110)
- Não expõe stack trace ao cliente
- Transparente para Features — nenhum `try-catch` é necessário nos Controllers ou Use Cases

```csharp
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
// ...
app.UseExceptionHandler();
```

**Exceções permitidas**: `try-catch` específicos em Repositories para exceções de infraestrutura (violação de constraint, timeout) continuam permitidos conforme PAD-005.

---

## Decorator Pattern para Memory Cache (DA-018)

Endpoints GET que consomem APIs externas implementam Memory Cache usando `IMemoryCache` com o padrão Decorator:

```
IOpenMeteoApiClient (interface)
    ├── OpenMeteoApiClient (implementação real — chama API externa)
    └── CachedOpenMeteoApiClient (decorator — verifica cache antes de delegar)
```

Características:
- Cache usa o ID do usuário autenticado como chave (definida no código, não configurável via JSON)
- Duração e tipo de expiração configuráveis via `appsettings.json` seção `EndpointCache`
- Cache por usuário garante isolamento e evita vazamento de dados entre usuários
- `AuthenticateFilter` armazena `AuthenticatedUser` em `HttpContext.Items` para acesso pela camada de cache via `IHttpContextAccessor`

O mesmo padrão é aplicado em todas as integrações externas:
- `CachedOpenMeteoApiClient` (clima)
- `CachedGitHubApiClient` (repositórios GitHub)
- `CachedPokemonApiClient` (PokéAPI)

---

## Anti-Padrões Conhecidos

| Anti-Padrão | Por Que Evitar | Alternativa |
|---|---|---|
| Pastas globais `Services/`, `Repositories/` | Acoplamento entre Features; dificulta isolamento | Organizar por Slice em `Features/Query/` ou `Features/Command/` |
| Lógica de negócio no Controller | Viola SRP; dificulta testes | Mover para o UseCase da Slice |
| Lógica de negócio no Repository | Acopla infraestrutura ao negócio | Mover para o UseCase da Slice |
| Comunicação direta entre Slices | Cria acoplamento entre Features | Mover lógica compartilhada para `Shared/` |
| `try-catch` genérico espalhado | Oculta erros reais; viola SRP | Usar handler centralizado (`GlobalExceptionHandler`) |
| Validação de payload fora do Input | Viola SRP; dificulta rastreamento | Mover validação para o objeto Input da Slice |
| Models de Feature em `Shared/` | Acoplamento oculto entre Slices; viola isolamento | Manter em `<Feature>Models/` dentro da Slice |
| Feature retornando model de `Shared/ExternalApi/` | Acopla contrato da Feature ao contrato da API externa | Criar Output model próprio em `<Feature>Models/` e mapear |
| Controller com múltiplas Slices | Quebra o isolamento da Vertical Slice | Um Controller por Slice, na pasta `<Feature>Endpoint/` |

---

## Referências

- [Arquitetura](Governance-Architecture) — visão geral da estrutura do projeto
- [Convenções de Código](Governance-Code-Conventions) — nomenclatura e estilo de código
- [Integrações](Governance-Integrations) — padrões de integração com APIs externas
