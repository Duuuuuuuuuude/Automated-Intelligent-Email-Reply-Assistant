using MailKit;
using Microsoft.Extensions.Logging;
using System.Text;

namespace AIERA.AIEmailClient.Logging;


/// <summary>
/// A protocol logger that logs messages from the smtp or imap client and server to an to an <see cref="ILogger{TCategoryName}"/>.
/// </summary>
/// <typeparam name="TCategoryName"></typeparam>
public sealed class ProtocolLoggerILogger<TCategoryName> : IProtocolLogger
{
    private readonly ILogger<TCategoryName> _logger;

    public IAuthenticationSecretDetector? AuthenticationSecretDetector { get; set; } // TODO: Implement AuthenticationSecretDetector.

    public ProtocolLoggerILogger(ILogger<TCategoryName> logger) => _logger = logger;


    public void LogConnect(Uri uri)
    {
        _logger.LogInformation("Connected to {Uri}", uri);
    }

    public void LogClient(byte[] buffer, int offset, int count)
    {
        var message = Encoding.UTF8.GetString(buffer, offset, count);
        _logger.LogDebug("Client: {Message}", message.TrimEnd());
    }

    public void LogServer(byte[] buffer, int offset, int count)
    {
        var message = Encoding.UTF8.GetString(buffer, offset, count);
        _logger.LogDebug("Server: {Message}", message.TrimEnd());
    }

    public void Dispose()
    {
        //throw new NotImplementedException();
    }
}
