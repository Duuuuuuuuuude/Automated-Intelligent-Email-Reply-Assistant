using AIERA.AIEmailClient.IoC;
using AIERA.Desktop.WinForms.Dependency_Injection;
using AIERA.Desktop.WinForms.IoC.Dependency_Injection;
using AIERA.Desktop.WinForms.IoC.Factories;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Extensions.Hosting;
using System.Diagnostics;

namespace AIERA.Desktop.WinForms;

public static class Program
{
    public static string ApplicationNameFull { get; } = "Automated Intelligent Email Reply Assistant";
    public static string ApplicationNameAbbreviation { get; } = "AIERA";

    [STAThread]
    static void Main()
    {
        Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
        Log.Logger = CreateReloadableLogger();
        Log.Information($"Starting up {ApplicationNameAbbreviation} Desktop...");

        IHost host = null!;
        try
        {
            host = CreateHostApplication();
            CompositionRootManager.CompositionRoot(host);

            ApplicationConfiguration.Initialize();

            new ServiceFactoryDesktop().CreateToastOperations().OnToastActivated();

            Application.Run(new FormFactory().CreateMainApplicationContext());
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An unhandled exception occurred.");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static ReloadableLogger CreateReloadableLogger()
    {
        return new LoggerConfiguration().Enrich.WithEnvironmentName()
                                        .WriteTo.Debug(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} (Env: {EnvironmentName}{NewLine}{Exception}")
                                        .MinimumLevel.Verbose()
                                        .CreateBootstrapLogger();
    }

    private static IHost CreateHostApplication()
    {
        Log.Information("Creating host");
#if DEBUG
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
#endif

        HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder();

        // Add Options
        hostBuilder.Services.AddConfigurations(hostBuilder.Configuration);

        // Add Services
        hostBuilder.Services.AddLogger(hostBuilder.Configuration);
        hostBuilder.Services.AddAuthentication();
        hostBuilder.Services.AddPublicClientApplication();
        hostBuilder.Services.AddToastNotification();
        hostBuilder.Services.AddPolly();
        hostBuilder.Services.AddAIEmailService(hostBuilder.Configuration);

        var host = hostBuilder.Build();

        return host;
    }
}