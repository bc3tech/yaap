namespace Common;

using Microsoft.AspNetCore.SignalR;

internal class UserIdProvider() : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection) => connection.GetHttpContext()?.Request.Query["userid"].ToString();
}
