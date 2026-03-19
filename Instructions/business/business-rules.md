# Regras de Negócio

## Propósito

Este arquivo registra as regras de negócio formalizadas deste repositório. Regras de negócio definem o que o sistema deve ou não deve fazer — são a fonte de verdade para comportamento esperado.

**Prioridade**: Regras de negócio prevalecem sobre preferências arquiteturais quando houver conflito.

---

## Como Ler Este Arquivo

Cada regra segue a estrutura:
- **Id**: identificador único (RN-NNN)
- **Título**: nome curto e descritivo
- **Enunciado**: a regra em linguagem de negócio clara
- **Condição**: quando a regra se aplica
- **Ação**: o que deve acontecer quando a condição é satisfeita
- **Exceções**: casos em que a regra não se aplica
- **Dependências**: outras regras ou invariantes relacionados
- **BDD relacionado**: cenários que especificam comportamento desta regra
- **Contrato relacionado**: contratos que formalizam esta regra
- **Workflows relacionados**: fluxos que implementam esta regra
- **Status**: Ativo | Substituído | Depreciado

---

## Regras Ativas

### RN-001 — Endpoint de verificação de disponibilidade da aplicação
**Enunciado**: A aplicação deve expor um endpoint de verificação de disponibilidade que retorna a string "funcionando".
**Condição**: Quando uma requisição GET é recebida no endpoint de teste.
**Ação**: O sistema retorna HTTP 200 com o corpo contendo a string `"funcionando"`.
**Exceções**: Nenhuma.
**Dependências**: Nenhuma.
**BDD relacionado**: Nenhum no momento.
**Contrato relacionado**: Nenhum no momento.
**Workflows relacionados**: Nenhum.
**Status**: Ativo

---

### RN-002 — Autenticação de usuário via login com credenciais
**Enunciado**: A aplicação deve expor um endpoint de login que valida as credenciais do usuário e retorna um Bearer Token JWT quando as credenciais são válidas.
**Condição**: Quando uma requisição POST é recebida no endpoint de login com `userName` e `password`.
**Ação**: O sistema valida as credenciais contra a lista de usuários registrada. Se válidas, retorna HTTP 200 com um JWT contendo as propriedades `id` e `userName` do usuário. Se inválidas, retorna HTTP 401 com corpo em formato Problem Details.
**Exceções**: O endpoint de login em si não exige autenticação.
**Dependências**: Nenhuma.
**BDD relacionado**: Nenhum no momento.
**Contrato relacionado**: Nenhum no momento.
**Workflows relacionados**: Nenhum.
**Status**: Ativo

---

### RN-003 — Proteção de endpoints por Bearer Token
**Enunciado**: Toda requisição a endpoints da aplicação, exceto o endpoint de login, deve apresentar um Bearer Token JWT válido no header `Authorization`.
**Condição**: Quando uma requisição é recebida em qualquer endpoint que não seja o de login.
**Ação**: O sistema valida o Bearer Token. Se válido, a requisição prossegue normalmente e as propriedades `id` e `userName` do token são enriquecidas automaticamente nos logs da requisição. Se inválido ou ausente, o sistema retorna HTTP 401 com corpo em formato Problem Details.
**Exceções**: Endpoint de login (`POST /login`). Endpoint de health check (`GET /health`).
**Dependências**: RN-002.
**BDD relacionado**: Nenhum no momento.
**Contrato relacionado**: Nenhum no momento.
**Workflows relacionados**: Nenhum.
**Status**: Ativo

---

### RN-004 — Consulta de condições climáticas atuais de São Paulo
**Enunciado**: A aplicação deve expor um endpoint autenticado para consultar as condições climáticas atuais do município de São Paulo, retornando o payload completo retornado pela API Open-Meteo, sem filtragem, redução de campos ou mapeamento parcial. A resposta deve ser cacheada por usuário autenticado com duração configurável.
**Condição**: Quando uma requisição GET autenticada é recebida no endpoint de condições climáticas.
**Ação**: O sistema verifica o Memory Cache usando o ID do usuário autenticado como chave. Se houver cache válido, retorna a resposta cacheada sem consultar a API externa. Se não houver cache, consulta a API Open-Meteo (`GET /v1/forecast`) com as coordenadas centrais do município de São Paulo conforme indicadas pela Prefeitura de São Paulo (latitude: -23.5475, longitude: -46.6361) e os campos de condição atual definidos, armazena o resultado no cache e retorna HTTP 200 com o payload JSON completo da resposta da Open-Meteo, preservando sua estrutura original.
**Exceções**: Nenhuma.
**Dependências**: RN-003 (autenticação obrigatória).
**BDD relacionado**: Nenhum no momento.
**Contrato relacionado**: Nenhum no momento.
**Workflows relacionados**: Nenhum.
**Status**: Ativo

---

### Template de Regra de Negócio

```markdown
### RN-[número] — [Título]
**Enunciado**: [A regra em linguagem de negócio clara]
**Condição**: Quando [condição]
**Ação**: [O que deve acontecer]
**Exceções**: [Casos que não seguem a regra, se houver]
**Dependências**: [Outras regras ou invariantes]
**BDD relacionado**: [Referência a cenários BDD]
**Contrato relacionado**: [Referência a contratos]
**Workflows relacionados**: [Referência a fluxos]
**Status**: Ativo
```

---

### RN-005 — Health Check com verificação do Datadog Agent
**Enunciado**: O endpoint de health check da aplicação deve verificar a disponibilidade do Datadog Agent além da própria aplicação.
**Condição**: Quando uma requisição GET é recebida no endpoint `/health`.
**Ação**: O sistema verifica se o Datadog Agent está acessível via HTTP. Se ambos (aplicação e Datadog Agent) estiverem disponíveis, retorna HTTP 200 com corpo `Healthy`. Se o Datadog Agent responder com status inesperado, retorna HTTP 200 com corpo `Degraded`. Se o Datadog Agent estiver inacessível, retorna HTTP 503 com corpo `Unhealthy`.
**Exceções**: Nenhuma.
**Dependências**: RN-003 (o endpoint `/health` é exceção à autenticação).
**BDD relacionado**: Nenhum no momento.
**Contrato relacionado**: Nenhum no momento.
**Workflows relacionados**: Nenhum.
**Status**: Ativo

---

## Regras Substituídas ou Depreciadas

> Nenhuma regra substituída no momento.

---

## Dúvidas Abertas sobre Regras de Negócio

> Ver `open-questions.md` para dúvidas abertas relacionadas a regras de negócio.

---

## Referências Cruzadas

- `Instructions/business/invariants.md` — condições que nunca podem ser violadas
- `Instructions/business/workflows.md` — fluxos que implementam as regras
- `Instructions/business/domain-model.md` — entidades às quais as regras se aplicam
- `Instructions/bdd/` — cenários que especificam o comportamento das regras
- `Instructions/contracts/` — contratos que formalizam as regras

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| Bootstrap | Estrutura criada sem regras específicas | — |
| 2026-03-15 | RN-001 criada: endpoint TestGet retorna "funcionando" | Instrução do usuário |
| 2026-03-15 | RN-002 criada: endpoint de login com JWT retornando id e userName | Instrução do usuário |
| 2026-03-15 | RN-003 criada: proteção de endpoints por Bearer Token com enriquecimento de logs | Instrução do usuário |
| 2026-03-16 | RN-004 criada: consulta de condições climáticas de São Paulo via Open-Meteo, payload completo | Instrução do usuário |
| 2026-03-17 | RN-005 criada: health check inclui verificação do Datadog Agent via HTTP | Instrução do usuário |
