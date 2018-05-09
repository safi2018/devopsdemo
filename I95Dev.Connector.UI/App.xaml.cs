using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using I95Dev.Connector.Base.Helpers;
using I95Dev.Connector.Base.Services;
using I95Dev.Connector.UI.Base.Services;

namespace I95Dev.Connector.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Handles the Startup event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StartupEventArgs"/> instance containing the event data.</param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            InitializeService.InitializeConnectionStrings(true);
            if (e.Args.Length > 0)
            {
                try
                {
                    ManageSchedulers.StartScheduler(e.Args);
                }
                catch (System.Exception exception)
                {
                    Logger.LogMessage(exception.Message, "StartUp", LogType.Error, exception);
                }
                CloseApp();
            }
            else
            {
                Current.DispatcherUnhandledException += AppDispatcherUnhandledException;
                var homeView = new Home();
                homeView.Show();
                homeView.Closed += HomeView_Closed;
            }
        }

        /// <summary>
        /// Handles the Closed event of the HomeView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HomeView_Closed(object sender, System.EventArgs e)
        {
            CloseApp();
        }

        /// <summary>
        /// Closes the application.
        /// </summary>
        private static void CloseApp()
        {
            Current.Shutdown(0);
        }

        /// <summary>
        /// Applications the dispatcher un handled exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DispatcherUnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
#if DEBUG

            e.Handled = false;

#else

            ShowUnhandledException(e);

#endif
        }

        /// <summary>
        /// Shows the un handled exception.
        /// </summary>
        /// <param name="e">The <see cref="DispatcherUnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private static void ShowUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            string errorMessage = string.Format(CultureInfo.CurrentCulture, "An application error occurred.\n\nError: {0}\n\nDo you want to continue?\n(if you click Yes you will continue with your work, if you click No the application will close)",

            e.Exception.Message + (e.Exception.InnerException != null ? "\n" +
            e.Exception.InnerException.Message : null));

            if (MessageBox.Show(errorMessage, "Application Error", MessageBoxButton.YesNoCancel, MessageBoxImage.Error) == MessageBoxResult.No)
            {
                Current.Shutdown();
            }
        }
    }
}