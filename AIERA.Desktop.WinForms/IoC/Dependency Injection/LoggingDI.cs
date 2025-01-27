using AIERA.Desktop.WinForms.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.LoggingExtensions;
using Serilog;

namespace AIERA.Desktop.WinForms.IoC.Dependency_Injection;
public static class LoggingDI
{
    public static IServiceCollection AddLogger(this IServiceCollection services, IConfiguration config)
    {
        // Inject Serilog
        _ = services.AddSerilog((services, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(config));

        // Inject identity logger for MSAL
        _ = services.AddSingleton(sp => new IdentityLoggerAdapter(sp.GetRequiredService<ILogger<PublicClientApplicationSetup>>()));
        return services;
    }
}
