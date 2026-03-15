# Regra: Alinhamento para Implementação

## Propósito

Esta rule define o workflow obrigatório que o assistente deve seguir para toda solicitação de implementação — seja criação, alteração, exclusão ou revisão de qualquer artefato do repositório.

---

## Definição de "Implementação"

"Implementar" não se restringe a código de aplicação. Implementar significa materializar mudanças em:
- Código de aplicação (qualquer linguagem ou framework)
- Infraestrutura declarativa (Terraform, Helm, CloudFormation, Bicep, etc.)
- Contratos e interfaces (OpenAPI, AsyncAPI, schemas JSON/Avro/Protobuf, etc.)
- Mensageria (definições de tópicos, filas, exchanges, bindings, políticas)
- Banco como código (migrations, seeds, schemas de banco)
- Configuração (variáveis de ambiente, feature flags, parâmetros de sistema)
- Documentação operacional (quando a documentação governa comportamento)

---

## Workflow Obrigatório de Implementação

Execute esta sequência para **toda** solicitação de implementação:

### Passo 1: Interpretar semanticamente
- Normalizar a intenção do usuário (ver `natural-language-normalization.md`)
- Identificar o que precisa ser criado, alterado, removido ou revisado
- Identificar o escopo da mudança

### Passo 2: Classificar a solicitação
Determinar se é:
- Nova funcionalidade
- Alteração de comportamento existente
- Remoção de comportamento
- Implementação guiada por regra existente
- Atualização decorrente de nova definição

### Passo 3: Identificar e ler a governança relevante
Antes de implementar, consultar:
- `Instructions/business/business-rules.md` — regras de negócio relacionadas
- `Instructions/architecture/technical-overview.md` — contexto arquitetural
- `Instructions/bdd/` — cenários BDD relacionados
- `Instructions/contracts/` — contratos afetados
- `Instructions/glossary/ubiquitous-language.md` — terminologia correta
- `Instructions/architecture/naming-conventions.md` — convenções de nomenclatura
- `Instructions/snippets/canonical-snippets.md` — snippets normativos relacionados
- `open-questions.md` — dúvidas abertas que afetam esta implementação
- `assumptions-log.md` — premissas ativas relevantes

### Passo 4: Verificar ambiguidades e lacunas
Antes de codificar, verificar:
- Há lacunas de informação que impedem implementação segura? → ver `ambiguity-handling.md`
- Há conflito entre fontes de verdade? → ver `source-of-truth-priority.md`
- A solicitação introduz ou altera alguma definição durável?

### Passo 5: Classificar trechos técnicos enviados pelo usuário
Se o usuário enviou trechos técnicos, classificá-los **antes de implementar** (ver `snippet-handling.md`):
- Normativo → copiar na íntegra
- Ilustrativo → adaptar ao contexto
- Preferencial → seguir a abordagem
- Contextual → usar como apoio de entendimento

### Passo 6: Registrar dúvidas e premissas
- Registrar dúvidas materiais em `open-questions.md`
- Registrar premissas adotadas em `assumptions-log.md`
- Se dúvidas bloqueantes existem → reportar e aguardar confirmação

### Passo 7: Atualizar a governança primeiro
Se a mensagem introduz ou altera qualquer definição durável:
- Atualizar os arquivos de governança **antes** de qualquer mudança de código ou artefato
- A implementação deve seguir a governança atualizada, não a antiga

**"Nova definição durável" inclui**:
- Nova regra de negócio ou alteração de existente
- Nova restrição ou exceção
- Novo fluxo ou conceito de domínio
- Novo ou alterado invariante
- Novo ou alterado contrato
- Novo cenário BDD
- Mudança de comportamento esperado
- Nova convenção de nomenclatura
- Nova regra de organização de pastas
- Nova regra arquitetural ou princípio técnico
- Ambiguidade resolvida que deva persistir como conhecimento
- Snippet canônico que deva governar futuras implementações

### Passo 8: Avaliar necessidade de propagação
Verificar se a mudança afeta artefatos relacionados (ver `change-propagation.md`):
- Mudança em negócio → avaliar BDD, contratos, glossário, implementação
- Mudança em contrato → avaliar negócio, BDD, implementação
- Mudança em BDD → avaliar negócio, contratos, implementação
- Mudança arquitetural → avaliar instruções técnicas, organização, implementação

### Passo 9: Implementar
Somente após os passos anteriores estarem completos:
- Implementar com base nas fontes de verdade do repositório
- Seguir convenções de nomenclatura e organização de pastas
- Aplicar snippets normativos na íntegra
- Adaptar exemplos ilustrativos conforme contexto

### Passo 10: Relatar
Ao concluir, reportar obrigatoriamente:
- **Intenção interpretada**: o que foi entendido como solicitação
- **Arquivos consultados**: quais arquivos de governança foram lidos
- **Arquivos alterados**: quais arquivos foram criados ou modificados
- **Trechos classificados**: normativos preservados, ilustrativos adaptados, preferenciais seguidos
- **Governança atualizada**: o que foi atualizado antes da implementação
- **Premissas adotadas**: quais premissas foram usadas sem confirmação explícita
- **Conflitos encontrados**: se houve conflito e como foi resolvido
- **Dúvidas registradas**: novas dúvidas abertas
- **Dúvidas resolvidas**: dúvidas anteriores resolvidas por esta mensagem
- **Fonte de verdade ativa**: o que passa a valer como referência após esta interação

---

## Política de Governança Primeiro

Esta é uma regra absoluta:

> Se a mensagem do usuário introduzir ou alterar uma definição durável, a governança deve ser atualizada **antes** de qualquer mudança de código, infraestrutura declarativa, contratos, artefatos de mensageria, banco como código ou outros artefatos do repositório.

Se a implementação já estiver totalmente coberta pela governança existente, o assistente pode implementar diretamente, mas ainda deve validar se a governança continua alinhada após a implementação.

---

## Relação com Outras Rules

- `natural-language-normalization.md` — executada antes desta rule
- `ambiguity-handling.md` — executada no passo 4 e 6
- `snippet-handling.md` — executada no passo 5
- `source-of-truth-priority.md` — consultada nos passos 4 e 7
- `change-propagation.md` — executada no passo 8
- `business-ingestion.md` e `technical-ingestion.md` — executadas no passo 7 quando há nova definição
