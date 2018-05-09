namespace I95Dev.Connector.UI.Base.Models
{
    public class PaymentSettingsModel
    {
        public string GpPaymentName { get; set; }
        public string MagentoPaymentName { get; set; }

        public int GpPaymentId { get; set; }
        public int Id { get; set; }
        public bool IsErpDefault { get; set; }
        public bool IsEcommerceDefault { get; set; }
    }
}