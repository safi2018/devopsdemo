namespace I95Dev.Connector.UI.Base.ViewModels.Helpers
{
    public class MessageSearchModel : DateSearchModel
    {
        private int statusId;

        /// <summary>
        /// Gets or sets the status identifier.
        /// </summary>
        /// <value>
        /// The status identifier.
        /// </value>
        public int StatusId
        {
            get { return statusId; }
            set { SetProperty(ref statusId, value); }
        }

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
            set { DataModified = SetProperty(ref entityId, value); }
        }

        private string erpId;

        /// <summary>
        /// Gets or sets the erp identifier.
        /// </summary>
        /// <value>
        /// The erp identifier.
        /// </value>
        public string ErpId
        {
            get { return erpId; }
            set { SetProperty(ref erpId, value); }
        }

        private string ecommerceId;

        /// <summary>
        /// Gets or sets the ecommerce identifier.
        /// </summary>
        /// <value>
        /// The ecommerce identifier.
        /// </value>
        public string EcommerceId
        {
            get { return ecommerceId; }
            set { SetProperty(ref ecommerceId, value); }
        }

        private int? syncCounter;

        /// <summary>
        /// Gets or sets the synchronize counter.
        /// </summary>
        /// <value>
        /// The synchronize counter.
        /// </value>
        public int? SyncCounter
        {
            get { return syncCounter; }
            set { SetProperty(ref syncCounter, value); }
        }

        private string reference;

        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        /// <value>
        /// The reference.
        /// </value>
        public string Reference
        {
            get { return reference; }
            set { SetProperty(ref reference, value); }
        }

        private string messageId;

        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public string MessageId
        {
            get { return messageId; }
            set { SetProperty(ref messageId, value); }
        }
    }
}