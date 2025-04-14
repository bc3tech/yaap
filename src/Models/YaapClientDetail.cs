namespace Yaap.Models;

/// <summary>
/// Represents the details of a YAAP client.
/// </summary>
/// <param name="Name">The name of the client.</param>
/// <param name="Description">A brief description of the client.</param>
/// <param name="CallbackUrl">
/// An optional callback URL associated with the client. 
/// This can be null if no callback URL is required due to the protocol in use.
/// </param>
public record YaapClientDetail(string Name, string Description, Uri? CallbackUrl = null);
