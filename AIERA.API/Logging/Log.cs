namespace AIERA.API.Logging;

// https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator
public static partial class Log
{
    [LoggerMessage(EventId = 0, Message = "Logging worked: `{HostName}`")]
    public static partial void TestLogging(this ILogger logger, LogLevel level, string hostName);
}