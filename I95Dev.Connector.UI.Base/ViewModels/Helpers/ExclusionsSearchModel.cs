using I95Dev.Connector.UI.Base.Models;

namespace I95Dev.Connector.UI.Base.ViewModels.Helpers
{
    public class ExclusionsSearchModel : BaseModel
    {
        private int entityId;

        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        /// <value>
        /// The entity identifier.
        /// </value>
        public int EntityId
        {
            get { return entityId; }
            set { SetProperty(ref entityId, value); }
        }

        private DateModel fromDate;

        /// <summary>
        /// Gets or sets from date.
        /// </summary>
        /// <value>
        /// From date.
        /// </value>
        public DateModel FromDate
        {
            get { return fromDate; }
            set { SetProperty(ref fromDate, value); }
        }

        private DateModel toDate;

        /// <summary>
        /// Gets or sets to date.
        /// </summary>
        /// <value>
        /// To date.
        /// </value>
        public DateModel ToDate
        {
            get { return toDate; }
            set { SetProperty(ref toDate, value); }
        }

        private string recordNumber;

        /// <summary>
        /// Gets or sets the record number.
        /// </summary>
        /// <value>
        /// The record number.
        /// </value>
        public string RecordNumber
        {
            get { return recordNumber; }
            set { SetProperty(ref recordNumber, value); }
        }
    }
}