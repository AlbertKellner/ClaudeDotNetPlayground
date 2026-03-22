# Governança da GitHub Wiki

## Propósito

Este arquivo define como o assistente deve criar, manter e evoluir a documentação da GitHub Wiki deste repositório. A Wiki é a documentação pública do projeto.

---

## Princípio Fundamental

> A Wiki prioriza clareza, previsibilidade e facilidade de navegação.
> Toda página deve ser verificável diretamente nos arquivos do repositório ou na governança.
> Documentação especulativa ou aspiracional não pertence à Wiki.

---

## Organização por Agrupamentos

A Wiki é organizada em quatro agrupamentos temáticos:

| Agrupamento | Propósito | Prefixo de página |
|-------------|-----------|-------------------|
| **Governança** | Diretrizes, padrões, restrições, decisões técnicas e operacionais — genéricas e específicas deste repositório | `Governance-` |
| **Domínio e Negócio** | Regras de negócio, features implementadas, conceitos de domínio, fluxos funcionais | `Domain-` ou `Feature-` |
| **Claude** | Skills, hooks, convenções e comportamentos específicos do Claude neste repositório | `Claude-` |
| **Não categorizado** | Conteúdos em consolidação, workflows de conhecimento que ainda não se encaixam nos agrupamentos anteriores | Sem prefixo específico |

---

## O Que Documentar

### Governança
- Estrutura arquitetural adotada no código
- Padrões de desenvolvimento e convenções de código
- Estratégia de testes
- Mecanismos de segurança implementados
- Observabilidade (logging, tracing, métricas)
- Pipelines de CI/CD
- Padrões de integração com APIs externas
- Configuração e operação do projeto
- Tratamento de erros e qualidade
- Decisões arquiteturais, restrições e caminhos de evolução

### Domínio e Negócio
- Visão geral do domínio da aplicação
- Regras de negócio implementadas
- Funcionalidades implementadas (Features — endpoints, comportamento, contratos)
- Fluxos funcionais e conceitos de domínio

### Claude
- Sistema de governança operacional (skills, hooks, rules)
- Comportamentos esperados e convenções de uso
- Pipeline de validação pré-commit

### Não categorizado
- Workflows de conhecimento em consolidação
- Registros que ainda não se encaixam nos agrupamentos anteriores

---

## O Que NÃO Documentar

A Wiki **nunca** deve conter:
- Conteúdo especulativo sobre funcionalidades futuras
- Detalhes internos de implementação que não tenham relevância para consulta

---

## Estrutura Obrigatória de Páginas

### Páginas estruturais

| Página | Propósito |
|--------|-----------|
| `Home.md` | Visão geral do projeto e sumário navegável por agrupamento |
| `_Sidebar.md` | Sidebar de navegação organizada por agrupamento |

### Páginas de Governança

| Página | Propósito |
|--------|-----------|
| `Governance-Architecture.md` | Estilo arquitetural, estrutura de pastas, componentes, fluxo de request |
| `Governance-Development-Patterns.md` | Padrões de desenvolvimento: Vertical Slice, CQRS, UseCase, Decorator |
| `Governance-Code-Conventions.md` | Convenções de código: nomenclatura, namespaces, variáveis, logging |
| `Governance-Testing.md` | Estratégia de testes, padrões de teste, cobertura |
| `Governance-Security.md` | Mecanismo de autenticação JWT, proteção de endpoints |
| `Governance-Observability.md` | Correlation ID, Serilog, Datadog Agent |
| `Governance-CI-CD.md` | Pipelines de CI/CD e workflows do GitHub Actions |
| `Governance-Integrations.md` | Padrão de integração HTTP externa (Refit + Polly), Memory Cache |
| `Governance-Operation.md` | Pré-requisitos, configuração, build, execução, Docker |
| `Governance-Quality.md` | Tratamento de exceções, Problem Details, políticas de erro |
| `Governance-Decisions.md` | Decisões arquiteturais, restrições AOT, decisões pendentes, evolução |

### Páginas de Domínio e Negócio

| Página | Propósito |
|--------|-----------|
| `Domain-Overview.md` | Visão geral do domínio e propósito da aplicação |
| `Domain-Business-Rules.md` | Índice das regras de negócio com links para Features |
| Uma página por Feature | Documentação individual de cada endpoint/funcionalidade |

### Páginas do Claude

| Página | Propósito |
|--------|-----------|
| `Claude-Overview.md` | Como o Claude opera neste repositório |
| `Claude-Skills.md` | Catálogo de skills disponíveis |
| `Claude-Hooks.md` | Hooks configurados e seus comportamentos |
| `Claude-Conventions.md` | Convenções, pipeline pré-commit, restrições |

---

## Padrão Mínimo para Cada Página

Toda página da Wiki deve seguir esta estrutura mínima:

```markdown
# [Título da Página]

## Descrição
[Explica rapidamente o propósito da página, qual problema resolve, qual é seu escopo e por que existe.]

## Contexto
[Apresenta o cenário em que o assunto se aplica e sua relação com outras partes da documentação.]

## [Conteúdo Principal]
[Desenvolve o tema da página com o nível de detalhe necessário.]

## Referências
[Aponta páginas relacionadas, dependências e complementos.]
```

---

## Template Obrigatório para Páginas de Feature

Toda página de Feature (`Feature-*.md`) deve seguir este template, que incorpora o padrão mínimo:

```markdown
# [Título da Funcionalidade]

## Descrição
[Propósito da funcionalidade, escopo, quando consultar esta página e temas relacionados.]

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

## Referências
[Páginas relacionadas: regras de negócio, governança, outras features]
```

---

## Requisito de Navegabilidade

Toda menção a conceito, componente, endpoint ou assunto que possua página própria na Wiki deve ser feita com link Markdown:

```markdown
[Texto descritivo](NomeDaPagina)
```

---

## Nomenclatura de Páginas

| Tipo | Padrão | Exemplos |
|------|--------|---------|
| Governança | `Governance-[Tema].md` | `Governance-Architecture.md`, `Governance-Security.md` |
| Domínio | `Domain-[Tema].md` | `Domain-Overview.md`, `Domain-Business-Rules.md` |
| Feature | `Feature-[NomeDaFeature].md` | `Feature-UserLogin.md`, `Feature-Health.md` |
| Claude | `Claude-[Tema].md` | `Claude-Overview.md`, `Claude-Skills.md` |
| Páginas estruturais | PascalCase | `Home.md` |
| Sidebar | `_Sidebar.md` | (padrão GitHub Wiki) |

---

## Política de Atualização

### Quando uma nova Feature for adicionada ao código:
1. Criar nova página `Feature-[Nome].md` seguindo o template obrigatório
2. Adicionar link em `Home.md`, `_Sidebar.md` e `Domain-Business-Rules.md`

### Quando uma Feature existente for alterada:
1. Atualizar a página correspondente
2. Verificar se links de outras páginas continuam válidos

### Quando uma Feature for removida:
1. Remover a página correspondente
2. Remover links em `Home.md`, `_Sidebar.md` e `Domain-Business-Rules.md`

### Quando componente de governança mudar:
1. Atualizar a página `Governance-*` correspondente
2. Se nova página for necessária, atualizar `Home.md` e `_Sidebar.md`

### Quando configuração ou CI/CD mudar:
1. Atualizar `Governance-Operation.md` ou `Governance-CI-CD.md`

### Quando skills, hooks ou convenções do Claude mudarem:
1. Atualizar a página `Claude-*` correspondente

### Quando regras de negócio forem criadas ou alteradas:
1. Atualizar `Domain-Business-Rules.md`
2. Atualizar a seção "Comportamento" da Feature correspondente

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

- `Instructions/architecture/technical-overview.md` — fonte de verdade arquitetural
- `Instructions/business/business-rules.md` — fonte de verdade das regras de negócio
- `.github/workflows/` — fonte de verdade de CI/CD
- `src/` — fonte de verdade dos contratos de entrada/saída de cada Feature
- `.claude/` — fonte de verdade de skills, hooks e convenções do Claude
- `.claude/rules/governance-audit.md` — verificações automatizadas de completude da wiki

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|------|---------|-----------|
| 2026-03-15 | Criado: governança inicial da GitHub Wiki | Instrução do usuário |
| 2026-03-15 | Publicação alterada para automática via wiki-publish.yml | Instrução do usuário |
| 2026-03-19 | Adicionado: gatilhos de atualização da wiki para BDD, regras de negócio, contratos e testes | Instrução do usuário |
| 2026-03-21 | Esclarecido: nomenclatura de páginas wiki distingue Feature, Infra e páginas estruturais | Análise estrutural de governança |
| 2026-03-21 | Adicionado: referência cruzada para checks de auditoria automatizada | Análise de capacidade de auto-diagnóstico |
| 2026-03-22 | Reorganizado: estrutura por agrupamentos (Governança, Domínio e Negócio, Claude, Não categorizado); padrão mínimo de página com descrição resumida obrigatória; nomenclatura por prefixo de grupo; páginas de Infra migradas para Governança; seção Claude adicionada | Instrução do usuário |
