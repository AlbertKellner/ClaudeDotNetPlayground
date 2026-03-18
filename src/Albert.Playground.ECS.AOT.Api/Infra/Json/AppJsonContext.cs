using System.Text.Json.Serialization;
using Albert.Playground.ECS.AOT.Api.Features.Command.UserLogin;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.OpenMeteo;
using Microsoft.AspNetCore.Mvc;

namespace Albert.Playground.ECS.AOT.Api.Infra.Json;

[JsonSerializable(typeof(UserLoginInput))]
[JsonSerializable(typeof(UserLoginOutput))]
[JsonSerializable(typeof(OpenMeteoOutput))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(ProblemDetails))]
internal sealed partial class AppJsonContext : JsonSerializerContext { }
