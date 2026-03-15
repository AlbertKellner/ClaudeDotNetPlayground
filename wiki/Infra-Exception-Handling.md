# Tratamento de Exceções

## Resumo

Mecanismo centralizado de captura e tratamento de exceções não tratadas na aplicação. Intercepta qualquer exceção que não tenha sido tratada explicitamente pelo código e retorna uma resposta padronizada em formato **Problem Details** (RFC 7807).

## Como Funciona

O `GlobalExceptionHandler` é registrado como middleware na pipeline do ASP.NET Core. Quando uma exceção não tratada ocorre em qualquer ponto do processamento da requisição:

1. A exceção é capturada pelo handler
2. A exceção é registrada nos logs com nível `Error`
3. O status HTTP da resposta é definido como `500 Internal Server Error`
4. Uma resposta em formato Problem Details é retornada ao cliente

## Formato da Resposta de Erro

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.6.1",
  "title": "An unexpected error occurred",
  "status": 500
}
```

O corpo segue o padrão **Problem Details for HTTP APIs** (RFC 7807).

## Comportamento

- Captura **todas** as exceções não tratadas, independentemente de onde ocorram na pipeline
- Registra a exceção completa nos logs com nível `Error` antes de responder
- O log inclui automaticamente o `CorrelationId` do request (via [Correlation ID](Infra-Correlation-ID)), permitindo rastrear o erro nos logs
- Retorna sempre `HTTP 500` para exceções inesperadas
- Não expõe detalhes internos da exceção (como stack trace) na resposta ao cliente

## Transparência para Features

As Features não precisam implementar blocos `try/catch` para exceções genéricas. O `GlobalExceptionHandler` garante que qualquer exceção não tratada seja capturada e respondida de forma padronizada.

## Componentes Envolvidos

| Componente | Localização | Responsabilidade |
|------------|-------------|-----------------|
| `GlobalExceptionHandler` | `Infra/ExceptionHandling/` | Implementação do `IExceptionHandler` do ASP.NET Core |

## Relação com Arquitetura

Consulte [Arquitetura](Architecture) para entender onde o `GlobalExceptionHandler` se posiciona na pipeline de processamento de requests.
