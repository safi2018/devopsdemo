using System;
using I95Dev.Connector.UI.Base.DataAccess;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using I95Dev.Connector.UI.Base.ViewModels.Helpers;

namespace I95Dev.Connector.UI.Base.ViewModels.Dashboard.Ecommerce
{
    public class EcommerceViewModel : BaseViewModel
    {
        #region Properties

        private readonly ISqlService localSql;
        private EcommerceDashboardViewModel dashboardViewModel;

        /// <summary>
        /// Gets or sets the dashboard view model.
        /// </summary>
        /// <value>
        /// The dashboard view model.
        /// </value>
        public EcommerceDashboardViewModel DashboardViewModel
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

        private EcommerceReportViewModel reportViewModel;

        /// <summary>
        /// Gets or sets the report view model.
        /// </summary>
        /// <value>
        /// The report view model.
        /// </value>
        public EcommerceReportViewModel ReportViewModel
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

        /// <summary>
        /// Initializes a new instance of the <see cref="EcommerceViewModel"/> class.
        /// </summary>
        public EcommerceViewModel() : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EcommerceViewModel" /> class.
        /// </summary>
        /// <param name="fromTime">From time.</param>
        public EcommerceViewModel(DateTime fromTime)
        {
            localSql = new SqlService();
            DashboardViewModel = new EcommerceDashboardViewModel(localSql, fromTime);
            DashboardViewModel.ViewRecordsClicked += DashboardViewModel_ViewRecordsClicked;
            SelectedTab = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EcommerceViewModel"/> class.
        /// </summary>
        /// <param name="loadTab">The load tab.</param>
        public EcommerceViewModel(int loadTab)
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
                    if (DashboardViewModel == null)
                    {
                        DashboardViewModel = new EcommerceDashboardViewModel(localSql);
                        DashboardViewModel.ViewRecordsClicked += DashboardViewModel_ViewRecordsClicked;
                    }
                    break;

                case 1:
                    if (ReportViewModel == null)
                        ReportViewModel = new EcommerceReportViewModel(localSql);
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
                ReportViewModel = new EcommerceReportViewModel(localSql, searchModel);
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