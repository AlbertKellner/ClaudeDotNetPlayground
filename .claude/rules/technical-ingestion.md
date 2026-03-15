# Regra: Ingestão de Definições Técnicas

## Propósito

Esta rule define o workflow que o assistente deve seguir quando o usuário introduz ou altera qualquer definição técnica durável — seja um princípio, padrão, restrição, convenção, decisão tecnológica ou snippet técnico canônico.

---

## O Que É Uma Entrada Técnica

Entrada técnica é qualquer mensagem do usuário que introduza ou altere:
- Princípios de engenharia
- Padrões arquiteturais
- Restrições técnicas
- Convenções de nomenclatura técnica
- Regras de organização de pastas e módulos
- Decisões tecnológicas (linguagem, framework, banco, mensageria, runtime)
- Configurações estruturais
- Contratos de interface técnica (não de negócio)
- Snippets técnicos normativos

---

## Workflow de Classificação de Entrada Técnica

```
1. Identificar o tipo de entrada técnica:
   ├── Princípio → Instructions/architecture/engineering-principles.md
   ├── Padrão → Instructions/architecture/patterns.md
   ├── Restrição técnica → Instructions/architecture/engineering-principles.md (seção de restrições)
   ├── Decisão tecnológica → Instructions/decisions/ (novo ADR)
   ├── Convenção de nomenclatura → Instructions/architecture/naming-conventions.md
   ├── Organização de pastas → Instructions/architecture/folder-structure.md
   ├── Visão geral técnica → Instructions/architecture/technical-overview.md
   └── Snippet técnico canônico → Instructions/snippets/canonical-snippets.md

2. Verificar se a entrada conflita com definições técnicas existentes:
   ├── Sem conflito → registrar e implementar
   ├── Conflito menor (ex: variação de estilo) → registrar com nota, adotar a versão mais recente
   └── Conflito relevante → registrar em open-questions.md, reportar e aguardar confirmação

3. Verificar se a entrada de negócio pertence ao escopo técnico:
   ├── Sim, é puramente técnica → processar nesta rule
   ├── Tem relação com negócio → processar também em business-ingestion.md
   └── É motivada por negócio → registrar motivação no ADR, manter artefatos separados

4. Verificar consistência entre arquivos técnicos:
   ├── naming-conventions.md alinhado com patterns.md?
   ├── folder-structure.md alinhado com technical-overview.md?
   └── engineering-principles.md sem contradições internas?

5. Atualizar os arquivos técnicos relevantes antes de implementar
```

---

## Regras de Persistência de Conhecimento Técnico

### O que deve ser persistido na governança técnica:
- Princípios que devem guiar decisões futuras
- Padrões que devem ser seguidos sistematicamente
- Restrições que devem ser respeitadas em todas as implementações
- Decisões que afetam a estrutura do repositório
- Convenções que devem ser consistentes em todo o repositório

### O que NÃO deve ser promovido à governança técnica:
- Detalhes de implementação incidentais que não afetam decisões futuras
- Escolhas pontuais de uma única função sem impacto estrutural
- Variações de estilo que não refletem decisão consciente
- Código de exemplo que não representa padrão canônico
- Configurações temporárias ou específicas de ambiente de desenvolvimento

---

## Validação de Consistência entre Arquivos Técnicos

Após qualquer atualização em arquivo de arquitetura, verificar:

| Arquivo Atualizado | Verificar Consistência Com |
|---|---|
| `engineering-principles.md` | `patterns.md`, `technical-overview.md` |
| `patterns.md` | `naming-conventions.md`, `folder-structure.md` |
| `naming-conventions.md` | `patterns.md`, `folder-structure.md` |
| `folder-structure.md` | `patterns.md`, `technical-overview.md` |
| `technical-overview.md` | Todos os outros arquivos de arquitetura |
| `architecture-decisions.md` | Todos os outros arquivos de arquitetura |

---

## Tratamento de Entradas Técnicas Ambíguas

Quando a entrada técnica for ambígua quanto ao nível de impacto:

1. Adotar a classificação mais conservadora
2. Registrar em `assumptions-log.md` com nível de risco adequado
3. Se a ambiguidade afetar decisão estrutural, registrar em `open-questions.md` e reportar

---

## Relação com Outras Rules

- `natural-language-normalization.md` vem antes desta rule
- `architecture-governance.md` define o que pertence a cada arquivo de arquitetura
- `snippet-handling.md` é ativada quando a entrada técnica contém trechos de código ou configuração
- `naming-governance.md` é consultada quando a entrada afeta nomenclatura
- `folder-governance.md` é consultada quando a entrada afeta organização de pastas
- `change-propagation.md` é ativada após atualizar arquivos técnicos para propagar impactos
