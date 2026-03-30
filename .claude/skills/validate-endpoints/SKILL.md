# Skill: validate-endpoints

## Propósito

Executar o workflow de validação de endpoints HTTP após implementação de feature, conforme a política definida em `.claude/rules/endpoint-validation.md`.

---

## Quando Usar

Esta skill é ativada pelo passo 6 do pipeline de validação pré-commit (CLAUDE.md), quando a tarefa criou ou alterou features com endpoint HTTP.

---

## Pré-requisitos

- Aplicação em execução via `docker compose` na porta `8080`
- `/health` já respondeu HTTP 200

---

## Workflow

### Passo 1: Identificar endpoints afetados

Listar todos os endpoints criados ou alterados na tarefa atual:
- Rota (ex: `GET /items`)
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

### Passo 3.2: Validar cache via segunda requisição consecutiva (quando aplicável)

**Quando se aplica**: sempre que o endpoint validado utilizar cache de requisição (Memory Cache). Endpoints com cache são identificados pela presença de um decorator `Cached*Client` na cadeia de dependências da Feature (ex: `Cached<Servico>ApiClient`, `Cached<Servico>ApiClient`).

**Endpoints com cache conhecidos**:
- `GET /items` — cache via `Cached<Servico>ApiClient`
- `GET /items/{id}` — cache via `Cached<Servico>ApiClient`

**Workflow**:

1. Imediatamente após a primeira requisição (Passo 3) e captura de logs (Passo 3.1), executar uma **segunda requisição idêntica** ao mesmo endpoint, com o mesmo token e mesmos parâmetros
2. Capturar o body completo e o status code da segunda requisição (mesmo procedimento do Passo 3)
3. Capturar os logs da segunda requisição (mesmo procedimento do Passo 3.1)

**Validações obrigatórias**:
- A segunda requisição deve retornar o **mesmo status code** da primeira (normalmente 200)
- A segunda requisição deve retornar **resposta válida** (estrutura consistente com a primeira)
- Os logs da **primeira requisição** devem conter evidência de **cache miss** (ex: `Cache miss. Consultar API externa`)
- Os logs da **segunda requisição** devem conter evidência de **cache hit** (ex: `Retornar resposta do cache`)

**Se a validação de cache falhar** (segunda requisição não retornar cache hit):
1. Exibir os logs de ambas as requisições
2. Registrar o erro em `bash-errors-log.md`
3. **Não prosseguir para o commit** — investigar a causa

**Reportar no relatório** (Passo 5): para endpoints com cache, incluir obrigatoriamente ambas as requisições (primeira e segunda), identificando qual foi cache miss e qual foi cache hit, com os respectivos logs de storytelling.

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
2. **Endpoint completo** com método HTTP, URL e todos os parâmetros, headers e body utilizados na chamada
3. **JSON completo** retornado pelo endpoint, formatado como bloco de código Markdown JSON
4. **Logs de storytelling da requisição na íntegra** — exibir os logs **completos e literais** capturados do container da aplicação (Passo 3.1), como bloco de código Markdown independente por requisição. **Não resumir, não parafrasear, não listar parcialmente.** Cada requisição HTTP executada deve ter seu próprio bloco de logs exibido. O bloco deve conter todas as linhas de log emitidas durante a requisição, filtradas pelo CorrelationId ou proximidade temporal. O usuário precisa verificar visualmente que o padrão SNP-001 (storytelling por classe e método) está sendo seguido:
   - Prefixo `[NomeDaClasse][NomeDoMétodo]` presente em cada linha de log
   - Logs de entrada e saída de cada método visíveis
   - Sequência narrativa coerente do fluxo da requisição

**Para endpoints com cache** (Passo 3.2), exibir obrigatoriamente:
5. **Logs da primeira requisição (cache miss)** — bloco de código Markdown com os logs completos da primeira chamada, contendo evidência de cache miss
6. **Logs da segunda requisição (cache hit)** — bloco de código Markdown separado com os logs completos da segunda chamada, contendo evidência de cache hit

---

## Arquivos de Governança Relacionados

- `.claude/rules/endpoint-validation.md` — política que este workflow implementa
- `.claude/rules/bash-error-logging.md` — erros de chamada HTTP devem ser registrados
- `scripts/operational-runbook.md` — credenciais de teste e portas

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-21 | Criado: workflow extraído de endpoint-validation.md (separação rules/skills) | Auditoria de governança |
| 2026-03-21 | Passo 5 reforçado: exibição de logs na íntegra obrigatória; proibição de resumir; bloco independente por requisição; itens 5-6 para cache miss/hit | Instrução do usuário |
