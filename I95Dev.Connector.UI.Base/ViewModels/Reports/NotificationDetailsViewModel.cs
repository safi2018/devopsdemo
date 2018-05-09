using System;
using System.Windows.Input;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using MvvmDialogs;

namespace I95Dev.Connector.UI.Base.ViewModels.Reports
{
    public class NotificationDetailsViewModel : BaseViewModel, IModalDialogViewModel
    {
        #region Properties

        public long Id { get; set; }

        public string CategoryName { get; set; }

        public string Description { get; set; }

        public DateTime CreatedTime { get; set; }

        public string IsMailSent { get; set; }

        public string ViewRecord { get; set; }

        public string DetailDescription { get; set; }

        public int CategoryId { get; set; }

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

        public NotificationDetailsViewModel(NotificationModel model)
        {
            Id = model.CategoryId;

            CategoryName = model.CategoryName;

            Description = model.Description;

            IsMailSent = model.IsMailSent;

            ViewRecord = model.ViewRecord;

            CreatedTime = model.CreatedTime;

            DetailDescription = model.DetailDescription;

            CloseCommand = new BaseCommand(CloseWindow);
        }

        #region Commands

        /// <summary>
        /// Gets the close command.
        /// </summary>
        /// <value>
        /// The close command.
        /// </value>
        public ICommand CloseCommand { get; private set; }

        /// <summary>
        /// Closes the window.
        /// </summary>
        private void CloseWindow()
        {
            DialogResult = true;
        }

        #endregion Commands
    }
}