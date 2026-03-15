# Contratos de Interface

## Propósito

Esta pasta contém os contratos formais de interface deste repositório — especificações de APIs, mensagens assíncronas, schemas e exemplos de payloads.

**Contratos são fontes de alta prioridade.** Conflitos entre contratos e texto narrativo são resolvidos em favor do contrato.

---

## O Que É Um Contrato

Contrato é qualquer artefato formal que especifica uma interface de comunicação:
- **API REST/HTTP**: especificação OpenAPI (YAML/JSON)
- **Eventos e mensagens**: especificação AsyncAPI
- **Schemas de dados**: JSON Schema, Avro Schema, Protobuf
- **Integrações**: qualquer interface com sistema externo que deva ser formalizada

**Contratos não se restringem a APIs HTTP.**
Payloads de mensagens, schemas de eventos, formatos de filas e tópicos também são contratos.

---

## Quando Contratos Devem Ser Criados ou Atualizados

Criar ou atualizar contrato quando:
- Uma nova interface for exposta para consumidores externos ou outros sistemas
- Um payload de mensagem for publicado em tópico ou fila
- Um schema de dados tiver que ser compartilhado entre produtores e consumidores
- Uma integração com sistema externo for formalizada

**Contratos não são obrigatórios para toda mudança.** Não criar contratos para:
- Lógica interna sem interface exposta
- Interfaces usadas apenas dentro de um único módulo sem consumidores externos
- Mudanças de implementação sem alteração de interface

---

## Como Contratos se Relacionam com Regras de Negócio

Contratos formalizam como as regras de negócio são expostas para o mundo externo.
- O comportamento esperado em um contrato deve ser rastreável a uma regra de negócio
- Mudanças em regras de negócio que afetam interfaces públicas devem se refletir nos contratos
- Contratos não devem conter lógica de negócio — apenas a especificação da interface

---

## Como Contratos se Relacionam com BDD

Cenários BDD podem especificar o comportamento de endpoints ou consumo de eventos.
- Cenários com tag `@contrato` verificam conformidade com contratos formais
- Quando BDD e contrato divergirem sobre comportamento → registrar em `open-questions.md`

---

## Como Mudanças em Contratos se Propagam

Quando um contrato for alterado:
1. Verificar se as regras de negócio relacionadas estão atualizadas
2. Verificar se os cenários BDD correspondentes estão atualizados
3. Verificar se o glossário tem os termos dos campos novos ou alterados
4. **Avaliar o impacto em consumidores** — contratos têm dependentes externos
5. Decidir entre versionamento (nova versão do contrato) ou mudança não-breaking

---

## Como Contratos Guiam Implementação

- A implementação deve ser gerada a partir do contrato, não o contrário
- O contrato define a interface; o código implementa a interface
- Mudanças de implementação não podem alterar contratos sem instrução explícita

---

## Estrutura da Pasta

```
contracts/
├── README.md                   # (este arquivo) — política e convenções
├── openapi.yaml                # Especificação OpenAPI placeholder
├── asyncapi.yaml               # Especificação AsyncAPI placeholder
├── schemas/
│   └── example.schema.json     # Exemplo de JSON Schema
└── examples/
    ├── valid-example.json       # Exemplo de payload válido
    └── invalid-example.json     # Exemplo de payload inválido
```

---

## Versionamento de Contratos

> **Pendente de definição.** A estratégia de versionamento de contratos será definida quando os primeiros contratos reais forem criados.

Exemplos de estratégias a considerar:
- Versionamento via URL (`/v1/`, `/v2/`)
- Versionamento via header (`Accept: application/vnd.api+json;version=2`)
- Versionamento via evolução sem breaking changes (abordagem aditiva)

---

## Referências Cruzadas

- `Instructions/business/business-rules.md` — regras formalizadas pelos contratos
- `Instructions/bdd/` — cenários que verificam os contratos
- `Instructions/glossary/ubiquitous-language.md` — terminologia usada nos contratos
