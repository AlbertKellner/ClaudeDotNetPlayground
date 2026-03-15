# Convenções de Nomenclatura

## Propósito

Este arquivo registra todas as convenções de nomenclatura deste repositório — técnicas e de domínio. Convenções registradas aqui são **obrigatórias** e devem ser aplicadas consistentemente em código, contratos, BDD, documentação e qualquer outro artefato.

---

## Princípio Fundamental

A **terminologia de domínio** (ubiquitous language) prevalece sobre convenções técnicas na camada de domínio.
Convenções técnicas de casing (PascalCase, snake_case, etc.) são aplicadas respeitando a terminologia de domínio.

*Ver conflitos em: `.claude/rules/naming-governance.md` e `.claude/rules/source-of-truth-priority.md`*

---

## Convenções por Tipo de Artefato

> **Estado atual**: convenções específicas serão registradas quando a stack for definida.

| Tipo de Artefato | Convenção | Exemplo | Decisão |
|---|---|---|---|
| Classes/tipos | A definir | — | — |
| Funções/métodos | A definir | — | — |
| Variáveis | A definir | — | — |
| Constantes | A definir | — | — |
| Arquivos de código | A definir | — | — |
| Pastas/módulos | A definir | — | — |
| Tabelas de banco | A definir | — | — |
| Colunas de banco | A definir | — | — |
| Campos de contrato (JSON) | A definir | — | — |
| Campos de contrato (YAML) | A definir | — | — |
| Tópicos de mensageria | A definir | — | — |
| Filas | A definir | — | — |
| Variáveis de ambiente | A definir | — | — |
| Rotas de API | A definir | — | — |

---

## Terminologia de Domínio

A terminologia de domínio é definida no glossário:
`Instructions/glossary/ubiquitous-language.md`

**Regra**: nenhum nome de conceito de domínio pode divergir da definição do glossário sem alteração explícita do glossário primeiro.

---

## Prefixos e Sufixos Padronizados

> **Pendente de definição.** Quando convenções de prefixo/sufixo forem estabelecidas, registrar aqui.

Exemplos a definir:
- Sufixos para tipos específicos (ex: `Service`, `Repository`, `Handler`, `Event`, `Command`)
- Prefixos para tópicos de eventos (ex: `domain.`, `integration.`)
- Convenções para nomes de migrations (ex: timestamp + descrição)

---

## Abreviações Permitidas

> **Pendente de definição.** Abreviações aceitas no repositório serão listadas aqui.

Regra: só usar abreviações registradas aqui. Sem abreviações não documentadas.

---

## Termos Proibidos

> **Pendente de definição.** Termos que causam ambiguidade e foram banidos serão listados aqui.

Exemplos genéricos de padrões a evitar:
- Nomes genéricos sem contexto (`data`, `info`, `manager`, `helper`, `util`)
- Termos de implementação onde deveria haver terminologia de domínio
- Sinônimos não documentados de termos canônicos

---

## Referências Cruzadas

- `Instructions/glossary/ubiquitous-language.md` — fonte de terminologia de domínio
- `Instructions/architecture/patterns.md` — padrões que usam estas convenções
- `Instructions/architecture/folder-structure.md` — nomes de pastas e módulos

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| Bootstrap | Estrutura criada sem convenções específicas | — |
