# Regra: Convenções C# / .NET

## Propósito

Esta rule consolida convenções específicas de C# e .NET para este repositório, complementando os princípios genéricos de `engineering-principles.md`. Adaptada do repositório everything-claude-code (rules/csharp/).

---

## Estilo de Código

### Tipos e Modelos
- Preferir `record` ou `record struct` para modelos imutáveis de valor (DTOs, outputs)
- Usar `class` para entidades com identidade e ciclo de vida
- Usar `interface` para fronteiras de serviço e abstrações
- Proibido `dynamic` em código de aplicação; preferir generics ou modelos explícitos

```csharp
// Correto — record para Output imutável
public sealed record WeatherConditionsGetOutput(double Temperature, double WindSpeed);

// Correto — interface para fronteira de serviço
public interface IWeatherConditionsGetUseCase
{
    Task<WeatherConditionsGetOutput> ExecuteAsync();
}
```

### Imutabilidade
- Preferir `init` setters, parâmetros de construtor e coleções imutáveis para estado compartilhado
- Não mutar modelos de input in-place ao produzir estado atualizado

### Async e Tratamento de Erros
- Preferir `async`/`await` sobre chamadas bloqueantes como `.Result` ou `.Wait()`
- Passar `CancellationToken` em APIs async públicas quando aplicável
- Lançar exceções específicas e logar com propriedades estruturadas (SNP-001)
- Handler centralizado para exceções genéricas (DA-010, PAD-008)

### Formatação
- File-scoped namespace obrigatório (P007)
- `var` sempre que possível (P008)
- Manter `using` directives organizados e remover imports não usados
- Expression-bodied members apenas quando mantêm legibilidade

---

## Padrões C#

### Options Pattern
Usar opções fortemente tipadas para configuração em vez de ler strings brutas.

```csharp
public sealed class EndpointCacheOptions
{
    public const string SectionName = "EndpointCache";
    public required int DurationMinutes { get; init; }
    public required string ExpirationType { get; init; }
}
```

### Injeção de Dependência
- Depender de interfaces nas fronteiras de serviço
- Manter construtores focados; se precisa de muitas dependências, dividir responsabilidades
- Registrar lifetimes intencionalmente: singleton para stateless/compartilhados, scoped para dados de request, transient para workers leves e puros
- Primary constructors permitidos para injeção limpa em Controllers e UseCases

---

## Segurança C#

### Gestão de Secrets
- Nunca hardcodar API keys, tokens ou connection strings no código-fonte
- Usar variáveis de ambiente ou `appsettings.json` para desenvolvimento, e secret manager em produção
- Manter `appsettings.*.json` livre de credenciais reais

### Prevenção de SQL Injection
- Sempre usar queries parametrizadas com ADO.NET, Dapper ou EF Core
- Nunca concatenar input do usuário em strings SQL
- Validar campos de sort e operadores de filtro antes de composição dinâmica

### Validação de Input
- Validar DTOs na fronteira da aplicação (no objeto Input de cada Slice — PAD-006)
- Rejeitar model state inválido antes de executar lógica de negócio

### Autenticação e Autorização
- Usar `[Authenticate]` filter (DA-013) em vez de parsing manual de token
- Nunca logar tokens brutos, senhas ou PII

### Tratamento de Erros
- Retornar mensagens seguras para o cliente (Problem Details — DA-010)
- Logar exceções detalhadas com contexto estruturado server-side
- Não expor stack traces, SQL ou caminhos de filesystem em respostas de API

---

## Testes C#

### Framework
- **xUnit** para testes unitários (projeto `Albert.Playground.ECS.AOT.UnitTest`)
- Validação por tipo de evento + conteúdo parcial via `Assert.Contains` (SNP-001)
- `ITestOutputHelper` para output de teste

### Organização
- Espelhar estrutura `src/` no projeto de testes
- Nomear testes por comportamento, não por detalhes de implementação
- Padrão: `MetodoSobTeste_DeveResultadoEsperado_QuandoCondicao`

```csharp
public sealed class WeatherConditionsGetUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_DeveRetornarCondicoes_QuandoApiResponde()
    {
        // Arrange
        // Act
        // Assert
    }
}
```

### Cobertura
- Focar cobertura em lógica de domínio, validação, auth e caminhos de falha
- Executar `dotnet test` como gate obrigatório (passo 3 do pipeline)

---

## Compatibilidade AOT

Conforme DA-009 e P014:
- Todo código novo deve ser AOT-compatível
- Sem reflection dinâmica não anotada, sem `Assembly.Load`, sem `dynamic`
- `JsonSerializerContext` source-generated para serialização (ver `AppJsonContext`)
- Controllers MVC geram avisos AOT conhecidos — registrados em DA-009
- `dotnet build`/`dotnet run` usam JIT; AOT só ativa em `dotnet publish`

---

## Relação com Outras Rules

- `governance-policies.md` — políticas genéricas que esta rule especializa para C#
- `source-of-truth-priority.md` — hierarquia de prioridade aplicável a conflitos
- `architecture-governance.md` — decisões arquiteturais que motivam estas convenções

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-22 | Criado: consolidação de 4 rules de C# do ECC (coding-style, patterns, security, testing) adaptadas ao contexto do Playground | Adaptação do ECC |
