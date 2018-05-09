using System;
using I95Dev.Connector.UI.Base.DataAccess;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using I95Dev.Connector.UI.Base.ViewModels.Helpers;

namespace I95Dev.Connector.UI.Base.ViewModels.Dashboard.Erp
{
    public class ErpViewModel : BaseViewModel
    {
        #region Properties

        private readonly ISqlService localSql;
        private ErpDashboardViewModel dashboardViewModel;

        /// <summary>
        /// Gets or sets the dashboard view model.
        /// </summary>
        /// <value>
        /// The dashboard view model.
        /// </value>
        public ErpDashboardViewModel DashboardViewModel
        {
            get
            {
                return dashboardViewModel;
            }
            set
            {
                SetProperty(ref dashboardViewModel, value);
            }
        }

        private ErpReportViewModel reportViewModel;

        /// <summary>
        /// Gets or sets the report view model.
        /// </summary>
        /// <value>
        /// The report view model.
        /// </value>
        public ErpReportViewModel ReportViewModel
        {
            get
            {
                return reportViewModel;
            }
            set
            {
                SetProperty(ref reportViewModel, value);
            }
        }

        private int selectedTab;

        /// <summary>
        /// Gets or sets the selected tab.
        /// </summary>
        /// <value>
        /// The selected tab.
        /// </value>
        public int SelectedTab
        {
            get
            {
                return selectedTab;
            }
            set
            {
                if (SetProperty(ref selectedTab, value))
                    LoadData();
            }
        }

        #endregion Properties

        public ErpViewModel() : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErpViewModel"/> class.
        /// </summary>
        /// <param name="fromTime">From time.</param>
        public ErpViewModel(DateTime fromTime)
        {
            localSql = new SqlService();
            DashboardViewModel = new ErpDashboardViewModel(localSql, fromTime);
            DashboardViewModel.ViewRecordsClicked += DashboardViewModel_ViewRecordsClicked;

            SelectedTab = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErpViewModel"/> class.
        /// </summary>
        /// <param name="loadTab">The load tab.</param>
        public ErpViewModel(int loadTab)
        {
            localSql = new SqlService();
            SelectedTab = loadTab;
            if (loadTab == 0)
                LoadData();
        }

        #region Methods

        /// <summary>
        /// Loads the data.
        /// </summary>
        private void LoadData()
        {
            switch (SelectedTab)
            {
                default:
                case 0:
                    if (DashboardViewModel == null)
                    {
                        DashboardViewModel = new ErpDashboardViewModel(localSql);
                        DashboardViewModel.ViewRecordsClicked += DashboardViewModel_ViewRecordsClicked;
                    }
                    break;

                case 1:
                    if (ReportViewModel == null)
                        ReportViewModel = new ErpReportViewModel(localSql);
                    break;
            }
        }

        /// <summary>
        /// Dashboards the view model view records clicked.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        private void DashboardViewModel_ViewRecordsClicked(MessageSearchModel searchModel)
        {
            if (ReportViewModel == null)
            {
                ReportViewModel = new ErpReportViewModel(localSql, searchModel);
            }
            else
            {
                ReportViewModel.LoadDataWithFilters(searchModel);
            }
            SelectedTab = 1;
        }

        #endregion Methods
    }
}