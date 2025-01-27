namespace Common.Exceptions;

[Serializable]
public class NetworkException : Exception
{
    /// <summary>
    /// Can be thrown if there is no internet connection.
    /// Default message is "No internet connection. Check your internet and try again.".
    /// </summary>
    public NetworkException() : base("No internet connection. Check your internet and try again.")
    {
    }


    /// <summary>
    /// Can be thrown if there is no internet connection.
    /// </summary>
    /// <param name="message"></param>
    public NetworkException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Can be thrown if there is no internet connection.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public NetworkException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}