using System.Text.Json;
using Falc.Communications.Api.ComponentTests.TestInfrastructure;
using Falc.Communications.Domain.Tooling;

namespace Falc.Communications.Api.ComponentTests;

[TestFixture]
public class SearchUsersEndpointTests
{
    private PostgreSqlContainer _postgresContainer = null!;
    private CommunicationsApiFactory _factory = null!;
    private HttpClient _client = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("communications_component_tests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithCleanUp(true)
            .Build();

        await _postgresContainer.StartAsync();

        _factory = new CommunicationsApiFactory(_postgresContainer.GetConnectionString());
        _client = _factory.CreateClient();
    }

    [SetUp]
    public async Task SetUp()
    {
        await ResetUsersAsync();
        _client.DefaultRequestHeaders.Authorization = null;
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgresContainer.DisposeAsync();
    }

    [Test]
    public async Task SearchUsers_Returns401_WhenAuthHeaderMissing()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/communications/users/search",
            new { emailAddress = "a@b.com", pageNumber = 1, pageSize = 25 });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task SearchUsers_Returns401_WhenJwtTokenIsInvalid()
    {
        SetBearerToken(TestJwtTokenFactory.CreateInvalidToken(Guid.NewGuid()));

        var response = await _client.PostAsJsonAsync(
            "/api/communications/users/search",
            new { emailAddress = "a@b.com", pageNumber = 1, pageSize = 25 });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task SearchUsers_Returns403_WhenAdminRoleClaimIsMissing()
    {
        SetBearerToken(TestJwtTokenFactory.CreateValidTokenWithoutRole(Guid.NewGuid()));

        var response = await _client.PostAsJsonAsync(
            "/api/communications/users/search",
            new { emailAddress = "a@b.com", pageNumber = 1, pageSize = 25 });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public async Task SearchUsers_Returns400_WhenPageNumberIsLessThanOne(int pageNumber)
    {
        SetBearerToken(TestJwtTokenFactory.CreateValidToken(Guid.NewGuid()));

        var response = await _client.PostAsJsonAsync(
            "/api/communications/users/search",
            new { emailAddress = "hello@example.com", pageNumber, pageSize = 25 });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var content = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);
        Assert.That(document.RootElement.GetProperty("errors").TryGetProperty("PageNumber", out _), Is.True);
    }

    [TestCase(0)]
    [TestCase(101)]
    public async Task SearchUsers_Returns400_WhenPageSizeIsOutOfRange(int pageSize)
    {
        SetBearerToken(TestJwtTokenFactory.CreateValidToken(Guid.NewGuid()));

        var response = await _client.PostAsJsonAsync(
            "/api/communications/users/search",
            new { emailAddress = "hello@example.com", pageNumber = 1, pageSize });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var content = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);
        Assert.That(document.RootElement.GetProperty("errors").TryGetProperty("PageSize", out _), Is.True);
    }

    [Test]
    public async Task SearchUsers_Returns200WithEmptyResult_WhenEmailAddressNotProvided()
    {
        SetBearerToken(TestJwtTokenFactory.CreateValidToken(Guid.NewGuid()));

        var response = await _client.PostAsJsonAsync(
            "/api/communications/users/search",
            new { pageNumber = 1, pageSize = 25 });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var result = await response.Content.ReadFromJsonAsync<SearchUsers.Result>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Users, Is.Empty);
        Assert.That(result.TotalCount, Is.EqualTo(0));
        Assert.That(result.PageNumber, Is.EqualTo(1));
        Assert.That(result.PageSize, Is.EqualTo(25));
    }

    [Test]
    public async Task SearchUsers_Returns200_WhenEmailAddressProvided()
    {
        await SeedUserAsync("target@example.com");
        await SeedUserAsync("other@example.com");

        SetBearerToken(TestJwtTokenFactory.CreateValidToken(Guid.NewGuid()));

        var response = await _client.PostAsJsonAsync(
            "/api/communications/users/search",
            new { emailAddress = "target@example.com", pageNumber = 1, pageSize = 25 });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var result = await response.Content.ReadFromJsonAsync<SearchUsers.Result>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.TotalCount, Is.EqualTo(1));
        Assert.That(result.Users.Count, Is.EqualTo(1));
        Assert.That(result.Users.Single().Email, Is.EqualTo("target@example.com"));
    }

    [Test]
    public async Task SearchUsers_IsCaseInsensitive_ForExactEmailMatch()
    {
        await SeedUserAsync("mixed.case@example.com");
        await SeedUserAsync("mixed.case@example.com.extra");

        SetBearerToken(TestJwtTokenFactory.CreateValidToken(Guid.NewGuid()));

        var response = await _client.PostAsJsonAsync(
            "/api/communications/users/search",
            new { emailAddress = "MIXED.CASE@EXAMPLE.COM", pageNumber = 1, pageSize = 25 });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var result = await response.Content.ReadFromJsonAsync<SearchUsers.Result>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.TotalCount, Is.EqualTo(1));
        Assert.That(result.Users.Count, Is.EqualTo(1));
        Assert.That(result.Users.Single().Email, Is.EqualTo("mixed.case@example.com"));
    }

    private void SetBearerToken(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task ResetUsersAsync()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CommunicationsDbContext>();
        dbContext.Users.RemoveRange(dbContext.Users);
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedUserAsync(string emailAddress)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CommunicationsDbContext>();
        var user = new User(
            Guid.NewGuid(),
            emailAddress,
            new MarketingPreferences(false, false, false),
            new DefaultDateTimeProvider());
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }
}
