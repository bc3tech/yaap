FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt-get update \
    && apt-get upgrade -y
USER app

WORKDIR /app

COPY . .

ENTRYPOINT ["dotnet", "AgentsMcpServer.dll"]