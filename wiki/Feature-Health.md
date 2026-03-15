# Health Check

## Resumo

Endpoint de verificação de disponibilidade da aplicação. Retorna o status de saúde da aplicação, indicando que ela está em execução e respondendo a requisições. É implementado pelo mecanismo nativo de HealthChecks do ASP.NET Core.

## Autenticação

**Não requer autenticação.** Este endpoint é acessível publicamente e é explicitamente excluído da proteção por Bearer Token. Consulte [Autenticação JWT](Infra-Authentication) para entender como a proteção funciona nos demais endpoints.

## Contrato de Entrada

| Campo | Valor |
|-------|-------|
| **Método** | `GET` |
| **Rota** | `/health` |
| **Headers** | Nenhum obrigatório |
| **Body** | Nenhum |

## Contrato de Saída

| Status | Corpo | Descrição |
|--------|-------|-----------|
| `200 OK` | `Healthy` | Aplicação em funcionamento |

## Comportamento

- Quando a aplicação está em execução e responde a requisições, retorna `HTTP 200` com corpo `Healthy`.
- Não realiza verificações adicionais de dependências externas (banco de dados, serviços externos) além da disponibilidade da própria aplicação.
- Não exige autenticação — é exceção explícita à regra de proteção por Bearer Token.

Regra de negócio relacionada: [RN-001](Business-Rules#rn-001---verificação-de-disponibilidade-da-aplicação)

## Testes Automatizados

Nenhum teste automatizado presente no repositório.

## BDD

Nenhum cenário BDD definido para esta funcionalidade.
