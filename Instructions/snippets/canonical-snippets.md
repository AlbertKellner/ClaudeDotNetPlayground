# Registro de Snippets Normativos Canônicos

## Propósito

Este arquivo armazena snippets normativos — trechos técnicos fornecidos explicitamente pelo usuário para serem preservados na íntegra e respeitados em futuras implementações.

**Snippets registrados aqui são obrigatórios** — não devem ser reescritos livremente.
Qualquer alteração exige nova instrução explícita do usuário ou conflito técnico devidamente reportado.

---

## Como Usar Este Arquivo

Para adicionar um snippet canônico, usar a estrutura abaixo.
Para consultar um snippet existente, localizar pelo id ou escopo.

---

## Estrutura de Cada Snippet

```markdown
## Snippet [ID]

| Campo | Valor |
|---|---|
| **Id** | SNP-[número] |
| **Data** | [YYYY-MM-DD] |
| **Título** | [Título descritivo] |
| **Intenção** | [O que este snippet deve fazer] |
| **Instrução original** | [Resumo do que o usuário pediu] |
| **Classificação** | Normativo |
| **Escopo** | [Onde se aplica: arquivo, módulo, contexto] |
| **Regra de preservação** | [Como deve ser aplicado] |
| **Adaptações mínimas permitidas** | [O que pode ser ajustado sem violar a intenção] |
| **Artefatos relacionados** | [Arquivos ou componentes que usam este snippet] |

### Conteúdo do Snippet

[Conteúdo exato do snippet aqui]

### Histórico de Alterações

| Data | Alteração | Motivo |
|---|---|---|
| [data] | Registrado inicialmente | Instrução explícita do usuário |
```

---

## Snippets Registrados

> **Estado atual**: nenhum snippet canônico foi registrado ainda.
> Snippets serão adicionados aqui quando o usuário fornecer trechos normativos explícitos.

---

## Instruções para o Assistente

Quando um snippet for registrado aqui:

1. **Antes de implementar** artefatos no escopo do snippet, verificar se este arquivo contém um snippet aplicável
2. **Aplicar o snippet na íntegra** nos locais de destino indicados
3. **Não reescrever** o conteúdo do snippet — mesmo que pareça que há uma "versão melhor"
4. **Reportar** adaptações mínimas inevitáveis com justificativa explícita
5. **Registrar conflito** se o snippet conflitar com estrutura existente, segurança ou contratos — não substituir silenciosamente

Quando o usuário fornecer novo trecho para substituir um snippet existente:
1. Atualizar o conteúdo do snippet neste arquivo
2. Registrar a alteração na tabela de histórico
3. Propagar a mudança para os artefatos de destino
4. Reportar os locais que foram atualizados
