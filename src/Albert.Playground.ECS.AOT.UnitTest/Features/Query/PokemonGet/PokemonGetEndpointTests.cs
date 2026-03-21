using Albert.Playground.ECS.AOT.Api.Features.Query.PokemonGet;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon.Models;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Features.Query.PokemonGet;

public sealed class PokemonGetEndpointTests
{
    private sealed class FakePokemonApiClient : IPokemonApiClient
    {
        public Task<PokemonOutput> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var output = new PokemonOutput
            {
                Id = 25,
                Name = "pikachu",
                Height = 4,
                Weight = 60,
                BaseExperience = 112,
                Types =
                [
                    new PokemonTypeSlot
                    {
                        Slot = 1,
                        Type = new PokemonNamedResource { Name = "electric" }
                    }
                ],
                Abilities =
                [
                    new PokemonAbilitySlot
                    {
                        Slot = 1,
                        IsHidden = false,
                        Ability = new PokemonNamedResource { Name = "static" }
                    }
                ],
                Stats =
                [
                    new PokemonStatSlot
                    {
                        BaseStat = 35,
                        Effort = 0,
                        Stat = new PokemonNamedResource { Name = "hp" }
                    }
                ],
                Sprites = new PokemonSprites
                {
                    FrontDefault = "https://example.com/25.png"
                }
            };

            return Task.FromResult(output);
        }
    }

    [Fact]
    public async Task Get_DeveRetornarOkComOutput()
    {
        var fakeClient = new FakePokemonApiClient();
        var fakeLoggerUseCase = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<PokemonGetEndpoint>();
        var endpoint = new PokemonGetEndpoint(useCase, fakeLoggerEndpoint);

        var actionResult = await endpoint.Get(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var output = Assert.IsType<PokemonGetOutput>(okResult.Value);
        Assert.Equal("pikachu", output.Name);
        Assert.Equal(25, output.Id);
    }

    [Fact]
    public async Task Get_DeveRegistrarLogNoInicio()
    {
        var fakeClient = new FakePokemonApiClient();
        var fakeLoggerUseCase = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<PokemonGetEndpoint>();
        var endpoint = new PokemonGetEndpoint(useCase, fakeLoggerEndpoint);

        await endpoint.Get(CancellationToken.None);

        var logs = fakeLoggerEndpoint.GetSnapshot();

        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Processar"));
    }

    [Fact]
    public async Task Get_DeveRegistrarLogDeRetorno()
    {
        var fakeClient = new FakePokemonApiClient();
        var fakeLoggerUseCase = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<PokemonGetEndpoint>();
        var endpoint = new PokemonGetEndpoint(useCase, fakeLoggerEndpoint);

        await endpoint.Get(CancellationToken.None);

        var logs = fakeLoggerEndpoint.GetSnapshot();

        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar"));
    }
}
