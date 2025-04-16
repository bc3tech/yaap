# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5173

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["NuGet.config", "."]
COPY ["Agents/WebSockets/Teams_WS/Teams_WS.csproj", "Agents/WebSockets/Teams_WS/"]
COPY ["Agents/WebSockets/wsAgent.Core/wsAgent.Core.csproj", "Agents/WebSockets/wsAgent.Core/"]
COPY ["ApiClient/ApiClient.csproj", "ApiClient/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Assistants/Assistants.csproj", "Assistants/"]
COPY ["Models/Models.csproj", "Models/"]
COPY . .
WORKDIR "/src/Agents/WebSockets/Teams_WS"

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Teams_WS.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false 

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Teams_WS.dll"]