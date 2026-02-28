using System.Net;

namespace Falc.Communications.Api.IntegrationTests.Auth;

[TestFixture]
public class SearchUsersAuthIntegrationTests
{
    private AuthIntegrationTestSettings _settings = null!;
    private HttpClient _httpClient = null!;
    private TokenClient _tokenClient = null!;
    private CommunicationsApiClient _communicationsApiClient = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _settings = await SettingsLoader.LoadAsync();
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_settings.BaseUrl),
            Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds)
        };

        _tokenClient = new TokenClient(_httpClient, _settings);
        _communicationsApiClient = new CommunicationsApiClient(_httpClient);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _httpClient.Dispose();
    }

    [SetUp]
    public void SetUp()
    {
        TokenClient.SetBearerToken(_httpClient, null);
    }

    [Test]
    [Category("integration")]
    [Category("authn")]
    [Category("communications")]
    public async Task SearchUsers_Returns401_WhenTokenIsMissing()
    {
        using var response = await _communicationsApiClient.SearchUsersAsync(
            new { pageNumber = 1, pageSize = 25 },
            CancellationToken.None);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("integration")]
    [Category("authn")]
    [Category("communications")]
    public async Task SearchUsers_Returns401_WhenTokenIsInvalid()
    {
        TokenClient.SetBearerToken(_httpClient, "this-is-not-a-valid-jwt");

        using var response = await _communicationsApiClient.SearchUsersAsync(
            new { pageNumber = 1, pageSize = 25 },
            CancellationToken.None);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("integration")]
    [Category("authz")]
    [Category("communications")]
    public async Task SearchUsers_Returns403_WhenRoleIsMissing()
    {
        var nonAdminToken = await _tokenClient.GetNonAdminTokenAsync(CancellationToken.None);
        TokenClient.SetBearerToken(_httpClient, nonAdminToken);

        using var response = await _communicationsApiClient.SearchUsersAsync(
            new { pageNumber = 1, pageSize = 25 },
            CancellationToken.None);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    [Category("integration")]
    [Category("authz")]
    [Category("communications")]
    public async Task SearchUsers_Returns200_WhenAdminRoleIsPresent()
    {
        var adminToken = await _tokenClient.GetAdminTokenAsync(CancellationToken.None);
        TokenClient.SetBearerToken(_httpClient, adminToken);

        using var response = await _communicationsApiClient.SearchUsersAsync(
            new { pageNumber = 1, pageSize = 25 },
            CancellationToken.None);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var payload = await CommunicationsApiClient.ReadSearchUsersResponseAsync(response, CancellationToken.None);
        Assert.That(payload, Is.Not.Null);
        Assert.That(payload!.PageNumber, Is.EqualTo(1));
        Assert.That(payload.PageSize, Is.EqualTo(25));
    }
}
