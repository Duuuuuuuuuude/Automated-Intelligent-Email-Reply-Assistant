﻿using Common.Models;

namespace AIERA.API.Models.Result_Pattern;


public class HttpError : Error
{
    public int StatusCode { get; }

    public HttpError(string code, string message, int statusCode) : base(code, message)
    {
        StatusCode = statusCode;
    }
    //public static Error GeneralError(string message) => new Error("general.error", message);
    //public static Error ValueIsNull<T>() => new Error("value.null", $"The value of type ${typeof(T)} is null.");
    //public static Error UnspecifiedError(string message) => new Error("unspecified.error", message);
    //public static Error FromException(Exception exception) => new Error("exception", exception.Message, 500);
    //public static Error NotFound<T>(T id) where T : struct => new Error("entity.not.found", $"Could not find entity with ID {id}.", statusCode: 404);
    //public static Error ValueIsRequired(string valueName) => new Error("value.is.required", $"Value '{valueName}' is required.");
    //public static Error ValueTooSmall(string valueName, int minValue) => new Error("value.too.small", $"Value '{valueName}' should be at least {minValue}.");
    //public static Error ValueTooLarge(string valueName, int maxValue) => new Error("value.too.large", $"Value '{valueName}' should not exceed {maxValue}.");
    //public static Error UnexpectedValue(string value) => new Error("unexpected.value", $"Value '{value}' is not valid in this context");
    //public static Error Unauthorized() => new Error("unauthorizaed", $"Could not authorize access to entity");
    //public static Error ValueIsEmpty(string value) => new Error("value.empty", $"The value cannot be empty: {value} ");

    //public static Error ValueOutOfRange(string valueName, int minValue, int maxValue) =>
    //    new Error("value.out.of.Range", $"Value '{valueName}' should be between {minValue} and {maxValue}.");
}