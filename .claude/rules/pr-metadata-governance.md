# Regra: Governança de Metadados de Pull Request

## Propósito

Esta rule define como o assistente deve criar, manter e atualizar os metadados de pull requests neste repositório. Título, descrição e labels de todo PR devem refletir com precisão o estado atual da implementação.

---

## Princípio Fundamental

> Pull requests são a interface pública do trabalho realizado.
> Título e descrição devem ser sempre consistentes com o estado real do código.
> Um PR desatualizado gera confusão e dificulta revisão e governança.

---

## Obrigações do Assistente

### Quando criar um novo PR:
1. Definir título claro, objetivo e tecnicamente descritivo (máximo ~70 caracteres)
2. Preencher a descrição com as três seções obrigatórias (ver template)
3. Adicionar as labels correspondentes ao tipo de alteração

### Quando adicionar novos commits a um PR existente:
1. Revisar o título — atualizar se a nova mudança alterar o escopo ou foco
2. Revisar cada seção da descrição e incorporar o que foi adicionado
3. Remover qualquer referência a alterações descartadas

### Quando descartar alterações de um PR:
1. Remover da descrição qualquer referência ao que foi descartado
2. Verificar se o título ainda é preciso após a remoção
3. Atualizar labels se o escopo da mudança se alterou

---

## Formato Obrigatório do Título

O título deve ser um **Semantic Commit** no formato:

```
<tipo>(<escopo>): <descrição imperativa em português>
```

**Tipos aceitos:**
- `feat` — nova funcionalidade
- `fix` — correção de bug
- `docs` — documentação
- `refactor` — refatoração sem mudança de comportamento
- `test` — testes
- `chore` — manutenção, dependências, configuração
- `ci` — CI/CD e workflows

**Exemplos de títulos válidos:**
- `feat(auth): adicionar endpoint de login com JWT Bearer Token`
- `fix(health-check): corrigir retorno Healthy quando Datadog bloqueia IPs privados`
- `docs(wiki): documentar arquitetura e features implementadas`
- `refactor(governance): redefinir setup-env.sh como modelo declarativo`

---

## Estrutura Obrigatória da Descrição

A descrição deve sempre seguir as três seções do template `.github/pull_request_template.md`:

### Seção 1 — Motivos da alteração
- Por que esta mudança é necessária
- Qual problema foi identificado
- Qual regra de negócio é implementada
- Qual comportamento incorreto é corrigido

### Seção 2 — Plano de execução
- Quais etapas foram planejadas
- Qual sequência de implementação foi adotada
- Quais decisões técnicas relevantes foram tomadas

### Seção 3 — O que foi realizado
- Descrição completa e técnica de tudo que foi feito
- Arquivos criados ou modificados (com breve descrição da mudança)
- Mudanças de comportamento observáveis
- Endpoints adicionados ou alterados
- Regras de negócio implementadas
- Qualquer outro detalhe relevante para revisão

**Esta seção deve estar sempre atualizada.** Toda vez que um novo commit for adicionado, ela deve ser revisada e atualizada para refletir o estado atual.

---

## Labels Obrigatórias

Todo PR deve ter pelo menos uma label de tipo e uma de impacto:

### Labels de tipo (escolher a mais representativa):
| Label | Quando usar |
|---|---|
| `feature` | Nova funcionalidade implementada |
| `bugfix` | Correção de bug ou comportamento incorreto |
| `documentation` | Criação ou atualização de documentação |
| `refactoring` | Refatoração sem mudança de comportamento |
| `governance` | Atualização de regras, governança ou processo |
| `infrastructure` | Docker, CI/CD, scripts de ambiente |
| `testing` | Adição ou correção de testes |

### Labels de impacto (escolher uma):
| Label | Quando usar |
|---|---|
| `breaking-change` | Altera interface pública, contrato ou comportamento esperado |
| `non-breaking` | Mudança compatível com versão atual |

### Labels opcionais:
| Label | Quando usar |
|---|---|
| `wip` | Trabalho em andamento, não pronto para merge |
| `needs-review` | Aguardando revisão |

---

## Política de Consistência

- A descrição é a **fonte de verdade textual** do PR
- Se há discrepância entre a descrição e os commits, a descrição deve ser corrigida
- Se há alterações no código que não estão na descrição, a descrição está incompleta
- Se há referências na descrição a código que não existe mais, a descrição está desatualizada

**O assistente é responsável por manter essa consistência automaticamente**, sem necessidade de instrução explícita do usuário a cada atualização.

---

## Relação com Outras Rules

- Skill `implement-request` — o PR é criado no passo 11 do workflow de implementação, após commit e push
- `governance-policies.md` §3 — quando o escopo de mudança se expande, o PR deve ser atualizado
- `bash-error-logging.md` — erros encontrados durante validação devem ser refletidos no PR quando relevantes
- `endpoint-validation.md` — resultados de validação de endpoints devem constar na seção "O que foi realizado"

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-18 | Criado: governança de metadados de PR | Instrução do usuário |
| 2026-03-18 | Referência cruzada atualizada: passo 11 do implement-request agora implementa criação do PR | Correção de lacuna de governança |
