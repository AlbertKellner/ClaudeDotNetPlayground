using Microsoft.AspNetCore.Mvc;

namespace ClaudeDotNetPlayground.Infra.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AuthenticateAttribute : TypeFilterAttribute
{
    public AuthenticateAttribute() : base(typeof(AuthenticateFilter)) { }
}
