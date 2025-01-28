using AIERA.Desktop.WinForms.Authentication.Accounts.Microsoft;
using AIERA.Desktop.WinForms.Authentication.Models.Result_Pattern;
using AIERA.Desktop.WinForms.IoC.Factories;
using AIERA.Desktop.WinForms.Models.ViewModels;
using AIERA.Desktop.WinForms.Toaster.Enums;
using Microsoft.Toolkit.Uwp.Notifications;

namespace AIERA.Desktop.WinForms.Toaster.ToastActionHandlers.Handlers;

public class AcquireTokenInteractivelyHandler : IToastActionHandler
{
    private readonly IMicrosoftAuthentication microsoftAuthentication;

    public AcquireTokenInteractivelyHandler(IMicrosoftAuthentication microsoftAuthentication)
    {
        this.microsoftAuthentication = microsoftAuthentication;
    }

    public void HandleToastAction(ToastArguments args) => AcquireTokenInteractively(args);

    private void AcquireTokenInteractively(ToastArguments args)
    {
        MainApplicationContext.UIContext!.Post(_ =>
        {
            string loginHint = args.Get(nameof(ToastArgumentKey.LoginHint));
            if (string.IsNullOrWhiteSpace(loginHint))
                throw new InvalidOperationException($"The '{nameof(ToastArgumentKey.LoginHint)}' argument in args is required but was null or empty.");

            string identifier = args.Get(nameof(ToastArgumentKey.Identifier));
            if (string.IsNullOrWhiteSpace(identifier))
                throw new InvalidOperationException($"The '{nameof(ToastArgumentKey.Identifier)}' argument in args is required but was null or empty.");

            _ = args.TryGetValue(nameof(ToastArgumentKey.Claims), out string? claims);

            // Explicitly capture and handle the Task to avoid fire-and-forget warning.
            Task task = microsoftAuthentication.SignInAsync(hWnd: null, loginHint, identifier, claims)
                                               .ContinueWith(task =>
            {
                if (task.Exception is not null)
                {
                    _ = MessageBox.Show($"An unknown error has occurred while trying to sign in with '{loginHint}'. Please try again or contact admin.",
                                        $"{Program.ApplicationNameAbbreviation} - Unexpected error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    throw task.Exception; // LOG
                }

                Result<MicrosoftAccountViewModel> authResult = task.Result;

                MicrosoftAccountViewModel accountViewModel = authResult.GetMicrosoftAccountViewModel();

                if (new FormFactory().GetOpenSettingsForm() is { IsDisposed: false } settingsForm)
                    settingsForm.UpsertAccountButton(authResult.Value!);

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }, state: null);
    }
}