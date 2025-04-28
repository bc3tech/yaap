namespace Yaap.Core.Models;

using A2A.Models;

/// <summary>
/// Represents the details of a YAAP client.
/// </summary>
public record YaapClientDetail : AgentCard
{
    /// <summary>
    /// Gets an optional callback URL associated with the client.
    /// This can be null if no callback URL is required due to the protocol in use.
    /// </summary>
    public Uri? CallbackUrl => this.Url;

    /// <summary>
    /// Initializes a new instance of the <see cref="YaapClientDetail"/> class.
    /// </summary>
    /// <param name="name">The name of the client.</param>
    /// <param name="description">A brief description of the client.</param>
    /// <param name="callbackUrl">
    /// An optional callback URL associated with the client.
    /// This can be null if no callback URL is required due to the protocol in use.
    /// </param>
    public YaapClientDetail(string name, string description, Uri? callbackUrl = null)
    {
        this.Name = name;
        this.Description = description;
        if (callbackUrl is not null)
        {
            this.Url = callbackUrl;
        }
    }
}
