namespace SignalRASPHub;

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;

using Common;
using Common.Extensions;

using Microsoft.AspNetCore.SignalR;

internal class TbaSignalRHub(ILoggerFactory loggerFactory) : Hub
{
    private static readonly ConcurrentDictionary<string, string> UserConnections = [];
    private readonly ILogger _log = loggerFactory.CreateLogger<TbaSignalRHub>();

    public override async Task OnConnectedAsync()
    {
        using IDisposable scope = _log.CreateMethodScope();

        var username = this.Context.UserIdentifier;
        if (string.IsNullOrWhiteSpace(username))
        {
            _log.LogWarning("UserID empty!");
        }
        else
        {
            UserConnections.AddOrUpdate(username, this.Context.ConnectionId, (_, _) => this.Context.ConnectionId);
            _log.LogDebug("Stored connection {connectionId} for user {userId}", this.Context.ConnectionId, username);
            if (username.EndsWith("expert", StringComparison.InvariantCultureIgnoreCase) is true)
            {
                _log.LogInformation("Expert {expertName} connected.", username);
                await this.Clients.User(Constants.SignalR.Users.EndUser).SendAsync(Constants.SignalR.Functions.ExpertJoined, username);

                _log.LogTrace("All clients notified.");
            }
            else
            {
                _log.LogInformation("{user} connected.", username);
            }
        }
    }

    [HubMethodName("Connect")]
    public async Task<string> ConnectAsync()
    {
        await OnConnectedAsync();

        return UserConnections.ContainsKey(this.Context.UserIdentifier!).ToString();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        using IDisposable scope = _log.CreateMethodScope();

        if (this.Context.UserIdentifier?.EndsWith("Expert", StringComparison.InvariantCultureIgnoreCase) is true)
        {
            await this.Clients.Users([Constants.SignalR.Users.Orchestrator, Constants.SignalR.Users.EndUser])
                .SendAsync(Constants.SignalR.Functions.ExpertLeft, this.Context.UserIdentifier);
        }
        else if (this.Context.UserIdentifier is "Orchestrator")
        {
            _log.LogDebug("Orchestrator disconnected. Requesting reintroductions from experts...");

            await this.Clients.AllExcept(Constants.SignalR.Users.EndUser).SendAsync(Constants.SignalR.Functions.Reintroduce);
        }
    }

    [HubMethodName(Constants.SignalR.Functions.PostStatus)]
    public Task PostStatusAsync(string message) => this.Clients.User(Constants.SignalR.Users.EndUser).SendAsync(Constants.SignalR.Functions.PostStatus, this.Context.UserIdentifier, message);

    [HubMethodName(Constants.SignalR.Functions.GetAnswer)]
    public async Task<string> GetAnswerAsync(string targetUser, string question)
    {
        using IDisposable scope = _log.CreateMethodScope();

        var conn = await WaitForUserToConnectAsync(targetUser);

        _log.LogTrace("Sending request to {user}", targetUser);
        var result = await this.Clients.Client(conn).InvokeAsync<string>(Constants.SignalR.Functions.GetAnswer, question, default);
        _log.LogTrace("Response received.");

        return result;
    }

    [HubMethodName(Constants.SignalR.Functions.GetStreamedAnswer)]
    public async Task<ChannelReader<string>> GetStreamedAnswerAsync(string targetUser, string question, CancellationToken cancellationToken)
    {
        using IDisposable scope = _log.CreateMethodScope();
        cancellationToken.Register(() => _log.LogWarning("CANCELLED."));

        var id = Guid.NewGuid().ToString();
        var tcs = Channel.CreateUnbounded<string>(new() { SingleWriter = true, SingleReader = true, AllowSynchronousContinuations = true });
        if (!_completions.TryAdd(id, tcs))
        {
            _log.LogWarning("Error adding completion to dictionary!");
        }

        var conn = await WaitForUserToConnectAsync(targetUser, cancellationToken: cancellationToken);

        _log.LogTrace("Sending request to {user}", targetUser);

        await this.Clients.User(targetUser).SendAsync(Constants.SignalR.Functions.SendStreamedAnswerBack, id, question, cancellationToken: cancellationToken);
        return tcs.Reader;
    }

    [HubMethodName(Constants.SignalR.Functions.SendStreamedAnswerBack)]
    public async Task SendStreamedAnswerBackAsync(string completionId, IAsyncEnumerable<string> answerStream)
    {
        using IDisposable scope = _log.CreateMethodScope();

        _log.BeginScope("User[{callingUserId}]", this.Context.UserIdentifier ?? "unknown user");

        if (!_completions.TryRemove(completionId, out Channel<string>? completion))
        {
            _log.LogWarning("Unable to get completion {completionId} from dictionary!", completionId);
        }
        else
        {
            _log.LogTrace("Writing to stream");
            await foreach (var s in answerStream)
            {
                if (!completion.Writer.TryWrite(s))
                {
                    _log.LogWarning("Error writing token!");
                }
            }

            completion.Writer.Complete();
        }

        _log.LogTrace("Completed.");
    }

    [HubMethodName(Constants.SignalR.Functions.Introduce)]
    public async Task IntroduceAsync(string name, string description)
    {
        using IDisposable scope = _log.CreateMethodScope();

        await WaitForUserToConnectAsync(Constants.SignalR.Users.Orchestrator);

        _log.LogTrace("Introduction received: {expertName}", name);

        await this.Clients.Client(UserConnections[Constants.SignalR.Users.Orchestrator]).SendAsync(Constants.SignalR.Functions.Introduce, name, description);
        _log.LogDebug("Introduction for {expertName} sent to Orchestrator", name);
    }

    private static readonly ConcurrentDictionary<string, Channel<string>> _completions = [];

    [return: NotNull]
    private async Task<string> WaitForUserToConnectAsync([NotNull] string user, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        _log.LogTrace("Waiting for {user} to connect...", Throws.IfNullOrWhiteSpace(user));

        var waitTask = Task.Run(async () =>
        {
            string? userConn;
            while (!UserConnections.TryGetValue(user, out userConn) || string.IsNullOrWhiteSpace(userConn))
            {
                await this.Clients.User(user).SendAsync("Connect");
                await Task.Delay(100, cancellationToken);
            }

            return userConn;

        }, cancellationToken);

        Task completedTask = await Task.WhenAny(waitTask, Task.Run(async () => await Task.Delay(timeout ?? TimeSpan.FromSeconds(30), cancellationToken), cancellationToken));

        cancellationToken.ThrowIfCancellationRequested();

        if (completedTask != waitTask)
        {
            throw new TimeoutException($@"Timed out waiting for user '{user}' to connect.");
        }

        _log.LogDebug("{user} connected.", user);

        return await waitTask;
    }
}
