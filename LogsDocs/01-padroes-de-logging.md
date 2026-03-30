# Padrões de Logging

## Visão Geral

O projeto adota o padrão de **Logging Estruturado com Storytelling** (SNP-001), que permite reconstruir a narrativa completa de qualquer requisição a partir dos logs, sem necessidade de debugger. Todo log segue um formato padronizado que identifica precisamente a origem (classe e método) e descreve a ação em linguagem imperativa.

---

## Formato de Prefixo Obrigatório

Todo log da aplicação deve seguir o formato:

```
[NomeDaClasse][NomeDoMétodo] DescriçãoBreveFrase
```

### Exemplos

```csharp
logger.LogInformation("[SampleQueryGetEndpoint][Get] Processar requisição GET /sample-query");

logger.LogInformation("[ExternalServiceApiClient][GetForecastAsync] Executar requisição HTTP ao External Service. Latitude={Latitude}, Longitude={Longitude}", input.Latitude, input.Longitude);

logger.LogWarning("[AuthenticateFilter][OnActionExecutionAsync] Retornar 401 - token ausente na requisição");
```

---

## Regras de Escrita de Logs

### 1. Linguagem Imperativa

A descrição após o prefixo deve ser breve, objetiva e no imperativo:

| Correto | Incorreto |
|---------|-----------|
| `Processar requisição` | `A requisição está sendo processada` |
| `Retornar resposta` | `Respondendo com o resultado` |
| `Gerar token JWT` | `Token JWT foi gerado` |

### 2. Log de Entrada do Método

Todo método deve ter um log informando **o que será executado** e registrando os **objetos/parâmetros recebidos**:

```csharp
public async Task<ItemGetByIdOutput> ExecuteAsync(int id, CancellationToken cancellationToken = default)
{
    logger.LogInformation(
        "[ItemGetByIdUseCase][ExecuteAsync] Executar caso de uso de consulta de Item. ItemId={ItemId}", id);

    // ... lógica do método
}
```

### 3. Log de Saída do Método

Antes de cada `return`, deve haver um log informando **o que está sendo retornado**:

```csharp
    logger.LogInformation(
        "[ItemGetByIdUseCase][ExecuteAsync] Retornar dados do Item. ItemId={ItemId}, ItemName={ItemName}",
        output.Id, output.Name);

    return output;
```

### 4. Log Antes e Depois de Loops

Estruturas de iteração (`for`, `foreach`, `while`) devem ter log antes de iniciar e após concluir:

```csharp
logger.LogInformation(
    "[ItemGetByIdUseCase][ExecuteAsync] Iterar tipos do Item. Count={Count}", result.Types.Count);

foreach (var typeSlot in result.Types)
{
    types.Add(new ItemGetByIdTypeItem { Slot = typeSlot.Slot, Name = typeSlot.Type.Name });
}

logger.LogInformation("[ItemGetByIdUseCase][ExecuteAsync] Iteracao de tipos concluida");
```

### 5. Isolamento Visual no Código

Toda instrução `logger.Log*()` deve ter uma **linha em branco acima** e uma **linha em branco abaixo** no código-fonte, garantindo legibilidade:

```csharp
var result = await externalServiceApiClient.GetForecastAsync(input, cancellationToken);

logger.LogInformation(
    "[SampleQueryGetUseCase][ExecuteAsync] Mapear resposta da External Service para model da Feature");

var output = new SampleQueryGetOutput { /* ... */ };
```

### 6. Logs em Program.cs

O `Program.cs` segue regras especiais:
- Log inicial informando que a aplicação está sendo iniciada
- Um log por **bloco lógico** (Serilog, DI de infraestrutura, DI de features, DI de segurança, pipeline de middlewares)
- O log é escrito **antes** do conjunto de instruções do bloco
- **Não** criar log por instrução individual dentro do bloco

---

## Responsabilidade de Logging por Camada

Cada camada da arquitetura tem um papel específico nos logs:

| Camada | Responsabilidade de Logging |
|--------|----------------------------|
| **Endpoint (Controller)** | Início e fim da requisição HTTP; parâmetros da rota/query; resultado retornado |
| **UseCase** | Início do caso de uso; decisões de fluxo; mapeamentos; resultado da orquestração |
| **ApiClient** | Parâmetros da chamada HTTP externa; resposta recebida da API; loops de paginação |
| **CachedApiClient** | Cache hit vs. cache miss; chave de cache; duração do cache |
| **Middleware** | Geração de CorrelationId; enriquecimento de contexto |
| **AuthenticateFilter** | Validação de token; 401 por token ausente/inválido; enriquecimento de UserId/UserName |

---

## Exemplo Completo: Endpoint (Controller)

O Endpoint é responsável por logar o início e o fim da requisição HTTP, incluindo parâmetros recebidos e dados-chave do resultado retornado. Não contém lógica de negócio — apenas orquestra request/response e delega ao UseCase.

### Endpoint com parâmetros de query

```csharp
[ApiController]
[Route("sample-query")]
[Authenticate]
public sealed class SampleQueryGetEndpoint(
    SampleQueryGetUseCase useCase,
    ILogger<SampleQueryGetEndpoint> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[SampleQueryGetEndpoint][Get] Processar requisição GET /sample-query. Latitude={Latitude}, Longitude={Longitude}",
            latitude, longitude);

        var input = new SampleQueryGetInput { Latitude = latitude, Longitude = longitude };
        var result = await useCase.ExecuteAsync(input, cancellationToken);

        logger.LogInformation(
            "[SampleQueryGetEndpoint][Get] Retornar resposta do endpoint com dados da consulta. Latitude={Latitude}, Longitude={Longitude}",
            latitude, longitude);

        return Ok(result);
    }
}
```

**Observações**:
- Log de entrada: descreve a rota, método HTTP e parâmetros recebidos via query
- Log de saída: descreve o que está sendo retornado com os parâmetros da consulta
- Sem lógica de negócio — apenas dois logs, construção do Input e delegação ao UseCase

### Endpoint com parâmetro de rota

```csharp
[ApiController]
[Route("items")]
[Authenticate]
public sealed class ItemGetByIdEndpoint(
    ItemGetByIdUseCase useCase,
    ILogger<ItemGetByIdEndpoint> logger) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        logger.LogInformation("[ItemGetByIdEndpoint][Get] Processar requisicao GET /items/{ItemId}", id);

        var result = await useCase.ExecuteAsync(id, cancellationToken);

        logger.LogInformation(
            "[ItemGetByIdEndpoint][Get] Retornar resposta do endpoint com dados do Item. ItemId={ItemId}, ItemName={ItemName}",
            id, result.Name);

        return Ok(result);
    }
}
```

**Observações**:
- O parâmetro de rota `id` é logado na entrada como `{ItemId}` (nome semântico, não técnico)
- Na saída, dados-chave do resultado (`ItemId`, `ItemName`) são incluídos para rastreabilidade

### Endpoint com parâmetros de query

```csharp
[ApiController]
[Route("sample-query")]
[Authenticate]
public sealed class SampleQueryGetEndpoint(
    SampleQueryGetUseCase useCase,
    ILogger<SampleQueryGetEndpoint> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[SampleQueryGetEndpoint][Get] Processar requisição GET /sample-query. Latitude={Latitude}, Longitude={Longitude}",
            latitude, longitude);

        var input = new SampleQueryGetInput { Latitude = latitude, Longitude = longitude };
        var result = await useCase.ExecuteAsync(input, cancellationToken);

        logger.LogInformation(
            "[SampleQueryGetEndpoint][Get] Retornar resposta do endpoint com dados da consulta. Latitude={Latitude}, Longitude={Longitude}",
            latitude, longitude);

        return Ok(result);
    }
}
```

---

## Exemplo Completo: UseCase

O UseCase é responsável por logar a orquestração da lógica de negócio: início do caso de uso, chamadas a serviços externos, mapeamentos, iterações e resultado final. Toda decisão de fluxo relevante deve ter um log.

### UseCase simples (chamada a API externa + mapeamento)

```csharp
public sealed class SampleQueryGetUseCase(
    IExternalServiceApiClient externalServiceApiClient,
    ILogger<SampleQueryGetUseCase> logger)
{
    private const string CurrentFields = "temperature_2m,relative_humidity_2m,apparent_temperature,...";

    public async Task<SampleQueryGetOutput> ExecuteAsync(
        SampleQueryGetInput input, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[SampleQueryGetUseCase][ExecuteAsync] Executar caso de uso de dados da consulta. Latitude={Latitude}, Longitude={Longitude}",
            input.Latitude, input.Longitude);

        var externalServiceInput = new ExternalServiceInput
        {
            Latitude = input.Latitude,
            Longitude = input.Longitude,
            Current = CurrentFields
        };

        logger.LogInformation(
            "[SampleQueryGetUseCase][ExecuteAsync] Consultar API External Service. Latitude={Latitude}, Longitude={Longitude}",
            externalServiceInput.Latitude, externalServiceInput.Longitude);

        var result = await externalServiceApiClient.GetForecastAsync(externalServiceInput, cancellationToken);

        logger.LogInformation(
            "[SampleQueryGetUseCase][ExecuteAsync] Mapear resposta da External Service para model da Feature");

        var output = new SampleQueryGetOutput
        {
            Latitude = result.Latitude,
            Longitude = result.Longitude,
            Timezone = result.Timezone,
            // ... demais campos mapeados
        };

        logger.LogInformation(
            "[SampleQueryGetUseCase][ExecuteAsync] Retornar dados da consulta obtidas da External Service. Timezone={Timezone}",
            output.Timezone);

        return output;
    }
}
```

**Observações**:
- Log de entrada: descreve o caso de uso
- Log antes da chamada externa: registra parâmetros enviados à API
- Log após receber a resposta: descreve a ação de mapeamento
- Log de saída: informa dados-chave do resultado

### UseCase com iterações (mapeamento de listas)

```csharp
public sealed class ItemGetByIdUseCase(
    IItemApiClient itemsApiClient,
    ILogger<ItemGetByIdUseCase> logger)
{
    public async Task<ItemGetByIdOutput> ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[ItemGetByIdUseCase][ExecuteAsync] Executar caso de uso de consulta de Item. ItemId={ItemId}", id);

        logger.LogInformation(
            "[ItemGetByIdUseCase][ExecuteAsync] Consultar External API. ItemId={ItemId}", id);

        var result = await itemsApiClient.GetByIdAsync(id, cancellationToken);

        logger.LogInformation(
            "[ItemGetByIdUseCase][ExecuteAsync] Mapear resposta da External API para model da Feature");

        var types = new List<ItemGetByIdTypeItem>();

        logger.LogInformation(
            "[ItemGetByIdUseCase][ExecuteAsync] Iterar tipos do Item. Count={Count}", result.Types.Count);

        foreach (var typeSlot in result.Types)
        {
            types.Add(new ItemGetByIdTypeItem
            {
                Slot = typeSlot.Slot,
                Name = typeSlot.Type.Name
            });
        }

        logger.LogInformation("[ItemGetByIdUseCase][ExecuteAsync] Iteracao de tipos concluida");

        var abilities = new List<ItemGetByIdAbilityItem>();

        logger.LogInformation(
            "[ItemGetByIdUseCase][ExecuteAsync] Iterar habilidades do Item. Count={Count}", result.Abilities.Count);

        foreach (var abilitySlot in result.Abilities)
        {
            abilities.Add(new ItemGetByIdAbilityItem
            {
                Name = abilitySlot.Ability.Name,
                IsHidden = abilitySlot.IsHidden
            });
        }

        logger.LogInformation("[ItemGetByIdUseCase][ExecuteAsync] Iteracao de habilidades concluida");

        var stats = new List<ItemGetByIdStatItem>();

        logger.LogInformation(
            "[ItemGetByIdUseCase][ExecuteAsync] Iterar stats do Item. Count={Count}", result.Stats.Count);

        foreach (var statSlot in result.Stats)
        {
            stats.Add(new ItemGetByIdStatItem
            {
                Name = statSlot.Stat.Name,
                BaseStat = statSlot.BaseStat,
                Effort = statSlot.Effort
            });
        }

        logger.LogInformation("[ItemGetByIdUseCase][ExecuteAsync] Iteracao de stats concluida");

        var output = new ItemGetByIdOutput
        {
            Id = result.Id,
            Name = result.Name,
            Height = result.Height,
            Weight = result.Weight,
            BaseExperience = result.BaseExperience,
            FrontDefaultSprite = result.Sprites.FrontDefault,
            FrontShinySprite = result.Sprites.FrontShiny,
            Types = types,
            Abilities = abilities,
            Stats = stats
        };

        logger.LogInformation(
            "[ItemGetByIdUseCase][ExecuteAsync] Retornar dados do Item. ItemId={ItemId}, ItemName={ItemName}",
            output.Id, output.Name);

        return output;
    }
}
```

**Observações**:
- Cada iteração (`foreach`) tem log **antes** (com `Count`) e **depois** (confirmação de conclusão)
- O padrão se repete para cada lista mapeada (tipos, habilidades, stats)
- A linha em branco acima e abaixo de cada `logger.Log*()` é respeitada (isolamento visual)
- O log de saída inclui `ItemId` e `ItemName` para correlação com o log de entrada

---

## Ganhos do Padrão Storytelling

### 1. Rastreabilidade Completa

Cada requisição gera uma sequência narrativa nos logs que permite reconstruir **exatamente** o que aconteceu, em qual ordem, e com quais dados — sem acesso ao código-fonte ou debugger.

### 2. Diagnóstico sem Reprodução

Erros em produção podem ser diagnosticados apenas pelos logs, sem necessidade de reproduzir o cenário localmente. O prefixo `[Classe][Método]` permite localizar instantaneamente o ponto exato do código.

### 3. Filtragem Eficiente

O formato padronizado permite filtros precisos em ferramentas de observabilidade:

- `[SampleQueryGetEndpoint]` — todos os logs de um endpoint específico
- `[GetForecastAsync]` — todos os logs de um método específico, em qualquer classe
- `CorrelationId=abc-123` — todos os logs de uma requisição específica

### 4. Onboarding Acelerado

Novos desenvolvedores entendem o fluxo de execução lendo os logs, que descrevem em linguagem natural o que cada componente faz.

### 5. Validação de Comportamento

O padrão permite que testes automatizados validem que o fluxo de logging está correto, usando `FakeLogger<T>` e asserções baseadas em `Contains` (ver [05-testes-de-logging.md](05-testes-de-logging.md)).

---

## Template do Console

O template de saída do console é normativo e usa tema ANSI colorido:

```csharp
.WriteTo.Console(
    theme: AnsiConsoleTheme.Code,
    outputTemplate: "[{Timestamp:dd/MM/yyyy HH:mm:ss.fffffff}] [{CorrelationId}] [{UserName}] {Message:lj}{NewLine}{Exception}")
```

### Campos do Template

| Campo | Formato | Origem |
|-------|---------|--------|
| `Timestamp` | `dd/MM/yyyy HH:mm:ss.fffffff` | Gerado automaticamente pelo Serilog |
| `CorrelationId` | GUID v7 | Enriquecido pelo `CorrelationIdMiddleware` via `LogContext.PushProperty` |
| `UserName` | String | Enriquecido pelo `AuthenticateFilter` via `LogContext.PushProperty` |
| `Message` | Template estruturado | Definido em cada instrução `logger.Log*()` |
| `Exception` | Stack trace completo | Incluído automaticamente quando há exceção |

### Exemplo de Saída no Console

```
[23/03/2026 14:32:01.1234567] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [SampleQueryGetEndpoint][Get] Processar requisição GET /sample-query
[23/03/2026 14:32:01.1235000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [SampleQueryGetUseCase][ExecuteAsync] Executar caso de uso de dados da consulta de São Paulo
[23/03/2026 14:32:01.1236000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [CachedExternalServiceApiClient][GetForecastAsync] Verificar cache para dados da consulta
[23/03/2026 14:32:01.1237000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [CachedExternalServiceApiClient][GetForecastAsync] Cache miss. Consultar API externa. CacheKey=ExternalService:SampleQueryGet:1
[23/03/2026 14:32:01.4500000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [ExternalServiceApiClient][GetForecastAsync] Executar requisição HTTP ao External Service. Latitude=-23.5475, Longitude=-46.6361
[23/03/2026 14:32:01.8900000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [ExternalServiceApiClient][GetForecastAsync] Retornar resposta da API External Service. Timezone=America/Sao_Paulo
[23/03/2026 14:32:01.8910000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [CachedExternalServiceApiClient][GetForecastAsync] Armazenar resposta no cache. CacheKey=ExternalService:SampleQueryGet:1, DurationSeconds=10
[23/03/2026 14:32:01.8920000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [SampleQueryGetUseCase][ExecuteAsync] Mapear resposta da External Service para model da Feature
[23/03/2026 14:32:01.8930000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [SampleQueryGetUseCase][ExecuteAsync] Retornar dados da consulta obtidas da External Service. Timezone=America/Sao_Paulo
[23/03/2026 14:32:01.8940000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [SampleQueryGetEndpoint][Get] Retornar resposta do endpoint com dados da consulta de São Paulo
```

---

## Referências

- [02-enriquecimento-de-contexto.md](02-enriquecimento-de-contexto.md) — detalhes do CorrelationId e campos de usuário
- [03-exemplo-requisicao-completa.md](03-exemplo-requisicao-completa.md) — exemplo ponta a ponta
- [04-integracao-datadog.md](04-integracao-datadog.md) — como os logs chegam ao Datadog
- `Instructions/snippets/canonical-snippets.md` — SNP-001 (snippet normativo)
- `Instructions/architecture/architecture-decisions.md` — DA-015 (decisão arquitetural)
