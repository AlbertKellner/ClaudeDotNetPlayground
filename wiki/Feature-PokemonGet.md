# Consulta de Pokémon por ID

## Resumo
Endpoint autenticado que consulta dados de um Pokémon por ID via PokéAPI. Neste primeiro momento, o ID permanece hardcoded como 25 (Pikachu). A resposta é cacheada por usuário autenticado.

## Autenticação
Requer autenticação: Sim. Bearer Token JWT obtido via [POST /login](Feature-UserLogin).

## Contrato de Entrada
- **Método**: `GET`
- **Rota**: `/pokemon`
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
- A consulta é feita à PokéAPI (`GET /api/v2/pokemon/25`) com ID hardcoded (RN-009)
- O resultado é cacheado por usuário autenticado via Memory Cache (DA-018)
- Duração do cache configurável via `appsettings.json` (`ExternalApi:Pokemon:EndpointCache:PokemonGet:DurationSeconds`)
- Campos retornados: id, nome, altura, peso, experiência base, sprites (front default e shiny), tipos, habilidades e stats
- A Feature possui seu próprio model de Output (DA-020) — não expõe o model da PokéAPI diretamente

## Testes Automatizados
- `PokemonGetUseCaseTests` — 8 testes: log de início, ID do Pikachu passado à API, dados básicos mapeados, tipos mapeados, habilidades mapeadas, stats mapeados, sprites mapeados, log de retorno
- `PokemonGetEndpointTests` — 3 testes: retorna OK com output, log de início, log de retorno

## BDD
Nenhum cenário BDD definido para esta funcionalidade.
