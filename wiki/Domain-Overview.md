# Visão Geral do Domínio

## Descrição

Apresenta o propósito da aplicação, o domínio que ela atende e os conceitos implementados. Esta página é o ponto de entrada para entender o que a aplicação faz do ponto de vista de negócio. Deve ser consultada durante onboarding ou para compreender o escopo da aplicação.

## Contexto

Este projeto é um playground de exploração de padrões e integrações, construído com ASP.NET Core (.NET 10) e compilado com Native AOT. A aplicação expõe uma API web autenticada que consome APIs externas e implementa padrões arquiteturais modernos.

---

## O Que É Esta Aplicação

- API web construída com **ASP.NET Core (.NET 10)**, compilada com **Native AOT** para reduzir tempo de startup e consumo de memória
- Implementa autenticação via **JWT Bearer Token**, logging estruturado com **Serilog** (padrão storytelling) e **Vertical Slice Architecture** com segregação Command/Query
- Projeto playground para exploração de padrões de engenharia, integrações HTTP externas, observabilidade com Datadog e containerização com Docker

---

## Funcionalidades Implementadas

| Funcionalidade | Endpoint | Descrição | Regra de Negócio | Página |
|---|---|---|---|---|
| Health Check | `GET /health` | Verificação de disponibilidade da aplicação e do Datadog Agent | RN-005 | [Feature-Health](Feature-Health) |
| Login de Usuário | `POST /login` | Autenticação com credenciais; retorna JWT Bearer Token | RN-002 | [Feature-UserLogin](Feature-UserLogin) |
| Condições Climáticas | `GET /weather-conditions?latitude={lat}&longitude={lng}` | Consulta de condições climáticas por coordenadas via Open-Meteo | RN-004 | [Feature-WeatherConditionsGet](Feature-WeatherConditionsGet) |
| Pesquisa GitHub | `GET /github-repo-search` | Pesquisa de repositórios da conta AlbertKellner no GitHub | RN-008 | [Feature-GitHubRepoSearch](Feature-GitHubRepoSearch) |
| Consulta Pokémon | `GET /pokemon/{id}` | Consulta de dados de Pokémon por ID via PokéAPI | RN-009 | [Feature-PokemonGet](Feature-PokemonGet) |

---

## APIs Externas Consumidas

| API | Propósito | Autenticação | Endpoint Base |
|---|---|---|---|
| **Open-Meteo** | Condições climáticas atuais de São Paulo | Nenhuma (API pública) | `api.open-meteo.com` |
| **GitHub API** | Listagem de repositórios da conta AlbertKellner | PAT (opcional — sem PAT, rate limiting mais restritivo) | `api.github.com` |
| **PokéAPI** | Dados de Pokémon por ID (nome, tipos, stats, sprites) | Nenhuma (API pública) | `pokeapi.co` |

Todas as integrações seguem o padrão `Shared/ExternalApi/<Servico>/` com Refit + Polly v8 para resiliência (retry exponencial + timeout por tentativa). Os resultados são cacheados por usuário autenticado via Memory Cache com duração configurável.

---

## Conceitos e Entidades

Este projeto é um **playground** — nenhuma entidade formal de domínio foi definida até o momento. O modelo de domínio será definido quando o projeto evoluir para um domínio real.

Os principais conceitos em uso são:

| Conceito | Descrição | Localização |
|---|---|---|
| **AuthenticatedUser** | Modelo de usuário autenticado com `Id` (int) e `UserName` (string), extraído do JWT Bearer Token | `Infra/Security/AuthenticatedUser.cs` |
| **Bearer Token** | Credencial de acesso JWT HS256 com claims `id` e `userName`; expira em 1 hora | Gerado por `TokenService`, validado por `AuthenticateFilter` |
| **CorrelationId** | GUID v7 único por requisição HTTP, enriquecido automaticamente nos logs via Serilog LogContext; opaco para Features | `Infra/Middlewares/CorrelationIdMiddleware.cs` |

---

## Referências

- [Regras de Negócio](Domain-Business-Rules)
- [Arquitetura](Governance-Architecture)
- [Integrações](Governance-Integrations)
- [Feature: Health Check](Feature-Health)
- [Feature: Login de Usuário](Feature-UserLogin)
- [Feature: Condições Climáticas](Feature-WeatherConditionsGet)
- [Feature: Pesquisa GitHub](Feature-GitHubRepoSearch)
- [Feature: Consulta Pokémon](Feature-PokemonGet)
