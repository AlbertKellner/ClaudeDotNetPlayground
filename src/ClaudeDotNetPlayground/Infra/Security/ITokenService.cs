namespace ClaudeDotNetPlayground.Infra.Security;

public interface ITokenService
{
    string GenerateToken(int userId, string userName);
    AuthenticatedUser? ValidateToken(string token);
}
