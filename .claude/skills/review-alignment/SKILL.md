# Skill: review-alignment

## Nome
Revisão de Alinhamento entre Artefatos

## Descrição
Esta skill orienta o assistente quando o usuário solicita (ou quando o assistente identifica necessidade de) revisão de consistência entre os artefatos do repositório — verificando se código, governança, BDD, contratos, glossário, decisões, snippets canônicos e listas de dúvidas e premissas estão todos alinhados entre si.

## Quando Usar
Ativar esta skill quando:
- O usuário solicitar revisão ou auditoria de consistência
- O usuário perguntar "isso está alinhado com...?"
- Após uma mudança significativa, para verificar se outros artefatos precisam ser atualizados
- Quando houver suspeita de divergência entre implementação e governança
- Quando o usuário pedir para validar se o repositório está consistente

## Entradas Esperadas
- Pode ser uma solicitação explícita de revisão
- Pode ser uma pergunta sobre consistência entre dois ou mais artefatos específicos
- Pode ser implícita (o usuário entrega um artefato e quer saber se está "certo")
- Pode ter escopo amplo (tudo) ou restrito (ex: "o BDD está alinhado com as regras?")

## Workflow Interno

```
1. DEFINIR ESCOPO DA REVISÃO
   - O usuário especificou o escopo? → usar o escopo especificado
   - O usuário não especificou? → revisar a área mais relevante à mensagem recente
   - Revisão completa solicitada? → mapear todos os pares de artefatos relacionados

2. COLETAR ARTEFATOS PARA REVISÃO
   Dependendo do escopo:
   - Código e artefatos declarativos (se acessíveis)
   - Instructions/architecture/ (visão técnica, princípios, padrões)
   - Instructions/business/ (regras, invariantes, workflows, domínio)
   - Instructions/bdd/ (cenários comportamentais)
   - Instructions/contracts/ (contratos de interface)
   - Instructions/glossary/ (terminologia)
   - Instructions/decisions/ (ADRs)
   - Instructions/snippets/ (snippets canônicos)
   - open-questions.md (dúvidas abertas)
   - assumptions-log.md (premissas ativas)

3. DETECTAR INCONSISTÊNCIAS
   Verificar:
   - Código implementa o que as regras de negócio definem?
   - BDD está alinhado com as regras de negócio?
   - Contratos refletem o comportamento definido em BDD e regras?
   - Terminologia no código é consistente com o glossário?
   - Terminologia nos contratos é consistente com o glossário?
   - Padrões arquiteturais registrados são seguidos na implementação?
   - Snippets canônicos registrados são respeitados na implementação?
   - Dúvidas abertas já foram resolvidas mas não fechadas formalmente?
   - Premissas registradas foram confirmadas ou invalidadas mas não atualizadas?

4. IDENTIFICAR LACUNAS
   - Há comportamentos implementados sem cobertura em BDD quando BDD seria útil?
   - Há interfaces sem contrato formalizado quando formalização seria necessária?
   - Há conceitos de domínio usados sem definição no glossário?
   - Há decisões tomadas sem ADR registrado?

5. REPORTAR CONFLITOS
   Para cada inconsistência encontrada:
   - Descrever o conflito (artefato A diz X, artefato B diz Y)
   - Identificar qual fonte deve prevalecer (ver source-of-truth-priority.md)
   - Propor resolução
   - Indicar se precisa de confirmação do usuário ou pode ser resolvido automaticamente

6. RESOLVER OU REGISTRAR
   - Inconsistências que podem ser resolvidas com segurança → resolver e reportar
   - Inconsistências que exigem confirmação → registrar em open-questions.md e reportar
   - Lacunas que o usuário deve decidir sobre → registrar em open-questions.md e reportar

7. RELATAR
   - Escopo revisado
   - Inconsistências encontradas e resolvidas
   - Inconsistências encontradas e registradas como dúvidas abertas
   - Lacunas identificadas
   - O que está alinhado (confirmação positiva)
```

## Saídas Esperadas
- Relatório de alinhamento com inconsistências e lacunas identificadas
- Artefatos corrigidos quando a correção foi segura e clara
- Dúvidas abertas registradas quando a resolução exige confirmação
- Confirmação explícita do que está consistente

## Arquivos de Governança Relacionados
- `.claude/rules/source-of-truth-priority.md` — hierarquia de prioridade entre fontes
- `.claude/rules/governance-policies.md` — políticas de propagação (§3) e ambiguidade (§4)
- `Instructions/operating-model.md`
- Todos os arquivos de `Instructions/`
- `open-questions.md`
- `assumptions-log.md`

## Nota sobre Invocação
Esta skill pode ser ativada proativamente pelo assistente após mudanças significativas, sem necessidade de solicitação explícita do usuário.
