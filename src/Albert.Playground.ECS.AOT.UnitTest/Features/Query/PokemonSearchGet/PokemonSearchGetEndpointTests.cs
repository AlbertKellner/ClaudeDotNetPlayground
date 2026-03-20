using Albert.Playground.ECS.AOT.Api.Features.Query.PokemonSearchGet;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.PokeApi;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Features.Query.PokemonSearchGet;

public sealed class PokemonSearchGetEndpointTests
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
                BaseExperience = 112,
                Height = 4,
                Weight = 60,
                IsDefault = true,
                Order = 35,
                Species = new PokeApiNamedResource { Name = "pikachu", Url = "https://pokeapi.co/api/v2/pokemon-species/25/" },
                Types =
                [
                    new PokeApiTypeSlot
                    {
                        Slot = 1,
                        Type = new PokeApiNamedResource { Name = "electric", Url = "https://pokeapi.co/api/v2/type/13/" }
                    }
                ]
            };

            return Task.FromResult(output);
        }
    }

    [Fact]
    public async Task Get_DeveRegistrarLogInformationNoInicio()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLoggerUseCase = new FakeLogger<PokemonSearchGetUseCase>();
        var useCase = new PokemonSearchGetUseCase(fakeClient, fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<PokemonSearchGetEndpoint>();
        var endpoint = new PokemonSearchGetEndpoint(useCase, fakeLoggerEndpoint);

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
        var fakeLoggerUseCase = new FakeLogger<PokemonSearchGetUseCase>();
        var useCase = new PokemonSearchGetUseCase(fakeClient, fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<PokemonSearchGetEndpoint>();
        var endpoint = new PokemonSearchGetEndpoint(useCase, fakeLoggerEndpoint);

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
        var fakeLoggerUseCase = new FakeLogger<PokemonSearchGetUseCase>();
        var useCase = new PokemonSearchGetUseCase(fakeClient, fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<PokemonSearchGetEndpoint>();
        var endpoint = new PokemonSearchGetEndpoint(useCase, fakeLoggerEndpoint);

        var actionResult = await endpoint.Get(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var output = Assert.IsType<PokemonSearchGetOutput>(okResult.Value);
        Assert.Equal(25, output.Id);
        Assert.Equal("pikachu", output.Name);
    }

    [Fact]
    public async Task Get_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLoggerUseCase = new FakeLogger<PokemonSearchGetUseCase>();
        var useCase = new PokemonSearchGetUseCase(fakeClient, fakeLoggerUseCase);
        var fakeLoggerEndpoint = new FakeLogger<PokemonSearchGetEndpoint>();
        var endpoint = new PokemonSearchGetEndpoint(useCase, fakeLoggerEndpoint);

        await endpoint.Get(CancellationToken.None);

        var logs = fakeLoggerEndpoint.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("PokemonSearchGetEndpoint", l.Message));
    }
}
