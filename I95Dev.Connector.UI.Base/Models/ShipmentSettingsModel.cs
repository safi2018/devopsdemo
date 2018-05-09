using I95Dev.Connector.Base.Common;

namespace I95Dev.Connector.UI.Base.Models
{
    public class ShipmentSettingsModel : BaseModel
    {
        private int shipmentMethodId;
        private string gpShipmentId;
        private string magentoShipmentId;
        private string description;
        private string carrierName;
        private int carrierId;
        private bool isEcommerceDefault;
        private bool isErpDefault;

        public int CarrierId
        {
            get { return carrierId; }
            set { SetProperty(ref carrierId, value); }
        }

        public string CarrierName
        {
            get { return carrierName; }
            set { SetProperty(ref carrierName, value); }
        }

        public int ShipmentMethodId
        {
            get { return shipmentMethodId; }
            set { SetProperty(ref shipmentMethodId, value); }
        }

        public string GpShipmentId
        {
            get { return gpShipmentId?.ToUpper(Constants.DefaultCulture); }
            set { SetProperty(ref gpShipmentId, value); }
        }

        public string MagentoShipmentId
        {
            get { return magentoShipmentId; }
            set { SetProperty(ref magentoShipmentId, value); }
        }

        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        public bool IsEcommerceDefault
        {
            get { return isEcommerceDefault; }
            set { SetProperty(ref isEcommerceDefault, value); }
        }

        public bool IsErpDefault
        {
            get { return isErpDefault; }
            set { SetProperty(ref isErpDefault, value); }
        }
    }
}