# REVIEW.md — Checklist de Revisão Estruturado

## Propósito

Este arquivo define o processo obrigatório de revisão para toda criação ou alteração de instruções, regras, skills e artefatos de governança neste repositório. Funciona como meta-governança — governa a própria governança.

---

## Quando Este Checklist É Ativado

Este checklist é **obrigatório** sempre que qualquer um destes arquivos for criado, alterado ou removido:
- `CLAUDE.md`
- `REVIEW.md` (auto-referente)
- Qualquer arquivo em `Instructions/`
- Qualquer arquivo em `.claude/rules/`
- Qualquer arquivo em `.claude/skills/`
- `.claude/settings.json`
- Qualquer arquivo em `.claude/hooks/`

---

## Checklist 1 — Localização Correta

| # | Verificação | Critério |
|---|---|---|
| 1.1 | A instrução está no arquivo correto? | CLAUDE.md = global conciso; rules = políticas; skills = workflows; Instructions = domínio e arquitetura |
| 1.2 | É uma política ou restrição? | Deve estar em `.claude/rules/` |
| 1.3 | É um workflow executável? | Deve estar em `.claude/skills/` |
| 1.4 | É um princípio global que afeta toda interação? | Deve estar em `CLAUDE.md` |
| 1.5 | É definição de domínio, arquitetura ou negócio? | Deve estar em `Instructions/` |

---

## Checklist 2 — Duplicação

| # | Verificação | Ação se positivo |
|---|---|---|
| 2.1 | O conteúdo já existe em outro arquivo? | Consolidar no local canônico; usar referência no outro |
| 2.2 | Um rule e um skill cobrem o mesmo workflow? | Manter workflow no skill; manter apenas política no rule |
| 2.3 | CLAUDE.md repete conteúdo de um rule? | Remover do CLAUDE.md; manter referência via import |
| 2.4 | operating-model.md repete conteúdo de CLAUDE.md? | Eliminar a duplicata; manter no local de maior prioridade |

---

## Checklist 3 — Separação de Responsabilidades

| # | Verificação | Critério |
|---|---|---|
| 3.1 | Rules definem apenas POLÍTICAS (o quê)? | Rules não devem conter workflows procedurais detalhados |
| 3.2 | Skills definem apenas WORKFLOWS (como)? | Skills não devem redefinir políticas — devem referenciar rules |
| 3.3 | Hooks fazem apenas ENFORCEMENT? | Hooks não devem conter lógica de negócio |
| 3.4 | CLAUDE.md é conciso e global? | Máximo ~100 linhas; detalhes operacionais delegados a rules/skills |

---

## Checklist 4 — Enforcement

| # | Verificação | Ação se positivo |
|---|---|---|
| 4.1 | A regra requer enforcement automatizado? | Criar ou atualizar hook correspondente |
| 4.2 | A regra afeta permissões de segurança? | Atualizar `.claude/settings.json` |
| 4.3 | A regra requer validação antes de commit? | Garantir que hook pre-commit cobre o caso |
| 4.4 | A regra requer validação após escrita de arquivo? | Garantir que hook PostToolUse cobre o caso |

---

## Checklist 5 — Consistência

| # | Verificação | Ação se positivo |
|---|---|---|
| 5.1 | Os imports em CLAUDE.md estão atualizados? | Adicionar/remover imports conforme arquivos criados/removidos |
| 5.2 | Referências cruzadas entre rules estão corretas? | Atualizar seção "Relação com Outras Rules" |
| 5.3 | Skills referenciam rules corretas? | Atualizar seção "Arquivos de Governança Relacionados" |
| 5.4 | A instrução introduz nova dependência de ambiente? | Atualizar `scripts/setup-env.sh` e `environment-readiness.md` |

---

## Checklist 6 — Propagação

| # | Verificação | Ação se positivo |
|---|---|---|
| 6.1 | A mudança afeta regras de negócio? | Verificar BDD, contratos, glossário, implementação |
| 6.2 | A mudança afeta contratos? | Verificar negócio, BDD, implementação |
| 6.3 | A mudança afeta arquitetura? | Verificar technical-overview, folder-structure, naming-conventions |
| 6.4 | A mudança afeta nomenclatura? | Verificar glossário, BDD, contratos, código |

---

## Checklist 7 — Revisão de Código (quando aplicável)

| # | Verificação | Critério |
|---|---|---|
| 7.1 | Alinhado com regras de negócio ativas? | Consultar `Instructions/business/business-rules.md` |
| 7.2 | Alinhado com arquitetura (Vertical Slice)? | Consultar `Instructions/architecture/technical-overview.md` |
| 7.3 | Convenções de nomenclatura seguidas? | Consultar `Instructions/architecture/naming-conventions.md` |
| 7.4 | Padrão de logging SNP-001 aplicado? | Consultar `Instructions/snippets/canonical-snippets.md` |
| 7.5 | Testes adicionados/atualizados? | Verificar cobertura de testes para a mudança |
| 7.6 | Endpoints validados via HTTP real? | Executar validação conforme `endpoint-validation.md` |

---

## Checklist 8 — Auditoria Automatizada

| # | Verificação | Critério |
|---|---|---|
| 8.1 | `bash scripts/governance-audit.sh` passa sem falhas? | Todas as verificações retornam `[OK]` |
| 8.2 | Se houve falhas, foram corrigidas antes do commit? | Nenhuma falha pendente |

---

## Referências Cruzadas

- `.claude/rules/instruction-review.md` — meta-regra que ativa este checklist
- `.claude/rules/governance-audit.md` — política de auditoria automatizada (Checklist 8)
- `.claude/skills/review-instructions/SKILL.md` — skill que executa este checklist
- `.claude/hooks/instruction-change-detector.sh` — hook que detecta mudanças, emite lembrete e executa auditoria
- `scripts/governance-audit.sh` — script de auditoria automatizada
- `CLAUDE.md` — ponto de entrada global que referencia este arquivo

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-18 | Criado: checklist estruturado de revisão de instruções | Reestruturação de governança |
| 2026-03-21 | Adicionado: Checklist 8 — Auditoria Automatizada via scripts/governance-audit.sh | Análise de inconsistências do repositório |
