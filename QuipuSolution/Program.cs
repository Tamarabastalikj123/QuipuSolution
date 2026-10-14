using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using QuipuSolution.Application.Contracts.Repository.Authors;
using QuipuSolution.Application.Contracts.Repository.Books;
using QuipuSolution.Application.Contracts.Services;
using QuipuSolution.Application.Service.Books;
using QuipuSolution.ExceptionHandling;
using QuipuSolution.Infrastructure.Persistence;
using QuipuSolution.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Authentication - validate JWT tokens issued by IdentityServer
var identityServer = builder.Configuration.GetSection("IdentityServer");
var authority = identityServer["Authority"]!;
var scope = identityServer["Scope"]!;

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            ClientCredentials = new OpenApiOAuthFlow
            {
                TokenUrl = new Uri($"{authority}/connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    [scope] = "Access to the Books API"
                }
            }
        }
    });

    // Send the token with every request
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { scope }
        }
    });
});
builder.Services.AddDbContext<QuipuSolutionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authority;                                 
        options.Audience = identityServer["Audience"];
    });

// Authorization - the token must contain the "book.api" scope
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BookApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", scope);                              
    });
});

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var db = serviceScope.ServiceProvider.GetRequiredService<QuipuSolutionDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler();
//app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>                                        
    {
        options.OAuthClientId("quipu.client");
    });
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
