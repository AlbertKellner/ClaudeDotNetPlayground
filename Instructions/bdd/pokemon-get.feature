# Regras relacionadas: RN-008, RN-003
# Contratos relacionados: GET /pokemon
# Última atualização: 2026-03-20

Feature: Consulta de perfil essencial de Pokemon via PokeAPI
  Como usuário autenticado
  Quero consultar o perfil essencial de um Pokemon
  Para que eu possa visualizar as informações básicas do Pokemon

  Background:
    Given que a aplicação está em execução
    And que a PokeAPI está acessível

  # --- Autenticação (RN-003) ---

  @smoke
  Scenario: não deve permitir acesso sem autenticação
    Given que o usuário não está autenticado
    When o usuário solicita o perfil do Pokemon
    Then o sistema retorna status 401
    And o corpo da resposta está em formato Problem Details

  # --- Fluxo principal (RN-008) ---

  @smoke
  Scenario: deve retornar o perfil essencial do Pikachu
    Given que o usuário está autenticado
    When o usuário solicita o perfil do Pokemon
    Then o sistema consulta a PokeAPI para o Pokemon pikachu
    And o sistema retorna status 200
    And a resposta contém o perfil essencial do Pokemon

  Scenario: deve conter os campos essenciais na resposta
    Given que o usuário está autenticado
    When o usuário solicita o perfil do Pokemon
    Then o sistema retorna status 200
    And a resposta contém o campo id
    And a resposta contém o campo name
    And a resposta contém o campo height
    And a resposta contém o campo weight
    And a resposta contém o campo base_experience
    And a resposta contém o campo types
    And a resposta contém o campo abilities
    And a resposta contém o campo stats
    And a resposta contém o campo sprites
