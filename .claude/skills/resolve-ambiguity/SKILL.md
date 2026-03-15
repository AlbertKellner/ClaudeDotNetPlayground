# Skill: resolve-ambiguity

## Nome
Resolução de Ambiguidade e Lacunas

## Descrição
Esta skill orienta o comportamento do assistente quando há dúvida, ambiguidade ou lacuna relevante que precisa ser tratada antes ou durante a implementação.

## Quando Usar
Ativar esta skill quando:
- A mensagem do usuário for ambígua quanto ao comportamento esperado
- Houver múltiplas interpretações válidas com consequências funcionais diferentes
- Faltar informação crítica para implementação segura
- Houver conflito entre fontes de verdade do repositório
- O assistente precisar adotar uma premissa para avançar
- Uma dúvida anterior registrada em open-questions.md for respondida pela nova mensagem
- Uma premissa registrada em assumptions-log.md precisar ser revisada

## Entradas Esperadas
- Pode ser detectada internamente durante o processamento de qualquer mensagem
- Pode ser explicitamente reportada pelo usuário ("você entendeu certo?", "pode confirmar que...?")
- Pode ser uma mensagem do usuário que responde a uma dúvida anterior registrada

## Workflow Interno

```
1. INTERPRETAR A DÚVIDA OU AMBIGUIDADE
   - Qual é a dúvida exata?
   - É uma dúvida sobre intenção do usuário?
   - É uma dúvida sobre comportamento de negócio?
   - É uma dúvida sobre abordagem técnica?
   - É um conflito entre fontes de verdade?

2. AVALIAR O IMPACTO
   - Classificar: pequena (não bloqueante) ou material (potencialmente bloqueante)?
   - Quais artefatos são afetados se a interpretação errada for seguida?
   - A implementação errada é reversível ou irreversível?

3. CLASSIFICAR COMO BLOQUEANTE OU NÃO BLOQUEANTE
   Bloqueante quando afeta: comportamento funcional, regra de negócio, contratos, BDD,
   arquitetura, segurança, integração, mensageria, persistência, modelagem de dados.

   Não bloqueante quando: não altera comportamento, arquitetura, contrato, segurança,
   persistência, mensageria, nomenclatura relevante ou experiência do usuário.

4. SE NÃO BLOQUEANTE
   - Adotar premissa mínima conservadora
   - Registrar em assumptions-log.md
   - Continuar a implementação
   - Reportar a premissa adotada no relatório final

5. SE BLOQUEANTE
   a. Registrar em open-questions.md:
      - id, data, resumo, dúvida, impacto, artefatos afetados, se é bloqueante
   b. Verificar se parte da solicitação pode ser executada com segurança
   c. Responder no prompt com:
      - O que foi interpretado
      - Qual é a dúvida específica
      - Por que ela importa (qual é o impacto de cada interpretação)
      - O que pode ser feito agora vs. o que aguarda confirmação
   d. Aguardar confirmação antes de implementar a parte bloqueante

6. QUANDO O USUÁRIO ESCLARECER (mensagem posterior)
   a. Verificar se a mensagem resolve alguma dúvida registrada em open-questions.md
   b. Se resolve:
      - Remover a dúvida da lista ativa em open-questions.md
      - Atualizar ou remover premissas relacionadas em assumptions-log.md
      - Consolidar a resolução nos arquivos definitivos do repositório (regras, arquitetura, etc.)
      - Implementar a parte que estava pendente
   c. Se não resolve completamente:
      - Atualizar a dúvida em open-questions.md com o novo contexto
      - Registrar a nova premissa se houver avanço parcial

7. RELATAR
   - Quais dúvidas foram registradas
   - Quais dúvidas foram resolvidas e removidas da lista ativa
   - Quais premissas foram adotadas
   - Quais premissas foram atualizadas ou removidas
   - O que foi consolidado nos artefatos definitivos
```

## Saídas Esperadas
- Dúvidas materiais registradas antes de implementação insegura
- Premissas conservadoras documentadas
- Resposta clara ao usuário com a dúvida e seu impacto
- Resolução consolidada nos artefatos definitivos quando o usuário esclarecer
- Registros de dúvidas e premissas limpos após resolução

## Arquivos de Governança Relacionados
- `.claude/rules/ambiguity-handling.md`
- `.claude/rules/source-of-truth-priority.md`
- `open-questions.md`
- `assumptions-log.md`
- Arquivos de `Instructions/` relevantes para a dúvida em questão

## Nota sobre Invocação
Esta skill não exige ativação explícita. É ativada automaticamente quando o assistente detecta ambiguidade ou lacuna durante o processamento de qualquer mensagem. Ela também é ativada quando o usuário responde a uma pergunta anterior, para que o ciclo de resolução seja concluído corretamente.
