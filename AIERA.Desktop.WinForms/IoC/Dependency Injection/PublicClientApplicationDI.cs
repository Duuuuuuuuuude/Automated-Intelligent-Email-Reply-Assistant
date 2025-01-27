using AIERA.Desktop.WinForms.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;

namespace AIERA.Desktop.WinForms.Dependency_Injection;

public static class PublicClientApplicationDI
{
    public static IServiceCollection AddPublicClientApplication(this IServiceCollection services)
    {
        // Inject 'PublicClientApplication'
        _ = services.AddSingleton<IPublicClientApplication>(serviceProvider =>
        {
            return ActivatorUtilities.CreateInstance<PublicClientApplicationSetup>(serviceProvider).CreatePublicClientApplication();
        });

        return services;
    }
}
