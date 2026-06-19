# syntax=docker/dockerfile:1

# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution + project files first for restore-layer caching.
COPY SolaxHub.sln ./
COPY SolaxHub/SolaxHub.csproj SolaxHub/
COPY SolaxHub.Application/SolaxHub.Application.csproj SolaxHub.Application/
COPY SolaxHub.Domain/SolaxHub.Domain.csproj SolaxHub.Domain/
COPY SolaxHub.Infrastructure/SolaxHub.Infrastructure.csproj SolaxHub.Infrastructure/
COPY SolaxHub.Integration.Tests/SolaxHub.Integration.Tests.csproj SolaxHub.Integration.Tests/
RUN dotnet restore SolaxHub/SolaxHub.csproj

# Copy the rest of the source and publish (framework-dependent).
COPY . .
RUN dotnet publish SolaxHub/SolaxHub.csproj -c Release -o /app/publish \
    && rm -f /app/publish/appsettings.Development.json

# ---- Runtime stage ----
# The aspnet image (not the smaller runtime image) is required: OpenTelemetry.Instrumentation.AspNetCore
# adds a FrameworkReference to Microsoft.AspNetCore.App, which the bare runtime image does not ship.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Run as the non-root user shipped in the base image.
USER $APP_UID

COPY --from=build /app/publish .

# Live overview dashboard (DashboardOptions.Port; override with -e DashboardOptions__Port=...).
EXPOSE 8080

# No ASPNETCORE_ENVIRONMENT set => defaults to Production, so the dev secrets
# file is never loaded. Supply real config via environment variables, e.g.
#   -e ModbusOptions__Host=192.168.1.100 -e KnxOptions__Enabled=false
ENTRYPOINT ["dotnet", "SolaxHub.dll"]
