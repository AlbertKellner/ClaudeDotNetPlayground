# Regra: Validação de Endpoint após Implementação de Feature

## Propósito

Esta rule define a política de validação obrigatória de endpoints HTTP após implementação de feature. O workflow procedural está em `.claude/skills/validate-endpoints/SKILL.md`.

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

O passo de validação de endpoint é inserido após o health check e antes de exibir logs finais (passo 6 do pipeline em CLAUDE.md).

---

## Workflow

O workflow completo de validação (identificação de endpoints, obtenção de token, consumo HTTP, captura de logs, validação de cache, verificação de resultado e relatório) está definido em `.claude/skills/validate-endpoints/SKILL.md`.

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

- `environment-readiness.md` — a aplicação deve estar em execução antes desta rule ser ativada
- `bash-error-logging.md` — erros de chamada HTTP devem ser registrados neste log
- `governance-policies.md` §3 — se a validação revelar comportamento incorreto, propagar a correção antes de prosseguir

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-17 | Criado: rule de validação obrigatória de endpoints após implementação de feature | Instrução do usuário |
| 2026-03-20 | Adicionado: relatório de sucesso deve incluir status code, endpoint completo com parâmetros e JSON completo da resposta formatado em Markdown | Instrução do usuário |
| 2026-03-20 | Adicionado: Passo 3.1 (captura de logs por requisição) e item 4 no Passo 5 (logs de storytelling obrigatórios no relatório, com verificação visual do padrão SNP-001) | Instrução do usuário |
| 2026-03-21 | Adicionado: Passo 3.2 (validação de cache via segunda requisição consecutiva para endpoints com Memory Cache; validação de cache miss na primeira e cache hit na segunda) | Instrução do usuário |
| 2026-03-21 | Refatorado: workflow procedural extraído para skill validate-endpoints; rule simplificada para conter apenas política | Auditoria de governança |
