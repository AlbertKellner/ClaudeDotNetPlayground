using System.Net.Http.Headers;

namespace Albert.Playground.ECS.AOT.Api.Shared.ExternalApi.GitHub;

public sealed class GitHubAuthenticationHandler(IConfiguration configuration) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = configuration["ExternalApi:GitHub:HttpRequest:Token"];

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        request.Headers.TryAddWithoutValidation("User-Agent", "Albert.Playground.ECS.AOT.Api");
        request.Headers.TryAddWithoutValidation("Accept", "application/vnd.github+json");

        return base.SendAsync(request, cancellationToken);
    }
}
