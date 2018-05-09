namespace I95Dev.Connector.UI.Base.Models
{
    public class MessageSummaryModel
    {
        public int CompleteCount { get; set; }

        public int PendingCount { get; set; }

        public int ProcessingCount { get; set; }

        public int SuccessCount { get; set; }

        public int ErrorCount { get; set; }

        public int TotalCount { get { return ErrorCount + SuccessCount + ProcessingCount + PendingCount + CompleteCount + ResponseReceivedCount; } }
        public string EntityName { get; set; }

        public int EntityId { get; set; }
        public bool IsEnabled { get; set; }
        public int ResponseReceivedCount { get; set; }
    }
}