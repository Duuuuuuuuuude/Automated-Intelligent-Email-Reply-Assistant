using Microsoft.Identity.Client;

namespace AIERA.Desktop.WinForms.Authentication.Accounts.Microsoft;
public static class ExceptionErrorMessagesMicrosoft
{
    public static string GetExceptionErrorMessage(MsalUiRequiredException ex, IAccount account) => GetExceptionErrorMessage(ex, account.Username);
    public static string GetExceptionErrorMessage(MsalUiRequiredException ex, string username)
    {
        string errorMessage = ex.Classification switch
        {
            UiRequiredExceptionClassification.BasicAction => $"Failed to authenticate '{username}'. \n" +
                                                             "Please sign in again to solve the problem.",

            UiRequiredExceptionClassification.AdditionalAction => $"Failed to authenticate '{username}'.\n" +
                                                                  $"Additional steps are required to continue using this account with {Program.ApplicationNameAbbreviation}.\n" +
                                                                  "Please sign in again to see problem.",

            UiRequiredExceptionClassification.MessageOnly => $"Failed to authenticate '{username}'." +
                                                             $"\nPlease sign in again to see problem.",

            UiRequiredExceptionClassification.ConsentRequired => "At least one required consent is missing, or has been revoked.\n" +
                                                                $"Please sign in with account '{username}' again and give the neccessary consent in order for {Program.ApplicationNameAbbreviation} " +
                                                                $"to be able to access and reply emails from that account.",


            UiRequiredExceptionClassification.UserPasswordExpired => $"Password has expired. Please reset password to continue using '{username}' with {Program.ApplicationNameAbbreviation}.",

            //UiRequiredExceptionClassification.PromptNeverFailed => "",

            UiRequiredExceptionClassification.AcquireTokenSilentFailed => $"Token acquisition failed. Please sign in with account '{username}' again to see more.",

            UiRequiredExceptionClassification.None => $"Failed to authenticate '{username}'.\n" +
                                                      "Unknown error occurred.\n" +
                                                      $"Please sign-in again to continue using {Program.ApplicationNameAbbreviation} with this account.",

            _ => "Failed to authenticate account." +
                 "\nUnknown error occurred.\n" +
                 $"Please sign-in again to continue using {Program.ApplicationNameAbbreviation} with '{username}'.",
        };
        return errorMessage;
    }

    public static string GetExceptionErrorMessage(MsalServiceException ex, IAccount account) => GetExceptionErrorMessage(ex, account.Username);
    public static string GetExceptionErrorMessage(MsalServiceException ex, string? username)
    {
        string userDisplayName = string.IsNullOrEmpty(username) ? "the account" : $"account '{username}'";
        string userDisplayNameCapitalFirstLetter = string.IsNullOrEmpty(username) ? "The account" : $"Account '{username}'";

        string errorMessage = ex.ErrorCode switch
        {
            MsalServiceErrorCodes.InternetConnection => $"Failed to authenticate {userDisplayName}. Check internet connection and try again.",
            MsalServiceErrorCodes.AccountDoesNotExist => $"{userDisplayNameCapitalFirstLetter} doesn't exists.",
            //MsalServiceErrorCodes.NoCredentialsAvailable => "",
            MsalServiceErrorCodes.AccountDetailsUpdateRequired => $"You must sign in to {userDisplayName} using a browser and update account details to continue using the account with {Program.ApplicationNameAbbreviation}",
            MsalServiceErrorCodes.ReSignInRequired => $"Re-sign in is required, to continue using {userDisplayName} with {Program.ApplicationNameAbbreviation}",
            MsalServiceErrorCodes.EmailVerificationRequired => $"Email verification is required before using {userDisplayName}.",
            MsalServiceErrorCodes.UnusualActivity => $"Sign in using a browser to ensure {userDisplayName} is not suspended.",
            MsalServiceErrorCodes.SuspiciousActivity => $"{userDisplayNameCapitalFirstLetter} is temporarily blocked by Microsoft because of suspicious activity.",
            MsalServiceErrorCodes.UserInteractionRequiredForAuthentication => $"Failed to authenticate {userDisplayName}. Sign in again is required to continue using {Program.ApplicationNameAbbreviation} with this account.",
            MsalServiceErrorCodes.UserInteractionRequired => $"Failed to authenticate {userDisplayName}. Sign in again is required to continue using {Program.ApplicationNameAbbreviation} with this account.",
            MsalServiceErrorCodes.UserAccountNotFoundInTenant => $"{userDisplayNameCapitalFirstLetter} was not found in the tenant directory. Ensure the account is registered in Microsoft Entra ID.",
            MsalServiceErrorCodes.MultiFactorAuthenticationExpired => "Presented multi-factor authentication has expired.\n" +
                            "An Microsoft Entra administrator must configure the MFA settings to make sure they are up to date.",
            //MsalServiceErrorCodes.KeysetNotFound => ".",
            //MsalServiceErrorCodes.DeviceNotFound => ".",
            //MsalServiceErrorCodes.BadKeyset => ".",
            _ => $"Authentication service error, while authenticating {userDisplayName}.\n" +
                 $"Try again later or contact your account administrator.",
        };

        return errorMessage;
    }

    // https://learn.microsoft.com/en-us/entra/msal/dotnet/advanced/exceptions/wam-errors
    private static class MsalServiceErrorCodes
    {
        public const string InternetConnection = "2147943631";
        public const string AccountDoesNotExist = "2147943717";
        public const string NoCredentialsAvailable = "2148074254";
        public const string AccountDetailsUpdateRequired = "2156265477";
        public const string ReSignInRequired = "2156265478";
        public const string EmailVerificationRequired = "2156265481";
        public const string UnusualActivity = "2156265482";
        public const string SuspiciousActivity = "2156265483";
        public const string UserInteractionRequiredForAuthentication = "2156265484";
        public const string UserInteractionRequired = "3399548929";
        public const string UserAccountNotFoundInTenant = "3399614467";
        public const string MultiFactorAuthenticationExpired = "3399614476";
        public const string KeysetNotFound = "2148073494";
        public const string DeviceNotFound = "2148073520";
        public const string BadKeyset = "80090016";
    }
}
