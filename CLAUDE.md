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

### 9. Avaliar eficiência em toda tarefa (instrução permanente)
Em toda tarefa, antes de iniciar qualquer sequência de operações, avaliar ativamente:
- Existe artefato já gerado que pode ser reutilizado? (imagem Docker, resultado de build, `.env` válido)
- Existe etapa que pode ser antecipada para evitar falha custosa posterior?
- Existe etapa redundante que pode ser eliminada sem comprometer o resultado?
- Existe abordagem mais rápida e reversível que produz o mesmo resultado?

Aplicar a otimização quando a resposta for sim. Registrar quando a otimização não for possível e por quê.
Ver detalhamento completo em `.claude/rules/environment-readiness.md`.

---

## Pipeline de Validação Pré-Commit (Obrigatório)

Antes de qualquer commit, executar obrigatoriamente esta sequência:

0. Verificar pré-requisitos de ambiente (checklist em `.claude/rules/environment-readiness.md`). O ambiente deve estar pronto — se não estiver, seguir o protocolo de ambiente não pronto antes de prosseguir.
1. `dotnet build` — verificar compilação em modo Debug sem erros
2. `dotnet run` (modo debug) — iniciar a aplicação localmente, aguardar `/health` responder (qualquer código HTTP confirma inicialização), encerrar o processo. Primeira validação em modo debug antes de executar os testes.
3. `dotnet test` — executar todos os testes em modo debug. **Gate obrigatório**: falha em qualquer teste bloqueia o avanço para os passos seguintes. Somente se todos os testes passarem, prosseguir.
4. `docker compose up -d` — publicar (Release/Native AOT) e iniciar aplicação + Datadog Agent em Docker. Executado somente após aprovação no gate de testes (passo 3).
5. Aguardar `/health` responder HTTP 200 (polling até 30 tentativas)
6. Se a tarefa criou ou alterou features com endpoint: validar cada endpoint via chamada HTTP real (ver `.claude/rules/endpoint-validation.md`). Se o endpoint exigir autenticação, obter Bearer Token via `POST /login` antes de consumir. Status code inesperado bloqueia o commit.
7. Exibir logs do container da aplicação
8. `docker compose down` — parar todos os containers
9. Somente então realizar o commit

**O Passo 0 é obrigatório e não deve ser pulado.** Previne o ciclo de falhas em cascata documentado em `bash-errors-log.md`. Ver `.claude/rules/environment-readiness.md` para o protocolo completo.

**O Passo 3 é um gate obrigatório.** O `docker compose up -d` (publish Release/AOT) só deve ser executado após todos os testes passarem em modo debug. Testes falhando bloqueiam o commit — corrigir antes de avançar.

**`scripts/setup-env.sh` é um modelo declarativo** copiado manualmente pelo usuário em ferramenta externa de configuração de container. O agente não executa esse script — o ambiente deve chegar já pronto. Se um pré-requisito estiver ausente, o agente atualiza o script e sinaliza ao usuário para sincronizar a ferramenta externa.

**A aplicação deve ser executada via `docker compose`** para que os logs fluam ao Datadog e o usuário possa visualizá-los em tempo real. A execução em modo debug (passo 2) é uma validação intermediária local, não substitui a execução via Docker.

Se `DD_API_KEY` não estiver disponível no host, o pipeline prosseguirá sem Datadog — os logs aparecerão quando o CI executar com a chave configurada.

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
@.claude/rules/environment-readiness.md
@.claude/rules/endpoint-validation.md
@.claude/rules/pr-metadata-governance.md

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
