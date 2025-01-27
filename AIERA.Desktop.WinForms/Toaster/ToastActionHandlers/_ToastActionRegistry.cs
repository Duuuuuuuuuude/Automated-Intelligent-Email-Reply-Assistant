using AIERA.Desktop.WinForms.IoC.Factories;
using AIERA.Desktop.WinForms.Toaster.ToastActionHandlers.Handlers;
using System.Diagnostics;

namespace AIERA.Desktop.WinForms.Toaster.ToastActionHandlers;
public class ToastActionRegistry
{
    public IReadOnlyDictionary<ToastActionValue, IToastActionHandler> ActionHandlers { get; private set; }

    public ToastActionRegistry(IToastActionHandlerFactory toastActionHandlerFactory)
    {
        ActionHandlers = new Dictionary<ToastActionValue, IToastActionHandler>()
    {
            { ToastActionValue.AcquireTokenInteractively, toastActionHandlerFactory.CreateAcquireTokenInteractivelyHandler() },
            { ToastActionValue.OpenAccountSettings, toastActionHandlerFactory.CreateOpenAccountSettingsHandler() },
    };
        Debug.Assert(ActionHandlers.Count == Enum.GetValues<ToastActionValue>().Length, $"{nameof(ToastActionValue)} and {nameof(ActionHandlers)} should have the same number of elements.");
        Debug.Assert(Enum.GetValues<ToastActionValue>().All(toastActionEnum => ActionHandlers.ContainsKey(toastActionEnum)), $"Not all {nameof(ToastActionValue)} values have handlers.");
    }

    public enum ToastActionValue { AcquireTokenInteractively, OpenAccountSettings }
}
