using System;
using I95Dev.Connector.Base.Helpers;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using I95Dev.Connector.UI.Base.ViewModels.Schedulers;

namespace I95Dev.Connector.UI.Base.ViewModels.Dashboard
{
    public class LastSyncViewModel : BaseViewModel
    {
        private DateTime? erpToMagneto;

        public DateTime? ErpToMagento
        {
            get { return erpToMagneto; }
            set
            {
                SetProperty(ref erpToMagneto, value);
            }
        }

        private DateTime? magentoToErp;

        public DateTime? MagentoToErp
        {
            get { return magentoToErp; }
            set
            {
                SetProperty(ref magentoToErp, value);
            }
        }

        public LastSyncViewModel()
        {
            LoadTime();
        }

        private void LoadTime()
        {
            ErpToMagento = new SchedulersViewModel(SchedulerType.PushData).PushDataScheduler.LastRuntime;
            MagentoToErp = new SchedulersViewModel(SchedulerType.PullData).PullDataScheduler.LastRuntime;
        }
    }
}