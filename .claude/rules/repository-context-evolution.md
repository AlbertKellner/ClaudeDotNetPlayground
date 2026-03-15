# Regra: Evolução do Contexto do Repositório

## Propósito

Esta rule define como o contexto específico deste repositório se acumula, é preservado e deve ser priorizado em relação a suposições genéricas do assistente.

---

## Princípio Fundamental

> O bootstrap foi genérico apenas no momento da inicialização.
> A partir da primeira interação real, este repositório se torna um contexto específico.
> O assistente deve operar com base no histórico acumulado deste repositório,
> não com base em suposições genéricas sobre stacks, domínios ou arquiteturas.

---

## O Que Constitui o Contexto Específico do Repositório

O contexto específico é formado pela governança acumulada em:
- `Instructions/architecture/` — decisões, princípios e padrões técnicos reais do projeto
- `Instructions/business/` — regras de negócio, invariantes e modelo de domínio reais
- `Instructions/bdd/` — cenários comportamentais reais do sistema
- `Instructions/contracts/` — contratos reais de interface
- `Instructions/glossary/` — terminologia real do domínio
- `Instructions/decisions/` — decisões reais tomadas ao longo do tempo
- `Instructions/snippets/` — fragmentos canônicos reais declarados pelo usuário
- `assumptions-log.md` — premissas reais adotadas
- `open-questions.md` — dúvidas reais pendentes

---

## Como o Contexto Evolui

### A cada interação real do usuário:
1. Novas definições são adicionadas à governança
2. Definições existentes são refinadas
3. Dúvidas são abertas ou resolvidas
4. Premissas são confirmadas, invalidadas ou criadas
5. Decisões são registradas
6. Snippets canônicos são registrados
7. O vocabulário do domínio cresce

### Critério de evolução:
- Apenas definições **duráveis** devem ser persistidas (ver regra de conhecimento durável)
- Detalhes incidentais de implementação não devem ser promovidos à governança
- O repositório deve ser mais específico e mais rico a cada interação, não mais genérico

---

## Como o Contexto Deve Ser Utilizado

### O assistente deve:
1. **Preferir sempre** a governança acumulada deste repositório a suposições genéricas
2. **Consultar** os arquivos relevantes antes de qualquer implementação
3. **Inferir** a stack, domínio e padrões com base no que já está registrado
4. **Não assumir** stacks, frameworks, arquiteturas ou domínios que não estejam registrados
5. **Usar** a terminologia do glossário em todo conteúdo persistido
6. **Respeitar** as decisões registradas em ADRs

### O usuário não deve precisar:
- Reapresentar a arquitetura do projeto a cada sessão
- Reexplicar regras de negócio que já estão registradas
- Reforçar convenções que já estão na governança
- Lembrar o assistente de processos que já estão nas rules
- Repetir preferências que já foram registradas como decisões

---

## Política de Conhecimento Durável

### O que DEVE ser persistido:
- Regras que devem guiar comportamento futuro
- Decisões que não devem ser refeitas sem reflexão
- Restrições que devem ser respeitadas sistematicamente
- Terminologia que deve ser consistente
- Padrões que devem ser seguidos
- Snippets que devem ser preservados literalmente
- Premissas que afetam implementações presentes ou futuras

### O que NÃO deve ser persistido:
- Detalhes transitórios de implementação sem impacto futuro
- Escolhas incidentais de variáveis locais
- Comentários de progresso de desenvolvimento
- Configurações temporárias específicas de ambiente de desenvolvimento
- Logs brutos de atividade do assistente

---

## Comportamento em Repositório Novo vs. Repositório Maduro

### Repositório recém-inicializado (bootstrap recente):
- Contexto ainda genérico — poucas definições específicas acumuladas
- Ambiguidades são esperadas — mais dependência de premissas conservadoras
- O assistente deve registrar mais premissas nesta fase
- O assistente deve perguntar mais sobre escolhas fundamentais (stack, arquitetura, domínio)

### Repositório com histórico acumulado:
- Contexto específico rico — preferir a governança acumulada
- Menos premissas necessárias — mais inferências a partir do histórico
- O assistente deve ser mais assertivo baseado no contexto existente
- Novas dúvidas devem ser avaliadas no contexto das definições já existentes

---

## Regras de Não Regressão de Contexto

- Decisões registradas não devem ser desfeitas sem instrução explícita do usuário
- Snippets canônicos registrados não devem ser removidos sem instrução explícita
- Definições de domínio registradas não devem ser substituídas silenciosamente
- Convenções registradas não devem ser ignoradas silenciosamente
- O assistente deve agir como guardião do contexto acumulado, não como executor de cada mensagem de forma isolada

---

## Relação com Outras Rules

- Todas as outras rules dependem desta — o contexto do repositório informa como cada rule se aplica
- `implementation-alignment.md` — o contexto é consultado no passo 3 e 5
- `ambiguity-handling.md` — o contexto existente é a primeira fonte para resolver ambiguidades
- `source-of-truth-priority.md` — a hierarquia de prioridade é aplicada ao contexto acumulado
