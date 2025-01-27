using AIERA.Desktop.WinForms.Toaster;
using Microsoft.Extensions.DependencyInjection;

namespace AIERA.Desktop.WinForms.IoC.Factories;

public interface IServiceFactoryDesktop
{
    ToastOperations CreateToastOperations();
}


public class ServiceFactoryDesktop : IServiceFactoryDesktop
{
    private static IServiceFactoryDesktop? _provider;

    public static void SetProvider(IServiceFactoryDesktop provider) => _provider = provider;


    public ToastOperations CreateToastOperations() => _provider!.CreateToastOperations();
}


public class ServiceFactoryDesktopImplProduction(IServiceProvider serviceProvider) : IServiceFactoryDesktop
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;


    public ToastOperations CreateToastOperations() => _serviceProvider.GetRequiredService<ToastOperations>();
}