using AIERA.Desktop.WinForms.IoC.Factories;
using AIERA.Desktop.WinForms.Toaster;
using AIERA.Desktop.WinForms.Toaster.ToastActionHandlers;
using AIERA.Desktop.WinForms.Toaster.ToastActionHandlers.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace AIERA.Desktop.WinForms.Dependency_Injection;
public static class ToasterDI
{
    public static IServiceCollection AddToastNotification(this IServiceCollection services)
    {
        // Inject 'Toast action handlers
        _ = services.AddSingleton<IToastActionHandlerFactory, ToastActionHandlerFactory>();

        //services.AddTransient<OpenSettingsHandler>();
        _ = services.AddTransient<AcquireTokenInteractivelyHandler>();
        _ = services.AddTransient<OpenAccountSettingsHandler>();

        // Inject 'Toast'
        _ = services.AddSingleton<ToastActionRegistry>();
        _ = services.AddTransient<ToastOperations>();

        // Inject 'Toast Notification'
        _ = services.AddSingleton<IToastNotification, ToastNotification>();

        return services;
    }
}
