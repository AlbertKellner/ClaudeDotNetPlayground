# Busca de Pokémon via PokeAPI

## Resumo

Endpoint autenticado que consulta a API pública PokeAPI (pokeapi.co) e retorna a ficha completa de um Pokémon. Na implementação atual, o nome do Pokémon buscado é hardcoded como "pikachu".

## Autenticação

Requer autenticação: **Sim**. Bearer Token JWT válido no header `Authorization`. Ver [Autenticação JWT](Infra-Authentication).

## Contrato de Entrada

| Item | Valor |
|------|-------|
| **Método** | `GET` |
| **Rota** | `/pokemon-search` |
| **Headers obrigatórios** | `Authorization: Bearer <token>` |
| **Body** | Nenhum |
| **Parâmetros de query** | Nenhum |

## Contrato de Saída

### HTTP 200 — Sucesso

Body JSON com a ficha completa do Pokémon, incluindo:

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `id` | `int` | Número na Pokédex Nacional |
| `name` | `string` | Nome do Pokémon (minúsculo) |
| `base_experience` | `int` | Experiência base obtida ao derrotar |
| `height` | `int` | Altura em decímetros |
| `weight` | `int` | Peso em hectogramas |
| `is_default` | `bool` | Se é a forma padrão da espécie |
| `order` | `int` | Índice de ordenação entre todos os Pokémon |
| `location_area_encounters` | `string` | URL para endpoint de encontros |
| `abilities` | `array` | Habilidades do Pokémon (nome, se é oculta, slot) |
| `forms` | `array` | Formas do Pokémon |
| `game_indices` | `array` | Índice por versão do jogo |
| `held_items` | `array` | Itens que o Pokémon pode segurar |
| `moves` | `array` | Movimentos que o Pokémon pode aprender |
| `species` | `object` | Referência à espécie |
| `sprites` | `object` | URLs de imagens (sprites padrão, shiny, artwork oficial) |
| `cries` | `object` | URLs de áudio (sons do Pokémon) |
| `stats` | `array` | Estatísticas base (HP, Ataque, Defesa, Ataque Especial, Defesa Especial, Velocidade) |
| `types` | `array` | Tipos do Pokémon (ex: electric) |
| `past_types` | `array` | Tipos em gerações anteriores (se alterados) |
| `past_abilities` | `array` | Habilidades em gerações anteriores (se alteradas) |

### HTTP 401 — Não autenticado

Retornado quando o Bearer Token é ausente ou inválido. Corpo em formato Problem Details (RFC 7807).

### HTTP 500 — Erro interno

Retornado quando a consulta à PokeAPI falha. Corpo em formato Problem Details (RFC 7807).

## Comportamento

- O endpoint consulta a PokeAPI (`GET https://pokeapi.co/api/v2/pokemon/pikachu`) e retorna o payload completo da resposta, preservando a estrutura original (RN-008)
- O nome do Pokémon buscado é hardcoded como "pikachu" para testes iniciais
- A PokeAPI é uma API pública, sem necessidade de autenticação ou token
- A resiliência da chamada HTTP usa Polly v8 com retry exponencial e timeout por tentativa
- Os sprites incluem imagens padrão, shiny e artwork oficial; sprites por geração/jogo não são incluídos
- A integração HTTP é implementada via Refit em `Shared/ExternalApi/PokeApi/`

## Testes Automatizados

- `PokemonSearchGetUseCaseTests` — 11 testes: logs de storytelling, mapeamento completo, busca hardcoded de "pikachu", mapeamento de tipos/habilidades/stats/cries
- `PokemonSearchGetEndpointTests` — 4 testes: logs de storytelling, retorno OK com output completo

## BDD

Nenhum cenário BDD definido para esta funcionalidade.
