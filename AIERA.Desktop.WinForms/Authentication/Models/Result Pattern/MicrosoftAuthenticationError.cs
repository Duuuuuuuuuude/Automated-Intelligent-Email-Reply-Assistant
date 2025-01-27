﻿using AIERA.Desktop.WinForms.Models.ViewModels;
using Common.Models;

namespace AIERA.Desktop.WinForms.Authentication.Models.Result_Pattern;
public class MicrosoftAuthenticationError : Error
{
    public MicrosoftAccountViewModel AccountViewModel { get; }

    public MicrosoftAuthenticationError(string code, string message, MicrosoftAccountViewModel accountViewModel) : base(code, message)
    {
        AccountViewModel = accountViewModel;
    }

    internal static MicrosoftAuthenticationError AcquireTokenInteractiveError(string errorMessage, MicrosoftAccountViewModel microsoftAccountViewModel)
        => new("MicrosoftAuthenticationError.AcquireTokenInteractiveError", errorMessage, microsoftAccountViewModel);
    internal static MicrosoftAuthenticationError MsalUiRequiredExceptionError(string errorMessage, MicrosoftAccountViewModel microsoftAccountViewModel)
        => new("MicrosoftAuthenticationError.MsalUiRequiredExceptionError", errorMessage, microsoftAccountViewModel);
    internal static MicrosoftAuthenticationError MsalServiceExceptionError(string errorMessage, MicrosoftAccountViewModel microsoftAccountViewModel)
        => new("MicrosoftAuthenticationError.MsalServiceExceptionError", errorMessage, microsoftAccountViewModel);
    internal static MicrosoftAuthenticationError MsalClientExceptionError(string errorMessage, MicrosoftAccountViewModel microsoftAccountViewModel)
        => new("MicrosoftAuthenticationError.MsalClientExceptionError", errorMessage, microsoftAccountViewModel);
    internal static MicrosoftAuthenticationError HostNotFoundExceptionError(string errorMessage, MicrosoftAccountViewModel microsoftAccountViewModel)
        => new("MicrosoftAuthenticationError.HostNotFoundExceptionError", errorMessage, microsoftAccountViewModel);
    internal static MicrosoftAuthenticationError ExceptionError(string errorMessage, MicrosoftAccountViewModel microsoftAccountViewModel)
        => new("MicrosoftAuthenticationError.ExceptionError", errorMessage, microsoftAccountViewModel);
}



public static class ResultExtensions
{
    public static MicrosoftAccountViewModel GetMicrosoftAccountViewModel(
        this Result<MicrosoftAccountViewModel, MicrosoftAuthenticationError> result)
    {
        return result.Match(success: value => value,
                            failure: error => error.AccountViewModel);
    }
}