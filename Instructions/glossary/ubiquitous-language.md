# Linguagem Ubíqua

## Propósito

Este arquivo é a fonte de verdade terminológica deste repositório. Todo termo de domínio — seja em código, contratos, BDD, documentação ou conversas — deve estar alinhado com as definições aqui registradas.

**Consistência terminológica é uma regra, não uma preferência.**

---

## Regras de Uso

1. **Use sempre o termo canônico** — não invente sinônimos não registrados
2. **Sinônimos permitidos** são os únicos alternativos aceitáveis quando indicados explicitamente
3. **Termos proibidos** são banidos porque causam ambiguidade ou inconsistência — nunca usar
4. **Novos termos** só entram neste glossário via instrução explícita do usuário ou quando o assistente identifica a necessidade e registra como pendência
5. **Este arquivo prevalece** sobre qualquer nomenclatura técnica na camada de domínio

---

## Como Ler Este Glossário

Cada entrada segue a estrutura:

```
### [Termo Canônico]
**Definição**: o que este termo significa neste domínio
**Sinônimos permitidos**: termos equivalentes aceitáveis (se houver)
**Termos proibidos**: termos que não devem ser usados para este conceito
**Contexto**: onde e quando este termo se aplica
**Exemplos**: exemplos de uso correto
**Notas**: observações importantes sobre o uso
**Relacionado a**: outros termos relacionados
```

---

## Termos do Domínio

> **Estado atual**: nenhum termo específico do domínio foi definido ainda.
> Termos serão adicionados à medida que o domínio for definido.

### Template de Entrada

```markdown
### [Termo]
**Definição**:
**Sinônimos permitidos**:
**Termos proibidos**:
**Contexto**:
**Exemplos**:
**Notas**:
**Relacionado a**:
```

---

## Termos do Sistema de Governança

Os seguintes termos são usados internamente no sistema de governança e têm definições precisas:

### Snippet Normativo
**Definição**: Trecho técnico fornecido pelo usuário com instrução explícita de cópia literal, preservação íntegra ou inclusão sem reescrita.
**Sinônimos permitidos**: trecho canônico, trecho literal
**Termos proibidos**: "apenas um exemplo" (quando o usuário disse que é normativo)
**Contexto**: Classificação de trechos técnicos enviados pelo usuário
**Notas**: Deve ser copiado na íntegra para o destino. Não pode ser reescrito livremente.

### Exemplo Ilustrativo
**Definição**: Trecho técnico fornecido pelo usuário como referência, inspiração ou ponto de partida, sem exigência de cópia literal.
**Sinônimos permitidos**: trecho de referência, exemplo de base
**Contexto**: Classificação de trechos técnicos enviados pelo usuário
**Notas**: Pode ser adaptado ao contexto do projeto. A intenção deve ser preservada.

### Definição Durável
**Definição**: Conhecimento que deve persistir e guiar trabalho futuro — regras, decisões, restrições, terminologia, padrões.
**Termos proibidos**: "detalhe de implementação" (quando o conteúdo é durável)
**Contexto**: Decisão sobre o que persiste na governança
**Notas**: Contraste com detalhe incidental, que não deve ser promovido à governança.

### Invariante
**Definição**: Condição que nunca pode ser violada, independentemente da operação. Mais restritivo que uma regra de negócio.
**Termos proibidos**: "regra que nunca quebra" (informal)
**Contexto**: Modelagem de domínio

### Premissa Conservadora
**Definição**: Suposição mínima adotada na ausência de informação completa, que prefere o comportamento menos destrutivo e mais reversível.
**Contexto**: Tratamento de ambiguidades

### Ferramenta Operacional
**Definição**: Recurso externo disponível para o assistente durante o desenvolvimento que amplia suas capacidades operacionais. Inclui MCP servers, CLIs autenticados, APIs com tokens, integrações configuradas via variáveis de ambiente ou `.mcp.json`.
**Sinônimos permitidos**: recurso operacional, integração externa
**Termos proibidos**: "plugin" (genérico demais), "serviço" (ambíguo com serviços de domínio)
**Contexto**: Registro e propagação de dependências operacionais em `governance-policies.md` §3, `technical-overview.md` (seção Recursos Operacionais) e `environment-readiness.md`
**Exemplos**: Datadog MCP, GitHub CLI (`gh`), tokens PAT, chaves de API
**Notas**: Quando uma nova ferramenta operacional é disponibilizada, deve ser registrada em `technical-overview.md` e propagada para `environment-readiness.md` e `required-vars.md` conforme o protocolo em `technical-overview.md` seção "Como Novos Recursos São Registrados".

---

## Termos Pendentes de Definição

> Quando o assistente identificar um termo sendo usado sem definição clara, registrar aqui:

| Termo | Contexto de Uso | Status |
|---|---|---|
| — | — | — |

---

## Referências Cruzadas

- `Instructions/business/domain-model.md` — entidades que usam estes termos
- `Instructions/bdd/` — cenários que usam estes termos
- `Instructions/contracts/` — contratos que usam estes termos
- `Instructions/architecture/naming-conventions.md` — como os termos são expressos em código

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| Bootstrap | Termos do sistema de governança criados | — |
| 2026-03-21 | Adicionado: "Ferramenta Operacional" — conceito usado em governance-policies.md §3 sem definição formal | Análise de capacidade de auto-diagnóstico |
