namespace Yaap.A2A.Core.Models;
/// <summary>
/// Represents the state of a task.
/// </summary>
/// <param name="Value">The value of the task state.</param>
public record TaskState(string Value);

/// <summary>
/// Represents a part of text.
/// </summary>
/// <param name="Type">The type of the part.</param>
/// <param name="Text">The text content.</param>
/// <param name="Metadata">The metadata associated with the text part.</param>
public record TextPart(string Type, string Text, Dictionary<string, object>? Metadata);

/// <summary>
/// Represents the content of a file.
/// </summary>
/// <param name="Name">The name of the file.</param>
/// <param name="MimeType">The MIME type of the file.</param>
/// <param name="Bytes">The byte content of the file.</param>
/// <param name="Uri">The URI of the file.</param>
public record FileContent(string? Name, string? MimeType, string? Bytes, string? Uri);

/// <summary>
/// Represents a part of a file.
/// </summary>
/// <param name="Type">The type of the part.</param>
/// <param name="File">The file content.</param>
/// <param name="Metadata">The metadata associated with the file part.</param>
public record FilePart(string Type, FileContent File, Dictionary<string, object>? Metadata);

/// <summary>
/// Represents a part of data.
/// </summary>
/// <param name="Type">The type of the part.</param>
/// <param name="Data">The data content.</param>
/// <param name="Metadata">The metadata associated with the data part.</param>
public record DataPart(string Type, Dictionary<string, object> Data, Dictionary<string, object>? Metadata);

/// <summary>
/// Represents a message.
/// </summary>
/// <param name="Role">The role of the message sender.</param>
/// <param name="Parts">The parts of the message.</param>
/// <param name="Metadata">The metadata associated with the message.</param>
public record Message(string Role, List<object> Parts, Dictionary<string, object>? Metadata);

/// <summary>
/// Represents the status of a task.
/// </summary>
/// <param name="State">The state of the task.</param>
/// <param name="Message">The message associated with the task status.</param>
/// <param name="Timestamp">The timestamp of the task status.</param>
public record TaskStatus(TaskState State, Message? Message, DateTime Timestamp);

/// <summary>
/// Represents an artifact.
/// </summary>
/// <param name="Name">The name of the artifact.</param>
/// <param name="Description">The description of the artifact.</param>
/// <param name="Parts">The parts of the artifact.</param>
/// <param name="Metadata">The metadata associated with the artifact.</param>
/// <param name="Index">The index of the artifact.</param>
/// <param name="Append">Indicates whether the artifact should be appended.</param>
/// <param name="LastChunk">Indicates whether the artifact is the last chunk.</param>
public record Artifact(string? Name, string? Description, List<object> Parts, Dictionary<string, object>? Metadata, int Index, bool? Append, bool? LastChunk);

/// <summary>
/// Represents a task.
/// </summary>
/// <param name="Id">The ID of the task.</param>
/// <param name="SessionId">The session ID of the task.</param>
/// <param name="Status">The status of the task.</param>
/// <param name="Artifacts">The artifacts associated with the task.</param>
/// <param name="History">The history of the task.</param>
/// <param name="Metadata">The metadata associated with the task.</param>
public record AgentTask(string Id, string? SessionId, TaskStatus Status, List<Artifact>? Artifacts, List<Message>? History, Dictionary<string, object>? Metadata);

/// <summary>
/// Represents a task status update event.
/// </summary>
/// <param name="Id">The ID of the event.</param>
/// <param name="Status">The status of the task.</param>
/// <param name="Final">Indicates whether the event is final.</param>
/// <param name="Metadata">The metadata associated with the event.</param>
public record TaskStatusUpdateEvent(string Id, TaskStatus Status, bool Final, Dictionary<string, object>? Metadata);

/// <summary>
/// Represents a task artifact update event.
/// </summary>
/// <param name="Id">The ID of the event.</param>
/// <param name="Artifact">The artifact associated with the event.</param>
/// <param name="Metadata">The metadata associated with the event.</param>
public record TaskArtifactUpdateEvent(string Id, Artifact Artifact, Dictionary<string, object>? Metadata);

/// <summary>
/// Represents authentication information.
/// </summary>
/// <param name="Schemes">The authentication schemes.</param>
/// <param name="Credentials">The credentials for authentication.</param>
public record AuthenticationInfo(List<string> Schemes, string? Credentials);

/// <summary>
/// Represents push notification configuration.
/// </summary>
/// <param name="Url">The URL for push notifications.</param>
/// <param name="Token">The token for push notifications.</param>
/// <param name="Authentication">The authentication information for push notifications.</param>
public record PushNotificationConfig(string Url, string? Token, AuthenticationInfo? Authentication);

/// <summary>
/// Represents task ID parameters.
/// </summary>
/// <param name="Id">The ID of the task.</param>
/// <param name="Metadata">The metadata associated with the task ID.</param>
public record TaskIdParams(string Id, Dictionary<string, object>? Metadata) : IReadOnlyDictionary<string, object?>
{
    private readonly IReadOnlyDictionary<string, object?> _dict = new Dictionary<string, object?>()
        {
            { nameof(Id), Id },
            { nameof(Metadata), Metadata }
        }.AsReadOnly();

    #region IDictionary impl
    /// <inheritdoc />
    object? IReadOnlyDictionary<string, object?>.this[string key] => _dict[key];

    /// <inheritdoc />
    IEnumerable<string> IReadOnlyDictionary<string, object?>.Keys => _dict.Keys;

    /// <inheritdoc />
    IEnumerable<object?> IReadOnlyDictionary<string, object?>.Values => _dict.Values;

    /// <inheritdoc />
    int IReadOnlyCollection<KeyValuePair<string, object?>>.Count => _dict.Count;

    /// <inheritdoc />
    bool IReadOnlyDictionary<string, object?>.ContainsKey(string key) => _dict.ContainsKey(key);

    /// <inheritdoc />
    bool IReadOnlyDictionary<string, object?>.TryGetValue(string key, out object? value) => _dict.TryGetValue(key, out value);

    /// <inheritdoc />
    IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => _dict.GetEnumerator();

    /// <inheritdoc />
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _dict.GetEnumerator();
    #endregion
}

/// <summary>
/// Represents task query parameters.
/// </summary>
/// <param name="Id">The ID of the task.</param>
/// <param name="Metadata">The metadata associated with the task ID.</param>
/// <param name="HistoryLength">The length of the task history.</param>
public record TaskQueryParams(string Id, Dictionary<string, object>? Metadata, int? HistoryLength) : IReadOnlyDictionary<string, object?>
{
    private readonly IReadOnlyDictionary<string, object?> _dict = new Dictionary<string, object?>()
        {
            { nameof(Id), Id },
            { nameof(Metadata), Metadata },
            { nameof(HistoryLength), HistoryLength }
        }.AsReadOnly();

    #region IDictionary impl
    /// <inheritdoc />
    object? IReadOnlyDictionary<string, object?>.this[string key] => _dict[key];

    /// <inheritdoc />
    IEnumerable<string> IReadOnlyDictionary<string, object?>.Keys => _dict.Keys;

    /// <inheritdoc />
    IEnumerable<object?> IReadOnlyDictionary<string, object?>.Values => _dict.Values;

    /// <inheritdoc />
    int IReadOnlyCollection<KeyValuePair<string, object?>>.Count => _dict.Count;

    /// <inheritdoc />
    bool IReadOnlyDictionary<string, object?>.ContainsKey(string key) => _dict.ContainsKey(key);

    /// <inheritdoc />
    bool IReadOnlyDictionary<string, object?>.TryGetValue(string key, out object? value) => _dict.TryGetValue(key, out value);

    /// <inheritdoc />
    IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => _dict.GetEnumerator();

    /// <inheritdoc />
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _dict.GetEnumerator();
    #endregion
}

/// <summary>
/// Represents task send parameters.
/// </summary>
/// <param name="Id">The ID of the task.</param>
/// <param name="SessionId">The session ID of the task.</param>
/// <param name="Message">The message associated with the task.</param>
/// <param name="AcceptedOutputModes">The accepted output modes for the task.</param>
/// <param name="PushNotification">The push notification configuration for the task.</param>
/// <param name="HistoryLength">The length of the task history.</param>
/// <param name="Metadata">The metadata associated with the task.</param>
public record TaskSendParams(string Id, string SessionId, Message Message, List<string>? AcceptedOutputModes, PushNotificationConfig? PushNotification, int? HistoryLength, Dictionary<string, object>? Metadata) : IReadOnlyDictionary<string, object?>
{
    private readonly IReadOnlyDictionary<string, object?> _dict = new Dictionary<string, object?>()
        {
            { nameof(Id), Id },
            { nameof(SessionId), SessionId },
            { nameof(Message), Message },
            { nameof(AcceptedOutputModes), AcceptedOutputModes },
            { nameof(PushNotification), PushNotification },
            { nameof(HistoryLength), HistoryLength },
            { nameof(Metadata), Metadata }
        }.AsReadOnly();

    #region IDictionary impl
    /// <inheritdoc />
    object? IReadOnlyDictionary<string, object?>.this[string key] => _dict[key];

    /// <inheritdoc />
    IEnumerable<string> IReadOnlyDictionary<string, object?>.Keys => _dict.Keys;

    /// <inheritdoc />
    IEnumerable<object?> IReadOnlyDictionary<string, object?>.Values => _dict.Values;

    /// <inheritdoc />
    int IReadOnlyCollection<KeyValuePair<string, object?>>.Count => _dict.Count;

    /// <inheritdoc />
    bool IReadOnlyDictionary<string, object?>.ContainsKey(string key) => _dict.ContainsKey(key);

    /// <inheritdoc />
    bool IReadOnlyDictionary<string, object?>.TryGetValue(string key, out object? value) => _dict.TryGetValue(key, out value);

    /// <inheritdoc />
    IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => _dict.GetEnumerator();

    /// <inheritdoc />
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _dict.GetEnumerator();
    #endregion
}

/// <summary>
/// Represents task push notification configuration.
/// </summary>
/// <param name="Id">The ID of the task.</param>
/// <param name="PushNotificationConfig">The push notification configuration for the task.</param>
public record TaskPushNotificationConfig(string Id, PushNotificationConfig PushNotificationConfig) : IReadOnlyDictionary<string, object?>
{
    private readonly IReadOnlyDictionary<string, object?> _dict = new Dictionary<string, object?>()
        {
            { nameof(Id), Id },
            { nameof(PushNotificationConfig), PushNotificationConfig }
        }.AsReadOnly();

    #region IDictionary impl
    /// <inheritdoc />
    object? IReadOnlyDictionary<string, object?>.this[string key] => _dict[key];

    /// <inheritdoc />
    IEnumerable<string> IReadOnlyDictionary<string, object?>.Keys => _dict.Keys;

    /// <inheritdoc />
    IEnumerable<object?> IReadOnlyDictionary<string, object?>.Values => _dict.Values;

    /// <inheritdoc />
    int IReadOnlyCollection<KeyValuePair<string, object?>>.Count => _dict.Count;

    /// <inheritdoc />
    bool IReadOnlyDictionary<string, object?>.ContainsKey(string key) => _dict.ContainsKey(key);

    /// <inheritdoc />
    bool IReadOnlyDictionary<string, object?>.TryGetValue(string key, out object? value) => _dict.TryGetValue(key, out value);

    /// <inheritdoc />
    IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => _dict.GetEnumerator();

    /// <inheritdoc />
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _dict.GetEnumerator();
    #endregion
}

/// <summary>
/// Represents a JSON-RPC message.
/// </summary>
/// <param name="Id">The ID of the message.</param>
public record JSONRPCMessage(object Id)
{
    /// <summary>The JSON-RPC version.</summary>
    public const string Jsonrpc = "2.0";
}

/// <summary>
/// Represents a JSON-RPC request.
/// </summary>
/// <param name="Id">
/// The unique identifier for the JSON-RPC request, which can be an integer, string, or null.
/// </param>
public abstract record JSONRPCRequest(object Id)
{
    /// <summary>
    /// The JSON-RPC version, which is always "2.0".
    /// </summary>
    public const string Jsonrpc = "2.0";

    /// <summary>
    /// Gets the method name for the JSON-RPC request.
    /// </summary>
    public abstract string Method { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JSONRPCRequest"/> class with a generated ID.
    /// </summary>
    public JSONRPCRequest() : this(Guid.NewGuid()) { }
}

/// <summary>
/// Represents a JSON-RPC request with typed parameters.
/// </summary>
/// <typeparam name="TParams">
/// The type of the parameters for the JSON-RPC request. Must implement <see cref="IReadOnlyDictionary{TKey, TValue}"/>.
/// </typeparam>
public abstract record JSONRPCRequest<TParams>(object Id, TParams? Params) : JSONRPCRequest(Id) where TParams : IReadOnlyDictionary<string, object?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JSONRPCRequest{TParams}"/> class with a generated ID.
    /// </summary>
    /// <param name="Params">The parameters for the JSON-RPC request.</param>
    public JSONRPCRequest(TParams? Params) : this(Guid.NewGuid().ToString(), Params) { }
}

/// <summary>
/// Represents a JSON-RPC error.
/// </summary>
/// <param name="Code">The error code.</param>
/// <param name="Message">The error message.</param>
/// <param name="Data">The data associated with the error.</param>
public record JSONRPCError(int Code, string Message, object? Data);

/// <summary>
/// Represents a standard JSON-RPC response.
/// </summary>
/// <param name="Id">
/// The unique identifier for the response, which can be an integer, string, or null.
/// </param>
/// <param name="Result">
/// The result of the JSON-RPC call, which may be null.
/// </param>
/// <param name="Error">
/// The error details, which can be a <see cref="JSONRPCError"/> object or null.
/// </param>
public record JSONRPCResponse(object Id, object? Result = default, JSONRPCError? Error = null) : JSONRPCResponse<object>(Id, Result, Error);

/// <summary>
/// Represents a standard JSON-RPC response.
/// </summary>
/// <typeparam name="TResult">The type of the result returned in the response.</typeparam>
/// <param name="Id">
/// The unique identifier for the response, which can be an integer, string, or null.
/// </param>
/// <param name="Result">
/// The result of the JSON-RPC call, which is of type <typeparamref name="TResult"/> or null.
/// </param>
/// <param name="Error">
/// The error details, which can be a <see cref="JSONRPCError"/> object or null.
/// </param>
public record JSONRPCResponse<TResult>(object Id, TResult? Result = default, JSONRPCError? Error = null)
{
    /// <summary>
    /// The JSON-RPC version, which is always "2.0".
    /// </summary>
    public const string Jsonrpc = "2.0";

    /// <summary>
    /// Initializes a new instance of the <see cref="JSONRPCResponse{TResult}"/> class with a generated ID.
    /// </summary>
    /// <param name="Result">
    /// The result of the JSON-RPC call, which can be an object of type <typeparamref name="TResult"/> or null.
    /// </param>
    /// <param name="Error">
    /// The error details, which can be a <see cref="JSONRPCError"/> object or null.
    /// </param>
    public JSONRPCResponse(TResult? Result = default, JSONRPCError? Error = null) : this(Guid.NewGuid().ToString(), Result, Error) { }
}

/// <summary>
/// Represents a request to send a task.
/// </summary>
public record SendTaskRequest(object Id, TaskSendParams Params) : JSONRPCRequest<TaskSendParams>(Id, Params)
{
    /// <inheritdoc />
    public override string Method => "tasks/send";

    /// <summary>
    /// Initializes a new instance of the <see cref="SendTaskRequest"/> class with a generated ID.
    /// </summary>
    /// <param name="Params">The parameters for the JSON-RPC request.</param>
    public SendTaskRequest(TaskSendParams Params) : this(Guid.NewGuid(), Params) { }
}

/// <summary>
/// Represents a response for sending a task.
/// </summary>
public record SendTaskResponse(AgentTask? Result = null) : JSONRPCResponse<AgentTask>(Guid.NewGuid(), Result, null);

/// <summary>
/// Represents a request to send a task with streaming updates.
/// </summary>
public record SendTaskStreamingRequest(object Id, TaskSendParams Params) : JSONRPCRequest<TaskSendParams>(Id, Params)
{
    /// <inheritdoc />
    public override string Method => "tasks/sendSubscribe";

    /// <summary>
    /// Initializes a new instance of the <see cref="SendTaskStreamingRequest"/> class with a generated ID.
    /// </summary>
    /// <param name="Params">The parameters for the JSON-RPC request.</param>
    public SendTaskStreamingRequest(TaskSendParams Params) : this(Guid.NewGuid(), Params) { }
}

/// <summary>
/// Represents a response for sending a task with streaming updates.
/// </summary>
public record SendTaskStreamingResponse(object? Result = null) : JSONRPCResponse<object>(Guid.NewGuid(), Result, null);

/// <summary>
/// Represents a request to get a task.
/// </summary>
public record GetTaskRequest(object Id, TaskQueryParams Params) : JSONRPCRequest<TaskQueryParams>(Id, Params)
{
    /// <inheritdoc />
    public override string Method => "tasks/get";

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTaskRequest"/> class with a generated ID.
    /// </summary>
    /// <param name="Params">The parameters for the JSON-RPC request.</param>
    public GetTaskRequest(TaskQueryParams Params) : this(Guid.NewGuid(), Params) { }
}

/// <summary>
/// Represents a response for getting a task.
/// </summary>
/// <param name="Id">
/// The unique identifier for the response, which can be an integer, string, or null.
/// </param>
/// <param name="Result">
/// The result of the JSON-RPC call, which is an <see cref="AgentTask"/> object or null.
/// </param>
/// <param name="Error">
/// The error details, which can be a <see cref="JSONRPCError"/> object or null.
/// </param>
public record GetTaskResponse(object Id, AgentTask? Result = null, JSONRPCError? Error = null) : JSONRPCResponse<AgentTask>(Id, Result, Error)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTaskResponse"/> class with a result.
    /// </summary>
    /// <param name="Result">
    /// The result of the JSON-RPC call, which can be an <see cref="AgentTask"/> object or null.
    /// </param>
    public GetTaskResponse(AgentTask? Result = null) : this(Guid.NewGuid(), Result, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTaskResponse"/> class with an error.
    /// </summary>
    /// <param name="Error">
    /// The error details, which can be a <see cref="JSONRPCError"/> object or null.
    /// </param>
    public GetTaskResponse(JSONRPCError? Error) : this(Guid.NewGuid(), null, Error) { }
}

/// <summary>
/// Represents a request to cancel a task.
/// </summary>
public record CancelTaskRequest(object Id, TaskIdParams Params) : JSONRPCRequest<TaskIdParams>(Id, Params)
{
    /// <inheritdoc />
    public override string Method => "tasks/cancel";

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelTaskRequest"/> class with a generated ID.
    /// </summary>
    /// <param name="Params">The parameters for the JSON-RPC request.</param>
    public CancelTaskRequest(TaskIdParams Params) : this(Guid.NewGuid(), Params) { }
}

/// <summary>
/// Represents a response for canceling a task.
/// </summary>
/// <param name="Id">
/// The unique identifier for the response, which can be an integer, string, or null.
/// </param>
/// <param name="Result">
/// The result of the JSON-RPC call, which is an <see cref="AgentTask"/> object or null.
/// </param>
/// <param name="Error">
/// The error details, which can be a <see cref="JSONRPCError"/> object or null.
/// </param>
public record CancelTaskResponse(object Id, AgentTask? Result = null, JSONRPCError? Error = null) : JSONRPCResponse<AgentTask>(Id, Result, Error)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CancelTaskResponse"/> class with a result.
    /// </summary>
    /// <param name="Result">
    /// The result of the JSON-RPC call, which can be an <see cref="AgentTask"/> object or null.
    /// </param>
    public CancelTaskResponse(AgentTask? Result = null) : this(Guid.NewGuid(), Result, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelTaskResponse"/> class with an error.
    /// </summary>
    /// <param name="Error">
    /// The error details, which can be a <see cref="JSONRPCError"/> object or null.
    /// </param>
    public CancelTaskResponse(JSONRPCError? Error) : this(Guid.NewGuid(), null, Error) { }
}

/// <summary>
/// Represents a request to set task push notification configuration.
/// </summary>
public record SetTaskPushNotificationRequest(object Id, TaskPushNotificationConfig Params) : JSONRPCRequest<TaskPushNotificationConfig>(Id, Params)
{
    /// <inheritdoc />
    public override string Method => "tasks/pushNotification/set";

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTaskPushNotificationRequest"/> class with a generated ID.
    /// </summary>
    /// <param name="Params">The parameters for the JSON-RPC request.</param>
    public SetTaskPushNotificationRequest(TaskPushNotificationConfig Params) : this(Guid.NewGuid(), Params) { }
}

/// <summary>
/// Represents a response for setting task push notification configuration.
/// </summary>
public record SetTaskPushNotificationResponse(object Id, TaskPushNotificationConfig? Result = null, JSONRPCError? Error = null) : JSONRPCResponse<TaskPushNotificationConfig>(Id, Result, Error)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SetTaskPushNotificationResponse"/> class with a result.
    /// </summary>
    /// <param name="Result">
    /// The result of the JSON-RPC call, which can be an <see cref="TaskPushNotificationConfig"/> object or null.
    /// </param>
    public SetTaskPushNotificationResponse(TaskPushNotificationConfig? Result = null) : this(Guid.NewGuid(), Result, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTaskPushNotificationResponse"/> class with an error.
    /// </summary>
    /// <param name="Error">
    /// The error details, which can be a <see cref="JSONRPCError"/> object or null.
    /// </param>
    public SetTaskPushNotificationResponse(JSONRPCError? Error) : this(Guid.NewGuid(), null, Error) { }
}

/// <summary>
/// Represents a request to get task push notification configuration.
/// </summary>
public record GetTaskPushNotificationRequest(object Id, TaskIdParams Params) : JSONRPCRequest<TaskIdParams>(Id, Params)
{
    /// <inheritdoc />
    public override string Method => "tasks/pushNotification/get";

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTaskPushNotificationRequest"/> class with a generated ID.
    /// </summary>
    /// <param name="Params">The parameters for the JSON-RPC request.</param>
    public GetTaskPushNotificationRequest(TaskIdParams Params) : this(Guid.NewGuid(), Params) { }
}

/// <summary>
/// Represents a response for getting task push notification configuration.
/// </summary>
public record GetTaskPushNotificationResponse(object Id, TaskPushNotificationConfig? Result = null, JSONRPCError? Error = null) : JSONRPCResponse<TaskPushNotificationConfig>(Id, Result, Error)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTaskPushNotificationResponse"/> class with a result.
    /// </summary>
    /// <param name="Result">
    /// The result of the JSON-RPC call, which can be an <see cref="TaskPushNotificationConfig"/> object or null.
    /// </param>
    public GetTaskPushNotificationResponse(TaskPushNotificationConfig? Result = null) : this(Guid.NewGuid(), Result, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTaskPushNotificationResponse"/> class with an error.
    /// </summary>
    /// <param name="Error">
    /// The error details, which can be a <see cref="JSONRPCError"/> object or null.
    /// </param>
    public GetTaskPushNotificationResponse(JSONRPCError? Error) : this(Guid.NewGuid(), null, Error) { }
}

/// <summary>
/// Represents a request to resubscribe to a task.
/// </summary>
/// <param name="Id">The unique identifier for the JSON-RPC request.</param>
/// <param name="Params">The parameters for the JSON-RPC request, containing the task ID and associated metadata.</param>
public record TaskResubscriptionRequest(object Id, TaskIdParams Params) : JSONRPCRequest<TaskIdParams>(Id, Params)
{
    /// <inheritdoc />
    public override string Method => "tasks/resubscribe";

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskResubscriptionRequest"/> class with a generated ID.
    /// </summary>
    /// <param name="Params">The parameters for the JSON-RPC request, containing the task ID and associated metadata.</param>
    public TaskResubscriptionRequest(TaskIdParams Params) : this(Guid.NewGuid(), Params) { }
}

/// <summary>
/// Represents a JSON parse error.
/// </summary>
/// <param name="Data">Optional data associated with the error.</param>
public record JSONParseError(object? Data = null) : JSONRPCError(-32700, "Invalid JSON payload", Data);

/// <summary>
/// Represents an invalid request error.
/// </summary>
/// <param name="Data">Optional data associated with the error.</param>
public record InvalidRequestError(object? Data = null) : JSONRPCError(-32600, "Request payload validation error", Data);

/// <summary>
/// Represents a method not found error.
/// </summary>
public record MethodNotFoundError() : JSONRPCError(-32601, "Method not found", null);

/// <summary>
/// Represents an invalid parameters error.
/// </summary>
/// <param name="Data">Optional data associated with the error.</param>
public record InvalidParamsError(object? Data = null) : JSONRPCError(-32602, "Invalid parameters", Data);

/// <summary>
/// Represents an internal error.
/// </summary>
/// <param name="Data">Optional data associated with the error.</param>
public record InternalError(object? Data = null) : JSONRPCError(-32603, "Internal error", Data);

/// <summary>
/// Represents a task not found error.
/// </summary>
public record TaskNotFoundError() : JSONRPCError(-32001, "Task not found", null);

/// <summary>
/// Represents a task not cancelable error.
/// </summary>
public record TaskNotCancelableError() : JSONRPCError(-32002, "Task cannot be canceled", null);

/// <summary>
/// Represents a push notification not supported error.
/// </summary>
public record PushNotificationNotSupportedError() : JSONRPCError(-32003, "Push Notification is not supported", null);

/// <summary>
/// Represents an unsupported operation error.
/// </summary>
public record UnsupportedOperationError() : JSONRPCError(-32004, "This operation is not supported", null);

/// <summary>
/// Represents a content type not supported error.
/// </summary>
public record ContentTypeNotSupportedError() : JSONRPCError(-32005, "Incompatible content types", null);

/// <summary>
/// Represents an agent provider.
/// </summary>
/// <param name="Organization">The organization of the agent provider.</param>
/// <param name="Url">The URL of the agent provider.</param>
public record AgentProvider(string Organization, string? Url);

/// <summary>
/// Represents agent capabilities.
/// </summary>
/// <param name="Streaming">Indicates whether streaming is supported.</param>
/// <param name="PushNotifications">Indicates whether push notifications are supported.</param>
/// <param name="StateTransitionHistory">Indicates whether state transition history is supported.</param>
public record AgentCapabilities(bool Streaming, bool PushNotifications, bool StateTransitionHistory);

/// <summary>
/// Represents agent authentication.
/// </summary>
/// <param name="Schemes">The authentication schemes.</param>
/// <param name="Credentials">The credentials for authentication.</param>
public record AgentAuthentication(List<string> Schemes, string? Credentials);

/// <summary>
/// Represents an agent skill.
/// </summary>
/// <param name="Id">The ID of the agent skill.</param>
/// <param name="Name">The name of the agent skill.</param>
/// <param name="Description">The description of the agent skill.</param>
/// <param name="Tags">The tags associated with the agent skill.</param>
/// <param name="Examples">The examples of the agent skill.</param>
/// <param name="InputModes">The input modes for the agent skill.</param>
/// <param name="OutputModes">The output modes for the agent skill.</param>
public record AgentSkill(string Id, string Name, string? Description, List<string>? Tags, List<string>? Examples, List<string>? InputModes, List<string>? OutputModes);

/// <summary>
/// Represents an agent card containing information about the agent.
/// </summary>
/// <param name="Name">The name of the agent.</param>
/// <param name="Description">A description of the agent.</param>
/// <param name="Url">The URL of the agent.</param>
/// <param name="Provider">The provider of the agent.</param>
/// <param name="Version">The version of the agent.</param>
/// <param name="DocumentationUrl">The URL for the agent's documentation.</param>
/// <param name="Capabilities">The capabilities of the agent.</param>
/// <param name="Authentication">The authentication information for the agent.</param>
/// <param name="DefaultInputModes">The default input modes supported by the agent.</param>
/// <param name="DefaultOutputModes">The default output modes supported by the agent.</param>
/// <param name="Skills">The list of skills available to the agent.</param>
public record AgentCard(
    string Name,
    string? Description,
    Uri Url,
    AgentProvider? Provider,
    string Version,
    string? DocumentationUrl,
    AgentCapabilities Capabilities,
    AgentAuthentication? Authentication,
    List<string> DefaultInputModes,
    List<string> DefaultOutputModes,
    List<AgentSkill> Skills
)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AgentCard"/> class with default input modes.
    /// <summary>
    /// Initializes a new instance of the <see cref="AgentCard"/> class with default input modes.
    /// </summary>
    /// <param name="Name">The name of the agent.</param>
    /// <param name="Description">A description of the agent.</param>
    /// <param name="Url">The URL of the agent.</param>
    /// <param name="Provider">The provider of the agent.</param>
    /// <param name="Version">The version of the agent.</param>
    /// <param name="DocumentationUrl">The URL for the agent's documentation.</param>
    /// <param name="Capabilities">The capabilities of the agent.</param>
    /// <param name="Authentication">The authentication information for the agent.</param>
    public AgentCard(
        string Name,
        string? Description,
        Uri Url,
        AgentProvider? Provider,
        string Version,
        string? DocumentationUrl,
        AgentCapabilities Capabilities,
        AgentAuthentication? Authentication) : this(Name, Description, Url, Provider, Version, DocumentationUrl, Capabilities, Authentication, ["text"]) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentCard"/> class with specified input modes.
    /// <summary>
    /// Initializes a new instance of the <see cref="AgentCard"/> class with specified input modes.
    /// </summary>
    /// <param name="Name">The name of the agent.</param>
    /// <param name="Description">A description of the agent.</param>
    /// <param name="Url">The URL of the agent.</param>
    /// <param name="Provider">The provider of the agent.</param>
    /// <param name="Version">The version of the agent.</param>
    /// <param name="DocumentationUrl">The URL for the agent's documentation.</param>
    /// <param name="Capabilities">The capabilities of the agent.</param>
    /// <param name="Authentication">The authentication information for the agent.</param>
    /// <param name="DefaultInputModes">The default input modes supported by the agent.</param>
    public AgentCard(
        string Name,
        string? Description,
        Uri Url,
        AgentProvider? Provider,
        string Version,
        string? DocumentationUrl,
        AgentCapabilities Capabilities,
        AgentAuthentication? Authentication,
        List<string> DefaultInputModes) : this(Name, Description, Url, Provider, Version, DocumentationUrl, Capabilities, Authentication, DefaultInputModes, ["text"]) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentCard"/> class with specified input and output modes.
    /// <summary>
    /// Initializes a new instance of the <see cref="AgentCard"/> class with specified input and output modes.
    /// </summary>
    /// <param name="Name">The name of the agent.</param>
    /// <param name="Description">A description of the agent.</param>
    /// <param name="Url">The URL of the agent.</param>
    /// <param name="Provider">The provider of the agent.</param>
    /// <param name="Version">The version of the agent.</param>
    /// <param name="DocumentationUrl">The URL for the agent's documentation.</param>
    /// <param name="Capabilities">The capabilities of the agent.</param>
    /// <param name="Authentication">The authentication information for the agent.</param>
    /// <param name="DefaultInputModes">The default input modes supported by the agent.</param>
    /// <param name="DefaultOutputModes">The default output modes supported by the agent.</param>
    public AgentCard(
        string Name,
        string? Description,
        Uri Url,
        AgentProvider? Provider,
        string Version,
        string? DocumentationUrl,
        AgentCapabilities Capabilities,
        AgentAuthentication? Authentication,
        List<string> DefaultInputModes,
        List<string> DefaultOutputModes) : this(Name, Description, Url, Provider, Version, DocumentationUrl, Capabilities, Authentication, DefaultInputModes, DefaultOutputModes, []) { }
}
