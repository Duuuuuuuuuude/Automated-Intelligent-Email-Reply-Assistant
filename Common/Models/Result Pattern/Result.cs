namespace Common.Models;


/// <summary>
/// A generic way of returning objects from a method. Also prevents exceptions being thrown back as communication
/// </summary>
public class Result
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; private set; }

    protected Result(bool success, Error? error)
    {
        IsSuccess = success;
        Error = error;
    }

    public static Result Fail(Error error) => new(success: false, error);

    public static Result<TValue, TError> Fail<TValue, TError>(Error error) where TError : Error
    {
        return new Result<TValue, TError>(value: default, success: false, error);
    }

    public static Result Ok() => new(success: true, error: null);

    public static Result<TValue, TError> Ok<TValue, TError>(TValue value) where TError : Error
    {
        return new Result<TValue, TError>(value, success: true, error: null);
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



public class Result<TValue, TError> : Result where TError : Error
{
    public new TError? Error { get; private set; }

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

    protected internal Result(TValue? value, bool success, Error? error) : base(success, error)
    {
        if (value == null && success)
            throw new InvalidOperationException("Pass a value if result is successful");

        Value = value;
    }

    public static implicit operator Result<TValue, TError>(TValue from) => Ok<TValue, TError>(from);
    public static implicit operator TValue(Result<TValue, TError> from) => from.Value!;

    public Result<TValue, TError> Match(Func<TValue, Result<TValue, TError>> success,
                                        Func<TError, Result<TValue, TError>> failure)
    {
        if (IsSuccess)
        {
            return success(Value!);
        }

        return failure(Error!);
    }
}