# Skill: apply-user-snippet

## Nome
Aplicação de Trecho Técnico do Usuário

## Descrição
Esta skill orienta o comportamento do assistente quando o usuário fornece um trecho técnico — código, configuração, schema, YAML, JSON, SQL, Terraform, Helm, política IAM, definição de mensageria, contrato ou qualquer outro fragmento técnico — e o assistente precisa decidir como tratá-lo antes de aplicá-lo.

## Quando Usar
Ativar esta skill sempre que a mensagem do usuário contiver:
- Trecho de código em qualquer linguagem
- Arquivo de configuração (YAML, JSON, TOML, .env, etc.)
- Schema (JSON Schema, Avro, Protobuf, SQL DDL, etc.)
- Contrato (OpenAPI, AsyncAPI, GraphQL SDL, etc.)
- Infraestrutura como código (Terraform, Helm, CloudFormation, Bicep, etc.)
- Definição de mensageria (tópicos, filas, exchanges, bindings, políticas)
- Qualquer outro fragmento técnico que deva ser aplicado ao repositório

## Entradas Esperadas
- O trecho técnico em si
- Contexto narrativo do usuário sobre o trecho (pode ser mínimo ou ausente)
- Indicações explícitas ou implícitas sobre como o trecho deve ser tratado

## Workflow Interno

```
1. SEPARAR TRECHO DO CONTEXTO NARRATIVO
   - Identificar o trecho técnico na mensagem
   - Identificar o texto narrativo ao redor (intenção, contexto, instrução)
   - Processar o texto narrativo com governance-policies.md §1 (normalização de linguagem)

2. CLASSIFICAR O TRECHO
   Analisar os sinais na mensagem:

   NORMATIVO se o usuário disse:
   - "inclua exatamente assim", "copie isso", "preserve isso"
   - "use exatamente esse trecho", "não altere"
   - "quero que fique literalmente assim"
   - "é pra ser esse código mesmo"

   ILUSTRATIVO se o usuário disse:
   - "algo assim", "tipo isso", "como exemplo"
   - "baseado nisso", "inspirado nisto", "nessa linha"
   - "parecido com isso"

   PREFERENCIAL se o usuário disse:
   - "prefiro esse padrão", "tente seguir essa estrutura"
   - "use esse estilo", "quero seguir essa abordagem"

   CONTEXTUAL se o usuário disse:
   - "para você entender", "só para contextualizar"
   - "assim é como funciona hoje"

   SEM SINAL CLARO → assumir ILUSTRATIVO (conservador)
   DÚVIDA MATERIAL sobre normativo vs. ilustrativo → registrar em open-questions.md antes de implementar de forma irreversível

3. VERIFICAR CONFLITO COM GOVERNANÇA EXISTENTE
   - O trecho conflita com contratos ativos?
   - O trecho conflita com regras de segurança do repositório?
   - O trecho conflita com padrões arquiteturais registrados?
   - Se sim: registrar o conflito, não substituir silenciosamente

4. APLICAR CONFORME A CLASSIFICAÇÃO

   NORMATIVO:
   - Copiar na íntegra para o local apropriado
   - Fazer apenas ajustes mínimos e inevitáveis de encaixe estrutural
   - Documentar qualquer adaptação mínima com justificativa
   - Registrar em Instructions/snippets/canonical-snippets.md se relevante para governança futura

   ILUSTRATIVO:
   - Adaptar ao contexto do projeto (arquitetura, padrões, nomenclatura)
   - Preservar a intenção técnica, não a literalidade
   - Reportar como foi adaptado

   PREFERENCIAL:
   - Manter a abordagem e filosofia do trecho
   - Adaptações permitidas desde que a intenção permaneça reconhecível
   - Reportar divergências relevantes

   CONTEXTUAL:
   - Usar apenas como apoio de entendimento
   - Não copiar literalmente
   - Não registrar em snippets canônicos

5. REGISTRAR SNIPPET CANÔNICO (quando normativo e relevante)
   - Registrar em Instructions/snippets/canonical-snippets.md com:
     id, data, título, intenção, resumo da instrução, classificação, escopo,
     regra de preservação, adaptações mínimas permitidas, artefatos relacionados, conteúdo

6. RELATAR
   - Classificação atribuída ao trecho
   - O que foi copiado literalmente (se normativo)
   - O que foi adaptado e como (se ilustrativo ou preferencial)
   - Adaptações mínimas feitas em trecho normativo, com justificativa
   - Conflitos encontrados e como foram tratados
   - Se foi registrado como snippet canônico
```

## Saídas Esperadas
- Trecho aplicado corretamente de acordo com a classificação
- Relatório explícito sobre como o trecho foi tratado
- Registro em snippets canônicos quando aplicável
- Conflitos reportados sem substituição silenciosa

## Arquivos de Governança Relacionados
- `.claude/rules/governance-policies.md` — políticas de classificação de snippets (§5) e ambiguidade (§4)
- `.claude/rules/source-of-truth-priority.md` — hierarquia de prioridade entre fontes
- `Instructions/snippets/README.md`
- `Instructions/snippets/canonical-snippets.md`
- `open-questions.md`
- `assumptions-log.md`

## Nota sobre Invocação
Esta skill é ativada automaticamente quando o assistente identifica um trecho técnico na mensagem do usuário. Não exige comando especial.
