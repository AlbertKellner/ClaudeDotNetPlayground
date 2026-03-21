using System.Text.Json.Serialization;
using Albert.Playground.ECS.AOT.Api.Features.Command.UserLogin;
using Albert.Playground.ECS.AOT.Api.Features.Query.GitHubRepoSearch;
using Albert.Playground.ECS.AOT.Api.Features.Query.PokemonGet;
using Albert.Playground.ECS.AOT.Api.Features.Query.WeatherConditionsGet;
using Microsoft.AspNetCore.Mvc;

namespace Albert.Playground.ECS.AOT.Api.Infra.Json;

[JsonSerializable(typeof(UserLoginInput))]
[JsonSerializable(typeof(UserLoginOutput))]
[JsonSerializable(typeof(WeatherConditionsGetOutput))]
[JsonSerializable(typeof(GitHubRepoSearchOutput))]
[JsonSerializable(typeof(List<GitHubRepoSearchItem>))]
[JsonSerializable(typeof(PokemonGetOutput))]
[JsonSerializable(typeof(List<PokemonGetTypeItem>))]
[JsonSerializable(typeof(List<PokemonGetAbilityItem>))]
[JsonSerializable(typeof(List<PokemonGetStatItem>))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(ProblemDetails))]
internal sealed partial class AppJsonContext : JsonSerializerContext { }
