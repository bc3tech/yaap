namespace Yaap.Core.Models;

using A2A.Models;

public static class Extensions
{
    public static YaapClientDetail ToYaapClientDetail(this AgentCard agentCard)
    {
        return new YaapClientDetail(agentCard.Name, agentCard.Description ?? string.Empty, agentCard.Url);
    }
}
