using AIERA.Desktop.WinForms.Toaster.ToastActionHandlers.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace AIERA.Desktop.WinForms.IoC.Factories;


public interface IToastActionHandlerFactory
{
    AcquireTokenInteractivelyHandler CreateAcquireTokenInteractivelyHandler();
    OpenAccountSettingsHandler CreateOpenAccountSettingsHandler();
}

public class ToastActionHandlerFactory : IToastActionHandlerFactory
{
    private static IToastActionHandlerFactory? _provider;

    public static void SetProvider(IToastActionHandlerFactory provider) => _provider = provider;


    public AcquireTokenInteractivelyHandler CreateAcquireTokenInteractivelyHandler() => _provider!.CreateAcquireTokenInteractivelyHandler();
    public OpenAccountSettingsHandler CreateOpenAccountSettingsHandler() => _provider!.CreateOpenAccountSettingsHandler();
}


public class ToastActionHandlerFactoryImplProduction(IServiceProvider serviceProvider) : IToastActionHandlerFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;


    public AcquireTokenInteractivelyHandler CreateAcquireTokenInteractivelyHandler() => _serviceProvider.GetRequiredService<AcquireTokenInteractivelyHandler>();
    public OpenAccountSettingsHandler CreateOpenAccountSettingsHandler() => _serviceProvider.GetRequiredService<OpenAccountSettingsHandler>();
}