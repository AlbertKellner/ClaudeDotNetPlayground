using Albert.Playground.ECS.AOT.Api.Infra.Security;
using Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;
using Albert.Playground.ECS.AOT.UnitTest.TestHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Albert.Playground.ECS.AOT.UnitTest.Shared.ExternalApi.GitHub;

public sealed class CachedGitHubApiClientTests
{
    private sealed class FakeGitHubApi : IGitHubApi
    {
        public int CallCount { get; private set; }

        public Task<List<GitHubRepositoryOutput>> GetRepositoriesAsync(
            string username,
            int per_page = 100,
            int page = 1,
            CancellationToken cancellationToken = default)
        {
            CallCount++;

            var result = page == 1
                ? new List<GitHubRepositoryOutput>
                {
                    new()
                    {
                        Name = "repo-1",
                        FullName = "AlbertKellner/repo-1",
                        GitUrl = "git://github.com/AlbertKellner/repo-1.git"
                    }
                }
                : [];

            return Task.FromResult(result);
        }
    }

    private sealed class FakeHttpContextAccessor(HttpContext httpContext) : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; } = httpContext;
    }

    private static (CachedGitHubApiClient cached, FakeGitHubApi fakeApi, FakeLogger<CachedGitHubApiClient> logger) CreateSut(
        int userId = 1,
        int durationSeconds = 60)
    {
        var fakeApi = new FakeGitHubApi();
        var innerLogger = new FakeLogger<GitHubApiClient>();
        var innerClient = new GitHubApiClient(fakeApi, innerLogger);

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var httpContext = new DefaultHttpContext();
        httpContext.Items[AuthenticateFilter.AuthenticatedUserItemKey] = new AuthenticatedUser(userId, "testuser");
        var httpContextAccessor = new FakeHttpContextAccessor(httpContext);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ExternalApi:GitHub:EndpointCache:GitHubRepoSearch:DurationSeconds"] = durationSeconds.ToString()
            })
            .Build();

        var logger = new FakeLogger<CachedGitHubApiClient>();

        var cached = new CachedGitHubApiClient(innerClient, memoryCache, httpContextAccessor, logger, configuration);
        return (cached, fakeApi, logger);
    }

    [Fact]
    public async Task GetRepositoriesAsync_CacheMiss_DeveChamarClienteInterno()
    {
        var (cached, fakeApi, _) = CreateSut();

        await cached.GetRepositoriesAsync("AlbertKellner");

        Assert.Equal(2, fakeApi.CallCount);
    }

    [Fact]
    public async Task GetRepositoriesAsync_CacheHit_NaoDeveChamarClienteInternoNovamente()
    {
        var (cached, fakeApi, _) = CreateSut();

        await cached.GetRepositoriesAsync("AlbertKellner");
        await cached.GetRepositoriesAsync("AlbertKellner");

        Assert.Equal(2, fakeApi.CallCount);
    }

    [Fact]
    public async Task GetRepositoriesAsync_CacheHit_DeveRetornarMesmoResultado()
    {
        var (cached, _, _) = CreateSut();

        var first = await cached.GetRepositoriesAsync("AlbertKellner");
        var second = await cached.GetRepositoriesAsync("AlbertKellner");

        Assert.Same(first, second);
    }

    [Fact]
    public async Task GetRepositoriesAsync_UsuariosDiferentes_DevemTerCachesSeparados()
    {
        var fakeApi = new FakeGitHubApi();
        var innerLogger = new FakeLogger<GitHubApiClient>();
        var innerClient = new GitHubApiClient(fakeApi, innerLogger);
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ExternalApi:GitHub:EndpointCache:GitHubRepoSearch:DurationSeconds"] = "60"
            })
            .Build();

        var httpContext1 = new DefaultHttpContext();
        httpContext1.Items[AuthenticateFilter.AuthenticatedUserItemKey] = new AuthenticatedUser(1, "user1");
        var accessor1 = new FakeHttpContextAccessor(httpContext1);
        var logger1 = new FakeLogger<CachedGitHubApiClient>();
        var cached1 = new CachedGitHubApiClient(innerClient, memoryCache, accessor1, logger1, configuration);

        var httpContext2 = new DefaultHttpContext();
        httpContext2.Items[AuthenticateFilter.AuthenticatedUserItemKey] = new AuthenticatedUser(2, "user2");
        var accessor2 = new FakeHttpContextAccessor(httpContext2);
        var logger2 = new FakeLogger<CachedGitHubApiClient>();
        var cached2 = new CachedGitHubApiClient(innerClient, memoryCache, accessor2, logger2, configuration);

        await cached1.GetRepositoriesAsync("AlbertKellner");
        await cached2.GetRepositoriesAsync("AlbertKellner");

        Assert.Equal(4, fakeApi.CallCount);
    }

    [Fact]
    public async Task GetRepositoriesAsync_CacheMiss_DeveRegistrarLogDeCacheMiss()
    {
        var (cached, _, logger) = CreateSut();

        await cached.GetRepositoriesAsync("AlbertKellner");

        var logs = logger.GetSnapshot();

        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Cache miss"));
    }

    [Fact]
    public async Task GetRepositoriesAsync_CacheHit_DeveRegistrarLogDeCacheHit()
    {
        var (cached, _, logger) = CreateSut();

        await cached.GetRepositoriesAsync("AlbertKellner");
        await cached.GetRepositoriesAsync("AlbertKellner");

        var logs = logger.GetSnapshot();

        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Retornar resposta do cache"));
    }

    [Fact]
    public async Task GetRepositoriesAsync_DeveRegistrarLogsComPrefixoCorreto()
    {
        var (cached, _, logger) = CreateSut();

        await cached.GetRepositoriesAsync("AlbertKellner");

        var logs = logger.GetSnapshot();

        Assert.All(logs, l => Assert.Contains("CachedGitHubApiClient", l.Message));
    }

    [Fact]
    public async Task GetRepositoriesAsync_DeveRegistrarLogDeArmazenamentoNoCache()
    {
        var (cached, _, logger) = CreateSut();

        await cached.GetRepositoriesAsync("AlbertKellner");

        var logs = logger.GetSnapshot();

        Assert.Contains(logs, l =>
            l.Level == LogLevel.Information &&
            l.Message.Contains("Armazenar resposta no cache"));
    }
}
