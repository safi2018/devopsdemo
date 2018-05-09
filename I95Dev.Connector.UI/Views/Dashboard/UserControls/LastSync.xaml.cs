using System.Windows.Controls;

namespace I95Dev.Connector.UI.Views.Dashboard.UserControls
{
    /// <summary>
    /// Interaction logic for LastSync.xaml
    /// </summary>
    public partial class LastSync : UserControl
    {
        public LastSync()
        {
            InitializeComponent();

            //SchedulerViewModel scheduleViewModel = new SchedulerViewModel();
            //SchedulerModel sm = new SchedulerModel();
            //this.DataContext = sm;
            //DateTime[] dateTime = new DateTime[2];
            //String[] SchedulerName = new string[] { "i95DevERPToEcommerceSync", "i95DevMessageQueueReceive" };
            //for (int i = 0; i < SchedulerName.Length; i++)
            //{
            //    dateTime[i] = scheduleViewModel.LastRuntimewithSchedulersNames(SchedulerName[i]);
            //}
            //sm.EndTimeDate_AX = dateTime[0];
            //sm.EndTimeDate_Magento = dateTime[1];
        }
    }
}