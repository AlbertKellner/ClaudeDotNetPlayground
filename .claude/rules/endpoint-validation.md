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

Para cada endpoint identificado no Passo 1, executar a chamada HTTP capturando o body completo e o status code:

#### Endpoint sem autenticação:
```bash
RESPONSE=$(curl -s -w "\n%{http_code}" \
  -X <MÉTODO> http://localhost:8080<ROTA>)
HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | sed '$d')
echo "Status: $HTTP_CODE"
echo "$BODY"
```

#### Endpoint com autenticação:
```bash
RESPONSE=$(curl -s -w "\n%{http_code}" \
  -X <MÉTODO> http://localhost:8080<ROTA> \
  -H "Authorization: Bearer <TOKEN>")
HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | sed '$d')
echo "Status: $HTTP_CODE"
echo "$BODY"
```

#### Endpoint com body (POST/PUT/PATCH):
```bash
RESPONSE=$(curl -s -w "\n%{http_code}" \
  -X POST http://localhost:8080<ROTA> \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <TOKEN>" \
  -d '<PAYLOAD_JSON>')
HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | sed '$d')
echo "Status: $HTTP_CODE"
echo "$BODY"
```

### Passo 3.1: Capturar logs de storytelling da requisição

Imediatamente após cada chamada HTTP (Passo 3), capturar os logs do container correspondentes à requisição executada:

```bash
docker logs $(docker compose ps -q app) --tail 30 2>&1
```

Os logs capturados devem ser filtrados para incluir apenas as linhas correspondentes à requisição validada, identificáveis pelo CorrelationId presente na resposta ou pela proximidade temporal (logs emitidos entre a chamada e a captura). Estes logs serão apresentados no relatório (Passo 5, item 4).

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
- Se token foi gerado: confirmar geração bem-sucedida
- Resultado geral: todos aprovados / algum reprovado (e qual)

**Para cada endpoint validado com sucesso**, incluir obrigatoriamente:
1. **Status code** da requisição (ex: `200`)
2. **Endpoint completo** com método HTTP, URL e todos os parâmetros, headers e body utilizados na chamada (ex: `GET http://localhost:8080/weather-conditions` com header `Authorization: Bearer <token>`)
3. **JSON completo** retornado pelo endpoint, formatado como bloco de código Markdown JSON para facilitar a visualização:

```json
{
  "exemplo": "resposta completa do endpoint"
}
```

4. **Logs de storytelling da requisição** capturados do container da aplicação imediatamente após a chamada ao endpoint (Passo 3.1). Os logs devem ser apresentados como bloco de código Markdown, filtrados para mostrar apenas as linhas correspondentes à requisição validada (identificáveis pelo CorrelationId ou pela proximidade temporal). O usuário deve poder verificar visualmente que o padrão SNP-001 (storytelling por classe e método) está sendo seguido:
   - Prefixo `[NomeDaClasse][NomeDoMétodo]` presente em cada linha de log
   - Logs de entrada e saída de cada método visíveis
   - Sequência narrativa coerente do fluxo da requisição

```log
[20/03/2026 14:30:00.0000000] [correlation-id] [UserName] [Controller][Action] Processar requisição...
[20/03/2026 14:30:00.0000001] [correlation-id] [UserName] [UseCase][Method] Executar lógica...
...
```

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
- `governance-policies.md` §3 — se a validação revelar comportamento incorreto, propagar a correção antes de prosseguir

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-17 | Criado: rule de validação obrigatória de endpoints após implementação de feature | Instrução do usuário |
| 2026-03-20 | Adicionado: relatório de sucesso deve incluir status code, endpoint completo com parâmetros e JSON completo da resposta formatado em Markdown | Instrução do usuário |
| 2026-03-20 | Adicionado: Passo 3.1 (captura de logs por requisição) e item 4 no Passo 5 (logs de storytelling obrigatórios no relatório, com verificação visual do padrão SNP-001) | Instrução do usuário |
