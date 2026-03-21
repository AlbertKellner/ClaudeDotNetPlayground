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
**Status**: Ativo (expandido por DA-014)
**Decisão**: Código sempre em inglês. Respostas ao usuário sempre em português. Toda execução de tarefa deve incluir resumo em português das mudanças e justificativa técnica. Pull requests (título, descrição e corpo) sempre em português brasileiro.
**Motivação**: Manter consistência técnica do código com padrões internacionais, enquanto a comunicação com o usuário e a documentação de mudanças permanecem acessíveis em português.
**Ver**: DA-014 para o mecanismo de enforcement de idioma em PRs.

### DA-014 — Idioma de Pull Requests: Português Brasileiro Obrigatório
**Data**: 2026-03-15
**Status**: Ativo
**Decisão**: Todo conteúdo de pull request — título, descrição e seções do corpo — deve ser escrito em português brasileiro. O repositório expõe um template de PR em português para guiar colaboradores.
**Motivação**: Manter consistência com P006 (linguagem e comunicação) e garantir que mudanças de código sejam documentadas de forma compreensível para o time. Pull requests são artefatos de comunicação; a regra de idioma se aplica a eles da mesma forma que se aplica às respostas do agente.
**Alternativas consideradas**: Validação semântica automática de idioma via API externa — descartada por introduzir dependência externa e complexidade desproporcional ao benefício. Ausência de validação — descartada: regra sem enforcement técnico tende a ser ignorada.
**Trade-offs**: A validação automatizada é estrutural (título e corpo não vazios, mínimo de caracteres), não semântica. Conteúdo redigido em outro idioma não seria bloqueado automaticamente — a regra é reforçada por governança, template e revisão humana.
**Consequências**:
- `.github/pull_request_template.md` criado com estrutura de seções em português.
- `.github/workflows/pr-language-check.yml` foi criado e posteriormente removido (2026-03-17) durante reorganização dos workflows de CI/CD — a regra permanece ativa via governança e template.
- P006 registra este princípio como regra obrigatória.
- DA-007 atualizado para referenciar PRs explicitamente.

---

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

### DA-010 — Tratamento de Exceções: IExceptionHandler com Problem Details
**Data**: 2026-03-15
**Status**: Ativo
**Decisão**: O tratamento centralizado de exceções não tratadas é feito via `IExceptionHandler` (ASP.NET Core 8), registrado em `Shared/Middleware/GlobalExceptionHandler.cs`. Respostas de erro seguem o formato Problem Details (RFC 7807 / RFC 9110).
**Motivação**: P010 proíbe `try-catch` genérico espalhado. `IExceptionHandler` é a abordagem oficial do ASP.NET Core 8 para handlers centralizados, compatível com AOT e com o sistema de Problem Details nativo do framework.
**Alternativas consideradas**:
- Middleware customizado (`IMiddleware`) — descartado: `IExceptionHandler` oferece integração mais limpa com `AddProblemDetails()` e é a API recomendada no .NET 8.
- `UseExceptionHandler(path)` com endpoint separado — descartado: mais complexo e menos expressivo.
**Trade-offs**: nenhum significativo; `IExceptionHandler` é totalmente compatível com AOT e com Controllers MVC.
**Consequências**:
- `GlobalExceptionHandler` reside em `Infra/ExceptionHandling/` — não em Features.
- `Program.cs` registra `AddExceptionHandler<GlobalExceptionHandler>()`, `AddProblemDetails()` e `app.UseExceptionHandler()`.
- Todo erro não tratado retorna HTTP 500 com body em `application/problem+json`.
- `try-catch` genérico fora de Repositories continua proibido (DA-006, P010).

### DA-011 — Estrutura Infra/ e Correlation ID Middleware
**Data**: 2026-03-15
**Status**: Ativo
**Decisão**: Componentes de infraestrutura transversal residem em `Infra/` com subpastas semânticas (`Middlewares/`, `ExceptionHandling/`, `Correlation/`). A pasta `Shared/Middleware/` é removida. O Correlation ID é garantido por request via `CorrelationIdMiddleware`, que usa GUID v7 e enriquece todos os logs da requisição através do `Serilog.Context.LogContext`. O Correlation ID é completamente opaco para Features e Endpoints.
**Motivação**: Separar infraestrutura transversal (middlewares, exception handling) de abstrações reutilizáveis de domínio (`Shared/`). Garantir rastreabilidade por request em logs estruturados sem acoplamento entre camada de aplicação e infraestrutura de observabilidade.
**Alternativas consideradas**: Manter em `Shared/Middleware/` — descartado: mistura infraestrutura com abstrações de domínio. Usar `ILogger.BeginScope()` — descartado em favor do Serilog LogContext por ser mais limpo e nativamente suportado pelo Serilog.
**Trade-offs**: Features não têm acesso direto ao CorrelationId (design intencional). O CorrelationId só é enriquecido nos logs; não é injetável via DI.
**Consequências**:
- `Infra/Middlewares/CorrelationIdMiddleware.cs` é o único lugar que conhece o Correlation ID.
- `Infra/Correlation/GuidV7.cs` é utilitário interno de Infra — não exposto para Features.
- `Serilog.AspNetCore` é dependência obrigatória do projeto.
- `CorrelationIdMiddleware` deve ser registrado ANTES de `UseExceptionHandler()` no pipeline.
- Header de entrada e saída: `X-Correlation-Id`.

### DA-012 — Runtime: Migração de .NET 8 para .NET 10
**Data**: 2026-03-15
**Status**: Ativo
**Decisão**: O projeto é migrado de `net8.0` para `net10.0`.
**Motivação**: .NET 10 introduz `Guid.CreateVersion7()` nativo (adicionado no .NET 9), eliminando a necessidade de implementação manual de GUID v7. .NET 10 é LTS e oferece melhorias de performance e AOT.
**Alternativas consideradas**: Permanecer em .NET 8 com implementação manual de GUID v7 — descartado: gera código desnecessário e dívida técnica quando a API nativa está disponível no runtime atual do projeto.
**Trade-offs**: Requer runtime .NET 10 no ambiente de execução e CI/CD.
**Consequências**:
- `<TargetFramework>net10.0</TargetFramework>` no `.csproj`.
- `Guid.CreateVersion7()` é a API canônica de geração de GUID v7.
- AOT e `InvariantGlobalization` mantidos (DA-009 permanece ativo).
- DA-004 atualizado implicitamente: "C# (.NET)" agora significa .NET 10.

### DA-013 — Autenticação: JWT Bearer Token com filtro de ação
**Data**: 2026-03-15
**Status**: Ativo
**Decisão**: A autenticação é implementada via JWT HS256 usando `System.IdentityModel.Tokens.Jwt`. A validação é feita por um `IAsyncActionFilter` (`AuthenticateFilter`) ativado pelo atributo `[Authenticate]` (implementado como `TypeFilterAttribute`). O enriquecimento de logs com `UserId` e `UserName` é realizado dentro do filtro, de forma transparente para Features e endpoints. Toda a infraestrutura de segurança reside em `Infra/Security/`.
**Motivação**: RN-002 e RN-003 exigem autenticação por Bearer Token. O padrão de filtro de ação espelha o padrão de enriquecimento já estabelecido pelo `CorrelationIdMiddleware` (DA-011) e preserva a transparência para as Features.
**Alternativas consideradas**:
- `Microsoft.AspNetCore.Authentication.JwtBearer` com middleware global — descartado: acoplamento maior e não alinha com o padrão de decorador explícito por endpoint solicitado.
- Middleware customizado com exclusão de rotas — descartado: menos expressivo e mais frágil que o atributo explícito.
**Trade-offs**:
- `JwtSecurityTokenHandler` usa reflection, gerando potenciais avisos AOT (conhecido, igual ao trade-off do MVC em DA-009).
- `dotnet build` e `dotnet run` funcionam normalmente; avisos só em `dotnet publish --aot`.
**Consequências**:
- `Infra/Security/` criada com: `ITokenService`, `AuthenticatedUser`, `TokenService`, `AuthenticateFilter`, `AuthenticateAttribute`.
- Controllers protegidos usam `[Authenticate]` na classe — nenhuma lógica de auth no corpo do endpoint.
- `UserId` e `UserName` enriquecidos automaticamente no Serilog LogContext para toda requisição autenticada.
- JWT secret configurado via `appsettings.json` (`Jwt:Secret`).
- Token expira em 1 hora. Claims: `"id"` (int) e `"userName"` (string).

### DA-015 — Padrão de Logging Estruturado: Storytelling por Classe e Método
**Data**: 2026-03-15
**Status**: Ativo
**Decisão**: Todos os logs da aplicação seguem o padrão `[NomeDaClasse][NomeDoMétodo] DescriçãoBreve` em linguagem imperativa. Todo método registra log de entrada (o que será executado + parâmetros) e log de saída (o que está sendo retornado). Loops têm log antes e depois. Toda instrução `logger.Log*()` tem linha em branco acima e abaixo no código (isolamento visual). O console usa `AnsiConsoleTheme.Code` com template `[{Timestamp}] [{CorrelationId}] [{UserName}] {Message:lj}{NewLine}{Exception}`.
**Motivação**: Rastreabilidade completa do fluxo de execução em produção sem necessidade de debugger. O formato storytelling permite reconstruir a narrativa de qualquer request a partir dos logs estruturados.
**Alternativas consideradas**: Logs ad hoc por preferência individual — descartado por inconsistência e dificuldade de filtragem. Logs apenas em pontos de erro — descartado por perda de contexto de execução normal.
**Trade-offs**: Mais verbosidade de código nos métodos. Compensado pelo ganho de rastreabilidade em produção.
**Consequências**:
- Prefixo `[NomeDaClasse][NomeDoMétodo]` é obrigatório em todos os logs de todas as classes.
- Texto descritivo após o prefixo deve ser imperativo, breve e objetivo.
- Template de console com `CorrelationId` e `UserName` é normativo (SNP-001).
- Testes validam tipo do evento + conteúdo parcial via `Contains`.
- `Program.cs` tem um log por bloco lógico de registros DI, não por instrução individual.
**ADR completo**: SNP-001 em `Instructions/snippets/canonical-snippets.md` documenta o snippet canônico completo deste padrão.

### DA-017 — Padrão de Integração HTTP Externa: Shared/ExternalApi
**Data**: 2026-03-16
**Status**: Ativo
**Decisão**: Toda integração com API HTTP externa deve ser implementada em `Shared/ExternalApi/<Servico>/`, contendo: interface Refit (`I<Servico>Api.cs`), interface de serviço (`I<Servico>ApiClient.cs`), classe cliente (`<Servico>ApiClient.cs`) e subpasta `Models/` com `<Servico>Input.cs` e `<Servico>Output.cs`. A comunicação HTTP usa Refit (`Refit.HttpClientFactory`). A resiliência usa Polly v8 via `Microsoft.Extensions.Http.Resilience` (`AddResilienceHandler`), configurada no `AddRefitClient` com retry exponencial e timeout por tentativa. O `BaseAddress` é configurado no `appsettings.json`; rotas específicas são codificadas diretamente nas interfaces Refit.
**Motivação**: Centralizar integrações externas em `Shared/ExternalApi/` garante localização previsível, separação de responsabilidades (HTTP vs. negócio) e facilita evolução e teste independente de cada integração.
**Alternativas consideradas**: `HttpClient` manual — descartado por boilerplate e menor expressividade. `IHttpClientFactory` sem Refit — descartado por ausência de tipagem via interface. Resiliência via `DelegatingHandler` manual — descartado em favor do `AddResilienceHandler` de `Microsoft.Extensions.Http.Resilience`, que é AOT-compatível e integra diretamente ao `IHttpClientBuilder`.
**Trade-offs**: Refit com Native AOT requer configuração explícita de `JsonSerializerContext` para desserialização (`SystemTextJsonContentSerializer` com contexto source-generated). A `OpenMeteoJsonContext` garante compatibilidade AOT.
**Consequências**:
- Pacotes adicionados: `Refit.HttpClientFactory`, `Microsoft.Extensions.Http.Resilience`.
- `BaseAddress` de toda integração deve ter entrada correspondente em `appsettings.json`.
- Estratégia de resiliência padrão: `AddRetry` (externo, com `DelayBackoffType.Exponential`) + `AddTimeout` (interno, por tentativa).
- Features podem depender de `I<Servico>ApiClient` de `Shared/ExternalApi/` via DI.
- `<Servico>ApiClient` implementa `I<Servico>ApiClient` e aplica logging SNP-001 obrigatório.

---

### DA-016 — Containerização e Observabilidade com Datadog Docker Agent
**Data**: 2026-03-16
**Status**: Ativo
**Decisão**: A aplicação é containerizada via Dockerfile multi-stage (SDK para Native AOT → runtime-deps para imagem mínima). O Datadog Agent é executado como container adjacente via docker-compose, coletando métricas de container e host via Docker socket e DogStatsD. Sem APM. O `DD_ENV` varia por contexto de execução (`build`, `ci`, `local`) para permitir filtragem nos dashboards do Datadog. A API key é armazenada no GitHub Environment `ClaudeCode` como secret `DD_API_KEY`.
**Motivação**: Prover observabilidade de infraestrutura e métricas de container de forma desacoplada da aplicação, sem alterar o código da app. O modelo Docker foi escolhido pelo usuário — integração via Docker socket é a abordagem recomendada pelo Datadog para coletar métricas de containers.
**Alternativas consideradas**: APM com .NET tracer — descartado pelo usuário; incompatível com Native AOT (DA-009). Agente instalado no host — descartado em favor do modelo Docker por portabilidade.
**Trade-offs**: O Datadog Agent consome recursos adicionais no mesmo host. Métricas de APM (traces da aplicação) não estão disponíveis nesta configuração.
**Consequências**:
- `src/ClaudeDotNetPlayground/Dockerfile` criado: estágio build com `mcr.microsoft.com/dotnet/sdk:10.0`, estágio runtime com `mcr.microsoft.com/dotnet/runtime-deps:10.0`.
- `docker-compose.yml` criado na raiz: serviço `app` + serviço `datadog-agent` com configuração normativa do usuário.
- `.env.example` criado: template com `DD_API_KEY`, `DD_SITE`, `DD_ENV=local`.
- `.env` (gitignored): valores reais para execução local.
- GitHub Environment `ClaudeCode` criado com secret `DD_API_KEY`.
- `ci.yml` atualizado: todos os jobs declaram `environment: ClaudeCode`; jobs `run` e `healthcheck` iniciam o Datadog Agent container; `DD_ENV` diferente por job (`build`, `ci`).
- Job `docker-build` foi adicionado ao CI e posteriormente removido em 2026-03-17 durante reorganização dos workflows (ver histórico de `technical-overview.md`).

### DA-018 — Memory Cache para Endpoints GET com Decorator Pattern
**Data**: 2026-03-19
**Status**: Ativo
**Decisão**: Endpoints GET que consomem APIs externas devem implementar Memory Cache usando `IMemoryCache`. O cache usa o ID do usuário autenticado como chave (definida no código, não configurável via JSON). A duração e o tipo de expiração são configuráveis via `appsettings.json` na seção `EndpointCache`.
**Motivação**: Reduzir chamadas repetidas a APIs externas, melhorar tempo de resposta e respeitar limites de rate limiting de serviços externos.
**Alternativas consideradas**: Cache no UseCase (rejeitado — mistura responsabilidades), cache inline no OpenMeteoApiClient (rejeitado — viola SRP), distributed cache com Redis (desnecessário para escopo atual).
**Trade-offs**: Cache por usuário consome mais memória que cache global, mas garante isolamento e evita vazamento de dados entre usuários.
**Consequências**:
- Novo decorator `CachedOpenMeteoApiClient` implementa `IOpenMeteoApiClient` e envolve `OpenMeteoApiClient`.
- Configuração de ExternalApi reestruturada em `HttpRequest`, `CircuitBreaker` e `EndpointCache`.
- `AuthenticateFilter` armazena `AuthenticatedUser` em `HttpContext.Items` para acesso pela camada de cache via `IHttpContextAccessor`.
- `IMemoryCache` e `IHttpContextAccessor` registrados no DI.

### DA-019 — Integração GitHub API com Refit + Polly e Persistência em Arquivo JSON
**Data**: 2026-03-19
**Status**: Revogado (2026-03-20) — funcionalidades de busca e sincronização de repositórios removidas do sistema
**Decisão**: A integração com a API do GitHub segue o mesmo padrão de `Shared/ExternalApi/` (DA-017): interface Refit, resiliência Polly v8, e um `DelegatingHandler` dedicado (`GitHubAuthenticationHandler`) para injetar Bearer PAT, User-Agent e Accept no pipeline HTTP. Os dados de repositórios são persistidos em arquivo JSON local com modelo compartilhado (`RepositoryFileEntry`) em `Shared/Repositories/`.
**Motivação**: Consistência com o padrão já estabelecido em DA-017 para integrações HTTP externas. Persistência em arquivo JSON atende ao requisito sem necessidade de banco de dados.
**Consequências**:
- `Shared/ExternalApi/GitHub/` criada com `IGitHubApi`, `IGitHubApiClient`, `GitHubApiClient`, `GitHubAuthenticationHandler`.
- `Shared/ExternalApi/GitHub/Models/GitHubRepositoryOutput.cs` criada com modelo de resposta e `GitHubJsonContext`.
- `Shared/Repositories/RepositoryFileEntry.cs` criada com modelo compartilhado e `RepositoryFileJsonContext`.
- Features `RepositoriesGetAll` e `RepositoriesSyncAll` criadas em `Features/Query/` e `Features/Command/` respectivamente.

### DA-020 — Isolamento de Models de Feature: Input e Output não compartilhados via Shared
**Data**: 2026-03-19
**Status**: Ativo
**Decisão**: Models de Input e Output de cada Feature devem residir exclusivamente em `<Feature>Models/` dentro da própria Slice. Não podem ser colocados em `Shared/` nem em qualquer localização fora da Feature. Features também não devem usar models de `Shared/` (incluindo `Shared/ExternalApi/*/Models/`) como tipo de retorno de seus Use Cases ou Endpoints. Se a Feature consome uma API externa via Shared, deve possuir seu próprio Output model no `<Feature>Models/` e mapear os dados internamente.
**Motivação**: Reforça o isolamento da Vertical Slice Architecture (DA-005). Models de Feature fazem parte do contrato da Slice — compartilhá-los via `Shared/` criaria acoplamento oculto entre Slices e violaria a independência de cada Slice. Usar models de `Shared/ExternalApi/` diretamente como Output da Feature acopla o contrato do endpoint ao contrato da API externa.
**Alternativas consideradas**: Permitir compartilhamento de models entre Slices via `Shared/` — rejeitado por criar dependências implícitas entre Features. Permitir reuso de models de APIs externas como Output de Features — rejeitado por acoplar o contrato da Feature ao contrato da API externa.
**Trade-offs**: Pode haver duplicação mínima de campos entre models de Features e models de APIs externas, mas a independência entre Slices e a separação de contratos são mais valiosas que a eliminação de duplicação.
**Consequências**:
- Restrição adicionada a `technical-overview.md` seção "Restrições Técnicas Conhecidas".
- PAD-007 atualizado em `patterns.md` com proibição explícita.
- `folder-structure.md` atualizado com regra de residência de models.
- Models de APIs externas permanecem em `Shared/ExternalApi/*/Models/` (DA-017) como contratos do cliente HTTP, mas não podem ser usados diretamente como Output de Features.
- `Shared/Repositories/` removida (2026-03-20) — a persistência compartilhada em arquivo foi eliminada junto com as features que a utilizavam (DA-019 revogada).

### DA-021 — Integração GitHub API com Refit + Polly e Pesquisa de Repositórios
**Data**: 2026-03-20
**Status**: Ativo
**Decisão**: A integração com a API do GitHub segue o padrão de `Shared/ExternalApi/` (DA-017): interface Refit (`IGitHubApi`), resiliência Polly v8, e um `DelegatingHandler` dedicado (`GitHubAuthenticationHandler`) para injetar PAT e User-Agent no pipeline HTTP. A feature `GitHubRepoSearch` expõe um endpoint GET autenticado que lista repositórios da conta AlbertKellner com nome e endereço Git. O resultado é cacheado por usuário autenticado via Memory Cache (DA-018) com duração configurável. Paginação automática na consulta à API do GitHub.
**Motivação**: Consistência com o padrão já estabelecido em DA-017 para integrações HTTP externas. Reutilização dos padrões de cache (DA-018) e autenticação (DA-013) já implementados.
**Alternativas consideradas**: `HttpClient` manual — descartado por boilerplate. Integração sem cache — descartada para reduzir chamadas repetidas à API GitHub e respeitar rate limiting.
**Trade-offs**: PAT é opcional; sem PAT, a API GitHub impõe rate limiting mais restritivo (60 requests/hora vs. 5000/hora).
**Consequências**:
- `Shared/ExternalApi/GitHub/` criada com `IGitHubApi`, `IGitHubApiClient`, `GitHubApiClient`, `CachedGitHubApiClient`, `GitHubAuthenticationHandler`.
- `Shared/ExternalApi/GitHub/Models/GitHubRepositoryOutput.cs` criada com modelo de resposta e `GitHubJsonContext`.
- Feature `GitHubRepoSearch` criada em `Features/Query/GitHubRepoSearch/` com Endpoint, UseCase e Models próprios (DA-020).
- 95 testes unitários passando.

### DA-022 — Escopo Atual: Contratos OpenAPI Formais e Cenários BDD Diferidos
**Data**: 2026-03-21
**Status**: Ativo
**Decisão**: Contratos OpenAPI formais e cenários BDD reais não são obrigatórios no escopo atual do projeto. Os placeholders existentes em `Instructions/contracts/` e o exemplo genérico em `Instructions/bdd/example.feature` permanecem como estrutura preparada para uso futuro. A auditoria automatizada sinaliza a ausência como aviso (não bloqueio).
**Motivação**: O projeto está em fase de exploração funcional (playground). As 4 features implementadas possuem regras de negócio claras em `business-rules.md` e contratos de entrada/saída documentados nas páginas da Wiki. Criar contratos OpenAPI formais e cenários BDD para features de playground teria custo desproporcional ao benefício, considerando que o comportamento já está validado por 95+ testes unitários e validação de endpoint via HTTP real.
**Alternativas consideradas**: (1) Criar contratos e BDD completos para todas as features — descartada por custo desproporcional. (2) Remover os placeholders e a infraestrutura de BDD — descartada: a estrutura deve estar pronta quando o domínio justificar.
**Trade-offs**: A hierarquia de prioridade de fontes de verdade (`source-of-truth-priority.md`) coloca contratos e BDD acima de regras de negócio textuais. Com esta decisão, as fontes de maior prioridade estão vazias. Quando o projeto evoluir para domínio real, contratos e BDD devem ser os primeiros artefatos criados.
**Consequências**:
- PREM-003 (contratos placeholder) permanece ativa mas agora está amparada por esta decisão explícita.
- `governance-audit.sh` emite avisos (não falhas) para features sem BDD e contratos placeholder.
- Quando uma feature de domínio real for implementada, esta decisão deve ser revisitada.

---

## Decisões Pendentes

| Id | Decisão Necessária | Impacto |
|---|---|---|
| DP-001 | Estratégia de persistência (banco de dados, ORM ou SQL direto) | Médio-Alto |
| DP-002 | Estratégia de mensageria (se aplicável) | Médio |
| DP-003 | Estratégia de testes (cobertura mínima, tipos de testes por camada) | Médio |
| DP-004a | Observabilidade — log sinks em produção (Seq, Application Insights, Elasticsearch etc.) | Médio |
| DP-004b | Observabilidade — distributed tracing (W3C TraceContext, OpenTelemetry) | Médio |
| DP-004c | Observabilidade — métricas (.NET Meter API, Prometheus etc.) | Médio |

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
| 2026-03-15 | DA-010 criada: IExceptionHandler com Problem Details como handler centralizado de exceções | P010, PAD-008 |
| 2026-03-15 | DA-013 criada: JWT Bearer Token com AuthenticateFilter/AuthenticateAttribute; Infra/Security/ criada | RN-002, RN-003 |
| 2026-03-15 | DA-007 atualizado: PRs explicitamente incluídos na regra de idioma. DA-014 criada: template de PR em português + workflow de validação | P006, instrução do usuário |
| 2026-03-16 | DA-016 criada: containerização Docker + Datadog Agent; GitHub Environment ClaudeCode; DD_ENV por contexto | Instrução do usuário |
| 2026-03-16 | DA-017 criada: padrão Shared/ExternalApi para integrações HTTP externas com Refit + Polly; primeira integração: Open-Meteo | Instrução do usuário |
| 2026-03-18 | DA-014 atualizado: nota sobre remoção do pr-language-check.yml em 2026-03-17 adicionada | Revisão de governança |
| 2026-03-18 | DA-016 atualizado: nota sobre remoção do job docker-build em 2026-03-17 adicionada | Revisão de governança |
| 2026-03-18 | DA-015 criada: padrão de logging estruturado storytelling — referenciada por technical-overview.md e SNP-001 mas ausente do registro | Revisão de governança |
| 2026-03-19 | DA-018 criada: Memory Cache para endpoints GET com Decorator Pattern; reestruturação de ExternalApi config em HttpRequest/CircuitBreaker/EndpointCache | Instrução do usuário |
| 2026-03-19 | DA-020 criada: isolamento de models de Feature — Input e Output não compartilhados via Shared | Instrução do usuário |
| 2026-03-20 | DA-019 revogada: funcionalidades de busca e sincronização de repositórios removidas; Shared/ExternalApi/GitHub/ e Shared/Repositories/ removidos | Instrução do usuário |
| 2026-03-20 | DA-021 criada: integração GitHub API com Refit + Polly + DelegatingHandler para PAT; Feature GitHubRepoSearch com cache por usuário | RN-008 |
| 2026-03-21 | DA-022 criada: contratos OpenAPI e cenários BDD diferidos para escopo de playground; auditoria emite avisos | Análise de causas-raiz |
