# CLAUDE.md — Sistema de Governança Operacional

Este arquivo é o ponto de entrada do sistema de governança deste repositório.
Toda mensagem do usuário neste repositório é **entrada operacional**. Nenhuma instrução de processo adicional é necessária do usuário.

---

## Comportamento Obrigatório

### 1. Interpretar antes de agir
Toda mensagem deve ser interpretada semanticamente antes de qualquer ação.
Normalize a intenção do usuário. Resolva erros de português, fragmentos e ambiguidades apenas para fins de entendimento. Nunca persista formulação bruta mal escrita.

### 2. Ler a governança relevante antes de implementar
Antes de qualquer implementação, consulte os arquivos de governança pertinentes.
A implementação deve seguir o que está persistido neste repositório.

### 3. Verificar ambiguidades antes de implementar
Se houver dúvida material que comprometa a implementação correta, registre a dúvida antes de agir.
Responda no prompt com as dúvidas encontradas antes de codificar.

### 4. Classificar trechos técnicos enviados pelo usuário
Todo fragmento de código, configuração, schema, YAML, SQL ou artefato técnico enviado pelo usuário deve ser classificado como:
- **Normativo**: copiado na íntegra
- **Ilustrativo**: adaptado ao contexto do projeto
- **Preferencial**: abordagem seguida, mas não literal
- **Contextual**: usado apenas como apoio de entendimento

### 5. Atualizar a governança primeiro
Se a mensagem introduzir ou alterar qualquer definição durável, atualize a governança antes de qualquer mudança de código ou artefato.

### 6. Seguir a prioridade entre fontes de verdade
Contratos normativos e snippets canônicos > BDD > Regras de negócio > Arquitetura > Convenções

### 7. O contexto deste repositório é específico e acumulado
Prefira o histórico e a governança acumulada deste repositório a suposições genéricas.
O comportamento futuro é governado pelos arquivos criados neste bootstrap e evoluídos ao longo do tempo.

### 8. Não depender de repetição de instruções de processo
O usuário não deve precisar dizer: "classifique isso", "consulte as regras", "atualize a governança primeiro", "use BDD", "use contratos", "alinhe com a arquitetura".
Esses comportamentos estão escritos aqui e devem ser executados automaticamente.

---

## Pipeline de Validação Pré-Commit (Obrigatório)

Antes de qualquer commit, executar obrigatoriamente esta sequência:

1. `dotnet build` — verificar compilação sem erros
2. `docker compose up -d` — iniciar aplicação + Datadog Agent em Docker
3. Aguardar `/health` responder HTTP 200 (polling até 30 tentativas)
4. Exibir logs do container da aplicação
5. `docker compose down` — parar todos os containers
6. Somente então realizar o commit

**A aplicação deve ser executada via `docker compose`** para que os logs fluam ao Datadog e o usuário possa visualizá-los em tempo real.

Se Docker daemon não estiver disponível no ambiente (ex: sandbox sem socket):
- Usar `dotnet build` + `dotnet run` para validar build e health check
- Registrar como premissa de ambiente; os logs aparecerão no Datadog quando o CI executar

Se não houver `.env` com `DD_API_KEY` válida, registrar como premissa e prosseguir apenas com build + health check (sem Datadog).

---

## Pipeline de Execução Obrigatório

Para toda mensagem do usuário, siga internamente esta sequência:

1. Interpretar semanticamente a mensagem
2. Normalizar a intenção do usuário
3. Classificar a solicitação
4. Identificar arquivos de governança relevantes
5. Ler a governança relevante antes de agir
6. Verificar ambiguidades, dúvidas materiais ou lacunas
7. Classificar trechos técnicos enviados pelo usuário
8. Registrar dúvidas e premissas quando necessário
9. Atualizar ou remover dúvidas/premissas resolvidas pela nova mensagem
10. Atualizar a governança primeiro quando houver nova definição durável
11. Propagar impactos entre artefatos relacionados
12. Implementar apenas depois que a governança estiver alinhada
13. Relatar: intenção interpretada, arquivos consultados, arquivos alterados, snippets classificados, premissas adotadas, conflitos, dúvidas registradas, dúvidas resolvidas, fonte de verdade ativa

---

## Imports de Governança

@Instructions/operating-model.md
@Instructions/architecture/technical-overview.md
@Instructions/business/business-rules.md
@Instructions/bdd/conventions.md
@Instructions/contracts/README.md
@Instructions/glossary/ubiquitous-language.md
@Instructions/snippets/README.md
@Instructions/snippets/canonical-snippets.md
@Instructions/wiki/wiki-governance.md
@open-questions.md
@assumptions-log.md
@bash-errors-log.md

### Rules operacionais ativas

@.claude/rules/natural-language-normalization.md
@.claude/rules/technical-ingestion.md
@.claude/rules/business-ingestion.md
@.claude/rules/source-of-truth-priority.md
@.claude/rules/implementation-alignment.md
@.claude/rules/ambiguity-handling.md
@.claude/rules/snippet-handling.md
@.claude/rules/architecture-governance.md
@.claude/rules/naming-governance.md
@.claude/rules/folder-governance.md
@.claude/rules/change-propagation.md
@.claude/rules/repository-context-evolution.md
@.claude/rules/bash-error-logging.md

---

## Escopo de Aplicação

Este sistema de governança serve para repositórios de:
- Código de aplicação
- Infraestrutura como código
- Mensageria e contratos de eventos (SNS, SQS, tópicos, filas)
- Definições de banco como código
- Schemas, payloads e artefatos operacionais ou declarativos

"Implementar" significa materializar mudanças em código, infraestrutura declarativa, contratos, mensageria, banco, configuração ou documentação operacional.

"Contratos" incluem APIs HTTP, contratos de mensagens, schemas e interfaces operacionais.
