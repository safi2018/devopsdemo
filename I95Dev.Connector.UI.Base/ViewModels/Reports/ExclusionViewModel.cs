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
    public class ExclusionViewModel : BaseListViewModel<ExclusionModel>
    {
        #region Properties

        private readonly ISqlService sqlService;
        private readonly IReportService reportService;
        private readonly IDialogService dialogService;

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        /// <value>
        /// The categories.
        /// </value>
        public IList<IntegerComboBoxModel> Entities { get; private set; }

        private ExclusionsSearchModel searchModel;

        /// <summary>
        /// Gets or sets the search model.
        /// </summary>
        /// <value>
        /// The search model.
        /// </value>
        public ExclusionsSearchModel SearchModel
        {
            get { return searchModel; }
            set { SetProperty(ref searchModel, value); }
        }

        #endregion Properties

        public ExclusionViewModel()
        {
            sqlService = new SqlService();
            reportService = new ReportService();
            BindData();
            SetDefaultFilters();
            LoadReport();

            FindCommand = new BaseCommand(LoadReport);
            ResetCommand = new BaseCommand(ResetFields);
            ViewRecordCommand = new BaseCommand(ViewRecord);
            dialogService = Utilities.CreateDialogServiceInstance();
        }

        #region Commands

        /// <summary>
        /// Gets the reset command.
        /// </summary>
        /// <value>
        /// The reset command.
        /// </value>
        public ICommand ResetCommand { get; }

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
        public ICommand FindCommand { get; }

        /// <summary>
        /// Gets the view record command.
        /// </summary>
        /// <value>
        /// The view record command.
        /// </value>
        public ICommand ViewRecordCommand { get; }

        /// <summary>
        /// Views the record.
        /// </summary>
        /// <param name="arg">The argument.</param>
        private void ViewRecord(object arg)
        {
            ExclusionModel model = arg as ExclusionModel;
            if (model != null)
            {
                dialogService.ShowDialog(this, new ExclusionDetailsViewModel(model));
            }
        }

        #endregion Commands

        #region Methods

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            Entities = sqlService.GetReverseEntities();
            Entities.Insert(0, new IntegerComboBoxModel { Name = "All", Value = 0 });
        }

        /// <summary>
        /// Sets the default filters.
        /// </summary>
        private void SetDefaultFilters()
        {
            SearchModel = new ExclusionsSearchModel
            {
                EntityId = 0,
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
        /// Loads the report.
        /// </summary>
        private void LoadReport()
        {
            SetRecordList(reportService.GetExclusionsData(SearchModel));
        }

        #endregion Methods
    }
}