using AIERA.Desktop.WinForms.Toaster.Enums;
using Microsoft.Identity.Client;
using Microsoft.Toolkit.Uwp.Notifications;

namespace AIERA.Desktop.WinForms.Toaster;

public interface IToastNotification
{
    ToastButton CreateToastAccountSettingsButton();
    ToastButton CreateToastAuthButton(string buttonText,
                                         string loginHint,
                                         string? claims,
                                         string identifier);
    void ShowMsalToastNotification(HeaderId headerId,
                            string headerTitle,
                            ToastGroup group,
                            IAccount account,
                            IEnumerable<ToastButton> buttons,
                            string firstLine,
                            string? secondLine = null,
                            string? thirdLine = null);
}
