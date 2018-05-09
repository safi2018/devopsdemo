using System;
using System.DirectoryServices.AccountManagement;
using System.Windows.Input;
using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using MvvmDialogs;

namespace I95Dev.Connector.UI.Base.ViewModels.Share
{
    public class PasswordViewModel : BaseViewModel, IModalDialogViewModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

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

        private bool? dialogResult;

        /// <summary>
        /// methods.
        /// </summary>
        public bool? DialogResult
        {
            get
            {
                return dialogResult;
            }
            private set
            {
                SetProperty(ref dialogResult, value);
            }
        }

        #endregion Properties

        public PasswordViewModel()
        {
            UserName = Environment.UserName;
            SubmitCommand = new BaseCommand(CanSubmit, CheckCredentials);
        }

        #region Commands

        public ICommand SubmitCommand { get; set; }

        /// <summary>
        /// Determines whether this instance can submit.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can submit; otherwise, <c>false</c>.
        /// </returns>
        private bool CanSubmit()
        {
            return !string.IsNullOrEmpty(Password);
        }

        /// <summary>
        /// Checks the credentials
        /// </summary>
        internal void CheckCredentials()
        {
            CheckCredentials(true);
        }

        /// <summary>
        /// Checks the credentials.
        /// </summary>
        internal void CheckCredentials(bool isUiProcess)
        {
            ValidationMessage = "";
            if (string.IsNullOrEmpty(Password))
            {
                ValidationMessage = isUiProcess ? Utilities.GetResourceValue("PasswordIsRequired") : "Password is required";
                return;
            }

            bool valid;
            ContextType currenContext = GetAuthenticationContext();

            using (var context = new PrincipalContext(currenContext))
            {
                valid = context.ValidateCredentials(UserName, Password);
            }
            if (valid)
            {
                DialogResult = true;
            }
            else
            {
                ValidationMessage = isUiProcess ? Utilities.GetResourceValue("InvalidPassword") : "Invalid Process";
            }
        }

        /// <summary>
        /// Gets the context of the authentication.
        /// </summary>
        /// <returns>Returns the Type of authentication whether it may be machine level or domain level</returns>
        private static ContextType GetAuthenticationContext()
        {
            string machineName = Environment.MachineName;
            string domainName = Environment.UserDomainName;

            return string.Equals(machineName, domainName, StringComparison.OrdinalIgnoreCase) ? ContextType.Machine : ContextType.Domain;
        }

        #endregion Commands
    }
}