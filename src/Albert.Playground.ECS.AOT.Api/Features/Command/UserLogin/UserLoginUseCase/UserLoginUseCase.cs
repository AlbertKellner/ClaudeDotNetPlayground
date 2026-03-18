using Albert.Playground.ECS.AOT.Api.Infra.Security;

namespace Albert.Playground.ECS.AOT.Api.Features.Command.UserLogin;

public sealed class UserLoginUseCase(ITokenService tokenService, ILogger<UserLoginUseCase> logger)
{
    private sealed record User(int Id, string UserName, string Password);

    private static readonly User[] Users =
    [
        new User(123, "Albert", "albert123")
    ];

    public UserLoginOutput? Execute(UserLoginInput input)
    {
        logger.LogInformation("[UserLoginUseCase][Execute] Executar caso de uso de login. UserName={UserName}", input.UserName);

        logger.LogInformation("[UserLoginUseCase][Execute] Localizar usuário na base em memória. UserName={UserName}", input.UserName);

        var user = Array.Find(Users, u =>
            string.Equals(u.UserName, input.UserName, StringComparison.Ordinal) &&
            string.Equals(u.Password, input.Password, StringComparison.Ordinal));

        if (user is null)
        {
            logger.LogWarning("[UserLoginUseCase][Execute] Retornar nulo - usuário não encontrado. UserName={UserName}", input.UserName);

            return null;
        }

        logger.LogInformation("[UserLoginUseCase][Execute] Gerar token JWT para usuário autenticado. UserId={UserId}", user.Id);

        var token = tokenService.GenerateToken(user.Id, user.UserName);
        var output = new UserLoginOutput(token);

        logger.LogInformation("[UserLoginUseCase][Execute] Retornar resposta com token JWT gerado. UserId={UserId}", user.Id);

        return output;
    }
}
