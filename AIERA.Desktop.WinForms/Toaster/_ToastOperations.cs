using AIERA.Desktop.WinForms.Toaster.Enums;
using AIERA.Desktop.WinForms.Toaster.ToastActionHandlers;
using Microsoft.Toolkit.Uwp.Notifications;
using static AIERA.Desktop.WinForms.Toaster.ToastActionHandlers.ToastActionRegistry;

namespace AIERA.Desktop.WinForms.Toaster;


public class ToastOperations
{
    private readonly ToastActionRegistry _toastActionRegistry;

    public ToastOperations(ToastActionRegistry toastActionRegistry) => _toastActionRegistry = toastActionRegistry;

    public void OnToastActivated()
    {
        //#if DEBUG
        //            ToastNotificationManagerCompat.History.Clear();
        //#endif
        ToastNotificationManagerCompat.OnActivated += toastArgs =>
        {
            ToastArguments args = ToastArguments.Parse(toastArgs.Argument);
            HandleToastActions(args);
        };
    }

    private void HandleToastActions(ToastArguments args)
    {
        if (args.TryGetValue(nameof(ToastArgumentKey.Action), out ToastActionValue buttonAction))
        {
            _ = _toastActionRegistry.ActionHandlers.TryGetValue(buttonAction, out var handler);
            handler!.HandleToastAction(args);
        }
    }
}