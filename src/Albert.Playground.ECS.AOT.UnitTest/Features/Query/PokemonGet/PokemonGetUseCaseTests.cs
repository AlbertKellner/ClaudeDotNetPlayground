using Albert.Playground.ECS.AOT.Api.Features.Query.PokemonGet;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.PokeApi;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Features.Query.PokemonGet;

public sealed class PokemonGetUseCaseTests
{
    private sealed class FakePokeApiClient : IPokeApiClient
    {
        public string? LastName { get; private set; }

        public Task<PokeApiOutput> GetPokemonAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            LastName = name;

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
                        Type = new PokeApiNamedResource { Name = "electric", Url = "https://pokeapi.co/api/v2/type/13/" }
                    }
                ],
                Abilities =
                [
                    new PokeApiAbilitySlot
                    {
                        Ability = new PokeApiNamedResource { Name = "static", Url = "https://pokeapi.co/api/v2/ability/9/" },
                        IsHidden = false,
                        Slot = 1
                    }
                ],
                Stats =
                [
                    new PokeApiStatSlot
                    {
                        BaseStat = 35,
                        Effort = 0,
                        Stat = new PokeApiNamedResource { Name = "hp", Url = "https://pokeapi.co/api/v2/stat/1/" }
                    }
                ],
                Sprites = new PokeApiSprites
                {
                    FrontDefault = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/25.png"
                }
            };

            return Task.FromResult(output);
        }
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogInformationNoInicio()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLogger = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Executar"));
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogInformationAoConsultarApi()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLogger = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Consultar"));
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogInformationAoMapear()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLogger = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Mapear"));
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogInformationNoRetorno()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLogger = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar"));
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLogger = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("PokemonGetUseCase", l.Message));
    }

    [Fact]
    public async Task ExecuteAsync_DevePassarNomePikachuParaApi()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLogger = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        Assert.Equal("pikachu", fakeClient.LastName);
    }

    [Fact]
    public async Task ExecuteAsync_DeveRetornarOutputComDadosCorretos()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLogger = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLogger);

        var result = await useCase.ExecuteAsync();

        Assert.IsType<PokemonGetOutput>(result);
        Assert.Equal(25, result.Id);
        Assert.Equal("pikachu", result.Name);
        Assert.Equal(4, result.Height);
        Assert.Equal(60, result.Weight);
        Assert.Equal(112, result.BaseExperience);
    }

    [Fact]
    public async Task ExecuteAsync_DeveMapearTiposCorretamente()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLogger = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLogger);

        var result = await useCase.ExecuteAsync();

        Assert.Single(result.Types);
        Assert.Equal("electric", result.Types[0].Name);
        Assert.Equal(1, result.Types[0].Slot);
    }

    [Fact]
    public async Task ExecuteAsync_DeveMapearAbilitiesCorretamente()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLogger = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLogger);

        var result = await useCase.ExecuteAsync();

        Assert.Single(result.Abilities);
        Assert.Equal("static", result.Abilities[0].Name);
        Assert.False(result.Abilities[0].IsHidden);
    }

    [Fact]
    public async Task ExecuteAsync_DeveMapearStatsCorretamente()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLogger = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLogger);

        var result = await useCase.ExecuteAsync();

        Assert.Single(result.Stats);
        Assert.Equal("hp", result.Stats[0].Name);
        Assert.Equal(35, result.Stats[0].BaseStat);
    }

}
