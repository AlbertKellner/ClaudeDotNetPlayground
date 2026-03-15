# BDD — Especificação por Comportamento

## Propósito

Esta pasta contém os cenários comportamentais do repositório escritos em Gherkin (Given/When/Then).
BDD (Behavior-Driven Development) permite especificar comportamentos esperados do sistema em linguagem próxima ao negócio.

---

## Quando BDD Deve Ser Usado

BDD é adequado quando:
- O comportamento envolve fluxo com múltiplas condições e resultados distintos
- O comportamento tem variações significativas dependendo do estado inicial
- A regra de negócio é complexa e se beneficia de exemplos concretos
- O comportamento é testável em termos de entrada e saída observável
- Há necessidade de comunicação clara entre negócio e implementação sobre o comportamento esperado

---

## Quando BDD NÃO É Necessário

BDD não deve ser forçado quando:
- A mudança é puramente técnica sem impacto em comportamento de negócio observável
- O comportamento é trivial e autoexplicativo
- A regra é tão simples que o cenário não acrescenta clareza
- A mudança é infraestrutural (ex: alterar ferramenta de logging, mudar configuração de pool de conexão)
- A mudança é de estilo ou refatoração sem alteração de comportamento

---

## Como Organizar Cenários

- Um arquivo `.feature` por funcionalidade ou contexto de negócio
- Nomes de arquivo descritivos: `[contexto]-[funcionalidade].feature`
- Cenários agrupados por funcionalidade relacionada, não por componente técnico
- Usar tags para categorizar: `@smoke`, `@regression`, `@integration`, `@contrato`

---

## Como Cenários se Relacionam com Regras de Negócio

Cada cenário deve poder ser rastreado até uma regra de negócio em `Instructions/business/business-rules.md`.
Use comentários no arquivo `.feature` para referenciar a regra:

```gherkin
# Regra relacionada: RN-001
Scenario: [título do cenário]
```

---

## Como Mudanças em BDD se Propagam

Quando um cenário BDD é criado ou alterado:
1. Verificar se a regra de negócio correspondente está atualizada em `business-rules.md`
2. Verificar se o contrato correspondente está atualizado em `Instructions/contracts/`
3. Verificar se a implementação está alinhada com o novo comportamento especificado
4. Verificar se o glossário tem os termos usados no cenário

---

## Como Cenários Devem Ser Nomeados

- Usar linguagem de negócio, não técnica
- Descrever o comportamento esperado, não o mecanismo de implementação
- Usar formato: "deve [resultado] quando [condição]" ou "permite [ação] dado [estado]"
- Evitar nomes genéricos como "cenário de teste" ou "caso 1"

**Exemplos de bons nomes**:
- `deve rejeitar criação quando nome estiver vazio`
- `permite aprovação quando todos os critérios forem atendidos`
- `retorna erro quando recurso não existir`

**Exemplos de nomes ruins**:
- `teste de criação`
- `caso de erro`
- `fluxo feliz`

---

## Estrutura dos Arquivos

- `README.md` — este arquivo: política de uso e convenções
- `conventions.md` — convenções detalhadas de escrita de cenários
- `*.feature` — arquivos de cenários

---

## Referências Cruzadas

- `Instructions/business/business-rules.md` — regras que os cenários especificam
- `Instructions/contracts/` — contratos que os cenários validam
- `.claude/rules/business-ingestion.md` — política de quando criar BDD
