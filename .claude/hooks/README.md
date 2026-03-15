# Hooks de Governança

## Propósito

Esta pasta contém scripts placeholder e configurações que reforçam o modelo operacional deste repositório.

Os hooks aqui definidos são **placeholders genéricos** porque a stack tecnológica do repositório ainda não está determinada no momento do bootstrap.

## Por Que São Placeholders

Os hooks de enforcement real dependem da stack concreta do repositório:
- Hooks de linting dependem da linguagem (ESLint, Flake8, RuboCop, etc.)
- Hooks de teste dependem do framework (Jest, pytest, RSpec, etc.)
- Hooks de validação de schema dependem do tipo de contrato (OpenAPI validator, JSON Schema validator, etc.)
- Hooks de consistência de governança dependem dos formatos escolhidos

**Não inventamos enforcement específico de stack antes de saber a stack.**

## Como Adaptar Quando a Stack For Conhecida

1. Identificar a stack do repositório (linguagem, framework, ferramentas de CI)
2. Substituir os scripts placeholder pelos hooks reais correspondentes
3. Testar os hooks no ambiente de desenvolvimento
4. Registrar a decisão de tooling em `Instructions/decisions/`
5. Atualizar `Instructions/architecture/engineering-principles.md` com as práticas de qualidade adotadas

## Hooks Disponíveis

| Arquivo | Propósito | Gatilho Pretendido |
|---|---|---|
| `governance-change-detector.sh` | Detecta mudanças em arquivos de governança | Pre-commit |
| `governance-first-reminder.sh` | Lembra o modelo "governança antes de implementação" | Pre-commit |
| `alignment-check.sh` | Sinaliza inconsistências entre artefatos | Pre-commit / CI |
| `governance-consistency.sh` | Valida coerência mínima após mudanças de governança | Post-commit / CI |
| `ambiguity-guard.sh` | Verifica se dúvidas bloqueantes estão registradas | Pre-commit |
| `assumptions-tracker.sh` | Monitora premissas registradas sem resolução | CI |
| `canonical-snippets-guard.sh` | Verifica preservação de snippets normativos | Pre-commit |
| `settings.json` | Configuração de hooks para Claude Code | Configuração |

## Integração com Claude Code

O arquivo `settings.json` contém configuração de hooks para o Claude Code.
Ajuste conforme o comportamento desejado para este repositório.
