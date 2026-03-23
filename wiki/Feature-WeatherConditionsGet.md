# Condições Climáticas por Coordenadas

## Descrição

Documenta o endpoint de consulta de condições climáticas (`GET /weather-conditions?latitude={lat}&longitude={lng}`), que retorna dados em tempo real de qualquer localização geográfica via API Open-Meteo. Esta página cobre o contrato HTTP, o comportamento de cache por usuário e coordenadas, a estratégia de resiliência com Polly, e a regra de negócio RN-004. Consulte quando precisar entender a integração com a Open-Meteo ou o padrão de cache. Relaciona-se com [Integrações](Governance-Integrations) (padrão Refit + Polly) e [Segurança](Governance-Security) (autenticação obrigatória).

## Resumo

Consulta as condições climáticas atuais de uma localização geográfica especificada por latitude e longitude a partir da API Open-Meteo e retorna o payload completo da resposta, preservando a estrutura original sem filtragem ou mapeamento parcial.

## Autenticação

Sim. Requer Bearer Token JWT válido no header `Authorization`. Ver [Segurança](Governance-Security).

## Contrato de Entrada

| Campo | Valor |
|-------|-------|
| **Método** | `GET` |
| **Rota** | `/weather-conditions` |
| **Headers** | `Authorization: Bearer <token>` (obrigatório) |
| **Body** | Não aplicável |
| **Query params** | `latitude` (double, obrigatório), `longitude` (double, obrigatório) |

### Exemplo de chamada

```
GET /weather-conditions?latitude=-23.5475&longitude=-46.6361
Authorization: Bearer <token>
```

## Contrato de Saída

### HTTP 200 — Condições climáticas retornadas com sucesso

Payload completo retornado pela API Open-Meteo, preservando a estrutura original:

```json
{
  "latitude": -23.5475,
  "longitude": -46.6361,
  "generationtime_ms": 0.123,
  "utc_offset_seconds": 0,
  "timezone": "GMT",
  "timezone_abbreviation": "GMT",
  "elevation": 760.0,
  "current_units": {
    "time": "iso8601",
    "interval": "seconds",
    "temperature_2m": "°C",
    "relative_humidity_2m": "%",
    "apparent_temperature": "°C",
    "is_day": "",
    "precipitation": "mm",
    "rain": "mm",
    "showers": "mm",
    "weather_code": "wmo code",
    "cloud_cover": "%",
    "wind_speed_10m": "km/h",
    "wind_direction_10m": "°"
  },
  "current": {
    "time": "2026-03-16T12:00",
    "interval": 900,
    "temperature_2m": 25.0,
    "relative_humidity_2m": 80,
    "apparent_temperature": 26.5,
    "is_day": 1,
    "precipitation": 0.0,
    "rain": 0.0,
    "showers": 0.0,
    "weather_code": 1,
    "cloud_cover": 25,
    "wind_speed_10m": 12.0,
    "wind_direction_10m": 180
  }
}
```

### HTTP 401 — Token ausente ou inválido

```json
{
  "type": "...",
  "title": "Unauthorized",
  "status": 401,
  "detail": "..."
}
```

## Comportamento

Conforme [RN-004](Domain-Business-Rules):

- As coordenadas são recebidas como parâmetros de query (`latitude` e `longitude`)
- Os campos de condição atual consultados são: `temperature_2m`, `relative_humidity_2m`, `apparent_temperature`, `is_day`, `precipitation`, `rain`, `showers`, `weather_code`, `cloud_cover`, `wind_speed_10m`, `wind_direction_10m`
- O payload retornado pela Open-Meteo é entregue integralmente ao cliente, sem filtragem ou redução de campos

### Resiliência

A integração com a API Open-Meteo usa Polly v8 com:

| Tentativa | Timeout por tentativa | Espera antes da próxima |
|-----------|-----------------------|-------------------------|
| 1ª | 3 segundos | 3 segundos |
| 2ª | 3 segundos | 6 segundos |
| 3ª | 3 segundos | 12 segundos |
| 4ª (última) | 3 segundos | — (retorna erro) |

## Endpoints Consumidos

Esta feature consome a API Open-Meteo via Refit com resiliência Polly e Memory Cache.

### `GET /v1/forecast` — Open-Meteo

| Aspecto | Valor |
|---------|-------|
| **Serviço** | Open-Meteo |
| **Rota** | `GET /v1/forecast` |
| **Interface Refit** | `IOpenMeteoApi` |
| **Cliente** | `OpenMeteoApiClient` (HTTP) → `CachedOpenMeteoApiClient` (cache) |

#### Configuração HttpRequest

| Propriedade | Caminho no JSON | Valor |
|-------------|----------------|-------|
| URL Base | `ExternalApi:OpenMeteo:HttpRequest:BaseUrl` | `https://api.open-meteo.com` |

#### Configuração CircuitBreaker (Polly)

| Propriedade | Caminho no JSON | Valor |
|-------------|----------------|-------|
| Tentativas máximas | `ExternalApi:OpenMeteo:CircuitBreaker:MaxRetryAttempts` | `3` |
| Delay entre tentativas | `ExternalApi:OpenMeteo:CircuitBreaker:DelaySeconds` | `3` |
| Tipo de backoff | `ExternalApi:OpenMeteo:CircuitBreaker:BackoffType` | `Exponential` |

#### Configuração EndpointCache (Memory Cache)

| Propriedade | Caminho no JSON | Valor |
|-------------|----------------|-------|
| Duração do cache | `ExternalApi:OpenMeteo:EndpointCache:WeatherConditionsGet:DurationSeconds` | `10` |
| Tipo de expiração | `ExternalApi:OpenMeteo:EndpointCache:WeatherConditionsGet:ExpirationType` | `Absolute` |
| Chave de cache | *(definida no código)* | `OpenMeteo:WeatherConditionsGet:{userId}:{latitude}:{longitude}` |

O cache é por usuário autenticado e por coordenadas. Cada combinação de usuário + latitude + longitude tem seu próprio cache. Ao expirar o tempo configurado, a próxima requisição faz nova chamada HTTP à Open-Meteo e recicla o cache.

---

## Testes Automatizados

| Arquivo | Cobertura |
|---------|-----------|
| `WeatherConditionsGetEndpointTests.cs` | Logs do endpoint, retorno HTTP 200 com payload correto |
| `WeatherConditionsGetUseCaseTests.cs` | Logs do use case, coordenadas passadas à API, retorno sem mapeamento parcial |
| `OpenMeteoApiClientTests.cs` | Logs do cliente HTTP, delegação correta para a interface Refit |
| `CachedOpenMeteoApiClientTests.cs` | Cache hit/miss, isolamento por usuário, logs de cache, armazenamento |

## BDD

Nenhum cenário BDD definido para esta funcionalidade no momento.

## Referências

- [Regras de Negócio](Domain-Business-Rules) — RN-004
- [Integrações](Governance-Integrations) — padrão Refit + Polly e Memory Cache
- [Segurança](Governance-Security) — autenticação obrigatória
- [Arquitetura](Governance-Architecture) — posição no fluxo de request
