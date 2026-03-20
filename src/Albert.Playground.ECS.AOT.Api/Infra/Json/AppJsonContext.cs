using System.Text.Json.Serialization;
using Albert.Playground.ECS.AOT.Api.Features.Command.RepositoriesSyncAll;
using Albert.Playground.ECS.AOT.Api.Features.Command.UserLogin;
using Albert.Playground.ECS.AOT.Api.Features.Query.RepositoriesGetAll;
using Albert.Playground.ECS.AOT.Api.Features.Query.PokemonGet;
using Albert.Playground.ECS.AOT.Api.Features.Query.WeatherConditionsGet;
using Albert.Playground.ECS.AOT.Api.Shared.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Albert.Playground.ECS.AOT.Api.Infra.Json;

[JsonSerializable(typeof(UserLoginInput))]
[JsonSerializable(typeof(UserLoginOutput))]
[JsonSerializable(typeof(PokemonGetOutput))]
[JsonSerializable(typeof(WeatherConditionsGetOutput))]
[JsonSerializable(typeof(RepositoriesGetAllOutput))]
[JsonSerializable(typeof(List<RepositoryFileEntry>))]
[JsonSerializable(typeof(RepositoriesSyncAllOutput))]
[JsonSerializable(typeof(List<RepositorySyncResult>))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(ProblemDetails))]
internal sealed partial class AppJsonContext : JsonSerializerContext { }
