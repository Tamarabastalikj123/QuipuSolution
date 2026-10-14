# Build context: the solution root (see docker-compose.yml)

# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore first, using only the project files, so this layer is cached until a dependency changes
COPY Directory.Build.props Directory.Packages.props ./
COPY QuipuSolution.IdentityServer/QuipuSolution.IdentityServer.csproj QuipuSolution.IdentityServer/
RUN dotnet restore QuipuSolution.IdentityServer/QuipuSolution.IdentityServer.csproj

COPY QuipuSolution.IdentityServer/ QuipuSolution.IdentityServer/
RUN dotnet publish QuipuSolution.IdentityServer/QuipuSolution.IdentityServer.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# curl is used by the container health check
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .

# Signing keys and Data Protection keys are stored here (mounted as a volume) - owned by the non-root user
RUN mkdir -p /app/keys && chown app:app /app/keys

# Run as the non-root user that ships with the .NET images
USER app

EXPOSE 8080
ENTRYPOINT ["dotnet", "QuipuSolution.IdentityServer.dll"]
