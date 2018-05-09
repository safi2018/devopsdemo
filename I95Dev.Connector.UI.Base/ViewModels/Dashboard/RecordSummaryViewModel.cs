using System;
using System.Collections.Generic;
using System.Windows.Input;
using I95Dev.Connector.UI.Base.DataAccess;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Helpers.Controls;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using I95Dev.Connector.UI.Base.ViewModels.Helpers;

namespace I95Dev.Connector.UI.Base.ViewModels.Dashboard
{
    public class RecordSummaryViewModel : BaseViewModel
    {
        private readonly ISqlService localSql = new SqlService();

        public ICommand ViewAll { get; }

        public IList<IntegerComboBoxModel> ComboBoxData { get; private set; }

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

        private int magentoToErpCount;

        public int MagentoToErpCount
        {
            get { return magentoToErpCount; }
            set
            {
                SetProperty(ref magentoToErpCount, value);
            }
        }

        private int erpToMagentoCount;

        public int ErpToMagentoCount
        {
            get
            {
                return erpToMagentoCount;
            }
            set
            {
                SetProperty(ref erpToMagentoCount, value);
            }
        }

        private decimal magentoOrderValue;

        public decimal MagentoOrderValue
        {
            get { return magentoOrderValue; }
            set { SetProperty(ref magentoOrderValue, value); }
        }

        private decimal erpOrderValue;

        public decimal ErpOrderValue
        {
            get { return erpOrderValue; }
            set { SetProperty(ref erpOrderValue, value); }
        }

        private void LoadDataToUi()
        {
            var searchData = new MessageSearchModel { CreatedFrom = DateTime.Today.AddDays(-fromDateSelected), CreatedTo = DateTime.Today.AddDays(1) };

            MagentoToErpCount = localSql.GetEcommerceRecordCount(searchData);
            ErpToMagentoCount = localSql.GetErpRecordCount(searchData);

            MagentoOrderValue = localSql.GetMagentoOrderValue(searchData);
            ErpOrderValue = localSql.GetErpOrderValue(searchData);
        }

        public RecordSummaryViewModel()
        {
            LoadDefaultData();
            ViewAll = new BaseCommand(LoadViewAllPage);
            LoadDataToUi();
        }

        private static void LoadViewAllPage(object arg)
        {
        }

        private void LoadDefaultData()
        {
            ComboBoxData = ComboBoxHelper.PrepareComboBoxDates();
            FromDateSelected = 0;
        }
    }
}