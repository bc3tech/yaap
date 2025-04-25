namespace wsAgent.Core;

using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Common;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

using Yaap.Client;

public abstract class Expert : BaseYaapClient
{
    protected readonly IConfiguration _config;
    protected readonly ILogger _log;
    protected readonly Kernel _kernel;
    protected readonly PromptExecutionSettings _promptSettings;
    protected readonly IHttpClientFactory _httpFactory;

    protected Expert(
        IConfiguration appConfig,
        ILoggerFactory loggerFactory,
        IHttpClientFactory httpClientFactory,
        Kernel sk,
        PromptExecutionSettings promptSettings) : base(appConfig, loggerFactory)
    {
        _config = appConfig;
        _kernel = sk;
        _promptSettings = promptSettings;
        _httpFactory = httpClientFactory;

        this.Name = Throws.IfNullOrWhiteSpace(appConfig[Constants.Configuration.Paths.AgentName]);
        this.Description = appConfig[Constants.Configuration.Paths.AgentDescription];
        var securePort = _config["ASPNETCORE_HTTPS_PORTS"];
        if (securePort is not null)
        {
            this.CallbackPort = new Uri(securePort).Port;
            this.Secured = true;
        }
        else
        {
            this.CallbackPort = int.TryParse(_config["ASPNETCORE_HTTP_PORTS"], out var p) ? p : 80;
        }

        _log = Throws.IfNull(loggerFactory).CreateLogger(this.Name);
    }

    public string Name { get; protected init; }
    public string? Description { get; protected init; }
    public int CallbackPort { get; protected init; }
    public bool Secured { get; protected init; }
    public string Hostname { get; protected init; } = Environment.MachineName;

    public Task HandleWebSocketAsync(WebSocket webSocket, CancellationToken cancellationToken) => AIHelpers.HandleWebSocketAsync(webSocket, (socket, raw, prompt, ct) => AIHelpers.ProcessMessageAsync(socket, raw, prompt, _kernel, _promptSettings, ct, _log), cancellationToken);

    public override Task StartingAsync(CancellationToken cancellationToken)
    {
        AgentDefinition.OutputRegisteredSkFunctions(_kernel, new LogTraceTextWriter(_log));
        return Task.CompletedTask;
    }

    protected ArraySegment<byte> Buffer { get; } = ClientWebSocket.CreateClientBuffer(8192, 8192);

    protected ClientWebSocket YaapServerConnection { get; } = new();
    public override async Task SayHelloAsync(CancellationToken cancellationToken)
    {
        try
        {
            await this.YaapServerConnection.ConnectAsync(this.YaapServerEndpoint, cancellationToken).ConfigureAwait(false);
            var message = JsonSerializer.Serialize(new
            {
                action = "Hello",
                detail = this.Detail,
            });

            var messageBytes = Encoding.UTF8.GetBytes(message);

            await this.YaapServerConnection.SendAsync(new ReadOnlyMemory<byte>(messageBytes), WebSocketMessageType.Text, true, cancellationToken).ConfigureAwait(false);

            (var result, var bytes) = await AIHelpers.ReceiveResponseAsync(this.YaapServerConnection, this.Buffer, cancellationToken).ConfigureAwait(false);
            var jsonValue = JsonSerializer.Deserialize<JsonElement>(Encoding.UTF8.GetString([.. bytes], 0, bytes.Length));
            if (!jsonValue.TryGetProperty("message", out JsonElement messageElement) || messageElement.GetString() is not "Agent acknowledged")
            {
                _log.BadAckForHelloFromYAAPServerAckPayload(jsonValue);
            }
        }
        catch (Exception e)
        {
            _log.ErrorTryingToSayHelloToYAAPServerAtYaapServerEndpoint(e, this.YaapServerEndpoint);
        }
    }

    protected override Task StartedInternalAsync(CancellationToken cancellationToken)
    {
        _log.AwaitingQuestion();
        return Task.CompletedTask;
    }

    public override async Task SayGoodbyeAsync(CancellationToken cancellationToken)
    {
        var socketCt = new CancellationToken();
        using var socket = new ClientWebSocket();
        try
        {
            await socket.ConnectAsync(this.YaapServerEndpoint, socketCt).ConfigureAwait(false);
            var message = JsonSerializer.Serialize(new
            {
                action = "Goodbye",
                detail = this.Detail,
            });

            await AIHelpers.SendMessageAsync(socket, message, socketCt).ConfigureAwait(false);
            (var result, var bytes) = await AIHelpers.ReceiveResponseAsync(socket, this.Buffer, socketCt).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _log.ErrorTryingToSayGoodbyeToYAAPServerAtYaapServerEndpoint(e, this.YaapServerEndpoint);
        }
    }
}
