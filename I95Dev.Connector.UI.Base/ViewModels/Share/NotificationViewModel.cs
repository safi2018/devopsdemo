using System.Windows.Input;
using I95Dev.Connector.Base.Common;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using MvvmDialogs;

namespace I95Dev.Connector.UI.Base.ViewModels.Share
{
    public class NotificationViewModel : BaseViewModel, IModalDialogViewModel
    {
        public bool ShowInformation { get; set; }

        public bool ShowWarning { get; set; }

        public bool ShowError { get; set; }

        public string Message { get; set; }

        public ICommand CloseCommand { get; private set; }

        private bool? dialogResult;

        /// <summary>
        /// methods.
        /// </summary>
        public bool? DialogResult
        {
            get
            {
                return dialogResult;
            }
            private set
            {
                SetProperty(ref dialogResult, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationViewModel"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        public NotificationViewModel(string message, string caption)
        {
            Message = message;

            switch (caption.ToUpper(Constants.DefaultCulture))
            {
                case "WARNING":
                    ShowWarning = true;
                    break;

                case "ERROR":
                    ShowError = true;
                    break;

                default:
                    ShowInformation = true;
                    break;
            }

            CloseCommand = new BaseCommand(CloseWindow);
        }

        private void CloseWindow()
        {
            DialogResult = true;
        }
    }
}