using System;
using System.Collections.Generic;
using System.Windows.Input;
using I95Dev.Connector.UI.Base.DataAccess;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using I95Dev.Connector.UI.Base.ViewModels.Helpers;

namespace I95Dev.Connector.UI.Base.ViewModels.Dashboard
{
    public class NotificationViewModel : BaseViewModel
    {
        private readonly IReportService reportService;
        private readonly HomeViewModel dataContext;

        /// <summary>
        /// Gets the find command.
        /// </summary>
        /// <value>
        /// The find command.
        /// </value>
        public ICommand ViewAllCommand { get; private set; }

        public int RecordCount { get; set; }

        public IList<NotificationModel> RecordList { get; private set; }

        public NotificationViewModel(HomeViewModel homeDataContext)
        {
            dataContext = homeDataContext;
            reportService = new ReportService();
            ViewAllCommand = new BaseCommand(ViewAll);
            LoadData();
        }

        private void LoadData()
        {
            var searchModel = new NotificationsSearchModel
            {
                CategoryId = 0,
                IsAll = true,
                FromDate = new DateModel
                {
                    Date = DateTime.Today,
                    HasValue = true
                },
                ToDate = new DateModel
                {
                    Date = DateTime.Today.AddDays(1),
                    HasValue = true
                },
            };

            RecordList = reportService.GetNotificationsData(searchModel);
            RecordCount = RecordList.Count;
        }

        private void ViewAll()
        {
            dataContext.Element = new Reports.ReportViewModel(0);
        }
    }
}