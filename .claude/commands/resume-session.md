---
description: Carregar o arquivo de sessão mais recente e retomar trabalho com contexto completo de onde a última sessão parou.
---

# Comando Resume Session

Carrega o último estado de sessão salvo e se orienta completamente antes de trabalhar.

## Quando Usar

- Iniciando nova sessão para continuar trabalho anterior
- Após iniciar sessão fresca por limites de contexto
- Quando receber arquivo de sessão de outra fonte

## Uso

```
/resume-session                              # carrega mais recente
/resume-session 2026-03-22                   # carrega sessão da data
/resume-session .claude/sessions/arquivo.md  # carrega arquivo específico
```

## Processo

### Passo 1: Encontrar arquivo de sessão

Se nenhum argumento:
1. Verificar `.claude/sessions/`
2. Pegar o arquivo `*-session.md` mais recente
3. Se pasta não existir ou estiver vazia:
   ```
   Nenhum arquivo de sessão encontrado em .claude/sessions/
   Execute /save-session ao final de uma sessão para criar um.
   ```

### Passo 2: Ler arquivo completo

Ler todo o conteúdo. Não resumir ainda.

### Passo 3: Confirmar entendimento

```
SESSÃO CARREGADA: [caminho do arquivo]
════════════════════════════════════════

PROJETO: [nome / tópico]

O QUE ESTAMOS CONSTRUINDO:
[resumo de 2-3 frases]

ESTADO ATUAL:
✅ Funcionando: [contagem] itens confirmados
🔄 Em Progresso: [lista de arquivos em progresso]
🗒️ Não Iniciado: [lista de planejados mas não tocados]

O QUE NÃO REPETIR:
[lista de cada abordagem falhada com razão — crítico]

QUESTÕES ABERTAS / BLOQUEIOS:
[lista]

PRÓXIMO PASSO:
[passo exato se definido no arquivo]

════════════════════════════════════════
Pronto para continuar. O que gostaria de fazer?
```

### Passo 4: Aguardar o usuário

NÃO começar a trabalhar automaticamente. NÃO tocar em arquivos. Aguardar instrução.

## Casos Especiais

- **Sessão com mais de 7 dias**: Avisar que as coisas podem ter mudado
- **Arquivo referencia arquivos inexistentes**: Notar durante o briefing
- **Arquivo vazio ou malformado**: Reportar e sugerir `/save-session`

## Notas

- Nunca modificar o arquivo de sessão ao carregá-lo — é registro histórico de leitura
- "O Que Não Repetir" deve sempre ser mostrado, mesmo se vazio
- Após retomar, o usuário pode executar `/save-session` ao final da nova sessão
