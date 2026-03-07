using Falc.Communications.Api.Client;

namespace Falc.Communications.Api.IntegrationTests.Auth;

[SetUpFixture]
public sealed class AuthIntegrationTestContext
{
    public static AuthIntegrationTestSettings Settings { get; private set; } = null!;
    public static HttpClient HttpClient { get; private set; } = null!;
    public static TokenClient TokenClient { get; private set; } = null!;
    public static CommunicationsHttpClient CommunicationsHttpClient { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Settings = await SettingsLoader.LoadAsync();

        HttpClient = new HttpClient
        {
            BaseAddress = new Uri(Settings.BaseUrl),
            Timeout = TimeSpan.FromSeconds(Settings.TimeoutSeconds)
        };

        TokenClient = new TokenClient(HttpClient, Settings);
        CommunicationsHttpClient = new CommunicationsHttpClient(HttpClient);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        HttpClient.Dispose();
    }
}

