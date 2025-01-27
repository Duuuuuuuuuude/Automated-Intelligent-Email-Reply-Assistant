using AIERA.Desktop.WinForms.Authentication.Configurations.Authentication;
using System.ComponentModel.DataAnnotations;

namespace AIERA.Desktop.WinForms.Authentication.Configurations.AcquireToken;

/// <summary>
/// Configuration for acquiring a token.
/// </summary>
public class AcquireTokenConfig
{
    public const string ConfigurationSectionName = $"MicrosoftEntra:{nameof(AuthenticationConfig)}:{nameof(AcquireTokenConfig)}";

    [Required(AllowEmptyStrings = false)]
    public IEnumerable<string> Scopes { get; private set; } = [];
}