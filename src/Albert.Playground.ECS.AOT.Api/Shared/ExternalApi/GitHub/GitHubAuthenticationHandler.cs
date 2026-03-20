using System.Net.Http.Headers;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;

public sealed class GitHubAuthenticationHandler(IConfiguration configuration) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Albert.Playground.ECS.AOT.Api", "1.0"));

        var pat = configuration["ExternalApi:GitHub:HttpRequest:PersonalAccessToken"];

        if (!string.IsNullOrEmpty(pat))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("token", pat);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
