# Test Get

## Resumo

Endpoint protegido que retorna uma string de confirmação de funcionamento da aplicação. Exige autenticação por Bearer Token JWT. Serve para verificar que o fluxo completo de autenticação e resposta está operacional.

## Autenticação

**Requer autenticação.** É obrigatório enviar um Bearer Token JWT válido no header `Authorization`. O token é obtido via [Login de Usuário](Feature-UserLogin). Consulte [Autenticação JWT](Infra-Authentication) para detalhes sobre o mecanismo de validação.

## Contrato de Entrada

| Campo | Valor |
|-------|-------|
| **Método** | `GET` |
| **Rota** | `/test` |
| **Header obrigatório** | `Authorization: Bearer {token}` |
| **Body** | Nenhum |

## Contrato de Saída

### Sucesso — `200 OK`

```
"funcionando"
```

### Token ausente — `401 Unauthorized`

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authorization header with Bearer token is required."
}
```

### Token inválido ou expirado — `401 Unauthorized`

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
  "title": "Unauthorized",
  "status": 401,
  "detail": "The provided Bearer token is invalid or expired."
}
```

## Comportamento

- Exige Bearer Token JWT válido no header `Authorization`.
- Se o token estiver **ausente**: retorna `HTTP 401` indicando que o header de autorização é obrigatório.
- Se o token estiver **inválido ou expirado**: retorna `HTTP 401` indicando que o token é inválido ou expirado.
- Se o token for **válido**: retorna `HTTP 200` com a string `"funcionando"`. Os dados do usuário autenticado (`UserId` e `UserName`) são enriquecidos automaticamente nos logs da requisição.

Regra de negócio relacionada: [RN-003](Business-Rules#rn-003---proteção-de-endpoints-por-bearer-token)

## Testes Automatizados

Nenhum teste automatizado presente no repositório.

## BDD

Nenhum cenário BDD definido para esta funcionalidade.
