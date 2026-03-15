# Regra: Propagação de Mudanças

## Propósito

Esta rule define como o assistente deve avaliar e propagar o impacto de mudanças entre artefatos relacionados do repositório. Toda mudança relevante deve ser analisada transversalmente antes de ser considerada concluída.

---

## Princípio Fundamental

> Nenhuma mudança existe isolada.
> Antes de concluir qualquer implementação, avalie o impacto em artefatos relacionados.
> Consistência entre artefatos é responsabilidade do assistente, não do usuário.

---

## Mapa de Propagação Obrigatório

### Se Negócio Muda (regras, invariantes, fluxos, modelo de domínio)
Avaliar obrigatoriamente:
- [ ] BDD: há cenários que dependem dessa regra? Devem ser atualizados?
- [ ] Contratos: há contratos que formalizam esse comportamento? Devem ser atualizados?
- [ ] Glossário: há termos novos ou alterados que devem ser registrados?
- [ ] Implementação: há código ou artefatos declarativos que implementam esse comportamento?
- [ ] Workflows: há fluxos relacionados que devem ser atualizados?
- [ ] Invariants: há invariantes que devem ser revistos?

### Se Contratos Mudam (OpenAPI, AsyncAPI, schemas, payloads)
Avaliar obrigatoriamente:
- [ ] Negócio: a mudança de contrato reflete mudança de regra de negócio? Atualizar `business-rules.md`
- [ ] BDD: há cenários que testam esse contrato? Devem ser atualizados?
- [ ] Glossário: há campos ou tipos com terminologia nova?
- [ ] Implementação: há código de cliente ou servidor que implementa este contrato?
- [ ] Dúvidas abertas: a mudança resolve alguma dúvida aberta? Atualizar `open-questions.md`
- [ ] **Atenção**: contratos com dependentes externos exigem cuidado especial com versionamento

### Se BDD Muda (cenários comportamentais)
Avaliar obrigatoriamente:
- [ ] Negócio: o BDD atualizado reflete uma regra de negócio que deve ser formalizada?
- [ ] Contratos: o comportamento especificado exige formalização de contrato?
- [ ] Implementação: há código que implementa esses cenários? Está alinhado?
- [ ] Glossário: há novos termos de domínio nos cenários?

### Se Arquitetura Muda (princípios, padrões, decisões tecnológicas)
Avaliar obrigatoriamente:
- [ ] Instruções técnicas: `technical-overview.md`, `engineering-principles.md`, `patterns.md` estão alinhados?
- [ ] Organização de pastas: `folder-structure.md` deve ser atualizado?
- [ ] Convenções de nomenclatura: `naming-conventions.md` é afetado?
- [ ] ADRs: a mudança exige novo ADR ou atualização de ADR existente?
- [ ] Implementação: há código que precisa ser refatorado para seguir a nova arquitetura?

### Se Nomenclatura Muda (terminologia de negócio ou convenção técnica)
Avaliar obrigatoriamente:
- [ ] Glossário: `ubiquitous-language.md` atualizado?
- [ ] BDD: cenários usam o nome antigo?
- [ ] Contratos: campos de contrato usam o nome antigo?
- [ ] Implementação: código usa o nome antigo?
- [ ] Documentação e instruções: arquivos de governança usam o nome antigo?

### Se Snippet Canônico Muda
Avaliar obrigatoriamente:
- [ ] Implementações que usam este snippet: estão desatualizadas?
- [ ] Referências no repositório: há outros artefatos que referenciam este snippet?
- [ ] Contratos ou configurações derivadas do snippet: devem ser atualizados?

---

## Workflow de Propagação

```
1. Identificar a mudança e seu tipo (negócio, contrato, BDD, arquitetura, nomenclatura, snippet)

2. Aplicar o mapa de propagação correspondente

3. Para cada item do mapa:
   ├── Verificar se o artefato existe no repositório
   ├── Se existe: avaliar se precisa de atualização
   └── Se não existe: avaliar se deve ser criado

4. Priorizar as atualizações:
   ├── Governança primeiro (regras, definições, princípios)
   ├── Contratos e BDD depois (especificações formais)
   └── Implementação por último (código e artefatos declarativos)

5. Reportar no resultado final:
   - Quais artefatos foram avaliados para propagação
   - Quais foram atualizados
   - Quais foram identificados como pendentes de atualização futura
   - Se houver impacto que não pode ser resolvido nesta interação, registrar em open-questions.md
```

---

## Limites da Propagação Automática

A propagação automática é esperada quando:
- O impacto é claro e não ambíguo
- A atualização é tecnicamente segura
- Não há risco de perda de informação

A propagação deve ser pausada e reportada quando:
- Há conflito entre a mudança e um artefato existente
- A atualização implicaria alterar um contrato com dependentes externos não mapeados
- A mudança afeta um snippet normativo registrado como canônico
- Há dúvida sobre a intenção da mudança em relação ao artefato afetado

---

## Pensamento Transversal Obrigatório

Antes de concluir qualquer mudança, o assistente deve se perguntar:
1. "Esta mudança afeta algum artefato que não está no meu plano atual?"
2. "Algum arquivo de governança ficou desatualizado com esta mudança?"
3. "Alguma dúvida aberta foi resolvida ou criada por esta mudança?"
4. "Alguma premissa ativa foi confirmada, invalidada ou criada por esta mudança?"

---

## Relação com Outras Rules

- `implementation-alignment.md` — a propagação é o passo 8 do workflow de implementação
- `business-ingestion.md` — define o que propagar quando negócio muda
- `technical-ingestion.md` — define o que propagar quando arquitetura muda
- `naming-governance.md` — define o que propagar quando nomenclatura muda
- `ambiguity-handling.md` — quando a propagação encontra conflito ou ambiguidade
