# Starter.Template.AOT

## Descrição

Ponto de entrada da documentação do projeto. API web construída com **ASP.NET Core** em **.NET 10**, compilada com **Native AOT**. Implementa autenticação por **JWT Bearer Token**, logging estruturado com **Serilog** e arquitetura **Vertical Slice**. Esta página organiza toda a documentação em quatro agrupamentos temáticos: Governança, Domínio e Negócio, Claude, e conteúdos em consolidação.

---

## Governança

Diretrizes, padrões, restrições, decisões técnicas e operacionais deste repositório.

| Página | Descrição |
|--------|-----------|
| [Arquitetura](Governance-Architecture) | Estilo Vertical Slice, estrutura de pastas, componentes e fluxo de request |
| [Padrões de Desenvolvimento](Governance-Development-Patterns) | Vertical Slice, CQRS, UseCase, Decorator, validação em Input |
| [Convenções de Código](Governance-Code-Conventions) | Nomenclatura, namespaces, variáveis, padrão de logging SNP-001 |
| [Testes](Governance-Testing) | Estratégia de testes, padrões e cobertura |
| [Segurança](Governance-Security) | Autenticação JWT, proteção de endpoints |
| [Observabilidade](Governance-Observability) | Correlation ID, Serilog, Datadog Agent |
| [CI/CD e Deploy](Governance-CI-CD) | Pipelines de build, execução e validação |
| [Integrações](Governance-Integrations) | Padrão Refit + Polly, Memory Cache, APIs externas |
| [Operação](Governance-Operation) | Pré-requisitos, configuração, build, Docker |
| [Qualidade e Manutenção](Governance-Quality) | Tratamento de exceções, Problem Details |
| [Restrições e Decisões](Governance-Decisions) | Decisões arquiteturais, restrições AOT, evolução |

---

## Domínio e Negócio

Regras de negócio, funcionalidades implementadas e conceitos de domínio.

| Página | Descrição |
|--------|-----------|
| [Visão Geral do Domínio](Domain-Overview) | Propósito da aplicação e conceitos de domínio |
| [Regras de Negócio](Domain-Business-Rules) | Índice das regras de negócio com links para as Features |

### Funcionalidades (Features)

| Página | Endpoint | Descrição |
|--------|----------|-----------|
| [Health Check](Feature-Health) | `GET /health` | Verificação de disponibilidade da aplicação |

---

## Claude

Documentação das capacidades, convenções e mecanismos do Claude neste repositório.

| Página | Descrição |
|--------|-----------|
| [Visão Geral](Claude-Overview) | Sistema de governança operacional e pipeline de validação |
| [Skills](Claude-Skills) | Catálogo de skills disponíveis por tipo de ativação |
| [Hooks](Claude-Hooks) | Hooks configurados e seus comportamentos |
| [Convenções e Restrições](Claude-Conventions) | Comportamentos obrigatórios, linguagem, restrições |
