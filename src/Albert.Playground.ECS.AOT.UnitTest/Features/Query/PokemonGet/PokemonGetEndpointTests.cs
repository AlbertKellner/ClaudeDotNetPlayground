using Albert.Playground.ECS.AOT.Api.Features.Query.PokemonGet;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.PokeApi;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Features.Query.PokemonGet;

public sealed class PokemonGetEndpointTests
{
    private sealed class FakePokeApiClient : IPokeApiClient
    {
        public Task<PokeApiOutput> GetPokemonAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            var output = new PokeApiOutput
            {
                Id = 25,
                Name = "pikachu",
                Height = 4,
                Weight = 60,
                BaseExperience = 112,
                Types =
                [
                    new PokeApiTypeSlot
                    {
                        Slot = 1,
                        Type = new PokeApiNamedResource { Name = "electric", Url = "" }
                    }
                ],
                Abilities = [],
                Stats = [],
                Sprites = new PokeApiSprites { FrontDefault = "https://example.com/25.png" }
            };

            return Task.FromResult(output);
        }
    }

    [Fact]
    public async Task Get_DeveRegistrarLogInformationNoInicio()
    {
        var fakeClient = new FakePokeApiClient();
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
    public async Task Get_DeveRegistrarLogInformationNoRetorno()
    {
        var fakeClient = new FakePokeApiClient();
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

    [Fact]
    public async Task Get_DeveRetornarOkComOutputCompleto()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLoggerUseCase = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<PokemonGetEndpoint>();
        var endpoint = new PokemonGetEndpoint(useCase, fakeLoggerEndpoint);

        var actionResult = await endpoint.Get(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var output = Assert.IsType<PokemonGetOutput>(okResult.Value);
        Assert.Equal(25, output.Id);
        Assert.Equal("pikachu", output.Name);
    }

    [Fact]
    public async Task Get_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLoggerUseCase = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<PokemonGetEndpoint>();
        var endpoint = new PokemonGetEndpoint(useCase, fakeLoggerEndpoint);

        await endpoint.Get(CancellationToken.None);

        var logs = fakeLoggerEndpoint.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("PokemonGetEndpoint", l.Message));
    }
}
