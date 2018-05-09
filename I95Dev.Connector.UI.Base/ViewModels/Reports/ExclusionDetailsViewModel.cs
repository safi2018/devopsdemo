using System.Windows;
using System.Windows.Input;
using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using MvvmDialogs;

namespace I95Dev.Connector.UI.Base.ViewModels.Reports
{
    public class ExclusionDetailsViewModel : BaseViewModel, IModalDialogViewModel
    {
        #region Properties

        public string EntityName { get; set; }
        public string RecordId { get; set; }
        public string CreatedDate { get; set; }
        public string Parameters { get; }
        public bool IsJson { get; }

        private bool? dialogResult;

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

        #endregion Properties

        public ExclusionDetailsViewModel(ExclusionModel model)
        {
            EntityName = model.EntityName;
            RecordId = model.RecordId;
            Parameters = model.Parameters;
            IsJson = model.IsJson;
            CreatedDate = ExtensionMethods.FormatDateTime(model.CreatedTime);
            CloseCommand = new BaseCommand(CloseWindow);
            CopyCommand = new BaseCommand(CopyText);
        }

        #region Commands

        /// <summary>
        /// Gets the close command.
        /// </summary>
        /// <value>
        /// The close command.
        /// </value>
        public ICommand CloseCommand { get; }

        /// <summary>
        /// Closes the window.
        /// </summary>
        private void CloseWindow()
        {
            DialogResult = true;
        }

        /// <summary>
        /// Gets the copy command.
        /// </summary>
        /// <value>
        /// The copy command.
        /// </value>
        public ICommand CopyCommand { get; }

        /// <summary>
        /// Copies the text.
        /// </summary>
        private void CopyText()
        {
            if (!string.IsNullOrEmpty(Parameters))
            {
                Clipboard.SetText(Parameters);
            }
        }

        #endregion Commands
    }
}