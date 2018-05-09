using System;
using System.Collections.Generic;
using I95Dev.Connector.Base.Common;

namespace I95Dev.Connector.UI.Base.Helpers.Mvvm
{
    internal class DialogTypeLocatorCache
    {
        private readonly Dictionary<Type, Type> cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogTypeLocatorCache"/> class.
        /// </summary>
        internal DialogTypeLocatorCache()
        {
            cache = new Dictionary<Type, Type>();
        }

        /// <summary>
        /// Adds the specified view model type with its corresponding dialog type.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="dialogType">Type of the dialog.</param>
        internal void Add(Type viewModelType, Type dialogType)
        {
            if (viewModelType == null)
                throw new ArgumentNullException("viewModelType");
            if (dialogType == null)
                throw new ArgumentNullException("dialogType");
            if (cache.ContainsKey(viewModelType))
                throw new ArgumentException(string.Format(Constants.DefaultCulture, "View model of type '{0}' is already added.", viewModelType.ToString()));

            cache.Add(viewModelType, dialogType);
        }

        /// <summary>
        /// Gets the dialog type for specified view model type.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns>The dialog type if found; otherwise null.</returns>
        internal Type Get(Type viewModelType)
        {
            if (viewModelType == null)
                throw new ArgumentNullException("viewModelType");

            Type dialogType;
            cache.TryGetValue(viewModelType, out dialogType);
            return dialogType;
        }
    }
}