using AIERA.AIEmailClient.Exceptions;
using AIERA.Desktop.WinForms.Authentication.Accounts.Microsoft;
using AIERA.Desktop.WinForms.Authentication.Configurations.AcquireToken;
using AIERA.Desktop.WinForms.IoC.Factories;
using AIERA.Desktop.WinForms.Toaster;
using AIERA.Desktop.WinForms.Toaster.Enums;
using Common.Helpers;
using LanguageExt;
using MailKit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Polly;
using System.Globalization;
using System.Net.Sockets;
using static AIERA.AIEmailClient.IoC.Bootstrapper;

namespace AIERA.Desktop.WinForms;

public partial class MainApplicationContext : ApplicationContext
{
    /// <summary>
    /// Makes it possible to invoke methods on the UI thread from other threads, 
    /// since <see cref="MainApplicationContext"/> doesn't have an <see cref="Control.Invoke"/> method."/> or a MainForm to use for invoking.
    /// </summary>
    public static SynchronizationContext? UIContext { get; private set; }

    private readonly IAIEmailServiceFactory _aIEmailServiceFactory;
    private readonly IPublicClientApplication _PublicClientApp;
    private readonly IToastNotification _toastNotification;
    private readonly AcquireTokenConfig _acquireTokenOptions;
    private readonly IAsyncPolicy _retryPolicy;
    private readonly ILogger<MainApplicationContext> _logger;
    private static readonly CancellationTokenSource _appExitCancellationTokenSource = new();
    private CancellationTokenSource? _replyAllCancellationTokenSource;


    /// <summary>
    ///     
    /// </summary>
    /// <remarks>Do not call directly, but use <see cref="FormFactory.CreateMainApplicationContext"/> instead.</remarks>
    /// <param name="aIEmailService"></param>
    /// <param name="PublicClientApp"></param>
    public MainApplicationContext(IAIEmailServiceFactory aIEmailServiceFactory,
                                  IPublicClientApplication PublicClientApp,
                                  IToastNotification toastNotification,
                                  IOptions<AcquireTokenConfig> AcquireTokenConfigOptions,
                                  IAsyncPolicy retryPolicy,
                                  ILogger<MainApplicationContext> logger)
    {
        _aIEmailServiceFactory = aIEmailServiceFactory;
        _PublicClientApp = PublicClientApp;
        _toastNotification = toastNotification;
        _acquireTokenOptions = AcquireTokenConfigOptions.Value;
        _retryPolicy = retryPolicy;
        _logger = logger;
        InitializeComponent();
        UIContext = SynchronizationContext.Current;
    }

    #region Event Handlers

    #region NotifyIcon event handler
    private async void NotifyIcon_MouseClick(object sender, MouseEventArgs e) // TODO: Async void kan stadig give problemer som det er indtil videre.
    {
        try
        {
            //_logger.LogInformation("***TEST***Logger is working. {test}", 1);
            _logger.LogInformation("***TEST***Logger is working. {hostname}", true); // TODO: Fjern.

            if (e.Button is MouseButtons.Left)
                OpenSettingsForm();

            else if (e.Button is MouseButtons.Right)
            {
                await SetReplyAllMenuItemEnabledBool();
            }
        }
        catch (Exception)
        {
            _ = MessageBox.Show("An unknown error has occurred. Please try again or contact an administrator.", $"{Program.ApplicationNameAbbreviation} - Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task SetReplyAllMenuItemEnabledBool()
    {
        IEnumerable<IAccount> accounts = await _PublicClientApp.GetAccountsAsync();

        if (accounts.Any())
        {
            menuItemReplyAll.ToolTipText = _MenuItemReplyAllDefaultText;
            menuItemReplyAll.Enabled = true;
        }
        else
        {
            menuItemReplyAll.ToolTipText = "Add an account to enable this feature.";
            menuItemReplyAll.Enabled = false;
        }
    }
    #endregion

    #region Settings event
    private void OpenSettingsForm() => new FormFactory().GetOpenOrCreateNewSettingsForm().Show();

    private void SettingsItem_MouseClick(object sender, EventArgs e)
    {
        try
        {
            ((ToolStripMenuItem)sender).Enabled = false;
            OpenSettingsForm();
        }
        catch (Exception)
        {
            _ = MessageBox.Show("An unknown error has occurred. Please try again or contact an administrator.", $"{Program.ApplicationNameAbbreviation} - Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw;
            // LOG
        }
        finally
        {
            ((ToolStripMenuItem)sender).Enabled = true;
        }
    }
    #endregion

    #region Add account clicked event
    private void AddAccountItem_Mouse(object sender, EventArgs e)
    {
        try
        {
            ((ToolStripMenuItem)sender).Enabled = false;
            OnAddAccountClicked();
        }
        catch (Exception)
        {
            _ = MessageBox.Show("An unknown error has occurred. Please try again or contact an administrator.", $"{Program.ApplicationNameAbbreviation} - Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw;
            // LOG
        }
        finally
        {
            ((ToolStripMenuItem)sender).Enabled = true;
        }
    }

    private void OnAddAccountClicked()
    {
        using LoginForm loginForm = new FormFactory().GetOpenOrCreateNewLoginForm();
        _ = loginForm.ShowDialog();
    }
    #endregion

    #region Exit Event
    private void ExitMenuItem_MouseClick(object sender, EventArgs e)
    {
        try
        {
            OnApplicationExit(e);
        }
        catch (Exception)
        {
            throw;
            // LOG
        }
    }

    private void OnApplicationExit(EventArgs e)
    {
        _appExitCancellationTokenSource.Cancel();
        Application.Exit();
    }
    #endregion

    #region Cancel reply to all event
    private void CancelReplyAllItem_Mouse(object sender, EventArgs e) => OnCancelReplyAllItemClicked();

    private void OnCancelReplyAllItemClicked()
    {
        try
        {
            menuItemCancelReplyAll.Visible = false;
            _replyAllCancellationTokenSource!.Cancel();

            _replyAllCancellationTokenSource.Dispose();
        }
        catch (Exception)
        {
            throw;
            // LOG
        }
    }
    #endregion

    #region Reply to all event
    private async void ReplyToAllItem_Mouse(object sender, EventArgs e)
    {
        try
        {
            menuItemReplyAll.Enabled = false;
            _replyAllCancellationTokenSource = new();
            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_replyAllCancellationTokenSource.Token, _appExitCancellationTokenSource.Token);
            menuItemCancelReplyAll.Visible = true;
            await ReplyToAllAsync(await _PublicClientApp.GetAccountsAsync(), linkedCts.Token);
        }
        catch (Exception)
        { throw; /*LOG*/ }
        finally
        {
            menuItemCancelReplyAll.Visible = false;
            menuItemReplyAll.Enabled = true;
            RestoreStatusInUIToDefaultText();
        }
    }

    private async Task ReplyToAllAsync(IEnumerable<IAccount> accounts, CancellationToken cancellationToken)
    {
        UpdateUIStatus(menuItemStatus: "'Replying to all' will start soon...",
                       settingsFormLabelStatus: "'Replying to all' will start soon...",
                       notifyIconTextStatus: "'Replying to all' will start soon...");

        UIReplyStatusState uiReplyStatusState = new UIReplyStatusState(accountsToProcessCount: accounts.Count()).UpdateUIReplyStatus();

        foreach (IAccount account in accounts)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            await ReplyToAllAsync(account, uiReplyStatusState, cancellationToken);
            _ = uiReplyStatusState.IncrementAccountsProcessedCount().UpdateUIReplyStatus();
        }

        await Task.Delay(800, cancellationToken); // To make sure it doesn't suddenly look like it stopped at ex. "account: (3/4)" because it only showed "account: (4/4)" for a fraction of a second. // Also gives the OS more time to clean up the disconnected socket(s), before instantiating a new client (https://github.com/jstedfast/MailKit/issues/941).
        RestoreStatusInUIToDefaultText();
    }

    // TODO: Handle more exceptions from AIEmailClient.
    /// <summary>
    /// 
    /// </summary>
    /// <param name="account"></param>
    /// <param name="uiReplyStatusState">A <see cref="UIReplyStatusState"/> should only be provided as an argument when this method is called from a method that iterates over multiple accounts. Otherwise a new one will be created.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task ReplyToAllAsync(IAccount account, UIReplyStatusState? uiReplyStatusState, CancellationToken cancellationToken)
    {
        _ = (uiReplyStatusState ??= new()).ResetEmailAddress().ResetEmailsRepliedCount().ResetUnansweredEmailsCountInt().UpdateUIReplyStatus();
        try
        {
            var authResult = await _retryPolicy.ExecuteAsync(async cancellationToken =>
            {
                return await _PublicClientApp.AcquireTokenSilent(_acquireTokenOptions.Scopes, account)
                                             .ExecuteAsync(cancellationToken);
            }, cancellationToken);

            _ = uiReplyStatusState.SetEmailAddress(AuthenticationResultHelper.TryGetEmailFromAuthenticationResult(authResult, account, out string? emailAddress) ? emailAddress! : string.Empty);

            using var aiEmailService = _aIEmailServiceFactory.Create(authResult);

            var replyFolder = await aiEmailService.GetMessageFolderAsync("AIERA/Reply", createIfNotFound: true, cancellationToken); // TODO: Find folder path in DB
            var repliedFolder = await aiEmailService.GetMessageFolderAsync("AIERA/Replied", createIfNotFound: true, cancellationToken); // TODO: Find folder path in DB

            IList<UniqueId> unansweredEmails = await aiEmailService.GetUnansweredEmailsInEmailFolder(replyFolder, cancellationToken);
            _ = uiReplyStatusState.SetUnansweredEmailsCountInt(unansweredEmails.Count).UpdateUIReplyStatus();

            await foreach (string messageId in aiEmailService.ReplyAllWithAIMessageAsync(replyFolder, repliedFolder, cancellationToken)) // If there is a problem replying to one email, it is assumed none of the emails for that account can be send, therefore exception thrown here will cause the rest of the emails on that account to be skipped.
                _ = uiReplyStatusState.IncrementEmailsRepliedCount().UpdateUIReplyStatus();

            await Task.Delay(800, cancellationToken); // To make sure it doesn't look like it suddenly stopped at ex. "replied: (3/4)", because it only showed "replied: (4/4)" for a fraction of a second. Also gives the OS more time to clean up the disconnected socket(s), before instantiating a new client (https://github.com/jstedfast/MailKit/issues/941).
        }
        catch (TaskCanceledException) {  /*LOG*/ }
        catch (MailKit.Security.AuthenticationException) // LOG
        {
            HandleMailKitAuthenticationException(account, uiReplyStatusState);
        }
        catch (MsalUiRequiredException ex) // LOG
        {
            HandleMsalUiRequiredException(ex, account, uiReplyStatusState);
        }
        catch (MsalClientException)
        {
            HandleMsalClientException(account, uiReplyStatusState); // LOG. https://learn.microsoft.com/en-us/dotnet/api/microsoft.identity.client.msalexception.additionalexceptiondata
        }
        catch (MsalServiceException ex)
        {
            HandleMsalServiceException(ex, account, uiReplyStatusState); // LOG
        }
        catch (HttpRequestException ex) when (ex.InnerException is SocketException { SocketErrorCode: SocketError.HostNotFound }) // Sometimes happens instead of MsalServiceException, if internet is not available.
        {
            HandleHostNotFoundException(account, uiReplyStatusState);
            //throw; 
            // LOG
        }
        catch (FolderNameNotLegalException ex) // LOG
        {
            HandleFolderNameNotLegalException(ex, account, uiReplyStatusState);
        }
        catch (FolderNotFoundException ex) // LOG
        {
            HandleFolderNotFoundException(ex, account, uiReplyStatusState);
        }
        catch (Exception) // LOG
        {
            HandleBaseException(account, uiReplyStatusState);
        }
    }
    #endregion
    #endregion

    #region Update status in UI
    private partial class UIReplyStatusState
    {
        public int AccountsToProcessCount { get; } = 0;
        public UIReplyStatusState(int accountsToProcessCount = 0) => AccountsToProcessCount = accountsToProcessCount;


        public int AccountsProcessedCount { get; private set; } = 0;
        public UIReplyStatusState IncrementAccountsProcessedCount()
        {
            AccountsProcessedCount++;
            return this;
        }


        public int SkippedAccountsCount { get; private set; } = 0;
        public UIReplyStatusState IncrementSkippedAccountsCount()
        {
            SkippedAccountsCount++;
            return this;
        }

        public int EmailsRepliedCount { get; private set; } = 0;
        public UIReplyStatusState IncrementEmailsRepliedCount()
        {
            EmailsRepliedCount++;
            return this;
        }

        public UIReplyStatusState ResetEmailsRepliedCount()
        {
            EmailsRepliedCount = 0;
            return this;
        }


        //public int SkippedEmailsTotalCount { get; private set; } = 0;


        //private int _skippedEmailsCurrentAccount;
        //public int SkippedEmailsCurrentAccount
        //{
        //    get => _skippedEmailsCurrentAccount;
        //    private set
        //    {
        //        _skippedEmailsCurrentAccount = value;
        //        SkippedEmailsTotalCount++;
        //    }
        //}
        //public UIReplyStatusState IncrementSkippedEmailsCurrent()
        //{
        //    SkippedEmailsCurrentAccount++;
        //    return this;
        //}
        //public UIReplyStatusState ResetSkippedEmailsCurrent()
        //{
        //    SkippedEmailsCurrentAccount = 0;
        //    return this;
        //}


        public string UnansweredEmailsCountString { get; private set; } = "?";
        public UIReplyStatusState SetUnansweredEmailsCountInt(int? count)
        {
            UnansweredEmailsCountString = count.HasValue ? count.Value.ToString(CultureInfo.InvariantCulture) : "?";
            return this;
        }
        public UIReplyStatusState ResetUnansweredEmailsCountInt()
        {
            _ = SetUnansweredEmailsCountInt(count: null);
            return this;
        }

        private string _emailAddress = "?";
        public string? EmailAddress { get => _emailAddress; }
        public UIReplyStatusState SetEmailAddress(string? emailAddress)
        {
            _emailAddress = string.IsNullOrWhiteSpace(emailAddress) ? "?" : emailAddress;
            return this;
        }
        public UIReplyStatusState ResetEmailAddress()
        {
            _ = SetEmailAddress(emailAddress: null);
            return this;
        }

        public UIReplyStatusState UpdateUIReplyStatus()
        {
            // Status templates
            string menuItemStatusTemplate = "Replying to emails: Accounts processed: ({0}/{1}), Skipped: {2}, Replied: ({3}/{4}) - {5}";
            string settingsFormLabelStatusTemplate = "'Replying to all' in progress - Account {0} of {1}, Skipped: {2}, Account: '{3}', Emails replied ({4}/{5}) in 'AI Reply Folder'."; // TODO: Find navnet på 'AI Reply Folder' i DB
            string notifyIconStatusTemplate = _notifyIconDefaultText + "\n Account: ({0}/{1}), Skipped: {2}, Replied: ({3}/{4}) - {5}"; // Hvis sidste del af denne string ændres, så skal koden inde i det næste if statement muligvis også ændres.

            // Formating the status templates with the values.
            string menuItemStatusFormated = string.Format(CultureInfo.InvariantCulture, menuItemStatusTemplate, AccountsProcessedCount, AccountsToProcessCount, SkippedAccountsCount, EmailsRepliedCount, UnansweredEmailsCountString, EmailAddress);
            string settingsFormLabelStatusFormated = string.Format(CultureInfo.InvariantCulture, settingsFormLabelStatusTemplate, AccountsProcessedCount, AccountsToProcessCount, SkippedAccountsCount, EmailAddress, EmailsRepliedCount, UnansweredEmailsCountString);
            string notifyIconTextStatusFormated = string.Format(CultureInfo.InvariantCulture, notifyIconStatusTemplate, AccountsProcessedCount, AccountsToProcessCount, SkippedAccountsCount, EmailsRepliedCount, UnansweredEmailsCountString, EmailAddress);

            // notifyIcon.Text has a limit of less than 128 characters.
            if (notifyIconTextStatusFormated.Length >= 128)
            {
                // Remove email address from the status templates, if the string is too long, since it is very unlikely to be anything else that makes the string too long for notifyIcon.Text.
                notifyIconTextStatusFormated = string.Format(CultureInfo.InvariantCulture, notifyIconStatusTemplate,
                                                             AccountsProcessedCount,
                                                             AccountsToProcessCount,
                                                             SkippedAccountsCount,
                                                             EmailsRepliedCount,
                                                             UnansweredEmailsCountString,
                                                             string.Empty)
                                                      [..(notifyIconTextStatusFormated.Length - 4)]; // Removes the last 4 characters, which contains the empty space and '-' for where the email address would have been.
            }

            UpdateUIStatus(menuItemStatus: menuItemStatusFormated,
                           settingsFormLabelStatus: settingsFormLabelStatusFormated,
                           notifyIconTextStatus: notifyIconTextStatusFormated);

            return this;
        }
    }

    private static void UpdateUIStatus(string menuItemStatus,
                                      string settingsFormLabelStatus,
                                      string notifyIconTextStatus)
    {
        // Update MenuItemReplyAll with shorter text
        UpdateMenuItemReplyAllText(menuItemStatus);

        // Update LblStatus in SettingsForm with more detailed text
        UpdateSettingsFormLabel(settingsFormLabelStatus);

        // Update notifyIcon.Text with shorter text
        UpdateNotifyIconTextStatus(notifyIconTextStatus);
    }

    public static void UpdateMenuItemReplyAllText(string text) => menuItemReplyAll.Text = text;

    private static void UpdateSettingsFormLabel(string text)
    {
        if (new FormFactory().GetOpenSettingsForm() is { IsDisposed: false } settingsForm)
        {
            settingsForm.UpdateLblStatus(text);
        }
    }

    private static void UpdateNotifyIconTextStatus(string notifyIconTextStatus) => notifyIcon.Text = notifyIconTextStatus;

    private void RestoreStatusInUIToDefaultText()
    {
        if (new FormFactory().GetOpenSettingsForm() is { IsDisposed: false } settingsForm)
            settingsForm.RestoreLblStatusToDefaultText();

        UpdateMenuItemReplyAllText(_MenuItemReplyAllDefaultText);
        UpdateNotifyIconTextStatus(_notifyIconDefaultText);
    }
    #endregion

    #region Exception handleres
    #region FolderNotFoundException handler
    private void HandleFolderNotFoundException(FolderNotFoundException ex, IAccount account, UIReplyStatusState uiStatusState) => ShowFolderNotFoundExceptionToast(account, ex, uiStatusState);

    private void ShowFolderNotFoundExceptionToast(IAccount account, FolderNotFoundException ex, UIReplyStatusState uiReplyStatusState)
    {
        _ = uiReplyStatusState.IncrementSkippedAccountsCount().UpdateUIReplyStatus();

        _toastNotification.ShowMsalToastNotification(headerId: HeaderId.FolderErrors,
                           headerTitle: $"{Program.ApplicationNameAbbreviation} - Folder not found",
                           group: ToastGroup.FolderNotFoundError, // TODO: Kan bruges til at fjerne notificationen med når problemet er løst.
                           account: account, // TODO: Kan bruges til at fjerne notificationen med når problemet er løst.
                           buttons: [_toastNotification.CreateToastAccountSettingsButton(),],
                           firstLine: $"Directory not found.",
                           secondLine: $"The folder '{ex.FolderName}' was not found in the email account '{account.Username}'.");
    }
    #endregion

    #region NotLegalFolderNameException handler
    private void HandleFolderNameNotLegalException(FolderNameNotLegalException ex, IAccount account, UIReplyStatusState uiReplyStatusState)
    {
        _ = uiReplyStatusState.IncrementSkippedAccountsCount().UpdateUIReplyStatus();

        _toastNotification.ShowMsalToastNotification(headerId: HeaderId.FolderErrors,
                           headerTitle: $"{Program.ApplicationNameAbbreviation} - Folder name not legal",
                           group: ToastGroup.FolderNameNotLegal, // TODO: Kan bruges til at fjerne notificationen med når problemet er løst.
                           account: account, // TODO: Kan bruges til at fjerne notificationen med når problemet er løst.
                           buttons: [_toastNotification.CreateToastAccountSettingsButton(),],
                           firstLine: $"Folder name '{ex.FolderPath}' is not a legal.",
                           secondLine: $"Go to settings and change the folder name for the email account '{account.Username}'.");
    }
    #endregion

    #region Base exception handler
    private void HandleBaseException(IAccount account, UIReplyStatusState uiStatusState) => ShowBaseExceptionToast(account, uiStatusState);

    private void ShowBaseExceptionToast(IAccount account, UIReplyStatusState uiStatusState)
    {
        _ = uiStatusState.IncrementSkippedAccountsCount().UpdateUIReplyStatus();

        _toastNotification.ShowMsalToastNotification(headerId: HeaderId.UnknownError,
                           headerTitle: $"{Program.ApplicationNameAbbreviation} - Unexpected error",
                           group: ToastGroup.FolderNotFoundError, // TODO: Kan bruges til at fjerne notificationen med når problemet er løst.
                           account: account, // TODO: Kan bruges til at fjerne notificationen med når problemet er løst.
                           buttons: [_toastNotification.CreateToastAccountSettingsButton(),],
                           firstLine: $"Unexpected error while replying to emails using account: '{account.Username}'",
                           secondLine: $"Please contact an admin.");
    }
    #endregion

    #region MailKit exception handlers
    private void HandleMailKitAuthenticationException(IAccount account, UIReplyStatusState uiStatusState) => ShowMailKitAuthenticationExceptionToast(account, uiStatusState);

    private void ShowMailKitAuthenticationExceptionToast(IAccount account, UIReplyStatusState uiStatusState)
    {
        _ = uiStatusState.IncrementSkippedAccountsCount().UpdateUIReplyStatus();

        _toastNotification.ShowMsalToastNotification(headerId: HeaderId.AuthErrors,
                           headerTitle: $"{Program.ApplicationNameAbbreviation} - Unable to authenticate with Email server",
                           group: ToastGroup.AuthenticationError, // TODO: Kan bruges til at fjerne notificationen med når problemet er løst.
                           account: account, // TODO: Kan bruges til at fjerne notificationen med når problemet er løst.
                           buttons: [_toastNotification.CreateToastAuthButton("Sign-in...", account.Username, claims: null, account.HomeAccountId.Identifier),
                                     _toastNotification.CreateToastAccountSettingsButton(),],
                           firstLine: "Unable to authenticate your account.",
                           secondLine: "Please verify your login credentials or server settings.",
                           thirdLine: "Click here to adjust your account settings.");
    }
    #endregion

    #region MsalException handlers
    private void HandleMsalUiRequiredException(MsalUiRequiredException ex, IAccount account, UIReplyStatusState uiStatusState)
    {
        _ = uiStatusState.IncrementSkippedAccountsCount().UpdateUIReplyStatus();

        string signInbuttonText = ex.Classification switch
        {
            UiRequiredExceptionClassification.BasicAction => "Sign-in...",
            UiRequiredExceptionClassification.AdditionalAction => "Sign-in...",
            UiRequiredExceptionClassification.MessageOnly => "Sign-in...",
            UiRequiredExceptionClassification.ConsentRequired => "Give consent...",
            UiRequiredExceptionClassification.UserPasswordExpired => "Reset Password...",
            // UiRequiredExceptionClassification.PromptNeverFailed => "",
            UiRequiredExceptionClassification.AcquireTokenSilentFailed => "Sign-in...",
            UiRequiredExceptionClassification.None => "Sign-in...",
            _ => "Sign-in...",
        };

        string firstToastLine = $"Problems with authentication on account '{account.Username}'";
        string? secondToastLine = ExceptionErrorMessagesMicrosoft.GetExceptionErrorMessage(ex, account);

        ShowMsalExceptionToast(account,
                               signInbuttonText,
                               firstToastLine,
                               secondToastLine,
                               claims: ex.Claims);
    }

    private void HandleMsalClientException(IAccount account, UIReplyStatusState uiStatusState)
    {
        _ = uiStatusState.IncrementSkippedAccountsCount().UpdateUIReplyStatus();

        ShowMsalExceptionToast(account,
                               buttonText: "Sign-in...",
                               firstLine: $"Failed to authenticate '{account.Username}'.",
                               secondLine: "An unexpected error on the client side has occurred while trying to reply to emails. Please try to sign in again or try again latere.");
    }

    private void HandleMsalServiceException(MsalServiceException ex, IAccount account, UIReplyStatusState uiStatusState)
    {
        _ = uiStatusState.IncrementSkippedAccountsCount().UpdateUIReplyStatus();

        ShowMsalExceptionToast(account,
                               buttonText: "Sign-in...",
                               firstLine: $"Failed to authenticate '{account.Username}'.",
                               secondLine: ExceptionErrorMessagesMicrosoft.GetExceptionErrorMessage(ex, account),
                               claims: ex.Claims);
    }

    private void ShowMsalExceptionToast(IAccount account,
                                                              string buttonText,
                                                              string firstLine,
                                                              string? secondLine = null,
                                                              string? thirdLine = null,
                                               string claims = "")
    {
        _toastNotification.ShowMsalToastNotification(headerId: HeaderId.AuthErrors,
                           headerTitle: $"{Program.ApplicationNameAbbreviation} - Authentication Issues",
                           group: ToastGroup.FolderNotFoundError, // TODO: Kan bruges til at fjerne notificationen med når problemet er løst.  
                           account: account, // TODO: Kan bruges til at fjerne notificationen med når problemet er løst.
                           buttons: [ _toastNotification.CreateToastAuthButton(buttonText,
                                                            loginHint: account.Username,
                                                            claims,
                                                            account.HomeAccountId.Identifier),
                                      _toastNotification.CreateToastAccountSettingsButton(),],
                           firstLine: firstLine,
                           secondLine: secondLine,
                           thirdLine: thirdLine);
    }
    #endregion

    #region HttpRequest/SocketException Host not found handler.
    private void HandleHostNotFoundException(IAccount account, UIReplyStatusState uiStatusState)
    {
        _ = uiStatusState.IncrementSkippedAccountsCount().UpdateUIReplyStatus();

        string errorMessage;

        if (InternetAvailability.IsInternetAvailable() is false)
            errorMessage = "Check internet connection and try again.";
        else
            errorMessage = "Possible cause: no Internet connection.";

        ShowHostNotFoundExceptionToast(account, errorMessage);
    }

    private void ShowHostNotFoundExceptionToast(IAccount account, string errorMessage)
    {
        _toastNotification.ShowMsalToastNotification(headerId: HeaderId.SocketError,
                           headerTitle: $"{Program.ApplicationNameAbbreviation} - Possible connection error",
                           group: ToastGroup.ConnectionError, // TODO: Kan bruges til at fjerne notificationen med når problemet er løst.
                           account: account, // TODO: Kan bruges til at fjerne notificationen med når problemet er løst.
                           buttons: [_toastNotification.CreateToastAccountSettingsButton(),],
                           firstLine: $"Failed to authenticate account '{account.Username}'.",
                           secondLine: $"Error occurred while attempting to reply all emails.",
                           thirdLine: errorMessage);
    }
    #endregion
    #endregion
}