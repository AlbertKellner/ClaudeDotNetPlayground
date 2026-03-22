# Regra: Otimização de Tokens e Compactação Estratégica

## Propósito

Esta rule define as políticas de otimização de consumo de tokens, compactação estratégica de contexto e uso eficiente de subagentes. O objetivo é maximizar a qualidade de cada sessão enquanto minimiza o custo e a degradação de contexto.

---

## Princípio Fundamental

> Contexto é um recurso finito. Cada token consumido deve contribuir para o resultado.
> Compactar no momento certo preserva qualidade. Compactar no momento errado destrói contexto.
> Delegar para subagentes protege o contexto principal de informação volumosa e transiente.

---

## Políticas

### Compactação Estratégica

A compactação deve ocorrer em **pontos lógicos do pipeline**, não em percentual arbitrário de contexto.

#### Pontos ótimos de compactação (escopo Código)

| Após passo | Razão | O que descartar | O que preservar |
|---|---|---|---|
| 0 (ambiente verificado) | Diagnóstico de ambiente é transiente | Output de verificação de pré-requisitos | Resultado: ambiente pronto ou não |
| 3 (testes passaram) | Output de testes é volumoso | Saída completa dos testes | Resultado: pass/fail + lista de falhas |
| 6 (endpoints validados) | HTTP responses são volumosas | Respostas HTTP brutas, logs de curl | Relatório de validação formatado |
| 8 (docker down) | Contexto Docker não é mais necessário | Logs de container, output de docker commands | Sumário de validação |
| 11 (CI verificado) | Logs de CI são extensos | Output completo das GitHub Actions | Resultado: pass/fail + erros encontrados |

#### Quando NUNCA compactar

- Durante implementação multi-arquivo (perda de contexto entre edições)
- Durante debug ativo (perda de rastreabilidade do problema)
- Entre passos 1-2 (build → run são dependentes e sequenciais)
- Entre passos 4-6 (docker up → validação são dependentes)
- No meio de análise de PR (perda de contexto dos review comments)

### Uso de Subagentes

Tarefas que geram output volumoso devem ser delegadas a subagentes para proteger o contexto principal.

| Tarefa | Tipo de subagente | Modelo | Retorno esperado |
|---|---|---|---|
| Exploração de codebase | `Explore` | haiku | Resumo de 2-3 parágrafos |
| Execução de auditoria de governança | `general-purpose` | haiku | Lista de falhas/avisos |
| Monitoramento de CI (passo 11) | `general-purpose` | haiku | Status + erros encontrados |
| Análise de logs Docker (passo 7) | `general-purpose` | haiku | Storytelling resumido |
| Pesquisa de padrões em observações | `general-purpose` | haiku | Instintos candidatos |

**Princípio**: o subagente lê 20 arquivos e 5000+ linhas → retorna 1-2 parágrafos. O contexto principal permanece limpo.

### Configuração de Ambiente

| Configuração | Valor recomendado | Efeito |
|---|---|---|
| `CLAUDE_AUTOCOMPACT_PCT_OVERRIDE` | `60` | Compacta mais cedo que o default (95%), antes da degradação |
| `CLAUDE_CODE_SUBAGENT_MODEL` | `haiku` | Subagentes de exploração usam Haiku (~80% mais barato) |

### Governança sob Demanda (Lazy Loading)

Nem todos os arquivos de governança são necessários em toda sessão. A prioridade de carregamento deve ser:

**Sempre carregados (core):**
- `Instructions/operating-model.md`
- `Instructions/architecture/technical-overview.md`
- `Instructions/architecture/engineering-principles.md`
- `Instructions/business/business-rules.md`
- `.claude/rules/governance-policies.md`
- `.claude/rules/source-of-truth-priority.md`

**Carregados por escopo:**
- Arquitetura detalhada (`patterns.md`, `folder-structure.md`, `naming-conventions.md`): quando a tarefa cria/altera estrutura
- BDD/Contratos: quando a tarefa envolve comportamento formal
- Wiki governance: quando a tarefa altera wiki
- Domínio (`domain-model.md`, `invariants.md`, `workflows.md`): quando a tarefa envolve modelagem

**Nota**: esta é uma diretriz para evolução futura. A implementação de lazy loading no CLAUDE.md requer reestruturação dos imports (Fase 7 do plano).

---

## Relação com Outras Rules

- `continuous-learning.md` — observações de uso de tokens podem gerar instintos de otimização
- `environment-readiness.md` — configuração de `CLAUDE_AUTOCOMPACT_PCT_OVERRIDE` é pré-requisito de ambiente
- `governance-policies.md` §2 — eficiência de contexto complementa eficiência de execução
- `execution-time-tracking.md` — visibilidade de tempo + tokens dão visão completa de eficiência

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-22 | Criado: rule de otimização de tokens com políticas de compactação estratégica, subagentes e lazy loading | Adaptação do ECC — Fase 5 do sistema de aprendizado |
