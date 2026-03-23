# Testes

## Descrição

Documenta a estratégia e os padrões de testes utilizados neste projeto. Deve ser consultado ao escrever ou manter testes.

## Contexto

O projeto possui um projeto dedicado de testes unitários que espelha a estrutura do projeto principal. Todos os testes devem passar antes de qualquer `docker compose up -d` (gate obrigatório do pipeline pré-commit, passo 3). Atualmente, o projeto conta com mais de 95 testes unitários passando.

---

## Projeto de Testes

| Item | Valor |
|---|---|
| Nome do projeto | `Albert.Playground.ECS.AOT.UnitTest` |
| Localização | `src/Albert.Playground.ECS.AOT.UnitTest/` |
| Estrutura | Espelha o projeto principal: `Features/`, `Infra/`, `Shared/`, `TestHelpers/` |
| Cobertura atual | 95+ testes unitários passando |

---

## Padrão de Testes de Log (SNP-001)

Os testes de log validam o tipo do evento e o conteúdo parcial da mensagem via `Contains`:

```csharp
Assert.Contains(logs, l =>
    l.Level == LogLevel.Information &&
    l.Message.Contains("termo-principal-esperado"));
```

Este padrão garante que:
- O nível de log está correto (Information, Warning, Error, etc.)
- A mensagem contém o conteúdo esperado sem depender da formatação exata
- Mudanças cosméticas na mensagem não quebram os testes

---

## Gate Obrigatório no Pipeline Pré-Commit

O passo 3 do pipeline de validação pré-commit é um **gate obrigatório**:

```bash
dotnet test src/Albert.Playground.ECS.AOT.UnitTest/Albert.Playground.ECS.AOT.UnitTest.csproj
```

- Falha em qualquer teste **bloqueia** o avanço para os passos seguintes
- O `docker compose up -d` (publish Release/AOT) só é executado após todos os testes passarem em modo debug
- Testes falhando bloqueiam o commit — corrigir antes de avançar

---

## O Que Testar por Camada

| Camada | O Que Testar |
|---|---|
| **Endpoint** | Logs de storytelling (entrada/saída), status codes HTTP retornados, delegação correta ao UseCase |
| **UseCase** | Orquestração da lógica de negócio, chamadas corretas às dependências (via mock), logging de fluxo |
| **Client** (Shared/ExternalApi) | Delegação HTTP correta, logging SNP-001, tratamento de erros |
| **Cache** (Decorator) | Cache hit (retorno do cache sem chamar API), cache miss (chamada à API e armazenamento), isolamento por usuário |
| **Infra** | Comportamento de middlewares, geração/validação de tokens, tratamento de exceções |

---

## Estrutura de Testes

O projeto de testes espelha a organização do projeto principal:

```
src/Albert.Playground.ECS.AOT.UnitTest/
├── Features/
│   ├── Query/
│   │   ├── TestGet/
│   │   ├── WeatherConditionsGet/
│   │   ├── GitHubRepoSearch/
│   │   └── PokemonGet/
│   └── Command/
│       └── UserLogin/
├── Infra/
│   ├── Security/
│   ├── Middlewares/
│   └── ExceptionHandling/
├── Shared/
│   └── ExternalApi/
│       ├── OpenMeteo/   # OpenMeteoApiClientTests, CachedOpenMeteoApiClientTests
│       ├── GitHub/      # GitHubApiClientTests, CachedGitHubApiClientTests
│       └── Pokemon/     # PokemonApiClientTests, CachedPokemonApiClientTests
└── TestHelpers/
```

---

## Cenários BDD e Contratos OpenAPI

Atualmente, o projeto não possui cenários BDD nem contratos OpenAPI formais (DA-022). O projeto está em fase de exploração funcional (playground), e estas formalizações serão adicionadas quando o domínio justificar.

A auditoria automatizada (`governance-audit.sh`) emite **avisos** (não falhas) para features sem BDD e contratos placeholder.

---

## Boas Práticas

- Testes devem ser independentes — nenhum teste deve depender da ordem de execução
- Usar mocks para isolar a camada sob teste
- Validar logs com `Contains` (não comparação exata) para resiliência a mudanças cosméticas
- Nomear testes descritivamente: `Should_ReturnOk_When_ValidInput`, `Should_LogWarning_When_CacheExpired`
- Cada teste deve validar um único comportamento

---

## Referências

- [CI/CD](Governance-CI-CD) — pipeline que executa os testes automaticamente
- [Convenções de Código](Governance-Code-Conventions) — padrões de logging testados
- [Padrões de Desenvolvimento](Governance-Development-Patterns) — padrões que determinam a organização dos testes
