using System;
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
    public class ShippingCarrierViewModel : BaseViewModel
    {
        #region Properties

        private readonly IShippingMethodDataAccess localSql;
        private bool showPopup;
        private string validationMessage;
        private ShippingCarrierModel carrier;

        public ShippingCarrierModel Carrier
        {
            get { return carrier; }
            set { SetProperty(ref carrier, value); }
        }

        public bool ShowPopup
        {
            get { return showPopup; }
            set
            {
                if (SetProperty(ref showPopup, value))
                    StatusUpdate.IsPopUpOpen = value;
            }
        }

        public string ValidationMessage
        {
            get { return validationMessage; }
            private set { SetProperty(ref validationMessage, value); }
        }

        public ObservableRangeCollection<ShippingCarrierModel> Records { get; }

        #endregion Properties

        public ShippingCarrierViewModel()
        {
            localSql = new ShippingMethodDataAccess();

            bool result = localSql.IsShippingCarrierTableExists();
            if (!result)
            {
                NotificationHelper.ShowMessage("ShippingCarrier table not found", Utilities.GetResourceValue("CaptionError"));
                return;
            }

            ClosePopupCommand = new BaseCommand(ClosePopup);
            AddNewCarrierCommand = new BaseCommand(AddNewCarrierRecord);
            SaveCarrierCommand = new BaseCommand(SaveCarrier);
            ViewRecordCommand = new BaseCommand(ViewRecord);
            Records = new ObservableRangeCollection<ShippingCarrierModel>();

            BindData();
        }

        #region Commands

        public ICommand AddNewCarrierCommand { get; }

        private void AddNewCarrierRecord()
        {
            ClearValues();
            ShowPopup = true;
        }

        public ICommand SaveCarrierCommand { get; }

        private void SaveCarrier()
        {
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("Loading");
            try
            {
                if (string.IsNullOrEmpty(Carrier.CarrierCode) || string.IsNullOrEmpty(Carrier.CarrierDescription))
                {
                    ValidationMessage = "Fill Required Fields";
                    return;
                }

                bool result = localSql.IsCarrierExists(Carrier);
                if (result)
                {
                    ValidationMessage = "Carrier Already Exist";
                    return;
                }

                result = localSql.SaveCarrierDetails(Carrier);
                if (result)
                {
                    ShowPopup = false;
                    NotificationHelper.ShowMessage("Saved Successfully", Utilities.GetResourceValue("CaptionInfo"));
                    BindData();
                }
            }
            catch (Exception exception)
            {
                ValidationMessage = exception.Message;
            }
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("DefaultStatus");
        }

        public ICommand ViewRecordCommand { get; }

        private void ViewRecord(object arg)
        {
            if (!(arg is ShippingCarrierModel model)) return;

            Carrier = model;
            ValidationMessage = "";
            ShowPopup = true;
        }

        public ICommand ClosePopupCommand { get; }

        private void ClosePopup()
        {
            ShowPopup = false;
        }

        #endregion Commands

        #region Methods

        private void BindData()
        {
            Records.Clear();
            Records.AddRange(localSql.GetShipmentCarriers());
        }

        private void ClearValues()
        {
            Carrier = new ShippingCarrierModel();
            ValidationMessage = "";
        }

        #endregion Methods
    }
}