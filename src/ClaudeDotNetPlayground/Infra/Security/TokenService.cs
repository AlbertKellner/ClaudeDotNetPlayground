using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ClaudeDotNetPlayground.Infra.Security;

public sealed class TokenService(IConfiguration configuration) : ITokenService
{
    private const string IdClaimType = "id";
    private const string UserNameClaimType = "userName";

    public string GenerateToken(int userId, string userName)
    {
        var key = GetSigningKey();
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(IdClaimType, userId.ToString()),
                new Claim(UserNameClaimType, userName)
            ]),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = credentials
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(descriptor);
        return handler.WriteToken(token);
    }

    public AuthenticatedUser? ValidateToken(string token)
    {
        var key = GetSigningKey();

        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, parameters, out _);

            var idClaim = principal.FindFirstValue(IdClaimType);
            var userNameClaim = principal.FindFirstValue(UserNameClaimType);

            if (idClaim is null || userNameClaim is null || !int.TryParse(idClaim, out var id))
                return null;

            return new AuthenticatedUser(id, userNameClaim);
        }
        catch
        {
            return null;
        }
    }

    private SymmetricSecurityKey GetSigningKey()
    {
        var secret = configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("Jwt:Secret is not configured.");
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    }
}
