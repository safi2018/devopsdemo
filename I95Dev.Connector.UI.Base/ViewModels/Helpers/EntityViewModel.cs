using System;
using I95Dev.Connector.Base.Models.UI;

namespace I95Dev.Connector.UI.Base.ViewModels.Helpers
{
    public class EntityViewModel : BaseModel
    {
        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public bool IsInboundActive { get; set; }

        public bool IsOutboundActive { get; set; }

        public bool IsChecked { get; set; }

        public DateTime? LastSyncTime { get; set; }
    }
}