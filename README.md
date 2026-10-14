# Quipu Books API

A REST API for managing and searching books, secured with OAuth 2.0 by a Duende IdentityServer token server.

- **CRUD for books**: protected by the **client credentials** flow (machine-to-machine)
- **Book search** by title and author, with **pagination**: protected by the **implicit** flow (a user logs in)
- Testable from the embedded **Swagger UI**, which is configured for both flows
- Runs from Visual Studio / the command line (SQL Server LocalDB) or fully in **Docker** (SQL Server container)

## Contents

- [Tech stack](#tech-stack)
- [Architecture](#architecture)
- [Running the solution](#running-the-solution)
- [Authentication](#authentication)
- [Testing the API with Swagger](#testing-the-api-with-swagger)
- [API reference](#api-reference)
- [Configuration](#configuration)
- [Tests and CI](#tests-and-ci)
- [Production readiness](#production-readiness)
- [Design decisions](#design-decisions)
- [Possible next steps](#possible-next-steps)

## Tech stack

| Area | Technology |
|------|------------|
| Runtime | .NET 8, ASP.NET Core Web API |
| Data access | Entity Framework Core 8, code-first migrations, SQL Server |
| Token server | Duende IdentityServer 7 |
| API documentation | Swagger / Swashbuckle with OAuth 2.0 |
| Tests | xUnit v3, NSubstitute, ASP.NET Core `WebApplicationFactory`, SQLite in-memory |
| Containers | Docker, Docker Compose |
| CI | GitHub Actions |

## Architecture

```
QuipuSolution.sln
├── QuipuSolution.Domain            Entities (Book, Author) and their business rules
├── QuipuSolution.Application       Use cases (BookService), DTOs, repository interfaces
├── QuipuSolution.Infrastructure    EF Core DbContext, mappings, migrations, repositories
├── QuipuSolution                   Web API: controllers, authentication, Swagger, error handling
├── QuipuSolution.IdentityServer    OAuth 2.0 token server with login page
└── tests
    ├── QuipuSolution.UnitTests         Domain rules and BookService
    └── QuipuSolution.IntegrationTests  The real API in memory: endpoints, security, Swagger
```

Clean architecture: dependencies point inward.

```
QuipuSolution (API) ──► Application ──► Domain
Infrastructure ───────► Application
```

- **Domain** has no dependencies. Entities protect their own rules (for example, a title must be 3–100 characters), so an invalid book cannot be created.
- **Application** defines what the system does and the interfaces it needs (`IBookRepository`). It knows nothing about EF Core or HTTP.
- **Infrastructure** implements those interfaces with EF Core and SQL Server.
- **API** is a thin HTTP layer. It translates exceptions into HTTP responses (`NotFoundException` → 404, `DomainException` → 400).
- **IdentityServer** is a separate application. The API never sees passwords or secrets; it only validates the tokens IdentityServer issues.

## Running the solution

Two options: **locally** (Visual Studio or the command line, with SQL Server LocalDB) or **Docker**.

### Option 1: Local (Visual Studio / command line)

**Prerequisites**

- .NET 8 SDK (or newer)
- SQL Server LocalDB, which is included in Visual Studio (*Data storage and processing* workload)
- A trusted HTTPS development certificate: `dotnet dev-certs https --trust`

The database `QuipuDB` is created in LocalDB automatically on the first start, including the seed data.

**IdentityServer and the API must both run.**

**Visual Studio**

1. Right-click the solution → **Configure Startup Projects…** → **Multiple startup projects**
2. Set `QuipuSolution.IdentityServer` and `QuipuSolution` to **Start**, with IdentityServer first
3. Make sure this multi-project profile is selected in the toolbar dropdown next to the ▶ Start button
4. Press **F5**. Two browser tabs open: IdentityServer and Swagger.

**Command line**

```powershell
# Terminal 1
dotnet run --project QuipuSolution.IdentityServer --launch-profile https

# Terminal 2
dotnet run --project QuipuSolution --launch-profile https
```

| Application | URL |
|-------------|-----|
| IdentityServer | https://localhost:5001 |
| API / Swagger UI | https://localhost:7107/swagger |

Check that both are running: https://localhost:5001/health/live and https://localhost:7107/health/ready should return `Healthy`.

### Option 2: Docker

**Prerequisites:** Docker Desktop (Windows / macOS) or Docker Engine with Compose (Linux).

```bash
docker compose up --build
```

This starts SQL Server, IdentityServer and the API. The API waits until SQL Server and IdentityServer are healthy, then creates the database and seed data.

| Application | URL |
|-------------|-----|
| IdentityServer | http://localhost:5101 |
| API / Swagger UI | http://localhost:5100/swagger |
| SQL Server | `localhost,1433` (user `sa`) |

To change the SQL Server password or the client secret, copy `.env.example` to `.env` and edit the values. Use `docker compose down -v` to stop and delete all data.

> Docker uses plain HTTP so it runs without certificates. In a real deployment, TLS is terminated by a reverse proxy or load balancer in front of the containers.

## Authentication

Each area of the API uses its own OAuth 2.0 flow and its own scope. A token for one area is rejected (403) by the other.

| | CRUD (`/api/book`) | Search (`/api/book/search`) |
|---|---|---|
| Flow | Client credentials | Implicit |
| Who | an application, no user | a user, who logs in |
| Client ID | `quipu.client` | `quipu.swagger` |
| Client secret | `quipu-secret` | none (browser clients cannot keep secrets) |
| Scope | `book.api` | `book.search` |
| Extra requirement | none | token must belong to a user (`sub` claim) |

**Test users** (login page of IdentityServer):

| Username | Password |
|----------|----------|
| `alice` | `alice` |
| `bob` | `bob` |

> All credentials above are for local development and the Docker demo only. In other environments, the client secret comes from configuration (for example, an environment variable) and the test users are disabled.

### About the implicit flow

The implicit flow is used here because the task requires it. Current OAuth security guidance ([RFC 9700, OAuth 2.0 Security Best Current Practice](https://www.rfc-editor.org/rfc/rfc9700)) recommends **not** using it any more, because the access token is returned in the browser URL, where it can leak through history, logs or scripts. For a production browser client, the replacement is the **Authorization Code flow with PKCE**. With this design, switching means changing one client definition in IdentityServer and the flow in Swagger; the API itself would not change.

## Testing the API with Swagger

Open the Swagger UI (https://localhost:7107/swagger locally, or http://localhost:5100/swagger in Docker) and click **Authorize**. The dialog has one section per flow.

**CRUD endpoints (`client_credentials` section)**

1. `client_id`: `quipu.client`, `client_secret`: `quipu-secret`
2. Tick the `book.api` scope, click **Authorize**, then **Close**
3. Use **Try it out** on any `/api/book` endpoint

**Search endpoint (`implicit` section)**

1. `client_id`: `quipu.swagger`
2. Tick the `book.search` scope and click **Authorize**
3. The IdentityServer login page opens. Sign in as `alice` / `alice`
4. You are sent back to Swagger. Click **Close** and try `GET /api/book/search`

Swagger sends the right token to each endpoint automatically.

### Without Swagger (command line)

Client credentials token and a CRUD call (PowerShell):

```powershell
$token = (Invoke-RestMethod -Method Post -Uri https://localhost:5001/connect/token -Body @{
    grant_type    = "client_credentials"
    client_id     = "quipu.client"
    client_secret = "quipu-secret"
    scope         = "book.api"
}).access_token

Invoke-RestMethod -Uri https://localhost:7107/api/book -Headers @{ Authorization = "Bearer $token" }
```

The same with curl:

```bash
curl -X POST https://localhost:5001/connect/token \
  -d "grant_type=client_credentials&client_id=quipu.client&client_secret=quipu-secret&scope=book.api"

curl https://localhost:7107/api/book -H "Authorization: Bearer <access_token>"
```

The file `QuipuSolution/QuipuSolution.http` contains ready-made requests for Visual Studio / VS Code. The implicit flow needs a browser login, so use Swagger for the search endpoint.

## API reference

### Endpoints

| Method | Route | Description | Success | Errors |
|--------|-------|-------------|---------|--------|
| GET | `/api/book` | All books | 200 | 401, 403 |
| GET | `/api/book/{bookId}` | One book | 200 | 401, 403, 404 |
| POST | `/api/book` | Create a book | 201 + `Location` | 400, 401, 403, 404 |
| PUT | `/api/book/{bookId}` | Update a book | 200 | 400, 401, 403, 404 |
| DELETE | `/api/book/{bookId}` | Delete a book | 204 | 401, 403, 404 |
| GET | `/api/book/search` | Search with pagination | 200 | 400, 401, 403 |
| GET | `/health/live` | The process is running (anonymous) | 200 | |
| GET | `/health/ready` | The database is reachable (anonymous) | 200 | 503 |

### Object model

| Entity | Field | Type | Rules |
|--------|-------|------|-------|
| Author | `authorId` | int32 | required |
| Author | `name` | string | required, 3–100 characters |
| Book | `bookId` | int32 | required |
| Book | `author` | Author | required |
| Book | `title` | string | required, 3–100 characters |
| Book | `subTitle` | string | optional |

The rules are enforced three times: by request validation (a clear 400), by the domain entities, and by the database schema.

### Create / update a book

```http
POST /api/book
Content-Type: application/json

{
  "title": "Crime and Punishment",
  "subTitle": null,
  "authorId": 1
}
```

Response:

```json
{
  "bookId": 1,
  "author": { "authorId": 1, "name": "Fyodor Dostoevsky" },
  "title": "Crime and Punishment",
  "subTitle": null
}
```

The API manages books only, so a book references one of the seeded authors:

| authorId | name |
|----------|------|
| 1 | Fyodor Dostoevsky |
| 2 | Katerina Shoshko |
| 3 | Frida Mcfadden |
| 4 | Chris Carter |

Four sample books are seeded as well.

### Search

```http
GET /api/book/search?title=crime&author=dosto&page=1&pageSize=10
```

| Parameter | Default | Description |
|-----------|---------|-------------|
| `title` | none | Part of the title, case-insensitive |
| `author` | none | Part of the author's name, case-insensitive |
| `page` | 1 | Page number, starting at 1 |
| `pageSize` | 10 | Items per page, 1–50 |

Both filters are optional. When both are given, a book must match both.

```json
{
  "items": [
    {
      "bookId": 1,
      "author": { "authorId": 1, "name": "Fyodor Dostoevsky" },
      "title": "Crime and Punishment",
      "subTitle": null
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 1,
  "totalPages": 1
}
```

### Errors

Errors are returned as [Problem Details (RFC 9457)](https://www.rfc-editor.org/rfc/rfc9457):

```json
{
  "title": "Not Found",
  "status": 404,
  "detail": "Book with id 999 was not found."
}
```

| Status | When |
|--------|------|
| 400 | Invalid input, for example a title shorter than 3 characters or `pageSize` above 50 |
| 401 | Missing, invalid or expired access token |
| 403 | Valid token, but for the other area of the API (wrong flow or scope) |
| 404 | Book or author not found |
| 429 | Rate limit exceeded (see the `Retry-After` header) |
| 500 | Unexpected error. The details are logged, never returned to the client |

## Configuration

All environment-specific values and secrets come from configuration (`appsettings.json`, `appsettings.{Environment}.json`, environment variables). Nothing environment-specific is hardcoded. The base `appsettings.json` contains safe production defaults; `appsettings.Development.json` contains the local development values. Required settings are validated at startup, and the application refuses to start if one is missing.

Environment variables use `__` as the separator, for example `IdentityServer__Authority`.

### API (`QuipuSolution/appsettings.json`)

| Setting | Description | Production default |
|---------|-------------|--------------------|
| `ConnectionStrings:DefaultConnection` | SQL Server connection string | *required* |
| `IdentityServer:Authority` | Token server URL, as reachable from the API | *required* |
| `IdentityServer:PublicAuthority` | Token server URL, as reachable from the browser (Swagger), if different | = Authority |
| `IdentityServer:RequireHttpsMetadata` | Require HTTPS to reach the token server | `true` |
| `IdentityServer:Audience`, `CrudScope`, `SearchScope` | Expected audience and scopes | `book-api`, `book.api`, `book.search` |
| `Database:ApplyMigrationsOnStartup` | Apply EF Core migrations at startup | `false` |
| `Swagger:Enabled` | Serve the Swagger UI | `false` |
| `RateLimiting:PermitLimit`, `WindowSeconds` | Requests per caller per window | `100` per `60` s |

### IdentityServer (`QuipuSolution.IdentityServer/appsettings.json`)

| Setting | Description | Production default |
|---------|-------------|--------------------|
| `AuthServer:ClientCredentialsClientSecret` | Secret of `quipu.client` (at least 8 characters) | *required* |
| `AuthServer:SwaggerOrigins` | Swagger UI origins allowed to request and receive tokens | *required* |
| `AuthServer:EnableTestUsers` | Enable the demo users alice and bob | `false` |
| `AuthServer:IssuerUri` | Fixed token issuer, needed when the server is reached under several addresses (Docker) | from request |
| `AuthServer:DataProtectionKeysPath` | Persistent folder for Data Protection keys | framework default |

### Database migrations

Locally and in Docker, migrations are applied automatically at startup. In production, apply them as a deployment step:

```powershell
dotnet ef database update --project QuipuSolution.Infrastructure --startup-project QuipuSolution
```

Add a new migration after changing the model:

```powershell
dotnet ef migrations add <Name> --project QuipuSolution.Infrastructure --startup-project QuipuSolution --output-dir Migrations
```

## Tests and CI

```powershell
dotnet test
```

| Project | What it covers |
|---------|----------------|
| `QuipuSolution.UnitTests` (31 tests) | Domain rules (length limits, trimming, required author) and `BookService` (mapping, not-found cases, paging) with mocked repositories |
| `QuipuSolution.IntegrationTests` (34 tests) | The real API in memory: all endpoints, validation, error responses, search and pagination, the authorization rules (401/403 for each flow), health checks, security headers, and the Swagger security configuration |

The integration tests replace SQL Server with an in-memory **SQLite** database (fast, and every test class gets a fresh database) and replace JWT validation with a header-based test login. The API's **real authorization policies** still decide every request. One difference to note: SQLite compares text case-sensitively and SQL Server (default collation) does not, so the search tests use matching case.

Code coverage (generated migration code excluded):

```powershell
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

**GitHub Actions** (`.github/workflows/ci.yml`) runs on every push and pull request:

1. **Build and test**: Release build with warnings treated as errors, all tests, coverage report as an artifact
2. **Docker smoke test**: `docker compose up --build`, waits for all health checks, gets a real token from IdentityServer and calls the API, and checks that requests without a token are rejected

## Production readiness

| Area | What is in place |
|------|------------------|
| **Security** | Separate scope and policy per area; the search policy requires a user. Secrets only in configuration. Signing keys and `.env` are excluded from git. Security headers (CSP, `X-Frame-Options`, `nosniff`, `Referrer-Policy`) and HSTS. Swagger is off in production. The login page only redirects back to valid authorization requests (no open redirects). |
| **Validation** | Request validation, domain rules and database constraints. Startup validation of all required settings (fail fast). |
| **Error handling** | Central exception handling, Problem Details responses, no internal details leaked. |
| **Resilience** | Per-caller rate limiting (user → client → IP). EF Core retries transient SQL errors. |
| **Observability** | `/health/live` and `/health/ready` (database check). JSON console logs with trace ids outside Development. |
| **Data** | Code-first migrations, configurable automatic migration, indexes and constraints from the EF model, `AsNoTracking` for reads, stable ordering for pagination. |
| **Containers** | Multi-stage Docker builds, non-root user, cached restore layer, health checks, startup order, volumes for the database and the signing keys. |
| **Code quality** | Clean architecture, settings shared in `Directory.Build.props`, central package versions in `Directory.Packages.props`, warnings as errors, no known vulnerable packages. |

## Design decisions

- **Authors are seeded, not managed.** The task asks for CRUD on books only. A book must reference an existing author, so the authors are part of the seed data.
- **Two controllers.** CRUD and search need different authorization policies. ASP.NET Core combines `[Authorize]` attributes (all must pass), so separate controllers keep each policy clean.
- **Search uses `LIKE '%…%'`** (EF Core `Contains`). It is simple and correct for this data size. For large catalogs, a full-text index would be the next step.
- **Repositories instead of using `DbContext` directly in the service.** This keeps the Application layer independent of EF Core and makes the service unit-testable.
- **In-memory IdentityServer configuration.** Clients and scopes are defined in code, while secrets and URLs come from configuration. A database-backed configuration store is only needed when clients change at runtime.
- **Test users.** The demo users are switched on by configuration only (Development and the Docker demo). A real deployment plugs in a proper user store, such as ASP.NET Core Identity.

## Possible next steps

- **Upgrade to .NET 10 (LTS).** .NET 8 reaches end of support on 10 November 2026.
- **Replace the implicit flow** with Authorization Code + PKCE (see [About the implicit flow](#about-the-implicit-flow)).
- **A real user store** for IdentityServer (ASP.NET Core Identity) and a signing key managed in a key vault.
- **OpenTelemetry** for distributed tracing and metrics.
- **API versioning** once a second version of the API is needed.
- **Distributed rate limiting** (for example with Redis) when the API runs on several instances.

## Notes

- **Duende license:** IdentityServer logs a license warning at startup. No license key is needed for development, testing and evaluation. Production use requires a license (a free Community Edition exists for small companies).
- **Visual Studio launch profiles:** the API's `http` profile (http://localhost:5179) also works with Swagger authorization. Both addresses are configured in IdentityServer.
