# Albert.Playground.ECS.AOT

API web construída com **ASP.NET Core** em **.NET 10**, compilada com **Native AOT**. Implementa autenticação por **JWT Bearer Token**, logging estruturado com **Serilog** e arquitetura **Vertical Slice**.

## O que está implementado

- Endpoint de disponibilidade (`GET /health`)
- Autenticação de usuário com geração de JWT (`POST /login`)
- Endpoint protegido por Bearer Token (`GET /test`)
- Consulta de condições climáticas de São Paulo via Open-Meteo (`GET /weather-conditions`)
- Pesquisa de repositórios GitHub da conta AlbertKellner (`GET /github-repo-search`)
- Consulta de Pokémon via PokéAPI (`GET /pokemon`)
- Infraestrutura transversal: rastreabilidade por Correlation ID, tratamento centralizado de exceções, autenticação JWT

---

## Sumário

### Configuração e Execução

| Página | Descrição |
|--------|-----------|
| [Configuração do Projeto](Project-Setup) | Pré-requisitos, configuração, build e como executar a aplicação |

### Arquitetura

| Página | Descrição |
|--------|-----------|
| [Arquitetura](Architecture) | Estilo arquitetural Vertical Slice, estrutura de pastas e fluxo de request |

### Funcionalidades (Features)

| Página | Descrição |
|--------|-----------|
| [Health Check](Feature-Health) | Endpoint de verificação de disponibilidade da aplicação (`GET /health`) |
| [Login de Usuário](Feature-UserLogin) | Autenticação com credenciais e geração de JWT Bearer Token (`POST /login`) |
| [Test Get](Feature-TestGet) | Endpoint protegido que retorna confirmação de funcionamento (`GET /test`) |
| [Condições Climáticas de São Paulo](Feature-WeatherConditionsGet) | Consulta das condições climáticas atuais de São Paulo via Open-Meteo API (`GET /weather-conditions`) |
| [Pesquisa de Repositórios GitHub](Feature-GitHubRepoSearch) | Pesquisa e exibição dos repositórios da conta AlbertKellner (`GET /github-repo-search`) |
| [Consulta de Pokémon](Feature-PokemonGet) | Consulta de dados do Pokémon Pikachu via PokéAPI (`GET /pokemon`) |

### Infraestrutura

| Página | Descrição |
|--------|-----------|
| [Correlation ID](Infra-Correlation-ID) | Rastreabilidade de requests via GUID v7 e header `X-Correlation-Id` |
| [Autenticação JWT](Infra-Authentication) | Geração e validação de Bearer Token JWT, proteção de endpoints |
| [Tratamento de Exceções](Infra-Exception-Handling) | Captura centralizada de exceções e resposta padronizada em Problem Details |

### Regras de Negócio e CI/CD

| Página | Descrição |
|--------|-----------|
| [Regras de Negócio](Business-Rules) | Índice das regras de negócio implementadas com links para as features correspondentes |
| [CI/CD](CI-CD) | Pipelines de build, execução e validação de pull requests |
