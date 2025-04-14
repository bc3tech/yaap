FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt-get update \
    && apt-get upgrade -y
USER app

ARG AZURE_OPENAI_KEY
ENV AzureOpenAIKey=$AZURE_OPENAI_KEY

ARG SignalREndpoint
ENV SignalREndpoint=$SignalREndpoint

ARG TBA_API_KEY
ENV TBA_API_KEY=$TBA_API_KEY

WORKDIR /app

COPY . .

ENTRYPOINT ["dotnet", "Matches_SignalR.dll"]