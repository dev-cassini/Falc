using Falc.IdentityProvider.Api.Proxy.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UsePathBase("/api/idp-proxy");
app.RegisterEndpoints();
app.UseHttpsRedirection();

app.Run();

public partial class Program;
