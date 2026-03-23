# Claude — Skills

## Descrição

Catálogo de todas as skills disponíveis neste repositório, organizadas por tipo de ativação: skills ativadas por tipo de mensagem e skills de pipeline. Deve ser consultada para entender quais capacidades automatizadas existem.

## Contexto

Skills são workflows executáveis que definem **como** executar processos. Cada skill reside em `.claude/skills/{nome-da-skill}/SKILL.md` e contém: Nome, Quando Usar, Workflow Interno e Arquivos Relacionados. Skills referenciam as políticas das rules mas não as redefinem.

---

## Skills Ativadas por Tipo de Mensagem

Estas 8 skills são ativadas automaticamente quando a mensagem do usuário é classificada no tipo correspondente:

| Skill | Gatilho | Propósito |
|---|---|---|
| `ingest-definition` | Nova definição durável | Persistir regras, decisões, restrições, terminologia e padrões na governança |
| `implement-request` | Solicitação de implementação | Criar, alterar ou remover artefatos de código no repositório |
| `review-alignment` | Revisão de alinhamento | Verificar consistência entre artefatos de governança e implementação |
| `evolve-governance` | Evolução de governança | Reorganizar, consolidar ou melhorar a base de governança |
| `resolve-ambiguity` | Resolução de ambiguidade | Responder dúvidas registradas e esclarecer lacunas |
| `apply-user-snippet` | Trecho técnico fornecido | Classificar (normativo, ilustrativo, preferencial, contextual) e aplicar código ou configuração |
| `review-instructions` | Alteração de instrução | Executar o checklist completo de `REVIEW.md` para mudanças de governança |
| `pr-analysis` | Análise de PR | Analisar solicitações de mudança em pull request aberto |

---

## Skills de Pipeline

Estas 4 skills são invocadas como passos do pipeline pré-commit definido em `CLAUDE.md`, não por classificação de mensagem:

| Skill | Passo do Pipeline | Propósito |
|---|---|---|
| `verify-environment` | Passo 0 | Verificar pré-requisitos de ambiente antes de qualquer operação substantiva |
| `validate-endpoints` | Passo 6 | Validação HTTP real de endpoints criados ou alterados após implementação |
| `manage-pr-lifecycle` | Passos 10-11 | Criação ou atualização de Pull Request e acompanhamento de GitHub Actions |
| `governance-behavior-tracking` | Início e fim da tarefa | Coleta, apresentação e verificação de todos os comportamentos esperados da sessão |

---

## Total de Skills

O repositório possui **12 skills** no total: 8 ativadas por tipo de mensagem + 4 de pipeline. Todas residem em `.claude/skills/`.

---

## Localização

Todas as skills residem em `.claude/skills/{nome-da-skill}/SKILL.md`.

Cada arquivo `SKILL.md` contém obrigatoriamente:

| Seção | Conteúdo |
|---|---|
| **Nome / Propósito** | Identificação e finalidade da skill |
| **Quando Usar** | Condições de ativação |
| **Workflow Interno** | Passos procedurais detalhados |
| **Arquivos Relacionados** | Rules, instructions e artefatos referenciados |

A auditoria automatizada (`governance-audit.sh`, check #19) verifica que toda skill possui a estrutura mínima obrigatória.

---

## Referências

- [Claude — Visão Geral](Claude-Overview)
- [Claude — Convenções e Restrições](Claude-Conventions)
- [Claude — Hooks](Claude-Hooks)
