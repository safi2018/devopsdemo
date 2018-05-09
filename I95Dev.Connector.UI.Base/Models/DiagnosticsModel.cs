using System.Data.SqlClient;

namespace I95Dev.Connector.UI.Base.Models
{
    public class DiagnosticsModel : BaseModel
    {
        internal string ConnectionString = string.Empty;

        private int authenticationType;

        public int AuthenticationType
        {
            get { return authenticationType; }
            set
            {
                if (SetProperty(ref authenticationType, value))
                {
                    IsSqlLogOnVisible = value == 1;
                    IsResultsVisible = IsDatabasesVisible = false;
                }
            }
        }

        private string serverIp;

        public string ServerIp
        {
            get { return serverIp; }
            set
            {
                if (SetProperty(ref serverIp, value))
                {
                    UpdateConnectionString();
                }
            }
        }

        private string databaseUserName;

        public string DatabaseUserName
        {
            get { return databaseUserName; }
            set { SetProperty(ref databaseUserName, value); }
        }

        private string databaseUserPassword;

        public string DatabaseUserPassword
        {
            get { return databaseUserPassword; }
            set
            {
                if (SetProperty(ref databaseUserPassword, value))
                {
                    UpdateConnectionString();
                }
            }
        }

        private string selectedDatabase;

        public string SelectedDatabase
        {
            get { return selectedDatabase; }
            set
            {
                if (!SetProperty(ref selectedDatabase, value)) return;
                if (value != "0" & !string.IsNullOrEmpty(value))
                {
                    UpdateConnectionString();
                }
                else
                {
                    IsResultsVisible = false;
                }
            }
        }

        private string gpVersion;

        public string GPVersion
        {
            get { return gpVersion; }
            set { SetProperty(ref gpVersion, value); }
        }

        private string magentoUrl;

        public string ServiceUrl
        {
            get { return magentoUrl; }
            set { SetProperty(ref magentoUrl, value); }
        }

        private string magentoUserName;

        public string MagentoUserName
        {
            get { return magentoUserName; }
            set { SetProperty(ref magentoUserName, value); }
        }

        private string magentoPassword;

        public string MagentoPassword
        {
            get { return magentoPassword; }
            set { SetProperty(ref magentoPassword, value); }
        }

        private string validationMessage;

        public string ValidationMessage
        {
            get { return validationMessage; }
            set { SetProperty(ref validationMessage, value); }
        }

        #region Visibility

        private bool isSqlLogOnVisible;

        public bool IsSqlLogOnVisible
        {
            get { return isSqlLogOnVisible; }
            set { SetProperty(ref isSqlLogOnVisible, value); }
        }

        private bool isDatabasesVisible;

        public bool IsDatabasesVisible
        {
            get { return isDatabasesVisible; }
            set { SetProperty(ref isDatabasesVisible, value); }
        }

        private bool isResultsVisible;

        public bool IsResultsVisible
        {
            get { return isResultsVisible; }
            set { SetProperty(ref isResultsVisible, value); }
        }

        #endregion Visibility

        /// <summary>
        /// Determines whether data is valid.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if data is valid; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsDataValid()
        {
            ValidationMessage = "All fields are mandatory";

            if (string.IsNullOrEmpty(ServerIp)) return false;
            if (IsSqlLogOnVisible)
            {
                if (string.IsNullOrEmpty(DatabaseUserName)) return false;
                if (string.IsNullOrEmpty(DatabaseUserPassword)) return false;
            }

            if (IsDatabasesVisible)
            {
                if (SelectedDatabase == "0") return false;
                if (string.IsNullOrEmpty(ServiceUrl)) return false;
                //else if (string.IsNullOrEmpty(MagentoUserName)) return false;
                //else if (string.IsNullOrEmpty(MagentoPassword)) return false;
            }
            ValidationMessage = "";
            return true;
        }

        internal void UpdateConnectionString()
        {
            var builder = new SqlConnectionStringBuilder(ConnectionString)
            {
                ConnectTimeout = 30,
                PersistSecurityInfo = false,
                PacketSize = 4096,
                DataSource = ServerIp,
                IntegratedSecurity = false
            };
            if (authenticationType == 0)
            {
                builder.IntegratedSecurity = true;
            }
            else if (!string.IsNullOrEmpty(DatabaseUserName) & !string.IsNullOrEmpty(DatabaseUserPassword))
            {
                builder.UserID = DatabaseUserName;
                builder.Password = DatabaseUserPassword;
            }
            if (!string.IsNullOrEmpty(SelectedDatabase) && SelectedDatabase != "0") builder.InitialCatalog = SelectedDatabase;
            ConnectionString = builder.ConnectionString;
        }

        internal bool ValidateSqlDetails()
        {
            if (!IsDataValid()) return false;
            UpdateConnectionString();
            return true;
        }

        /// <summary>
        /// Notifies the change.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        internal void NotifyChange(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
    }
}