---
description: Salvar estado da sessão atual em arquivo datado para que o trabalho possa ser retomado em sessão futura com contexto completo.
---

# Comando Save Session

Captura tudo que aconteceu na sessão — o que foi construído, o que funcionou, o que falhou, o que falta — e salva em arquivo datado.

## Quando Usar

- Final de uma sessão de trabalho antes de fechar
- Antes de atingir limites de contexto
- Após resolver um problema complexo que vale lembrar
- Quando precisar transferir contexto para sessão futura

## Processo

### Passo 1: Coletar contexto

- Ler arquivos modificados na sessão (via `git diff`)
- Revisar o que foi discutido, tentado e decidido
- Anotar erros encontrados e como foram resolvidos
- Verificar estado atual de testes/build

### Passo 2: Criar pasta de sessões

```bash
mkdir -p .claude/sessions
```

### Passo 3: Escrever arquivo de sessão

Criar `.claude/sessions/YYYY-MM-DD-<id-curto>-session.md` com a data atual.

### Passo 4: Preencher todas as seções

```markdown
# Sessão: YYYY-MM-DD

**Projeto:** [nome do projeto]
**Tópico:** [resumo de uma linha]

---

## O Que Estamos Construindo
[1-3 parágrafos com contexto suficiente para alguém sem memória da sessão]

---

## O Que FUNCIONOU (com evidência)
- **[item]** — confirmado por: [evidência específica]

---

## O Que NÃO Funcionou (e por quê)
- **[abordagem tentada]** — falhou porque: [razão exata / mensagem de erro]

---

## O Que NÃO Foi Tentado Ainda
- [abordagem / ideia promissora]

---

## Estado Atual dos Arquivos

| Arquivo | Status | Notas |
|---------|--------|-------|
| `caminho/arquivo.cs` | Completo / Em Progresso / Quebrado / Não Iniciado | [detalhes] |

---

## Decisões Tomadas
- **[decisão]** — razão: [por que foi escolhida]

---

## Bloqueios e Questões Abertas
- [bloqueio / questão aberta]

---

## Próximo Passo Exato
[O passo mais importante ao retomar. Preciso o suficiente para não precisar pensar.]

---

## Notas de Ambiente
[Comandos, env vars, serviços necessários — apenas se relevante]
```

### Passo 5: Mostrar ao usuário

Exibir conteúdo completo e perguntar:
```
Sessão salva em [caminho]. Está preciso? Algo para corrigir?
```

## Notas

- Cada sessão tem seu próprio arquivo — nunca anexar a arquivo anterior
- A seção "O Que NÃO Funcionou" é a mais crítica — sessões futuras vão repetir abordagens falhadas sem ela
- O arquivo é lido pelo `/resume-session` no início da próxima sessão
- Arquivos de sessão NÃO são versionados (adicionar `.claude/sessions/` ao `.gitignore`)
