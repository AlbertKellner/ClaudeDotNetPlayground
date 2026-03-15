using System.ComponentModel.DataAnnotations;

namespace ClaudeDotNetPlayground.Features.Command.UserLogin;

public sealed class UserLoginInput
{
    [Required]
    public string UserName { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}

public sealed record UserLoginOutput(string Token);
