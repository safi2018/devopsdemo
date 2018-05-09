using System.ComponentModel;
using I95Dev.Connector.UI.Base.Services;
using MvvmDialogs;
using MvvmDialogs.DialogTypeLocators;

namespace I95Dev.Connector.UI.Base.Helpers.Mvvm
{
    internal class LocalDialogService : DialogService, IDialogService
    {
        public LocalDialogService(IDialogTypeLocator dialogTypeLocator) : base(null, dialogTypeLocator)
        {
        }

        /// <summary>
        /// Displays a modal dialog of a type that is determined by the dialog type locater.
        /// </summary>
        /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
        /// <param name="viewModel">The view model of the new dialog.</param>
        /// <returns>
        /// A nullable value of type <see cref="T:System.Boolean" /> that signifies how a window was closed by
        /// the user.
        /// </returns>
        public new bool? ShowDialog(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel)
        {
            StatusUpdate.IsPopUpOpen = true;
            bool? result = base.ShowDialog(ownerViewModel, viewModel);
            StatusUpdate.IsPopUpOpen = false;
            return result;
        }
    }
}