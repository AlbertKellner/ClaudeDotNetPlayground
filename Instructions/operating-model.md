# Modelo Operacional do Repositório

## Propósito

Este documento consolida o comportamento de runtime do assistente neste repositório.
É a referência central que descreve como todas as interações em linguagem natural são processadas, interpretadas e executadas.

---

## Como Este Repositório Funciona

Este repositório opera com um sistema de governança persistente. Todo conteúdo de governança acumulado nos arquivos de `Instructions/`, `.claude/rules/` e `.claude/skills/` constitui o contexto operacional permanente.

**O usuário não precisa reexplicar processo.** O processo está escrito aqui e nas rules.
**O usuário não precisa reexplicar contexto acumulado.** O contexto está nos arquivos de governança.

---

## Como Mensagens São Interpretadas

Toda mensagem do usuário é tratada como entrada operacional suficiente, independentemente de:
- Ser formal ou informal
- Ser completa ou fragmentada
- Conter erros de português
- Ter sido ditada
- Ser uma solicitação imperativa curta

**Sequência de interpretação obrigatória**:
1. Ler a mensagem completa
2. Identificar o núcleo da intenção
3. Identificar elementos secundários (contexto, restrições, snippets)
4. Resolver ambiguidades lexicais com o glossário do repositório
5. Reconstruir a intenção em linguagem técnica clara
6. Verificar compatibilidade com a governança existente

---

## Como Tipos de Mensagem São Classificados

| Tipo | Descrição | Skill Ativada |
|---|---|---|
| Nova definição durável | Introduz ou altera regra, princípio, invariante, convenção, decisão | `ingest-definition` |
| Solicitação de implementação | Cria, altera, remove ou revisa artefato do repositório | `implement-request` |
| Revisão de alinhamento | Verifica consistência entre artefatos | `review-alignment` |
| Evolução de governança | Reorganiza, consolida ou melhora a base de governança | `evolve-governance` |
| Resolução de ambiguidade | Responde a dúvida registrada ou esclarece lacuna | `resolve-ambiguity` |
| Fornecimento de trecho técnico | Entrega código, configuração, schema ou artefato para aplicar | `apply-user-snippet` |

Uma mensagem pode ativar múltiplos tipos simultaneamente.

---

## O Que Conta Como Nova Definição Durável

Uma definição é durável quando deve continuar guiando trabalho futuro. Inclui:

- Nova regra de negócio ou alteração de regra existente
- Nova restrição ou nova exceção
- Novo fluxo ou conceito de domínio
- Novo invariante ou alteração de invariante
- Novo contrato ou alteração de contrato
- Novo cenário BDD
- Mudança no comportamento esperado do sistema
- Nova convenção de nomenclatura
- Nova regra de organização de pastas
- Nova regra arquitetural ou princípio técnico
- Nova decisão tecnológica relevante
- Ambiguidade resolvida que deva persistir como conhecimento
- Snippet canônico que deva governar futuras implementações

---

## Por Que a Governança É Atualizada Antes da Implementação

A implementação é a materialização de uma intenção. Se a intenção não está registrada antes da implementação, o repositório fica sem memória de por que a implementação foi feita. Isso leva a:
- Inconsistência entre código e regras de negócio
- Dificuldade em refatorar corretamente no futuro
- Perda de contexto quando novas interações chegarem

**A governança é a memória. A implementação é a expressão da memória.**

Se a governança já cobre a solicitação, implementar diretamente é correto — mas ainda é necessário validar que a implementação permanece alinhada.

---

## Como o Conhecimento Durável É Selecionado

### Deve ser persistido:
- Regras que devem guiar comportamento futuro
- Decisões que não devem ser refeitas sem reflexão
- Restrições que devem ser respeitadas sistematicamente
- Terminologia que deve ser consistente
- Padrões que devem ser seguidos
- Snippets que devem ser preservados literalmente

### Não deve ser persistido:
- Detalhes transitórios de implementação
- Escolhas incidentais de variáveis locais sem impacto futuro
- Logs de atividade do assistente
- Configurações temporárias de ambiente de desenvolvimento

---

## Como a Propagação entre Artefatos Funciona

Mudança em qualquer artefato pode afetar outros. A propagação segue o mapa:

```
Negócio muda     → avaliar BDD, contratos, glossário, implementação
Contratos mudam  → avaliar negócio, BDD, glossário, implementação
BDD muda         → avaliar negócio, contratos, implementação
Arquitetura muda → avaliar instruções técnicas, organização, implementação
Nomenclatura muda→ avaliar todos os artefatos relacionados
Snippet canônico muda → avaliar implementações relacionadas
```

O assistente é responsável por essa propagação — o usuário não precisa listá-la.

---

## Como a Prioridade entre Fontes Funciona

```
1. Contratos executáveis, artefatos formais e snippets normativos canônicos
2. BDD (cenários comportamentais formalizados)
3. Regras de negócio estruturadas
4. Arquitetura e padrões técnicos
5. Convenções de nomenclatura, estilo e organização
```

Conflitos são sempre reportados. A fonte de maior prioridade prevalece.
Preferência arquitetural **nunca** pode invalidar comportamento de negócio exigido.

---

## Como Ambiguidades São Tratadas

### Dúvida pequena (não bloqueante)
Não altera comportamento, arquitetura, contrato, segurança ou experiência do usuário.
**Tratamento**: premissa mínima conservadora + registro em `assumptions-log.md`.

### Dúvida material (potencialmente bloqueante)
Afeta comportamento funcional, regra de negócio, contratos, BDD, arquitetura, segurança, integração, persistência ou modelagem.
**Tratamento**: registrar em `open-questions.md` + reportar no prompt + aguardar confirmação.

### Quando parte da solicitação for segura e parte ambígua:
- Executar o que for seguro
- Deixar a divisão explícita
- Registrar o que está bloqueado
- Aguardar confirmação

---

## Como Dúvidas São Registradas

`open-questions.md` contém **apenas** dúvidas ainda abertas.
Cada dúvida deve ter: id, data, resumo, dúvida, impacto, artefatos afetados, se é bloqueante, status.

**Quando o usuário resolve a dúvida**:
1. Remover da lista ativa em `open-questions.md`
2. Consolidar a resolução nos arquivos definitivos
3. Atualizar premissas relacionadas em `assumptions-log.md`

---

## Como Premissas São Registradas

`assumptions-log.md` contém **apenas** premissas ainda ativas.
Cada premissa deve ter: id, data, premissa, motivo, escopo, artefatos impactados, nível de risco, se precisa de confirmação.

**Quando premissa for confirmada ou invalidada**:
- Confirmar: consolidar na governança definitiva, remover do log ativo
- Invalidar: remover do log ativo, não permanecer como regra

---

## Como Snippets São Classificados

Todo trecho técnico do usuário é classificado como:

| Classificação | Sinal | Tratamento |
|---|---|---|
| Normativo | "inclua exatamente", "copie isso", "preserve" | Copiar na íntegra |
| Ilustrativo | "algo assim", "tipo isso", "como exemplo" | Adaptar ao contexto |
| Preferencial | "prefiro esse padrão", "use esse estilo" | Seguir a abordagem |
| Contextual | "só para contextualizar", "para entender" | Apenas apoio |

**Padrão conservador**: na ausência de sinal claro, assumir ilustrativo.

---

## Quando a Implementação Deve Aguardar Esclarecimento

A implementação deve aguardar quando:
- Há dúvida material sobre comportamento esperado
- Há conflito entre fontes de verdade que não pode ser resolvido pela hierarquia
- Um snippet normativo conflita com a estrutura existente
- A mudança afetaria contratos com dependentes externos não mapeados

A implementação pode prosseguir quando:
- A governança existente cobre completamente a solicitação
- A ambiguidade é pequena e resolve com premissa conservadora
- A parte segura da solicitação pode ser executada independentemente da parte ambígua

---

## Quando um Trecho Deve Ser Copiado Literalmente

Copiar literalmente quando:
- O usuário usou linguagem de literalidade explícita
- O trecho foi classificado como normativo

**Nunca** copiar literalmente por padrão sem classificação consciente.

---

## Quando um Trecho Pode Ser Adaptado

Adaptar quando:
- O trecho foi classificado como ilustrativo ou preferencial
- A adaptação preserva a intenção técnica
- A adaptação alinha o trecho com arquitetura, nomenclatura e padrões do repositório

---

## Como Solicitações Curtas São Tratadas

Mensagens curtas e imperativas (ex: "crie um endpoint de login", "exclua por id") são tratadas assim:

1. Consultar o contexto do repositório (regras de negócio, arquitetura, convenções)
2. Inferir o comportamento esperado com base no contexto
3. Adotar premissas conservadoras para o que não estiver definido
4. Registrar premissas relevantes
5. Executar sem pedir esclarecimentos desnecessários quando a governança for suficiente
6. Registrar dúvidas quando a solicitação curta não for suficiente para implementação segura

---

## Como o Contexto Específico do Repositório Evolui

O bootstrap foi genérico. A partir da primeira interação real, o repositório acumula:
- Regras de negócio reais
- Decisões arquiteturais reais
- Terminologia real do domínio
- Padrões reais adotados pela equipe
- Histórico de decisões e suas motivações

A cada interação, o repositório se torna mais específico e o assistente opera com mais precisão e menos dependência de premissas.

**O contexto não regride.** Decisões registradas persistem. O assistente age como guardião do contexto acumulado.
