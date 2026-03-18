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
