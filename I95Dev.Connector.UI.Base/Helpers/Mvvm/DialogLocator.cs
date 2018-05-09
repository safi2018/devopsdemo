using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using I95Dev.Connector.Base.Common;
using MvvmDialogs.DialogTypeLocators;

namespace I95Dev.Connector.UI.Base.Helpers.Mvvm
{
    public class DialogLocator : IDialogTypeLocator
    {
        private static readonly DialogTypeLocatorCache Cache = new DialogTypeLocatorCache();

        public static readonly string MainAssemblyPath = Assembly.GetEntryAssembly() != null ? Assembly.GetEntryAssembly().GetName().Name : "";

        /// <inheritdoc />
        public Type Locate(INotifyPropertyChanged viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            Type viewModelType = viewModel.GetType();

            Type dialogType = Cache.Get(viewModelType);
            if (dialogType != null)
            {
                return dialogType;
            }

            string dialogName = GetDialogName(viewModelType);

            dialogType = GetAssemblyFromType().GetType(dialogName);
            if (dialogType == null)
                throw new TypeLoadException(string.Format(Constants.DefaultCulture, "Dialog with name '{0}' is missing.", dialogName));

            Cache.Add(viewModelType, dialogType);

            return dialogType;
        }

        private static string GetDialogName(Type viewModelType)
        {
            string dialogName = viewModelType.FullName.Replace(".Base.ViewModels.", ".Views.");

            if (!dialogName.EndsWith("ViewModel", StringComparison.Ordinal))
                throw new TypeLoadException(string.Format(Constants.DefaultCulture, "View model of type '{0}' doesn't follow naming convention since it isn't suffixed with 'ViewModel'.", viewModelType));

            return dialogName.Substring(
                0,
                dialogName.Length - "Model".Length);
        }

        private static Assembly GetAssemblyFromType()
        {
            return AppDomain.CurrentDomain.GetAssemblies().First(c1 => string.Equals(c1.GetName().Name, MainAssemblyPath, StringComparison.OrdinalIgnoreCase));
        }
    }
}