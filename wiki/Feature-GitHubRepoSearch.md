# Pesquisa de Repositórios GitHub

## Resumo
Endpoint autenticado que pesquisa e exibe todos os repositórios da conta GitHub AlbertKellner, retornando o nome de cada repositório e o respectivo endereço Git. A resposta é cacheada por usuário autenticado com duração configurável.

## Autenticação
Requer autenticação: Sim. Bearer Token JWT deve ser enviado no header `Authorization`.

## Contrato de Entrada
| Item | Valor |
|---|---|
| Método | `GET` |
| Rota | `/github-repo-search` |
| Headers obrigatórios | `Authorization: Bearer <token>` |
| Body | Nenhum |
| Query parameters | Nenhum |

## Contrato de Saída

### HTTP 200 — Sucesso
```json
{
  "repositories": [
    {
      "name": "nome-do-repositorio",
      "gitUrl": "git://github.com/AlbertKellner/nome-do-repositorio.git"
    }
  ]
}
```

### HTTP 401 — Não autorizado
Retornado quando o Bearer Token está ausente, expirado ou inválido. Corpo em formato [Problem Details](Infra-Authentication).

### HTTP 500 — Erro interno
Retornado quando a API do GitHub está indisponível ou retorna erro. Corpo em formato [Problem Details](Infra-Exception-Handling).

## Comportamento
- O sistema verifica o Memory Cache usando o ID do usuário autenticado como chave ([RN-008](Business-Rules))
- Se houver cache válido, retorna a resposta cacheada sem consultar a API externa
- Se não houver cache, consulta a API do GitHub (`GET /users/AlbertKellner/repos`) com paginação automática (100 repositórios por página)
- O resultado é armazenado no cache com duração configurável via `appsettings.json` (seção `ExternalApi:GitHub:EndpointCache:GitHubRepoSearch:DurationSeconds`, padrão: 60 segundos)
- A autenticação na API do GitHub utiliza Personal Access Token (PAT) configurável via variável de ambiente `GITHUB_PAT` ou `appsettings.json`
- Resiliência HTTP via Polly v8: retry exponencial + timeout por tentativa

## Testes Automatizados
- `GitHubRepoSearchEndpointTests` — testes de logging e retorno do endpoint
- `GitHubRepoSearchUseCaseTests` — testes de mapeamento, logging e passagem do username correto
- `GitHubApiClientTests` — testes de logging, retorno e paginação do cliente HTTP
- `CachedGitHubApiClientTests` — testes de cache hit/miss, isolamento por usuário e logging

## BDD
Nenhum cenário BDD definido para esta funcionalidade.
