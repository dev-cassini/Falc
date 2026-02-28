namespace Falc.Communications.Api.ComponentTests.TestInfrastructure;

public class CommunicationsApiFactory(string postgresConnectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            var testConfiguration = new Dictionary<string, string?>
            {
                ["ConnectionStrings:Postgres"] = postgresConnectionString,
                ["ConnectionStrings:Rmq"] = "amqp://guest:guest@localhost",
                ["Authentication:Schemes:Bearer:Authority"] = TestJwtAuthHandler.ValidIssuer,
                ["Authentication:Schemes:Bearer:ValidIssuer"] = TestJwtAuthHandler.ValidIssuer,
                ["Authentication:Schemes:Bearer:ValidAudience"] = TestJwtAuthHandler.ValidAudience,
                ["Authentication:Schemes:Bearer:RequireHttpsMetadata"] = "false"
            };

            configBuilder.AddInMemoryCollection(testConfiguration);
        });

        builder.ConfigureTestServices(services =>
        {
            // Replace production auth with deterministic test JWT validation.
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestJwtAuthHandler.Scheme;
                    options.DefaultChallengeScheme = TestJwtAuthHandler.Scheme;
                })
                .AddScheme<AuthenticationSchemeOptions, TestJwtAuthHandler>(TestJwtAuthHandler.Scheme, _ => { });

            services.RemoveMassTransitHostedService();
            services.AddMassTransitTestHarness(configurator =>
            {
                configurator.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
            });
        });
    }
}
