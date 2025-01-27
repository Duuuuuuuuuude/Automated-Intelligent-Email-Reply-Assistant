using AIERA.Desktop.WinForms.IoC.Factories;
using Microsoft.Toolkit.Uwp.Notifications;

namespace AIERA.Desktop.WinForms.Toaster.ToastActionHandlers.Handlers;

public class OpenAccountSettingsHandler : IToastActionHandler
{
    public void HandleToastAction(ToastArguments args)
    {
        ShowSettingsForm();
    }

    // TODO: Implement ToastAction handler that takes the user to the specified account in the settings form.
    private static void ShowSettingsForm()
    {
        MainApplicationContext.UIContext!.Send(_ =>
        {
            try
            {
                // TODO: Open settings for the specified account.
                var settingsForm = new FormFactory().GetOpenOrCreateNewSettingsForm();
                settingsForm.Show();
            }
            catch (Exception)
            {
                // LOG
                _ = MessageBox.Show("An unknown error has occurred. Please try again or contact an administrator.", $"{Program.ApplicationNameAbbreviation} - Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }, state: null);
    }
}
