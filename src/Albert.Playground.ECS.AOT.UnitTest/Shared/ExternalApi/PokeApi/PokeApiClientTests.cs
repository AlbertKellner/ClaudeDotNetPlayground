using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.PokeApi;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Shared.ExternalApi.PokeApi;

public sealed class PokeApiClientTests
{
    private sealed class FakePokeApi : IPokeApi
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
                BaseExperience = 112
            };

            return Task.FromResult(output);
        }
    }

    [Fact]
    public async Task GetPokemonAsync_DeveRegistrarLogInformationNoInicio()
    {
        var fakeApi = new FakePokeApi();
        var fakeLogger = new FakeLogger<PokeApiClient>();
        var client = new PokeApiClient(fakeApi, fakeLogger);

        await client.GetPokemonAsync("pikachu");

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Executar"));
    }

    [Fact]
    public async Task GetPokemonAsync_DeveRegistrarLogInformationNoRetorno()
    {
        var fakeApi = new FakePokeApi();
        var fakeLogger = new FakeLogger<PokeApiClient>();
        var client = new PokeApiClient(fakeApi, fakeLogger);

        await client.GetPokemonAsync("pikachu");

        var logs = fakeLogger.GetSnapshot();
        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar"));
    }

    [Fact]
    public async Task GetPokemonAsync_DeveRegistrarLogsComPrefixoCorreto()
    {
        var fakeApi = new FakePokeApi();
        var fakeLogger = new FakeLogger<PokeApiClient>();
        var client = new PokeApiClient(fakeApi, fakeLogger);

        await client.GetPokemonAsync("pikachu");

        var logs = fakeLogger.GetSnapshot();
        Assert.All(logs, l => Assert.Contains("PokeApiClient", l.Message));
    }

    [Fact]
    public async Task GetPokemonAsync_DeveDelegarChamadaParaRefitApi()
    {
        var fakeApi = new FakePokeApi();
        var fakeLogger = new FakeLogger<PokeApiClient>();
        var client = new PokeApiClient(fakeApi, fakeLogger);

        await client.GetPokemonAsync("pikachu");

        Assert.Equal("pikachu", fakeApi.LastName);
    }

    [Fact]
    public async Task GetPokemonAsync_DeveRetornarOutputDaApi()
    {
        var fakeApi = new FakePokeApi();
        var fakeLogger = new FakeLogger<PokeApiClient>();
        var client = new PokeApiClient(fakeApi, fakeLogger);

        var result = await client.GetPokemonAsync("pikachu");

        Assert.Equal(25, result.Id);
        Assert.Equal("pikachu", result.Name);
    }
}
