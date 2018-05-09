namespace I95Dev.Connector.UI.Base.Models
{
    public class ShippingCarrierModel : BaseModel
    {
        private int carrierId;
        private string carrierDescription;
        private string carrierCode;

        public int CarrierId
        {
            get { return carrierId; }
            set { SetProperty(ref carrierId, value); }
        }

        public string CarrierCode
        {
            get { return carrierCode; }
            set { SetProperty(ref carrierCode, value); }
        }

        public string CarrierDescription
        {
            get { return carrierDescription; }
            set { SetProperty(ref carrierDescription, value); }
        }
    }
}