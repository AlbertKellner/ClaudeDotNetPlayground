# Testes de Logging

## Visão Geral

O projeto valida o comportamento de logging através de um `FakeLogger<T>` que captura todos os logs emitidos durante a execução de testes unitários. Os testes verificam que o padrão SNP-001 é seguido: tipo do evento (nível de log) e conteúdo parcial da mensagem via `Contains`.

---

## FakeLogger — Implementação

O `FakeLogger<T>` é uma implementação de `ILogger<T>` que armazena todos os logs em uma lista para posterior asserção:

```csharp
public sealed record FakeLogRecord(LogLevel Level, string Message);

public sealed class FakeLogger<T> : ILogger<T>
{
    private readonly List<FakeLogRecord> _records = [];

    public IReadOnlyList<FakeLogRecord> GetSnapshot() => _records.AsReadOnly();

    public void Clear() => _records.Clear();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        => NullDisposable.Instance;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        _records.Add(new FakeLogRecord(logLevel, message));
    }

    private sealed class NullDisposable : IDisposable
    {
        public static readonly NullDisposable Instance = new();
        public void Dispose() { }
    }
}
```

### Características

| Aspecto | Detalhe |
|---------|---------|
| **Thread-safe** | Não — projetado para uso em testes unitários (single-thread) |
| **Todos os níveis habilitados** | `IsEnabled` retorna `true` para qualquer `LogLevel` |
| **Scopes ignorados** | `BeginScope` retorna `NullDisposable` — scopes do LogContext não afetam a captura |
| **Snapshot** | `GetSnapshot()` retorna cópia imutável para evitar concorrência em asserções |
| **Limpeza** | `Clear()` permite reutilizar o logger entre testes |

---

## Padrão de Teste de Logging

### Estrutura Geral

```csharp
[Fact]
public async Task NomeDoMetodo_DeveRegistrarLogComPrefixoCorreto()
{
    // Arrange — instanciar FakeLogger e componente sob teste
    var fakeLogger = new FakeLogger<MinhaClasse>();
    var componente = new MinhaClasse(/* dependências */, fakeLogger);

    // Act — executar o método que gera logs
    await componente.ExecuteAsync(/* parâmetros */);

    // Assert — verificar logs capturados
    var logs = fakeLogger.GetSnapshot();

    Assert.Contains(logs, l =>
        l.Level == LogLevel.Information &&
        l.Message.Contains("termo-esperado"));
}
```

### Asserções Recomendadas

| Tipo de Asserção | Quando Usar | Exemplo |
|-----------------|-------------|---------|
| `Assert.Contains` com `Level` + `Contains` | Verificar que um log específico foi emitido | `Assert.Contains(logs, l => l.Level == LogLevel.Information && l.Message.Contains("Processar"))` |
| `Assert.All` com `Contains` | Verificar prefixo em todos os logs | `Assert.All(logs, l => Assert.Contains("NomeDaClasse", l.Message))` |
| `Assert.DoesNotContain` | Verificar que um log NÃO foi emitido | `Assert.DoesNotContain(logs, l => l.Level == LogLevel.Error)` |

---

## Exemplos de Testes Reais

### Teste de Middleware — CorrelationIdMiddleware

Valida que o middleware emite log de `Information` no início da requisição:

```csharp
[Fact]
public async Task InvokeAsync_DeveRegistrarLogInformationNoInicio()
{
    // Arrange
    var fakeLogger = new FakeLogger<CorrelationIdMiddleware>();
    var nextCalled = false;
    var middleware = new CorrelationIdMiddleware(_ =>
    {
        nextCalled = true;
        return Task.CompletedTask;
    }, fakeLogger);

    var httpContext = new DefaultHttpContext();
    httpContext.Response.Body = new MemoryStream();

    // Act
    await middleware.InvokeAsync(httpContext);

    // Assert
    Assert.True(nextCalled);
    var logs = fakeLogger.GetSnapshot();

    Assert.Contains(logs, l =>
        l.Level == LogLevel.Information &&
        l.Message.Contains("Processar"));
}
```

### Teste de ApiClient — Prefixo Correto

Valida que todos os logs do `OpenMeteoApiClient` contêm o prefixo da classe:

```csharp
[Fact]
public async Task GetForecastAsync_DeveRegistrarLogsComPrefixoCorreto()
{
    // Arrange
    var fakeApi = new FakeOpenMeteoApi();
    var fakeLogger = new FakeLogger<OpenMeteoApiClient>();
    var client = new OpenMeteoApiClient(fakeApi, fakeLogger);
    var input = new OpenMeteoInput
    {
        Latitude = -23.5475,
        Longitude = -46.6361,
        Current = "temperature_2m"
    };

    // Act
    await client.GetForecastAsync(input);

    // Assert
    var logs = fakeLogger.GetSnapshot();

    Assert.All(logs, l => Assert.Contains("OpenMeteoApiClient", l.Message));
}
```

### Teste de UseCase — Fluxo Completo de Storytelling

Valida que o UseCase emite logs de entrada, processamento e saída:

```csharp
[Fact]
public async Task ExecuteAsync_DeveRegistrarLogDeEntradaESaida()
{
    // Arrange
    var fakeApiClient = new FakeOpenMeteoApiClient();
    var fakeLogger = new FakeLogger<WeatherConditionsGetUseCase>();
    var useCase = new WeatherConditionsGetUseCase(fakeApiClient, fakeLogger);

    // Act
    await useCase.ExecuteAsync();

    // Assert
    var logs = fakeLogger.GetSnapshot();

    // Log de entrada
    Assert.Contains(logs, l =>
        l.Level == LogLevel.Information &&
        l.Message.Contains("Executar caso de uso"));

    // Log de saída
    Assert.Contains(logs, l =>
        l.Level == LogLevel.Information &&
        l.Message.Contains("Retornar"));
}
```

### Teste de Autenticação — Log de Warning para Token Ausente

Valida que o filtro de autenticação emite Warning quando o token está ausente:

```csharp
[Fact]
public async Task OnActionExecutionAsync_TokenAusente_DeveRegistrarWarning()
{
    // Arrange
    var fakeTokenService = new FakeTokenService();
    var fakeLogger = new FakeLogger<AuthenticateFilter>();
    var filter = new AuthenticateFilter(fakeTokenService, fakeLogger);
    var context = CreateActionContextWithoutToken();

    // Act
    await filter.OnActionExecutionAsync(context, () => Task.FromResult(CreateActionExecutedContext(context)));

    // Assert
    var logs = fakeLogger.GetSnapshot();

    Assert.Contains(logs, l =>
        l.Level == LogLevel.Warning &&
        l.Message.Contains("401") &&
        l.Message.Contains("token ausente"));
}
```

---

## O Que Testar nos Logs

### Obrigatório

| Aspecto | O Que Verificar |
|---------|-----------------|
| **Prefixo da classe** | Todos os logs contêm `[NomeDaClasse]` |
| **Log de entrada** | Primeiro log do método informa a ação e parâmetros |
| **Log de saída** | Último log antes do return informa o resultado |
| **Nível correto** | `Information` para fluxo normal, `Warning` para rejeições, `Error` para falhas |

### Opcional (quando aplicável)

| Aspecto | O Que Verificar |
|---------|-----------------|
| **Logs de loop** | Log antes e depois de iterações |
| **Cache hit/miss** | Log diferenciado para cache hit vs. miss |
| **Parâmetros nos logs** | Dados relevantes (IDs, contagens, nomes) presentes na mensagem |

---

## Anti-Padrões em Testes de Log

| Anti-Padrão | Por Que Evitar | Alternativa |
|------------|----------------|-------------|
| Comparar mensagem completa com `Assert.Equal` | Quebra com qualquer mudança de texto | Usar `Assert.Contains` com termo-chave |
| Verificar ordem exata dos logs | Implementação pode mudar a sequência | Verificar presença, não posição |
| Verificar contagem exata de logs | Novos logs intermediários quebram o teste | Usar `Assert.Contains` para logs esperados |
| Ignorar o `LogLevel` na asserção | Logs de warning/error devem ser validados no nível correto | Sempre incluir `l.Level ==` na asserção |

---

## Referências

- [01-padroes-de-logging.md](01-padroes-de-logging.md) — padrão SNP-001 que os testes validam
- `src/Albert.Playground.ECS.AOT.UnitTest/TestHelpers/FakeLogger.cs` — implementação do FakeLogger
- `Instructions/snippets/canonical-snippets.md` — SNP-001, seção "Padrão de testes de log"
