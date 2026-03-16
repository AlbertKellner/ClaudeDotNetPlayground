using System.Net;
using System.Net.Http.Json;
using ClaudeDotNetPlayground.Shared.ExternalApi.OpenMeteo;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ClaudeDotNetPlayground.Tests.Features.Integration;

public sealed class WeatherConditionsAuthenticatedTests : IClassFixture<WeatherConditionsAuthenticatedTests.AppFactory>
{
    public sealed class AppFactory : WebApplicationFactory<Program>
    {
        private sealed class FakeOpenMeteoApiClient : IOpenMeteoApiClient
        {
            public Task<OpenMeteoOutput> GetForecastAsync(
                OpenMeteoInput input,
                CancellationToken cancellationToken = default)
            {
                return Task.FromResult(new OpenMeteoOutput
                {
                    Latitude = -23.5475,
                    Longitude = -46.6361,
                    Timezone = "America/Sao_Paulo"
                });
            }
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.Single(d => d.ServiceType == typeof(IOpenMeteoApiClient));

                services.Remove(descriptor);

                services.AddScoped<IOpenMeteoApiClient, FakeOpenMeteoApiClient>();
            });
        }
    }

    private sealed record LoginResponse(string Token);

    private readonly AppFactory _factory;

    public WeatherConditionsAuthenticatedTests(AppFactory factory) => _factory = factory;

    [Fact]
    public async Task ConsultarCondicoesDoTempo_ComTokenGeradoViaLogin_DeveRetornarOk()
    {
        var client = _factory.CreateClient();

        var loginResponse = await client.PostAsJsonAsync("/login", new { userName = "Albert", password = "albert123" });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(loginResult?.Token);

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);

        var weatherResponse = await client.GetAsync("/weather-conditions");

        Assert.Equal(HttpStatusCode.OK, weatherResponse.StatusCode);
    }

    [Fact]
    public async Task ConsultarCondicoesDoTempo_SemToken_DeveRetornar401()
    {
        var client = _factory.CreateClient();

        var weatherResponse = await client.GetAsync("/weather-conditions");

        Assert.Equal(HttpStatusCode.Unauthorized, weatherResponse.StatusCode);
    }
}
