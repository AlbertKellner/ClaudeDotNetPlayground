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

> **Estado atual**: nenhuma dúvida ativa no momento do bootstrap.
> Dúvidas serão registradas aqui ao longo das interações.

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
- `.claude/rules/ambiguity-handling.md` — regras de como tratar estas dúvidas
- `.claude/skills/resolve-ambiguity/SKILL.md` — workflow de resolução
