using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using QuipuSolution.Application;
using QuipuSolution.Configuration;
using QuipuSolution.ExceptionHandling;
using QuipuSolution.Extensions;
using QuipuSolution.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var identityServerSettings = builder.Configuration.GetIdentityServerSettings();
var swaggerEnabled = builder.Configuration.GetValue<bool>("Swagger:Enabled");

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionHandler>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApiAuthentication(identityServerSettings);
builder.Services.AddApiRateLimiting(builder.Configuration);

if (swaggerEnabled)
{
    builder.Services.AddApiSwagger(identityServerSettings);
}

var app = builder.Build();

// In production, migrations are a deployment step - enable this only for local/demo environments
if (app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
{
    await app.Services.ApplyMigrationsAsync();
}

app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseSecurityHeaders();

if (swaggerEnabled)
{
    app.UseApiSwagger();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

// Liveness: the process is running. Readiness: the database is reachable.
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains(QuipuSolution.Infrastructure.DependencyInjection.ReadinessTag) });

app.MapControllers().RequireRateLimiting(RateLimitingExtensions.ApiPolicy);

app.Run();

// Makes the entry point visible to the integration tests (WebApplicationFactory<Program>)
public partial class Program { }
