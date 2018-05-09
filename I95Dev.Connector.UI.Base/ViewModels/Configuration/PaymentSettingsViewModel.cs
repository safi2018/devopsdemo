using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using I95Dev.Connector.UI.Base.DataAccess;
using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Helpers.Controls;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.Services;
using I95Dev.Connector.UI.Base.ViewModels.Base;

namespace I95Dev.Connector.UI.Base.ViewModels.Configuration
{
    public class PaymentSettingsViewModel : BaseViewModel
    {
        #region Properties

        private readonly IPaymentMethodDataAccess localSql;
        private string validationMessage;
        private bool showPopup;
        private PaymentSettingsModel paymentMethod;

        public ObservableRangeCollection<PaymentSettingsModel> Responses { get; }

        public IList<IntegerComboBoxModel> ComboBoxGpPayment { get; }

        public PaymentSettingsModel PaymentMethod
        {
            get { return paymentMethod; }
            set { SetProperty(ref paymentMethod, value); }
        }

        public string ValidationMessage
        {
            get { return validationMessage; }
            private set { SetProperty(ref validationMessage, value); }
        }

        public bool ShowPopup
        {
            get { return showPopup; }
            set
            {
                if (!SetProperty(ref showPopup, value)) return;
                ValidationMessage = string.Empty;
                StatusUpdate.IsPopUpOpen = value;
            }
        }

        #endregion Properties

        public PaymentSettingsViewModel()
        {
            localSql = new PaymentMethodDataAccess();

            bool result = localSql.IsPaymentMethodTableExists();
            if (!result)
            {
                NotificationHelper.ShowMessage("PaymentMethod table not found", Utilities.GetResourceValue("CaptionError"));
                return;
            }

            Responses = new ObservableRangeCollection<PaymentSettingsModel>();
            ComboBoxGpPayment = AddGpPayments();
            LoadGrid();
            ClosePopupCommand = new BaseCommand(ClosePopup);
            AddNewPaymentCommand = new BaseCommand(AddNewPaymentRecord);
            SavePaymentCommand = new BaseCommand(SavePaymentRecord);
            ViewRecordCommand = new BaseCommand(ViewRecord);
        }

        #region Commands

        public ICommand AddNewPaymentCommand { get; }

        private void AddNewPaymentRecord()
        {
            PaymentMethod = new PaymentSettingsModel { GpPaymentId = -1 };
            ShowPopup = true;
        }

        public ICommand SavePaymentCommand { get; }

        private void SavePaymentRecord()
        {
            try
            {
                ValidationMessage = "All fields are required";
                if (string.IsNullOrEmpty(PaymentMethod.MagentoPaymentName) || PaymentMethod.GpPaymentId < 0) return;

                PaymentMethod.GpPaymentName = GetGpPaymentName(PaymentMethod.GpPaymentId);
                if (localSql.CheckPaymentCombination(PaymentMethod))
                {
                    ValidationMessage = "Combination Already Exist";
                }
                else if (IsDataValid(PaymentMethod))
                {
                    if (localSql.SavePaymentDetails(PaymentMethod))
                    {
                        ShowPopup = false;
                        NotificationHelper.ShowMessage("Updated Successfully", Utilities.GetResourceValue("CaptionInfo"));
                        ValidationMessage = "";
                        LoadGrid();
                    }
                    else
                    {
                        ValidationMessage = "Unknown error occured. Please try again after some time";
                    }
                }
            }
            catch (Exception exception)
            {
                ValidationMessage = exception.Message;
            }
        }

        private static bool IsDataValid(PaymentSettingsModel paymentSettingsModel)
        {
            //TODO Need to valid data with Magento & GP
            return true;
        }

        private static string GetGpPaymentName(int paymentId)
        {
            switch (paymentId)
            {
                case 0:
                    return "Creditlimits";

                case 1:
                    return "CashDeposit";

                case 2:
                    return "CheckDeposit";

                case 3:
                    return "CreditCardDeposit";

                case 6:
                    return "CreditCardPayment";

                default:
                    return string.Empty;
            }
        }

        public ICommand ClosePopupCommand { get; }

        private void ClosePopup()
        {
            ShowPopup = false;
        }

        public ICommand ViewRecordCommand { get; }

        private void ViewRecord(object arg)
        {
            if (!(arg is PaymentSettingsModel model)) return;

            PaymentMethod = model;
            ShowPopup = true;
        }

        #endregion Commands

        #region Methods

        private static IList<IntegerComboBoxModel> AddGpPayments()
        {
            var list = new List<IntegerComboBoxModel>
            {
                new IntegerComboBoxModel{Name="CREDIT LIMIT" ,Value=0},
                new IntegerComboBoxModel{Name="CASH PAYMENT" ,Value=1},
                new IntegerComboBoxModel{Name="CHECK" ,Value=2},
                new IntegerComboBoxModel{Name="CREDIT CARD DEPOSIT" ,Value=3},
                new IntegerComboBoxModel{Name="CREDIT CARD PAYMENT" ,Value=6}
            };
            return list;
        }

        private void LoadGrid()
        {
            IList<PaymentSettingsModel> entityList = localSql.GetPaymentMethodsList();
            Responses.Clear();
            if (!entityList.Any()) return;
            Responses.AddRange(entityList);
        }

        #endregion Methods
    }
}