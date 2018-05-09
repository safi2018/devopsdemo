using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Input;
using I95Dev.Connector.Base.Common;
using I95Dev.Connector.Base.Helpers;
using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Helpers.Controls;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.Services;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using Utilities = I95Dev.Connector.UI.Base.Helpers.Utilities;

namespace I95Dev.Connector.UI.Base.ViewModels.Admin
{
    public class DiagnosticsViewModel : BaseViewModel
    {
        private readonly string validationId;

        #region Properties

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public DiagnosticsModel Data { get; set; }

        public IList<StringComboBoxModel> Databases { get; private set; }

        public IList<IntegerComboBoxModel> AuthenticationType { get; private set; }

        public ObservableRangeCollection<DiagnosticsResponseModel> Responses { get; private set; }

        #endregion Properties

        public DiagnosticsViewModel()
        {
            validationId = Guid.NewGuid().ToString();
            LoadDefaultData();
            ValidateCommand = new BaseCommand(Validate);
            ClearCommand = new BaseCommand(Clear);
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("DefaultStatus");
        }

        public ICommand ValidateCommand { get; private set; }

        public ICommand ClearCommand { get; private set; }

        /// <summary>
        /// Loads the default data.
        /// </summary>
        private void LoadDefaultData()
        {
            LoadDatabaseTypes();

            Responses = new ObservableRangeCollection<DiagnosticsResponseModel>();

            SetDefaultData();

            if (Data == null)
            {
                Data = new DiagnosticsModel
                {
                    AuthenticationType = 0
                };
            }
        }

        private void SetDefaultData()
        {
            Data = new DiagnosticsModel();
            var builder = new SqlConnectionStringBuilder(Constants.ErpDatabase);
            Data.ServerIp = builder.DataSource;
            Data.DatabaseUserName = builder.UserID;
            Data.DatabaseUserPassword = builder.Password;

            Data.IsDatabasesVisible = BindDatabases();
            Data.SelectedDatabase = builder.InitialCatalog;
            Data.NotifyChange("SelectedDatabase");
            Data.ServiceUrl = ConfigurationHelper.GetConfigurationValue(ConfigurationConstants.ServiceUrl);
            Validate();
        }

        private void LoadDatabaseTypes()
        {
            AuthenticationType = new List<IntegerComboBoxModel>{
                new IntegerComboBoxModel{Name = "Windows Authentication", Value = 0 },
                new IntegerComboBoxModel{Name = "Sql server Authentication", Value = 1 }
            };
        }

        private void Validate()
        {
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("Loading");
            Data.ValidationMessage = string.Empty;
            Data.IsResultsVisible = false;

            if (Data.ValidateSqlDetails())
            {
                if (!Data.IsDatabasesVisible)
                {
                    Data.IsDatabasesVisible = BindDatabases();
                    OnPropertyChanged("Databases");
                    Data.SelectedDatabase = "0";
                }
                else if (Data.IsDatabasesVisible)
                {
                    Data.UpdateConnectionString();
                    Responses.Clear();

                    Responses.Add(DiagnosticsService.CheckSqlPermission(Data));
                    Responses.Add(DiagnosticsService.CheckGPeConnect(Data));
                    Responses.Add(DiagnosticsService.CheckWebServicesRunning(Data));
                    Responses.Add(DiagnosticsService.CheckCloudIsRunning(Data));
                    Responses.Add(DiagnosticsService.CheckSubscription(Data));
                    //Responses.Add(DiagnosticsService.CheckMagentoIsRunning(Data));
                    //Responses.Add(DiagnosticsService.CheckMagentoConnectorIsRunning(Data));

                    Data.IsResultsVisible = true;
                    DiagnosticsService.SaveToDatabase(Responses, validationId);
                }
            }
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("DefaultStatus");
        }

        private void Clear()
        {
            Data = new DiagnosticsModel
            {
                AuthenticationType = 0
            };
            OnPropertyChanged("Data");
        }

        /// <summary>
        /// Binds the databases.
        /// </summary>
        private bool BindDatabases()
        {
            try
            {
                using (var sqlConnection = new SqlConnection(Data.ConnectionString))
                {
                    const string commandText = @"Use DYNAMICS SELECT CMPNYNAM,INTERID FROM SY01500";

                    using (var command = new SqlCommand(commandText, sqlConnection))
                    {
                        sqlConnection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Databases = new List<StringComboBoxModel>
                            {
                                new StringComboBoxModel{ Name="Please select Company", Value="0" }
                            };
                            while (reader.Read())
                            {
                                string databaseName = reader.GetString(0).TrimEnd();
                                string databaseValue = reader.GetString(1).TrimEnd();
                                Databases.Add(new StringComboBoxModel { Name = databaseName, Value = databaseValue });
                            }

                            return true;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                NotificationHelper.ShowMessage("Sql Authentication Error: \n" + exception.Message, "Error");
            }
            return false;
        }
    }
}