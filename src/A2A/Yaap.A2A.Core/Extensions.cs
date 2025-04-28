namespace Yaap.A2A.Core;

using global::A2A.Models;

using Yaap.Core.Models;

public static class Extensions
{
    public static YaapClientDetail ToYaapClientDetail(this AgentCard agentCard)
    {
        return new YaapClientDetail(agentCard.Name, agentCard.Description ?? string.Empty, agentCard.Url);
    }
}
