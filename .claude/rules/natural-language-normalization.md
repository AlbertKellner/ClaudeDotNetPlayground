# Regra: Normalização de Linguagem Natural

## Propósito

Esta rule define como o assistente deve processar toda entrada do usuário antes de qualquer ação.
A normalização é **obrigatória** e deve ocorrer antes de interpretar, classificar ou implementar qualquer coisa.

---

## Regras de Interpretação de Entrada Imperfeita

### O assistente DEVE:

1. **Interpretar semanticamente** toda mensagem antes de agir — identificar o que o usuário quer dizer, não apenas o que escreveu.

2. **Reconstruir a intenção** quando a formulação for fragmentada, ditada, com autocorreções embutidas, incompleta ou ambígua.

3. **Resolver silenciosamente** erros ortográficos óbvios, palavras faltantes, concordância incorreta e estruturas fragmentadas — apenas para fins de entendimento interno.

4. **Normalizar para linguagem técnica limpa** antes de persistir qualquer conteúdo em arquivos de governança, contratos, BDD, código ou artefatos declarativos.

5. **Consultar o contexto do repositório** antes de assumir interpretação para termos ambíguos — o glossário e as regras de negócio existentes têm prioridade sobre suposições genéricas.

6. **Usar a menor premissa coerente** compatível com o contexto já existente quando a mensagem não for suficientemente clara.

7. **Preservar a intenção, não a formulação** — a versão persistida deve representar o significado correto em linguagem técnica, não a transcrição bruta da mensagem.

### O assistente NUNCA deve:

- Exigir que o usuário reescreva uma mensagem apenas porque ela foi informal, ditada, fragmentada ou imperfeita.
- Copiar erros brutos de escrita para arquivos de governança, instruções, contratos, BDD, código ou artefatos declarativos.
- Inventar regras de domínio ausentes para preencher lacunas.
- Inventar restrições técnicas ausentes para preencher lacunas.
- Inventar comportamento de negócio ausente apenas porque a mensagem foi incompleta.
- Tratar ambiguidade como rejeição — ambiguidade é oportunidade de interpretação conservadora ou registro de dúvida.

---

## Workflow de Normalização

Execute esta sequência para toda mensagem do usuário:

```
1. Ler a mensagem completa sem interromper o raciocínio no primeiro fragmento
2. Identificar o núcleo da intenção (o que o usuário quer que aconteça)
3. Identificar elementos secundários (contexto, restrições, exemplos, snippets)
4. Resolver ambiguidades lexicais consultando o glossário do repositório
5. Reconstruir a intenção em linguagem técnica clara
6. Verificar se a intenção reconstruída é compatível com a governança existente
7. Se houver incompatibilidade material, registrar como dúvida antes de agir
8. Usar a versão normalizada como base para todas as ações subsequentes
```

---

## Padrões de Entrada e Como Tratar

| Padrão de Entrada | Tratamento Obrigatório |
|---|---|
| Frase imperativa curta ("crie um endpoint de login") | Consultar contexto do repositório, inferir padrões existentes, registrar premissas |
| Entrada ditada com repetições ("eu quero que... quero que o sistema...") | Extrair a intenção principal, ignorar repetições transitórias |
| Autocorreções embutidas ("usa... não, usa a versão nova do...") | Usar a versão final explicitada pelo usuário |
| Reformulações dentro da mesma mensagem | Usar a última formulação como intenção definitiva |
| Entrada com mistura de código e texto | Separar texto narrativo de trechos técnicos, classificar os trechos separadamente |
| Entrada com erros de português | Normalizar internamente, nunca persistir os erros |
| Solicitação com múltiplos itens | Processar como lista ordenada, identificar dependências entre itens |
| Pergunta retórica embutida em solicitação | Tratar como parte da intenção, não como pergunta literal separada |

---

## Persistência de Conteúdo Normalizado

- Todo conteúdo que for persistido em arquivos do repositório deve estar em linguagem técnica limpa.
- A normalização é um processo interno — o usuário não precisa ver a versão normalizada a menos que ela seja relevante para confirmar interpretação.
- Quando a interpretação puder afetar comportamento, arquitetura, contratos ou regras de negócio, **confirme explicitamente a interpretação adotada** no início da resposta.
- Quando a interpretação for óbvia e de baixo risco, execute sem confirmação e inclua no relatório final.

---

## Relação com Outras Rules

- Esta rule é pré-requisito para `ambiguity-handling.md` — normalizar vem antes de detectar ambiguidade.
- Esta rule alimenta `business-ingestion.md` e `technical-ingestion.md` — o conteúdo normalizado é o que essas rules processam.
- Esta rule interage com `snippet-handling.md` — trechos técnicos são separados do texto narrativo durante a normalização.
