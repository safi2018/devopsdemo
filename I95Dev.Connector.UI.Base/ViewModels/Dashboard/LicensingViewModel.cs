using System;
using I95Dev.Connector.Base.Common;
using I95Dev.Connector.Base.Helpers;
using I95Dev.Connector.UI.Base.ViewModels.Base;

namespace I95Dev.Connector.UI.Base.ViewModels.Dashboard
{
    public class LicensingViewModel : BaseViewModel
    {
        private string customerId;
        private string subscriptionKey;
        private DateTime expiryDate;
        private int remainDays;

        public string CustomerId
        {
            get { return customerId; }
            set { SetProperty(ref customerId, value); }
        }

        public string SubscriptionKey
        {
            get { return subscriptionKey; }
            set { SetProperty(ref subscriptionKey, value); }
        }

        public DateTime ExpiryDate
        {
            get { return expiryDate; }
            set { SetProperty(ref expiryDate, value); }
        }

        public int RemainDays
        {
            get { return remainDays; }
            set { SetProperty(ref remainDays, value); }
        }

        public LicensingViewModel()
        {
            CustomerId = ConfigurationHelper.GetConfigurationValue(ConfigurationConstants.CustomerId);
            SubscriptionKey = ConfigurationHelper.GetConfigurationValue(ConfigurationConstants.SubscriptionKey);
            string expiryDateString = ConfigurationHelper.GetConfigurationValue(ConfigurationConstants.ExpiryDate);
            ExpiryDate = !DateTime.TryParse(expiryDateString, out expiryDate) ? DateTime.Now : expiryDate;
            RemainDays = ExpiryDate.Subtract(DateTime.Today).Days;
            if (RemainDays < 0) RemainDays = 0;
        }
    }
}