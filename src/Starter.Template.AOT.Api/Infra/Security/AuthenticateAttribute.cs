using Microsoft.AspNetCore.Mvc;

namespace Starter.Template.AOT.Api.Infra.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AuthenticateAttribute : TypeFilterAttribute
{
    public AuthenticateAttribute() : base(typeof(AuthenticateFilter)) { }
}
