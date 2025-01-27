using Microsoft.Extensions.Hosting;
using Serilog;

namespace AIERA.Desktop.WinForms.IoC.Factories;


public class CompositionRootManager
{
    public static void CompositionRoot(IHost host)
    {
        Log.Information("Composition Root");

        // container
        var serviceProvider = host.Services;

        // Composition roots
        CompositionRootForms();
        CompositionRootDesktopServices();
        CompositionRootToastActionHandler();

        void CompositionRootForms()
        {
            // form factory
            var formFactory = new FormFactoryImplProduction(serviceProvider);
            FormFactory.SetProvider(formFactory);
        }

        void CompositionRootDesktopServices()
        {
            // Service factory
            var formFactory = new ServiceFactoryDesktopImplProduction(serviceProvider);
            ServiceFactoryDesktop.SetProvider(formFactory);
        }

        void CompositionRootToastActionHandler()
        {
            // Toast factory
            var formFactory = new ToastActionHandlerFactoryImplProduction(serviceProvider);
            ToastActionHandlerFactory.SetProvider(formFactory);
        }
    }
}