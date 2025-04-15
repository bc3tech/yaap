namespace Models.SignalR;
/// <summary>
/// Contains necessary information for a SignalR client to connect to SignalR Service.
/// </summary>
public sealed record ConnectionInfo(Uri Url, string? AccessToken)
{
    public Task<string?> GetAccessToken() => Task.FromResult(this.AccessToken);
}
