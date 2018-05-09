namespace I95Dev.Connector.UI.Base.Models
{
    public class CategoryMappingModel : BaseModel
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private int eventCategory;

        public int EventCategory
        {
            get { return eventCategory; }
            set { SetProperty(ref eventCategory, value); }
        }

        private string exceptionType;

        public string ExceptionType
        {
            get { return exceptionType; }
            set { SetProperty(ref exceptionType, value); }
        }
    }
}