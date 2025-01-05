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

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddAuthorization();

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services
    .AddDomainTooling()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

DockerHelpers.UpdateCaCertificates();

app.RegisterEndpoints();
app
    .UseAuthentication()
    .UseRouting()
    .UseAuthorization();

app.Services
    .UseInfrastructure();

app.UseHttpsRedirection();

app.Run();