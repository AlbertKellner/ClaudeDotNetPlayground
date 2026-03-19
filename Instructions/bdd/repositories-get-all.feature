# Regras relacionadas: RN-006, RN-003
# Contratos relacionados: GET /repositories
# Última atualização: 2026-03-19

Feature: Busca de repositórios do team IntegrationRepos no GitHub
  Como usuário autenticado
  Quero consultar a lista de repositórios acessíveis ao team IntegrationRepos da organização WebMotors
  Para que eu possa visualizar e registrar localmente os repositórios disponíveis para integração

  Background:
    Given que a aplicação está em execução
    And que o Personal Access Token do GitHub está configurado

  # --- Autenticação (RN-003) ---

  @smoke
  Scenario: não deve permitir acesso sem autenticação
    Given que o usuário não está autenticado
    When o usuário solicita a lista de repositórios
    Then o sistema retorna status 401
    And o corpo da resposta está em formato Problem Details

  @smoke
  Scenario: não deve permitir acesso com token inválido
    Given que o usuário possui um token expirado ou inválido
    When o usuário solicita a lista de repositórios
    Then o sistema retorna status 401
    And o corpo da resposta está em formato Problem Details

  # --- Fluxo principal (RN-006) ---

  @smoke
  Scenario: deve retornar a lista de repositórios do team IntegrationRepos
    Given que o usuário está autenticado
    And que a API do GitHub está acessível
    And que o team IntegrationRepos possui repositórios cadastrados
    When o usuário solicita a lista de repositórios
    Then o sistema consulta a API do GitHub para o team IntegrationRepos da organização WebMotors
    And o sistema retorna status 200
    And a resposta contém a lista de repositórios com nome, descrição, URL Git, data da última modificação e campo de última sincronização

  Scenario: deve registrar cada repositório encontrado no log
    Given que o usuário está autenticado
    And que a API do GitHub está acessível
    And que o team IntegrationRepos possui repositórios cadastrados
    When o usuário solicita a lista de repositórios
    Then o sistema registra no log cada repositório encontrado com nível information

  Scenario: deve persistir os repositórios no arquivo JSON
    Given que o usuário está autenticado
    And que a API do GitHub está acessível
    And que o team IntegrationRepos possui repositórios cadastrados
    When o usuário solicita a lista de repositórios
    Then o sistema persiste no arquivo JSON configurado os dados de cada repositório
    And cada registro contém nome do repositório, descrição, URL Git e data da última modificação
    And o campo de última sincronização local está em branco para repositórios recém-registrados

  # --- Cenários de borda ---

  Scenario: deve retornar lista vazia quando o team não possui repositórios
    Given que o usuário está autenticado
    And que a API do GitHub está acessível
    And que o team IntegrationRepos não possui repositórios cadastrados
    When o usuário solicita a lista de repositórios
    Then o sistema retorna status 200
    And a resposta contém uma lista vazia de repositórios
