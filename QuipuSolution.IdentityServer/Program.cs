using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using QuipuSolution.IdentityServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityServer()
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddInMemoryApiResources(Config.ApiResources)
    .AddInMemoryClients(Config.Clients);

var app = builder.Build();

app.UseIdentityServer();

app.Run();

