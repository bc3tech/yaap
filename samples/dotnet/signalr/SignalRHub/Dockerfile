FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt-get update \
    && apt-get upgrade -y
USER app

ARG SIGNALR_CONNSTRING

ENV Azure__SignalR__ConnectionString=$SIGNALR_CONNSTRING
ENV ASPNETCORE_HTTP_PORTS=7128

WORKDIR /app

COPY . .

EXPOSE 7128

ENTRYPOINT ["dotnet", "SignalRHub.dll"]