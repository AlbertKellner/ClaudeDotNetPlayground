# Regra: Governança Arquitetural

## Propósito

Esta rule define como o assistente deve identificar, registrar, preservar e aplicar decisões e princípios arquiteturais neste repositório.

---

## Identificação de Solicitações Arquiteturais

O assistente deve reconhecer como entrada arquitetural mensagens que:
- Definem ou alteram a estrutura de módulos, serviços, camadas ou componentes
- Estabelecem princípios de design (ex: separação de responsabilidades, inversão de dependência)
- Definem padrões de comunicação (síncrono, assíncrono, eventos, mensagens)
- Estabelecem restrições técnicas (ex: sem dependências circulares, sem lógica de negócio na camada de infraestrutura)
- Definem estratégias de persistência, mensageria ou integração
- Introduzem ou alteram fronteiras de bounded contexts
- Definem ou alteram como componentes se comunicam
- Estabelecem decisões de tecnologia (banco de dados, message broker, framework, runtime)

---

## Registro de Princípios Técnicos

Quando o usuário introduzir um novo princípio técnico:
1. Registrá-lo em `Instructions/architecture/engineering-principles.md`
2. Verificar se conflita com princípios existentes
3. Se conflitar, registrar o conflito em `open-questions.md` e reportar antes de persistir
4. Atualizar `Instructions/architecture/technical-overview.md` se o princípio afetar a visão geral

---

## Registro de Padrões Técnicos

Quando o usuário introduzir ou confirmar um padrão arquitetural:
1. Registrá-lo em `Instructions/architecture/patterns.md` com contexto de quando e como aplicar
2. Verificar consistência com os princípios existentes
3. Verificar se há padrões conflitantes já registrados
4. Propagar para `Instructions/architecture/naming-conventions.md` e `Instructions/architecture/folder-structure.md` quando o padrão afetar nomenclatura ou organização

---

## Registro de Decisões Arquiteturais

Quando uma decisão arquitetural relevante for tomada (seja pelo usuário ou como resolução de conflito):
1. Criar ou atualizar um ADR em `Instructions/decisions/`
2. Usar o template em `Instructions/decisions/adr-template.md`
3. Incluir: contexto, decisão, alternativas consideradas, trade-offs e consequências
4. Referenciar o ADR nos arquivos de arquitetura afetados

---

## Separação de Governança Técnica e de Negócio

- `Instructions/architecture/` contém **apenas** governança técnica: princípios de engenharia, padrões, restrições estruturais, decisões tecnológicas.
- `Instructions/business/` contém **apenas** governança de negócio: regras de negócio, invariantes, workflows, modelo de domínio.
- **Regras de negócio não pertencem a arquivos de arquitetura**.
- **Decisões arquiteturais não pertencem a arquivos de negócio**.
- Quando uma decisão arquitetural for motivada por requisito de negócio, registrar a motivação no ADR mas manter os artefatos separados.

---

## Avaliação de Impacto Arquitetural

Antes de qualquer implementação que possa afetar a arquitetura:
1. Verificar se a solicitação está coberta pelos princípios existentes
2. Verificar se a solicitação conflita com restrições registradas
3. Verificar se a solicitação introduz nova dependência estrutural
4. Se alguma restrição for violada ou novo princípio for necessário, registrar antes de implementar

---

## Regra Crítica: Arquitetura Não Invalida Negócio

**Preferência arquitetural não pode prevalecer sobre comportamento de negócio exigido.**

Se uma decisão arquitetural impede ou compromete o comportamento esperado do sistema conforme definido em BDD, regras de negócio ou contratos:
- A preferência arquitetural deve ser adaptada
- O comportamento de negócio deve ser preservado
- O trade-off deve ser documentado em ADR
- O conflito deve ser reportado

---

## Evolução da Arquitetura

- Decisões arquiteturais devem ser versionadas em `Instructions/decisions/`
- Mudanças arquiteturais significativas devem sempre gerar um novo ADR
- ADRs substituídos devem ser marcados como substituídos (não deletados), referenciando o ADR que os substitui
- O histórico de decisões é parte do conhecimento durável do repositório

---

## Relação com Outras Rules

- `governance-policies.md` — políticas de propagação (§3) e normalização (§1) usadas por esta rule
- `source-of-truth-priority.md` define que arquitetura está subordinada a BDD e regras de negócio quando há conflito
- `naming-governance.md` e `folder-governance.md` são subordinadas a esta rule para decisões estruturais
