namespace Common;

using Microsoft.Extensions.Configuration;

public static class Constants
{
    public static class Configuration
    {
        public static class SectionNames
        {
            public const string AgentDefinition = nameof(AgentDefinition);
        }

        public static class VariableNames
        {
            public const string TBA_API_KEY = nameof(TBA_API_KEY);

            public const string Name = nameof(Name);

            public const string Description = nameof(Description);

            public const string McpServerEndpoint = nameof(McpServerEndpoint);

            public const string AzureOpenAIEndpoint = nameof(AzureOpenAIEndpoint);

            public const string AzureOpenAIModelDeployment = nameof(AzureOpenAIModelDeployment);

            public const string SystemPrompt = nameof(SystemPrompt);
        }

        public static class Paths
        {
            public static readonly string AgentName = ConfigurationPath.Combine(SectionNames.AgentDefinition, VariableNames.Name);

            public static readonly string AgentDescription = ConfigurationPath.Combine(SectionNames.AgentDefinition, VariableNames.Description);
        }
    }

    public static class Token
    {
        public const string EndToken = @"!END!";
    }

    public static class SignalR
    {
        public const string HubName = "TBASignalRHub";

        public static class Users
        {
            public const string EndUser = nameof(EndUser);
            public const string Orchestrator = nameof(Orchestrator);
        }

        public static class Functions
        {
            public const string GetAnswer = nameof(GetAnswer);

            public const string GetStreamedAnswer = nameof(GetStreamedAnswer);

            public const string SendStreamedAnswerBack = nameof(SendStreamedAnswerBack);

            public const string Introduce = nameof(Introduce);

            public const string Reintroduce = nameof(Reintroduce);

            public const string ExpertJoined = nameof(ExpertJoined);

            public const string ExpertLeft = nameof(ExpertLeft);

            public const string PostStatus = nameof(PostStatus);
        }
    }
}
