# Regra: Tratamento de Ambiguidades

## Propósito

Esta rule define como o assistente deve detectar, classificar, registrar e resolver dúvidas, ambiguidades e lacunas de informação antes e durante a implementação.

---

## Princípio Fundamental

> Implementação insegura é pior do que implementação pausada.
> Quando houver dúvida material, registre, reporte e aguarde confirmação.
> Quando a governança já permitir uma premissa mínima conservadora, execute com registro.

---

## Classificação de Ambiguidades

### Ambiguidade Pequena (não bloqueante)
Dúvidas que **não alteram** comportamento funcional, arquitetura, contrato, segurança, persistência, mensageria, nomenclatura relevante ou experiência do usuário.

**Tratamento**: Resolva com premissa mínima conservadora. Registre em `assumptions-log.md`. Não bloqueie a implementação.

**Exemplos**:
- Nome de variável interna de um método sem impacto externo
- Organização interna de um arquivo sem impacto em interfaces
- Ordem de campos em uma struct sem impacto em serialização

### Ambiguidade Material (potencialmente bloqueante)
Dúvidas que afetam **comportamento funcional, regra de negócio, contratos, BDD, arquitetura, segurança, integração, mensageria, persistência, modelagem de dados ou impacto estrutural**.

**Tratamento**: Registre em `open-questions.md`. Reporte no prompt com a dúvida, o impacto e o que precisa ser confirmado. **Não implemente a parte ambígua**.

**Exemplos**:
- Comportamento esperado quando uma entidade não existe (retornar 404, criar, ou erro de negócio?)
- Regra de validação ausente que afeta persistência
- Nomenclatura de campo em contrato público
- Fluxo alternativo em BDD não especificado

---

## Workflow Obrigatório

```
1. Após normalizar a intenção do usuário, verificar:
   - Há lacunas de informação que impedem implementação correta?
   - Há múltiplas interpretações válidas com consequências diferentes?
   - A governança existente já cobre esta situação?

2. Se a governança cobre → implementar com base na governança, sem bloqueio

3. Se há ambiguidade pequena → adotar premissa mínima, registrar em assumptions-log.md, implementar

4. Se há ambiguidade material:
   a. Registrar em open-questions.md com: id, data, resumo, dúvida, impacto, artefatos afetados, se é bloqueante
   b. Verificar se alguma parte da solicitação pode ser executada com segurança
   c. Responder no prompt: intenção interpretada + dúvidas encontradas + o que precisa ser confirmado + o que pode ser feito agora
   d. Aguardar confirmação antes de implementar a parte bloqueante

5. Quando o usuário esclarecer em mensagem posterior:
   a. Remover a dúvida da lista ativa em open-questions.md
   b. Atualizar ou remover premissas relacionadas em assumptions-log.md
   c. Consolidar a resolução nos arquivos definitivos do repositório
   d. Implementar a parte que estava pendente
```

---

## Regras para `open-questions.md`

- Contém **apenas** dúvidas e ambiguidades **ainda abertas**.
- Dúvidas resolvidas **devem ser removidas** da lista ativa imediatamente após resolução.
- A resolução deve ser refletida nos arquivos definitivos do repositório (regras de negócio, arquitetura, contratos, BDD etc.).
- Se rastreabilidade histórica for necessária, registre a resolução em log de histórico — mas **não mantenha como pendência aberta**.

---

## Regras para `assumptions-log.md`

- Contém **apenas** premissas **ainda ativas** ou ainda não confirmadas.
- Premissas confirmadas pelo usuário devem ser consolidadas na governança definitiva e **removidas do log ativo**.
- Premissas invalidadas devem ser **removidas ou marcadas como descartadas** — não devem permanecer como regra ativa.
- Toda premissa deve ter: contexto, artefatos impactados, nível de risco e se precisa de confirmação posterior.

---

## Divisão Explícita de Implementação

Quando parte da solicitação puder ser executada com segurança e outra parte depender de esclarecimento, o assistente deve:

1. Deixar essa divisão **explícita na resposta**
2. Executar o que for seguro
3. Registrar o que está bloqueado e por quê
4. Aguardar confirmação antes de implementar o que está bloqueado

---

## Política de Premissas

- Toda premissa relevante usada sem confirmação explícita **deve ser registrada** em `assumptions-log.md`.
- Premissas devem ser **conservadoras** — quando houver dúvida, adote o comportamento menos destrutivo e mais reversível.
- Nunca adote premissas que inventem regras de negócio, restrições de segurança ou comportamento de integração ausentes.
- Uma premissa é válida apenas enquanto não for contradita pela governança ou pelo usuário.

---

## Relação com Outras Rules

- `natural-language-normalization.md` vem antes desta rule — normalizar é pré-requisito para detectar ambiguidade.
- `implementation-alignment.md` usa esta rule para decidir se pode avançar para implementação.
- `source-of-truth-priority.md` orienta como resolver ambiguidades quando múltiplas fontes conflitam.
- `change-propagation.md` deve ser consultada quando a resolução de uma ambiguidade afeta múltiplos artefatos.
