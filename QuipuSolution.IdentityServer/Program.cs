using Microsoft.AspNetCore.DataProtection;
using QuipuSolution.IdentityServer;

var builder = WebApplication.CreateBuilder(args);

var settings = AuthServerSettings.Load(builder.Configuration);
builder.Services.AddSingleton(settings);

if (!string.IsNullOrWhiteSpace(settings.DataProtectionKeysPath))
{
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(settings.DataProtectionKeysPath))
        .SetApplicationName("QuipuSolution.IdentityServer");
}

// Login UI for the implicit flow
builder.Services.AddRazorPages();

builder.Services.AddIdentityServer(options =>
    {
        options.UserInteraction.LoginUrl = "/Account/Login";
        options.UserInteraction.ErrorUrl = "/Error";

        if (!string.IsNullOrWhiteSpace(settings.IssuerUri))
            options.IssuerUri = settings.IssuerUri;

        // Lax works for the redirect-based flows used here and, unlike None, also over plain HTTP (Docker)
        options.Authentication.CookieSameSiteMode = SameSiteMode.Lax;
        options.Authentication.CheckSessionCookieSameSiteMode = SameSiteMode.Lax;
    })
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddInMemoryApiResources(Config.ApiResources)
    .AddInMemoryClients(Config.GetClients(settings))
    .AddTestUsers(Config.GetUsers(settings));

builder.Services.AddHealthChecks();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseSecurityHeaders();

app.UseRouting();

app.UseIdentityServer();
app.UseAuthorization();

app.MapHealthChecks("/health/live");
app.MapRazorPages();

app.Run();
