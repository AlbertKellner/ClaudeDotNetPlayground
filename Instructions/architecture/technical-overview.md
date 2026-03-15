# Visão Geral Técnica

## Propósito

Este arquivo descreve a visão arquitetural de alto nível deste repositório. É o ponto de entrada para entender a estrutura técnica do projeto, como os componentes se relacionam e quais decisões fundamentais já foram tomadas.

---

## Stack Tecnológica

| Camada | Tecnologia | Decisão |
|---|---|---|
| Linguagem principal | C# (.NET) | DA-004 |
| Framework principal | ASP.NET Core — Controllers com Actions | DA-004, DA-008 |
| Persistência | A definir por Feature | — |
| Mensageria | A definir | — |
| Containerização | A definir | — |
| CI/CD | A definir | — |
| Observabilidade | A definir | — |

---

## Estilo Arquitetural

O projeto adota **Vertical Slice Architecture** com segregação explícita de operações de leitura (**Query**) e escrita (**Command**).

Cada funcionalidade é implementada como uma Slice vertical isolada, contendo todos os artefatos necessários (endpoint, use case, repository, models, interfaces, scripts SQL) dentro da sua própria pasta, sob `Features/Query` ou `Features/Command`.

Não há camadas horizontais globais (ex.: pasta `Services/` ou `Repositories/` global). Toda lógica especializada por funcionalidade reside dentro da Slice correspondente. Lógica genuinamente compartilhada entre Slices reside em `Shared/`.

*Referência: DA-004, DA-005 — `Instructions/architecture/architecture-decisions.md`*

---

## Componentes Principais

| Componente | Localização | Responsabilidade |
|---|---|---|
| Controllers com Actions | `Features/<tipo>/<Feature>/<Feature>Endpoint/` | Orquestração de request/response, logging de fluxo |
| Use Cases | `Features/<tipo>/<Feature>/<Feature>UseCase/` | Orquestração da lógica de negócio da Slice |
| Repositories | `Features/<tipo>/<Feature>/<Feature>Repository/` | Acesso a dados; materialização de objetos de domínio |
| Models (Input/Output/Entity) | `Features/<tipo>/<Feature>/<Feature>Models/` | Contratos de entrada, saída e entidades de domínio por Slice |
| Interfaces | `Features/<tipo>/<Feature>/<Feature>Interfaces/` | Contratos para repositórios e integrações externos ao UseCase |
| Shared | `Shared/` | Abstrações, utilitários, clientes e helpers reutilizáveis entre Slices |

---

## Fronteiras e Responsabilidades

- **Features/Query**: Slices de leitura — não alteram estado.
- **Features/Command**: Slices de escrita — alteram estado.
- **Shared**: Recursos reutilizáveis sem lógica especializada para uma única Slice.
- Slices **não se comunicam diretamente entre si**. Lógica compartilhada vai para `Shared/`.

---

## Fluxos Principais de Alto Nível

```
Request HTTP
    └── Controller / Action (pasta Endpoint)
            └── UseCase
                    └── Repository (via Interface)
                            └── Banco de dados / serviço externo
```

O Controller não contém lógica de negócio — apenas orquestra request/response, define status codes e escreve logs relevantes.

---

## Dependências Externas

> **Pendente de definição.** Dependências externas serão listadas aqui à medida que forem introduzidas.

---

## Restrições Técnicas Conhecidas

- Todo código deve compilar sem erros (`dotnet build`) antes de qualquer commit.
- Todos os testes devem passar sem erros antes de qualquer commit.
- Slices não podem depender de outras Slices diretamente.
- `Shared/` não pode depender de Features.
- Lógica de negócio não pode estar em Endpoints nem em Repositories.
- Validação de payload deve estar no objeto `Input` de cada Slice (em `<Feature>Models/`), não em repositórios ou componentes de persistência.

---

## Referências Cruzadas

- `Instructions/architecture/engineering-principles.md` — princípios que guiam as decisões
- `Instructions/architecture/patterns.md` — padrões adotados
- `Instructions/architecture/architecture-decisions.md` — decisões registradas
- `Instructions/architecture/folder-structure.md` — estrutura de pastas
- `Instructions/business/domain-model.md` — modelo de domínio relacionado

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| Bootstrap | Estrutura inicial criada | — |
| 2026-03-15 | Stack, arquitetura e componentes definidos | DA-004, DA-005 |
| 2026-03-15 | Framework HTTP atualizado: Minimal API substituída por Controllers com Actions | DA-008 |
