# Governança da GitHub Wiki

## Propósito

Este arquivo define como o assistente deve criar, manter e evoluir a documentação da GitHub Wiki deste repositório. A Wiki é a documentação pública do projeto e deve refletir exclusivamente o que está implementado na base de código.

---

## Princípio Fundamental

> A Wiki documenta o código. O código é a fonte de verdade.
> Toda página da Wiki deve ser verificável diretamente nos arquivos do repositório.
> Documentação especulativa, aspiracional ou não materializada no código não pertence à Wiki.

---

## O Que Documentar

A Wiki deve documentar exclusivamente:
- Funcionalidades implementadas (Features — endpoints, comportamento, contratos)
- Componentes de infraestrutura implementados (Middlewares, Handlers, Services)
- Configuração necessária para executar o projeto
- Estrutura arquitetural efetivamente adotada no código
- Pipelines de CI/CD existentes nos workflows do repositório
- Regras de negócio implementadas, com link para as Features correspondentes

---

## O Que NÃO Documentar

A Wiki **nunca** deve conter:
- Descrição ou referência a arquivos de instrução do Claude (`Instructions/`, `.claude/`, `CLAUDE.md`, `open-questions.md`, `assumptions-log.md`)
- Decisões arquiteturais não materializadas no código
- Conteúdo especulativo sobre funcionalidades futuras
- Configurações, padrões ou restrições que existem apenas na governança interna
- Detalhes de processo interno do assistente

---

## Estrutura Obrigatória de Páginas

A Wiki deve conter, no mínimo, as seguintes páginas:

| Página | Propósito |
|--------|-----------|
| `Home.md` | Visão geral do projeto e sumário navegável |
| `Project-Setup.md` | Pré-requisitos, configuração, build e execução |
| `Architecture.md` | Arquitetura adotada e fluxo de request |
| Uma página por Feature | Documentação individual de cada endpoint/funcionalidade |
| Uma página por funcionalidade de Infra | Documentação de cada componente transversal |
| `Business-Rules.md` | Índice das regras de negócio com links para Features |
| `CI-CD.md` | Pipelines de CI/CD existentes |
| `_Sidebar.md` | Sidebar de navegação do GitHub Wiki |

---

## Organização: Por Funcionalidade, Não Por Arquivo

A Wiki é organizada por funcionalidade do sistema, não por arquivo de código.

- **Errado**: uma página por arquivo `.cs`
- **Correto**: uma página por Feature (endpoint ou grupo funcional); uma página por tipo de funcionalidade de Infra

---

## Template Obrigatório para Páginas de Feature

Toda página de Feature (`Feature-*.md`) deve seguir este template:

```markdown
# [Título da Funcionalidade]

## Resumo
[O que a funcionalidade faz, em 1-3 frases]

## Autenticação
[Requer autenticação: Sim / Não. Se sim, descrever como.]

## Contrato de Entrada
[Método HTTP, rota, headers obrigatórios, body schema com tipos e obrigatoriedade]

## Contrato de Saída
[Status codes e body schema por status code; formato de erro quando aplicável]

## Comportamento
[Regras de negócio: condições, ações e exceções — exclusivamente o que está implementado]

## Testes Automatizados
[Lista de testes existentes ou "Nenhum teste automatizado presente no repositório"]

## BDD
[Cenários BDD definidos ou "Nenhum cenário BDD definido para esta funcionalidade"]
```

---

## Requisito de Navegabilidade

Toda menção a conceito, componente, endpoint ou assunto que possua página própria na Wiki deve ser feita com link Markdown:

```markdown
[Texto descritivo](NomeDaPagina)
```

Isso inclui:
- Referências entre páginas de Features
- Referências de Features para páginas de Infra
- Referências da página Business-Rules para as Features correspondentes
- Referências da Home para todas as seções

---

## Nomenclatura de Páginas

| Tipo | Padrão | Exemplos |
|------|--------|---------|
| Feature | `Feature-[NomeDaFeature].md` | `Feature-UserLogin.md`, `Feature-TestGet.md` |
| Infra | `Infra-[NomeDaFuncionalidade].md` | `Infra-Authentication.md`, `Infra-Correlation-ID.md` |
| Páginas estruturais | PascalCase | `Home.md`, `Architecture.md`, `Business-Rules.md` |
| Sidebar | `_Sidebar.md` | (padrão GitHub Wiki) |

---

## Política de Atualização

### Quando uma nova Feature for adicionada ao código:
1. Criar nova página `Feature-[Nome].md` seguindo o template obrigatório
2. Adicionar link para a nova página em `Home.md` (sumário) e `_Sidebar.md`
3. Se a Feature tiver regra de negócio, adicionar entrada em `Business-Rules.md`

### Quando uma Feature existente for alterada:
1. Atualizar a página correspondente para refletir os novos contratos e comportamento
2. Verificar se links de outras páginas para esta continuam válidos

### Quando uma Feature for removida:
1. Remover a página correspondente da Wiki
2. Remover links para essa página em `Home.md`, `_Sidebar.md` e `Business-Rules.md`

### Quando um componente de Infra for criado ou alterado:
1. Criar ou atualizar a página de Infra correspondente
2. Atualizar `Home.md` e `_Sidebar.md` se nova página for criada

### Quando configuração obrigatória mudar:
1. Atualizar `Project-Setup.md`

### Quando CI/CD mudar:
1. Atualizar `CI-CD.md`

### Quando cenários BDD forem criados ou alterados:
1. Atualizar a seção "BDD" da página `Feature-[Nome].md` correspondente
2. Atualizar `Business-Rules.md` se a regra de negócio agora possui BDD referenciado

### Quando regras de negócio forem criadas ou alteradas:
1. Atualizar `Business-Rules.md`
2. Atualizar a seção "Comportamento" da página `Feature-[Nome].md` correspondente

### Quando contratos forem criados ou alterados:
1. Atualizar as seções "Contrato de Entrada" e "Contrato de Saída" da página `Feature-[Nome].md` correspondente

### Quando testes forem criados ou alterados:
1. Atualizar a seção "Testes Automatizados" da página `Feature-[Nome].md` correspondente

---

## Fonte Canônica e Publicação

- A pasta `wiki/` no repositório principal é a **fonte canônica** das páginas da Wiki
- Todas as alterações de conteúdo são feitas na pasta `wiki/` como parte do fluxo normal de desenvolvimento
- A publicação na GitHub Wiki é **automática** via o workflow `.github/workflows/wiki-publish.yml`:
  - Disparado automaticamente a cada push para `main`/`master` que altere arquivos em `wiki/`
  - Clona o repositório wiki com o `GITHUB_TOKEN`, copia os arquivos e faz push
  - Só cria commit se houver mudanças efetivas
- Para forçar uma publicação sem alterar arquivos de wiki, usar o gatilho manual (`workflow_dispatch`) do workflow `Publicar Wiki` na aba Actions do GitHub

---

## Idioma

Toda a documentação da Wiki deve ser escrita em **português brasileiro**.

---

## Referências Cruzadas

- `Instructions/architecture/technical-overview.md` — fonte de verdade arquitetural que alimenta `Architecture.md`
- `Instructions/business/business-rules.md` — fonte de verdade das regras de negócio que alimenta `Business-Rules.md` e as seções "Comportamento" das Features
- `.github/workflows/` — fonte de verdade que alimenta `CI-CD.md`
- `src/` — fonte de verdade dos contratos de entrada/saída de cada Feature

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|------|---------|-----------|
| 2026-03-15 | Criado: governança inicial da GitHub Wiki | Instrução do usuário |
| 2026-03-15 | Publicação alterada para automática via wiki-publish.yml | Instrução do usuário |
| 2026-03-19 | Adicionado: gatilhos de atualização da wiki para BDD, regras de negócio, contratos e testes | Instrução do usuário |
