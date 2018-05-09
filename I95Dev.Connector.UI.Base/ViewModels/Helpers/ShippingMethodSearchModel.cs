using I95Dev.Connector.Base.Models.UI;

namespace I95Dev.Connector.UI.Base.ViewModels.Helpers
{
    public class ShippingMethodSearchModel : BaseModel
    {
        private int carrier;
        private string erpShippingMethod;
        private string ecommerceShippingMethod;

        public int CarrierId
        {
            get { return carrier; }
            set { SetProperty(ref carrier, value); }
        }

        public string ErpShippingMethod
        {
            get { return erpShippingMethod; }
            set { SetProperty(ref erpShippingMethod, value); }
        }

        public string EcommerceShippingMethod
        {
            get { return ecommerceShippingMethod; }
            set { SetProperty(ref ecommerceShippingMethod, value); }
        }
    }
}