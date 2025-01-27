using AIERA.Desktop.WinForms.Authentication.Configurations.AcquireToken;
using AIERA.Desktop.WinForms.RegularExpression;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace AIERA.Desktop.WinForms.Authentication.Configurations.Authentication;

public partial class ValidateAuthenticationConfig : IValidateOptions<AuthenticationConfig>
{
    private readonly List<string> _validationResults = [];

    public ValidateOptionsResult Validate(string? name, AuthenticationConfig authenticationConfig)
    {
        if (authenticationConfig is null)
            return ValidateOptionsResult.Fail("'AuthenticationConfig' cannot be null.");

        // Validate Properties on 'PublicClientAppOptions'.
        ValidatePublicClientApplicationOptions(authenticationConfig.PublicClientApplicationOptions);
        ValidateAcquireTokenOptions(authenticationConfig.AcquireTokenConfig);

        // Validate other properties 
        var validationContext = new ValidationContext(authenticationConfig);
        var dataAnnotationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(authenticationConfig, validationContext, dataAnnotationResults, validateAllProperties: true))
            _validationResults.AddRange(dataAnnotationResults.Select(validationResult => validationResult.ErrorMessage)!);

        return _validationResults.Count is not 0 ? ValidateOptionsResult.Fail(string.Join(";\n", _validationResults))
                                                 : ValidateOptionsResult.Success;
    }

    private void ValidatePublicClientApplicationOptions(PublicClientApplicationOptions publicClientAppOptions)
    {
        if (publicClientAppOptions is null)
            _validationResults.Add("'PublicClientApplicationOptions' cannot be null.");
        else
        {
            ValidateClientId(publicClientAppOptions.ClientId);
            ValidateTenantId(publicClientAppOptions.TenantId);
            ValidateInstance(publicClientAppOptions.Instance);
            ValidateAtLeastOneHasValue(publicClientAppOptions.TenantId, publicClientAppOptions.AadAuthorityAudience);
            ValidateAtLeastOneHasValue(publicClientAppOptions.Instance, publicClientAppOptions.AzureCloudInstance);
        }
    }

    private void ValidateAcquireTokenOptions(AcquireTokenConfig acquireTokenConfig)
    {
        if (acquireTokenConfig is null)
            _validationResults.Add("PublicClientAppOptions cannot be null.");
        else
        {
            if (!acquireTokenConfig.Scopes.Any())
                _validationResults.Add($"'AcquireTokenOptions.Scopes' must contain at least one scope.");
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    private void ValidateClientId(string ClientId)
    {
        if (string.IsNullOrWhiteSpace(ClientId) ||
        !Guid.TryParse(ClientId, out _))
        {
            _validationResults.Add($"ClientId must be a valid GUID. ClientId is currently set to: '{ClientId}'.");
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="tenantId"></param>
    private void ValidateTenantId(string tenantId)
    {
        // 'TenantId' is allowed to not be set, as long as 'AadAuthorityAudience' is not set to None.
        if (string.IsNullOrWhiteSpace(tenantId))
            return;

        // Check if valid GUID.
        if (Guid.TryParse(tenantId, out _))
            return;

        List<string> audience = ["organizations", "consumers", "common"];
        // Check if valid audience.
        if (audience.Contains(tenantId, StringComparer.OrdinalIgnoreCase))
            return;

        // Check if valid base url.
        if (RegexPatterns.BaseUrlRegex().IsMatch(tenantId))
            return;

        _validationResults.Add($"TenantId must be a valid GUID (tenant id), " +
                               $"one of these meta-tenant (organizations, consumers or common) " +
                               $"or a valid domain matching the following base url regex: {RegexPatterns.BaseUrlRegex()} " +
                               $"TenantId is currently set to: '{tenantId}'.");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="instance"></param>
    private void ValidateInstance(string instance)
    {
        // 'Instance' is allowed to not be set, as long as 'AzureCloudInstance' is not set to None.
        if (string.IsNullOrWhiteSpace(instance))
            return;

        // Check if valid base url.
        if (RegexPatterns.BaseUrlRegex().IsMatch(instance))
            return;

        _validationResults.Add($"'Instance' must be a valid domain " +
                               $"matching the following base url regex: {RegexPatterns.BaseUrlRegex()} " +
                               $"'Instance' is currently set to: '{instance}'.");
    }

    private void ValidateAtLeastOneHasValue(string tenantId, AadAuthorityAudience aadAuthorityAudience)
    {
        if (!string.IsNullOrWhiteSpace(tenantId))
            return;

        if (aadAuthorityAudience is AadAuthorityAudience.None)
            _validationResults.Add("Either 'TenantId' or 'AadAuthorityAudience' must have a value.");
    }

    private void ValidateAtLeastOneHasValue(string instance, AzureCloudInstance azureCloudInstance)
    {
        if (!string.IsNullOrWhiteSpace(instance))
            return;

        if (azureCloudInstance is AzureCloudInstance.None)
            _validationResults.Add("Either 'Instance' or 'AzureCloudInstance' must have a value.");
    }
}