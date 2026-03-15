# Decisões Arquiteturais

## Propósito

Este arquivo mantém um registro de alto nível das decisões arquiteturais mais importantes deste repositório. Cada decisão significativa deve ter um ADR completo em `Instructions/decisions/`.

---

## Decisões Ativas

### DA-001 — Adoção de Sistema de Governança Persistente
**Data**: Bootstrap
**Status**: Ativo
**Decisão**: Este repositório usa um sistema de governança persistente em linguagem natural que serve como sistema operacional de todas as interações futuras.
**Motivação**: Garantir consistência, preservar conhecimento durável e eliminar a necessidade de reapresentar contexto a cada sessão.
**ADR**: Ver `Instructions/decisions/` quando criado.

### DA-002 — Separação entre Governança Técnica e de Negócio
**Data**: Bootstrap
**Status**: Ativo
**Decisão**: Instruções técnicas ficam em `Instructions/architecture/` e instruções de negócio ficam em `Instructions/business/`. Os dois domínios não se mesclam nos mesmos arquivos.
**Motivação**: Separar responsabilidades facilita navegação, manutenção e propagação de mudanças sem contaminação entre domínios.

### DA-003 — Governança Antes de Implementação
**Data**: Bootstrap
**Status**: Ativo
**Decisão**: Toda definição durável deve ser persistida na governança antes de qualquer mudança de código ou artefato.
**Motivação**: Implementação sem governança cria código sem memória de intenção.

### DA-004 — Stack Tecnológica: C# (.NET) com ASP.NET Core
**Data**: 2026-03-15
**Status**: Ativo (mecanismo HTTP atualizado por DA-008)
**Decisão**: A linguagem principal é C# (.NET). O framework de exposição HTTP é ASP.NET Core.
**Motivação**: Stack definida pelas convenções e pelo código existente no repositório.
**Consequências**:
- Namespaces devem seguir o padrão file-scoped (ver P007).
- Variáveis devem usar `var` sempre que possível (ver P008).
- Todo código deve ser escrito em inglês.
- Ver DA-008 para o mecanismo de exposição HTTP adotado.

### DA-008 — Mecanismo de Exposição HTTP: Controllers com Actions
**Data**: 2026-03-15
**Status**: Ativo
**Decisão**: A exposição HTTP é feita via Controllers com Actions bem definidas, usando ASP.NET Core MVC (não Minimal API).
**Motivação**: Preferência explícita do time — Controllers com Actions oferecem organização mais explícita, melhor suporte a atributos de rota e são mais familiares para o time.
**Alternativas consideradas**: Minimal API — descartada em favor de Controllers.
**Consequências**:
- Cada Slice tem seu próprio Controller, localizado na pasta `<Feature>Endpoint/`.
- O Controller contém uma ou mais Actions bem definidas.
- O arquivo do Controller segue a nomenclatura `<Feature>Endpoint.cs`, dentro de `<Feature>Endpoint/`.
- O Controller não contém lógica de negócio — apenas orquestra request/response, status codes e logs.
- Toda lógica de negócio permanece no UseCase da Slice.
- Substitui a diretriz de Minimal API de DA-004.

---

### DA-005 — Arquitetura: Vertical Slice com Segregação Command/Query
**Data**: 2026-03-15
**Status**: Ativo
**Decisão**: O projeto adota Vertical Slice Architecture com segregação explícita de operações de leitura (Query) e escrita (Command). Toda funcionalidade é implementada como uma Slice vertical isolada, dentro de `Features/Query/` ou `Features/Command/`.
**Motivação**: Isolar mudanças por funcionalidade, tornar a intenção das operações explícita e facilitar a evolução independente de leituras e escritas.
**Alternativas consideradas**: Clean Architecture em camadas horizontais globais — descartada por criar acoplamento entre Features e dificultar o isolamento de mudanças.
**Consequências**:
- Toda funcionalidade nova deve ser classificada como Query ou Command antes de ser implementada.
- Slices não se comunicam diretamente entre si.
- Lógica compartilhada reside em `Shared/`.
- Pastas globais de `Services/` ou `Repositories/` não existem.

### DA-006 — Princípio da Responsabilidade Única como Regra Estrutural
**Data**: 2026-03-15
**Status**: Ativo
**Decisão**: O SRP é uma regra estrutural obrigatória, não uma recomendação. As responsabilidades são: Endpoint (request/response + logs), UseCase (orquestração de negócio), Repository (acesso a dados + materialização de domínio), Models/Input (validação de payload), Shared (infraestrutura genérica).
**Motivação**: Aumentar legibilidade, facilitar testes e reduzir o impacto de mudanças.
**Consequências**:
- Lógica de negócio fora do UseCase da Slice é uma violação.
- `try-catch` genérico fora de handler centralizado é proibido.
- Validação de payload fora do objeto Input é proibida.

### DA-007 — Linguagem e Comunicação do Agente
**Data**: 2026-03-15
**Status**: Ativo
**Decisão**: Código sempre em inglês. Respostas ao usuário sempre em português. Toda execução de tarefa deve incluir resumo em português das mudanças e justificativa técnica.
**Motivação**: Manter consistência técnica do código com padrões internacionais, enquanto a comunicação com o usuário permanece acessível em português.

### DA-009 — Compilação AOT (Native AOT)
**Data**: 2026-03-15
**Status**: Ativo
**Decisão**: Todos os projetos devem ser configurados com `<PublishAot>true</PublishAot>` e `<InvariantGlobalization>true</InvariantGlobalization>`. Todo código novo deve ser AOT-compatível.
**Motivação**: Reduzir tempo de startup, consumo de memória e tamanho do binário publicado; alinhar com práticas modernas de deployment de serviços .NET.
**Alternativas consideradas**: ReadyToRun — descartada por não oferecer os mesmos ganhos de startup e footprint que Native AOT.
**Trade-offs**:
- Controllers MVC (DA-008) usam reflection para roteamento e model binding, gerando avisos de incompatibilidade AOT durante `dotnet publish`. `dotnet build` e `dotnet run` continuam funcionando normalmente com JIT.
- Migração futura para Minimal API com source generators eliminaria esses avisos, mas DA-008 permanece ativo até decisão explícita do usuário.
**Consequências**:
- Todo código novo deve evitar reflection dinâmica não anotada.
- `dotnet publish` pode emitir avisos AOT relacionados ao MVC; esses avisos são conhecidos e registrados aqui.
- Ver P014 para o princípio de engenharia correspondente.

---

## Decisões Pendentes

| Id | Decisão Necessária | Impacto |
|---|---|---|
| DP-001 | Estratégia de persistência (banco de dados, ORM ou SQL direto) | Médio-Alto |
| DP-002 | Estratégia de mensageria (se aplicável) | Médio |
| DP-003 | Estratégia de testes (cobertura mínima, tipos de testes por camada) | Médio |
| DP-004 | Estratégia de observabilidade (logging estruturado, tracing, métricas) | Médio |

---

## Template para Nova Decisão

Ao adicionar uma nova decisão:
```
### DA-[número] — [Título da Decisão]
**Data**: [data]
**Status**: Ativo | Substituído por DA-[n] | Depreciado
**Decisão**: [o que foi decidido]
**Motivação**: [por que foi decidido assim]
**Alternativas consideradas**: [outras opções avaliadas]
**Trade-offs**: [o que se perde com esta decisão]
**Consequências**: [o que muda com esta decisão]
**ADR completo**: Instructions/decisions/[nome-do-adr].md
```

---

## Referências Cruzadas

- `Instructions/decisions/` — ADRs completos
- `Instructions/architecture/engineering-principles.md` — princípios que motivam as decisões
- `Instructions/architecture/technical-overview.md` — visão geral que reflete estas decisões

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| Bootstrap | DA-001, DA-002, DA-003 criadas | — |
| 2026-03-15 | DA-004 a DA-007 criadas: stack C#/.NET/Minimal API, Vertical Slice, SRP estrutural, linguagem do agente | Instruções do usuário |
| 2026-03-15 | DA-004 atualizada: referência ao mecanismo HTTP movida para DA-008. DA-008 criada: Controllers com Actions substituem Minimal API | Instrução do usuário |
| 2026-03-15 | DA-009 criada: Native AOT obrigatório; trade-off com Controllers MVC registrado | Instrução do usuário |
