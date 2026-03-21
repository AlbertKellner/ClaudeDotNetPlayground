# Skill: implement-request

## Nome
Execução de Solicitação de Implementação

## Descrição
Esta skill orienta o comportamento do assistente quando o usuário solicita criação, alteração, remoção ou revisão de qualquer artefato do repositório — código de aplicação, infraestrutura declarativa, contratos, mensageria, banco como código, configuração ou documentação operacional.

## Quando Usar
Ativar esta skill quando a mensagem do usuário:
- Solicitar criação de nova funcionalidade
- Solicitar alteração de comportamento existente
- Solicitar remoção de comportamento ou artefato
- Solicitar implementação guiada por uma regra existente
- Solicitar atualização de artefato com base em definições do repositório
- Solicitar criação ou alteração de infraestrutura declarativa
- Solicitar criação ou alteração de contratos
- Solicitar criação ou alteração de artefatos de mensageria
- Solicitar criação ou alteração de banco como código

## Entradas Esperadas
- Mensagem do usuário em linguagem natural (pode ser curta, informal, imperativa)
- Pode incluir trechos técnicos a serem classificados (normativos, ilustrativos, preferenciais, contextuais)
- Pode referenciar entidades, fluxos ou regras do domínio do repositório
- Pode ser implícita (a implementação necessária é inferida da mensagem)

## Workflow Interno

```
1. NORMALIZAR
   - Interpretar a mensagem semanticamente
   - Identificar o que deve ser criado, alterado ou removido
   - Identificar o escopo da mudança

2. CLASSIFICAR A SOLICITAÇÃO
   - Nova funcionalidade
   - Alteração de comportamento
   - Remoção
   - Implementação guiada por regra
   - Atualização decorrente de nova definição

3. LER A GOVERNANÇA RELEVANTE
   - Instructions/business/business-rules.md
   - Instructions/architecture/technical-overview.md
   - Instructions/bdd/ (cenários relacionados)
   - Instructions/contracts/ (contratos afetados)
   - Instructions/glossary/ubiquitous-language.md
   - Instructions/architecture/naming-conventions.md
   - Instructions/snippets/canonical-snippets.md
   - open-questions.md (dúvidas que afetam esta implementação)
   - assumptions-log.md (premissas ativas relevantes)

4. VERIFICAR AMBIGUIDADES
   - Há lacunas que impedem implementação segura?
   - Há conflito entre fontes de verdade?
   - A solicitação introduz nova definição durável?
   → Ver: .claude/rules/governance-policies.md §4

5. CLASSIFICAR TRECHOS TÉCNICOS (se houver)
   - Normativo → copiar na íntegra
   - Ilustrativo → adaptar ao contexto
   - Preferencial → seguir a abordagem
   - Contextual → usar como apoio
   → Ver: .claude/rules/governance-policies.md §5

6. REGISTRAR DÚVIDAS E PREMISSAS
   - Dúvidas materiais → open-questions.md
   - Premissas adotadas → assumptions-log.md
   - Se dúvida bloqueante → reportar e aguardar confirmação

7. ATUALIZAR A GOVERNANÇA PRIMEIRO (se necessário)
   - Se houver nova definição durável → atualizar governança antes do código
   → Política: governança antes de implementação (CLAUDE.md)

8. AVALIAR PROPAGAÇÃO
   - Quais artefatos relacionados são afetados?
   → Ver: .claude/rules/governance-policies.md §3

9. IMPLEMENTAR
   - Seguir as fontes de verdade do repositório
   - Usar terminologia do glossário
   - Seguir convenções de nomenclatura e organização de pastas
   - Aplicar snippets normativos na íntegra
   - Adaptar exemplos ilustrativos conforme contexto

10. VERIFICAR COBERTURA DE GOVERNANÇA (obrigatório pós-implementação)
    - Para cada arquivo .cs criado ou alterado, verificar:
      a. Se é em Infra/ → componente registrado em technical-overview.md? Subpasta em folder-structure.md?
      b. Se é em Shared/ExternalApi/ → integração registrada em technical-overview.md? Subpasta em folder-structure.md?
      c. Se é em Features/ → feature registrada em business-rules.md? Página na Wiki?
    - Se qualquer registro estiver ausente → atualizar ANTES de prosseguir para o relatório
    - Este passo NÃO é opcional — é o enforcement do passo 7 (governança antes de implementação)
    - Este passo previne a causa-raiz de componentes implementados sem registro na governança

11. RELATAR
    - Intenção interpretada
    - Arquivos de governança consultados
    - Arquivos alterados (governança + implementação)
    - Trechos classificados (normativos preservados, ilustrativos adaptados)
    - Governança atualizada (o que mudou antes da implementação)
    - Verificação de cobertura de governança (resultado do passo 10)
    - Premissas adotadas
    - Conflitos encontrados e resolução
    - Dúvidas registradas
    - Dúvidas resolvidas por esta mensagem
    - O que passa a valer como fonte de verdade ativa
```

## Saídas Esperadas
- Artefatos implementados ou atualizados
- Governança atualizada quando necessário
- Relatório completo de execução
- Dúvidas e premissas registradas quando necessário

## Arquivos de Governança Relacionados
- `.claude/rules/governance-policies.md` — políticas de normalização, ambiguidade, snippets, propagação e contexto
- `.claude/rules/source-of-truth-priority.md` — hierarquia de prioridade entre fontes
- `.claude/rules/instruction-review.md` — meta-regra de revisão de instruções
- `Instructions/operating-model.md`
- `open-questions.md`
- `assumptions-log.md`

## Nota sobre Invocação
Esta skill não exige que o usuário use linguagem específica. Toda mensagem que implica criação, alteração ou remoção de artefatos ativa esta skill automaticamente.
