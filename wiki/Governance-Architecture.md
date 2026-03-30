# Arquitetura

## Descrição

Esta página documenta o estilo arquitetural adotado (Vertical Slice Architecture com segregação Command/Query), a estrutura de pastas, os componentes por Slice, o fluxo de requisições e as responsabilidades de cada camada. Deve ser consultada ao entender a estrutura do projeto ou ao adicionar novas features.

## Contexto

O projeto adota **Vertical Slice Architecture** com segregação explícita de operações de leitura (**Query**) e escrita (**Command**). Cada funcionalidade é implementada como uma Slice vertical isolada, contendo todos os artefatos necessários dentro da sua própria pasta, sob `Features/Query` ou `Features/Command`. Não há camadas horizontais globais (ex.: pasta `Services/` ou `Repositories/` global). Lógica genuinamente compartilhada entre Slices reside em `Shared/`.

---

## Estilo Arquitetural

A Vertical Slice Architecture organiza o código por funcionalidade, não por camada técnica. Cada Slice contém todos os artefatos necessários para seu funcionamento: endpoint, use case, interfaces, models, repository e scripts SQL. Slices não se comunicam diretamente entre si.

A segregação Command/Query classifica toda funcionalidade como:
- **Query**: operação de leitura — não altera estado. Reside em `Features/Query/`.
- **Command**: operação de escrita — altera estado. Reside em `Features/Command/`.

A classificação é baseada na **intenção da operação**, não no verbo HTTP.

---

## Estrutura de Pastas

```
src/Starter.Template.AOT.Api/
├── Features/
│   ├── Query/
│   │   └── <NomeDaFeature>/
│   │       ├── <NomeDaFeature>Endpoint/
│   │       ├── <NomeDaFeature>UseCase/
│   │       ├── <NomeDaFeature>Interfaces/
│   │       └── <NomeDaFeature>Models/
│   └── Command/
│       └── <NomeDaFeature>/
│           ├── <NomeDaFeature>Endpoint/
│           ├── <NomeDaFeature>UseCase/
│           ├── <NomeDaFeature>Interfaces/
│           └── <NomeDaFeature>Models/
│
├── Infra/
│   ├── Correlation/          # GuidV7 — geração e validação de GUID v7
│   ├── ExceptionHandling/    # GlobalExceptionHandler — Problem Details (RFC 7807)
│   ├── HealthChecks/         # DatadogAgentHealthCheck — verificação do Datadog Agent
│   ├── Json/                 # AppJsonContext — serialização AOT-compatível
│   ├── Logging/              # DatadogHttpSink, DatadogLogEntry — logs diretos ao Datadog
│   ├── Middlewares/          # CorrelationIdMiddleware — GUID v7 por request
│   ├── ModelBinding/         # Providers AOT-compatíveis (NullModelBinderProvider, FallbackSimpleTypeModelBinderProvider, EnhancedModelMetadataActivator)
│   ├── ModelValidation/      # NoOpObjectModelValidator — validação AOT-compatível
│   └── Security/             # JWT, AuthenticateFilter, TokenService, AuthenticateAttribute
│
└── Shared/
    └── ExternalApi/
        └── <Servico>/        # Integração com API HTTP externa (DA-017)
            ├── I<Servico>Api.cs
            ├── I<Servico>ApiClient.cs
            ├── <Servico>ApiClient.cs
            ├── Cached<Servico>ApiClient.cs
            └── Models/
```

---

## Componentes por Slice

Cada Slice segue uma estrutura padronizada de componentes:

| Componente | Pasta | Responsabilidade |
|---|---|---|
| **Endpoint** (Controller) | `<Feature>Endpoint/` | Controller com Actions — orquestra request/response, define status codes, escreve logs de storytelling. Sem lógica de negócio. |
| **UseCase** | `<Feature>UseCase/` | Orquestração da lógica de negócio da Slice. Depende apenas de interfaces. |
| **Interfaces** | `<Feature>Interfaces/` | Contratos para repositórios e integrações externas ao UseCase. |
| **Models** | `<Feature>Models/` | Input (validação de payload), Output (contrato de saída), Entity (quando aplicável). Residem exclusivamente na Slice. |
| **Repository** | `<Feature>Repository/` | Acesso a dados e materialização de objetos de domínio. Contém Scripts SQL quando aplicável. |

Nem todos os componentes são obrigatórios — apenas os necessários para a Slice existem (regra de existência condicional).

---

## Fluxo de Requisição

```
Request HTTP
  └── CorrelationIdMiddleware (garante GUID v7; abre LogContext com CorrelationId)
        └── GlobalExceptionHandler (captura exceções não tratadas; retorna Problem Details RFC 7807)
              └── Controller / Action (pasta Endpoint)
                    ├── [sem Authenticate] endpoints públicos (ex: POST /login, GET /health)
                    └── [com Authenticate] demais endpoints → AuthenticateFilter (valida JWT; enriquece LogContext)
                          └── UseCase
                                └── Repository / ApiClient (via Interface)
                                      └── Banco de dados / serviço externo
```

O enrichment do Serilog é transversal:
- Todo log dentro do scope do `CorrelationIdMiddleware` recebe `{ CorrelationId: <guid-v7> }`
- Todo log dentro do scope do `AuthenticateFilter` (endpoints protegidos) recebe `{ UserId: <int>, UserName: <string> }`

---

## Responsabilidades por Camada

| Camada | Contém | Não contém |
|---|---|---|
| **Endpoint** | Orquestração request/response, status codes, logs de storytelling, delegação ao UseCase | Lógica de negócio, acesso a dados, validação de payload |
| **UseCase** | Lógica de negócio, orquestração de chamadas via interfaces | Acesso direto a infraestrutura, detalhes de framework HTTP |
| **Repository** | Acesso a dados, materialização de entidades, scripts SQL | Lógica de negócio, validação de payload |
| **Models** | Input (validação), Output (contrato de saída), Entity (domínio) | Lógica de orquestração |
| **Infra** | Middlewares, exception handling, segurança (JWT), correlação, JSON AOT, model binding AOT | Lógica de negócio, lógica especializada de Features |
| **Shared** | Clientes HTTP externos (Refit + Polly), abstrações genéricas, utilitários | Lógica especializada para uma única Slice, Models de Features |

---

## Features Implementadas

| Feature | Tipo | Endpoint | Autenticação | Descrição |
|---|---|---|---|---|
| Health | Infra | `GET /health` | Não | Verificação de disponibilidade da aplicação e do Datadog Agent |

> Features adicionais serão registradas aqui conforme forem implementadas.

---

## Componentes de Infraestrutura (`Infra/`)

Componentes transversais que suportam a aplicação sem conter lógica de negócio:

| Componente | Localização | Propósito |
|---|---|---|
| `CorrelationIdMiddleware` | `Infra/Middlewares/` | Garante GUID v7 por request; enriquece Serilog LogContext; opaco para Features |
| `GuidV7` | `Infra/Correlation/` | Geração (`Guid.CreateVersion7()`) e validação de GUID v7 (uso interno de Infra) |
| `GlobalExceptionHandler` | `Infra/ExceptionHandling/` | Handler centralizado de exceções; retorna Problem Details (RFC 7807) |
| `DatadogAgentHealthCheck` | `Infra/HealthChecks/` | Verifica disponibilidade do Datadog Agent via HTTP; determina Healthy/Degraded/Unhealthy |
| `AppJsonContext` | `Infra/Json/` | `JsonSerializerContext` source-generated para serialização AOT-compatível |
| `DatadogHttpSink` | `Infra/Logging/` | Serilog `ILogEventSink`: envia logs diretamente ao Datadog via HTTP; batching assíncrono via Channel |
| `DatadogLogEntry` | `Infra/Logging/` | Modelo de entrada de log + `DatadogLogJsonContext` para serialização AOT |
| `NullModelBinderProvider` | `Infra/ModelBinding/` | Substitui providers incompatíveis com AOT (TryParse, FloatingPoint) |
| `FallbackSimpleTypeModelBinderProvider` | `Infra/ModelBinding/` | Substitui `SimpleTypeModelBinderProvider` para compatibilidade AOT |
| `EnhancedModelMetadataActivator` | `Infra/ModelBinding/` | Workaround AOT: ativa `IsEnhancedModelMetadataSupported` antes do primeiro request |
| `NoOpObjectModelValidator` | `Infra/ModelValidation/` | Substitui `IObjectModelValidator` padrão (reflection-based) por implementação AOT-compatível |
| `ITokenService` | `Infra/Security/` | Contrato de geração e validação de JWT Bearer Token |
| `AuthenticatedUser` | `Infra/Security/` | Modelo de usuário autenticado extraído do token (Id, UserName) |
| `TokenService` | `Infra/Security/` | Implementação JWT HS256: geração e validação de Bearer Token |
| `AuthenticateFilter` | `Infra/Security/` | `IAsyncActionFilter`: valida Bearer Token, retorna 401 se inválido, enriquece LogContext com UserId e UserName |
| `AuthenticateAttribute` | `Infra/Security/` | `TypeFilterAttribute`: decorador `[Authenticate]` para Controllers |

---

## Restrições Arquiteturais

- Slices não se comunicam diretamente entre si
- `Shared/` não depende de `Features/`
- Lógica de negócio não pode estar em Endpoints nem em Repositories
- Validação de payload deve estar no objeto `Input` de cada Slice
- Models de Input e Output de cada Feature residem exclusivamente em `<Feature>Models/`
- Features não devem usar models de `Shared/` (incluindo `Shared/ExternalApi/*/Models/`) como tipo de retorno de seus Use Cases ou Endpoints

---

## Referências

- [Padrões de Desenvolvimento](Governance-Development-Patterns) — padrões que determinam a organização
- [Segurança](Governance-Security) — mecanismos de autenticação e autorização
- [Observabilidade](Governance-Observability) — logging, tracing e métricas
- [Testes](Governance-Testing) — estratégia e padrões de teste
