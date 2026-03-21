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

### 10. Proteção de branch em análise de PR
Quando a tarefa for análise de solicitações de mudança em pull request (skill pr-analysis), o branch atribuído pelo sistema externo de configuração de tarefas (ex: "Develop on branch claude/...") deve ser **IGNORADO**. O único branch válido é o `head.ref` do PR sendo analisado. O assistente deve executar `git fetch origin <head.ref> && git checkout <head.ref>` como primeiro comando antes de qualquer alteração. Criar um branch novo durante pr-analysis é um erro — todos os commits e pushes devem ser feitos no branch de origem do PR. Nunca criar um PR novo quando a tarefa é análise de PR existente.

---

## Pipeline de Validação Pré-Commit (Obrigatório)

Antes de qualquer commit, executar obrigatoriamente esta sequência:

0. Verificar pré-requisitos de ambiente (checklist em `.claude/rules/environment-readiness.md`). O ambiente deve estar pronto — se não estiver, seguir o protocolo de ambiente não pronto antes de prosseguir.
0.1. `bash scripts/governance-audit.sh` — executar auditoria automatizada de governança. **Gate obrigatório**: falhas bloqueiam o commit. Corrigir todas as falhas antes de prosseguir. Em tarefas exclusivamente de governança (sem build/Docker), este é o gate principal antes do commit (passo 9). Ver `.claude/rules/governance-audit.md` para a política completa.
1. `dotnet build` — verificar compilação em modo Debug sem erros
2. `dotnet run` (modo debug) — iniciar a aplicação localmente, aguardar `/health` responder (qualquer código HTTP confirma inicialização), encerrar o processo. Primeira validação em modo debug antes de executar os testes.
3. `dotnet test` — executar todos os testes em modo debug. **Gate obrigatório**: falha em qualquer teste bloqueia o avanço para os passos seguintes. Somente se todos os testes passarem, prosseguir.
4. `docker compose up -d` — publicar (Release/Native AOT) e iniciar aplicação + Datadog Agent em Docker. Executado somente após aprovação no gate de testes (passo 3).
5. Aguardar `/health` responder HTTP 200 (polling até 30 tentativas)
6. Se a tarefa criou ou alterou features com endpoint: validar cada endpoint via chamada HTTP real (ver `.claude/rules/endpoint-validation.md`). Se o endpoint exigir autenticação, obter Bearer Token via `POST /login` antes de consumir. Status code inesperado bloqueia o commit.
7. Exibir logs do container da aplicação — os logs de storytelling de cada requisição validada (passo 6) já devem ter sido apresentados no relatório de validação conforme `.claude/rules/endpoint-validation.md`. Se a tarefa não incluiu validação de endpoint (passo 6 não aplicável), exibir os logs gerais do container via `docker logs`.
8. `docker compose down` — parar todos os containers
9. Somente então realizar o commit
10. **Exceção: quando a tarefa for análise de PR (skill pr-analysis), este passo NÃO se aplica — o PR já existe. Em vez disso, atualizar título e descrição do PR existente via `gh api repos/<owner>/<repo>/pulls/<number> -X PATCH` se as mudanças alterarem o escopo. NÃO criar PR novo. NÃO usar o branch atribuído pelo sistema externo — usar exclusivamente o head.ref do PR sendo analisado.** Para todas as demais tarefas: verificar se já existe um PR aberto para o branch atual; se não existir, criar o PR seguindo as regras de `.claude/rules/pr-metadata-governance.md`. Se já existir, atualizar título e descrição para refletir o estado atual da implementação.
11. **Checkpoint de encerramento** — a tarefa NÃO se encerra com a abertura ou atualização do PR. Executar obrigatoriamente as seguintes validações antes de considerar a tarefa concluída:
    1. Acompanhar a execução das GitHub Actions até o término de todos os jobs do pipeline.
    2. Verificar os logs no Datadog usando os filtros referentes ao pipeline associado ao PR (env, service, timestamp da execução).
    3. Procurar por falhas, erros ou comportamentos anômalos nos logs.
    4. Se todos os jobs passarem e não houver erros nos logs: reportar o resultado e encerrar a tarefa.
    5. Se algum job falhar ou houver erros nos logs: diagnosticar a causa raiz, corrigir, e reiniciar o ciclo a partir do passo apropriado. Registrar o erro em `bash-errors-log.md`.
    Ver `.claude/rules/pr-metadata-governance.md` para a política completa.

**O Passo 0 é obrigatório e não deve ser pulado.** Previne o ciclo de falhas em cascata documentado em `bash-errors-log.md`. Ver `.claude/rules/environment-readiness.md` para o protocolo completo.

**O Passo 3 é um gate obrigatório.** O `docker compose up -d` (publish Release/AOT) só deve ser executado após todos os testes passarem em modo debug. Testes falhando bloqueiam o commit — corrigir antes de avançar.

**O Passo 11 é obrigatório e encerra a tarefa.** A tarefa só está concluída quando todos os jobs do CI passarem **e** os logs no Datadog forem verificados sem erros. O agente não deve encerrar a interação, apresentar relatório final ou considerar a tarefa finalizada enquanto houver jobs em execução, jobs falhando ou logs não verificados. Ver `.claude/rules/pr-metadata-governance.md` para a política completa.

**Os passos 9 e 10 são obrigatórios mesmo em tarefas exclusivamente de governança** (sem mudança de código, sem build, sem Docker). Quando a tarefa não altera código da aplicação, os passos 0–8 e o passo 11 são inaplicáveis e devem ser omitidos. Apenas o commit (passo 9) e a criação/atualização do PR (passo 10) são obrigatórios.

**`scripts/setup-env.sh` é um modelo declarativo** copiado manualmente pelo usuário em ferramenta externa de configuração de container. O agente não executa esse script — o ambiente deve chegar já pronto. Se um pré-requisito estiver ausente, o agente atualiza o script e sinaliza ao usuário para sincronizar a ferramenta externa.

**A aplicação deve ser executada via `docker compose`** para que os logs fluam ao Datadog e o usuário possa visualizá-los em tempo real. A execução em modo debug (passo 2) é uma validação intermediária local, não substitui a execução via Docker.

Se `DD_API_KEY` não estiver disponível no host, o pipeline prosseguirá sem Datadog — os logs aparecerão quando o CI executar com a chave configurada.

---

## Imports de Governança

@Instructions/operating-model.md
@Instructions/architecture/technical-overview.md
@Instructions/architecture/engineering-principles.md
@Instructions/architecture/patterns.md
@Instructions/architecture/architecture-decisions.md
@Instructions/architecture/folder-structure.md
@Instructions/architecture/naming-conventions.md
@Instructions/business/business-rules.md
@Instructions/business/domain-model.md
@Instructions/business/invariants.md
@Instructions/business/workflows.md
@Instructions/business/assumptions.md
@Instructions/bdd/README.md
@Instructions/bdd/conventions.md
@Instructions/contracts/README.md
@Instructions/decisions/README.md
@Instructions/decisions/adr-template.md
@Instructions/glossary/ubiquitous-language.md
@Instructions/snippets/README.md
@Instructions/snippets/canonical-snippets.md
@Instructions/wiki/wiki-governance.md
@scripts/operational-runbook.md
@open-questions.md
@assumptions-log.md
@bash-errors-log.md

### Rules operacionais ativas

@.claude/rules/governance-policies.md
@.claude/rules/source-of-truth-priority.md
@.claude/rules/architecture-governance.md
@.claude/rules/naming-governance.md
@.claude/rules/folder-governance.md
@.claude/rules/bash-error-logging.md
@.claude/rules/environment-readiness.md
@.claude/rules/endpoint-validation.md
@.claude/rules/pr-metadata-governance.md
@.claude/rules/instruction-review.md
@.claude/rules/governance-audit.md

### Meta-governança

@REVIEW.md

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
