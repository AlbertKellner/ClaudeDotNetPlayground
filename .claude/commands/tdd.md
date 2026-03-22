---
description: Aplicar desenvolvimento guiado por testes (TDD). Criar interfaces, escrever testes PRIMEIRO, depois implementar código mínimo para passar.
---

# Comando TDD

Aplica metodologia de desenvolvimento guiado por testes.

## O Que Este Comando Faz

1. **Definir Interfaces** — Criar contratos primeiro
2. **Escrever Testes Primeiro** — Testes que falham (RED)
3. **Implementar Código Mínimo** — Apenas o suficiente para passar (GREEN)
4. **Refatorar** — Melhorar mantendo testes verdes (REFACTOR)
5. **Verificar Cobertura** — Garantir cobertura adequada

## Ciclo TDD

```
RED → GREEN → REFACTOR → REPETIR

RED:      Escrever teste que falha
GREEN:    Escrever código mínimo para passar
REFACTOR: Melhorar código, manter testes passando
REPETIR:  Próxima funcionalidade/cenário
```

## Processo para .NET / xUnit

### Passo 1: Definir Interface (SCAFFOLD)

```csharp
// Na pasta <Feature>Interfaces/
public interface I<Feature>UseCase
{
    Task<FeatureOutput> ExecuteAsync(FeatureInput input);
}
```

### Passo 2: Escrever Teste que Falha (RED)

```csharp
// No projeto de testes, espelhando a estrutura
public class FeatureUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_DeveRetornarResultado_QuandoInputValido()
    {
        // Arrange
        // Act
        // Assert
    }
}
```

### Passo 3: Executar Testes — Verificar FALHA

```bash
dotnet test src/Albert.Playground.ECS.AOT.UnitTest/Albert.Playground.ECS.AOT.UnitTest.csproj
```

### Passo 4: Implementar Código Mínimo (GREEN)

Escrever apenas o suficiente para o teste passar.

### Passo 5: Executar Testes — Verificar SUCESSO

### Passo 6: Refatorar (IMPROVE)

Melhorar o código mantendo todos os testes verdes.

### Passo 7: Verificar Cobertura

## Boas Práticas TDD para o Playground

**FAÇA:**
- Escreva o teste ANTES da implementação
- Execute testes e verifique que FALHAM antes de implementar
- Siga o padrão SNP-001 de logging nos testes
- Use `Assert.Contains` para validação parcial de mensagens de log
- Siga a estrutura de Vertical Slice (PAD-001)

**NÃO FAÇA:**
- Escrever implementação antes dos testes
- Pular a execução de testes após cada mudança
- Testar detalhes de implementação (teste comportamento)
- Usar `try-catch` genérico nos testes

## Integração com Pipeline

Após completar o ciclo TDD:
1. `dotnet build` — verificar compilação
2. `dotnet test` — todos os testes devem passar (gate do passo 3 do pipeline)
3. Prosseguir com `docker compose up -d` e validação de endpoint

## Integração com Outros Comandos

- Use `/plan` primeiro para entender o que construir
- Use `/tdd` para implementar com testes
- Use `/build-fix` se ocorrerem erros de build
- Use `/code-review` para revisar a implementação
