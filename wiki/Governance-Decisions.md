# Restrições e Decisões

## Descrição

Documenta as principais decisões arquiteturais, as restrições técnicas (especialmente Native AOT) e as decisões pendentes. Deve ser consultado ao entender por que certas abordagens foram escolhidas ou quais limitações existem.

## Contexto

O projeto acumula decisões arquiteturais registradas como ADRs (Architecture Decision Records) que governam todas as implementações futuras. Cada decisão documenta o contexto, as alternativas consideradas, os trade-offs e as consequências. As restrições técnicas mais significativas derivam da adoção de Native AOT, que impõe limitações sobre o uso de reflection dinâmica e exige serialização source-generated.

---

## Decisões Ativas Principais

| ID | Título | Data | Status |
|---|---|---|---|
| DA-004 | C# .NET 10 com ASP.NET Core | 2026-03-15 | Ativo |
| DA-005 | Vertical Slice Architecture com Command/Query | 2026-03-15 | Ativo |
| DA-008 | Controllers com Actions (não Minimal API) | 2026-03-15 | Ativo |
| DA-009 | Native AOT (`PublishAot=true`) | 2026-03-15 | Ativo |
| DA-010 | Tratamento de Exceções: `IExceptionHandler` com Problem Details | 2026-03-15 | Ativo |
| DA-011 | Estrutura `Infra/` + Correlation ID Middleware | 2026-03-15 | Ativo |
| DA-012 | Runtime: Migração de .NET 8 para .NET 10 | 2026-03-15 | Ativo |
| DA-013 | Autenticação JWT Bearer Token | 2026-03-15 | Ativo |
| DA-014 | Idioma de Pull Requests: Português Brasileiro Obrigatório | 2026-03-15 | Ativo |
| DA-015 | Logging estruturado (padrão storytelling) | 2026-03-15 | Ativo |
| DA-016 | Containerização Docker + Datadog Agent | 2026-03-16 | Ativo |
| DA-017 | Padrão de integração com API externa (Refit + Polly) | 2026-03-16 | Ativo |
| DA-018 | Memory Cache com Decorator Pattern | 2026-03-19 | Ativo |
| DA-020 | Isolamento de models de Feature (`Input`/`Output` em `<Feature>Models/`) | 2026-03-19 | Ativo |

---

## Decisões Revogadas

| ID | Título | Data | Motivo |
|---|---|---|---|
| DA-019 | Integração externa com persistência em arquivo JSON | 2026-03-19 | Revogada — funcionalidades removidas durante sanitização do template |
| DA-023 | Integração externa removida | 2026-03-21 | Revogada — integração removida durante sanitização do template |

---

## Restrições Técnicas

As seguintes restrições são obrigatórias e aplicam-se a todo código novo ou alterado:

- Todo código deve compilar sem erros (`dotnet build`) antes de qualquer commit
- A aplicação deve iniciar e responder em `/health` antes de qualquer commit
- Todos os testes devem passar (`dotnet test`) antes de executar `docker compose up -d`
- Slices não podem depender diretamente de outras Slices
- `Shared/` não pode depender de `Features/`
- Lógica de negócio não pode estar em Endpoints nem em Repositories
- Validação de payload deve estar no objeto `Input` de cada Slice (em `<Feature>Models/`)
- Models de Input e Output de cada Feature devem residir exclusivamente em `<Feature>Models/` dentro da própria Slice — não podem ser compartilhados via `Shared/`
- Features não devem usar models de `Shared/` (incluindo `Shared/ExternalApi/*/Models/`) como tipo de retorno de seus Use Cases ou Endpoints — devem possuir seu próprio Output model e mapear os dados internamente

---

## Restrições do Native AOT (DA-009)

O projeto é configurado para publicação com Native AOT, o que impõe restrições específicas:

### Configuração obrigatória

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
  <InvariantGlobalization>true</InvariantGlobalization>
</PropertyGroup>
```

### Restrições de código

- **Proibido**: reflection dinâmica não anotada, `Assembly.Load`, uso de `dynamic`
- **Obrigatório**: `JsonSerializerContext` source-generated para toda serialização/desserialização JSON
- **Obrigatório**: model binding providers AOT-compatíveis (`NullModelBinderProvider`, `FallbackSimpleTypeModelBinderProvider`)
- **Obrigatório**: `NoOpObjectModelValidator` substituindo o validador padrão baseado em reflection

### Trade-offs conhecidos

- Controllers MVC (DA-008) utilizam reflection para roteamento e model binding, gerando avisos de incompatibilidade AOT durante `dotnet publish`
- `JwtSecurityTokenHandler` (DA-013) utiliza reflection, gerando potenciais avisos AOT
- `dotnet build` e `dotnet run` continuam funcionando normalmente com JIT — avisos AOT aparecem apenas em `dotnet publish`
- `AotControllerPreservation.PreserveControllers()` é chamado explicitamente em `Program.cs` para evitar que o linker remova os Controllers

### Componentes de compatibilidade AOT

| Componente | Localização | Propósito |
|---|---|---|
| `AppJsonContext` | `Infra/Json/` | `JsonSerializerContext` source-generated para serialização AOT-compatível |
| `NullModelBinderProvider` | `Infra/ModelBinding/` | Substitui providers incompatíveis com AOT (TryParse, FloatingPoint) |
| `FallbackSimpleTypeModelBinderProvider` | `Infra/ModelBinding/` | Substitui `SimpleTypeModelBinderProvider` para compatibilidade AOT |
| `EnhancedModelMetadataActivator` | `Infra/ModelBinding/` | Workaround AOT: ativa `IsEnhancedModelMetadataSupported` antes do primeiro request |
| `NoOpObjectModelValidator` | `Infra/ModelValidation/` | Substitui `IObjectModelValidator` padrão (reflection-based) por implementação vazia |

---

## Processo de Decisão Arquitetural (ADR)

Decisões arquiteturais relevantes são registradas como ADRs (Architecture Decision Records) em `Instructions/decisions/`. Cada ADR preserva o raciocínio por trás de uma decisão — não apenas "o que foi decidido", mas "por que foi decidido assim" e "quais alternativas foram consideradas".

### Quando registrar uma decisão

- Decisão tecnológica relevante (linguagem, framework, banco, broker)
- Decisão arquitetural estruturante (estilo, padrão de integração, fronteiras)
- Restrição técnica importante estabelecida
- Alternativa descartada cuja razão deve ser preservada
- Mudança significativa de direção técnica

### Estrutura de cada ADR

| Seção | Conteúdo |
|---|---|
| **Contexto** | Situação que levou à necessidade da decisão |
| **Decisão** | O que foi decidido — linguagem afirmativa e clara |
| **Alternativas** | Opções avaliadas e por que cada uma foi descartada |
| **Trade-offs** | O que a decisão sacrifica ou compromete |
| **Consequências** | Impacto em outros artefatos, restrições futuras |

### Status de ADRs

- **Ativo**: decisão em vigor
- **Substituído**: superado por ADR mais recente (referencia o substituto)
- **Revogado**: não mais aplicável, com justificativa
- **Depreciado**: não mais aplicável mas não substituído por outro

ADRs substituídos ou revogados **não são deletados** — documentam por que houve mudança de direção.

---

## Decisões Pendentes

| ID | Decisão Necessária | Impacto |
|---|---|---|
| DP-001 | Estratégia de persistência (banco de dados, ORM ou SQL direto) | Médio-Alto |
| DP-002 | Estratégia de mensageria (se aplicável) | Médio |
| DP-003 | Estratégia de testes (cobertura mínima, tipos de testes por camada) | Médio |
| DP-004a | Observabilidade — log sinks em produção (Seq, Application Insights, Elasticsearch etc.) | Médio |
| DP-004b | Observabilidade — distributed tracing (W3C TraceContext, OpenTelemetry) | Médio |
| DP-004c | Observabilidade — métricas (.NET Meter API, Prometheus etc.) | Médio |

---

## Referências

- [Arquitetura](Governance-Architecture) — visão geral da arquitetura e componentes
- [Padrões de Desenvolvimento](Governance-Development-Patterns) — padrões técnicos adotados no projeto
- [Qualidade e Manutenção](Governance-Quality) — políticas de qualidade e tratamento de erros
