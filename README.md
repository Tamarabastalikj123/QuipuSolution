REST API with CRUD operations for **Books**, protected by OAuth 2.0 Client Credentials flow using Duende IdentityServer.

## Tech stack

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core 8 (code-first, SQL Server LocalDB)
- Duende IdentityServer 7 (OAuth 2.0 token server)
- Swagger / Swashbuckle (with OAuth2 support)

## Solution structure


QuipuSolution.sln
├── QuipuSolution.Domain           # Entities (Book, Author) and business rules
├── QuipuSolution.Application      # DTOs, service (use cases), repository interfaces
├── QuipuSolution.Infrastructure   # EF Core DbContext, configurations, migrations, repositories
├── QuipuSolution                  # Web API: controller, exception handling, auth, Swagger
└── QuipuSolution.IdentityServer   # OAuth 2.0 token server (client credentials)

Dependencies point inward: Api → Application → Domain, Infrastructure → Application → Domain.
The Domain has no dependencies on other projects or frameworks.

## Prerequisites

- .NET 8 SDK
- SQL Server LocalDB (installed with Visual Studio, *Data storage and processing* workload)
- Trusted HTTPS development certificate: dotnet dev-certs https --trust

## Getting started

The database is created automatically on first run (migrations are applied at startup), including seed data.

Both projects must run at the same time.

### Visual Studio

1. Right-click the solution → Configure Startup Projects** → Multiple startup projects
2. Set `QuipuSolution.IdentityServer` and `QuipuSolution` to Start (IdentityServer first)
3. Press F5

### Command line

powershell
# Terminal 1
dotnet run --project QuipuSolution.IdentityServer --launch-profile https

# Terminal 2
dotnet run --project QuipuSolution --launch-profile https


| Application    | URL                                  |
|----------------|--------------------------------------|
| IdentityServer | https://localhost:5001               |
| API (Swagger)  | https://localhost:7107/swagger       |

## Authentication

All endpoints require a valid access token issued by IdentityServer, containing the `book.api` scope.

| Setting         | Value          |
|-----------------|----------------|
| Grant type      | `client_credentials` |
| Token endpoint  | `https://localhost:5001/connect/token` |
| Client ID       | `quipu.client` |
| Client secret   | `quipu-secret` |
| Scope           | `book.api`     |

> These credentials are for local development only.

### Testing with Swagger

1. Open https://localhost:7107/swagger
2. Click Authorize
3. Enter client secret `quipu-secret` (client ID is pre-filled), tick the `book.api` scope, click Authorize
4. Use Try it out on any endpoint


## API endpoints

| Method | Route                  | Description        | Success |
|--------|------------------------|--------------------|---------|
| GET    | `/api/book`            | Get all books      | 200     |
| GET    | `/api/book/{bookId}`   | Get a book by id   | 200     |
| POST   | `/api/book`            | Create a book      | 201     |
| PUT    | `/api/book/{bookId}`   | Update a book      | 200     |
| DELETE | `/api/book/{bookId}`   | Delete a book      | 204     |

### Request body (POST / PUT)

```json
{
  "title": "Crime and Punishment",
  "subTitle": null,
  "authorId": 1
}
```

### Response

```json
{
  "bookId": 1,
  "author": {
    "authorId": 1,
    "name": "Fyodor Dostoevsky"
  },
  "title": "Crime and Punishment",
  "subTitle": null
}
```

## Object model

| Entity | Field      | Type   | Rules |
|--------|------------|--------|-------|
| Author | `authorId` | int32  | required |
| Author | `name`     | string | required, 3–100 characters |
| Book   | `bookId`   | int32  | required |
| Book   | `author`   | Author | required |
| Book   | `title`    | string | required, 3–100 characters |
| Book   | `subTitle` | string | optional |

Validation rules are enforced in the Domain entities and in the database schema.

## Error handling

Errors are returned as 

| Status | When |
|--------|------|
| 400    | Validation failed (e.g. title shorter than 3 characters) |
| 401    | Missing, invalid or expired access token |
| 403    | Token is valid but does not contain the `book.api` scope |
| 404    | Book or author not found |
| 500    | Unexpected error (details are logged, not returned) |

```json
{
  "status": 404,
  "title": "Not Found",
  "detail": "Book with id 999 was not found."
}
```

## Seed data

The following authors are seeded (the API exposes CRUD for books only, so books reference these authors):

| authorId | name              |
|----------|-------------------|
| 1        | Fyodor Dostoevsky |
| 2        | Katerina Shoshko  |
| 3        | Frida Mcfadden    |
| 4        | Chris Carter      |

Four sample books are seeded as well.

## Notes

- Duende license:** IdentityServer logs a license warning at startup. No license key is required for development and testing, so the warning can be ignored.
- IdentityServer configuration** (clients, scopes) is kept in memory (`Config.cs`), which is sufficient for the client credentials flow.
- Database migrations** can also be applied manually:
  powershell
  dotnet ef database update --project QuipuSolution.Infrastructure --startup-project QuipuSolution
  

Check before committing

The README makes promises, so verify each one:

- [ ] Fixes 1–4 done, and dotnet build succeeds
- [ ] Swagger Authorize flow works
- [ ] GET /api/book/999 returns 404 (not 500)
- [ ] POST with "title": "ab" returns 400