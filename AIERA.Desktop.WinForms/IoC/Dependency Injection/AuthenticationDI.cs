using AIERA.Desktop.WinForms.Authentication.Accounts.Microsoft;
using Microsoft.Extensions.DependencyInjection;

namespace AIERA.Desktop.WinForms.IoC.Dependency_Injection;


public static class AuthenticationDI
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services)
    {
        // Inject Microsoft authentication
        _ = services.AddTransient<IMicrosoftAuthentication, MicrosoftAuthentication>();

        return services;
    }
}
