using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.Services;
using I95Dev.Connector.UI.Base.ViewModels.Base;

namespace I95Dev.Connector.UI.Base.ViewModels.Dashboard
{
    public class StartupViewModel : BaseViewModel

    {
        public Erp.DashboardSummaryViewModel A2MSummaryModel { get; set; }

        public Ecommerce.DashboardSummaryViewModel M2SSummaryModel { get; set; }

        public RecordSummaryViewModel RecordsAndOrdersCount { get; set; }

        public LastSyncViewModel LastSyncTime { get; set; }

        public NotificationViewModel NotificationsData { get; set; }

        public LicensingViewModel LicenseData { get; set; }

        public StartupViewModel(HomeViewModel dataContext)
        {
            A2MSummaryModel = new Erp.DashboardSummaryViewModel(dataContext);
            M2SSummaryModel = new Ecommerce.DashboardSummaryViewModel(dataContext);
            LastSyncTime = new LastSyncViewModel();
            RecordsAndOrdersCount = new RecordSummaryViewModel();
            NotificationsData = new NotificationViewModel(dataContext);
            LicenseData = new LicensingViewModel();

            StatusUpdate.StatusMessage = Utilities.GetResourceValue("DefaultStatus");
        }
    }
}