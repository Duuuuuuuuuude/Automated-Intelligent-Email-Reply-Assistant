//using AIERA.Desktop.IoC.Factories;
//using Microsoft.Toolkit.Uwp.Notifications;

//namespace AIERA.Desktop.Toaster.ToastActionHandlers.Handlers;


//public class OpenSettingsHandler : IToastActionHandler
//{
//    public void HandleToastAction(ToastArguments args) => ShowSettingsForm();

//    private static void ShowSettingsForm()
//    {
//        MainApplicationContext.UIContext!.Send(_ =>
//        {
//            try
//            {
//                var settingsForm = new FormFactory().GetOpenOrCreateNewSettingsForm();
//                settingsForm.Show();
//            }
//            catch (Exception)
//            {
//                // LOG
//                MessageBox.Show("An unknown error has occurred. Please try again or contact an administrator.", $"{Program.DisplayNameAcronym} - Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                throw;
//            }
//        }, state: null);
//    }
//}