using System.Windows.Input;

namespace I95Dev.Connector.UI.Base.ViewModels.Base
{
    public abstract class BaseNavigationViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the previous command.
        /// </summary>
        /// <value>
        /// The previous command.
        /// </value>
        public ICommand PreviousCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the next command.
        /// </summary>
        /// <value>
        /// The next command.
        /// </value>
        public ICommand NextCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the first command.
        /// </summary>
        /// <value>
        /// The first command.
        /// </value>
        public ICommand FirstCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the last command.
        /// </summary>
        /// <value>
        /// The last command.
        /// </value>
        public ICommand LastCommand { get; protected set; }
    }
}