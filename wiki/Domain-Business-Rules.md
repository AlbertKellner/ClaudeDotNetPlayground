# Regras de Negócio

## Descrição

Índice de todas as regras de negócio ativas implementadas na aplicação. Cada regra possui link para a página da feature com documentação completa. Deve ser consultada para compreender os comportamentos esperados ou ao adicionar novas funcionalidades.

## Contexto

As regras de negócio definem o que o sistema deve ou não deve fazer. São a fonte de verdade para o comportamento esperado da aplicação. Regras de negócio prevalecem sobre preferências arquiteturais quando houver conflito.

---

## RN-002 — Autenticação de usuário via login com credenciais

| Campo | Valor |
|---|---|
| **Endpoint** | `POST /login` |
| **Autenticação** | Não exigida |
| **Comportamento** | Credenciais válidas retornam HTTP 200 com JWT contendo `id` e `userName`. Credenciais inválidas retornam HTTP 401 com Problem Details |
| **Feature** | [Login de Usuário](Feature-UserLogin) |

Detalhes de segurança: [Autenticação](Governance-Security)

---

## RN-003 — Proteção de endpoints por Bearer Token

| Campo | Valor |
|---|---|
| **Escopo** | Todos os endpoints, exceto `POST /login` e `GET /health` |
| **Autenticação** | Obrigatória — Bearer Token JWT no header `Authorization` |
| **Comportamento** | Token válido: requisição processada normalmente, `UserId` e `UserName` enriquecidos nos logs. Token inválido ou ausente: HTTP 401 com Problem Details |
| **Feature** | Todos os endpoints protegidos |

Detalhes de segurança: [Autenticação](Governance-Security)

---

## RN-004 — Consulta de condições climáticas por coordenadas geográficas

| Campo | Valor |
|---|---|
| **Endpoint** | `GET /weather-conditions?latitude={lat}&longitude={lng}` |
| **Autenticação** | Exigida |
| **Comportamento** | Recebe latitude e longitude via query parameters. Consulta a API Open-Meteo com as coordenadas recebidas. Retorna o payload completo da Open-Meteo sem filtragem. Resposta cacheada por usuário autenticado e por coordenadas com duração configurável |
| **Feature** | [Condições Climáticas](Feature-WeatherConditionsGet) |

---

## RN-005 — Health Check com verificação do Datadog Agent

| Campo | Valor |
|---|---|
| **Endpoint** | `GET /health` |
| **Autenticação** | Não exigida |
| **Comportamento** | Verifica disponibilidade do Datadog Agent via HTTP. App + Agent disponíveis: HTTP 200 `Healthy`. Agent com status inesperado: HTTP 200 `Degraded`. Agent inacessível: HTTP 503 `Unhealthy` |
| **Feature** | [Health Check](Feature-Health) |

---

## RN-008 — Pesquisa de repositórios GitHub por conta

| Campo | Valor |
|---|---|
| **Endpoint** | `GET /github-repo-search` |
| **Autenticação** | Exigida |
| **Comportamento** | Consulta a API do GitHub (`GET /users/AlbertKellner/repos`) com paginação automática. Retorna lista de repositórios contendo nome e endereço Git. Resposta cacheada por usuário autenticado com duração configurável |
| **Feature** | [Pesquisa GitHub](Feature-GitHubRepoSearch) |

---

## RN-009 — Consulta de Pokémon por ID via PokéAPI

| Campo | Valor |
|---|---|
| **Endpoint** | `GET /pokemon/{id}` |
| **Autenticação** | Exigida |
| **Comportamento** | Recebe o ID do Pokémon como parâmetro de rota. Consulta a PokéAPI (`GET /api/v2/pokemon/{id}`). Retorna informações básicas (id, nome, altura, peso, experiência base, tipos, habilidades, stats, sprites). Resposta cacheada por usuário autenticado e ID do Pokémon com duração configurável |
| **Feature** | [Consulta Pokémon](Feature-PokemonGet) |

---

## Regras Removidas

| ID | Título | Status |
|---|---|---|
| RN-006 | Busca de repositórios do team IntegrationRepos no GitHub | Removida (2026-03-20) |
| RN-007 | Sincronização local de repositórios | Removida (2026-03-20) |

---

## Referências

- [Visão Geral do Domínio](Domain-Overview)
- [Feature: Health Check](Feature-Health)
- [Feature: Login de Usuário](Feature-UserLogin)
- [Feature: Condições Climáticas](Feature-WeatherConditionsGet)
- [Feature: Pesquisa GitHub](Feature-GitHubRepoSearch)
- [Feature: Consulta Pokémon](Feature-PokemonGet)
