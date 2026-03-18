# Regra: Registro de Erros de Bash

## Propósito

Esta rule define o comportamento obrigatório do assistente quando qualquer comando Bash falha durante uma sessão de trabalho neste repositório. Todo erro de Bash deve ser documentado de forma estruturada em `bash-errors-log.md` antes de prosseguir.

---

## Princípio Fundamental

> Todo erro de Bash é conhecimento operacional durável.
> Erros encontrados e resolvidos neste repositório não devem ser perdidos — eles informam sessões futuras e evitam que o mesmo problema seja investigado repetidamente.

---

## Quando Esta Rule Se Aplica

Esta rule se aplica a **qualquer comando Bash que retorne erro**, incluindo:
- Comandos de build (`dotnet build`, `dotnet publish`, `dotnet test`)
- Comandos Docker (`docker compose`, `docker build`, `docker run`, `docker exec`)
- Comandos de sistema (`apt-get`, `curl`, `cp`, `mkdir`)
- Comandos de CI/CD e scripts de pipeline
- Qualquer outro comando shell executado durante o desenvolvimento

---

## Workflow Obrigatório

```
1. Ao receber um erro de Bash:
   a. Registrar o erro em bash-errors-log.md ANTES de tentar a correção
   b. Investigar a causa raiz
   c. Tentar a correção
   d. Se a correção funcionar: atualizar o registro com o comando que funcionou
   e. Se a correção não funcionar: atualizar o registro com a tentativa e continuar investigando

2. Estrutura obrigatória de cada registro:
   - Número sequencial dentro do arquivo
   - Data do erro
   - Comando executado que falhou
   - Erro retornado (mensagem exata, sem truncamento)
   - Causa do erro (explicação técnica objetiva)
   - Novo comando / solução que funcionou

3. Se um novo erro for uma variação de erro já registrado:
   - Criar novo registro (não sobrescrever o existente)
   - Referenciar o erro anterior se for relacionado

4. Se o erro não tiver solução encontrada na sessão:
   - Registrar com "Solução: Pendente" no campo correspondente
   - Registrar dúvida relacionada em open-questions.md se for bloqueante
```

---

## Estrutura Obrigatória de Cada Registro

```markdown
## Erro [N] — [Título descritivo do problema]

| Campo | Valor |
|---|---|
| **Número** | [N] |
| **Data** | [YYYY-MM-DD] |
| **Comando executado** | `[comando exato que falhou]` |
| **Erro retornado** | `[mensagem de erro exata]` |
| **Causa** | [Explicação técnica objetiva da causa raiz] |
| **Novo comando / solução** | `[comando ou sequência que resolveu]` ou descrição da solução |
```

---

## Regras de Preenchimento

### Campo "Comando executado"
- Usar o comando exato executado, incluindo argumentos
- Se for um passo dentro de um script maior, indicar o contexto: `docker compose up --build (step: RUN apt-get update)`
- Não simplificar ou resumir o comando

### Campo "Erro retornado"
- Copiar a mensagem de erro exata, sem parafrasear
- Se o erro for longo, incluir a parte relevante e identificadora
- Separar múltiplos erros com ` / `

### Campo "Causa"
- Explicar a causa raiz, não apenas descrever o sintoma
- Indicar o componente ou configuração responsável pelo erro
- Usar linguagem técnica objetiva

### Campo "Novo comando / solução"
- Se a solução for um único comando: colocar o comando em backticks
- Se a solução envolver múltiplos passos: descrever cada passo em sequência
- Se a solução envolver alteração de arquivo: indicar o arquivo e a mudança

---

## Política de Numeração

- Números são sequenciais e nunca reutilizados
- O número N é sempre maior que o número do erro anterior no arquivo
- Não renumerar erros existentes ao adicionar novos

---

## Manutenção do Arquivo

- `bash-errors-log.md` é um log acumulativo — erros não são removidos após resolvidos
- O arquivo documenta o histórico operacional do repositório
- Erros resolvidos permanecem como referência para problemas futuros similares
- A seção "Resultado Final" deve ser atualizada quando o pipeline for concluído com sucesso

---

## Relação com Outros Artefatos

- Se o erro revelar uma premissa de ambiente → registrar também em `assumptions-log.md`
- Se o erro gerar dúvida bloqueante → registrar também em `open-questions.md`
- Se o erro exigir mudança em `docker-compose.yml`, `Dockerfile` ou outro artefato → propagar via `governance-policies.md` §3

---

## Relação com Outras Rules

- `governance-policies.md` — políticas de ambiguidade (§4) e propagação (§3) aplicáveis a erros
