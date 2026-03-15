using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ClaudeDotNetPlayground.Infra.Security;

public sealed class TokenService(IConfiguration configuration, ILogger<TokenService> logger) : ITokenService
{
    private const string IdClaimType = "id";
    private const string UserNameClaimType = "userName";

    public string GenerateToken(int userId, string userName)
    {
        logger.LogInformation("[TokenService][GenerateToken] Gerar token JWT. UserId={UserId}, UserName={UserName}", userId, userName);

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
        var tokenString = handler.WriteToken(token);

        logger.LogInformation("[TokenService][GenerateToken] Retornar token JWT gerado. UserId={UserId}", userId);

        return tokenString;
    }

    public AuthenticatedUser? ValidateToken(string token)
    {
        logger.LogInformation("[TokenService][ValidateToken] Validar token JWT recebido");

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
            {
                logger.LogWarning("[TokenService][ValidateToken] Retornar nulo - token inválido ou claims ausentes");

                return null;
            }

            var user = new AuthenticatedUser(id, userNameClaim);

            logger.LogInformation("[TokenService][ValidateToken] Retornar usuário autenticado extraído do token. UserId={UserId}", id);

            return user;
        }
        catch
        {
            logger.LogWarning("[TokenService][ValidateToken] Retornar nulo - token inválido ou expirado");

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
