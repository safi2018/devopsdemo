using I95Dev.Connector.UI.Base.ViewModels.Base;

namespace I95Dev.Connector.UI.Base.ViewModels.Reports
{
    public class ReportViewModel : BaseViewModel
    {
        #region Properties

        private int selectedTab;

        /// <summary>
        /// Gets or sets the selected tab.
        /// </summary>
        /// <value>
        /// The selected tab.
        /// </value>
        public int SelectedTab
        {
            get { return selectedTab; }
            set
            {
                if (SetProperty(ref selectedTab, value))
                {
                    LoadReport();
                }
            }
        }

        /// <summary>
        /// Gets or sets the exclusion view model.
        /// </summary>
        /// <value>
        /// The exclusion view model.
        /// </value>
        public ExclusionViewModel Exclusions { get; set; }

        /// <summary>
        /// Gets or sets the notifications.
        /// </summary>
        /// <value>
        /// The notifications.
        /// </value>
        public NotificationViewModel Notifications { get; set; }

        #endregion Properties

        public ReportViewModel() : this(0)
        {
            LoadReport();
        }

        public ReportViewModel(int loadTab)
        {
            SelectedTab = loadTab;
            if (loadTab == 0) LoadReport();
        }

        /// <summary>
        /// Loads the report.
        /// </summary>
        private void LoadReport()
        {
            switch (SelectedTab)
            {
                default:
                case 0:
                    if (Notifications == null)
                    {
                        Notifications = new NotificationViewModel();
                        OnPropertyChanged("Notifications");
                    }
                    break;

                case 1:
                    if (Exclusions == null)
                    {
                        Exclusions = new ExclusionViewModel();
                        OnPropertyChanged("Exclusions");
                    }
                    break;
            }
        }
    }
}