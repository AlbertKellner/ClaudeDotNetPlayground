# Regra: Políticas de Governança Consolidadas

## Propósito

Esta regra consolida as políticas (o quê, não como) de normalização de linguagem, contexto do repositório, propagação de mudanças, tratamento de ambiguidades e classificação de snippets. Os workflows procedurais correspondentes residem nas skills.

---

## 1. Política de Normalização de Linguagem Natural

### Obrigatório:
- Interpretar semanticamente toda mensagem antes de agir
- Reconstruir a intenção quando fragmentada, ditada ou ambígua
- Resolver erros ortográficos silenciosamente — apenas para entendimento interno
- Normalizar para linguagem técnica limpa antes de persistir em governança
- Consultar o glossário do repositório antes de assumir interpretação
- Preservar a intenção, não a formulação bruta

### Proibido:
- Exigir que o usuário reescreva mensagens informais
- Copiar erros brutos para arquivos de governança
- Inventar regras, restrições ou comportamentos ausentes para preencher lacunas

---

## 2. Política de Contexto do Repositório

### Princípio:
> O contexto acumulado deste repositório prevalece sobre suposições genéricas. O contexto não regride. Decisões registradas persistem.

### Obrigatório:
- Preferir sempre a governança acumulada a suposições genéricas
- Consultar arquivos relevantes antes de implementar
- Não assumir stacks, frameworks ou domínios não registrados
- Usar a terminologia do glossário em todo conteúdo persistido
- Respeitar decisões registradas em ADRs

### O que persiste (conhecimento durável):
- Regras, decisões, restrições, terminologia, padrões, snippets canônicos, ferramentas e recursos MCP disponíveis

### O que não persiste:
- Detalhes transitórios, variáveis locais incidentais, logs de atividade, configurações temporárias

---

## 3. Política de Propagação de Mudanças

### Princípio:
> Nenhuma mudança existe isolada. Consistência entre artefatos é responsabilidade do assistente.

### Mapa de propagação:
| Se muda... | Avaliar impacto em... |
|---|---|
| Negócio (regras, invariantes, fluxos) | BDD, contratos, glossário, implementação |
| Contratos (OpenAPI, schemas, payloads) | Negócio, BDD, glossário, implementação |
| BDD (cenários) | Negócio, contratos, implementação |
| Arquitetura (princípios, padrões) | Technical-overview, folder-structure, naming-conventions, implementação |
| Nomenclatura | Glossário, BDD, contratos, código, documentação |
| Snippet canônico | Implementações que usam o snippet |
| Ferramentas operacionais (MCP, tokens, integrações) | technical-overview, environment-readiness, required-vars, container-setup, pipeline pré-commit |

### Limites:
- Propagação automática quando o impacto é claro e seguro
- Pausar e reportar quando há conflito, dependentes externos não mapeados, ou dúvida sobre intenção

---

## 4. Política de Ambiguidades

### Classificação:
- **Pequena (não bloqueante)**: não altera comportamento, arquitetura, contrato, segurança. Tratamento: premissa mínima conservadora + registro em `assumptions-log.md`.
- **Material (bloqueante)**: afeta comportamento funcional, regras, contratos, segurança. Tratamento: registrar em `open-questions.md` + reportar + aguardar confirmação.

### Regras para `open-questions.md`:
- Contém **apenas** dúvidas ainda abertas
- Dúvidas resolvidas são removidas imediatamente; resolução consolidada nos arquivos definitivos

### Regras para `assumptions-log.md`:
- Contém **apenas** premissas ainda ativas
- Premissas confirmadas consolidam na governança definitiva e saem do log
- Premissas invalidadas são removidas

### Divisão de implementação:
- Se parte é segura e parte ambígua: executar o seguro, bloquear o ambíguo, explicitar a divisão

---

## 5. Política de Classificação de Snippets

### Classificação obrigatória antes de qualquer ação:
| Classificação | Sinal | Tratamento |
|---|---|---|
| Normativo | "inclua exatamente", "copie", "preserve", "literalmente" | Copiar na íntegra; não reescrever |
| Ilustrativo | "algo assim", "tipo isso", "como exemplo" | Adaptar ao contexto do projeto |
| Preferencial | "prefiro esse padrão", "use esse estilo" | Seguir a abordagem |
| Contextual | "para contextualizar", "para entender" | Apenas apoio |

### Padrão conservador:
Na ausência de sinal claro → assumir **ilustrativo**.

### Snippets canônicos (`Instructions/snippets/canonical-snippets.md`):
- Não podem ser reescritos livremente
- Alteração exige nova instrução explícita do usuário ou conflito técnico reportado
- Verificar antes de implementar artefatos no escopo de um snippet registrado

---

## Relação com Outras Rules

- `source-of-truth-priority.md` — hierarquia usada para resolver conflitos de propagação
- `architecture-governance.md` — governança técnica referenciada pela propagação
- `naming-governance.md` — terminologia referenciada pela propagação
- `instruction-review.md` — alterações nesta rule ativam revisão via REVIEW.md

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-18 | Criado: consolidação de natural-language-normalization, repository-context-evolution, change-propagation, ambiguity-handling e snippet-handling | Reestruturação de governança |
| 2026-03-19 | Adicionado: ferramentas operacionais ao mapa de propagação e à lista de conhecimento durável | Lacuna de governança identificada |
