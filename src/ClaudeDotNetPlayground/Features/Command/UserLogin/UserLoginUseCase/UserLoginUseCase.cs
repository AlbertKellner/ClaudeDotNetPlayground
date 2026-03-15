using ClaudeDotNetPlayground.Infra.Security;

namespace ClaudeDotNetPlayground.Features.Command.UserLogin;

public sealed class UserLoginUseCase(ITokenService tokenService)
{
    private sealed record User(int Id, string UserName, string Password);

    private static readonly User[] Users =
    [
        new User(123, "Albert", "albert123")
    ];

    public UserLoginOutput? Execute(UserLoginInput input)
    {
        var user = Array.Find(Users, u =>
            string.Equals(u.UserName, input.UserName, StringComparison.Ordinal) &&
            string.Equals(u.Password, input.Password, StringComparison.Ordinal));

        if (user is null)
            return null;

        var token = tokenService.GenerateToken(user.Id, user.UserName);
        return new UserLoginOutput(token);
    }
}
