using System.Diagnostics;
using System.Text.Json;
using Albert.Playground.ECS.AOT.Api.Shared.Repositories;

namespace Albert.Playground.ECS.AOT.Api.Features.Command.RepositoriesSyncAll;

public sealed class RepositoriesSyncAllUseCase(
    IConfiguration configuration,
    ILogger<RepositoriesSyncAllUseCase> logger)
{
    public async Task<RepositoriesSyncAllOutput> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[RepositoriesSyncAllUseCase][ExecuteAsync] Executar caso de uso de sincronização de repositórios");

        var jsonFilePath = configuration["Repositories:JsonFilePath"] ?? "data/repositories.json";

        if (!File.Exists(jsonFilePath))
        {
            logger.LogError(
                "[RepositoriesSyncAllUseCase][ExecuteAsync] Arquivo JSON de repositórios não encontrado. Path={Path}",
                jsonFilePath);

            return new RepositoriesSyncAllOutput
            {
                TotalRepositories = 0,
                SuccessCount = 0,
                ErrorCount = 0,
                Results = []
            };
        }

        var json = await File.ReadAllTextAsync(jsonFilePath, cancellationToken);
        var entries = JsonSerializer.Deserialize(json, RepositoryFileJsonContext.Default.ListRepositoryFileEntry) ?? [];

        var syncRootPath = configuration["Repositories:SyncRootPath"] ?? "c:/usuarios/albert/git";

        if (!Directory.Exists(syncRootPath))
        {
            Directory.CreateDirectory(syncRootPath);
        }

        logger.LogInformation(
            "[RepositoriesSyncAllUseCase][ExecuteAsync] Iterar {Count} repositórios para sincronizar. RootPath={RootPath}",
            entries.Count,
            syncRootPath);

        var results = new List<RepositorySyncResult>();
        var successCount = 0;
        var errorCount = 0;

        foreach (var entry in entries)
        {
            var targetDir = Path.Combine(syncRootPath, entry.Name);
            var gitDir = Path.Combine(targetDir, ".git");

            try
            {
                if (Directory.Exists(gitDir))
                {
                    var (pullSuccess, _, pullError) = await RunGitCommandAsync(targetDir, "pull");

                    if (pullSuccess)
                    {
                        logger.LogInformation(
                            "[RepositoriesSyncAllUseCase][ExecuteAsync] Sincronizar repositório com sucesso. Name={Name}, Action=pull",
                            entry.Name);

                        entry.LastSyncDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        results.Add(new RepositorySyncResult { Name = entry.Name, Status = "pulled" });
                        successCount++;
                    }
                    else
                    {
                        logger.LogError(
                            "[RepositoriesSyncAllUseCase][ExecuteAsync] Erro ao sincronizar repositório. Name={Name}, Error={Error}",
                            entry.Name,
                            pullError);

                        results.Add(new RepositorySyncResult { Name = entry.Name, Status = "error", ErrorMessage = pullError });
                        errorCount++;
                    }
                }
                else
                {
                    var (cloneSuccess, _, cloneError) = await RunGitCommandAsync(
                        syncRootPath,
                        $"clone {entry.GitUrl} {entry.Name}");

                    if (cloneSuccess)
                    {
                        logger.LogInformation(
                            "[RepositoriesSyncAllUseCase][ExecuteAsync] Sincronizar repositório com sucesso. Name={Name}, Action=clone",
                            entry.Name);

                        entry.LastSyncDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        results.Add(new RepositorySyncResult { Name = entry.Name, Status = "cloned" });
                        successCount++;
                    }
                    else
                    {
                        logger.LogError(
                            "[RepositoriesSyncAllUseCase][ExecuteAsync] Erro ao sincronizar repositório. Name={Name}, Error={Error}",
                            entry.Name,
                            cloneError);

                        results.Add(new RepositorySyncResult { Name = entry.Name, Status = "error", ErrorMessage = cloneError });
                        errorCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(
                    "[RepositoriesSyncAllUseCase][ExecuteAsync] Erro ao sincronizar repositório. Name={Name}, Error={Error}",
                    entry.Name,
                    ex.Message);

                results.Add(new RepositorySyncResult { Name = entry.Name, Status = "error", ErrorMessage = ex.Message });
                errorCount++;
            }
        }

        logger.LogInformation(
            "[RepositoriesSyncAllUseCase][ExecuteAsync] Iteração de sincronização concluída");

        var updatedJson = JsonSerializer.Serialize(entries, RepositoryFileJsonContext.Default.ListRepositoryFileEntry);
        await File.WriteAllTextAsync(jsonFilePath, updatedJson, cancellationToken);

        logger.LogInformation(
            "[RepositoriesSyncAllUseCase][ExecuteAsync] Atualizar arquivo JSON com datas de sincronização. Path={Path}",
            jsonFilePath);

        var output = new RepositoriesSyncAllOutput
        {
            TotalRepositories = entries.Count,
            SuccessCount = successCount,
            ErrorCount = errorCount,
            Results = results
        };

        logger.LogInformation(
            "[RepositoriesSyncAllUseCase][ExecuteAsync] Retornar resultado da sincronização. Success={Success}, Errors={Errors}",
            successCount,
            errorCount);

        return output;
    }

    private static async Task<(bool Success, string Output, string Error)> RunGitCommandAsync(
        string workingDirectory,
        string arguments)
    {
        using var process = new Process();

        process.StartInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        process.Start();

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        return (process.ExitCode == 0, output.Trim(), error.Trim());
    }
}
