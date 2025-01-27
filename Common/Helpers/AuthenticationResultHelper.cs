using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;

namespace Common.Helpers;

/// <summary>
/// Tries to get the email address from the 
/// - 'email' claim on <see cref="AuthenticationResult.IdToken"/>, 
/// - then the 'preferred_username' on <see cref="AuthenticationResult.IdToken"/>, 
/// - then the <see cref="AuthenticationResult.Account.Username"/>
/// - and finally the <see cref="IAccount.Username"/> if all else fails.
/// </summary>
public class AuthenticationResultHelper
{
    public static bool TryGetEmailFromAuthenticationResult(AuthenticationResult? authResult, IAccount? account, out string? emailAddress)
    {
        emailAddress = null;

        if (authResult is null || string.IsNullOrEmpty(authResult.IdToken) || account is null)
            return false;

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(authResult.IdToken);

        // Extract the email claim
        var email = token.Claims.FirstOrDefault(claim => claim.Type.Equals("email", StringComparison.OrdinalIgnoreCase))?.Value;

        if (string.IsNullOrEmpty(email))
            // Some tenants may use "preferred_username" instead
            email = token.Claims.FirstOrDefault(claim => claim.Type.Equals("preferred_username", StringComparison.OrdinalIgnoreCase))?.Value;

        if (string.IsNullOrEmpty(email))
            email = authResult.Account.Username;

        if (string.IsNullOrEmpty(email) && account is not null)
            email = account.Username; // TODO: Remove unnecessary characters from "account.Username", that are sometimes included as part of the email address.

        emailAddress = email;

        return !string.IsNullOrEmpty(email);
    }

}
