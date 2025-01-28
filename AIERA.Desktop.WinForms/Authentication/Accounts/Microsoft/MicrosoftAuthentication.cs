using AIERA.Desktop.WinForms.Authentication.Configurations.AcquireToken;
using AIERA.Desktop.WinForms.Authentication.Models.Result_Pattern;
using AIERA.Desktop.WinForms.Models.ViewModels;
using AIERA.Desktop.WinForms.Toaster;
using AIERA.Desktop.WinForms.Toaster.Enums;
using Common.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace AIERA.Desktop.WinForms.Authentication.Accounts.Microsoft;


public partial class MicrosoftAuthentication : IMicrosoftAuthentication
{
    private readonly IPublicClientApplication _PublicClientApp;
    private readonly IToastNotification _toastNotification;

    #region Sign in
    private readonly AcquireTokenConfig _acquireTokenOptions;

    public MicrosoftAuthentication(IPublicClientApplication publicClientApp,
                                   IToastNotification toastNotification,
                                   IOptions<AcquireTokenConfig> acquireTokenOptions)
    {
        _PublicClientApp = publicClientApp;
        _toastNotification = toastNotification;
        _acquireTokenOptions = acquireTokenOptions.Value;
    }

    /// <summary>
    /// Signs in to the Microsoft account interactively using.
    /// </summary>
    /// <param name="hWnd">The <see cref="Control.Handle"/> used by the broker.</param>
    /// <param name="account"></param>
    /// <param name="claims">Specifies the claims that the application might request the user to consent to.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ///     /// <exception cref="HttpRequestException">This exception is sometimes thrown when there is no internet connection.
    ///     It would then have an inner <see cref="SocketException"/> with a <see cref="SocketError.HostNotFound"/> error code indicating the host is unreachable.</exception>
    public Task<Result<MicrosoftAccountViewModel>> SignInAsync(nint? hWnd, IAccount? account, string? claims, CancellationToken cancellationToken = default)
    {
        return SignInAsync(hWnd, account?.Username, account?.HomeAccountId.Identifier, claims, cancellationToken);
    }

    /// <summary>
    /// Signs in to the Microsoft account interactively using.
    /// </summary>
    /// <param name="hWnd">The <see cref="Control.Handle"/> used by the broker.</param>
    /// <param name="loginHint">The login hint, which is the same us the <see cref="MicrosoftAccountViewModel.Username"/>.</param>
    /// <param name="identifier"></param>
    /// <param name="claims">Specifies the claims that the application might request the user to consent to.</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="HttpRequestException">This exception is sometimes thrown when there is no internet connection.
    ///     It would then have an inner <see cref="SocketException"/> with a <see cref="SocketError.HostNotFound"/> error code indicating the host is unreachable.</exception>
    /// <returns></returns>
    public async Task<Result<MicrosoftAccountViewModel>> SignInAsync(nint? hWnd, string? loginHint, string? identifier, string? claims, CancellationToken cancellationToken = default)
    {
        bool retry = false;
        string? errorMessage = null;
        AuthenticationResult? authResult;
        do
        {
            try
            {
                var acquireTokenInteractiveParameterBuilder = _PublicClientApp.AcquireTokenInteractive(_acquireTokenOptions.Scopes)
                                                                              .WithParentActivityOrWindow(hWnd ??= GetFallbackHandle());
                if (!string.IsNullOrWhiteSpace(loginHint))
                { // TODO: Virker ikke som forventet. Select account prompt bliver stadig vist.
                    acquireTokenInteractiveParameterBuilder = acquireTokenInteractiveParameterBuilder.WithLoginHint(loginHint);
                    acquireTokenInteractiveParameterBuilder = acquireTokenInteractiveParameterBuilder.WithPrompt(Prompt.NoPrompt);
                }

                if (!string.IsNullOrWhiteSpace(claims))
                    acquireTokenInteractiveParameterBuilder = acquireTokenInteractiveParameterBuilder.WithClaims(claims);

                authResult = await acquireTokenInteractiveParameterBuilder.ExecuteAsync(cancellationToken); // CancellationToken doesn't close the WAM Broker. After looking online, it seems like a common problem and there doesn't to be a solution.

                MicrosoftAccountViewModel accountViewModel = new(authResult.Account, authResult, claims);

                ShowSuccessfuleSignInToastNotification(accountViewModel);

                return accountViewModel;
            }
            catch (MsalClientException ex) when (ex.ErrorCode.Equals(MsalError.AuthenticationCanceledError, StringComparison.Ordinal) ||
                                                 ex.ErrorCode.Equals(MsalError.AuthenticationFailed, StringComparison.Ordinal))
            { /*If user canceled or mistyped password, do nothing.*/ }
            catch (MsalClientException)
            {
                HandleMsalClientException(username: (await _PublicClientApp.GetAccountAsync(identifier)).Username, out retry, out errorMessage);
                //throw; 
                // LOG. https://learn.microsoft.com/en-us/dotnet/api/microsoft.identity.client.msalexception.additionalexceptiondata
            }
            catch (HttpRequestException ex) when (ex.InnerException is SocketException { SocketErrorCode: SocketError.HostNotFound }) // Sometimes happens instead of MsalServiceException, if internet is not available.
            {
                HandleHostNotFoundError(username: (await _PublicClientApp.GetAccountAsync(identifier)).Username, out retry, out errorMessage);
                //throw; 
                // LOG
            }
            catch (MsalServiceException ex)
            {
                string? username = identifier is null ? null : (await _PublicClientApp.GetAccountAsync(identifier))?.Username;
                HandleServiceException(username: username, out claims, out retry, out errorMessage, ex);
                //throw; 
                // LOG
            }
        } while (retry);

        var error = MicrosoftAuthenticationError.AcquireTokenInteractiveError(errorMessage!, new MicrosoftAccountViewModel(await _PublicClientApp.GetAccountAsync(identifier),
                                                                              AuthResult: null,
                                                                              claims));

        return Result.Fail<MicrosoftAccountViewModel>(error);
    }

    #region Toasts
    private void ShowSuccessfuleSignInToastNotification(MicrosoftAccountViewModel accountViewModel)
    {
        _toastNotification.ShowMsalToastNotification(headerId: HeaderId.SuccessfuleSignIn,
                                                     headerTitle: $"{Program.ApplicationNameAbbreviation} - '{accountViewModel.Username}' has signed in.",
                                                     group: ToastGroup.SuccessfuleSignIn,
                                                     account: accountViewModel.Account,
                                                     buttons: [],
                                                     firstLine: $"'{accountViewModel.Username}' has successfully been signed in and is now ready to be used with {Program.ApplicationNameFull}.");
    }
    #endregion

    #region Exception handling
    private static void HandleMsalClientException(string? username, out bool retry, out string? errorMessage)
    {
        string userDisplayName = string.IsNullOrEmpty(username) ? "the account" : $"account '{username}'";

        errorMessage = $"An unexpected error on the client side has occurred while trying to sign in with '{userDisplayName}'. Please contact an administrator.";

        var dialogResult = MessageBox.Show(errorMessage,
                                           $"{Program.ApplicationNameAbbreviation} - Unexpected client side error",
                                           MessageBoxButtons.RetryCancel,
                                           MessageBoxIcon.Error);

        retry = dialogResult is DialogResult.Retry;
    }

    private static void HandleHostNotFoundError(string? username, out bool retry, out string? errorMessage)
    {
        string userDisplayName = string.IsNullOrEmpty(username) ? "the account" : $"account '{username}'";

        if (InternetAvailability.IsInternetAvailable() is false)
            errorMessage = $"Failed to sign in with {userDisplayName}. Please check your network connection and try again.";
        else
            errorMessage = $"Failed to sign in with {userDisplayName}. Possible cause: no Internet connection." +
                           "\nPlease check your network connection and try again.";

        var dialogResult = MessageBox.Show(errorMessage,
                                           $"{Program.ApplicationNameAbbreviation} - Unexpected Network Error",
                                           MessageBoxButtons.RetryCancel,
                                           MessageBoxIcon.Error);

        retry = dialogResult is DialogResult.Retry;
    }

    private static void HandleServiceException(string? username, out string? claims, out bool retry, out string? errorMessage, MsalServiceException ex)
    {
        errorMessage = ExceptionErrorMessagesMicrosoft.GetExceptionErrorMessage(ex, username);

        string accountDisplayName = string.IsNullOrEmpty(username) ? "" : $" - ({username})";
        var dialogResult = MessageBox.Show(errorMessage,
                                           $"{Program.ApplicationNameAbbreviation} - Authentication Error{accountDisplayName}",
                                           MessageBoxButtons.RetryCancel,
                                           MessageBoxIcon.Error);
        claims = ex.Claims;

        retry = dialogResult is DialogResult.Retry;
    }
    #endregion

    #region hWnd/Handle
    /// <summary>
    /// Gets the handle of the first open form in the application, or the desktop window handle if no forms are open.
    /// </summary>
    /// <remarks>
    /// This will first look for the <see cref="Control.Handle"/> of the <see cref="LoginForm"/> 
    /// then the <see cref="SettingsForm"/> 
    /// and finally the first open form or the desktop window handle if no forms are open.
    /// </remarks>
    /// <returns></returns>
    private static nint? GetFallbackHandle()
    {
        return Application.OpenForms[nameof(LoginForm)]?.Handle ??
               Application.OpenForms[nameof(SettingsForm)]?.Handle ??
               Application.OpenForms[0]?.Handle ??
               User32.GetDesktopWindow();
    }
    private static partial class User32
    {
        [LibraryImport("user32.dll")]
        public static partial nint GetDesktopWindow();
    }
    #endregion
    #endregion

    /// <summary>
    /// Handles the "Sign Out Account" button click. This will remove the cached token associated with the account, from
    /// the MSAL client, resulting in any future usage requiring a re-authentication
    /// experience, meaning the next token request will require the user to sign in.
    /// </summary>
    /// <param name="authResult"></param>
    public Task SignOutAsync(AuthenticationResult authResult) => _PublicClientApp.RemoveAsync(authResult.Account);

    /// <summary>
    /// Handles the "Sign Out Account" button click. This will remove the cached token associated with the account, from
    /// the MSAL client, resulting in any future usage requiring a re-authentication
    /// experience, meaning the next token request will require the user to sign in.
    /// </summary>
    /// <param name="account"></param>
    public Task SignOutAsync(IAccount account) => _PublicClientApp.RemoveAsync(account);
}
