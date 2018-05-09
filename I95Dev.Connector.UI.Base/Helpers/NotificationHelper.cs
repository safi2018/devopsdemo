using System.Windows;
using I95Dev.Connector.UI.Base.ViewModels;
using I95Dev.Connector.UI.Base.ViewModels.Share;
using MvvmDialogs;

namespace I95Dev.Connector.UI.Base.Helpers
{
    internal static class NotificationHelper
    {
        private static readonly IDialogService DialogService = Utilities.CreateDialogServiceInstance();

        /// <summary>
        /// Shows the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        public static void ShowMessage(string message, string caption)
        {
            Window parentWindow1 = Application.Current.MainWindow;
            if (parentWindow1 == null) return;
            var dataContext = parentWindow1.DataContext as HomeViewModel;
            ShowMessage(dataContext, message, caption);
        }

        /// <summary>
        /// Shows the message.
        /// </summary>
        /// <param name="parentWindow">The parent window.</param>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        private static void ShowMessage(HomeViewModel parentWindow, string message, string caption)
        {
            var notification = new NotificationViewModel(message, caption);
            parentWindow.IsPopupOpen = true;
            DialogService.ShowDialog(parentWindow, notification);
            parentWindow.IsPopupOpen = false;
        }
    }
}