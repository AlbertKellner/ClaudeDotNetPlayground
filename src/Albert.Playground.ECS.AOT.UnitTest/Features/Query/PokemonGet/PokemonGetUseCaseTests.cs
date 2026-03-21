using Albert.Playground.ECS.AOT.Api.Features.Query.PokemonGet;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon.Models;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Features.Query.PokemonGet;

public sealed class PokemonGetUseCaseTests
{
    private sealed class FakePokemonApiClient : IPokemonApiClient
    {
        public int? LastId { get; private set; }

        public Task<PokemonOutput> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            LastId = id;

            var output = new PokemonOutput
            {
                Id = id,
                Name = "pikachu",
                Height = 4,
                Weight = 60,
                BaseExperience = 112,
                Types =
                [
                    new PokemonTypeSlot
                    {
                        Slot = 1,
                        Type = new PokemonNamedResource { Name = "electric", Url = "https://pokeapi.co/api/v2/type/13/" }
                    }
                ],
                Abilities =
                [
                    new PokemonAbilitySlot
                    {
                        Slot = 1,
                        IsHidden = false,
                        Ability = new PokemonNamedResource { Name = "static", Url = "https://pokeapi.co/api/v2/ability/9/" }
                    },
                    new PokemonAbilitySlot
                    {
                        Slot = 3,
                        IsHidden = true,
                        Ability = new PokemonNamedResource { Name = "lightning-rod", Url = "https://pokeapi.co/api/v2/ability/31/" }
                    }
                ],
                Stats =
                [
                    new PokemonStatSlot
                    {
                        BaseStat = 35,
                        Effort = 0,
                        Stat = new PokemonNamedResource { Name = "hp", Url = "https://pokeapi.co/api/v2/stat/1/" }
                    },
                    new PokemonStatSlot
                    {
                        BaseStat = 55,
                        Effort = 0,
                        Stat = new PokemonNamedResource { Name = "attack", Url = "https://pokeapi.co/api/v2/stat/2/" }
                    }
                ],
                Sprites = new PokemonSprites
                {
                    FrontDefault = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/25.png",
                    FrontShiny = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/shiny/25.png"
                }
            };

            return Task.FromResult(output);
        }
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogInformationNoInicio()
    {
        var fakeClient = new FakePokemonApiClient();
        var fakeLogger = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();

        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Executar"));
    }

    [Fact]
    public async Task ExecuteAsync_DevePassarIdDoPikachuParaApi()
    {
        var fakeClient = new FakePokemonApiClient();
        var fakeLogger = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        Assert.Equal(25, fakeClient.LastId);
    }

    [Fact]
    public async Task ExecuteAsync_DeveRetornarOutputComDadosBasicosDoPokemon()
    {
        var fakeClient = new FakePokemonApiClient();
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
        var fakeClient = new FakePokemonApiClient();
        var fakeLogger = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLogger);

        var result = await useCase.ExecuteAsync();

        Assert.Single(result.Types);
        Assert.Equal("electric", result.Types[0].Name);
        Assert.Equal(1, result.Types[0].Slot);
    }

    [Fact]
    public async Task ExecuteAsync_DeveMapearHabilidadesCorretamente()
    {
        var fakeClient = new FakePokemonApiClient();
        var fakeLogger = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLogger);

        var result = await useCase.ExecuteAsync();

        Assert.Equal(2, result.Abilities.Count);
        Assert.Equal("static", result.Abilities[0].Name);
        Assert.False(result.Abilities[0].IsHidden);
        Assert.Equal("lightning-rod", result.Abilities[1].Name);
        Assert.True(result.Abilities[1].IsHidden);
    }

    [Fact]
    public async Task ExecuteAsync_DeveMapearStatsCorretamente()
    {
        var fakeClient = new FakePokemonApiClient();
        var fakeLogger = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLogger);

        var result = await useCase.ExecuteAsync();

        Assert.Equal(2, result.Stats.Count);
        Assert.Equal("hp", result.Stats[0].Name);
        Assert.Equal(35, result.Stats[0].BaseStat);
        Assert.Equal("attack", result.Stats[1].Name);
        Assert.Equal(55, result.Stats[1].BaseStat);
    }

    [Fact]
    public async Task ExecuteAsync_DeveMapearSpritesCorretamente()
    {
        var fakeClient = new FakePokemonApiClient();
        var fakeLogger = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLogger);

        var result = await useCase.ExecuteAsync();

        Assert.NotNull(result.FrontDefaultSprite);
        Assert.Contains("25.png", result.FrontDefaultSprite);
        Assert.NotNull(result.FrontShinySprite);
        Assert.Contains("shiny/25.png", result.FrontShinySprite);
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogDeRetorno()
    {
        var fakeClient = new FakePokemonApiClient();
        var fakeLogger = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();

        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar"));
    }
}
