# Regras relacionadas: RN-007, RN-006, RN-003
# Contratos relacionados: POST /repositories/sync
# Última atualização: 2026-03-19

Feature: Sincronização local de repositórios
  Como usuário autenticado
  Quero sincronizar localmente os repositórios registrados no arquivo JSON
  Para que eu tenha cópias atualizadas dos repositórios disponíveis para integração

  Background:
    Given que a aplicação está em execução

  # --- Autenticação (RN-003) ---

  @smoke
  Scenario: não deve permitir sincronização sem autenticação
    Given que o usuário não está autenticado
    When o usuário solicita a sincronização dos repositórios
    Then o sistema retorna status 401
    And o corpo da resposta está em formato Problem Details

  @smoke
  Scenario: não deve permitir sincronização com token inválido
    Given que o usuário possui um token expirado ou inválido
    When o usuário solicita a sincronização dos repositórios
    Then o sistema retorna status 401
    And o corpo da resposta está em formato Problem Details

  # --- Fluxo principal: clone (RN-007) ---

  @smoke
  Scenario: deve clonar repositório quando a pasta local não existe
    Given que o usuário está autenticado
    And que o arquivo JSON de repositórios existe com repositórios registrados
    And que a pasta local do repositório não existe
    When o usuário solicita a sincronização dos repositórios
    Then o sistema executa git clone para o repositório
    And o repositório é clonado na pasta raiz configurada com nome correspondente ao repositório
    And o sistema retorna status 200
    And o resultado contém status "cloned" para o repositório

  # --- Fluxo principal: pull (RN-007) ---

  Scenario: deve atualizar repositório quando a pasta local já existe com repositório Git
    Given que o usuário está autenticado
    And que o arquivo JSON de repositórios existe com repositórios registrados
    And que a pasta local do repositório já existe com repositório Git inicializado
    When o usuário solicita a sincronização dos repositórios
    Then o sistema executa git pull no repositório existente
    And o sistema retorna status 200
    And o resultado contém status "pulled" para o repositório

  # --- Registro de data de sincronização (RN-007) ---

  Scenario: deve gravar data e hora da sincronização no arquivo JSON após sucesso
    Given que o usuário está autenticado
    And que o arquivo JSON de repositórios existe com repositórios registrados
    When o usuário solicita a sincronização dos repositórios
    And a sincronização de um repositório é bem-sucedida
    Then o sistema grava no arquivo JSON a data e hora atuais no formato 24 horas dd/MM/yyyy HH:mm:ss
    And o campo de última sincronização do repositório sincronizado é atualizado

  # --- Contadores de resultado (RN-007) ---

  Scenario: deve retornar contadores de sucesso e erro com detalhes por repositório
    Given que o usuário está autenticado
    And que o arquivo JSON de repositórios existe com múltiplos repositórios registrados
    When o usuário solicita a sincronização dos repositórios
    Then o sistema retorna status 200
    And a resposta contém o total de repositórios processados
    And a resposta contém o contador de sucessos
    And a resposta contém o contador de erros
    And a resposta contém detalhes individuais por repositório com nome e status

  # --- Logging (RN-007) ---

  Scenario: deve registrar log com nível information para sincronização bem-sucedida
    Given que o usuário está autenticado
    And que o arquivo JSON de repositórios existe com repositórios registrados
    When o usuário solicita a sincronização dos repositórios
    And a sincronização de um repositório é bem-sucedida
    Then o sistema registra no log com nível information os detalhes da sincronização

  Scenario: deve registrar log com nível error para sincronização com falha
    Given que o usuário está autenticado
    And que o arquivo JSON de repositórios existe com repositórios registrados
    When o usuário solicita a sincronização dos repositórios
    And a sincronização de um repositório falha
    Then o sistema registra no log com nível error os detalhes da falha
    And o resultado contém status "error" para o repositório com mensagem de erro

  # --- Cenários de borda (RN-007) ---

  Scenario: deve retornar resultado vazio quando o arquivo JSON não existe
    Given que o usuário está autenticado
    And que o arquivo JSON de repositórios não existe
    When o usuário solicita a sincronização dos repositórios
    Then o sistema retorna status 200
    And a resposta contém zero repositórios processados
    And a resposta contém zero sucessos e zero erros
    And a resposta contém uma lista vazia de detalhes

  Scenario: deve retornar resultado vazio quando o arquivo JSON existe mas está vazio
    Given que o usuário está autenticado
    And que o arquivo JSON de repositórios existe mas não contém repositórios
    When o usuário solicita a sincronização dos repositórios
    Then o sistema retorna status 200
    And a resposta contém zero repositórios processados
