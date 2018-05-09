using System;

namespace I95Dev.Connector.UI.Base.Models
{
    public class MessageReportModel : BaseListModel
    {
        public long MessageId { get; set; }
        public string Reference { get; set; }
        public string EcommerceId { get; set; }
        public string ErpId { get; set; }
        public int SyncCounter { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string MessageStatus { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsCheckBoxEnabled { get; set; }
        public bool ShowActionLink { get; set; }
        public int EntityId { get; set; }
        public bool IsCheckBoxChecked { get; set; }
        public DateTime ReferenceTime { get; internal set; }
        public string EntityName { get; internal set; }
        public short StatusId { get; internal set; }

        public long CloudMessageId { get; set; }
    }
}