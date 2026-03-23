# Consulta de Pokémon por ID

## Descrição

Documenta o endpoint de consulta de Pokémon (`GET /pokemon/{id}`), que retorna dados de um Pokémon por ID via PokéAPI. Esta página cobre o contrato HTTP com parâmetro de rota, o mapeamento para model próprio da Feature (DA-020), o cache por usuário, e a regra de negócio RN-009. Consulte quando precisar entender a integração com a PokéAPI ou o padrão de isolamento de models. Relaciona-se com [Integrações](Governance-Integrations) (Refit + Polly) e [Padrões de Desenvolvimento](Governance-Development-Patterns) (isolamento de models).

## Resumo

Endpoint autenticado que consulta dados de um Pokémon por ID via PokéAPI. O ID do Pokémon é recebido como parâmetro de rota da requisição HTTP. A resposta é cacheada por usuário autenticado.

## Autenticação
Requer autenticação: Sim. Bearer Token JWT obtido via [POST /login](Feature-UserLogin).

## Contrato de Entrada
- **Método**: `GET`
- **Rota**: `/pokemon/{id}`
- **Parâmetros de rota**:
  - `id` (inteiro, obrigatório) — ID do Pokémon na PokéAPI
- **Headers obrigatórios**: `Authorization: Bearer <token>`
- **Body**: Nenhum

## Contrato de Saída

### HTTP 200 — Sucesso
```json
{
  "id": 25,
  "name": "pikachu",
  "height": 4,
  "weight": 60,
  "base_experience": 112,
  "front_default_sprite": "https://...",
  "front_shiny_sprite": "https://...",
  "types": [{ "slot": 1, "name": "electric" }],
  "abilities": [{ "name": "static", "is_hidden": false }],
  "stats": [{ "name": "hp", "base_stat": 35, "effort": 0 }]
}
```

### HTTP 401 — Não autenticado
Retorna Problem Details (RFC 7807) quando o token é ausente, inválido ou expirado.

## Comportamento
- O ID do Pokémon é recebido como parâmetro de rota (`GET /pokemon/{id}`) (RN-009)
- A consulta é feita à PokéAPI (`GET /api/v2/pokemon/{id}`) com o ID recebido
- O resultado é cacheado por usuário autenticado via Memory Cache (DA-018)
- Duração do cache configurável via `appsettings.json` (`ExternalApi:Pokemon:EndpointCache:PokemonGet:DurationSeconds`)
- Campos retornados: id, nome, altura, peso, experiência base, sprites (front default e shiny), tipos, habilidades e stats
- A Feature possui seu próprio model de Output (DA-020) — não expõe o model da PokéAPI diretamente

## Testes Automatizados
- `PokemonGetUseCaseTests` — 8 testes: log de início, ID recebido passado à API, dados básicos mapeados, tipos mapeados, habilidades mapeadas, stats mapeados, sprites mapeados, log de retorno
- `PokemonGetEndpointTests` — 3 testes: retorna OK com output, log de início, log de retorno
- `PokemonApiClientTests` — testes de logging e delegação HTTP do cliente PokéAPI
- `CachedPokemonApiClientTests` — testes de cache hit/miss, isolamento por usuário e logging

## BDD

Nenhum cenário BDD definido para esta funcionalidade.

## Referências

- [Regras de Negócio](Domain-Business-Rules) — RN-009
- [Integrações](Governance-Integrations) — padrão Refit + Polly e Memory Cache
- [Padrões de Desenvolvimento](Governance-Development-Patterns) — isolamento de models (DA-020)
- [Segurança](Governance-Security) — autenticação obrigatória
