using AIERA.Desktop.WinForms.Authentication.Configurations.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Broker;
using Microsoft.Identity.Client.Extensions.Msal;
using Microsoft.IdentityModel.LoggingExtensions;
using System.Diagnostics;
using System.Reflection;

namespace AIERA.Desktop.WinForms.Authentication;
public class PublicClientApplicationSetup
{
    private readonly IdentityLoggerAdapter _msalIdentityLogger;
    private readonly AuthenticationConfig _authenticationConfig;

    public PublicClientApplicationSetup(IdentityLoggerAdapter msalIdentityLogger, IOptions<AuthenticationConfig> _publicClientApplicationOptions)
    {
        _msalIdentityLogger = msalIdentityLogger;
        _authenticationConfig = _publicClientApplicationOptions.Value;
    }
    public IPublicClientApplication CreatePublicClientApplication()
    {
        PublicClientApplicationBuilder PublicClientApplicationBuilder = PublicClientApplicationBuilder
                        .CreateWithApplicationOptions(_authenticationConfig.PublicClientApplicationOptions);

        _ = PublicClientApplicationBuilder.WithDefaultRedirectUri()
                                          .WithBroker(GetBrokerOptions())
                                          //.WithDebugLoggingCallback(LogLevel.Verbose, true, true)
                                          .WithLogging(identityLogger: _msalIdentityLogger,
                                                       enablePiiLogging: _authenticationConfig.LogOptions
                                                                                              .EnablePiiLogging)
                                          .WithHttpClientFactory(httpClientFactory: null,
                                                                 retryOnceOn5xx: false); // Disables MSAL's internal simple retry policy. Polly is used instead.

        var PublicClientApp = PublicClientApplicationBuilder.Build();

        AddUserTokenCaching(PublicClientApp);

        return PublicClientApp;
    }

    private static BrokerOptions GetBrokerOptions()
    {
        return new BrokerOptions(BrokerOptions.OperatingSystems.Windows)
        { Title = Program.ApplicationNameFull };
    }

    // TODO: Sikre at cache bliver slettet efter uninstall af programmet.
    private static void AddUserTokenCaching(IPublicClientApplication PublicClientApp)
    {
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string assemblyName = Assembly.GetCallingAssembly().GetName().Name!;
        string cacheDirectory = Path.Combine(localAppData, assemblyName, ".cache"); // App is sandboxed in Debug mode: C:\Users\<User>\AppData\Local\Packages\<AppIdentifier>\LocalCache\Local\AIERA.Desktop\.cache\msalcache.bin

        var storageCreationProperties = new StorageCreationPropertiesBuilder(cacheFileName: "msalcache.bin", cacheDirectory).Build();

        var msalCacheHelper = MsalCacheHelper.CreateAsync(storageCreationProperties,
                                                          new TraceSource($"{Program.ApplicationNameAbbreviation}.MSAL.CacheTrace", SourceLevels.All)).GetAwaiter().GetResult();

        msalCacheHelper.RegisterCache(PublicClientApp.UserTokenCache);
    }
}
