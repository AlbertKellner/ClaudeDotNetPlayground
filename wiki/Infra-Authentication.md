# Autenticação JWT

## Resumo

Mecanismo de autenticação baseado em **JWT Bearer Token** (JSON Web Token). Protege os endpoints da aplicação, exigindo um token válido em toda requisição a endpoints não públicos. O token é obtido via [Login de Usuário](Feature-UserLogin).

## Como Funciona

### Geração do token

O token JWT é gerado pelo endpoint de [Login de Usuário](Feature-UserLogin) quando as credenciais fornecidas são válidas.

**Características do token gerado:**

| Propriedade | Valor |
|-------------|-------|
| Algoritmo | HS256 (HMAC SHA-256) |
| Validade | 1 hora a partir da geração |
| Claim `id` | ID numérico do usuário autenticado |
| Claim `userName` | Nome de usuário do usuário autenticado |
| Chave de assinatura | Configurada em `Jwt:Secret` no `appsettings.json` |

### Validação do token

Em endpoints protegidos, o `AuthenticateFilter` valida o token recebido:

1. Extrai o Bearer Token do header `Authorization`
2. Valida a assinatura, expiração e integridade do token
3. Se válido: extrai `id` e `userName` do token e enriquece os logs da requisição com `UserId` e `UserName`
4. Se inválido ou ausente: retorna `HTTP 401` com Problem Details e interrompe o processamento

### Proteção de endpoints

Para proteger um endpoint, o Controller deve ser decorado com o atributo `[Authenticate]`. A lógica de validação é executada automaticamente pelo `AuthenticateFilter`.

### Transparência para Features

Features apenas declaram `[Authenticate]` no Controller. Nenhuma lógica de autenticação ou extração de dados do token é implementada dentro das Features.

## Endpoints públicos (sem autenticação)

| Endpoint | Motivo |
|----------|--------|
| `POST /login` | Ponto de entrada para obtenção do token |
| `GET /health` | Verificação de disponibilidade |

Todos os demais endpoints exigem Bearer Token válido.

## Como usar o token

### 1. Obter o token

```bash
curl -X POST http://localhost:5000/login \
  -H "Content-Type: application/json" \
  -d '{"userName": "Albert", "password": "albert123"}'
```

Resposta:
```json
{
  "token": "eyJ..."
}
```

### 2. Usar o token em requisições protegidas

```bash
curl http://localhost:5000/test \
  -H "Authorization: Bearer eyJ..."
```

## Respostas de erro

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

## Componentes Envolvidos

| Componente | Localização | Responsabilidade |
|------------|-------------|-----------------|
| `ITokenService` | `Infra/Security/` | Interface de contrato para geração e validação de tokens |
| `TokenService` | `Infra/Security/` | Implementação JWT HS256: gera e valida tokens |
| `AuthenticatedUser` | `Infra/Security/` | Modelo com `Id` e `UserName` do usuário autenticado |
| `AuthenticateFilter` | `Infra/Security/` | Valida o token, enriquece logs, retorna 401 se inválido |
| `AuthenticateAttribute` | `Infra/Security/` | Decorador aplicado nos Controllers para ativar o filtro |

## Relação com Features

- [Login de Usuário](Feature-UserLogin) — endpoint que gera o token
- [Test Get](Feature-TestGet) — exemplo de endpoint protegido
