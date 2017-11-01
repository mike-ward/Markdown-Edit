using System.Threading.Tasks;
using System.Windows;
using Infrastructure;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ServicesModule.Services
{
    public class Notify : INotify
    {
        private static MetroDialogSettings Settings()
        {
            return new MetroDialogSettings
            {
                DialogMessageFontSize = 20,
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No",
                FirstAuxiliaryButtonText = "Cancel",
                AnimateShow = false,
                AnimateHide = false
            };
        }

        public async Task<bool> Alert(string alert)
        {
            var settings = Settings();
            settings.AffirmativeButtonText = "Close";
            await Show(alert, MessageDialogStyle.Affirmative, settings);
            return true;
        }

        public async Task<MessageBoxResult> ConfirmYesNo(string question)
        {
            var result = await Show(question, MessageDialogStyle.AffirmativeAndNegative, Settings());
            return result == MessageDialogResult.Affirmative ? MessageBoxResult.Yes : MessageBoxResult.No;
        }

        public async Task<MessageBoxResult> ConfirmYesNoCancel(string question)
        {
            var result = await Show(question, MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, Settings());
            if (result == MessageDialogResult.Affirmative) return MessageBoxResult.Yes;
            if (result == MessageDialogResult.Negative) return MessageBoxResult.No;
            return MessageBoxResult.Cancel;
        }

        private static async Task<MessageDialogResult> Show(string message, MessageDialogStyle buttons, MetroDialogSettings settings)
        {
            var text = message ?? "null";
            var window = (MetroWindow)Application.Current.MainWindow;
            return await window.ShowMessageAsync(string.Empty, text, buttons, settings);
        }
    }
}
