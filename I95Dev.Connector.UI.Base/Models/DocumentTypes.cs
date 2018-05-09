namespace I95Dev.Connector.UI.Base.Models
{
    public class DocumentType : BaseModel
    {
        private int sopType;
        private string erpDocumentId;
        private string orgin;
        private int id;

        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        public int SopType
        {
            get { return sopType; }
            set { SetProperty(ref sopType, value); }
        }

        public string ErpDocumentId
        {
            get { return erpDocumentId; }
            set { SetProperty(ref erpDocumentId, value); }
        }

        public string Orgin
        {
            get { return orgin; }
            set { SetProperty(ref orgin, value); }
        }
    }
}