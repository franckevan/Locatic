# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY Locatic.sln .
COPY Locatic.Domain/Locatic.Domain.csproj Locatic.Domain/
COPY Locatic.Application/Locatic.Application.csproj Locatic.Application/
COPY Locatic.Infrastructure/Locatic.Infrastructure.csproj Locatic.Infrastructure/
COPY Locatic.Web/Locatic.Web.csproj Locatic.Web/
COPY Locatic.Tests/Locatic.Tests.csproj Locatic.Tests/
RUN dotnet restore Locatic.Web/Locatic.Web.csproj
COPY . .
RUN dotnet publish Locatic.Web/Locatic.Web.csproj -c Release -o /app/publish --no-restore

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
RUN addgroup --system --gid 1000 appgroup \
    && adduser --system --uid 1000 --ingroup appgroup --shell /bin/false appuser \
    && mkdir -p /data \
    && chown -R appuser:appgroup /app /data
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080 \
    ConnectionStrings__DefaultConnection="Data Source=/data/locatic.db"
VOLUME ["/data"]
USER appuser
EXPOSE 8080
ENTRYPOINT ["dotnet", "Locatic.Web.dll"]
