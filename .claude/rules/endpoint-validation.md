# Regra: Validação de Endpoint após Implementação de Feature

## Propósito

Esta rule define o comportamento obrigatório do assistente ao concluir qualquer implementação de feature que exponha um endpoint HTTP. A validação deve ocorrer com a aplicação em execução, antes do commit, como parte do pipeline de validação pré-commit.

---

## Princípio Fundamental

> Código que compila mas não responde corretamente não está pronto.
> Todo endpoint implementado deve ser validado via chamada HTTP real antes de qualquer commit.
> Se o endpoint exigir autenticação, o token deve ser obtido programaticamente — não manualmente.

---

## Quando Esta Rule Se Aplica

Esta rule é ativada toda vez que a implementação criar ou alterar uma feature que possua endpoint HTTP acessível, incluindo:
- Novo endpoint criado (qualquer método HTTP: GET, POST, PUT, DELETE, PATCH)
- Endpoint existente com comportamento alterado (mudança de contrato, regra de negócio, validação)
- Endpoint com nova rota ou método HTTP adicionado

**Não se aplica** a:
- Mudanças puramente internas sem impacto no contrato ou comportamento do endpoint
- Mudanças apenas em arquivos de governança, documentação ou testes unitários
- Refatorações que preservam comportamento observável externamente (verificado por análise)

---

## Posição no Pipeline de Validação Pré-Commit

O passo de validação de endpoint é inserido após o health check e antes de exibir logs finais:

```
0. Verificar pré-requisitos de ambiente
1. dotnet build
2. docker compose up -d
3. Aguardar /health responder HTTP 200
4. [ESTA RULE] Validar endpoints das features implementadas ou alteradas
5. Exibir logs do container da aplicação
6. docker compose down
7. Realizar o commit
```

---

## Workflow Obrigatório

### Passo 1: Identificar endpoints afetados

Listar todos os endpoints criados ou alterados na tarefa atual:
- Rota (ex: `GET /weather-conditions`)
- Método HTTP
- Se requer autenticação (verificar `[Authenticate]` no Controller ou equivalente)
- Payload esperado (se POST/PUT/PATCH)
- Status code esperado para o caso de sucesso

### Passo 2: Obter token de autenticação (quando necessário)

Se **qualquer** endpoint da lista requer autenticação (RN-003), obter um Bearer Token antes de consumir os endpoints protegidos:

```bash
curl -s -X POST http://localhost:8080/login \
  -H "Content-Type: application/json" \
  -d '{"userName": "<usuario>", "password": "<senha>"}' \
  | grep -o '"token":"[^"]*"' | cut -d'"' -f4
```

**Credenciais**: usar as credenciais registradas nos usuários hardcoded da aplicação (conforme implementação de RN-002). Se não houver usuário registrado ou a senha não for conhecida, verificar o código de `UserLoginUseCase` para obter as credenciais vigentes.

O token obtido deve ser armazenado em variável e usado em todas as chamadas subsequentes autenticadas da mesma sessão de validação.

### Passo 3: Consumir cada endpoint

Para cada endpoint identificado no Passo 1, executar a chamada HTTP e verificar o resultado:

#### Endpoint sem autenticação:
```bash
curl -s -o /dev/null -w "%{http_code}" \
  -X <MÉTODO> http://localhost:8080<ROTA>
```

#### Endpoint com autenticação:
```bash
curl -s -o /dev/null -w "%{http_code}" \
  -X <MÉTODO> http://localhost:8080<ROTA> \
  -H "Authorization: Bearer <TOKEN>"
```

#### Endpoint com body (POST/PUT/PATCH):
```bash
curl -s -o /dev/null -w "%{http_code}" \
  -X POST http://localhost:8080<ROTA> \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <TOKEN>" \
  -d '<PAYLOAD_JSON>'
```

### Passo 4: Verificar resultado

Para cada chamada, verificar:
- **Status code**: deve corresponder ao esperado (normalmente 200 para sucesso)
- **Body**: se o endpoint retorna corpo, verificar que a estrutura básica é a esperada

Se o status code retornado **não** corresponder ao esperado:
1. Exibir os logs do container: `docker logs <container-name> --tail 50`
2. Registrar o erro em `bash-errors-log.md` com o comando executado e o resultado obtido
3. **Não prosseguir para o commit** — corrigir o problema antes

### Passo 5: Reportar resultado da validação

No relatório final da tarefa, incluir obrigatoriamente:
- Lista de endpoints validados
- Status code obtido em cada chamada
- Se token foi gerado: confirmar geração bem-sucedida
- Resultado: todos aprovados / algum reprovado (e qual)

---

## Casos Especiais

### Endpoint retorna 401 inesperado
- Verificar se o token foi gerado corretamente (não expirado, não corrompido)
- Verificar se o endpoint realmente possui `[Authenticate]`
- Verificar se o `AuthenticateFilter` está registrado corretamente

### Endpoint retorna 500
- Exibir logs completos do container antes de qualquer outra ação
- Registrar em `bash-errors-log.md`
- Não realizar commit — investigar e corrigir

### Endpoint retorna 404 inesperado
- Verificar se a rota está registrada corretamente no Controller
- Verificar se o Controller foi registrado no DI/pipeline do ASP.NET Core

### Token não pode ser gerado (POST /login retorna não-200)
- Verificar credenciais usadas
- Verificar implementação de `UserLoginUseCase`
- Registrar em `bash-errors-log.md`

---

## Porta da Aplicação

A aplicação expõe HTTP na porta `8080` do host quando executada via `docker compose`.
Todas as chamadas de validação devem usar `http://localhost:8080` como base URL.

---

## Relação com Outras Rules

- `environment-readiness.md` — a aplicação deve estar em execução (passo 2 e 3 do pipeline) antes desta rule ser ativada
- `bash-error-logging.md` — erros de chamada HTTP devem ser registrados neste log
- `implementation-alignment.md` — esta rule compõe o passo 9 (implementar) e o passo 10 (relatar) do workflow de implementação
- `change-propagation.md` — se a validação revelar comportamento incorreto, propagar a correção antes de prosseguir

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-17 | Criado: rule de validação obrigatória de endpoints após implementação de feature | Instrução do usuário |
