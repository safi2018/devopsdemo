using System.Windows;
using I95Dev.Connector.UI.Base.Helpers.Mvvm;
using MvvmDialogs;

namespace I95Dev.Connector.UI.Base.Helpers
{
    internal static class Utilities
    {
        /// <summary>
        /// The total status name
        /// </summary>
        public const string TotalStatusName = "Total by Status";

        private static IDialogService service;

        /// <summary>
        /// Gets the resource value.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns></returns>
        internal static string GetResourceValue(string resourceName)
        {
            return (string)Application.Current.Resources[resourceName];
        }

        /// <summary>
        /// Creates the Dialog Service instance.
        /// </summary>
        /// <returns></returns>
        internal static IDialogService CreateDialogServiceInstance()
        {
            if (service == null) service = new LocalDialogService(new DialogLocator());
            return service;
        }
    }
}