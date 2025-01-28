namespace AIERA.Desktop.WinForms.Authentication.Models.Result_Pattern;

/// <summary>
/// A generic way of returning objects from a method. Also prevents exceptions being thrown back as communication
/// </summary>
public class Result
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public MicrosoftAuthenticationError? Error { get; private set; }

    protected Result(bool success, MicrosoftAuthenticationError? microsoftAuthenticationError)
    {
        IsSuccess = success;
        Error = microsoftAuthenticationError;
    }

    public static Result Fail(MicrosoftAuthenticationError microsoftAuthenticationError) => new(success: false, microsoftAuthenticationError);

    public static Result<TValue> Fail<TValue>(MicrosoftAuthenticationError microsoftAuthenticationError)
    {
        return new Result<TValue>(value: default, success: false, microsoftAuthenticationError);
    }

    public static Result Ok() => new(success: true, microsoftAuthenticationError: null);

    public static Result<TValue> Ok<TValue>(TValue value)
    {
        return new Result<TValue>(value, success: true, microsoftAuthenticationError: null);
    }

    public static Result Combine(params Result[] results)
    {
        foreach (Result result in results)
        {
            if (result.IsFailure)
                return result;
        }

        return Ok();
    }
}



public class Result<TValue> : Result
{
    private TValue? _value;
    public TValue? Value

    {
        get
        {
            if (IsSuccess)
                return _value;

            throw new InvalidOperationException("Cannot fetch value on a failed result");
        }

        private set { _value = value; }
    }

    protected internal Result(TValue? value, bool success, MicrosoftAuthenticationError? microsoftAuthenticationError) : base(success, microsoftAuthenticationError)
    {
        if (value == null && success)
            throw new InvalidOperationException("Pass a value if result is successful");
        Value = value;
    }

    public static implicit operator Result<TValue>(TValue from) => Ok(from);
    public static implicit operator TValue(Result<TValue> from) => from.Value!;

    public Result<TValue> Match(Func<TValue, Result<TValue>> success,
                                Func<MicrosoftAuthenticationError, Result<TValue>> failure)
    {
        if (IsSuccess)
        {
            return success(Value!);
        }

        return failure(Error!);
    }
}