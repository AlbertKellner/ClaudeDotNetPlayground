# Regras de Negócio

Índice das regras de negócio implementadas na aplicação. Cada regra está documentada na página da feature correspondente.

---

## RN-001 — Verificação de disponibilidade da aplicação

A aplicação expõe um endpoint de verificação de disponibilidade que retorna o status de saúde da aplicação.

- **Endpoint:** `GET /health`
- **Autenticação:** não exigida
- **Comportamento:** retorna `HTTP 200` quando a aplicação está em funcionamento
- **Documentação completa:** [Feature: Health Check](Feature-Health)

---

## RN-002 — Autenticação de usuário via login

A aplicação expõe um endpoint de login que valida credenciais de usuário e retorna um Bearer Token JWT quando as credenciais são válidas.

- **Endpoint:** `POST /login`
- **Autenticação:** não exigida
- **Comportamento:** credenciais válidas → `HTTP 200` com token JWT; credenciais inválidas → `HTTP 401`
- **Documentação completa:** [Feature: Login de Usuário](Feature-UserLogin)
- **Componente de suporte:** [Autenticação JWT](Infra-Authentication)

---

## RN-003 — Proteção de endpoints por Bearer Token

Toda requisição a endpoints da aplicação, exceto o endpoint de login e o de health check, deve apresentar um Bearer Token JWT válido no header `Authorization`.

- **Exceções:** `POST /login` e `GET /health` são endpoints públicos
- **Comportamento:** token válido → requisição processada normalmente; token ausente ou inválido → `HTTP 401`
- **Documentação do endpoint protegido:** [Feature: Test Get](Feature-TestGet)
- **Documentação do mecanismo:** [Autenticação JWT](Infra-Authentication)

---

## RN-004 — Consulta de condições climáticas de São Paulo

A aplicação expõe um endpoint autenticado que retorna as condições climáticas atuais do município de São Paulo a partir da API Open-Meteo, preservando o payload completo da resposta sem filtragem ou mapeamento parcial.

- **Endpoint:** `GET /weather-conditions`
- **Autenticação:** exigida — Bearer Token JWT válido no header `Authorization`
- **Comportamento:** retorna `HTTP 200` com o payload completo da Open-Meteo; coordenadas de São Paulo e campos consultados são fixos na implementação
- **Documentação completa:** [Feature: Condições Climáticas de São Paulo](Feature-WeatherConditionsGet)

---

## RN-005 — Health Check com verificação do Datadog Agent

O endpoint de health check verifica a disponibilidade da aplicação e do Datadog Agent.

- **Endpoint:** `GET /health`
- **Autenticação:** não exigida
- **Comportamento:** retorna `HTTP 200 Healthy` quando ambos estão disponíveis; `HTTP 200 Degraded` quando o Datadog Agent responde com status inesperado; `HTTP 503 Unhealthy` quando o Datadog Agent está inacessível
- **Documentação completa:** [Feature: Health Check](Feature-Health)

---

## RN-006 — Busca de repositórios do team IntegrationRepos

A aplicação expõe um endpoint autenticado que consulta a API do GitHub para listar todos os repositórios acessíveis ao team IntegrationRepos da organização WebMotors, salvando o resultado em arquivo JSON.

- **Endpoint:** `GET /repositories`
- **Autenticação:** exigida — Bearer Token JWT válido no header `Authorization`
- **Comportamento:** consulta a API GitHub, mapeia os repositórios para um arquivo JSON com nome, descrição, URL Git, data de modificação e campo de sincronização em branco; cada repositório é registrado no log
- **BDD:** 7 cenários definidos em `Instructions/bdd/repositories-get-all.feature`
- **Documentação completa:** [Feature: Busca de Repositórios](Feature-RepositoriesGetAll)

---

## RN-007 — Sincronização local de repositórios

A aplicação expõe um endpoint autenticado que lê o arquivo JSON gerado pela busca e sincroniza localmente cada repositório via clone ou pull.

- **Endpoint:** `POST /repositories/sync`
- **Autenticação:** exigida — Bearer Token JWT válido no header `Authorization`
- **Comportamento:** para cada repositório no JSON, clona se ainda não existe ou executa pull se já clonado; atualiza o campo de última sincronização com data/hora no formato 24h; log info para sucesso, log error para falhas
- **BDD:** 11 cenários definidos em `Instructions/bdd/repositories-sync-all.feature`
- **Documentação completa:** [Feature: Sincronização de Repositórios](Feature-RepositoriesSyncAll)
