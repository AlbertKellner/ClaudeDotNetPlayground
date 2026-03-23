# Test Get

## Descrição

Documenta o endpoint protegido de teste (`GET /test`), que valida o fluxo completo de autenticação e resposta. Esta página cobre o contrato de entrada e saída, os cenários de autenticação válida e inválida, e a regra de negócio RN-003. Consulte quando precisar validar se a autenticação está funcionando corretamente. Relaciona-se com [Segurança](Governance-Security) (proteção por Bearer Token) e [Login de Usuário](Feature-UserLogin) (obtenção do token).

## Resumo

Endpoint protegido que retorna uma string de confirmação de funcionamento da aplicação. Exige autenticação por Bearer Token JWT. Serve para verificar que o fluxo completo de autenticação e resposta está operacional.

## Autenticação

**Requer autenticação.** É obrigatório enviar um Bearer Token JWT válido no header `Authorization`. O token é obtido via [Login de Usuário](Feature-UserLogin). Consulte [Segurança](Governance-Security) para detalhes sobre o mecanismo de validação.

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

Regra de negócio relacionada: [RN-003](Domain-Business-Rules#rn-003--proteção-de-endpoints-por-bearer-token)

## Endpoints Consumidos

Esta feature não consome endpoints de APIs externas. Retorna diretamente uma string estática de confirmação de funcionamento.

---

## Testes Automatizados

| Arquivo | Cobertura |
|---------|-----------|
| `TestGetEndpointTests.cs` | Logs do endpoint, retorno HTTP 200 com body "funcionando" e HTTP 401 sem token |
| `TestGetUseCaseTests.cs` | Logs do use case, retorno da string de funcionamento |

## BDD

Nenhum cenário BDD definido para esta funcionalidade.

## Referências

- [Regras de Negócio](Domain-Business-Rules) — RN-003
- [Segurança](Governance-Security) — mecanismo de autenticação
- [Login de Usuário](Feature-UserLogin) — obtenção do token
