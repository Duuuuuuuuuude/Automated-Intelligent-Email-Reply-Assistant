using Common.Helpers;
using Microsoft.Identity.Client;

namespace AIERA.Desktop.WinForms.Models.ViewModels;

public record MicrosoftAccountViewModel(IAccount Account, AuthenticationResult? AuthResult, string? Claims)
{
    /// <summary>
    /// The login hint, which is the same us the <see cref="MicrosoftAccountViewModel.Username"/>.
    /// </summary>
    /// <remarks>If not found, an empty string is returned</remarks>
    public string LoginHint
    {
        get
        {
            {
                return AuthenticationResultHelper.TryGetEmailFromAuthenticationResult(AuthResult, Account, out string? emailAddress)
                    ? emailAddress! : string.Empty;
            }
        }
    }

    /// <summary>
    /// Gets the login hint, which is typically the user's email or username. 
    /// The value is derived from the authentication result or account information.
    /// </summary>
    /// <remarks>If not found, an empty string is returned</remarks>
    public string Username
    {
        get
        {
            return LoginHint;
        }
    }
}
