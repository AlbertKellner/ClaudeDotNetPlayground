# Skill: review-instructions

## Nome
Revisão de Instruções e Governança

## Descrição
Esta skill executa o processo de revisão estruturado definido em `REVIEW.md` sempre que arquivos de instrução, regras, skills ou governança são criados, alterados ou removidos.

## Quando Usar
Ativar esta skill quando:
- Qualquer arquivo em `Instructions/`, `.claude/rules/`, `.claude/skills/`, `.claude/hooks/` for criado, alterado ou removido
- `CLAUDE.md`, `REVIEW.md` ou `.claude/settings.json` for alterado
- O hook `instruction-change-detector.sh` emitir lembrete
- O usuário solicitar revisão de instruções explicitamente
- Após uma reestruturação de governança

## Entradas Esperadas
- Lista de arquivos de instrução criados, alterados ou removidos na tarefa atual
- Pode ser implícita (o assistente detecta as mudanças)

## Workflow Interno

```
1. IDENTIFICAR ARQUIVOS ALTERADOS
   - Quais arquivos de instrução foram criados, alterados ou removidos?
   - Qual o tipo de cada arquivo? (CLAUDE.md, rule, skill, instruction, hook, settings)

2. EXECUTAR CHECKLIST DE LOCALIZAÇÃO (REVIEW.md §1)
   - A instrução está no tipo correto de arquivo?
   - Política → rule; Workflow → skill; Global → CLAUDE.md; Domínio → Instructions/
   - Se incorreto → propor migração

3. EXECUTAR CHECKLIST DE DUPLICAÇÃO (REVIEW.md §2)
   - O conteúdo já existe em outro arquivo?
   - Um rule e um skill cobrem o mesmo workflow?
   - CLAUDE.md ou operating-model.md repetem conteúdo de rules?
   - Se duplicação detectada → propor consolidação

4. EXECUTAR CHECKLIST DE SEPARAÇÃO (REVIEW.md §3)
   - Rules definem apenas políticas (o quê)?
   - Skills definem apenas workflows (como)?
   - Hooks fazem apenas enforcement?
   - CLAUDE.md é conciso e global?
   - Se violação detectada → propor correção

5. EXECUTAR CHECKLIST DE ENFORCEMENT (REVIEW.md §4)
   - A regra requer enforcement automatizado?
   - Afeta permissões de segurança?
   - Requer validação antes de commit ou após escrita?
   - Se sim → verificar/criar hook correspondente

6. EXECUTAR CHECKLIST DE CONSISTÊNCIA (REVIEW.md §5)
   - Imports em CLAUDE.md atualizados?
   - Referências cruzadas entre rules corretas?
   - Skills referenciam rules corretas?
   - Nova dependência de ambiente introduzida?
   - Se inconsistência → corrigir

7. EXECUTAR CHECKLIST DE PROPAGAÇÃO (REVIEW.md §6)
   - A mudança afeta regras de negócio, contratos, arquitetura ou nomenclatura?
   - Artefatos relacionados precisam ser atualizados?
   - Se sim → propagar ou registrar como pendência

8. RELATAR
   - Arquivos revisados
   - Violações encontradas e corrigidas
   - Violações encontradas e pendentes
   - Resultado: aprovado / reprovado com pendências
```

## Saídas Esperadas
- Relatório de revisão com resultado por checklist
- Correções aplicadas automaticamente quando seguras
- Pendências registradas quando correção requer confirmação
- Confirmação de que imports e referências estão consistentes

## Arquivos de Governança Relacionados
- `REVIEW.md` — checklists executados por esta skill
- `.claude/rules/instruction-review.md` — meta-regra que ativa esta skill
- `.claude/rules/governance-policies.md` — políticas protegidas por esta revisão
- `CLAUDE.md` — ponto de entrada cujos imports são verificados

## Nota sobre Invocação
Esta skill é ativada automaticamente pelo hook `instruction-change-detector.sh` e pela meta-regra `instruction-review.md`. Pode também ser invocada explicitamente pelo usuário.
