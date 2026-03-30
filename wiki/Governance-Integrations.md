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

Cada integração em `appsettings.json` segue a estrutura `HttpRequest` + `CircuitBreaker` + `EndpointCache`:

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

> **Estado atual**: nenhuma integração com API externa implementada. Integrações serão adicionadas aqui conforme features forem desenvolvidas.

---

## Referências

- [Arquitetura](Governance-Architecture) — visão geral da arquitetura e componentes
- [Padrões de Desenvolvimento](Governance-Development-Patterns) — padrões técnicos adotados
