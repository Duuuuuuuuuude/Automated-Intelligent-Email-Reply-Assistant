using AIERA.Desktop.WinForms.Authentication.Accounts.Microsoft;
using AIERA.Desktop.WinForms.Authentication.Models.Result_Pattern;
using AIERA.Desktop.WinForms.IoC.Factories;
using AIERA.Desktop.WinForms.Models.ViewModels;
using Common.Models;
using System.ComponentModel;

namespace AIERA.Desktop.WinForms;

public partial class LoginForm : Form
{
    private readonly IMicrosoftAuthentication _microsoftAuthentication;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Result<MicrosoftAccountViewModel, MicrosoftAuthenticationError>? MicrosoftAuthenticationResult { get; private set; }

    private readonly CancellationTokenSource _loginFormClosingCancellationTokenSource = new();

    /// <summary>
    ///  Shows the login form where a user can select the account type to login with.
    /// </summary>
    /// <remarks>Do not call directly, but use <see cref="FormFactory.GetOpenOrCreateNewLoginForm"/> instead.</remarks>
    /// <param name="microsoftAuthentication">Service used when a user wants to sign in using a Microsoft account.</param>
    public LoginForm(IMicrosoftAuthentication microsoftAuthentication)
    {
        _microsoftAuthentication = microsoftAuthentication;
        InitializeComponent();
    }

    #region Form load event handler
    private void LoginForm_Load(object sender, EventArgs e) => OnFormLoad();
    private void OnFormLoad() => Text += Program.ApplicationNameFull;
    #endregion

    #region Microsoft login clicked event handler
    private async void BtnLoginMicrosoft_Click(object sender, EventArgs e)
    {
        try
        {
            Cursor = Cursors.WaitCursor;
            Enabled = false;

            await LogInMicrosoftAsync();
        }
        catch (Exception)
        {
            _ = MessageBox.Show("An unknown error has occurred during login. Please try again or contact admin.",
                            $"{Program.ApplicationNameAbbreviation} - Unexpected error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

            DialogResult = DialogResult.Abort;
            //throw;  
            // LOG
        }
        finally
        {
            Enabled = true;
            Cursor = Cursors.Default;
        }
    }

    private async Task LogInMicrosoftAsync()
    {
        MicrosoftAuthenticationResult = await _microsoftAuthentication.SignInAsync(hWnd: Handle, account: null, claims: null, _loginFormClosingCancellationTokenSource.Token);
    }
    #endregion

    #region Form closing event handler
    private void LoginForm_FormClosing(object sender, FormClosingEventArgs e) => OnFormClosing();
    private void OnFormClosing()
    {
        _loginFormClosingCancellationTokenSource.Cancel();
    }
    #endregion
}
