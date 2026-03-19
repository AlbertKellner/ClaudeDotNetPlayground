# Condições Climáticas de São Paulo

## Resumo

Consulta as condições climáticas atuais do município de São Paulo a partir da API Open-Meteo e retorna o payload completo da resposta, preservando a estrutura original sem filtragem ou mapeamento parcial.

## Autenticação

Sim. Requer Bearer Token JWT válido no header `Authorization`. Ver [Autenticação JWT](Infra-Authentication).

## Contrato de Entrada

| Campo | Valor |
|-------|-------|
| **Método** | `GET` |
| **Rota** | `/weather-conditions` |
| **Headers** | `Authorization: Bearer <token>` (obrigatório) |
| **Body** | Não aplicável |
| **Query params** | Nenhum (coordenadas e campos são fixos na implementação) |

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

Conforme [RN-004](Business-Rules):

- As coordenadas do município de São Paulo são fixas na implementação: latitude `-23.5475`, longitude `-46.6361` (centro conforme a Prefeitura de São Paulo)
- Os campos de condição atual consultados são: `temperature_2m`, `relative_humidity_2m`, `apparent_temperature`, `is_day`, `precipitation`, `rain`, `showers`, `weather_code`, `cloud_cover`, `wind_speed_10m`, `wind_direction_10m`
- O payload retornado pela Open-Meteo é entregue integralmente ao cliente, sem filtragem ou redução de campos

### Cache em Memória

Conforme [RN-006](Business-Rules):

- A resposta da API Open-Meteo é cacheada em memória por usuário
- A chave de cache é `WeatherConditionsGet:{userId}` (definida no código, não em configuração)
- O tempo de expiração padrão é 10 segundos, configurável em `appsettings.json` (`ExternalApi:OpenMeteo:EndpointCache:WeatherConditionsGet:DurationSeconds`)
- O tipo de renovação é fixo (AbsoluteExpiration): ao expirar, o cache é reciclado
- Chamadas repetidas dentro do período de cache retornam a resposta cacheada sem consultar a API externa

### Resiliência

A integração com a API Open-Meteo usa Polly v8 com:

| Tentativa | Timeout por tentativa | Espera antes da próxima |
|-----------|-----------------------|-------------------------|
| 1ª | 3 segundos | 3 segundos |
| 2ª | 3 segundos | 6 segundos |
| 3ª | 3 segundos | 12 segundos |
| 4ª (última) | 3 segundos | — (retorna erro) |

## Testes Automatizados

| Arquivo | Cobertura |
|---------|-----------|
| `WeatherConditionsGetEndpointTests.cs` | Logs do endpoint, retorno HTTP 200 com payload correto |
| `WeatherConditionsGetUseCaseTests.cs` | Logs do use case, retorno de `OpenMeteoOutput` sem mapeamento parcial |
| `OpenMeteoApiClientTests.cs` | Logs do cliente, delegação correta para a interface Refit, cache hit/miss por usuário |

## BDD

Nenhum cenário BDD definido para esta funcionalidade no momento.
