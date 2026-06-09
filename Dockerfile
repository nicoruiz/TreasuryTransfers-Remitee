# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first for layer caching
COPY TreasuryTransfers.sln ./
COPY src/TreasuryTransfers.Domain/TreasuryTransfers.Domain.csproj src/TreasuryTransfers.Domain/
COPY src/TreasuryTransfers.Application/TreasuryTransfers.Application.csproj src/TreasuryTransfers.Application/
COPY src/TreasuryTransfers.Infrastructure/TreasuryTransfers.Infrastructure.csproj src/TreasuryTransfers.Infrastructure/
COPY src/TreasuryTransfers.Api/TreasuryTransfers.Api.csproj src/TreasuryTransfers.Api/
COPY tests/TreasuryTransfers.Tests/TreasuryTransfers.Tests.csproj tests/TreasuryTransfers.Tests/

RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish src/TreasuryTransfers.Api/TreasuryTransfers.Api.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser
USER appuser

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_EnableDiagnostics=0

ENTRYPOINT ["dotnet", "TreasuryTransfers.Api.dll"]
