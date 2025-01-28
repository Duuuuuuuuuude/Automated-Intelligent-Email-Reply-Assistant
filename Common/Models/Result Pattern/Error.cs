namespace Common.Models;
public class Error
{
    public string Code { get; }
    public string Message { get; }

    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    //public static Error TaskCancelledExceptionError(string errorMessage)
    //    => new("Error.TaskCanceledException", errorMessage);

    public override int GetHashCode()
    {
        return HashCode.Combine(Code);
    }
}