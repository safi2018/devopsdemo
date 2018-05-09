using I95Dev.Connector.Base.Models.UI;

namespace I95Dev.Connector.UI.Base.ViewModels.Helpers
{
    public class ConfigurationSearchModel : BaseModel
    {
        private int? configurationType;
        private string groupName;
        private string configurationKey;

        public int? ConfigurationType
        {
            get { return configurationType; }
            set { SetProperty(ref configurationType, value); }
        }

        public string GroupName
        {
            get { return groupName; }
            set { SetProperty(ref groupName, value); }
        }

        public string ConfigurationKey
        {
            get { return configurationKey; }
            set { SetProperty(ref configurationKey, value); }
        }
    }
}