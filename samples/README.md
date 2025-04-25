# Samples

## Setup

Each project of the sample has its own set of secrets, managed by [`dotnet user-secrets`](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=linux#set-a-secret)

Specifically, the following secrets are required for each agent (Teams, Districts, Events, Matches):

- `AzureOpenAIEndpoint`
- `AzureOpenAIKey`
- `TBA_API_KEY`

> Note: If you wish to run the samples as-is, you'll need an API key for The Blue Alliance website. These are free after signup, you can find more information on their [API documentation](https://www.thebluealliance.com/apidocs). \
[Future work](https://github.com/bc3tech/yaap/issues/2) for this repo includes samples that either do not require an API key or are more generic in nature.

For the orchestrator:

- `AzureOpenAIEndpoint`
- `AzureOpenAIKey`

And for the SignalR hub:

- `Azure:SignalR:ConnectionString`

So, for example, to set the Azure OpenAI endpoint for the Teams agent of the SignalR sample, you would run:

```powershell
dotnet user-secrets set "AzureOpenAIEndpoint" "https://<your-openai-endpoint>.openai.azure.com/" --project ./dotnet/signalr/Agents/SignalR/Teams_SignalR
```

from this location of this README.

> Note: Setting a secret for a project in one sample shares that secret with the same project in all other samples.

## Build

Samples can each be built with their `./build/build.ps1` files, you can use the following command to build them:

```powershell
./build/build.ps1 [-NoBuild] [-NoDocker] [-Deploy Docker|Kubernetes]
```

- `NoBuild`: Skips the build step
- `NoDocker`: Skips creating docker images for the agents, orchestrator, etc.
- `Deploy`: Deploys the agent to your local Docker or Kubernetes cluster

## Run

You can run each component of the samples with `./launch.ps1`.

If you deploy the sample to Docker, you need only run the client application via `./launch.ps1 client`.
Otherwise, you can launch the orchestrator and each agent with:

- `./launch.ps1 signalrhub` (if using the SignalR sample)
- `./launch.ps1 orchestrator`
- `./launch.ps1 teamsagent`
- `./launch.ps1 districtsagent`
- `./launch.ps1 matchesagent`
- `./launch.ps1 eventsagent`

and finally

- `./launch.ps1 client` to run the client application.
