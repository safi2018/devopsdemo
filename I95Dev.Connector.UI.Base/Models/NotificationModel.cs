using System;

namespace I95Dev.Connector.UI.Base.Models
{
    public class NotificationModel : BaseListModel
    {
        public long Id { get; set; }
        public string CategoryName { get; set; }

        public string Description { get; set; }

        public DateTime CreatedTime { get; set; }

        public string IsMailSent { get; set; }

        public string ViewRecord { get; set; }

        private string detailDescription;

        public string DetailDescription
        {
            get { return detailDescription; }
            set { SetProperty(ref detailDescription, value); }
        }

        public int CategoryId { get; set; }
    }
}