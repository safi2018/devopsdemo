using System;
using System.Windows.Input;
using System.Windows.Threading;
using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Services;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using I95Dev.Connector.UI.Base.ViewModels.Dashboard;
using I95Dev.Connector.UI.Base.ViewModels.LogFile;
using I95Dev.Connector.UI.Base.ViewModels.Share;
using MvvmDialogs;

namespace I95Dev.Connector.UI.Base.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        #region Properties

        private DispatcherTimer dispatcherTimer;

        private string statusMessage;

        /// <summary>
        /// Gets or sets the status message.
        /// </summary>
        /// <value>
        /// The status message.
        /// </value>
        public string StatusMessage
        {
            get
            {
                return statusMessage;
            }
            set
            {
                SetProperty(ref statusMessage, value);

                IsPopupOpen = ShowProgress = true;
                if (value == Utilities.GetResourceValue("DefaultStatus"))
                {
                    IsPopupOpen = ShowProgress = false;
                }
            }
        }

        private BaseViewModel element;

        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        /// <value>
        /// The element.
        /// </value>
        public BaseViewModel Element
        {
            get
            {
                return element;
            }
            set
            {
                SetProperty(ref element, value);
            }
        }

        private bool showProgress;

        /// <summary>
        /// Gets or sets a value indicating whether to show the progress window or not.
        /// </summary>
        /// <value>
        /// Returns <c>true</c> if [show progress]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowProgress
        {
            get
            {
                return showProgress;
            }
            set
            {
                SetProperty(ref showProgress, value);
            }
        }

        private bool showAdminMenus;

        /// <summary>
        /// Gets or sets a value indicating whether [show admin menus].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show admin menus]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowAdminMenus
        {
            get
            {
                return showAdminMenus;
            }
            set
            {
                SetProperty(ref showAdminMenus, value);
            }
        }

        private bool isPopupOpen;

        public bool IsPopupOpen
        {
            get { return isPopupOpen; }
            set
            {
                SetProperty(ref isPopupOpen, value);
            }
        }

        #endregion Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class.
        /// </summary>
        public HomeViewModel()
        {
            ManageSchedulers.SetLogFileName(0);
            MenuClickCommand = new BaseCommand(MenuClick);
            InputCommand = new BaseCommand(UserInput);
            StatusUpdate.StatusChanged += StatusUpdate_StatusChanged;
            StatusUpdate.PopUpStatusChanged += (isPopUpOpen) => { IsPopupOpen = isPopUpOpen; };

            StatusUpdate.StatusMessage = Utilities.GetResourceValue("Loading");
            ThreadHelper.RunBackGround(delegate
            {
                Element = new StartupViewModel(this);
            });
        }

        #region Commands

        /// <summary>
        /// Gets the menu click command.
        /// </summary>
        /// <value>
        /// The menu click command.
        /// </value>
        public ICommand MenuClickCommand { get; private set; }

        /// <summary>
        /// Menus the click.
        /// </summary>
        /// <param name="arg">The argument.</param>
        private void MenuClick(object arg)
        {
            StatusMessage = Utilities.GetResourceValue("Loading");
            switch ((string)arg)
            {
                case "Dashboard":
                    Element = new StartupViewModel(this);
                    break;

                case "EcommerceDashboard":
                    Element = new Dashboard.Ecommerce.EcommerceViewModel();
                    break;

                case "EcommerceDashboard1":
                    Element = new Dashboard.Ecommerce.EcommerceViewModel(1);
                    break;

                case "ErpDashBoard":
                    Element = new Dashboard.Erp.ErpViewModel();
                    break;

                case "ErpDashBoard1":
                    Element = new Dashboard.Erp.ErpViewModel(1);
                    break;

                case "SchedulerControl":
                    Element = new Schedulers.SchedulersViewModel();
                    break;

                case "Logfiles":
                    Element = new LogFileViewModel();
                    break;

                case "ConfigurationSettings":
                    Element = new Configuration.ConfigurationSettingsViewModel(this);
                    break;

                case "NotificationSettings":
                    Element = new Configuration.NotificationViewModel();
                    break;

                case "ShipmentSettings":
                    Element = new Configuration.ShipmentSettingsViewModel();
                    break;

                case "CarrierSettings":
                    Element = new Configuration.ShippingCarrierViewModel();
                    break;

                case "PaymentSettings":
                    Element = new Configuration.PaymentSettingsViewModel();
                    break;

                case "DocumentTypes":
                    Element = new Configuration.DocumentTypesViewModel();
                    break;

                case "NotificationReports":
                    Element = new Reports.ReportViewModel();
                    break;

                case "ExclusionReports":
                    Element = new Reports.ReportViewModel(1);
                    break;

                case "AdminConfig":
                    Element = new Admin.ConfigurationViewModel();
                    break;

                case "CategoryMaster":
                    Element = new Admin.CategoryMasterViewModel();
                    break;

                case "CategoryMapping":
                    Element = new Admin.CategoryMappingViewModel();
                    break;

                case "Diagnostics":
                    Element = new Admin.DiagnosticsViewModel();
                    break;

                case "Blank":
                    Element = new BlankViewModel();
                    break;
            }
            StatusMessage = Utilities.GetResourceValue("DefaultStatus");
        }

        /// <summary>
        /// Gets the input command.
        /// </summary>
        /// <value>
        /// The input command.
        /// </value>
        public ICommand InputCommand { get; private set; }

        /// <summary>
        /// Users the input.
        /// </summary>
        private void UserInput()
        {
            ShowMenu();
        }

        #endregion Commands

        #region Methods

        /// <summary>
        /// Statuses the update status changed.
        /// </summary>
        /// <param name="message">The message.</param>
        private void StatusUpdate_StatusChanged(string message)
        {
            StatusMessage = message;
        }

        /// <summary>
        /// Shows the menu.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ShowMenu()
        {
            var viewModel = new AuthenticationViewModel();
            IDialogService dialogService = Utilities.CreateDialogServiceInstance();
            dialogService.ShowDialog(this, viewModel);
            if (!viewModel.IsAuthenticated) return;
            ShowAdminMenus = true;
            dispatcherTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 1, 0),
                IsEnabled = true
            };
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();
        }

        #endregion Methods

        #region Events

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            ShowAdminMenus = false;
            dispatcherTimer.Stop();
        }

        #endregion Events
    }
}