## Motivos da alteração

<!-- Descreva os motivos que justificam esta alteração: qual problema foi identificado, qual regra de negócio é implementada, qual comportamento incorreto é corrigido, ou qual melhoria técnica é introduzida. -->

## Plano de execução

<!-- Descreva de forma clara e técnica o plano de execução seguido para implementar esta alteração: quais etapas foram planejadas, qual sequência foi adotada e quais decisões técnicas foram tomadas. -->

## O que foi realizado

<!-- Descreva de forma completa e técnica tudo o que foi feito neste PR. Inclua: arquivos criados ou modificados, mudanças de comportamento, endpoints adicionados ou alterados, regras de negócio implementadas, e qualquer outro detalhe relevante. Mantenha este campo sempre atualizado com o estado real do PR — remova referências a alterações descartadas e adicione novas quando houver mudanças. -->

## Checklist

- [ ] Build limpo (`dotnet build` sem erros)
- [ ] Testes passando em modo debug (`dotnet test`)
- [ ] HealthCheck passando (`/health` retorna `Healthy` ou `Degraded` esperado)
- [ ] Endpoints validados via chamada HTTP real (quando aplicável)
- [ ] Governança atualizada antes da implementação (quando aplicável)
- [ ] Título do PR claro, objetivo e tecnicamente descritivo
- [ ] Descrição do PR consistente com o estado real da implementação
- [ ] Título e descrição escritos em português brasileiro
- [ ] Commits seguindo Semantic Commits (`feat:`, `fix:`, `docs:`, `refactor:`, etc.)
