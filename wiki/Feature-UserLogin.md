# Login de Usuário

## Resumo

Endpoint de autenticação. Recebe credenciais de usuário, valida-as e retorna um Bearer Token JWT quando as credenciais são válidas. O token obtido deve ser utilizado nas requisições aos endpoints protegidos da aplicação.

## Autenticação

**Não requer autenticação.** Este endpoint é público e é o ponto de entrada para obtenção do token de acesso. Consulte [Autenticação JWT](Infra-Authentication) para entender como o token gerado é utilizado.

## Contrato de Entrada

| Campo | Valor |
|-------|-------|
| **Método** | `POST` |
| **Rota** | `/login` |
| **Content-Type** | `application/json` |

**Body:**

```json
{
  "userName": "string",
  "password": "string"
}
```

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|-------------|-----------|
| `userName` | `string` | Sim | Nome de usuário |
| `password` | `string` | Sim | Senha do usuário |

## Contrato de Saída

### Sucesso — `200 OK`

```json
{
  "token": "string"
}
```

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `token` | `string` | Bearer Token JWT com validade de 1 hora |

### Credenciais inválidas — `401 Unauthorized`

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Invalid username or password."
}
```

### Body inválido — `400 Bad Request`

Retornado quando campos obrigatórios estão ausentes no body da requisição.

## Comportamento

- Recebe `userName` e `password` no body da requisição.
- Valida as credenciais contra a lista de usuários registrados na aplicação.
- Se as credenciais forem **válidas**: gera um JWT Bearer Token contendo as informações do usuário (`id` e `userName`) e retorna `HTTP 200` com o token.
- Se as credenciais forem **inválidas**: retorna `HTTP 401` com corpo em formato Problem Details.
- O token gerado tem validade de **1 hora** a partir da geração.

Regra de negócio relacionada: [RN-002](Business-Rules#rn-002---autenticação-de-usuário-via-login)

## Testes Automatizados

Nenhum teste automatizado presente no repositório.

## BDD

Nenhum cenário BDD definido para esta funcionalidade.
