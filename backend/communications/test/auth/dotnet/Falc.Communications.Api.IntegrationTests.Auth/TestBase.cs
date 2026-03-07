using Falc.Communications.Api.Client;

namespace Falc.Communications.Api.IntegrationTests.Auth;

public abstract class TestBase
{
    protected HttpClient HttpClient { get; private set; } = null!;
    protected TokenClient TokenClient { get; private set; } = null!;
    protected CommunicationsHttpClient CommunicationsHttpClient { get; private set; } = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        HttpClient = AuthIntegrationTestContext.HttpClient;
        TokenClient = AuthIntegrationTestContext.TokenClient;
        CommunicationsHttpClient = AuthIntegrationTestContext.CommunicationsHttpClient;
    }

    [SetUp]
    protected void SetUp()
    {
        CommunicationsHttpClient.WithBearerToken(null);
    }
}
