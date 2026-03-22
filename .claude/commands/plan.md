---
description: Reformular requisitos, avaliar riscos e criar plano de implementação passo a passo. AGUARDAR confirmação do usuário ANTES de tocar em qualquer código.
---

# Comando Plan

Cria um plano de implementação abrangente antes de escrever qualquer código.

## O Que Este Comando Faz

1. **Reformular Requisitos** — Esclarecer o que precisa ser construído
2. **Identificar Riscos** — Levantar problemas e bloqueios potenciais
3. **Criar Plano por Fases** — Dividir implementação em etapas
4. **Aguardar Confirmação** — DEVE receber aprovação do usuário antes de prosseguir

## Quando Usar

Use `/plan` quando:
- Iniciando uma nova feature
- Fazendo mudanças arquiteturais significativas
- Trabalhando em refatoração complexa
- Múltiplos arquivos/componentes serão afetados
- Requisitos estão ambíguos

## Como Funciona

1. **Analisar a solicitação** e reformular requisitos em termos claros
2. **Dividir em fases** com passos específicos e acionáveis
3. **Identificar dependências** entre componentes
4. **Avaliar riscos** e bloqueios potenciais
5. **Estimar complexidade** (Alta/Média/Baixa)
6. **Apresentar o plano** e AGUARDAR confirmação explícita

## Formato de Saída

```markdown
# Plano de Implementação: [Título]

## Reformulação dos Requisitos
[O que precisa ser feito em termos claros]

## Fases de Implementação

### Fase 1: [Título]
- [passo específico]
- [passo específico]

### Fase 2: [Título]
- [passo específico]

## Dependências
- [dependência entre componentes ou externas]

## Riscos
- ALTO: [risco crítico]
- MÉDIO: [risco moderado]
- BAIXO: [risco menor]

## Complexidade Estimada: [ALTA/MÉDIA/BAIXA]

**AGUARDANDO CONFIRMAÇÃO**: Prosseguir com este plano? (sim/não/modificar)
```

## Notas Importantes

**CRÍTICO**: O comando **NÃO** escreverá nenhum código até que o usuário confirme explicitamente com "sim" ou "prosseguir".

Para solicitar mudanças:
- "modificar: [suas mudanças]"
- "abordagem diferente: [alternativa]"
- "pular fase 2 e fazer fase 3 primeiro"

## Integração com Outros Comandos

Após planejar:
- Use `/tdd` para implementar com desenvolvimento guiado por testes
- Use `/build-fix` se ocorrerem erros de build
- Use `/code-review` para revisar a implementação concluída

## Integração com Governança

O plano deve considerar:
- Regras de negócio em `Instructions/business/business-rules.md`
- Padrões em `Instructions/architecture/patterns.md`
- Princípios em `Instructions/architecture/engineering-principles.md`
- Pipeline pré-commit em `CLAUDE.md`
