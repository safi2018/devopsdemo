using System;
using System.Collections.Generic;
using System.Windows.Input;
using I95Dev.Connector.UI.Base.DataAccess;
using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using I95Dev.Connector.UI.Base.ViewModels.Helpers;
using MvvmDialogs;

namespace I95Dev.Connector.UI.Base.ViewModels.Reports
{
    public class NotificationViewModel : BaseListViewModel<NotificationModel>
    {
        #region Properties

        private IReportService reportService;
        private IDialogService dialogService;

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        /// <value>
        /// The categories.
        /// </value>
        public IList<IntegerComboBoxModel> Categories { get; private set; }

        /// <summary>
        /// Gets or sets the search model.
        /// </summary>
        /// <value>
        /// The search model.
        /// </value>
        public NotificationsSearchModel SearchModel
        { get; set; }

        #endregion Properties

        public NotificationViewModel(bool loadReport = true)
        {
            Initialize();

            if (loadReport)
            {
                LoadReport();
            }
        }

        public NotificationViewModel(DateTime startTime, DateTime endTime) : this(false)
        {
            SearchModel.FromDate.Date = startTime;
            SearchModel.ToDate.Date = endTime;
            LoadReport();
        }

        #region Commands

        /// <summary>
        /// Gets the reset command.
        /// </summary>
        /// <value>
        /// The reset command.
        /// </value>
        public ICommand ResetCommand { get; private set; }

        /// <summary>
        /// Resets the fields.
        /// </summary>
        private void ResetFields()
        {
            SetDefaultFilters();
            LoadReport();
        }

        /// <summary>
        /// Gets the find command.
        /// </summary>
        /// <value>
        /// The find command.
        /// </value>
        public ICommand FindCommand { get; private set; }

        /// <summary>
        /// Gets the view record command.
        /// </summary>
        /// <value>
        /// The view record command.
        /// </value>
        public ICommand ViewRecordCommand { get; private set; }

        /// <summary>
        /// Views the record.
        /// </summary>
        /// <param name="arg">The argument.</param>
        private void ViewRecord(object arg)
        {
            NotificationModel model = arg as NotificationModel;
            if (model == null) return;
            dialogService.ShowDialog(this, new NotificationDetailsViewModel(model));
        }

        #endregion Commands

        #region Methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            reportService = new ReportService();
            BindData();
            SetDefaultFilters();

            FindCommand = new BaseCommand(LoadReport);
            ResetCommand = new BaseCommand(ResetFields);
            ViewRecordCommand = new BaseCommand(ViewRecord);
            dialogService = Utilities.CreateDialogServiceInstance();
        }

        /// <summary>
        /// Sets the default filters.
        /// </summary>
        private void SetDefaultFilters()
        {
            SearchModel = new NotificationsSearchModel
            {
                CategoryId = 0,
                IsAll = true,
                FromDate = new DateModel
                {
                    Date = DateTime.Today.AddDays(-7),
                    HasValue = true
                },
                ToDate = new DateModel
                {
                    Date = DateTime.Today.AddDays(1),
                    HasValue = true
                },
            };
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            Categories = reportService.GetCategories();
            Categories.Insert(0, new IntegerComboBoxModel { Name = "All", Value = 0 });
        }

        /// <summary>
        /// Loads the report.
        /// </summary>
        private void LoadReport()
        {
            SetRecordList(reportService.GetNotificationsData(SearchModel));
        }

        #endregion Methods
    }
}