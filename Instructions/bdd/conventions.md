# Convenções de BDD

## Propósito

Este arquivo define as convenções de escrita de cenários BDD neste repositório. Todos os cenários devem seguir estas convenções para garantir clareza, consistência e rastreabilidade.

---

## Estrutura Obrigatória de um Cenário

```gherkin
# Regra relacionada: RN-[número]
# Contexto: [descrição breve do contexto de negócio]

Feature: [Nome da funcionalidade em linguagem de negócio]
  Como [ator]
  Quero [objetivo]
  Para que [benefício]

  Background: (opcional — estado comum a todos os cenários)
    Given [estado inicial compartilhado]

  Scenario: [deve/pode/não deve] [resultado] [quando/dado/se] [condição]
    Given [estado inicial específico]
    When [ação executada]
    Then [resultado observável esperado]
    And [resultado adicional, se necessário]
```

---

## Regras de Escrita de Passos

### Given (Dado)
- Descreve o estado inicial do sistema ou contexto antes da ação
- Deve ser verificável e determinístico
- Usar substantivos e estados: "existe um pedido com status Pendente"
- **Não** descrever como o estado foi criado — apenas que ele existe

### When (Quando)
- Descreve a ação que o ator executa
- Deve ser uma única ação (evitar múltiplas ações em um único When)
- Usar verbos de ação: "o cliente envia", "o sistema processa", "o usuário confirma"

### Then (Então)
- Descreve o resultado observável esperado após a ação
- Deve ser verificável objetivamente
- Usar afirmações: "o status é atualizado para", "o sistema retorna", "o evento é emitido"
- Múltiplos Then são permitidos com `And` para resultados diferentes do mesmo cenário

---

## Regras de Nomenclatura

- **Feature**: substantivo plural ou gerúndio que descreve a funcionalidade
  - ✅ "Aprovação de Pedidos", "Gestão de Usuários"
  - ❌ "Teste de pedido", "Módulo X"

- **Scenario**: frase descritiva que inclui condição e resultado esperado
  - ✅ "deve rejeitar pedido quando estoque for insuficiente"
  - ❌ "teste de pedido com estoque", "cenário 3"

- **Passos**: linguagem de negócio, não técnica
  - ✅ "dado que o cliente está autenticado"
  - ❌ "dado que o JWT token é válido no header Authorization"

---

## Uso de Tags

| Tag | Quando Usar |
|---|---|
| `@smoke` | Cenários críticos de verificação rápida |
| `@regression` | Cenários de regressão para mudanças frequentes |
| `@integration` | Cenários que envolvem integração com sistemas externos |
| `@contrato` | Cenários que verificam conformidade com contratos |
| `@pendente` | Cenários escritos mas ainda não implementados |
| `@wip` | Cenários em desenvolvimento |

---

## Uso de Scenario Outline

Usar `Scenario Outline` com `Examples` quando:
- O mesmo comportamento deve ser verificado com múltiplos conjuntos de dados
- Os dados variam mas a estrutura do comportamento é a mesma

```gherkin
Scenario Outline: deve calcular desconto corretamente para <categoria>
  Given um produto da categoria <categoria>
  When o desconto é calculado
  Then o percentual de desconto é <percentual>

  Examples:
    | categoria  | percentual |
    | Premium    | 15%        |
    | Standard   | 5%         |
    | Básico     | 0%         |
```

---

## Rastreabilidade Obrigatória

Todo arquivo `.feature` deve ter no cabeçalho:
```gherkin
# Regras relacionadas: RN-[número], RN-[número]
# Contratos relacionados: [nome do contrato]
# Última atualização: [data]
```

---

## Anti-Padrões a Evitar

| Anti-Padrão | Por Que Evitar | Como Corrigir |
|---|---|---|
| Passos muito técnicos | Perde legibilidade para o negócio | Abstrair detalhes técnicos em step definitions |
| Cenário único com muitos Then | Testa múltiplos comportamentos | Separar em cenários distintos |
| Given muito longo | Estado inicial complexo demais | Usar Background ou simplificar com abstração |
| Termos fora do glossário | Inconsistência terminológica | Usar sempre termos do glossário |
| Cenários sem rastreabilidade para regras | Difícil manter alinhamento | Sempre referenciar RN-NNN |

---

## Referências Cruzadas

- `Instructions/bdd/README.md` — política geral de uso do BDD
- `Instructions/business/business-rules.md` — regras que os cenários especificam
- `Instructions/glossary/ubiquitous-language.md` — terminologia usada nos cenários
