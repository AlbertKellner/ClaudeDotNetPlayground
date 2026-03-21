# Consulta de Pokémon

## Resumo

Endpoint autenticado que consulta dados de um Pokémon na PokéAPI. Neste primeiro momento, o ID do Pokémon está hardcoded com o valor do Pikachu (25). A resposta é cacheada por usuário autenticado com duração configurável.

## Autenticação

Requer autenticação: **Sim**. Bearer Token JWT válido no header `Authorization`. Obter token via [Login de Usuário](Feature-UserLogin).

## Contrato de Entrada

| Campo | Valor |
|-------|-------|
| **Método** | `GET` |
| **Rota** | `/pokemon` |
| **Headers obrigatórios** | `Authorization: Bearer <token>` |
| **Body** | Nenhum |
| **Query parameters** | Nenhum |

## Contrato de Saída

### Sucesso — `HTTP 200`

```json
{
  "id": 25,
  "name": "pikachu",
  "height": 4,
  "weight": 60,
  "types": [
    {
      "slot": 1,
      "name": "electric"
    }
  ],
  "spriteUrl": "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/25.png"
}
```

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `id` | `int` | Identificador do Pokémon na PokéAPI |
| `name` | `string` | Nome do Pokémon |
| `height` | `int` | Altura do Pokémon (em decímetros) |
| `weight` | `int` | Peso do Pokémon (em hectogramas) |
| `types` | `array` | Lista de tipos do Pokémon |
| `types[].slot` | `int` | Posição do tipo (1 = primário, 2 = secundário) |
| `types[].name` | `string` | Nome do tipo |
| `spriteUrl` | `string?` | URL da imagem frontal do Pokémon |

### Erro de autenticação — `HTTP 401`

Retornado quando o header `Authorization` está ausente ou contém token inválido. Formato [Problem Details](Infra-Exception-Handling).

## Comportamento

- O ID do Pokémon está hardcoded com o valor **25** (Pikachu) — regra de negócio [RN-009](Business-Rules)
- A resposta é cacheada por **usuário autenticado** usando Memory Cache com duração configurável via `appsettings.json` (`ExternalApi:Pokemon:EndpointCache:PokemonGet:DurationSeconds`)
- A API externa consumida é a PokéAPI (`https://pokeapi.co/api/v2/pokemon/{id}`), pública e sem autenticação
- Resiliência HTTP via Polly v8 (retry exponencial + timeout)
- Os dados retornados são mapeados para um model próprio da Feature, isolado do model da API externa

## Testes Automatizados

- `PokemonGetEndpointTests` — 4 testes: logs de entrada, saída, retorno OK e prefixo correto
- `PokemonGetUseCaseTests` — 8 testes: logs, mapeamento, ID hardcoded e iteração de tipos
- `PokeApiClientTests` — 5 testes: logs, retorno da API e chamada única
- `CachedPokeApiClientTests` — 9 testes: cache miss, cache hit, isolamento por usuário e logs de cache

## BDD

Nenhum cenário BDD definido para esta funcionalidade.
