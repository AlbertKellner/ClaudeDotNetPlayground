# Integrações

## Descrição

Documenta o padrão de integração com APIs HTTP externas (Refit + Polly) e a estratégia de Memory Cache. Deve ser consultado ao adicionar novas integrações com APIs externas.

## Contexto

O projeto segue um padrão padronizado para toda integração com API HTTP externa, definido em DA-017. Cada integração reside em `Shared/ExternalApi/<Servico>/` e utiliza Refit para clientes HTTP tipados, Polly v8 para resiliência e, opcionalmente, Memory Cache para reduzir chamadas repetidas. O padrão é AOT-compatível e requer `JsonSerializerContext` source-generated para desserialização.

---

## Padrão Shared/ExternalApi (DA-017)

### Estrutura por Serviço

Cada integração segue a mesma estrutura de arquivos:

```
Shared/ExternalApi/<Servico>/
├── I<Servico>Api.cs                    # Interface Refit (contrato HTTP; rota hardcoded)
├── I<Servico>ApiClient.cs              # Interface de serviço (Features injetam este contrato)
├── <Servico>ApiClient.cs               # Implementação: usa I<Servico>Api; aplica logging SNP-001
├── Cached<Servico>ApiClient.cs         # Decorator de cache (quando aplicável)
├── <Servico>AuthenticationHandler.cs   # DelegatingHandler (quando API requer autenticação)
└── Models/
    ├── <Servico>Input.cs               # Parâmetros da requisição
    └── <Servico>Output.cs              # Modelo de resposta + JsonSerializerContext
```

### Tecnologias Utilizadas

| Componente | Tecnologia | Propósito |
|---|---|---|
| Cliente HTTP tipado | Refit (`Refit.HttpClientFactory`) | Interfaces decoradas com atributos HTTP; implementação source-generated (AOT-compatível) |
| Resiliência | Polly v8 via `Microsoft.Extensions.Http.Resilience` | Retry exponencial + timeout por tentativa, integrado ao `IHttpClientBuilder` |
| Serialização | `JsonSerializerContext` source-generated | Compatibilidade com Native AOT |

### Configuração

- `BaseAddress` de cada integração é configurado no `appsettings.json`
- Rotas específicas são codificadas diretamente nas interfaces Refit
- O logging SNP-001 (storytelling por classe e método) é obrigatório em toda implementação de `<Servico>ApiClient`

---

## Memory Cache (DA-018)

### Padrão Decorator

O cache é implementado usando o padrão Decorator com `IMemoryCache`:

- `Cached<Servico>ApiClient` implementa `I<Servico>ApiClient`
- Envolve o `<Servico>ApiClient` original de forma transparente para as Features
- A Feature injeta `I<Servico>ApiClient` via DI e recebe o decorator automaticamente

### Chave de Cache

- A chave de cache utiliza o **ID do usuário autenticado** como componente
- Isso garante isolamento entre usuários e evita vazamento de dados
- A chave é definida no código, não configurável via JSON

### Configuração

Duração e tipo de expiração são configuráveis via `appsettings.json` na seção `EndpointCache`:

```json
{
  "ExternalApi": {
    "<Servico>": {
      "EndpointCache": {
        "Duration": 300,
        "ExpirationType": "Sliding"
      }
    }
  }
}
```

### Fluxo de Cache

```
Requisição autenticada
    └── CachedXxxApiClient
            ├── Cache hit → retorna resposta cacheada (sem chamada à API externa)
            └── Cache miss → XxxApiClient → API externa → armazena no cache → retorna resposta
```

---

## Estrutura de Configuração

Cada integração em `appsettings.json` segue a estrutura `HttpRequest` + `CircuitBreaker` + `EndpointCache`, com valores que variam por serviço:

### Valores reais por serviço

| Serviço | `DelaySeconds` | `MaxRetryAttempts` | `BackoffType` | Cache Duration | Cache Type |
|---|---|---|---|---|---|
| **OpenMeteo** | 3 | 3 | Exponential | 10s | Absolute |
| **GitHub** | 5 | 3 | Exponential | 60s | Absolute |
| **PokéAPI** | 3 | 3 | Exponential | 60s | Absolute |

> **Nota**: O GitHub utiliza delay de retry maior (5s vs 3s) devido ao rate limiting mais restritivo da API.

### Estrutura do `appsettings.json`

```json
{
  "ExternalApi": {
    "<Servico>": {
      "HttpRequest": {
        "BaseUrl": "https://api.exemplo.com"
      },
      "CircuitBreaker": {
        "MaxRetryAttempts": 3,
        "DelaySeconds": 3,
        "BackoffType": "Exponential"
      },
      "EndpointCache": {
        "<NomeDaFeature>": {
          "DurationSeconds": 60,
          "ExpirationType": "Absolute"
        }
      }
    }
  }
}
```

---

## Integrações Ativas

### Open-Meteo

| Propriedade | Valor |
|---|---|
| Serviço | Condições climáticas atuais de São Paulo |
| Autenticação | Nenhuma (API pública) |
| Rota | `GET /v1/forecast` |
| Cache | Sim, por usuário autenticado |
| Feature | [Condições Climáticas](Feature-WeatherConditionsGet) |

### GitHub API

| Propriedade | Valor |
|---|---|
| Serviço | Pesquisa de repositórios da conta AlbertKellner |
| Autenticação | PAT (opcional, via `DelegatingHandler`) |
| Rota | `GET /users/{username}/repos` |
| Cache | Sim, por usuário autenticado |
| Paginação | Automática |
| Feature | [Pesquisa GitHub](Feature-GitHubRepoSearch) |

### PokéAPI

| Propriedade | Valor |
|---|---|
| Serviço | Consulta de Pokémon por ID |
| Autenticação | Nenhuma (API pública) |
| Rota | `GET /api/v2/pokemon/{id}` |
| Cache | Sim, por usuário autenticado + ID do Pokémon |
| Feature | [Pokémon](Feature-PokemonGet) |

---

## Referências

- [Arquitetura](Governance-Architecture) — visão geral da arquitetura e componentes
- [Padrões de Desenvolvimento](Governance-Development-Patterns) — padrões técnicos adotados
- [Condições Climáticas](Feature-WeatherConditionsGet) — feature que utiliza integração Open-Meteo
- [Pesquisa GitHub](Feature-GitHubRepoSearch) — feature que utiliza integração GitHub API
- [Pokémon](Feature-PokemonGet) — feature que utiliza integração PokéAPI
