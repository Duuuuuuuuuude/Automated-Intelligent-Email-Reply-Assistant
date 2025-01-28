using AIERA.Desktop.WinForms.Authentication.Accounts.Microsoft;
using AIERA.Desktop.WinForms.Authentication.Configurations.AcquireToken;
using AIERA.Desktop.WinForms.Authentication.Models.Result_Pattern;
using AIERA.Desktop.WinForms.IoC.Factories;
using AIERA.Desktop.WinForms.Models.ViewModels;
using Common.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Polly;
using System.Net.Sockets;

namespace AIERA.Desktop.WinForms;

public partial class SettingsForm : Form
{
    private readonly IPublicClientApplication _PublicClientApp;
    private readonly AcquireTokenConfig _acquireTokenConfig;
    private readonly IMicrosoftAuthentication _microsoftAuthentication;

    private readonly CancellationTokenSource _settingsFormClosingCancellationTokenSource = new();
    private readonly IAsyncPolicy _retryPolicy;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Do not call directly, but use <see cref="FormFactory.GetOpenOrCreateNewSettingsForm"/> instead.</remarks>
    /// <param name="PublicClientApp"></param>
    /// <param name="AcquireTokenConfigOptions"></param>
    /// <param name="microsoftAuthentication"></param>
    /// <param name="retryPolicy"></param>
    public SettingsForm(IPublicClientApplication PublicClientApp, IOptions<AcquireTokenConfig> AcquireTokenConfigOptions, IMicrosoftAuthentication microsoftAuthentication, IAsyncPolicy retryPolicy)
    {
        _PublicClientApp = PublicClientApp;
        _acquireTokenConfig = AcquireTokenConfigOptions.Value;
        _microsoftAuthentication = microsoftAuthentication;
        _retryPolicy = retryPolicy;
        InitializeComponent();
    }

    public new void Show()
    {
        base.Show();
        WindowState = FormWindowState.Normal;
        BringToFront();
        Activate();
        _ = Focus();
    }

    #region Event handlers

    #region OnLoad event
    private async void SettingsForm_Load(object sender, EventArgs e)
    {
        try
        {
            await OnSettingsFormLoadAsync();
        }
        catch (Exception)
        {
            throw; // LOG
        }
    }

    private Task OnSettingsFormLoadAsync()
    {
        Text += Program.ApplicationNameFull;
        return InitializeAccountButtonsAsync();
    }


    #region Initialize account buttons
    /// <summary>
    /// Initializes the account buttons, by acquiring tokens for each account and creating buttons based on the results.
    /// </summary>
    /// <returns></returns>
    private async Task InitializeAccountButtonsAsync()
    {
        UpdateLblStatus("Loading accounts...");

        var accounts = await _PublicClientApp.GetAccountsAsync();

        UpdateLblAccountsLoadStatus(text: accounts.Any() ? "Accounts are loading..." : "No accounts found.");

        // Acquire tokens and return button data
        IEnumerable<Task<Result<MicrosoftAccountViewModel>>> accountDataTasks = accounts.Select(async account => await FetchAccountDataAsync(account, _settingsFormClosingCancellationTokenSource.Token));
        Result<MicrosoftAccountViewModel>[] accountData = await Task.WhenAll(accountDataTasks);

        // Create buttons based on account data
        Button[] buttons = accountData.Select(CreateButtonFromAccountData).ToArray<Button>();

        AddButtonsToUI(buttons);

        if (accounts.Any())
            UpdateLblAccountsLoadStatus(text: null);

        RestoreLblStatusToDefaultText();
    }


    /// <summary>
    /// Prepares account data by attempting to acquire a token or capturing an error message that can be used to inform the user of any issues or just to show the data for that account.
    /// </summary>
    /// <remarks>
    /// This method might be very slow in Debug mode, but fast in Release mode.
    /// </remarks>
    /// <param name="account"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<Result<MicrosoftAccountViewModel>> FetchAccountDataAsync(IAccount account, CancellationToken cancellationToken)
    {
        try
        {
            var authResult = await _retryPolicy.ExecuteAsync(async cancellationToken =>
            {
                return await _PublicClientApp.AcquireTokenSilent(_acquireTokenConfig.Scopes, account)
                                             .ExecuteAsync(cancellationToken);
            }, cancellationToken);

            return Result.Ok<MicrosoftAccountViewModel>(new MicrosoftAccountViewModel(authResult.Account,
                                                                                                                    authResult,
                                                                                                                    Claims: null));
        }
        catch (TaskCanceledException)
        {
            return HandleTaskCanceledException(account);
            // LOG
        }
        catch (MsalUiRequiredException ex) /*when (ex.ErrorCode.Equals(MsalError.InvalidGrantError))*/
        {
            //throw; // LOG:  https://learn.microsoft.com/en-us/dotnet/api/microsoft.identity.client.msalexception.additionalexceptiondata
            return HandleMsalUiRequiredException(account, ex);
        }
        catch (MsalClientException)
        {
            //throw; 
            // LOG. https://learn.microsoft.com/en-us/dotnet/api/microsoft.identity.client.msalexception.additionalexceptiondata
            return HandleMsalClientException(account);
        }
        catch (HttpRequestException ex) when (ex.InnerException is SocketException { SocketErrorCode: SocketError.HostNotFound }) // Sometimes happens instead of MsalServiceException, if internet is not available.
        {
            //throw; 
            // LOG
            return HandleHostNotFoundException(account);
        }
        catch (MsalServiceException ex)
        {
            //throw; 
            // LOG
            return HandleMsalServiceException(account, ex);
        }
        catch (Exception)
        {
            //throw; 
            // LOG
            return HandleException(account);
        }
    }


    #region Exception handlers

    /// <summary>
    /// Handles a <see cref="TaskCanceledException"/> that occurred while trying to acquire a token.
    /// </summary>
    /// <remarks>All paths has to return a value, otherwise this method would not be necessary.</remarks>
    /// <returns></returns>
    private static Result<MicrosoftAccountViewModel> HandleTaskCanceledException(IAccount account)
    {
        var error = MicrosoftAuthenticationError.TaskCancelledExceptionError("Authentication Canceled.", account);
        return Result.Fail<MicrosoftAccountViewModel>(error);
    }

    private static Result<MicrosoftAccountViewModel> HandleMsalUiRequiredException(IAccount account, MsalUiRequiredException ex)
    {
        MicrosoftAuthenticationError error = MicrosoftAuthenticationError.MsalUiRequiredExceptionError(ExceptionErrorMessagesMicrosoft.GetExceptionErrorMessage(ex, account),
                                                                                new MicrosoftAccountViewModel(account, AuthResult: null, ex.Claims));
        return Result.Fail<MicrosoftAccountViewModel>(error);
    }

    private static Result<MicrosoftAccountViewModel> HandleMsalClientException(IAccount account)
    {
        var error = MicrosoftAuthenticationError.MsalClientExceptionError("Client-side issue. Please contact an admin.",
                                                                          new MicrosoftAccountViewModel(account, AuthResult: null, Claims: null));
        return Result.Fail<MicrosoftAccountViewModel>(error);
    }

    private static Result<MicrosoftAccountViewModel> HandleHostNotFoundException(IAccount account)
    {
        string errorMessage;

        if (InternetAvailability.IsInternetAvailable() is false)
            errorMessage = "Failed to authenticate account. Check internet connection and try again.";
        else
            errorMessage = "Failed to authenticate account. Possible cause: no Internet connection.";

        var error = MicrosoftAuthenticationError.HostNotFoundExceptionError(errorMessage,
                                                                            new MicrosoftAccountViewModel(account, AuthResult: null, Claims: null));
        return Result.Fail<MicrosoftAccountViewModel>(error);
    }

    private static Result<MicrosoftAccountViewModel> HandleMsalServiceException(IAccount account, MsalServiceException ex)
    {
        var error = MicrosoftAuthenticationError.MsalServiceExceptionError(ExceptionErrorMessagesMicrosoft.GetExceptionErrorMessage(ex, account),
                                                                           new MicrosoftAccountViewModel(account, AuthResult: null, ex.Claims));
        return Result.Fail<MicrosoftAccountViewModel>(error);
    }

    private static Result<MicrosoftAccountViewModel> HandleException(IAccount account)
    {
        var error = MicrosoftAuthenticationError.ExceptionError($"Unexpected error while authenticating '{account.Username}'. Please contact an admin.",
                                                                new MicrosoftAccountViewModel(account, AuthResult: null, Claims: null));

        return Result.Fail<MicrosoftAccountViewModel>(error);
    }
    #endregion

    #endregion
    #endregion

    #region Sign out clicked
    private async void BtnRemoveAccount_Click(object sender, EventArgs e)
    {
        try
        {
            Button button = (Button)sender;
            var account = (IAccount)button.Tag!;

            await RemoveAccount(account);
        }
        catch (Exception)
        {
            // LOG
            _ = MessageBox.Show("An unknown error has occurred while trying to remove account. Please try again or contact admin.", $"{Program.ApplicationNameAbbreviation} - Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Task RemoveAccount(IAccount account) => _microsoftAuthentication.SignOutAsync(account);// TODO: Update UI. (Remove account from settings, cancel smtp and imapc operations.)
    #endregion

    #region Add accountbutton clicked/show login window
    private void BtnAddAccount_Click(object sender, EventArgs e)
    {
        try
        {
            ShowLoginWindow();
        }
        catch (Exception)
        {
            //throw;
            // LOG
            _ = MessageBox.Show("An unknown error has occurred. Please try again or contact an administrator.", $"{Program.ApplicationNameAbbreviation} - Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ShowLoginWindow()
    {
        using LoginForm loginForm = new FormFactory().GetOpenOrCreateNewLoginForm();
        DialogResult dialogResult = loginForm.ShowDialog();

        if (dialogResult is DialogResult.Abort or DialogResult.Cancel)
            return;

        UpsertAccountButton(loginForm.MicrosoftAuthenticationResult!);
    }
    #endregion

    #region Sign in button clicked event handler
    private async void BtnSignIn_Click(object sender, EventArgs e)
    {
        MicrosoftAccountViewModel? accountViewModel = null;
        try
        {
            var button = (Button)sender;

            accountViewModel = (MicrosoftAccountViewModel)button.Tag!;
            await SignInToAccount(accountViewModel);
        }
        catch (Exception)
        {
            // LOG
            _ = MessageBox.Show($"An unknown error has occurred while trying to sign in to '{accountViewModel!.Account.Username}'. Please try again or contact admin.",
                            $"{Program.ApplicationNameAbbreviation} - Unexpected Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
        }
    }

    private async Task SignInToAccount(MicrosoftAccountViewModel accountViewModel)
    {
        Result<MicrosoftAccountViewModel> authenticationResult = await _microsoftAuthentication.SignInAsync(Handle, accountViewModel.Account, claims: null, _settingsFormClosingCancellationTokenSource.Token);
        UpsertAccountButton(authenticationResult.GetMicrosoftAccountViewModel());
    }
    #endregion

    #region Form Closing event handler
    private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        _settingsFormClosingCancellationTokenSource.Cancel();
    }
    #endregion
    #endregion

    #region Status label update/restore

    /// <summary>
    /// Updates the visibility and text of the <see cref="LblAccountsLoadStatus"/> label, and manages its placement in the <see cref="TapelLayoutPanelAccounts"/> control.
    /// Ensures thread-safety by invoking on the UI thread if necessary.
    /// </summary>
    /// <remarks>
    /// <b>Important:</b> This method will clear the entire <see cref="TapelLayoutPanelAccounts"/> control each time it is called.
    /// This is because the label should be the only control in the panel when visible. If the caller has added other
    /// controls (e.g., buttons), those will be removed. Ensure that clearing the panel is acceptable in your use case.
    /// </remarks>
    /// <param name="text">An optional string parameter to update the text of the label. Defaults to null if not provided.</param>
    private void UpdateLblAccountsLoadStatus(string? text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            flowLayoutPanelAccountButtons.Controls.Clear();
            lblAccountsLoadStatus.Text = text;
            lblAccountsLoadStatus.Visible = true;

            // Adjust row styles to give all space to LblAccountsLoadStatus
            tableLayoutPanelAccountButtonsAndStatusLabel.RowStyles[0] = new RowStyle(SizeType.Absolute, 0); // Hide buttons
            tableLayoutPanelAccountButtonsAndStatusLabel.RowStyles[1] = new RowStyle(SizeType.Percent, 100); // Full height for status label
        }
        else
        {
            lblAccountsLoadStatus.Visible = false;
            lblAccountsLoadStatus.Text = string.Empty;
            // Adjust row styles to give all space to flowLayoutPanelAccountButtons
            tableLayoutPanelAccountButtonsAndStatusLabel.RowStyles[0] = new RowStyle(SizeType.Percent, 100); // Full height for buttons
            tableLayoutPanelAccountButtonsAndStatusLabel.RowStyles[1] = new RowStyle(SizeType.Absolute, 0); // Hide status label}
        }
    }


    public void UpdateLblStatus(string newStatus)
    {
        if (InternetAvailability.IsInternetAvailable() is false)
            newStatus += " - Unable to connect to the internet for one or more account.";

        lblStatus.Text = "status: " + newStatus;
    }

    public void RestoreLblStatusToDefaultText() => lblStatus.Text = _lblStatusDefaultText;
    #endregion
}

