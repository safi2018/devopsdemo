using System;
using System.Linq;
using System.Windows.Input;
using I95Dev.Connector.Base.Helpers;
using I95Dev.Connector.Base.Models;
using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Helpers.Controls;
using I95Dev.Connector.UI.Base.Services;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using I95Dev.Connector.UI.Base.ViewModels.Helpers;
using Utilities = I95Dev.Connector.UI.Base.Helpers.Utilities;

namespace I95Dev.Connector.UI.Base.ViewModels.Configuration
{
    public class ConfigurationSettingsViewModel : BaseViewModel
    {
        public HomeViewModel HomeViewModel { get; }

        private ConfigurationSearchModel searchModel;
        private ConfigurationModel setting;
        private bool showPopup;
        private string validationMessage;

        #region Properties

        /// <summary>
        /// Gets or sets the setting.
        /// </summary>
        /// <value>
        /// The setting.
        /// </value>
        public ConfigurationSearchModel SearchModel
        {
            get { return searchModel; }
            set { SetProperty(ref searchModel, value); }
        }

        /// <summary>
        /// Gets or sets the setting.
        /// </summary>
        /// <value>
        /// The setting.
        /// </value>
        public ConfigurationModel Setting
        {
            get { return setting; }
            set { SetProperty(ref setting, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show popup].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show popup]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowPopup
        {
            get { return showPopup; }
            set
            {
                if (SetProperty(ref showPopup, value))
                {
                    HomeViewModel.IsPopupOpen = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the validation message.
        /// </summary>
        /// <value>
        /// The validation message.
        /// </value>
        public string ValidationMessage
        {
            get { return validationMessage; }
            set { SetProperty(ref validationMessage, value); }
        }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public ObservableRangeCollection<ConfigurationModel> Settings { get; }

        #endregion Properties

        private ConfigurationSettingsViewModel()
        {
            SearchModel = new ConfigurationSearchModel();
            Settings = new ObservableRangeCollection<ConfigurationModel>();
            LoadConfigurations(true);

            ResetCommand = new BaseCommand(Reset);
            FindCommand = new BaseCommand(Find);
            ClosePopupCommand = new BaseCommand(ClosePopup);
            UpdateCommand = new BaseCommand(Update);
            RowSelectedCommand = new BaseCommand(RowSelected);
        }

        public ConfigurationSettingsViewModel(HomeViewModel homeViewModel) : this()
        {
            this.HomeViewModel = homeViewModel;
        }

        #region Commands

        public ICommand ResetCommand { get; }

        private void Reset()
        {
            SearchModel = new ConfigurationSearchModel();
            LoadConfigurations(true);
        }

        public ICommand FindCommand { get; }

        private void Find()
        {
            LoadConfigurations(true);
        }

        public ICommand ClosePopupCommand { get; }

        private void ClosePopup()
        {
            Setting = null;
            ShowPopup = false;
        }

        public ICommand UpdateCommand { get; }

        private void Update()
        {
            ValidationMessage = "";
            if (string.IsNullOrEmpty(setting.ConfigurationValue))
            {
                ValidationMessage = "Value is required";
                return;
            }
            ConfigurationHelper.SetConfigurationValue(setting.ConfigurationType, setting.GroupName, setting.ConfigurationKey, setting.ConfigurationValue);
            LoadConfigurations(false);
            ShowPopup = false;
        }

        public ICommand RowSelectedCommand { get; }

        private void RowSelected(object obj)
        {
            var model = obj as ConfigurationModel;
            if (model is null) return;

            Setting = model;
            ShowPopup = true;
        }

        #endregion Commands

        /// <summary>
        /// Loads the configurations.
        /// </summary>
        private void LoadConfigurations(bool isRefreshData)
        {
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("Loading");
            try
            {
                if (isRefreshData) ConfigurationHelper.FillAllConfigurations();
                Settings.Clear();

                IQueryable<ConfigurationModel> query = ConfigurationHelper.Keys.AsQueryable();
                if (SearchModel.ConfigurationType.HasValue) query = query.Where(c1 => c1.ConfigurationType == SearchModel.ConfigurationType);
                if (!string.IsNullOrEmpty(SearchModel.GroupName)) query = query.Where(c1 => c1.GroupName.ToLower().Contains(SearchModel.GroupName.ToLower()));
                if (!string.IsNullOrEmpty(SearchModel.ConfigurationKey)) query = query.Where(c1 => c1.ConfigurationKey.ToLower().Contains(SearchModel.ConfigurationKey.ToLower()));

                Settings.AddRange(query.OrderBy(c1 => c1.ConfigurationType).ThenBy(c1 => c1.GroupName).ThenBy(c1 => c1.ConfigurationKey).ToList());
            }
            catch (Exception exception)
            {
                NotificationHelper.ShowMessage(exception.Message, Utilities.GetResourceValue("CaptionError"));
            }
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("DefaultStatus");
        }
    }
}