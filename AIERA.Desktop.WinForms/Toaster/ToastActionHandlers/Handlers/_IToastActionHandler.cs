using Microsoft.Toolkit.Uwp.Notifications;

namespace AIERA.Desktop.WinForms.Toaster.ToastActionHandlers.Handlers;
public interface IToastActionHandler
{
    void HandleToastAction(ToastArguments args);
}