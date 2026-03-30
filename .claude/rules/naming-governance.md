# Regra: Governança de Nomenclatura

## Classificação

Meta-governança (ponte entre domínio técnico e de negócio). Unifica terminologia de domínio (linguagem ubíqua — negócio) com convenções de nomenclatura técnica (código, rotas, tabelas).

## Propósito

Esta rule define como convenções de nomenclatura são introduzidas, registradas, aplicadas e propagadas neste repositório.

---

## Tipos de Nomenclatura

### Terminologia de Negócio (Ubiquitous Language)
- Nomes de conceitos, entidades, eventos e ações do domínio
- Definidos pelo negócio, não pela tecnologia
- Registrados em `Instructions/glossary/ubiquitous-language.md`
- **Têm prioridade** sobre nomenclatura técnica na camada de domínio
- Devem ser consistentes entre: código de domínio, BDD, contratos, glossário e documentação

### Nomenclatura Técnica
- Nomes de arquivos, pastas, classes, funções, variáveis, rotas, tabelas, tópicos, filas
- Definida pelas convenções técnicas do repositório
- Registrada em `Instructions/architecture/naming-conventions.md`
- Subordinada à terminologia de negócio quando há conflito na camada de domínio

---

## Como Introduzir Novas Convenções de Nomenclatura

Quando o usuário introduzir uma nova convenção de nomenclatura:
1. Identificar o escopo: é terminologia de negócio ou convenção técnica?
2. Verificar se conflita com convenções existentes
3. Se for terminologia de negócio → atualizar `Instructions/glossary/ubiquitous-language.md`
4. Se for convenção técnica → atualizar `Instructions/architecture/naming-conventions.md`
5. Verificar impacto em artefatos existentes que usam a nomenclatura antiga
6. Propagar a mudança quando necessário (ver `governance-policies.md` §3)

---

## Como Registrar Convenções de Nomenclatura

Para cada convenção registrada em `Instructions/architecture/naming-conventions.md`, incluir:
- Escopo de aplicação (arquivos, classes, funções, rotas, tabelas, eventos, etc.)
- Padrão adotado (ex: PascalCase, snake_case, kebab-case, prefixos, sufixos)
- Exemplos corretos
- Anti-exemplos (o que não usar)
- Motivo ou referência à decisão

---

## Como Propagar Mudanças de Nomenclatura

Quando uma convenção de nomenclatura for alterada:
1. Identificar todos os artefatos que usam a nomenclatura antiga
2. Decidir se a mudança é retroativa (exige atualização imediata de todos os artefatos) ou prospectiva (aplica apenas a novos artefatos)
3. Se retroativa: criar lista de artefatos a atualizar, propagar de forma consistente
4. Atualizar contratos se a nomenclatura de campos ou rotas for alterada (impacto em interface pública)
5. Atualizar BDD se os cenários mencionam nomes afetados
6. Atualizar glossário se a terminologia de domínio for afetada
7. Reportar o escopo da mudança no relatório final

---

## Consistência Terminológica

### Regras obrigatórias de consistência:
- O mesmo conceito de domínio deve usar o mesmo nome em todo o repositório: código, BDD, contratos, glossário, documentação, mensagens de erro.
- Sinônimos são permitidos apenas quando registrados explicitamente no glossário como equivalentes.
- Abreviações são permitidas apenas quando registradas no glossário.
- Nomes ambíguos que já causaram confusão devem ser registrados como **proibidos** no glossário.

### Como evitar deriva de vocabulário:
- Quando o usuário usar um termo novo para se referir a um conceito existente, verificar se é sinônimo ou conceito novo.
- Se for sinônimo → registrar no glossário como sinônimo permitido ou proibido, dependendo da clareza.
- Se for conceito novo → registrar como novo termo no glossário e atualizar o modelo de domínio.
- Se houver dúvida → registrar em `open-questions.md` e reportar.

---

## Nomenclatura em Artefatos Específicos

### Contratos (APIs, mensagens, schemas)
- Campos de contrato devem usar a terminologia de negócio sempre que possível.
- Convenções de serialização (ex: camelCase em JSON) podem divergir da nomenclatura interna, mas devem ser documentadas.
- Mudanças em nomenclatura de campos de contrato têm impacto em dependentes externos — tratar com cuidado.

### BDD (cenários Gherkin)
- Nomes de cenários devem usar linguagem de negócio, não termos técnicos.
- Passos Given/When/Then devem usar terminologia do glossário.

### Banco de dados
- Nomes de tabelas e colunas devem ser documentados em `Instructions/architecture/naming-conventions.md`.
- Diferenças entre nomenclatura de domínio e nomenclatura de banco devem ser mapeadas explicitamente.

---

## Relação com Outras Rules

- `source-of-truth-priority.md` — terminologia de negócio prevalece sobre convenção técnica em conflito
- `governance-policies.md` — políticas de propagação (§3) e classificação de snippets (§5) usadas por esta rule
- `folder-governance.md` — nomes de pastas seguem as convenções registradas aqui
