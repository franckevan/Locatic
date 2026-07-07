# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Locatic.Domain/Locatic.Domain.csproj Locatic.Domain/
COPY Locatic.Application/Locatic.Application.csproj Locatic.Application/
COPY Locatic.Infrastructure/Locatic.Infrastructure.csproj Locatic.Infrastructure/
COPY Locatic.Web/Locatic.Web.csproj Locatic.Web/
RUN dotnet restore Locatic.Web/Locatic.Web.csproj

COPY Locatic.Domain/ Locatic.Domain/
COPY Locatic.Application/ Locatic.Application/
COPY Locatic.Infrastructure/ Locatic.Infrastructure/
COPY Locatic.Web/ Locatic.Web/
RUN dotnet publish Locatic.Web/Locatic.Web.csproj -c Release -o /app/publish --no-restore

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# curl is only used by the container HEALTHCHECK below; Kubernetes probes
# hit the same /health endpoint directly over HTTP and don't need it.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    ConnectionStrings__DefaultConnection="Data Source=/data/locatic.db"

# /data is where the SQLite file lives; a volume (Docker or Kubernetes PVC)
# is expected to be mounted there so the database survives container restarts.
RUN mkdir -p /data && chown -R app:app /data

COPY --from=build --chown=app:app /app/publish .

USER app
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -fs http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Locatic.Web.dll"]
