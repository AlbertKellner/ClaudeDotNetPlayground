# Regra: Tratamento de Snippets e Trechos Técnicos

## Propósito

Esta rule define como o assistente deve classificar, tratar e persistir todo trecho técnico enviado pelo usuário — seja código, configuração, schema, YAML, JSON, SQL, Terraform, Helm, políticas IAM, definições de mensageria, contratos ou qualquer outro fragmento técnico.

---

## Classificação Obrigatória

Todo trecho técnico enviado pelo usuário deve ser classificado em uma das categorias abaixo **antes** de qualquer ação:

### 1. Snippet Normativo
**Quando usar**: O usuário deixa explícito que o trecho deve ser incluído na íntegra, copiado, preservado exatamente, aplicado literalmente, inserido sem reescrita, tratado como canônico ou equivalente.

**Sinais de reconhecimento**:
- "inclua exatamente assim"
- "copie isso"
- "use exatamente este trecho"
- "mantenha esse código"
- "não altere isso"
- "quero que fique assim"
- "preserve isso"
- "é pra ser literalmente esse"

**Tratamento obrigatório**:
- Copiar na íntegra para o local apropriado do projeto
- Respeitar apenas ajustes estritamente necessários de encaixe estrutural (ex: indentação de arquivo de destino)
- **Não** "melhorar", "otimizar", "refatorar", "embelezar" ou reinterpretar o conteúdo
- Reportar qualquer adaptação mínima feita com justificativa explícita
- Se conflitar com estrutura existente, regras de segurança, contratos ou limitações técnicas: registrar o conflito, explicar o impacto e **não substituir silenciosamente**

### 2. Exemplo Ilustrativo
**Quando usar**: O usuário deixa explícito que o trecho é apenas exemplo, referência, inspiração, base, direção ou "algo nessa linha".

**Sinais de reconhecimento**:
- "algo assim"
- "tipo isso"
- "como exemplo"
- "inspirado nisto"
- "nessa linha"
- "algo parecido com"
- "baseado nisso"

**Tratamento obrigatório**:
- Não copiar literalmente por obrigação
- Interpretar à luz do contexto do projeto
- Adaptar, reestruturar, renomear ou reescrever conforme governança, arquitetura e estilo do repositório
- Preservar a **intenção técnica**, não a literalidade
- Reportar como foi adaptado

### 3. Padrão Preferencial
**Quando usar**: O usuário indica que quer seguir aquela abordagem, mas sem exigir cópia literal.

**Sinais de reconhecimento**:
- "prefiro usar esse padrão"
- "quero seguir essa estrutura"
- "tente seguir essa abordagem"
- "use esse estilo"

**Tratamento obrigatório**:
- Manter a abordagem, estrutura e filosofia do trecho
- Adaptações são permitidas desde que a intenção principal permaneça reconhecível
- Reportar divergências relevantes

### 4. Referência Técnica Contextual
**Quando usar**: O trecho serve para mostrar intenção técnica, esclarecer comportamento ou contextualizar um requisito — não é para ser aplicado diretamente.

**Sinais de reconhecimento**:
- "para você entender o que quero"
- "isso aqui é só pra contextualizar"
- "estou mostrando como funciona hoje"
- "veja como está atualmente"

**Tratamento obrigatório**:
- Usar como contexto complementar
- Não copiar literalmente
- Pode influenciar classificação, desenho, validação ou implementação
- Não requer registro em snippets canônicos

---

## Regra de Classificação Padrão

Quando o usuário **não indicar claramente** a natureza do trecho:

1. Inferir com cautela com base na formulação usada
2. Na ausência de sinal claro de literalidade obrigatória → assumir **ilustrativo ou preferencial**, **não normativo**
3. Se houver dúvida material sobre tratar o trecho como normativo ou ilustrativo de forma irreversível → registrar a ambiguidade em `open-questions.md` antes de implementar

---

## Persistência de Snippets Canônicos

Quando um snippet normativo for relevante para governança futura, registrá-lo em `Instructions/snippets/canonical-snippets.md` com:
- id único
- data
- título
- intenção
- resumo da instrução do usuário
- classificação
- escopo de aplicação
- regra de preservação
- adaptações mínimas permitidas
- artefatos relacionados
- conteúdo do snippet

**Regras para snippets canônicos registrados**:
- Devem ser respeitados em futuras implementações
- Não devem ser reescritos livremente
- Qualquer alteração exige nova instrução explícita do usuário ou conflito técnico devidamente reportado
- Se um snippet canônico conflitar com nova governança, reportar o conflito antes de resolver

---

## Relatório Obrigatório

Ao final de qualquer implementação que envolva trechos do usuário, reportar:
- Quais trechos foram classificados como normativos e onde foram aplicados na íntegra
- Quais trechos foram classificados como ilustrativos e como foram adaptados
- Quais trechos foram classificados como preferenciais e quais adaptações foram feitas
- Quais adaptações mínimas foram feitas em snippets normativos e por quê
- Quais conflitos foram encontrados e como foram tratados

---

## Relação com Outras Rules

- `natural-language-normalization.md` separa trechos técnicos do texto narrativo durante a normalização.
- `implementation-alignment.md` usa a classificação de snippets para decidir como aplicá-los.
- `ambiguity-handling.md` orienta o tratamento quando há dúvida sobre a classificação do trecho.
- `source-of-truth-priority.md` define que snippets normativos declarados canônicos têm a mais alta prioridade.
