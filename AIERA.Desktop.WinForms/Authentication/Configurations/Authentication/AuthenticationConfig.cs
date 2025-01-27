using AIERA.Desktop.WinForms.Authentication.Configurations.AcquireToken;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;


namespace AIERA.Desktop.WinForms.Authentication.Configurations.Authentication;

public class AuthenticationConfig
{
    public const string ConfigurationSectionName = $"MicrosoftEntra:{nameof(AuthenticationConfig)}";

    [ValidateObjectMembers]
    [Required]
    public PublicClientApplicationOptions PublicClientApplicationOptions { get; private set; } = new();

    [ValidateObjectMembers]
    [Required]
    public AcquireTokenConfig AcquireTokenConfig { get; private set; } = new();

    [ValidateObjectMembers]
    [Required]
    public LogOptions LogOptions { get; private set; } = new();
}
public class LogOptions
{
    /// <summary>
    /// Boolean used to enable/disable logging of
    /// Personally Identifiable Information (PII).
    /// PII logs are never written to default outputs like Console, Logcat or NSLog
    /// Default is set to <see langword="false"/>, which ensures that your application is compliant with GDPR.
    /// You can set it to <see langword="true"/> for advanced debugging requiring PII
    /// If both WithLogging apis are set, this one will override the other
    /// </summary>
    [Required]
    public bool EnablePiiLogging { get; private set; }
}
