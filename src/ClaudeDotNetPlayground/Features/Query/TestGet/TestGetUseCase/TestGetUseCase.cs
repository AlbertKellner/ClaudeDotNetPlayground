namespace ClaudeDotNetPlayground.Features.Query.TestGet;

public sealed class TestGetUseCase(ILogger<TestGetUseCase> logger)
{
    public string Execute()
    {
        logger.LogInformation("[TestGetUseCase][Execute] Executar caso de uso TestGet");

        var result = "funcionando";

        logger.LogInformation("[TestGetUseCase][Execute] Retornar resultado do caso de uso. Result={Result}", result);

        return result;
    }
}
