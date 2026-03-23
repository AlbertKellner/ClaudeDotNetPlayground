# Prompt para Implementar Boas Práticas de Logging Estruturado

Este documento contém um prompt único e autocontido para instruir o Claude (ou outro LLM) a implementar o padrão de logging estruturado com storytelling em qualquer projeto C# / ASP.NET Core com Serilog.

O prompt é genérico e pode ser copiado e usado em qualquer repositório.

---

## Prompt

```
Implemente o padrão de Logging Estruturado com Storytelling em todo o projeto C# / ASP.NET Core, seguindo rigorosamente as regras abaixo. O objetivo é permitir reconstruir a narrativa completa de qualquer requisição apenas lendo os logs, sem necessidade de debugger.

---

### 1. Dependência obrigatória

Instale e configure o Serilog como provider de logging:

- Pacote: `Serilog.AspNetCore`
- Pacote: `Serilog.Sinks.Console`

No `Program.cs`, configure o Serilog com console colorido:

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console(
        theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code,
        outputTemplate: "[{Timestamp:dd/MM/yyyy HH:mm:ss.fffffff}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();
```

---

### 2. Formato de prefixo obrigatório

Todo log da aplicação deve seguir o formato:

```
[NomeDaClasse][NomeDoMétodo] DescriçãoBreveFrase
```

Exemplos:
- `[OrderController][Create] Processar requisição POST /orders`
- `[OrderService][GetByIdAsync] Executar consulta de pedido. OrderId=42`
- `[PaymentClient][ChargeAsync] Executar requisição HTTP ao gateway de pagamento`

---

### 3. Regras de escrita de logs

#### 3.1 Linguagem imperativa
A descrição deve ser breve, objetiva e no imperativo:
- Correto: "Processar requisição", "Retornar resposta", "Executar consulta"
- Incorreto: "A requisição está sendo processada", "Respondendo com o resultado"

#### 3.2 Log de entrada do método
Todo método deve ter um log como primeira instrução informando o que será executado e registrando os parâmetros recebidos:

```csharp
public async Task<OrderOutput> GetByIdAsync(int id)
{
    logger.LogInformation(
        "[OrderService][GetByIdAsync] Executar consulta de pedido. OrderId={OrderId}", id);
    // ...
}
```

#### 3.3 Log de saída do método
Antes de cada `return`, deve haver um log informando o que está sendo retornado:

```csharp
    logger.LogInformation(
        "[OrderService][GetByIdAsync] Retornar pedido encontrado. OrderId={OrderId}, Status={Status}",
        output.Id, output.Status);

    return output;
```

#### 3.4 Log antes e depois de loops
Estruturas de iteração devem ter log antes de iniciar e após concluir:

```csharp
logger.LogInformation(
    "[OrderService][ProcessItemsAsync] Iterar itens do pedido. Count={Count}", items.Count);

foreach (var item in items)
{
    // processamento
}

logger.LogInformation("[OrderService][ProcessItemsAsync] Iteração de itens concluída");
```

#### 3.5 Isolamento visual no código
Toda instrução `logger.Log*()` deve ter uma linha em branco acima e uma linha em branco abaixo no código-fonte:

```csharp
var result = await repository.GetByIdAsync(id);

logger.LogInformation(
    "[OrderService][GetByIdAsync] Mapear resultado para output");

var output = MapToOutput(result);
```

---

### 4. Responsabilidade de logging por camada

| Camada | O que logar |
|--------|-------------|
| **Controller/Endpoint** | Início e fim da requisição HTTP; parâmetros de rota/query; dados-chave do resultado |
| **Service/UseCase** | Início do caso de uso; decisões de fluxo; mapeamentos; chamadas a dependências; resultado da orquestração |
| **Repository/Client** | Parâmetros da operação (query, chamada HTTP); resposta recebida; loops de paginação |
| **Cache (Decorator)** | Cache hit vs. cache miss; chave de cache utilizada; duração do cache |
| **Middleware** | Geração de identificadores de correlação; enriquecimento de contexto |

---

### 5. Logs no Program.cs

O `Program.cs` segue regras especiais:
- Log inicial informando que a aplicação está sendo iniciada
- Um log por bloco lógico de configuração (Serilog, DI de infraestrutura, DI de features, pipeline de middlewares)
- O log é escrito antes do conjunto de instruções do bloco
- Não criar log por instrução individual dentro do bloco

Exemplo:
```csharp
Log.Information("[Program] Configurar Serilog");
// ... configuração do Serilog

Log.Information("[Program] Registrar dependências de infraestrutura");
// ... registros de DI

Log.Information("[Program] Registrar dependências das features");
// ... registros de features

Log.Information("[Program] Configurar pipeline de middlewares");
// ... configuração do pipeline

Log.Information("[Program] Iniciar execução da aplicação");
app.Run();
```

---

### 6. Testes de logging

Crie um `FakeLogger<T>` para capturar logs nos testes unitários:

```csharp
public sealed record FakeLogRecord(LogLevel Level, string Message);

public sealed class FakeLogger<T> : ILogger<T>
{
    private readonly List<FakeLogRecord> _records = [];

    public IReadOnlyList<FakeLogRecord> GetSnapshot() => _records.AsReadOnly();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel, EventId eventId, TState state,
        Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _records.Add(new FakeLogRecord(logLevel, formatter(state, exception)));
    }
}
```

Para cada classe que emite logs, crie testes validando:
- Tipo do evento (LogLevel) + conteúdo parcial da mensagem via `Contains`
- Prefixo correto `[NomeDaClasse]` em todos os logs da classe

Exemplo:
```csharp
[Fact]
public async Task ExecuteAsync_DeveRegistrarLogInformationNoInicio()
{
    var fakeLogger = new FakeLogger<OrderService>();
    var service = new OrderService(fakeLogger);

    await service.GetByIdAsync(42);

    var logs = fakeLogger.GetSnapshot();
    Assert.Contains(logs, l =>
        l.Level == LogLevel.Information &&
        l.Message.Contains("Executar"));
}

[Fact]
public async Task ExecuteAsync_DeveRegistrarLogsComPrefixoCorreto()
{
    var fakeLogger = new FakeLogger<OrderService>();
    var service = new OrderService(fakeLogger);

    await service.GetByIdAsync(42);

    var logs = fakeLogger.GetSnapshot();
    Assert.All(logs, l => Assert.Contains("OrderService", l.Message));
}
```

---

### 7. O que NÃO fazer

- NÃO logar dados sensíveis (senhas, tokens, dados pessoais)
- NÃO usar logs genéricos sem prefixo (`logger.LogInformation("Processing...")`)
- NÃO usar try-catch genérico para logar exceções — use handler centralizado de exceções
- NÃO omitir parâmetros relevantes nos logs de entrada e saída
- NÃO colocar múltiplas instruções de log coladas sem linhas em branco entre elas
- NÃO usar passado ("processou", "retornou") — use imperativo ("processar", "retornar")

---

### 8. Checklist de aplicação

Para cada classe do projeto:
- [ ] Todo método tem log de entrada com prefixo `[Classe][Método]`
- [ ] Todo método tem log de saída antes do return
- [ ] Loops têm log antes e depois
- [ ] Instruções de log têm linha em branco acima e abaixo
- [ ] Linguagem é imperativa e breve
- [ ] Parâmetros relevantes são logados com nomes semânticos (`OrderId`, não `id`)
- [ ] Teste unitário valida presença dos logs e prefixo correto
```

---

## Referências

- [01-padroes-de-logging.md](01-padroes-de-logging.md) — Documentação completa dos padrões
- [05-testes-de-logging.md](05-testes-de-logging.md) — Estratégia de testes de logging
