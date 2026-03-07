using Falc.Communications.Api.Client.Contracts;
using System.Net;
using System.Net.Http.Json;

namespace Falc.Communications.Api.IntegrationTests.Auth;

[TestFixture]
public class SearchUsersAuthIntegrationTests : TestBase
{
    [Test]
    [Category("integration")]
    [Category("authn")]
    [Category("communications")]
    public async Task SearchUsers_Returns401_WhenTokenIsMissing()
    {
        using var response = await CommunicationsHttpClient
            .WithBearerToken(null)
            .SearchUsersAsync(new SearchUsersRequest(PageNumber: 1, PageSize: 25), CancellationToken.None);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("integration")]
    [Category("authn")]
    [Category("communications")]
    public async Task SearchUsers_Returns401_WhenTokenIsInvalid()
    {
        using var response = await CommunicationsHttpClient
            .WithBearerToken("this-is-not-a-valid-jwt")
            .SearchUsersAsync(new SearchUsersRequest(PageNumber: 1, PageSize: 25), CancellationToken.None);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("integration")]
    [Category("authz")]
    [Category("communications")]
    public async Task SearchUsers_Returns403_WhenRoleIsMissing()
    {
        using var response = await CommunicationsHttpClient
            .AuthenticateAsNonAdmin()
            .SearchUsersAsync(new SearchUsersRequest(PageNumber: 1, PageSize: 25), CancellationToken.None);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("integration")]
    [Category("authz")]
    [Category("communications")]
    public async Task SearchUsers_Returns200_WhenAdminRoleIsPresent()
    {
        using var response = await CommunicationsHttpClient
            .AuthenticateAsAdmin()
            .SearchUsersAsync(new SearchUsersRequest(PageNumber: 1, PageSize: 25), CancellationToken.None);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var payload = await response.Content.ReadFromJsonAsync<SearchUsersResponse>(cancellationToken: CancellationToken.None);
        Assert.That(payload, Is.Not.Null);
        Assert.That(payload!.PageNumber, Is.EqualTo(1));
        Assert.That(payload.PageSize, Is.EqualTo(25));
    }
}
