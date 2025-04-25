namespace Common;
using Microsoft.Extensions.Logging;

public class DebugHttpHandler : DelegatingHandler
{
    private readonly ILogger _log;

    public DebugHttpHandler(ILoggerFactory loggerFactory) : base() => _log = loggerFactory.CreateLogger<DebugHttpHandler>();

    public DebugHttpHandler(ILoggerFactory loggerFactory, HttpMessageHandler innerHandler) : base(innerHandler) => _log = loggerFactory.CreateLogger<DebugHttpHandler>();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_log.IsEnabled(LogLevel.Trace) && request.Content is not null)
        {
            var body = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            _log.REQUESTRequestBody(body);
        }

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (_log.IsEnabled(LogLevel.Trace) && response.Content is not null)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            _log.RESPONSEResponseContent(body);
        }

        return response;
    }
}