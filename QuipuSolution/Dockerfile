# Build context: the solution root (see docker-compose.yml)

# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore first, using only the project files, so this layer is cached until a dependency changes
COPY Directory.Build.props Directory.Packages.props ./
COPY QuipuSolution.Domain/QuipuSolution.Domain.csproj QuipuSolution.Domain/
COPY QuipuSolution.Application/QuipuSolution.Application.csproj QuipuSolution.Application/
COPY QuipuSolution.Infrastructure/QuipuSolution.Infrastructure.csproj QuipuSolution.Infrastructure/
COPY QuipuSolution/QuipuSolution.csproj QuipuSolution/
RUN dotnet restore QuipuSolution/QuipuSolution.csproj

COPY QuipuSolution.Domain/ QuipuSolution.Domain/
COPY QuipuSolution.Application/ QuipuSolution.Application/
COPY QuipuSolution.Infrastructure/ QuipuSolution.Infrastructure/
COPY QuipuSolution/ QuipuSolution/
RUN dotnet publish QuipuSolution/QuipuSolution.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# curl is used by the container health check
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .

# Run as the non-root user that ships with the .NET images
USER app

EXPOSE 8080
ENTRYPOINT ["dotnet", "QuipuSolution.dll"]
