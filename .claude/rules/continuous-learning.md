# Regra: Aprendizado Contínuo Automatizado

## Propósito

Esta rule define a política do sistema de aprendizado contínuo automatizado do repositório. O sistema captura padrões de uso de ferramentas, erros recorrentes e decisões de correção, transformando-os em **instintos** — conhecimento operacional durável que melhora a eficiência de sessões futuras.

---

## Princípio Fundamental

> Todo erro repetido é conhecimento não capturado. Todo padrão recorrente é uma oportunidade de automação.
> O sistema de aprendizado transforma observações em instintos, instintos em governança.
> Nenhum padrão detectado deve ser perdido entre sessões.

---

## Conceitos

### Observação
Registro atômico de uso de ferramenta capturado pelo hook `observe-tool-use.sh`. Contém: timestamp, ferramenta, resumo de input/output, resultado (sucesso/falha), contexto de pipeline. Observações são transientes — não versionadas, arquivadas automaticamente.

### Instinto
Padrão detectado a partir de observações recorrentes. Possui confidence scoring, domínio, gatilho e evidência. Instintos são versionados e evoluem ao longo do tempo.

### Graduação
Processo pelo qual um instinto maduro (confidence >= 0.85, observado em 3+ sessões) é promovido a rule, skill ou check de auditoria na governança formal do repositório.

---

## Políticas

### Captura de Observações

- O hook `observe-tool-use.sh` captura toda chamada de ferramenta via `PostToolUse`
- Observações são salvas em `.claude/learning/observations.jsonl` (gitignored)
- Secrets (tokens, API keys, senhas) são removidos via regex antes da persistência
- Output é truncado a 500 caracteres para controle de tamanho
- Auto-archive quando o arquivo excede 5MB
- Subagentes e o próprio hook não são observados (guard anti-loop)

### Confidence Scoring

| Observações | Confidence inicial |
|---|---|
| 1-2 | 0.3 (tentativo) — armazenado em `instincts/tentative/` |
| 3-5 | 0.5 (moderado) — promovido para `instincts/active/` |
| 6-10 | 0.7 (forte) |
| 11+ | 0.85 (muito forte) — candidato a graduação |

**Ajustes:**
- +0.05 por observação confirmadora
- -0.10 por observação contraditória
- -0.02 por semana sem observação (decay)

**Remoção automática:** confidence < 0.2

### Domínios de Instinto

| Domínio | Escopo | Exemplos |
|---|---|---|
| `environment` | Ambiente de execução | Proxy CA, Docker daemon, variáveis ausentes |
| `pipeline` | Pipeline pré-commit | Ordem de passos, gates, compactação |
| `governance` | Governança do repositório | Audit checks recorrentes, propagação esquecida |
| `code-pattern` | Padrões de código | .NET, AOT, logging, serialização |
| `endpoint` | Endpoints HTTP | Validação, autenticação, cache |
| `tooling` | Uso de ferramentas | Sequências otimizáveis, preferências de busca |

### Ciclo de Vida

```
Observação (hook) → Detecção de padrão (skill) → Instinto tentativo (0.3)
    → Confirmação → Instinto ativo (0.5+)
    → Maturação → Candidato a graduação (0.85+, 3+ sessões)
    → Aprovação do usuário → Rule/Skill/Check de auditoria
    → Histórico em graduated/
```

### Graduação

Um instinto é candidato a graduação quando:
1. Confidence >= 0.85
2. Observado em 3 ou mais sessões distintas
3. Sem observações contraditórias nas últimas 2 sessões

O assistente deve **propor** a graduação ao usuário, nunca graduá-la automaticamente. A proposta inclui:
- O instinto com sua evidência
- Sugestão de destino (rule, skill, check de auditoria)
- Impacto esperado na governança

### Decay e Limpeza

- Instintos perdem 0.02 de confidence por semana sem observação confirmadora
- Instintos com confidence < 0.2 são removidos automaticamente pelo script `instinct-manager.sh`
- A auditoria (`governance-audit.sh`) verifica instintos expirados como aviso

### Integração com Governança Existente

| Artefato existente | Integração |
|---|---|
| `bash-errors-log.md` | Erros recorrentes (mesmo tipo 3+ vezes) geram instinto de prevenção |
| `governance-audit.sh` | Checks futuros verificam instintos expirados e graduações pendentes |
| `assumptions-log.md` | Instintos maduros de ambiente podem virar premissas formais |
| Pipeline pré-commit | Instintos de ambiente podem virar checks no passo 0 |
| `governance-behavior-tracking` | Instintos de comportamento omitido retroalimentam o tracking |

---

## Estrutura de Armazenamento

```
.claude/learning/
├── config.json                    # Configuração do sistema
├── observations.jsonl             # Log de observações (gitignored)
├── observations.archive/          # Arquivo morto (gitignored)
├── instincts/
│   ├── active/                    # Instintos com confidence >= 0.5
│   ├── tentative/                 # Instintos com confidence < 0.5
│   └── README.md                  # Formato e convenções
└── graduated/                     # Instintos promovidos a governança (histórico)
```

- `observations.jsonl` e `observations.archive/` são **gitignored** (transientes)
- `instincts/` e `graduated/` são **versionados** (conhecimento durável)
- `config.json` é **versionado** (configuração do sistema)

---

## Relação com Outras Rules

- `bash-error-logging.md` — erros logados alimentam a detecção de instintos de prevenção
- `governance-audit.md` — futuros checks verificarão saúde do sistema de instintos
- `governance-behavior-tracking.md` — instintos de comportamento omitido complementam o tracking
- `environment-readiness.md` — instintos de ambiente complementam o checklist de pré-requisitos
- `governance-policies.md` — instintos graduados seguem as políticas de propagação (§3)

---

## Histórico de Mudanças

| Data | Mudança | Referência |
|---|---|---|
| 2026-03-22 | Criado: rule de aprendizado contínuo automatizado — políticas de captura, scoring, ciclo de vida, graduação e integração | Adaptação do ECC (everything-claude-code) ao sistema de governança do repositório |
