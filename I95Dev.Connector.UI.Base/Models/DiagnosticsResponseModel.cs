namespace I95Dev.Connector.UI.Base.Models
{
    public class DiagnosticsResponseModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsFailed { get { return !IsSuccess; } }
        public string Color { get { return IsSuccess ? "Green" : "Red"; } }
        public string Comment { get; set; }
    }
}