# Busca de Repositórios

## Resumo

Consulta a API do GitHub para listar todos os repositórios acessíveis ao team **IntegrationRepos** da organização **WebMotors**. Salva o resultado em um arquivo JSON local com informações de cada repositório e um campo de última sincronização iniciado em branco.

## Autenticação

Requer autenticação: **Sim**. Bearer Token JWT válido no header `Authorization`. Ver [Autenticação JWT](Infra-Authentication).

## Contrato de Entrada

| Campo | Valor |
|-------|-------|
| **Método** | `GET` |
| **Rota** | `/repositories` |
| **Headers obrigatórios** | `Authorization: Bearer <token>` |
| **Body** | Nenhum |

## Contrato de Saída

### HTTP 200 — Sucesso

```json
{
  "repositories": [
    {
      "name": "nome-do-repositorio",
      "description": "Descrição do repositório",
      "gitUrl": "https://github.com/WebMotors/nome-do-repositorio.git",
      "lastModifiedDate": "2026-03-19T10:00:00Z",
      "lastSyncDate": ""
    }
  ]
}
```

### HTTP 401 — Token ausente ou inválido

Retorna Problem Details (RFC 7807).

## Comportamento

1. Consulta a API GitHub via `GET /orgs/WebMotors/teams/IntegrationRepos/repos` usando o PAT configurado em `ExternalApi:GitHub:HttpRequest:Token`
2. Mapeia cada repositório para o modelo `RepositoryFileEntry` com: nome, descrição, URL Git (`.git`), data da última modificação e campo de última sincronização em branco
3. Registra cada repositório encontrado no log (nível information)
4. Salva a lista no arquivo JSON configurado em `Repositories:JsonFilePath` (padrão: `data/repositories.json`)
5. Retorna a lista completa ao chamador

### Configuração

| Chave | Descrição | Valor padrão |
|-------|-----------|--------------|
| `ExternalApi:GitHub:HttpRequest:BaseUrl` | URL base da API GitHub | `https://api.github.com` |
| `ExternalApi:GitHub:HttpRequest:Token` | Personal Access Token do GitHub | (vazio — deve ser configurado) |
| `Repositories:JsonFilePath` | Caminho do arquivo JSON de saída | `data/repositories.json` |

### Resiliência

Retry exponencial com Polly v8, configurável via `ExternalApi:GitHub:CircuitBreaker`.

## Testes Automatizados

- `RepositoriesGetAllEndpointTests.cs` — 4 testes: logs de entrada/saída, prefixo, retorno OkObjectResult
- `RepositoriesGetAllUseCaseTests.cs` — 8 testes: logs, mapeamento, escrita JSON, iteração
- `GitHubApiClientTests.cs` — 4 testes: logs, prefixo, delegação ao Refit fake

## BDD

Nenhum cenário BDD definido para esta funcionalidade.
