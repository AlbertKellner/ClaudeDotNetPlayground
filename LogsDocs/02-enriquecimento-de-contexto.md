# Enriquecimento de Contexto nos Logs

## Visão Geral

O sistema de logging utiliza o **Serilog LogContext** para enriquecer automaticamente todos os logs com propriedades de contexto, sem que as camadas de negócio (Features, UseCases) precisem conhecer ou manipular essas propriedades. O enriquecimento é completamente **opaco** para o código de aplicação.

Três campos são enriquecidos automaticamente:

| Campo | Origem | Escopo |
|-------|--------|--------|
| `CorrelationId` | `CorrelationIdMiddleware` | Toda requisição HTTP |
| `UserId` | `AuthenticateFilter` | Requisições autenticadas |
| `UserName` | `AuthenticateFilter` | Requisições autenticadas |

---

## CorrelationId

### O Que É

O CorrelationId é um identificador único por requisição HTTP, no formato **GUID v7** (ordenável por tempo). Ele permite rastrear todos os logs gerados durante o processamento de uma única requisição, independentemente de quantas camadas e componentes sejam envolvidos.

### Como É Gerado

O `CorrelationIdMiddleware` é o primeiro middleware da pipeline e opera da seguinte forma:

1. Verifica se a requisição contém o header `X-Correlation-Id` com um GUID v7 válido
2. Se sim, reutiliza o ID recebido (permite rastreabilidade entre serviços)
3. Se não, gera um novo GUID v7 via `Guid.CreateVersion7()` (.NET 10 nativo)
4. Armazena o ID em `HttpContext.Items["CorrelationId"]`
5. Adiciona o ID ao header de resposta `X-Correlation-Id`
6. Enriquece **todos os logs** da requisição via `LogContext.PushProperty`

### Implementação do Middleware

```csharp
public sealed class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
{
    public const string HeaderName = "X-Correlation-Id";
    internal const string HttpContextItemKey = "CorrelationId";

    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogInformation("[CorrelationIdMiddleware][InvokeAsync] Processar requisição e garantir CorrelationId");

        var correlationId = ResolveCorrelationId(context);

        context.Items[HttpContextItemKey] = correlationId;
        context.Response.Headers[HeaderName] = correlationId.ToString();

        using (LogContext.PushProperty(HttpContextItemKey, correlationId))
        {
            logger.LogInformation(
                "[CorrelationIdMiddleware][InvokeAsync] Prosseguir com CorrelationId enriquecido no contexto. CorrelationId={CorrelationId}",
                correlationId);

            await next(context);

            logger.LogInformation(
                "[CorrelationIdMiddleware][InvokeAsync] Retornar resposta com CorrelationId enriquecido. CorrelationId={CorrelationId}",
                correlationId);
        }
    }

    private static Guid ResolveCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderName, out var headerValue)
            && Guid.TryParse(headerValue, out var parsed)
            && GuidV7.IsVersion7(parsed))
        {
            return parsed;
        }

        return GuidV7.Create();
    }
}
```

### Por Que GUID v7

O GUID v7 (RFC 9562) contém um componente temporal ordenável nos primeiros 48 bits, o que significa que:

- IDs gerados mais tarde são lexicograficamente maiores
- Filtragem por faixa de tempo em ferramentas de observabilidade é mais eficiente
- Não há necessidade de sequencial numérico — o próprio ID é temporalmente ordenável

### Posição na Pipeline

O `CorrelationIdMiddleware` deve ser registrado **antes** de `UseExceptionHandler()` e de qualquer outro middleware, garantindo que até exceções não tratadas tenham o CorrelationId nos logs:

```csharp
// Program.cs — ordem da pipeline
app.UseMiddleware<CorrelationIdMiddleware>();  // ← primeiro
app.UseExceptionHandler();                     // ← segundo
// ... demais middlewares
```

### Transparência para Features

Features e Endpoints **nunca** acessam o CorrelationId diretamente. Ele é enriquecido via `LogContext.PushProperty` e aparece automaticamente em todo log emitido dentro do bloco `using`, sem que o desenvolvedor precise passá-lo como parâmetro:

```csharp
// Isso acontece automaticamente — o desenvolvedor NÃO precisa fazer isso:
// logger.LogInformation("... CorrelationId={CorrelationId}", correlationId);  ← DESNECESSÁRIO

// Basta escrever o log normalmente:
logger.LogInformation("[MeuEndpoint][Get] Processar requisição");
// O CorrelationId já aparece no output template: [{CorrelationId}]
```

---

## UserId e UserName

### O Que São

Os campos `UserId` e `UserName` identificam o usuário autenticado que está realizando a requisição. São extraídos do JWT Bearer Token durante a validação de autenticação.

### Como São Enriquecidos

O `AuthenticateFilter` é um `IAsyncActionFilter` ativado pelo atributo `[Authenticate]` nos Controllers. Quando o token é válido:

1. Extrai o Bearer Token do header `Authorization`
2. Valida o JWT via `ITokenService`
3. Obtém o `AuthenticatedUser` (Id e UserName) dos claims do token
4. Armazena o usuário em `HttpContext.Items` para acesso downstream (ex: cache por usuário)
5. Enriquece **todos os logs** subsequentes via `LogContext.PushProperty`

### Implementação do Filtro

```csharp
public sealed class AuthenticateFilter(
    ITokenService tokenService,
    ILogger<AuthenticateFilter> logger) : IAsyncActionFilter
{
    public const string AuthenticatedUserItemKey = "AuthenticatedUser";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        logger.LogInformation("[AuthenticateFilter][OnActionExecutionAsync] Validar Bearer Token da requisição");

        var token = ExtractBearerToken(context.HttpContext);

        if (token is null)
        {
            logger.LogWarning("[AuthenticateFilter][OnActionExecutionAsync] Retornar 401 - token ausente na requisição");
            context.Result = new ObjectResult(new ProblemDetails { /* ... */ }) { StatusCode = 401 };
            return;
        }

        var user = tokenService.ValidateToken(token);

        if (user is null)
        {
            logger.LogWarning("[AuthenticateFilter][OnActionExecutionAsync] Retornar 401 - token inválido ou expirado");
            context.Result = new ObjectResult(new ProblemDetails { /* ... */ }) { StatusCode = 401 };
            return;
        }

        context.HttpContext.Items[AuthenticatedUserItemKey] = user;

        using (LogContext.PushProperty("UserId", user.Id))
        using (LogContext.PushProperty("UserName", user.UserName))
        {
            logger.LogInformation(
                "[AuthenticateFilter][OnActionExecutionAsync] Prosseguir com requisição autenticada. UserId={UserId}, UserName={UserName}",
                user.Id, user.UserName);

            await next();
        }
    }
}
```

### Escopo de Enriquecimento

O enriquecimento de `UserId` e `UserName` ocorre **dentro** do escopo do `CorrelationId`, criando uma hierarquia de contexto:

```
CorrelationIdMiddleware
  └── LogContext: { CorrelationId }
       └── AuthenticateFilter
            └── LogContext: { CorrelationId, UserId, UserName }
                 └── Controller → UseCase → ApiClient → ...
                      (todos os logs recebem os 3 campos)
```

### Endpoints Sem Autenticação

Para endpoints que não possuem `[Authenticate]` (como `/health` e `/login`), os campos `UserId` e `UserName` aparecem **vazios** no template de log:

```
[23/03/2026 14:32:01.0000000] [019580a1-...] [] [HealthCheckEndpoint] Processar /health
```

Isso é comportamento esperado — o campo `[UserName]` fica vazio pois não há contexto de autenticação.

---

## Diagrama do Fluxo de Enriquecimento

```
Requisição HTTP
    │
    ▼
CorrelationIdMiddleware
    ├── Gera/resolve GUID v7
    ├── context.Items["CorrelationId"] = guid
    ├── response.Headers["X-Correlation-Id"] = guid
    └── LogContext.PushProperty("CorrelationId", guid)
         │
         ▼
    GlobalExceptionHandler (captura exceções não tratadas)
         │
         ▼
    Controller / Action
         │
         ├── [sem Authenticate] → POST /login, GET /health
         │    └── Logs com: { CorrelationId }
         │
         └── [com Authenticate] → demais endpoints
              │
              ▼
         AuthenticateFilter
              ├── Valida JWT Bearer Token
              ├── context.Items["AuthenticatedUser"] = user
              ├── LogContext.PushProperty("UserId", user.Id)
              └── LogContext.PushProperty("UserName", user.UserName)
                   │
                   ▼
              UseCase → ApiClient → ...
                   └── Logs com: { CorrelationId, UserId, UserName }
```

---

## Uso do AuthenticatedUser em Camadas Downstream

Além do enriquecimento de logs, o `AuthenticatedUser` armazenado em `HttpContext.Items` é utilizado pelo `CachedApiClient` para criar chaves de cache por usuário:

```csharp
// CachedOpenMeteoApiClient — acessa o usuário via IHttpContextAccessor
private int GetAuthenticatedUserId()
{
    var httpContext = httpContextAccessor.HttpContext;

    if (httpContext?.Items.TryGetValue(AuthenticateFilter.AuthenticatedUserItemKey, out var userObj) == true
        && userObj is AuthenticatedUser user)
    {
        return user.Id;
    }

    return 0;
}
```

A chave de cache resultante inclui o ID do usuário: `OpenMeteo:WeatherConditionsGet:1`, garantindo isolamento de cache entre usuários.

---

## Referências

- [01-padroes-de-logging.md](01-padroes-de-logging.md) — padrões de formato e storytelling
- [03-exemplo-requisicao-completa.md](03-exemplo-requisicao-completa.md) — exemplo ponta a ponta com os campos enriquecidos
- `Infra/Middlewares/CorrelationIdMiddleware.cs` — implementação do middleware
- `Infra/Security/AuthenticateFilter.cs` — implementação do filtro de autenticação
- `Infra/Correlation/GuidV7.cs` — utilitário de geração e validação de GUID v7
