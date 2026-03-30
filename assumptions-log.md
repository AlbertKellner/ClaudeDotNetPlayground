# Log de Premissas

## Instruções de Uso

Este arquivo contém **apenas** premissas **ainda ativas** ou ainda não confirmadas.

**Quando uma premissa for confirmada pelo usuário**:
1. Consolidar o conteúdo confirmado nos arquivos definitivos (regras, arquitetura, etc.)
2. Remover a premissa deste log ativo
3. Fechar a dúvida relacionada em `open-questions.md` se aplicável

**Quando uma premissa for invalidada pelo usuário**:
1. Remover esta premissa do log ativo
2. Rever as implementações que dependiam dela
3. Atualizar os artefatos afetados

**Premissas resolvidas não devem permanecer como regras ativas.**

---

## Premissas Ativas

### PREM-001
| Campo | Valor |
|---|---|
| **Id** | PREM-001 |
| **Data** | Bootstrap |
| **Premissa** | Os princípios genéricos de engenharia registrados em `Instructions/architecture/engineering-principles.md` são válidos até que princípios específicos do projeto sejam definidos. |
| **Motivo** | Bootstrap inicial sem stack ou domínio definido — necessário ter princípios de partida. |
| **Escopo** | `Instructions/architecture/engineering-principles.md` |
| **Artefatos impactados** | Todas as implementações futuras |
| **Nível de risco** | Baixo |
| **Precisa de confirmação** | Não — estes princípios são aceitos até instrução contrária explícita |
| **Status** | Ativo |

### PREM-002
| Campo | Valor |
|---|---|
| **Id** | PREM-002 |
| **Data** | Bootstrap |
| **Premissa** | A estrutura de governança criada no bootstrap é adequada para qualquer tipo de repositório (código, infraestrutura, mensageria, etc.) e será especializada conforme o domínio for definido. |
| **Motivo** | Bootstrap genérico por design — a especialização ocorre com uso. |
| **Escopo** | Toda a estrutura de governança |
| **Artefatos impactados** | `Instructions/`, `.claude/` |
| **Nível de risco** | Baixo |
| **Precisa de confirmação** | Não |
| **Status** | Ativo |

### PREM-003
| Campo | Valor |
|---|---|
| **Id** | PREM-003 |
| **Data** | Bootstrap |
| **Premissa** | Os contratos e schemas em `Instructions/contracts/` são placeholders genéricos e devem ser substituídos quando o domínio real for definido. Nenhuma implementação deve depender do conteúdo desses placeholders. |
| **Motivo** | Stack e domínio desconhecidos no momento do bootstrap. |
| **Escopo** | `Instructions/contracts/openapi.yaml`, `Instructions/contracts/asyncapi.yaml`, `Instructions/contracts/schemas/`, `Instructions/contracts/examples/` |
| **Artefatos impactados** | Implementações que dependem de contratos |
| **Nível de risco** | Baixo |
| **Precisa de confirmação** | Não — implícito pela natureza do bootstrap |
| **Status** | Ativo |

---

## Template para Nova Premissa

```markdown
### PREM-[número]
| Campo | Valor |
|---|---|
| **Id** | PREM-[número] |
| **Data** | [YYYY-MM-DD] |
| **Premissa** | [O que foi assumido sem confirmação explícita] |
| **Motivo** | [Por que foi necessário assumir sem confirmação] |
| **Escopo** | [Quais regras, fluxos, entidades ou artefatos dependem desta premissa] |
| **Artefatos impactados** | [Arquivos ou componentes concretos] |
| **Nível de risco** | Baixo / Médio / Alto |
| **Precisa de confirmação** | Sim / Não |
| **Status** | Ativo |
| **Dúvida relacionada** | [DUV-NNN ou AMB-NNN em open-questions.md, se aplicável] |
```

---

## Referências Cruzadas

- `open-questions.md` — dúvidas relacionadas às premissas
- `.claude/rules/governance-policies.md` §4 — políticas de ambiguidade e premissas
- `Instructions/business/assumptions.md` — premissas de negócio específicas
