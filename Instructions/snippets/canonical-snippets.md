# Registro de Snippets Normativos Canônicos

## Propósito

Este arquivo armazena snippets normativos — trechos técnicos fornecidos explicitamente pelo usuário para serem preservados na íntegra e respeitados em futuras implementações.

**Snippets registrados aqui são obrigatórios** — não devem ser reescritos livremente.
Qualquer alteração exige nova instrução explícita do usuário ou conflito técnico devidamente reportado.

---

## Como Usar Este Arquivo

Para adicionar um snippet canônico, usar a estrutura abaixo.
Para consultar um snippet existente, localizar pelo id ou escopo.

---

## Estrutura de Cada Snippet

```markdown
## Snippet [ID]

| Campo | Valor |
|---|---|
| **Id** | SNP-[número] |
| **Data** | [YYYY-MM-DD] |
| **Título** | [Título descritivo] |
| **Intenção** | [O que este snippet deve fazer] |
| **Instrução original** | [Resumo do que o usuário pediu] |
| **Classificação** | Normativo |
| **Escopo** | [Onde se aplica: arquivo, módulo, contexto] |
| **Regra de preservação** | [Como deve ser aplicado] |
| **Adaptações mínimas permitidas** | [O que pode ser ajustado sem violar a intenção] |
| **Artefatos relacionados** | [Arquivos ou componentes que usam este snippet] |

### Conteúdo do Snippet

[Conteúdo exato do snippet aqui]

### Histórico de Alterações

| Data | Alteração | Motivo |
|---|---|---|
| [data] | Registrado inicialmente | Instrução explícita do usuário |
```

---

## Snippets Registrados

## Snippet SNP-001

| Campo | Valor |
|---|---|
| **Id** | SNP-001 |
| **Data** | 2026-03-15 |
| **Título** | Padrão de Logging Estruturado — Storytelling por Classe e Método |
| **Intenção** | Definir o formato obrigatório de escrita de logs em toda a aplicação, garantindo rastreabilidade completa da execução em formato storytelling |
| **Instrução original** | Padrão `[NomeDaClasse][NomeDoMétodo] DescriçãoBreve` em linguagem imperativa; logs de entrada, processamento e saída em cada método; log antes e depois de cada loop; Program.cs com logs por bloco; console colorido com tema ANSI; template com timestamp, correlationId, userName e mensagem; linha em branco acima e abaixo de cada instrução de log; testes validando tipo de log e conteúdo parcial via Contains |
| **Classificação** | Normativo |
| **Escopo** | Todas as classes da aplicação: Features, Infra, Program.cs |
| **Regra de preservação** | O formato de prefixo `[NomeDaClasse][NomeDoMétodo]` deve ser preservado literalmente em todos os logs. O template de console deve ser preservado. A regra de isolamento (linha em branco) deve ser preservada. |
| **Adaptações mínimas permitidas** | O texto descritivo após o prefixo pode variar conforme o contexto, desde que seja imperativo, breve e objetivo. |
| **Artefatos relacionados** | Todos os arquivos `.cs` da aplicação; configuração do Serilog em `Program.cs`; projeto de testes |

### Conteúdo do Snippet

#### Formato de prefixo (normativo)
```
[NomeDaClasse][NomeDoMétodo] DescriçãoBreveFrase
```

#### Regras de escrita de logs (normativas)

1. **Prefixo obrigatório**: Todo log deve começar com `[NomeDaClasse][NomeDoMétodo]`
2. **Linguagem imperativa**: A descrição deve ser breve, objetiva e no imperativo (ex: "Processar requisição", "Retornar resposta", "Gerar token")
3. **Log de entrada do método**: Primeiro log de cada método informa o que será executado e registra os objetos recebidos como entrada
4. **Log de saída do método**: Último log antes do `return` informa o que está sendo retornado
5. **Log antes de loops**: Antes de qualquer `for`, `foreach` ou estrutura de iteração, deve haver um log informando o que será iterado
6. **Log após loops**: Na linha imediatamente após o encerramento do loop, deve haver um log informando que a iteração foi concluída
7. **Isolamento visual**: Toda instrução `logger.Log*()` deve ter uma linha em branco acima e uma linha em branco abaixo no código

#### Template de console colorido (normativo)
```csharp
.WriteTo.Console(
    theme: AnsiConsoleTheme.Code,
    outputTemplate: "[{Timestamp:dd/MM/yyyy HH:mm:ss.fffffff}] [{CorrelationId}] [{UserName}] {Message:lj}{NewLine}{Exception}")
```

#### Padrão de logs em Program.cs (normativo)
- Log inicial informando que a aplicação está sendo iniciada
- Um log por bloco lógico (Serilog, DI de infraestrutura, DI de features, DI de segurança, pipeline de middlewares)
- O log de bloco é escrito antes do conjunto de instruções do bloco
- Não criar log por instrução individual dentro do bloco

#### Padrão de testes de log (normativo)
```csharp
// Validação: tipo do evento + conteúdo parcial da mensagem via Contains
Assert.Contains(logs, l =>
    l.Level == LogLevel.Information &&  // ou Warning, Error, etc.
    l.Message.Contains("termo-principal-esperado"));
```

### Histórico de Alterações

| Data | Alteração | Motivo |
|---|---|---|
| 2026-03-15 | Registrado inicialmente | Instrução explícita do usuário — padrões de logging da aplicação |

---

## Instruções para o Assistente

Quando um snippet for registrado aqui:

1. **Antes de implementar** artefatos no escopo do snippet, verificar se este arquivo contém um snippet aplicável
2. **Aplicar o snippet na íntegra** nos locais de destino indicados
3. **Não reescrever** o conteúdo do snippet — mesmo que pareça que há uma "versão melhor"
4. **Reportar** adaptações mínimas inevitáveis com justificativa explícita
5. **Registrar conflito** se o snippet conflitar com estrutura existente, segurança ou contratos — não substituir silenciosamente

Quando o usuário fornecer novo trecho para substituir um snippet existente:
1. Atualizar o conteúdo do snippet neste arquivo
2. Registrar a alteração na tabela de histórico
3. Propagar a mudança para os artefatos de destino
4. Reportar os locais que foram atualizados
