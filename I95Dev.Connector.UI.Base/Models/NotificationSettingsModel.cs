using I95Dev.Connector.Base.Helpers;

namespace I95Dev.Connector.UI.Base.Models
{
    public class NotificationSettingsModel : BaseModel
    {
        public string ConfirmPassword { get; set; }
        public bool EnableSsl { get; set; }
        public string FromMail { get; set; }
        public int Id { get; set; }
        public NotificationFrequency NotificationFrequency { get; set; }
        public bool NotificationsEnabled { get; set; }

        public string Password { get; set; }
        public int PortNumber { get; set; }
        public string SenderName { get; set; }
        public string SmtpServer { get; set; }
        public string ToMail { get; set; }
        public bool UserAuthentication { get; set; }
        public string UserName { get; set; }

        public string OldPassword { get; set; }
    }
}