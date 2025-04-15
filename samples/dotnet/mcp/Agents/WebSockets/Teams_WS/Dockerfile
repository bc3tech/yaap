FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt-get update \
    && apt-get upgrade -y
USER app

ARG AZURE_OPENAI_KEY
ENV AzureOpenAIKey=$AZURE_OPENAI_KEY

ARG OrchestratorEndpoint
ENV OrchestratorEndpoint=$OrchestratorEndpoint

ARG TBA_API_KEY
ENV TBA_API_KEY=$TBA_API_KEY

WORKDIR /app

COPY . .

ENTRYPOINT ["dotnet", "Teams_SignalR.dll"]