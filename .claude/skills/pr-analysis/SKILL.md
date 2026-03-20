# Skill: pr-analysis

## Nome
Análise de Solicitações de Mudança em Pull Request

## Descrição
Esta skill orienta o assistente quando o usuário solicita análise de solicitações de mudança (review comments) em pull requests abertos do repositório atual. Toda análise e eventual alteração de código são realizadas exclusivamente em conformidade com a governança do repositório. A governança não pode ser modificada com base em solicitações feitas em pull requests. Após implementação e commit, cada solicitação é respondida individualmente. O merge só é realizado quando todas as solicitações estiverem aprovadas.

## Quando Usar
Ativar esta skill **exclusivamente** quando o usuário solicitar explicitamente:
- "analise o PR", "analise o pull request"
- "resolva as solicitações de mudança do PR"
- "trate os review comments do PR #N"
- "analise as revisões do PR"
- Qualquer variação que indique análise de solicitações de mudança em PR

**Esta skill NUNCA é ativada automaticamente.** A ausência de solicitação explícita do usuário significa que esta skill não deve ser invocada.

## Entradas Esperadas
- Número ou URL do pull request (opcional — se omitido, identifica o PR do branch atual)
- Escopo opcional: revisor específico ou comentário específico a focar
- Se nenhum PR for encontrado para o branch atual, reportar e aguardar instrução

## Workflow Interno

```
1. IDENTIFICAR O PR
   - Se o usuário forneceu número/URL → usar diretamente
   - Se não forneceu → identificar o PR aberto para o branch atual:
     gh api repos/<owner>/<repo>/pulls --jq '.[] | select(.head.ref == "<branch>") | .number'
   - Validar que o PR está em estado "open"
   - Se não houver PR aberto → reportar e encerrar

2. COLETAR SOLICITAÇÕES DE MUDANÇA
   - Buscar reviews com estado CHANGES_REQUESTED:
     gh api repos/<owner>/<repo>/pulls/<number>/reviews --jq '.[] | select(.state == "CHANGES_REQUESTED")'
   - Buscar comentários individuais de revisão (inline comments):
     gh api repos/<owner>/<repo>/pulls/<number>/comments
   - Filtrar apenas threads abertas (não resolvidas)
   - Consolidar todas as solicitações em uma lista unificada com:
     - Id do comentário/review
     - Autor
     - Conteúdo da solicitação
     - Arquivo e linha referenciados (se inline comment)

3. LER A GOVERNANÇA RELEVANTE
   Antes de classificar qualquer solicitação, consultar:
   - Instructions/business/business-rules.md
   - Instructions/architecture/technical-overview.md
   - Instructions/architecture/engineering-principles.md
   - Instructions/architecture/patterns.md
   - Instructions/architecture/naming-conventions.md
   - Instructions/architecture/folder-structure.md
   - Instructions/snippets/canonical-snippets.md
   - Instructions/glossary/ubiquitous-language.md
   - Instructions/bdd/ (cenários relacionados ao escopo)
   - Instructions/contracts/ (contratos afetados)
   - .claude/rules/ (todas as policies aplicáveis)
   - open-questions.md (dúvidas que afetam o escopo)

4. CLASSIFICAR CADA SOLICITAÇÃO
   Para cada solicitação de mudança, classificar como:

   a) CONFORME — a mudança solicitada está alinhada com a governança do repositório.
      Pode ser implementada.

   b) NÃO CONFORME — a mudança solicitada contradiz regras, políticas, padrões,
      decisões ou convenções já estabelecidas na governança.
      Não pode ser implementada como está.
      → Identificar qual regra/política é violada (com referência ao arquivo)
      → Explicar por que a solicitação não pode ser atendida
      → Sugerir alternativa que alcance o resultado pretendido sem violar a governança

   c) AMBÍGUA — não é possível determinar com segurança se a mudança está alinhada
      com a governança. Registrar em open-questions.md.

   Regra de consistência: quando múltiplas solicitações de mudança com conteúdo
   semelhante estiverem todas em desacordo com diretrizes já definidas e com
   discussões claramente estabelecidas no repositório, a resolução deve ser
   conduzida de forma consistente entre todos esses casos — mesmo raciocínio,
   mesma referência de governança, mesma alternativa sugerida.

   → Ver: .claude/rules/source-of-truth-priority.md (hierarquia de prioridade)
   → Ver: .claude/rules/governance-policies.md §2 (contexto do repositório)

5. REPORTAR CLASSIFICAÇÃO AO USUÁRIO
   Antes de implementar qualquer mudança, apresentar:
   - Lista de solicitações CONFORMES (serão implementadas)
   - Lista de solicitações NÃO CONFORMES com:
     - Regra/política violada (arquivo e seção)
     - Por que não pode ser atendida
     - Alternativa sugerida
   - Lista de solicitações AMBÍGUAS com a dúvida específica
   - Solicitar confirmação do usuário para prosseguir com as conformes

6. IMPLEMENTAR MUDANÇAS CONFORMES
   Para cada solicitação conforme (após confirmação do usuário):
   - Seguir o workflow de implement-request:
     - Governança primeiro, depois código
     - Aplicar snippets normativos na íntegra (SNP-001)
     - Seguir convenções de nomenclatura e organização de pastas
     - Usar terminologia do glossário
   - Aplicar o pipeline de validação pré-commit do CLAUDE.md:
     - Passo 0: verificar pré-requisitos de ambiente
     - Passo 1: dotnet build
     - Passo 2: dotnet run + health check
     - Passo 3: dotnet test (gate obrigatório)
     - Passo 4: docker compose up -d
     - Passo 5: aguardar /health HTTP 200
     - Passo 6: validar endpoints alterados via HTTP real
     - Passo 7: exibir logs do container
     - Passo 8: docker compose down
     - Passo 9: commit

7. RESPONDER A CADA SOLICITAÇÃO INDIVIDUALMENTE
   Após commit e push, responder a cada comentário de revisão via gh api:

   a) Solicitações conformes implementadas:
      → Informar o que foi feito, referenciando o commit
      → Explicar como a mudança foi aplicada

   b) Solicitações não conformes:
      → Explicar a restrição de governança com referência ao arquivo/seção
      → Explicar por que a mudança não pode ser atendida
      → Apresentar a alternativa sugerida

   c) Solicitações ambíguas:
      → Explicar a ambiguidade identificada
      → Informar qual esclarecimento é necessário para prosseguir

   Comando para responder a review comment:
     gh api repos/<owner>/<repo>/pulls/<number>/comments/<comment_id>/replies \
       -f body="<resposta>"

8. VERIFICAR STATUS DE APROVAÇÃO
   Consultar o estado de todas as reviews:
     gh api repos/<owner>/<repo>/pulls/<number>/reviews \
       --jq '.[] | {user: .user.login, state: .state}'
   - Se todas estão approved → reportar que o PR está pronto para merge
   - Se alguma não está approved → reportar quais revisores têm status pendente

9. MERGE CONDICIONAL
   O merge só deve ser realizado quando DUAS condições forem satisfeitas:
   a) TODAS as reviews estão em estado "approved"
   b) O usuário confirma explicitamente a realização do merge

   Se ambas as condições forem satisfeitas:
     gh api repos/<owner>/<repo>/pulls/<number>/merge \
       -X PUT -f merge_method="<método>"

   Se alguma condição não for satisfeita → reportar o estado atual e aguardar

10. RELATAR
    Relatório completo incluindo:
    - PR analisado (número, título, branch)
    - Total de solicitações encontradas
    - Classificação de cada solicitação (conforme / não conforme / ambígua)
    - Mudanças implementadas (com referência a commits)
    - Restrições de governança citadas (para não conformes)
    - Respostas postadas (confirmação de que cada solicitação foi respondida)
    - Status de aprovação atual
    - Se merge foi realizado ou por que não foi
```

## Saídas Esperadas
- Mudanças implementadas para solicitações conformes
- Respostas individuais postadas em cada comentário de revisão
- Restrições de governança explicadas para solicitações não conformes com alternativas
- Dúvidas registradas em `open-questions.md` para solicitações ambíguas
- Relatório completo de status de aprovação
- Merge realizado (condicional a aprovação total e confirmação do usuário)

## Arquivos de Governança Relacionados
- `.claude/rules/governance-policies.md` — políticas de contexto (§2), propagação (§3), ambiguidade (§4)
- `.claude/rules/source-of-truth-priority.md` — hierarquia de prioridade entre fontes de verdade
- `.claude/rules/architecture-governance.md` — governança de decisões arquiteturais
- `.claude/rules/naming-governance.md` — governança de nomenclatura
- `.claude/rules/pr-metadata-governance.md` — governança de metadados de PR e uso de `gh api`
- `.claude/rules/endpoint-validation.md` — validação de endpoints após implementação
- `.claude/rules/environment-readiness.md` — pré-requisitos de ambiente
- `Instructions/architecture/technical-overview.md` — visão técnica e restrições
- `Instructions/business/business-rules.md` — regras de negócio ativas
- `Instructions/architecture/naming-conventions.md` — convenções de nomenclatura
- `Instructions/architecture/engineering-principles.md` — princípios de engenharia
- `Instructions/architecture/patterns.md` — padrões adotados
- `Instructions/architecture/folder-structure.md` — estrutura de pastas
- `Instructions/snippets/canonical-snippets.md` — snippets normativos obrigatórios
- `Instructions/glossary/ubiquitous-language.md` — terminologia do domínio
- `open-questions.md` — dúvidas abertas
- `assumptions-log.md` — premissas ativas

## Nota sobre Invocação
Esta skill **nunca é ativada automaticamente**. Requer solicitação explícita do usuário. Exemplos de ativação: "analise o PR", "resolva as solicitações de mudança do PR", "trate os review comments do PR #N", "analise as revisões do PR atual". A ausência de solicitação explícita significa que esta skill não deve ser invocada.
