using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.Pokemon;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Shared.ExternalApi.Pokemon;

public sealed class PokeApiClientTests
{
    private sealed class FakePokeApi : IPokeApi
    {
        public int CallCount { get; private set; }

        public Task<PokeApiOutput> GetPokemonByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            CallCount++;

            var result = new PokeApiOutput
            {
                Id = id,
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
    public async Task GetPokemonByIdAsync_DeveRegistrarLogInformationNoInicio()
    {
        var fakeApi = new FakePokeApi();
        var fakeLogger = new FakeLogger<PokeApiClient>();
        var client = new PokeApiClient(fakeApi, fakeLogger);

        await client.GetPokemonByIdAsync(25);

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Executar"));
    }

    [Fact]
    public async Task GetPokemonByIdAsync_DeveRegistrarLogInformationNoRetorno()
    {
        var fakeApi = new FakePokeApi();
        var fakeLogger = new FakeLogger<PokeApiClient>();
        var client = new PokeApiClient(fakeApi, fakeLogger);

        await client.GetPokemonByIdAsync(25);

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar"));
    }

    [Fact]
    public async Task GetPokemonByIdAsync_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeApi = new FakePokeApi();
        var fakeLogger = new FakeLogger<PokeApiClient>();
        var client = new PokeApiClient(fakeApi, fakeLogger);

        await client.GetPokemonByIdAsync(25);

        var logs = fakeLogger.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("PokeApiClient", l.Message));
    }

    [Fact]
    public async Task GetPokemonByIdAsync_DeveRetornarDadosDaApi()
    {
        var fakeApi = new FakePokeApi();
        var fakeLogger = new FakeLogger<PokeApiClient>();
        var client = new PokeApiClient(fakeApi, fakeLogger);

        var result = await client.GetPokemonByIdAsync(25);

        Assert.Equal(25, result.Id);
        Assert.Equal("pikachu", result.Name);
        Assert.Equal(4, result.Height);
        Assert.Equal(60, result.Weight);
    }

    [Fact]
    public async Task GetPokemonByIdAsync_DeveChamarApiUmaVez()
    {
        var fakeApi = new FakePokeApi();
        var fakeLogger = new FakeLogger<PokeApiClient>();
        var client = new PokeApiClient(fakeApi, fakeLogger);

        await client.GetPokemonByIdAsync(25);

        Assert.Equal(1, fakeApi.CallCount);
    }
}
