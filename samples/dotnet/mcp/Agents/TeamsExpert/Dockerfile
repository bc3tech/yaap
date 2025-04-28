FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt-get update \
    && apt-get upgrade -y
USER app

ARG AZURE_OPENAI_ENDPOINT
ENV AzureOpenAIEndpoint=$AZURE_OPENAI_ENDPOINT

ARG AZURE_OPENAI_KEY
ENV AzureOpenAIKey=$AZURE_OPENAI_KEY

ARG TBA_API_KEY
ENV TBA_API_KEY=$TBA_API_KEY

ARG OrchestratorEndpoint
ENV Yaap__Server__Endpoint=$OrchestratorEndpoint

WORKDIR /app

COPY . .

ENTRYPOINT ["dotnet", "TeamsExpert.dll"]