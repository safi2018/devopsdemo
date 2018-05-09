using I95Dev.Connector.UI.Base.Models;

namespace I95Dev.Connector.UI.Base.ViewModels.Helpers
{
    public class NotificationsSearchModel : BaseModel
    {
        private int categoryId;

        /// <summary>
        /// Gets or sets the category identifier.
        /// </summary>
        /// <value>
        /// The category identifier.
        /// </value>
        public int CategoryId
        {
            get { return categoryId; }
            set { SetProperty(ref categoryId, value); }
        }

        private DateModel fromDate;

        /// <summary>
        /// Gets or sets the scheduler created from.
        /// </summary>
        /// <value>
        /// The scheduler created from.
        /// </value>
        public DateModel FromDate
        {
            get { return fromDate; }
            set { SetProperty(ref fromDate, value); }
        }

        private DateModel toDate;

        /// <summary>
        /// Gets or sets the scheduler created to.
        /// </summary>
        /// <value>
        /// The scheduler created to.
        /// </value>
        public DateModel ToDate
        {
            get { return toDate; }
            set { SetProperty(ref toDate, value); }
        }

        private int mailSent;

        /// <summary>
        /// Gets or sets the mail sent.
        /// </summary>
        /// <value>
        /// The mail sent.
        /// </value>
        public int MailSent
        {
            get { return mailSent; }
            set
            {
                if (!SetProperty(ref mailSent, value)) return;
                OnPropertyChanged("IsYes");
                OnPropertyChanged("IsNo");
                OnPropertyChanged("IsAll");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is yes.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is yes; otherwise, <c>false</c>.
        /// </value>
        public bool IsYes
        {
            get { return MailSent == 1; }
            set
            {
                if (value) MailSent = 1;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is no.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is no; otherwise, <c>false</c>.
        /// </value>
        public bool IsNo
        {
            get { return MailSent == 0; }
            set
            {
                if (value) MailSent = 0;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is all.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is all; otherwise, <c>false</c>.
        /// </value>
        public bool IsAll
        {
            get { return MailSent == 2; }
            set
            {
                if (value) MailSent = 2;
            }
        }
    }
}