using Albert.Playground.ECS.AOT.Api.Features.Query.PokemonSearchGet;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.PokeApi;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Features.Query.PokemonSearchGet;

public sealed class PokemonSearchGetUseCaseTests
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
                BaseExperience = 112,
                Height = 4,
                Weight = 60,
                IsDefault = true,
                Order = 35,
                LocationAreaEncounters = "https://pokeapi.co/api/v2/pokemon/25/encounters",
                Species = new PokeApiNamedResource { Name = "pikachu", Url = "https://pokeapi.co/api/v2/pokemon-species/25/" },
                Abilities =
                [
                    new PokeApiAbilitySlot
                    {
                        Ability = new PokeApiNamedResource { Name = "static", Url = "https://pokeapi.co/api/v2/ability/9/" },
                        IsHidden = false,
                        Slot = 1
                    }
                ],
                Types =
                [
                    new PokeApiTypeSlot
                    {
                        Slot = 1,
                        Type = new PokeApiNamedResource { Name = "electric", Url = "https://pokeapi.co/api/v2/type/13/" }
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
                Cries = new PokeApiCries
                {
                    Latest = "https://raw.githubusercontent.com/PokeAPI/cries/main/cries/pokemon/latest/25.ogg",
                    Legacy = "https://raw.githubusercontent.com/PokeAPI/cries/main/cries/pokemon/legacy/25.ogg"
                }
            };

            return Task.FromResult(output);
        }
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogInformationNoInicio()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLogger = new FakeLogger<PokemonSearchGetUseCase>();
        var useCase = new PokemonSearchGetUseCase(fakeClient, fakeLogger);

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
        var fakeLogger = new FakeLogger<PokemonSearchGetUseCase>();
        var useCase = new PokemonSearchGetUseCase(fakeClient, fakeLogger);

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
        var fakeLogger = new FakeLogger<PokemonSearchGetUseCase>();
        var useCase = new PokemonSearchGetUseCase(fakeClient, fakeLogger);

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
        var fakeLogger = new FakeLogger<PokemonSearchGetUseCase>();
        var useCase = new PokemonSearchGetUseCase(fakeClient, fakeLogger);

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
        var fakeLogger = new FakeLogger<PokemonSearchGetUseCase>();
        var useCase = new PokemonSearchGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("PokemonSearchGetUseCase", l.Message));
    }

    [Fact]
    public async Task ExecuteAsync_DeveRetornarOutputCompletoComDadosDoPikachu()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLogger = new FakeLogger<PokemonSearchGetUseCase>();
        var useCase = new PokemonSearchGetUseCase(fakeClient, fakeLogger);

        var result = await useCase.ExecuteAsync();

        Assert.IsType<PokemonSearchGetOutput>(result);
        Assert.Equal(25, result.Id);
        Assert.Equal("pikachu", result.Name);
        Assert.Equal(112, result.BaseExperience);
        Assert.Equal(4, result.Height);
        Assert.Equal(60, result.Weight);
    }

    [Fact]
    public async Task ExecuteAsync_DeveBuscarPikachuHardcoded()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLogger = new FakeLogger<PokemonSearchGetUseCase>();
        var useCase = new PokemonSearchGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        Assert.Equal("pikachu", fakeClient.LastName);
    }

    [Fact]
    public async Task ExecuteAsync_DeveMapearTiposCorretamente()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLogger = new FakeLogger<PokemonSearchGetUseCase>();
        var useCase = new PokemonSearchGetUseCase(fakeClient, fakeLogger);

        var result = await useCase.ExecuteAsync();

        Assert.Single(result.Types);
        Assert.Equal("electric", result.Types[0].Type.Name);
        Assert.Equal(1, result.Types[0].Slot);
    }

    [Fact]
    public async Task ExecuteAsync_DeveMapearHabilidadesCorretamente()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLogger = new FakeLogger<PokemonSearchGetUseCase>();
        var useCase = new PokemonSearchGetUseCase(fakeClient, fakeLogger);

        var result = await useCase.ExecuteAsync();

        Assert.Single(result.Abilities);
        Assert.NotNull(result.Abilities[0].Ability);
        Assert.Equal("static", result.Abilities[0].Ability!.Name);
        Assert.False(result.Abilities[0].IsHidden);
    }

    [Fact]
    public async Task ExecuteAsync_DeveMapearStatsCorretamente()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLogger = new FakeLogger<PokemonSearchGetUseCase>();
        var useCase = new PokemonSearchGetUseCase(fakeClient, fakeLogger);

        var result = await useCase.ExecuteAsync();

        Assert.Single(result.Stats);
        Assert.Equal(35, result.Stats[0].BaseStat);
        Assert.Equal("hp", result.Stats[0].Stat.Name);
    }

    [Fact]
    public async Task ExecuteAsync_DeveMapearCriesCorretamente()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLogger = new FakeLogger<PokemonSearchGetUseCase>();
        var useCase = new PokemonSearchGetUseCase(fakeClient, fakeLogger);

        var result = await useCase.ExecuteAsync();

        Assert.NotNull(result.Cries.Latest);
        Assert.Contains("25.ogg", result.Cries.Latest);
    }
}
