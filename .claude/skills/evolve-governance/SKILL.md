# Skill: evolve-governance

## Nome
Evolução da Base de Governança

## Descrição
Esta skill orienta o assistente quando a própria base de governança do repositório precisa ser evoluída — consolidando conteúdo, reorganizando seções, melhorando clareza, eliminando duplicações e garantindo que o conhecimento durável seja preservado durante a evolução.

## Quando Usar
Ativar esta skill quando:
- O usuário solicitar reorganização ou melhoria da governança
- Houver duplicação evidente entre arquivos de governança
- Seções de arquivos tiverem crescido de forma que prejudicam a clareza
- Houver contradição interna dentro de um arquivo de governança
- Uma decisão resolvida ainda aparecer como pendente em múltiplos lugares
- Uma premissa confirmada ainda aparecer como ativa em assumptions-log.md
- O assistente identificar que a governança ficou desatualizada após mudanças acumuladas

## Entradas Esperadas
- Pode ser solicitação explícita do usuário ("reorganize a governança", "limpe as dúvidas resolvidas")
- Pode ser implícita (o assistente identifica a necessidade durante outra operação)
- Pode ter escopo específico (um arquivo, uma seção, um tipo de artefato)
- Pode ser ampla (auditoria geral da governança)

## Workflow Interno

```
1. MAPEAR O ESCOPO DE EVOLUÇÃO
   - Quais arquivos estão desatualizados, duplicados ou confusos?
   - Há dúvidas resolvidas ainda abertas em open-questions.md?
   - Há premissas confirmadas ou invalidadas ainda no assumptions-log.md?
   - Há conteúdo duplicado entre arquivos de governança?
   - Há contradições internas dentro de arquivos?

2. PRESERVAR ANTES DE REORGANIZAR
   - Identificar todo conhecimento durável nos arquivos a serem evoluídos
   - Garantir que nenhum conhecimento durável seja perdido na reorganização
   - Registrar em assumptions-log.md se houver incerteza sobre o que preservar

3. CONSOLIDAR CONTEÚDO DUPLICADO
   - Identificar onde o mesmo conteúdo aparece em múltiplos arquivos
   - Definir o arquivo canônico para cada tipo de conteúdo
   - Mover conteúdo para o arquivo canônico
   - Substituir duplicatas por referências ao arquivo canônico

4. REORGANIZAR SEÇÕES
   - Melhorar a estrutura interna dos arquivos para maior clareza
   - Manter a consistência de formato entre arquivos do mesmo tipo
   - Usar cabeçalhos e seções que facilitem consulta futura

5. LIMPAR REGISTROS ATIVOS
   - Verificar open-questions.md: remover dúvidas resolvidas, consolidar nos artefatos definitivos
   - Verificar assumptions-log.md: remover premissas confirmadas ou invalidadas, consolidar nos artefatos definitivos
   - Se rastreabilidade histórica for necessária, registrar resolução em local de histórico

6. MELHORAR CLAREZA
   - Reescrever seções ambíguas ou imprecisas
   - Adicionar exemplos onde faltam
   - Remover conteúdo genérico que não é específico ao repositório

7. VERIFICAR CONSISTÊNCIA PÓS-EVOLUÇÃO
   - Os arquivos evoluídos são consistentes entre si?
   - Os imports em CLAUDE.md ainda refletem a estrutura atual?
   - A estrutura de pastas está atualizada em folder-structure.md?

8. RELATAR
   - O que foi consolidado
   - O que foi reorganizado
   - O que foi removido (e onde o conteúdo equivalente foi mantido)
   - Dúvidas e premissas removidas dos registros ativos e onde foram consolidadas
   - O que permanece como conhecimento durável
```

## Saídas Esperadas
- Arquivos de governança mais claros, concisos e consistentes
- Registros de dúvidas e premissas limpos (sem itens obsoletos)
- Relatório do que foi evoluído e por quê
- Garantia de preservação de todo conhecimento durável

## Arquivos de Governança Relacionados
- Todos os arquivos de `Instructions/`
- `.claude/rules/repository-context-evolution.md`
- `open-questions.md`
- `assumptions-log.md`
- `CLAUDE.md`

## Nota sobre Invocação
Esta skill deve ser ativada periodicamente, especialmente após acúmulo de interações que geraram muitas dúvidas, premissas ou definições. O assistente pode sugerir proativamente uma evolução da governança quando identificar sinais de desatualização ou inconsistência.
