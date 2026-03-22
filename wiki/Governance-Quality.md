# Qualidade e Manutenção

## Descrição

Documenta a estratégia de tratamento de erros, o formato Problem Details e as políticas de qualidade do projeto. Deve ser consultado ao entender respostas de erro ou ao tratar exceções.

## Contexto

O projeto adota uma abordagem centralizada para tratamento de exceções, utilizando `IExceptionHandler` do ASP.NET Core com respostas no formato Problem Details (RFC 7807). Blocos `try-catch` genéricos espalhados pelo código são explicitamente proibidos. A qualidade é garantida por um pipeline de validação pré-commit obrigatório que inclui build, execução, testes e validação de endpoints via HTTP real.

---

## Tratamento Centralizado de Exceções (DA-010, PAD-008)

### Componente

`GlobalExceptionHandler` em `Infra/ExceptionHandling/GlobalExceptionHandler.cs`

### Comportamento

- Implementa `IExceptionHandler` (ASP.NET Core)
- Captura todas as exceções não tratadas que escapam da pipeline de middleware
- Registra log completo da exceção com nível `Error` (inclui `CorrelationId` automaticamente via Serilog LogContext)
- Retorna HTTP 500 com corpo no formato Problem Details (RFC 7807)
- **Não expõe** stack trace ao cliente
- **Transparente para Features** — nenhum `try-catch` é necessário nos Controllers ou Use Cases

### Formato da Resposta de Erro

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.6.1",
  "title": "An error occurred while processing your request.",
  "status": 500
}
```

### Registro em `Program.cs`

```csharp
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
// ...
app.UseExceptionHandler();
```

---

## Política de Tratamento de Erros (P010)

### Proibido

Blocos `try-catch` genéricos espalhados pela aplicação. Exemplo de anti-padrão:

```csharp
// PROIBIDO — try-catch genérico em use case ou endpoint
try
{
    var result = await useCase.ExecuteAsync(input);
    return Ok(result);
}
catch (Exception ex)
{
    logger.LogError(ex, "Error");
    return StatusCode(500);
}
```

### Obrigatório

Utilizar o handler centralizado (`GlobalExceptionHandler`) para exceções genéricas.

### Permitido

Blocos `try-catch` para exceções bem definidas e específicas, geralmente nas camadas de infraestrutura ou repositórios:

```csharp
// PERMITIDO — exceção específica, em repositório
try
{
    await connection.ExecuteAsync(sql, parameters);
}
catch (SqlException ex) when (ex.Number == 2627)
{
    throw new DuplicateKeyException("Todo item already exists.", ex);
}
```

---

## Validação Pré-Commit

Antes de qualquer commit, a seguinte sequência de validação é obrigatória:

### 1. Build Limpo

```bash
dotnet build
```

Critério: zero erros de compilação.

### 2. Aplicação Sobe e Responde

```bash
dotnet run &
curl http://localhost:5000/health
```

Critério: `/health` responde com qualquer código HTTP (confirma inicialização).

### 3. Todos os Testes Passam (Gate Obrigatório)

```bash
dotnet test
```

Critério: 100% dos testes passando. **Falha bloqueia os passos seguintes.**

### 4. Docker Compose + Health Check

```bash
docker compose up -d --build
curl http://localhost:8080/health
```

Critério: `/health` responde HTTP 200. Executado **somente após aprovação no gate de testes**.

### 5. Validação de Endpoints

Para features que criam ou alteram endpoints: validação via chamada HTTP real ao endpoint, com a aplicação em execução. Se o endpoint exigir autenticação, o Bearer Token é obtido via `POST /login` antes da chamada. Status code inesperado bloqueia o commit.

---

## Referências

- [Arquitetura](Governance-Architecture) — visão geral da arquitetura e localização do GlobalExceptionHandler
- [Segurança](Governance-Security) — autenticação e tratamento de 401
- [Testes](Governance-Testing) — estratégia de testes unitários
