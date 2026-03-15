# Skill: ingest-definition

## Nome
Ingesta de Definição Durável

## Descrição
Esta skill orienta o comportamento do assistente quando o usuário introduz ou altera qualquer definição durável no repositório — seja uma regra de negócio, princípio técnico, padrão arquitetural, invariante, conceito de domínio, convenção de nomenclatura, decisão tecnológica ou qualquer outro conhecimento que deva persistir e governar interações futuras.

## Quando Usar
Ativar esta skill quando a mensagem do usuário:
- Introduzir uma nova regra de negócio
- Alterar ou revogar uma regra de negócio existente
- Introduzir ou alterar um princípio técnico
- Introduzir ou alterar um padrão arquitetural
- Introduzir ou alterar uma restrição técnica ou de negócio
- Introduzir ou alterar um invariante
- Introduzir um novo conceito de domínio
- Introduzir ou alterar uma convenção de nomenclatura
- Introduzir ou alterar uma decisão tecnológica relevante
- Resolver uma ambiguidade que deva persistir como conhecimento

## Entradas Esperadas
- Mensagem do usuário em linguagem natural (formal ou informal)
- Pode incluir trechos técnicos a serem classificados
- Pode ser fragmentada, ditada ou imperfeita
- Pode ser implícita (a definição emerge da solicitação sem ser declarada explicitamente)

## Workflow Interno

```
1. NORMALIZAR
   - Interpretar a mensagem semanticamente
   - Reconstruir a intenção em linguagem técnica limpa
   - Identificar a natureza da definição (técnica ou de negócio)

2. CLASSIFICAR
   - Técnica → roteiar para Instructions/architecture/
   - Negócio → roteiar para Instructions/business/
   - Terminologia → roteiar para Instructions/glossary/
   - Decisão → roteiar para Instructions/decisions/
   - Snippet canônico → roteiar para Instructions/snippets/

3. VERIFICAR CONFLITOS
   - A nova definição conflita com definições existentes?
   - Sim → registrar em open-questions.md, reportar antes de persistir
   - Não → prosseguir

4. ATUALIZAR A GOVERNANÇA
   - Atualizar o arquivo correto com a nova definição normalizada
   - Manter a estrutura e formato existente do arquivo
   - Não promover detalhes incidentais — apenas o que é durável

5. AVALIAR PROPAGAÇÃO
   - A nova definição afeta BDD? → atualizar ou criar cenários relevantes
   - A nova definição afeta contratos? → atualizar ou criar contratos relevantes
   - A nova definição introduz terminologia nova? → atualizar glossário
   - A nova definição implica nova organização? → atualizar folder-structure.md
   - A nova definição exige ADR? → criar em Instructions/decisions/

6. REGISTRAR PREMISSAS
   - Se a definição tiver partes não confirmadas pelo usuário → registrar em assumptions-log.md

7. IMPLEMENTAR (se aplicável)
   - Após governança atualizada, implementar mudanças de código ou artefatos se necessário

8. RELATAR
   - Definição ingerida e onde foi registrada
   - Arquivos de governança atualizados
   - Propagação realizada
   - Premissas adotadas
   - Conflitos encontrados e como foram tratados
```

## Saídas Esperadas
- Arquivos de governança atualizados com a nova definição
- Artefatos relacionados propagados
- Relatório claro do que foi persistido, propagado e assumido
- Dúvidas ou conflitos registrados se existirem

## Arquivos de Governança Relacionados
- `.claude/rules/business-ingestion.md`
- `.claude/rules/technical-ingestion.md`
- `.claude/rules/natural-language-normalization.md`
- `.claude/rules/change-propagation.md`
- `.claude/rules/source-of-truth-priority.md`
- `.claude/rules/ambiguity-handling.md`
- `Instructions/operating-model.md`
- `open-questions.md`
- `assumptions-log.md`

## Nota sobre Invocação
Esta skill não exige que o usuário diga "ingira esta definição". Ela é ativada automaticamente sempre que o assistente reconhece que a mensagem introduz ou altera conhecimento durável do repositório.
