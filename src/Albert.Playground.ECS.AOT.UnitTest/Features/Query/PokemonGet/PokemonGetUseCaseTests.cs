using Albert.Playground.ECS.AOT.Api.Features.Query.PokemonGet;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Features.Query.PokemonGet;

public sealed class PokemonGetUseCaseTests
{
    private sealed class FakePokeApiClient : IPokeApiClient
    {
        public int? LastId { get; private set; }

        public Task<PokeApiOutput> GetPokemonByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            LastId = id;

            var result = new PokeApiOutput
            {
                Id = 25,
                Name = "pikachu",
                Height = 4,
                Weight = 60,
                Types =
                [
                    new PokeApiTypeSlot
                    {
                        Slot = 1,
                        Type = new PokeApiTypeInfo { Name = "electric", Url = "https://pokeapi.co/api/v2/type/13/" }
                    }
                ],
                Sprites = new PokeApiSprites
                {
                    FrontDefault = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/25.png"
                }
            };

            return Task.FromResult(result);
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
    public async Task ExecuteAsync_DeveRetornarOutputComDadosMapeados()
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
        Assert.Single(result.Types);
        Assert.Equal(1, result.Types[0].Slot);
        Assert.Equal("electric", result.Types[0].Name);
        Assert.Equal("https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/25.png", result.SpriteUrl);
    }

    [Fact]
    public async Task ExecuteAsync_DevePassarIdDoPikachuParaApi()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLogger = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        Assert.Equal(25, fakeClient.LastId);
    }

    [Fact]
    public async Task ExecuteAsync_DeveRegistrarLogDeIteracao()
    {
        var fakeClient = new FakePokeApiClient();
        var fakeLogger = new FakeLogger<PokemonGetUseCase>();
        var useCase = new PokemonGetUseCase(fakeClient, fakeLogger);

        await useCase.ExecuteAsync();

        var logs = fakeLogger.GetSnapshot();

        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Iniciar iteração"));

        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Concluir iteração"));
    }
}
