using AIERA.Desktop.WinForms.Authentication.Configurations;
using AIERA.Desktop.WinForms.Authentication.Configurations.AcquireToken;
using AIERA.Desktop.WinForms.Authentication.Configurations.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AIERA.Desktop.WinForms.Dependency_Injection;
public static class OptionsDI
{
    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration config)
    {
        // Inject 'PublicClientApplicationOptions'
        _ = services.AddOptions<AuthenticationConfig>()
                .Bind(config.GetSection(AuthenticationConfig.ConfigurationSectionName),
                                        options => { options.BindNonPublicProperties = true; })
                .ValidateOnStart();
        _ = services.AddSingleton<IValidateOptions<AuthenticationConfig>, ValidateAuthenticationConfig>();

        // Inject 'AcquireTokenOptions'
        _ = services.AddOptions<AcquireTokenConfig>()
                .Bind(config.GetSection(AcquireTokenConfig.ConfigurationSectionName),
                                        options => { options.BindNonPublicProperties = true; })
                .ValidateOnStart();
        _ = services.AddSingleton<IValidateOptions<AcquireTokenConfig>, ValidateAcquireTokenConfig>();

        return services;
    }
}