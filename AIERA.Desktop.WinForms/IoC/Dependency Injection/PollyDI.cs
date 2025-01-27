using AIERA.Desktop.WinForms.Authentication.Polly;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace AIERA.Desktop.WinForms.IoC.Dependency_Injection;
public static class PollyDI
{
    public static IServiceCollection AddPolly(this IServiceCollection services)
    {
        // Add Polly retry logic
        _ = services.AddSingleton<IAsyncPolicy>(PollyRetryLogic.GetMsalRetryPolicy());

        return services;
    }
}
