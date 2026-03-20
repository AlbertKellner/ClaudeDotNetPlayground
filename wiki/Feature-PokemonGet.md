# Consulta de Pokemon via PokeAPI

## Resumo

Consulta o perfil essencial do Pokemon Pikachu a partir da PokeAPI e retorna os campos id, name, height, weight, base_experience, types, abilities e stats.

## Autenticação

Sim. Requer Bearer Token JWT válido no header `Authorization`. Ver [Autenticação JWT](Infra-Authentication).

## Contrato de Entrada

| Campo | Valor |
|-------|-------|
| **Método** | `GET` |
| **Rota** | `/pokemon` |
| **Headers** | `Authorization: Bearer <token>` (obrigatório) |
| **Body** | Não aplicável |
| **Query params** | Nenhum (Pokemon consultado é fixo na implementação: pikachu) |

## Contrato de Saída

### HTTP 200 — Perfil essencial do Pokemon retornado com sucesso

```json
{
  "id": 25,
  "name": "pikachu",
  "height": 4,
  "weight": 60,
  "base_experience": 112,
  "types": [...],
  "abilities": [...],
  "stats": [...]
}
```

### HTTP 401 — Token ausente ou inválido

```json
{
  "type": "...",
  "title": "Unauthorized",
  "status": 401,
  "detail": "..."
}
```

## Comportamento

Conforme [RN-008](Business-Rules):

- O Pokemon consultado é fixo na implementação: `pikachu`
- A PokeAPI é consumida via `GET /api/v2/pokemon/pikachu`
- O perfil essencial retornado contém: id, name, height, weight, base_experience, types, abilities e stats

### Resiliência

A integração com a PokeAPI usa Polly v8 com retry exponencial e timeout por tentativa, conforme padrão definido na stack (Refit + `Microsoft.Extensions.Http.Resilience`).

## Testes Automatizados

Nenhum teste automatizado presente no repositório no momento.

## BDD

3 cenários definidos em `Instructions/bdd/pokemon-get.feature`:
- Autenticação obrigatória (401 sem token)
- Retorno do perfil essencial do Pikachu (200)
- Resposta contém todos os campos essenciais
