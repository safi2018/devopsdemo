namespace I95Dev.Connector.UI.Base.Models
{
    public class MessageCountModel
    {
        public int Count { get; internal set; }
        public int EntityId { get; internal set; }
        public string EntityName { get; internal set; }
        public int StatusId { get; internal set; }
        public string StatusName { get; internal set; }
    }
}