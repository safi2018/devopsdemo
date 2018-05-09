using System;
using System.Windows.Input;
using I95Dev.Connector.Base.Common;
using I95Dev.Connector.Base.DataAccess.EventLogger;
using I95Dev.Connector.Base.Helpers;
using I95Dev.Connector.Base.Models.EventLogger;
using I95Dev.Connector.Base.Services.EventLogger;
using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.Services;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using Utilities = I95Dev.Connector.UI.Base.Helpers.Utilities;

namespace I95Dev.Connector.UI.Base.ViewModels.Configuration
{
    public class NotificationViewModel : BaseViewModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public NotificationSettingsModel Settings { get; set; }

        private string validationMessage;

        /// <summary>
        /// Gets or sets the validation message.
        /// </summary>
        /// <value>
        /// The validation message.
        /// </value>
        public string ValidationMessage
        {
            get
            {
                return validationMessage;
            }
            set
            {
                if (validationMessage == value) return;
                validationMessage = value;
                OnPropertyChanged("ValidationMessage");
            }
        }

        #endregion Properties

        public NotificationViewModel()
        {
            Settings = new NotificationSettingsModel
            {
                NotificationFrequency = NotificationFrequency.Immediate,
                PortNumber = 25,
                FromMail = "support@i95dev.com",
                SenderName = "i95Dev Support"
            };
            LoadData();

            TestSettingsCommand = new BaseCommand(CanSubmit, TestMail);
            UpdateSettingsCommand = new BaseCommand(CanSubmit, UpdateData);
        }

        #region Commands

        /// <summary>
        /// Gets the test settings command.
        /// </summary>
        /// <value>
        /// The test settings command.
        /// </value>
        public ICommand TestSettingsCommand { get; private set; }

        /// <summary>
        /// Gets the update settings command.
        /// </summary>
        /// <value>
        /// The update settings command.
        /// </value>
        public ICommand UpdateSettingsCommand { get; private set; }

        /// <summary>
        /// Updates the data.
        /// </summary>
        private void UpdateData()
        {
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("Loading");
            try
            {
                if (ValidateData())
                {
                    SmtpDetail mailDetails = FillData(null);
                    int result = SmtpDataAccess.UpdateSmtpDetails(mailDetails);
                    if (result > 0)
                    {
                        NotificationHelper.ShowMessage(Utilities.GetResourceValue("DataUpdateSuccess"), Utilities.GetResourceValue("CaptionInfo"));
                        LoadData();
                    }
                    else
                    {
                        NotificationHelper.ShowMessage(Utilities.GetResourceValue("NothingUpdated"), Utilities.GetResourceValue("CaptionWarning"));
                    }
                }
            }
            catch (Exception exception)
            {
                string message = exception.Message;
                if (exception.InnerException != null)
                {
                    message += Environment.NewLine + exception.InnerException.Message;
                }
                NotificationHelper.ShowMessage(message, Utilities.GetResourceValue("CaptionError"));
            }
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("DefaultStatus");
        }

        /// <summary>
        /// Tests the mail.
        /// </summary>
        private void TestMail()
        {
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("Loading");
            if (ValidateData())
            {
                bool result = SendTestMail(FillData(null));
                if (result)
                {
                    NotificationHelper.ShowMessage(Utilities.GetResourceValue("TestMailSuccessful"), Utilities.GetResourceValue("CaptionInfo"));
                }
            }
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("DefaultStatus");
        }

        /// <summary>
        /// Determines whether this instance can submit.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can submit; otherwise, <c>false</c>.
        /// </returns>
        private bool CanSubmit()
        {
            bool result = ValidateData();
            return result;
        }

        #endregion Commands

        #region Methods

        /// <summary>
        /// Loads the data.
        /// </summary>
        private void LoadData()
        {
            SmtpDetail data = SmtpDataAccess.GetSmtpDetails();
            if (data != null)
            {
                LoadData(data);
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="data">The data.</param>
        private void LoadData(SmtpDetail data)
        {
            Settings.EnableSsl = data.EnableSsl;
            Settings.FromMail = data.FromMail;
            Settings.Id = data.Id;
            Settings.NotificationFrequency = (NotificationFrequency)Enum.Parse(typeof(NotificationFrequency), data.NotificationFrequency.ToString(Constants.DefaultCulture));
            Settings.NotificationsEnabled = data.NotificationsEnabled;
            Settings.ConfirmPassword = Settings.OldPassword = Settings.Password = data.Password;
            Settings.PortNumber = data.PortNumber;
            if (!string.IsNullOrEmpty(data.SenderName))
                Settings.SenderName = data.SenderName;
            Settings.SmtpServer = data.SmtpServer;
            Settings.ToMail = data.ToMail;
            Settings.UserAuthentication = data.UserAuthentication == 1;
            Settings.UserName = data.UserName;
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private SmtpDetail FillData(SmtpDetail data)
        {
            if (data == null)
            {
                data = new SmtpDetail();
            }

            data.EnableSsl = Settings.EnableSsl;
            data.FromMail = Settings.FromMail;
            data.Id = Settings.Id;
            data.NotificationFrequency = (byte)Settings.NotificationFrequency;
            data.NotificationsEnabled = Settings.NotificationsEnabled;
            data.PortNumber = Settings.PortNumber;
            data.SenderName = Settings.SenderName;
            data.SmtpServer = Settings.SmtpServer;
            data.ToMail = Settings.ToMail;
            data.UserAuthentication = Settings.UserAuthentication ? 1 : 0;
            data.UserName = Settings.UserName;
            if (Settings.OldPassword != Settings.Password)
            {
                data.SetPassword(Settings.Password);
            }
            data.ModifiedTime = Constants.CurrentTime;

            return data;
        }

        /// <summary>
        /// Validates the form input data.
        /// </summary>
        /// <returns></returns>
        private bool ValidateData()
        {
            ValidationMessage = "All fields are mandatory";
            if (string.IsNullOrEmpty(Settings.SmtpServer))
            {
                return false;
            }
            if (Settings.PortNumber <= 0)
            {
                return false;
            }
            if (Settings.UserAuthentication)
            {
                if (string.IsNullOrEmpty(Settings.UserName))
                {
                    return false;
                }
                if (string.IsNullOrEmpty(Settings.Password))
                {
                    return false;
                }
                if (string.IsNullOrEmpty(Settings.ConfirmPassword))
                {
                    return false;
                }
                if (string.Compare(Settings.Password, Settings.ConfirmPassword, StringComparison.Ordinal) != 0)
                {
                    ValidationMessage = "Passwords not matching";
                    return false;
                }
            }
            if (string.IsNullOrEmpty(Settings.ToMail))
            {
                return false;
            }
            if (Settings.NotificationFrequency <= 0)
            {
                return false;
            }
            ValidationMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// Sends the test mail.
        /// </summary>
        /// <param name="mailDetails">The mail details.</param>
        /// <returns></returns>
        private static bool SendTestMail(SmtpDetail mailDetails)
        {
            bool result = MailService.SendMail(mailDetails, Utilities.GetResourceValue("MailSubjectTest"), GetNotificationMailBody());
            return result;
        }

        /// <summary>
        /// Gets the notification mail body.
        /// </summary>
        /// <returns></returns>
        private static string GetNotificationMailBody()
        {
            string mailBody = System.IO.File.ReadAllText(Constants.BaseDirectory + @"Templates\NotificationMail.html");
            return mailBody;
        }

        #endregion Methods
    }
}