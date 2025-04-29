namespace Yaap.Core.Models;

/// <summary>
/// Represents the details of a YAAP client.
/// </summary>
/// <param name="Name">Specifies the client name.</param>
/// <param name="Description">Provides a brief overview of the client.</param>
/// <param name="CallbackUrl">Defines the optional callback URL for client operations.</param>
public record YaapClientDetail(string Name, string Description, Uri? CallbackUrl = null);
