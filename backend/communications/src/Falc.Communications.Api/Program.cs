using Falc.Communications.Api.Authentication.Schemes;
using Falc.Communications.Api.Endpoints;
using Falc.Communications.Api.Tooling;
using Falc.Communications.Domain;
using Falc.Communications.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services.Configure<HmacAuthenticationScheme.Configuration>(builder.Configuration.GetSection("Authentication:Schemes:Hmac"));

builder.Services
    .AddCors(options =>
    {
        options.AddDefaultPolicy(policyBuilder =>
        {
            policyBuilder
                .WithOrigins("https://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    })
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddScheme<HmacAuthenticationScheme.Configuration, HmacAuthenticationScheme.Handler>(HmacAuthenticationScheme.Name, _ => {})
    .AddJwtBearer();

builder.Services.AddAuthorization();

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services
    .AddDomainTooling()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (bool.TryParse(app.Configuration["DOTNET_RUNNING_IN_CONTAINER"], out var runningInContainer) && runningInContainer)
{
    DockerHelpers.UpdateCaCertificates();
}

app.RegisterEndpoints();
app
    .UseAuthentication()
    .UseRouting()
    .UseCors()
    .UseAuthorization();

app.Services
    .UseInfrastructure();

app.UseHttpsRedirection();

app.Run();