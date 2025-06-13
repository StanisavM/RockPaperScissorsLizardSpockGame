# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files for restore (including dependencies)
COPY ["src/RockPaperScissorsLizardSpockGame.Api/RockPaperScissorsLizardSpockGame.Api.csproj", "src/RockPaperScissorsLizardSpockGame.Api/"]
COPY ["src/RockPaperScissorsLizardSpockGame.Application/RockPaperScissorsLizardSpockGame.Application.csproj", "src/RockPaperScissorsLizardSpockGame.Application/"]
COPY ["src/RockPaperScissorsLizardSpockGame.Domain/RockPaperScissorsLizardSpockGame.Domain.csproj", "src/RockPaperScissorsLizardSpockGame.Domain/"]
COPY ["src/RockPaperScissorsLizardSpockGame.Infrastructure/RockPaperScissorsLizardSpockGame.Infrastructure.csproj", "src/RockPaperScissorsLizardSpockGame.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/RockPaperScissorsLizardSpockGame.Api/RockPaperScissorsLizardSpockGame.Api.csproj"

# Copy the rest of the files
COPY . .

# Build the project
WORKDIR "/src/src/RockPaperScissorsLizardSpockGame.Api"
RUN dotnet build "RockPaperScissorsLizardSpockGame.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the app
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RockPaperScissorsLizardSpockGame.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RockPaperScissorsLizardSpockGame.Api.dll"]
