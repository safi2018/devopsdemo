using System;

namespace I95Dev.Connector.UI.Base.Models
{
    public class ExclusionModel : BaseListModel
    {
        public long Id { get; set; }

        public string EntityName { get; set; }

        public string RecordId { get; set; }

        public DateTime CreatedTime { get; set; }

        public string Parameters { get; set; }

        public bool IsJson { get; set; }
    }
}