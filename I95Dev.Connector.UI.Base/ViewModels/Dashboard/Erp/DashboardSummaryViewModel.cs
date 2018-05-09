using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using I95Dev.Connector.Base.Helpers;
using I95Dev.Connector.UI.Base.DataAccess;
using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Helpers.Controls;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using I95Dev.Connector.UI.Base.ViewModels.Helpers;

namespace I95Dev.Connector.UI.Base.ViewModels.Dashboard.Erp
{
    public class DashboardSummaryViewModel : BaseViewModel
    {
        private readonly HomeViewModel dataContext;
        private readonly ISqlService localSql = new SqlService();

        public ObservableRangeCollection<MessageSummaryModel> ReportData { get; private set; }

        public IList<IntegerComboBoxModel> ComboBoxData { get; private set; }

        public ICommand ViewAll { get; }

        private int fromDateSelected;

        public int FromDateSelected
        {
            get { return fromDateSelected; }
            set
            {
                if (SetProperty(ref fromDateSelected, value))
                {
                    LoadDataToUi();
                }
            }
        }

        private int entityCount;

        public int EntityCount
        {
            get { return entityCount; }
            set { SetProperty(ref entityCount, value); }
        }

        public DashboardSummaryViewModel(HomeViewModel homeDataContext)
        {
            ReportData = new ObservableRangeCollection<MessageSummaryModel>();
            LoadDefaultData();
            dataContext = homeDataContext;
            ViewAll = new BaseCommand(LoadViewAllPage);
            LoadDataToUi();
        }

        private void LoadDefaultData()
        {
            ComboBoxData = ComboBoxHelper.PrepareComboBoxDates();
            FromDateSelected = 0;
        }

        private void LoadDataToUi()
        {
            IList<MessageSummaryModel> data = new List<MessageSummaryModel>
            {
                new MessageSummaryModel { EntityId = (int)Entity.Product, EntityName = "Products", CompleteCount = 0, PendingCount=0, ErrorCount=0 },
                new MessageSummaryModel { EntityId = (int)Entity.SalesOrder, EntityName = "Sales Orders", CompleteCount = 0, PendingCount=0, ErrorCount=0 },
                new MessageSummaryModel { EntityId = (int)Entity.Customer, EntityName = "Customers", CompleteCount = 0, PendingCount=0, ErrorCount=0 },
                new MessageSummaryModel { EntityId = 0, EntityName = "Others", CompleteCount = 0, PendingCount=0, ErrorCount=0 },
            };
            var searchData = new MessageSearchModel { CreatedFrom = DateTime.Today.AddDays(-FromDateSelected), CreatedTo = DateTime.Today.AddDays(1) };
            IList<MessageCountModel> databaseData = localSql.GetErpMessageSummary(searchData);
            EntityCount = localSql.GetReverseEntities().Count;

            if (databaseData.Count > 0)
            {
                (from c1 in databaseData
                 join c2 in data on c1.EntityId equals c2.EntityId
                 select c1.StatusId == (int)MessageStatus.Complete ? c2.CompleteCount = c1.Count :
                 c1.StatusId == (int)MessageStatus.Error ? c2.ErrorCount = c1.Count : c2.PendingCount += c1.Count).ToArray();

                (from c1 in databaseData
                 from c2 in data
                 where c2.EntityId == 0 & c1.EntityId != (int)Entity.Customer & c1.EntityId != (int)Entity.SalesOrder & c1.EntityId != (int)Entity.Product
                 select c1.StatusId == (int)MessageStatus.Complete ? c2.CompleteCount += c1.Count :
                c1.StatusId == (int)MessageStatus.Error ? c2.ErrorCount += c1.Count : c2.PendingCount += c1.Count).ToArray();
            }
            ReportData.Clear();
            ReportData.AddRange(data);
        }

        private void LoadViewAllPage()
        {
            DateTime fromTime = DateTime.Today.AddDays(-FromDateSelected);
            var dashboardViewModel = new ErpViewModel(fromTime);
            dataContext.Element = dashboardViewModel;
        }
    }
}