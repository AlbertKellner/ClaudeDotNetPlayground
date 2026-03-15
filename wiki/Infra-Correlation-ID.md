# Correlation ID

## Resumo

Mecanismo de rastreabilidade que garante um identificador único por requisição HTTP. Permite correlacionar todos os logs gerados durante o processamento de uma mesma requisição.

## Como Funciona

### Identificador utilizado: GUID v7

O Correlation ID é sempre um **GUID versão 7** (UUID v7), que incorpora um timestamp monotônico, garantindo que identificadores gerados sequencialmente sejam ordenáveis cronologicamente.

### Resolução do ID por requisição

A cada requisição recebida, o middleware resolve o Correlation ID da seguinte forma:

1. Verifica se o header `X-Correlation-Id` foi enviado na requisição
2. Se o valor presente for um GUID v7 válido: **reutiliza** o ID recebido
3. Caso contrário (header ausente, formato inválido ou versão diferente de v7): **gera um novo** GUID v7

### Propagação

| Destino | Valor |
|---------|-------|
| Header de resposta `X-Correlation-Id` | O Correlation ID resolvido (recebido ou gerado) |
| Todos os logs da requisição | Enriquecidos automaticamente com a propriedade `CorrelationId` via Serilog LogContext |

### Transparência para Features

O Correlation ID é completamente opaco para as Features. Endpoints, UseCases e Repositories não precisam — e não devem — interagir com o Correlation ID diretamente. O enriquecimento dos logs é automático.

## Uso

### Enviar Correlation ID na requisição

Para rastrear um request com um ID específico (ex: ID gerado pelo cliente ou por outro serviço):

```
X-Correlation-Id: 01960000-0000-7000-8000-000000000000
```

O valor deve ser um GUID v7 válido. Caso contrário, a aplicação ignora o valor e gera um novo.

### Identificar o Correlation ID de uma resposta

O Correlation ID utilizado está sempre presente no header de resposta:

```
X-Correlation-Id: 01960000-0000-7000-8000-000000000000
```

### Consultar logs por Correlation ID

Todos os logs de uma requisição contêm a propriedade `CorrelationId`. Para correlacionar logs de uma requisição específica, filtre pelo valor retornado no header `X-Correlation-Id`.

## Componentes Envolvidos

| Componente | Localização | Responsabilidade |
|------------|-------------|-----------------|
| `CorrelationIdMiddleware` | `Infra/Middlewares/` | Resolve o ID, define o header de resposta, enriquece o LogContext |
| `GuidV7` | `Infra/Correlation/` | Utilitário interno de geração e validação de GUID v7 |

## Relação com Arquitetura

Consulte [Arquitetura](Architecture) para entender como o `CorrelationIdMiddleware` se encaixa no fluxo de request.
