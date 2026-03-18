# Arquitetura

## Estilo Arquitetural

O projeto adota **Vertical Slice Architecture** com segregação explícita entre operações de leitura e escrita.

Cada funcionalidade é implementada como uma **Slice vertical isolada**, contendo todos os artefatos necessários dentro da sua própria pasta. Não há camadas horizontais globais (como `Services/` ou `Repositories/` compartilhadas). Lógica genuinamente reutilizável entre Slices reside em `Shared/`.

---

## Estrutura de Pastas

```
src/ClaudeDotNetPlayground/
├── Features/
│   ├── Command/                          # Slices de escrita (alteram estado)
│   │   └── UserLogin/
│   │       ├── UserLoginEndpoint/        # Controller + Action
│   │       ├── UserLoginUseCase/         # Orquestração da lógica de negócio
│   │       └── UserLoginModels/          # Contratos de entrada e saída
│   └── Query/                            # Slices de leitura (não alteram estado)
│       ├── TestGet/
│       │   ├── TestGetEndpoint/          # Controller + Action
│       │   └── TestGetUseCase/           # Orquestração da lógica de negócio
│       └── WeatherConditionsGet/
│           ├── WeatherConditionsGetEndpoint/  # Controller + Action
│           └── WeatherConditionsGetUseCase/   # Orquestração da lógica de negócio
├── Infra/
│   ├── Correlation/                      # Utilitário de GUID v7
│   ├── ExceptionHandling/                # Handler centralizado de exceções
│   ├── HealthChecks/                     # Verificação do Datadog Agent
│   ├── Middlewares/                      # Middlewares transversais
│   └── Security/                         # Componentes de autenticação JWT
└── Shared/                               # Abstrações compartilhadas entre Slices
    └── ExternalApi/
        └── OpenMeteo/                    # Cliente HTTP para API Open-Meteo (Refit + Polly)
```

---

## Componentes de uma Slice

Cada Slice contém os artefatos específicos da sua funcionalidade:

| Componente | Responsabilidade |
|------------|-----------------|
| **Endpoint** | Controller + Action — recebe a requisição HTTP, chama o UseCase, retorna a resposta |
| **UseCase** | Orquestra a lógica de negócio da Slice |
| **Models** | Contratos de entrada (`Input`), saída (`Output`) e entidades de domínio |
| **Repository** *(quando aplicável)* | Acesso a dados e materialização de entidades |
| **Interfaces** *(quando aplicável)* | Contratos para dependências externas ao UseCase |

---

## Fluxo de Request

Todo request HTTP percorre a seguinte sequência:

```
Requisição HTTP
    └── CorrelationIdMiddleware
            Garante GUID v7 por request
            Enriquece logs com CorrelationId
            Adiciona X-Correlation-Id na resposta
        └── GlobalExceptionHandler
                Captura exceções não tratadas
                Retorna 500 Problem Details
            └── Controller / Action (Endpoint)
                    ├── [sem autenticação] POST /login → UserLoginUseCase
                    ├── [sem autenticação] GET /health → ASP.NET HealthChecks (+ DatadogAgentHealthCheck)
                    ├── [com autenticação] GET /test → AuthenticateFilter
                    │                                      Valida Bearer Token
                    │                                      Enriquece logs com UserId e UserName
                    │                                  └── TestGetUseCase
                    └── [com autenticação] GET /weather-conditions → AuthenticateFilter
                                                                          Valida Bearer Token
                                                                          Enriquece logs com UserId e UserName
                                                                      └── WeatherConditionsGetUseCase
                                                                              └── OpenMeteoApiClient (Refit + Polly)
```

---

## Responsabilidades por Camada

| Camada | O que contém | O que não contém |
|--------|-------------|-----------------|
| **Endpoint** | Orquestração HTTP, status codes, logging de request/response | Lógica de negócio |
| **UseCase** | Lógica de negócio da Slice | Acesso a dados direto, lógica HTTP |
| **Infra** | Componentes transversais (auth, logging, exceções) | Lógica de negócio específica de Feature |
| **Shared** | Abstrações reutilizáveis entre Slices | Lógica específica de uma única Slice |

---

## Infraestrutura Transversal

Os componentes de `Infra/` são transparentes para as Features:

- **[Correlation ID](Infra-Correlation-ID)** — rastreabilidade automática de requests; Features não precisam interagir com ele
- **[Autenticação JWT](Infra-Authentication)** — proteção de endpoints via atributo `[Authenticate]`; Features não implementam lógica de autenticação
- **[Tratamento de Exceções](Infra-Exception-Handling)** — captura centralizada; Features não precisam de blocos try/catch genéricos

---

## Observabilidade

### Datadog Agent (Docker)

O [Datadog Agent](https://docs.datadoghq.com/agent/) é executado como container adjacente à aplicação e coleta:
- Métricas de container e host via Docker socket
- Métricas customizadas via DogStatsD

A integração utiliza o modelo Docker sem APM. O tag `DD_ENV` varia por contexto de execução:

| Contexto | `DD_ENV` | Como filtrar no Datadog |
|----------|----------|------------------------|
| Build/compilação | `build` | `env:build` |
| CI run/healthcheck | `ci` | `env:ci` |
| Local | `local` | `env:local` |

Para execução local: copie `.env.example` para `.env`, preencha `DD_API_KEY` e execute `docker compose up`.

---

## Features Implementadas

| Feature | Tipo | Endpoint |
|---------|------|---------|
| [Health Check](Feature-Health) | — | `GET /health` |
| [Login de Usuário](Feature-UserLogin) | Command | `POST /login` |
| [Test Get](Feature-TestGet) | Query | `GET /test` |
| [Condições Climáticas](Feature-WeatherConditionsGet) | Query | `GET /weather-conditions` |
