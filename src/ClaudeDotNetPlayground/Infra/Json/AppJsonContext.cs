using System.Text.Json.Serialization;
using ClaudeDotNetPlayground.Features.Command.UserLogin;
using ClaudeDotNetPlayground.Shared.ExternalApi.OpenMeteo;
using Microsoft.AspNetCore.Mvc;

namespace ClaudeDotNetPlayground.Infra.Json;

[JsonSerializable(typeof(UserLoginInput))]
[JsonSerializable(typeof(UserLoginOutput))]
[JsonSerializable(typeof(OpenMeteoOutput))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(ProblemDetails))]
internal sealed partial class AppJsonContext : JsonSerializerContext { }
