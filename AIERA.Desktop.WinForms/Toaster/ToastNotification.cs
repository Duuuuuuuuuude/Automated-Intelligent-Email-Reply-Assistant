using AIERA.Desktop.WinForms.Toaster.Enums;
using Microsoft.Identity.Client;
using Microsoft.Toolkit.Uwp.Notifications;
using static AIERA.Desktop.WinForms.Toaster.ToastActionHandlers.ToastActionRegistry;

namespace AIERA.Desktop.WinForms.Toaster;

public class ToastNotification : IToastNotification
{
    public void ShowMsalToastNotification(HeaderId headerId, string headerTitle, ToastGroup group, IAccount account,
                                      IEnumerable<ToastButton> buttons, string firstLine, string? secondLine = null,
                                      string? thirdLine = null)
    {
        ToastContentBuilder toastBuilder = new();

        toastBuilder.AddHeader(id: nameof(headerId),
                               title: headerTitle,
                               new ToastArguments { { nameof(ToastArgumentKey.Action), nameof(ToastActionValue.OpenAccountSettings) }, });

        toastBuilder.AddArgument(nameof(ToastArgumentKey.Action), ToastActionValue.OpenAccountSettings);

        toastBuilder.AddText(firstLine, language: "en-US");

        if (!string.IsNullOrWhiteSpace(secondLine))
            toastBuilder.AddText(secondLine, language: "en-US");
        if (!string.IsNullOrWhiteSpace(thirdLine))
            toastBuilder.AddText(thirdLine, language: "en-US");

        foreach (var button in buttons)
            toastBuilder.AddButton(button);

#if DEBUG
        toastBuilder.SetToastDuration(ToastDuration.Long);
#else
    toastBuilder.SetToastDuration(ToastDuration.Short);
#endif
        toastBuilder.SetToastScenario(ToastScenario.Default);

        toastBuilder.Show(toast =>
        {
            if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 18362))
                toast.ExpiresOnReboot = false;

            toast.Group = nameof(group);
            toast.Tag = account?.HomeAccountId.ObjectId; // 'toast.Tag' has a limit of 64 characters. 'account.HomeAccountId.ToString()' would be too long and 'account.Username' can also be to long. That is why 'account.HomeAccountId.ObjectId' is used, even though it isn't as unique across tenants.
        });
    }

    public ToastButton CreateToastAccountSettingsButton() => new ToastButton().SetContent("Open Settings")
                                                                              .AddArgument(nameof(ToastArgumentKey.Action), ToastActionValue.OpenAccountSettings);

    public ToastButton CreateToastAuthButton(string buttonText, string loginHint, string? claims, string identifier)
    {
        return new ToastButton().SetContent(buttonText)
                                .AddArgument(nameof(ToastArgumentKey.Action), ToastActionValue.AcquireTokenInteractively)
                                .AddArgument(nameof(ToastArgumentKey.LoginHint), loginHint)
                                .AddArgument(nameof(ToastArgumentKey.Claims), claims)
                                .AddArgument(nameof(ToastArgumentKey.Identifier), identifier);
    }
}