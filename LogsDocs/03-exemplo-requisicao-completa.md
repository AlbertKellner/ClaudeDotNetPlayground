# Exemplo de Requisição Completa

## Visão Geral

Este documento demonstra o fluxo completo de logs gerados durante uma requisição real ao endpoint `GET /items/{id}`, percorrendo todas as camadas da aplicação: middleware, autenticação, endpoint, use case, cache e chamada a API externa.

---

## Cenário

- **Endpoint**: `GET /items/25`
- **Autenticação**: Bearer Token válido (usuário User1, id 1)
- **Cache**: miss na primeira requisição, hit na segunda
- **API externa**: External API (`GET /api/v2/items/25`)

---

## Fluxo de Execução

```
Requisição HTTP: GET /items/25
    │
    ├── 1. CorrelationIdMiddleware     → Gera GUID v7, enriquece logs
    ├── 2. AuthenticateFilter          → Valida JWT, enriquece UserId/UserName
    ├── 3. ItemGetByIdEndpoint          → Recebe requisição, delega ao UseCase
    ├── 4. ItemGetByIdUseCase           → Orquestra lógica de negócio
    ├── 5. CachedItemApiClient      → Verifica cache por usuário
    ├── 6. ItemApiClient            → Chama External API via Refit + Polly
    ├── 7. ItemGetByIdUseCase           → Mapeia resposta para model da Feature
    ├── 8. ItemGetByIdEndpoint          → Retorna HTTP 200 com dados
    └── 9. CorrelationIdMiddleware     → Finaliza com CorrelationId na resposta
```

---

## Logs da Primeira Requisição (Cache Miss)

Cada linha abaixo é um log real seguindo o padrão SNP-001, com o template `[Timestamp] [CorrelationId] [UserName] Mensagem`:

```
[23/03/2026 14:32:01.1234567] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [] [CorrelationIdMiddleware][InvokeAsync] Processar requisição e garantir CorrelationId
[23/03/2026 14:32:01.1235000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [] [CorrelationIdMiddleware][InvokeAsync] Prosseguir com CorrelationId enriquecido no contexto. CorrelationId=019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12
[23/03/2026 14:32:01.1240000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [] [AuthenticateFilter][OnActionExecutionAsync] Validar Bearer Token da requisição
[23/03/2026 14:32:01.1250000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [AuthenticateFilter][OnActionExecutionAsync] Prosseguir com requisição autenticada. UserId=1, UserName=User1
[23/03/2026 14:32:01.1260000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [ItemGetByIdEndpoint][Get] Processar requisicao GET /items/25
[23/03/2026 14:32:01.1270000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [ItemGetByIdUseCase][ExecuteAsync] Executar caso de uso de consulta de Item. ItemId=25
[23/03/2026 14:32:01.1280000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [ItemGetByIdUseCase][ExecuteAsync] Consultar External API. ItemId=25
[23/03/2026 14:32:01.1290000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [CachedItemApiClient][GetByIdAsync] Verificar cache para Item. ItemId=25
[23/03/2026 14:32:01.1300000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [CachedItemApiClient][GetByIdAsync] Cache miss. Consultar API externa. CacheKey=Item:ItemGetById:1:25
[23/03/2026 14:32:01.1310000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [ItemApiClient][GetByIdAsync] Executar requisição HTTP à External API. ItemId=25
[23/03/2026 14:32:01.5500000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [ItemApiClient][GetByIdAsync] Retornar resposta da External API. ItemName=sample-item
[23/03/2026 14:32:01.5510000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [CachedItemApiClient][GetByIdAsync] Armazenar resposta no cache. CacheKey=Item:ItemGetById:1:25, DurationSeconds=60
[23/03/2026 14:32:01.5520000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [ItemGetByIdUseCase][ExecuteAsync] Mapear resposta da External API para model da Feature
[23/03/2026 14:32:01.5530000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [ItemGetByIdUseCase][ExecuteAsync] Iterar tipos do Item. Count=2
[23/03/2026 14:32:01.5540000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [ItemGetByIdUseCase][ExecuteAsync] Iteracao de tipos concluida
[23/03/2026 14:32:01.5550000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [ItemGetByIdUseCase][ExecuteAsync] Iterar habilidades do Item. Count=2
[23/03/2026 14:32:01.5560000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [ItemGetByIdUseCase][ExecuteAsync] Iteracao de habilidades concluida
[23/03/2026 14:32:01.5570000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [ItemGetByIdUseCase][ExecuteAsync] Iterar stats do Item. Count=6
[23/03/2026 14:32:01.5580000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [ItemGetByIdUseCase][ExecuteAsync] Iteracao de stats concluida
[23/03/2026 14:32:01.5590000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [ItemGetByIdUseCase][ExecuteAsync] Retornar dados do Item. ItemId=25, ItemName=sample-item
[23/03/2026 14:32:01.5600000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [ItemGetByIdEndpoint][Get] Retornar resposta do endpoint com dados do Item. ItemId=25, ItemName=sample-item
[23/03/2026 14:32:01.5610000] [019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12] [User1] [CorrelationIdMiddleware][InvokeAsync] Retornar resposta com CorrelationId enriquecido. CorrelationId=019580a1-b2c3-7d4e-8f5a-6b7c8d9e0f12
```

### Análise do Fluxo (Cache Miss)

| # | Camada | Classe | Ação |
|---|--------|--------|------|
| 1 | Middleware | `CorrelationIdMiddleware` | Gera GUID v7, enriquece LogContext |
| 2 | Segurança | `AuthenticateFilter` | Valida JWT, enriquece UserId/UserName |
| 3 | Endpoint | `ItemGetByIdEndpoint` | Recebe a requisição, inicia storytelling |
| 4 | UseCase | `ItemGetByIdUseCase` | Orquestra caso de uso, prepara chamada à API |
| 5 | Cache | `CachedItemApiClient` | Verifica cache — **miss**, delega ao client real |
| 6 | ApiClient | `ItemApiClient` | Executa HTTP GET à External API via Refit |
| 7 | Cache | `CachedItemApiClient` | Armazena resultado no Memory Cache |
| 8 | UseCase | `ItemGetByIdUseCase` | Mapeia resposta, itera tipos/habilidades/stats |
| 9 | Endpoint | `ItemGetByIdEndpoint` | Retorna HTTP 200 com dados |
| 10 | Middleware | `CorrelationIdMiddleware` | Finaliza requisição |

---

## Logs da Segunda Requisição (Cache Hit)

A mesma requisição, feita dentro da janela de cache (60 segundos), gera logs significativamente mais curtos:

```
[23/03/2026 14:32:10.0000000] [019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c] [] [CorrelationIdMiddleware][InvokeAsync] Processar requisição e garantir CorrelationId
[23/03/2026 14:32:10.0010000] [019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c] [] [CorrelationIdMiddleware][InvokeAsync] Prosseguir com CorrelationId enriquecido no contexto. CorrelationId=019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c
[23/03/2026 14:32:10.0020000] [019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c] [] [AuthenticateFilter][OnActionExecutionAsync] Validar Bearer Token da requisição
[23/03/2026 14:32:10.0030000] [019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c] [User1] [AuthenticateFilter][OnActionExecutionAsync] Prosseguir com requisição autenticada. UserId=1, UserName=User1
[23/03/2026 14:32:10.0040000] [019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c] [User1] [ItemGetByIdEndpoint][Get] Processar requisicao GET /items/25
[23/03/2026 14:32:10.0050000] [019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c] [User1] [ItemGetByIdUseCase][ExecuteAsync] Executar caso de uso de consulta de Item. ItemId=25
[23/03/2026 14:32:10.0060000] [019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c] [User1] [ItemGetByIdUseCase][ExecuteAsync] Consultar External API. ItemId=25
[23/03/2026 14:32:10.0070000] [019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c] [User1] [CachedItemApiClient][GetByIdAsync] Verificar cache para Item. ItemId=25
[23/03/2026 14:32:10.0080000] [019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c] [User1] [CachedItemApiClient][GetByIdAsync] Retornar resposta do cache. CacheKey=Item:ItemGetById:1:25
[23/03/2026 14:32:10.0090000] [019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c] [User1] [ItemGetByIdUseCase][ExecuteAsync] Mapear resposta da External API para model da Feature
[23/03/2026 14:32:10.0100000] [019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c] [User1] [ItemGetByIdUseCase][ExecuteAsync] Iterar tipos do Item. Count=2
[23/03/2026 14:32:10.0110000] [019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c] [User1] [ItemGetByIdUseCase][ExecuteAsync] Iteracao de tipos concluida
[23/03/2026 14:32:10.0120000] [019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c] [User1] [ItemGetByIdUseCase][ExecuteAsync] Iterar habilidades do Item. Count=2
[23/03/2026 14:32:10.0130000] [019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c] [User1] [ItemGetByIdUseCase][ExecuteAsync] Iteracao de habilidades concluida
[23/03/2026 14:32:10.0140000] [019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c] [User1] [ItemGetByIdUseCase][ExecuteAsync] Iterar stats do Item. Count=6
[23/03/2026 14:32:10.0150000] [019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c] [User1] [ItemGetByIdUseCase][ExecuteAsync] Iteracao de stats concluida
[23/03/2026 14:32:10.0160000] [019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c] [User1] [ItemGetByIdUseCase][ExecuteAsync] Retornar dados do Item. ItemId=25, ItemName=sample-item
[23/03/2026 14:32:10.0170000] [019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c] [User1] [ItemGetByIdEndpoint][Get] Retornar resposta do endpoint com dados do Item. ItemId=25, ItemName=sample-item
[23/03/2026 14:32:10.0180000] [019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c] [User1] [CorrelationIdMiddleware][InvokeAsync] Retornar resposta com CorrelationId enriquecido. CorrelationId=019580a2-d4e5-7f6a-0b1c-2d3e4f5a6b7c
```

### Diferenças Cache Hit vs. Cache Miss

| Aspecto | Cache Miss | Cache Hit |
|---------|-----------|-----------|
| `CachedItemApiClient` log | "Cache miss. Consultar API externa" | "Retornar resposta do cache" |
| `ItemApiClient` invocado | Sim — chamada HTTP real | Não — pulado completamente |
| Armazenamento no cache | Sim — "Armazenar resposta no cache" | Não — já está no cache |
| Tempo de execução | ~400ms (rede) | ~1ms (memória) |

---

## Exemplo: Requisição ao Endpoint de Condições Climáticas com Coordenadas

- **Endpoint**: `GET /sample-query?latitude=-23.5475&longitude=-46.6361`
- **Autenticação**: Bearer Token válido (usuário User1, id 1)
- **Cache**: miss na primeira requisição

```
[23/03/2026 14:33:00.0000000] [019580c4-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [] [CorrelationIdMiddleware][InvokeAsync] Processar requisição e garantir CorrelationId
[23/03/2026 14:33:00.0010000] [019580c4-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [] [CorrelationIdMiddleware][InvokeAsync] Prosseguir com CorrelationId enriquecido no contexto. CorrelationId=019580c4-a1b2-7c3d-4e5f-6a7b8c9d0e1f
[23/03/2026 14:33:00.0020000] [019580c4-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [] [AuthenticateFilter][OnActionExecutionAsync] Validar Bearer Token da requisição
[23/03/2026 14:33:00.0030000] [019580c4-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [User1] [AuthenticateFilter][OnActionExecutionAsync] Prosseguir com requisição autenticada. UserId=1, UserName=User1
[23/03/2026 14:33:00.0040000] [019580c4-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [User1] [SampleQueryGetEndpoint][Get] Processar requisição GET /sample-query. Latitude=-23.5475, Longitude=-46.6361
[23/03/2026 14:33:00.0050000] [019580c4-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [User1] [SampleQueryGetUseCase][ExecuteAsync] Executar caso de uso de dados da consulta. Latitude=-23.5475, Longitude=-46.6361
[23/03/2026 14:33:00.0060000] [019580c4-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [User1] [SampleQueryGetUseCase][ExecuteAsync] Consultar API External Service. Latitude=-23.5475, Longitude=-46.6361
[23/03/2026 14:33:00.0070000] [019580c4-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [User1] [CachedExternalServiceApiClient][GetForecastAsync] Verificar cache para dados da consulta
[23/03/2026 14:33:00.0080000] [019580c4-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [User1] [CachedExternalServiceApiClient][GetForecastAsync] Cache miss. Consultar API externa. CacheKey=ExternalService:SampleQueryGet:1:-23.5475:-46.6361
[23/03/2026 14:33:00.0090000] [019580c4-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [User1] [ExternalServiceApiClient][GetForecastAsync] Executar requisição HTTP ao External Service. Latitude=-23.5475, Longitude=-46.6361
[23/03/2026 14:33:00.4500000] [019580c4-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [User1] [ExternalServiceApiClient][GetForecastAsync] Retornar resposta da External Service. Timezone=America/Sao_Paulo
[23/03/2026 14:33:00.4510000] [019580c4-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [User1] [CachedExternalServiceApiClient][GetForecastAsync] Armazenar resposta no cache. CacheKey=ExternalService:SampleQueryGet:1:-23.5475:-46.6361, DurationSeconds=10
[23/03/2026 14:33:00.4520000] [019580c4-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [User1] [SampleQueryGetUseCase][ExecuteAsync] Mapear resposta da External Service para model da Feature
[23/03/2026 14:33:00.4530000] [019580c4-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [User1] [SampleQueryGetUseCase][ExecuteAsync] Retornar dados da consulta obtidas da External Service. Timezone=America/Sao_Paulo
[23/03/2026 14:33:00.4540000] [019580c4-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [User1] [SampleQueryGetEndpoint][Get] Retornar resposta do endpoint com dados da consulta. Latitude=-23.5475, Longitude=-46.6361
[23/03/2026 14:33:00.4550000] [019580c4-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [User1] [CorrelationIdMiddleware][InvokeAsync] Retornar resposta com CorrelationId enriquecido. CorrelationId=019580c4-a1b2-7c3d-4e5f-6a7b8c9d0e1f
```

Note que:
- As coordenadas recebidas via query parameter aparecem nos logs do Endpoint e do UseCase
- A chave de cache inclui `userId:latitude:longitude`, permitindo cache independente por localização e por usuário
- O fluxo completo (middleware → auth → endpoint → use case → cache → API externa → mapeamento → retorno) é idêntico ao do Item, com a diferença de que não há iteração de listas

---

## Exemplo: Requisição com Token Inválido (401)

Quando o token é inválido, o fluxo é interrompido no `AuthenticateFilter`:

```
[23/03/2026 14:35:00.0000000] [019580b3-e5f6-7a8b-9c0d-1e2f3a4b5c6d] [] [CorrelationIdMiddleware][InvokeAsync] Processar requisição e garantir CorrelationId
[23/03/2026 14:35:00.0010000] [019580b3-e5f6-7a8b-9c0d-1e2f3a4b5c6d] [] [CorrelationIdMiddleware][InvokeAsync] Prosseguir com CorrelationId enriquecido no contexto. CorrelationId=019580b3-e5f6-7a8b-9c0d-1e2f3a4b5c6d
[23/03/2026 14:35:00.0020000] [019580b3-e5f6-7a8b-9c0d-1e2f3a4b5c6d] [] [AuthenticateFilter][OnActionExecutionAsync] Validar Bearer Token da requisição
[23/03/2026 14:35:00.0030000] [019580b3-e5f6-7a8b-9c0d-1e2f3a4b5c6d] [] [AuthenticateFilter][OnActionExecutionAsync] Retornar 401 - token inválido ou expirado
[23/03/2026 14:35:00.0040000] [019580b3-e5f6-7a8b-9c0d-1e2f3a4b5c6d] [] [CorrelationIdMiddleware][InvokeAsync] Retornar resposta com CorrelationId enriquecido. CorrelationId=019580b3-e5f6-7a8b-9c0d-1e2f3a4b5c6d
```

Note que:
- O campo `[UserName]` permanece vazio (token inválido, sem autenticação)
- O nível de log muda para `Warning` no ponto de falha
- O fluxo é interrompido — Endpoint, UseCase e ApiClient nunca são invocados

---

## Exemplo: Requisição ao Endpoint de Login (Sem Autenticação)

O endpoint `POST /login` não possui `[Authenticate]`, então o `AuthenticateFilter` não é executado:

```
[23/03/2026 14:30:00.0000000] [019580a0-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [] [CorrelationIdMiddleware][InvokeAsync] Processar requisição e garantir CorrelationId
[23/03/2026 14:30:00.0010000] [019580a0-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [] [CorrelationIdMiddleware][InvokeAsync] Prosseguir com CorrelationId enriquecido no contexto. CorrelationId=019580a0-a1b2-7c3d-4e5f-6a7b8c9d0e1f
[23/03/2026 14:30:00.0020000] [019580a0-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [] [UserLoginEndpoint][Post] Processar requisição POST /login
[23/03/2026 14:30:00.0030000] [019580a0-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [] [UserLoginUseCase][ExecuteAsync] Executar caso de uso de login. UserName=User1
[23/03/2026 14:30:00.0040000] [019580a0-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [] [UserLoginUseCase][ExecuteAsync] Validar credenciais do usuário
[23/03/2026 14:30:00.0050000] [019580a0-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [] [UserLoginUseCase][ExecuteAsync] Gerar token JWT para o usuário autenticado
[23/03/2026 14:30:00.0060000] [019580a0-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [] [UserLoginUseCase][ExecuteAsync] Retornar token gerado. UserName=User1
[23/03/2026 14:30:00.0070000] [019580a0-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [] [UserLoginEndpoint][Post] Retornar resposta do endpoint com token JWT
[23/03/2026 14:30:00.0080000] [019580a0-a1b2-7c3d-4e5f-6a7b8c9d0e1f] [] [CorrelationIdMiddleware][InvokeAsync] Retornar resposta com CorrelationId enriquecido. CorrelationId=019580a0-a1b2-7c3d-4e5f-6a7b8c9d0e1f
```

Note que o campo `[UserName]` permanece vazio em todos os logs, pois não há autenticação neste endpoint.

---

## Referências

- [01-padroes-de-logging.md](01-padroes-de-logging.md) — regras de formato e storytelling
- [02-enriquecimento-de-contexto.md](02-enriquecimento-de-contexto.md) — CorrelationId, UserId e UserName
- [04-integracao-datadog.md](04-integracao-datadog.md) — como estes logs chegam ao Datadog
