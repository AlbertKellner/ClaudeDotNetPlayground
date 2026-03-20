# Dúvidas e Ambiguidades Abertas

## Instruções de Uso

Este arquivo contém **apenas** dúvidas e ambiguidades **ainda abertas**.

**Quando uma dúvida for resolvida**:
1. Remover o item desta lista ativa
2. Consolidar a resolução nos arquivos definitivos do repositório (regras, arquitetura, etc.)
3. Atualizar premissas relacionadas em `assumptions-log.md`
4. Se rastreabilidade histórica for necessária, registrar a resolução em local apropriado — mas não manter aqui como pendência aberta

---

## Dúvidas Ativas

### DUV-001
| Campo | Valor |
|---|---|
| **Id** | DUV-001 |
| **Data** | 2026-03-20 |
| **Origem** | Implementação da skill `pr-analysis` — passo 9 (merge condicional) |
| **Dúvida** | Qual método de merge deve ser utilizado ao realizar merge de PRs aprovados? (merge commit, squash, rebase) |
| **Por que importa** | Cada método gera histórico de commits diferente. Squash consolida em um commit; rebase reescreve a árvore; merge commit preserva todos os commits. A escolha afeta rastreabilidade, legibilidade do histórico e convenções de CI/CD. |
| **Artefatos impactados** | `.claude/skills/pr-analysis/SKILL.md` (passo 9), política de merge do repositório |
| **Bloqueante** | Não — o merge só ocorre com confirmação explícita do usuário, que pode informar o método nesse momento |
| **Status** | Aberta |
| **Premissas relacionadas** | PREM-005 em assumptions-log.md |

### DUV-002
| Campo | Valor |
|---|---|
| **Id** | DUV-002 |
| **Data** | 2026-03-20 |
| **Origem** | Implementação da skill `pr-analysis` — passo 7 (resposta a solicitações não conformes) |
| **Dúvida** | Para solicitações de mudança classificadas como NÃO CONFORMES, o revisor deve ser notificado apenas via resposta no comentário ou também deve haver um mecanismo de escalação (ex: tag no comentário, issue aberta, mention ao revisor)? |
| **Por que importa** | Sem mecanismo claro de escalação, solicitações não conformes podem ficar sem resolução se o revisor não verificar os comentários de resposta. Com escalação excessiva, pode gerar ruído. |
| **Artefatos impactados** | `.claude/skills/pr-analysis/SKILL.md` (passo 7b) |
| **Bloqueante** | Não — a resposta no comentário já é suficiente como comportamento mínimo |
| **Status** | Aberta |
| **Premissas relacionadas** | PREM-006 em assumptions-log.md |

### Template de Dúvida

```markdown
### DUV-[número]
| Campo | Valor |
|---|---|
| **Id** | DUV-[número] |
| **Data** | [YYYY-MM-DD] |
| **Origem** | [Resumo da solicitação que gerou esta dúvida] |
| **Dúvida** | [Formulação precisa da dúvida] |
| **Por que importa** | [Qual o impacto se a interpretação errada for seguida] |
| **Artefatos impactados** | [Quais arquivos ou componentes são afetados] |
| **Bloqueante** | Sim / Não |
| **Status** | Aberta |
| **Premissas relacionadas** | [PREM-NNN em assumptions-log.md] |
```

---

## Ambiguidades Ativas

> **Estado atual**: nenhuma ambiguidade ativa no momento do bootstrap.
> Ambiguidades serão registradas aqui ao longo das interações.

### Template de Ambiguidade

```markdown
### AMB-[número]
| Campo | Valor |
|---|---|
| **Id** | AMB-[número] |
| **Data** | [YYYY-MM-DD] |
| **Origem** | [Resumo da solicitação que gerou esta ambiguidade] |
| **Ambiguidade** | [Descrição das interpretações possíveis] |
| **Interpretação A** | [Primeira interpretação válida] |
| **Interpretação B** | [Segunda interpretação válida] |
| **Impacto da escolha** | [Como a escolha afeta implementação ou comportamento] |
| **Artefatos impactados** | [Quais arquivos ou componentes são afetados] |
| **Bloqueante** | Sim / Não |
| **Status** | Aberta |
| **Premissa adotada (se não bloqueante)** | [PREM-NNN em assumptions-log.md] |
```

---

## Referências Cruzadas

- `assumptions-log.md` — premissas adotadas relacionadas a estas dúvidas
- `.claude/rules/governance-policies.md` §4 — políticas de tratamento de ambiguidades
- `.claude/skills/resolve-ambiguity/SKILL.md` — workflow de resolução
