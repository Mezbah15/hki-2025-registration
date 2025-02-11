# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["hki-2025-registration.csproj", "."]
RUN dotnet restore "./hki-2025-registration.csproj"
COPY . .
RUN dotnet build "./hki-2025-registration.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./hki-2025-registration.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8282
ENV ASPNETCORE_URLS=http://+:8282

ENTRYPOINT ["dotnet", "hki-2025-registration.dll"]
