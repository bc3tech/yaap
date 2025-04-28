FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt-get update \
    && apt-get upgrade -y
USER app

ARG AZURE_OPENAI_KEY
ENV AzureOpenAIKey=$AZURE_OPENAI_KEY

WORKDIR /app

COPY . .

ENTRYPOINT ["dotnet", "Orchestrator_WS.dll"]