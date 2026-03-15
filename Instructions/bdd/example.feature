# Arquivo de exemplo genérico e agnóstico de domínio
# Propósito: demonstrar o formato e convenções de BDD deste repositório
# Regras relacionadas: (a definir quando regras específicas forem criadas)
# Nota: este arquivo é um exemplo de referência — não representa comportamento real do sistema

Feature: Gestão de Recursos
  # Descrição em linguagem de negócio do que esta funcionalidade representa
  # 'Recurso' aqui é um placeholder agnóstico — substituir pelo conceito real do domínio

  Como usuário autorizado
  Quero gerenciar recursos do sistema
  Para que eu possa manter os dados do sistema atualizados e consistentes

  Background:
    Given que o sistema está disponível
    And que o usuário está autenticado com permissão adequada

  # Exemplo: fluxo feliz de criação
  Scenario: deve criar recurso com sucesso quando dados são válidos
    Given que não existe recurso com o identificador "REC-001"
    When o usuário solicita a criação do recurso com:
      | campo       | valor          |
      | identificador | REC-001      |
      | nome        | Recurso Exemplo |
      | status      | Ativo          |
    Then o recurso é criado com sucesso
    And o status do recurso é "Ativo"
    And o sistema retorna confirmação da criação

  # Exemplo: validação de dados obrigatórios
  Scenario: deve rejeitar criação quando campo obrigatório estiver ausente
    Given que o usuário tenta criar um recurso sem o campo "nome"
    When a solicitação de criação é enviada
    Then o sistema rejeita a operação
    And o sistema informa que o campo "nome" é obrigatório
    And nenhum recurso é criado

  # Exemplo: fluxo de busca
  Scenario: deve retornar recurso quando identificador existir
    Given que existe um recurso com o identificador "REC-002"
    When o usuário busca o recurso com identificador "REC-002"
    Then o recurso é retornado com seus dados
    And o status do recurso está presente na resposta

  # Exemplo: recurso não encontrado
  Scenario: deve indicar ausência quando recurso não existir
    Given que não existe recurso com o identificador "REC-999"
    When o usuário busca o recurso com identificador "REC-999"
    Then o sistema indica que o recurso não foi encontrado
    And nenhum dado de recurso é retornado

  # Exemplo: atualização com validação de invariante
  Scenario: deve rejeitar atualização que viola regra de negócio
    Given que existe um recurso com identificador "REC-003" e status "Encerrado"
    When o usuário tenta alterar dados do recurso com status "Encerrado"
    Then o sistema rejeita a operação
    And o sistema informa que recursos encerrados não podem ser alterados
    And o recurso permanece com seus dados originais

  # Exemplo: remoção com pré-condição
  Scenario: deve permitir remoção apenas quando não houver dependentes
    Given que existe um recurso com identificador "REC-004" sem dependentes associados
    When o usuário solicita a remoção do recurso "REC-004"
    Then o recurso é removido com sucesso
    And o recurso não está mais disponível para consulta

  Scenario: deve rejeitar remoção quando existirem dependentes associados
    Given que existe um recurso com identificador "REC-005"
    And que existem 3 dependentes associados ao recurso "REC-005"
    When o usuário solicita a remoção do recurso "REC-005"
    Then o sistema rejeita a remoção
    And o sistema informa que o recurso possui dependentes associados
    And o recurso permanece disponível para consulta

  # Exemplo com Scenario Outline: comportamento com múltiplos dados
  Scenario Outline: deve validar status de acordo com as transições permitidas
    Given que existe um recurso com status "<status_atual>"
    When o usuário solicita a transição para o status "<status_destino>"
    Then o sistema "<resultado>"

    Examples:
      | status_atual | status_destino | resultado |
      | Ativo        | Suspenso       | permite a transição |
      | Ativo        | Encerrado      | permite a transição |
      | Suspenso     | Ativo          | permite a transição |
      | Encerrado    | Ativo          | rejeita a transição |
      | Encerrado    | Suspenso       | rejeita a transição |
