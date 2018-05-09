using System.Windows.Input;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using MvvmDialogs;

namespace I95Dev.Connector.UI.Base.ViewModels.Share
{
    public class AuthenticationViewModel : BaseViewModel, IModalDialogViewModel
    {
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

        private string userName;

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        /// <value>
        /// The user name.
        /// </value>
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                SetProperty(ref userName, value);
            }
        }

        private string password;

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                SetProperty(ref password, value);
            }
        }

        private string validationMessage;

        /// <summary>
        /// Gets or sets the validation message.
        /// </summary>
        /// <value>
        /// The validation message.
        /// </value>
        public string ValidationMessage
        {
            get { return validationMessage; }
            set
            {
                SetProperty(ref validationMessage, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is canceled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is canceled; otherwise, <c>false</c>.
        /// </value>
        public bool IsCanceled { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this instance is authenticated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is authenticated; otherwise, <c>false</c>.
        /// </value>
        public bool IsAuthenticated { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationViewModel"/> class.
        /// </summary>
        public AuthenticationViewModel()
        {
            CloseCommand = new BaseCommand(CloseWindow);
            SubmitCommand = new BaseCommand(CanSubmit, CheckCredentials);
        }

        public ICommand CloseCommand { get; private set; }

        /// <summary>
        /// Closes the window.
        /// </summary>
        private void CloseWindow()
        {
            DialogResult = true;
            IsCanceled = true;
        }

        /// <summary>
        /// Gets the submit command.
        /// </summary>
        /// <value>
        /// The submit command.
        /// </value>
        public ICommand SubmitCommand { get; private set; }

        /// <summary>
        /// Determines whether this instance can submit.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can submit; otherwise, <c>false</c>.
        /// </returns>
        private bool CanSubmit()
        {
            return !string.IsNullOrEmpty(UserName) & !string.IsNullOrEmpty(Password);
        }

        /// <summary>
        /// Checks the credentials
        /// </summary>
        internal void CheckCredentials()
        {
            ValidationMessage = string.Empty;
            if (UserName == "admin" && Password == "i95devteam")
            {
                DialogResult = true;
                IsCanceled = false;
                IsAuthenticated = true;
            }
            else
                ValidationMessage = "Invalid credentials";
        }
    }
}