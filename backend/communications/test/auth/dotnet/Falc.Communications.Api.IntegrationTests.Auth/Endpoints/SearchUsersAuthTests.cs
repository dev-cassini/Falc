using System.Net;
using System.Net.Http.Json;
using Falc.Communications.Api.Client.Contracts;

namespace Falc.Communications.Api.IntegrationTests.Auth.Endpoints;

[TestFixture]
public class SearchUsersAuthTests : EndpointTestBase
{
    [Test]
    [Category("integration")]
    [Category("authn")]
    [Category("communications")]
    public async Task GivenNoBearerToken_WhenSearchUsersCalled_ThenReturns401()
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
    public async Task GivenInvalidBearerToken_WhenSearchUsersCalled_ThenReturns401()
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
    public async Task GivenCustomerUserToken_WhenSearchUsersCalled_ThenReturns403()
    {
        using var response = await CommunicationsHttpClient
            .AuthenticateAsCustomer()
            .SearchUsersAsync(new SearchUsersRequest(PageNumber: 1, PageSize: 25), CancellationToken.None);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("integration")]
    [Category("authz")]
    [Category("communications")]
    public async Task GivenAdminUserToken_WhenSearchUsersCalled_ThenReturns200()
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
