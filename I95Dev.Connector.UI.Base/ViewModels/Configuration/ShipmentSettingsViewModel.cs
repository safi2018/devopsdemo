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
using I95Dev.Connector.UI.Base.ViewModels.Helpers;

namespace I95Dev.Connector.UI.Base.ViewModels.Configuration
{
    public class ShipmentSettingsViewModel : BaseViewModel
    {
        #region Properties

        private readonly IShippingMethodDataAccess localSql;
        private bool showPopup;
        private string validationMessage;
        private ShippingMethodSearchModel searchModel;
        private ShipmentSettingsModel shippingMethod;
        private IList<StringComboBoxModel> shippingMethods;

        public ObservableRangeCollection<ShipmentSettingsModel> Responses { get; }

        public ObservableRangeCollection<IntegerComboBoxModel> Carriers { get; }

        public ObservableRangeCollection<IntegerComboBoxModel> SearchCarriers { get; }

        public IList<StringComboBoxModel> ShippingMethods
        {
            get { return shippingMethods; }
            private set { SetProperty(ref shippingMethods, value); }
        }

        public string ValidationMessage
        {
            get { return validationMessage; }
            set { SetProperty(ref validationMessage, value); }
        }

        public bool ShowPopup
        {
            get { return showPopup; }
            set
            {
                if (SetProperty(ref showPopup, value))
                {
                    StatusUpdate.IsPopUpOpen = value;
                }
            }
        }

        public ShippingMethodSearchModel SearchModel
        {
            get { return searchModel; }
            set { SetProperty(ref searchModel, value); }
        }

        public ShipmentSettingsModel ShippingMethod
        {
            get { return shippingMethod; }
            set { SetProperty(ref shippingMethod, value); }
        }

        #endregion Properties

        public ShipmentSettingsViewModel()
        {
            localSql = new ShippingMethodDataAccess();

            bool result = localSql.IsShippingMethodTableExists();
            if (!result)
            {
                NotificationHelper.ShowMessage("ShippingMethod table not found", Utilities.GetResourceValue("CaptionError"));
                return;
            }

            Responses = new ObservableRangeCollection<ShipmentSettingsModel>();
            Carriers = new ObservableRangeCollection<IntegerComboBoxModel>();
            SearchCarriers = new ObservableRangeCollection<IntegerComboBoxModel>();
            SearchModel = new ShippingMethodSearchModel();
            ShippingMethods = new List<StringComboBoxModel>();

            BindData();

            AddNewShipmentCommand = new BaseCommand(AddNewShipmentRecord);
            SaveShipmentCommand = new BaseCommand(SaveShipments);
            ClosePopupCommand = new BaseCommand(ClosePopup);
            ViewRecordCommand = new BaseCommand(ViewRecord);
            FindCommand = new BaseCommand(FindRecords);
            ResetCommand = new BaseCommand(Reset);
        }

        #region Commands

        public ICommand AddNewShipmentCommand { get; }

        private void AddNewShipmentRecord()
        {
            ShippingMethod = new ShipmentSettingsModel();
            ValidationMessage = "";
            ShowPopup = true;
        }

        public ICommand SaveShipmentCommand { get; }

        private void SaveShipments()
        {
            try
            {
                if (!ValidateData()) return;
                if (localSql.IsShippingMethodExists(ShippingMethod))
                {
                    ValidationMessage = "ShipmentMethod Combination Already Exist";
                    return;
                }

                bool result = localSql.SaveShipmentDetails(ShippingMethod);
                if (result)
                {
                    ShowPopup = false;
                    NotificationHelper.ShowMessage("Saved Successfully", Utilities.GetResourceValue("CaptionInfo"));
                    BindShippingMethods();
                    ValidationMessage = "";
                }
                else
                {
                    ValidationMessage = "Unknown error occured. Try again after some time";
                }
            }
            catch (Exception exception)
            {
                ValidationMessage = exception.Message;
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
            if (!(arg is ShipmentSettingsModel model)) return;

            ShippingMethod = model;
            ValidationMessage = "";
            ShowPopup = true;
        }

        public ICommand FindCommand { get; }

        private void FindRecords()
        {
            BindShippingMethods();
        }

        public ICommand ResetCommand { get; }

        private void Reset()
        {
            SearchModel = new ShippingMethodSearchModel();
            BindShippingMethods();
        }

        #endregion Commands

        #region Methods

        private void BindData()
        {
            BindCarriers();
            BindGpShippingMethods();
            BindShippingMethods();
        }

        private void BindGpShippingMethods()
        {
            ShippingMethods = localSql.GetGpShippingMethods();
        }

        private void BindShippingMethods()
        {
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("Loading");
            Responses.Clear();
            IList<ShipmentSettingsModel> records = localSql.GetShipmentMethodsList(SearchModel);
            if (!records.Any()) return;
            Responses.AddRange(records);
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("DefaultStatus");
        }

        private void BindCarriers()
        {
            Carriers.Clear();
            SearchCarriers.Clear();
            IList<IntegerComboBoxModel> entity = PrepareComboBoxEntity(localSql.GetShipmentCarriers());
            if (!entity.Any()) return;

            SearchCarriers.AddRange(entity);
            Carriers.AddRange(entity);

            SearchCarriers.Insert(0, new IntegerComboBoxModel { Value = 0, Name = "ALL" });
        }

        private static IList<IntegerComboBoxModel> PrepareComboBoxEntity(IEnumerable<ShippingCarrierModel> getShipmentCarriers)
        {
            return getShipmentCarriers.Select(c => new IntegerComboBoxModel { Name = c.CarrierDescription, Value = c.CarrierId }).ToList();
        }

        private bool ValidateData()
        {
            ValidationMessage = "All fields required";
            if (ShippingMethod.CarrierId <= 0) return false;
            if (string.IsNullOrEmpty(ShippingMethod.GpShipmentId)) return false;
            if (string.IsNullOrEmpty(ShippingMethod.MagentoShipmentId)) return false;
            if (string.IsNullOrEmpty(ShippingMethod.Description)) return false;

            return ValidateInSystems();
        }

        private bool ValidateInSystems()
        {
            if (!localSql.IsShippingMethodExistsInGp(ShippingMethod))
            {
                ValidationMessage = ShippingMethod.GpShipmentId + " not existing in GP";
                return false;
            }

            //TODO Need Check in Magento also
            return true;
        }

        #endregion Methods
    }
}