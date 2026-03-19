# Sincronização de Repositórios

## Resumo

Lê o arquivo JSON gerado pela [Busca de Repositórios](Feature-RepositoriesGetAll) e sincroniza localmente cada repositório, clonando novos e atualizando existentes via `git pull`. Após cada sincronização, atualiza o campo de última sincronização no arquivo JSON.

## Autenticação

Requer autenticação: **Sim**. Bearer Token JWT válido no header `Authorization`. Ver [Autenticação JWT](Infra-Authentication).

## Contrato de Entrada

| Campo | Valor |
|-------|-------|
| **Método** | `POST` |
| **Rota** | `/repositories/sync` |
| **Headers obrigatórios** | `Authorization: Bearer <token>` |
| **Body** | Nenhum |

## Contrato de Saída

### HTTP 200 — Sucesso

```json
{
  "totalRepositories": 5,
  "successCount": 4,
  "errorCount": 1,
  "results": [
    {
      "name": "repo-one",
      "status": "cloned",
      "errorMessage": null
    },
    {
      "name": "repo-two",
      "status": "pulled",
      "errorMessage": null
    },
    {
      "name": "repo-three",
      "status": "error",
      "errorMessage": "fatal: repository not found"
    }
  ]
}
```

### HTTP 401 — Token ausente ou inválido

Retorna Problem Details (RFC 7807).

## Comportamento

1. Lê o arquivo JSON configurado em `Repositories:JsonFilePath`
2. Se o arquivo não existir, retorna resultado vazio (sem erro HTTP)
3. Para cada repositório no JSON:
   - Se a pasta local já existe com `.git`: executa `git pull`
   - Se a pasta não existe: executa `git clone {gitUrl} {nome}`
4. Status possíveis por repositório: `cloned`, `pulled`, `error`
5. Para cada sincronização com sucesso: registra log nível **information** e atualiza `lastSyncDate` com data/hora no formato `dd/MM/yyyy HH:mm:ss` (24 horas)
6. Para cada sincronização com falha: registra log nível **error** com detalhes do erro
7. Salva o arquivo JSON atualizado com as datas de sincronização

### Configuração

| Chave | Descrição | Valor padrão |
|-------|-----------|--------------|
| `Repositories:JsonFilePath` | Caminho do arquivo JSON de repositórios | `data/repositories.json` |
| `Repositories:SyncRootPath` | Pasta raiz onde os repositórios serão clonados | `c:/usuarios/albert/git` |

### Pré-requisitos

- O binário `git` deve estar disponível no `PATH`
- O arquivo JSON deve ter sido gerado previamente pela feature [Busca de Repositórios](Feature-RepositoriesGetAll)

## Testes Automatizados

- `RepositoriesSyncAllEndpointTests.cs` — 4 testes: logs de entrada/saída, prefixo, retorno OkObjectResult
- `RepositoriesSyncAllUseCaseTests.cs` — 6 testes: logs, erro quando arquivo não existe, iteração

## BDD

Cenários definidos em `Instructions/bdd/repositories-sync-all.feature` (RN-007, RN-006, RN-003):

- **Autenticação**: não deve permitir sincronização sem autenticação; não deve permitir sincronização com token inválido
- **Clone**: deve clonar repositório quando a pasta local não existe
- **Pull**: deve atualizar repositório quando a pasta já existe com Git
- **Registro de data**: deve gravar data/hora da sincronização no JSON após sucesso
- **Contadores**: deve retornar contadores de sucesso/erro com detalhes por repositório
- **Logging**: deve registrar log information para sucesso; deve registrar log error para falha
- **Borda**: deve retornar resultado vazio quando JSON não existe; deve retornar resultado vazio quando JSON existe mas está vazio
