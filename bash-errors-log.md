# Log de Erros de Bash

Este arquivo documenta todos os erros de Bash encontrados durante sessões de trabalho neste repositório, incluindo causa raiz e solução adotada. É um log acumulativo — erros não são removidos após resolvidos.

## Template de Registro

```markdown
## Erro [N] — [Título descritivo do problema]

| Campo | Valor |
|---|---|
| **Número** | [N] |
| **Data** | [YYYY-MM-DD] |
| **Comando executado** | `[comando exato que falhou]` |
| **Erro retornado** | `[mensagem de erro exata]` |
| **Causa** | [Explicação técnica objetiva da causa raiz] |
| **Novo comando / solução** | `[comando ou sequência que resolveu]` |
```

---

> **Estado atual**: nenhum erro registrado. Erros serão documentados à medida que forem encontrados durante sessões de trabalho.

---

## Referências

- `docker-compose.yml` — arquivo principal afetado por correções de infraestrutura
- `src/Starter.Template.AOT.Api/Dockerfile` — modificado para suporte a CA customizada e hash symlinks
- `assumptions-log.md` — premissas de ambiente registradas
