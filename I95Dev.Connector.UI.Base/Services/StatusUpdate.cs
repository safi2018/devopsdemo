namespace I95Dev.Connector.UI.Base.Services
{
    internal static class StatusUpdate
    {
        #region Status Message

        private static string statusMessage;

        /// <summary>
        /// Gets or sets the status message.
        /// </summary>
        /// <value>
        /// The status message.
        /// </value>
        internal static string StatusMessage
        {
            set
            {
                if (value != statusMessage)
                {
                    OnStatusChanged(value);
                }
                statusMessage = value;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:StatusChanged" /> event.
        /// </summary>
        /// <param name="message">The status message.</param>
        internal static void OnStatusChanged(string message)
        {
            StatusChanged?.Invoke(message);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        internal static event StatusChangedEventHandler StatusChanged;

        /// <summary>
        /// Status changed event handler
        /// </summary>
        /// <param name="message">The message.</param>
        internal delegate void StatusChangedEventHandler(string message);

        #endregion Status Message

        #region Pop up status

        private static bool isPopUpOpen;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pop up open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is pop up open; otherwise, <c>false</c>.
        /// </value>
        internal static bool IsPopUpOpen
        {
            set
            {
                if (isPopUpOpen == value) return;
                isPopUpOpen = value;
                OnPopUpStatusChanged(value);
            }
        }

        private static void OnPopUpStatusChanged(bool value)
        {
            PopUpStatusChanged?.Invoke(value);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        internal static event PopUpStatusChangedEventHandler PopUpStatusChanged;

        /// <summary>
        /// Status changed event handler
        /// </summary>
        /// <param name="isPopUpOpen">if set to <c>true</c> [is pop up open].</param>
        internal delegate void PopUpStatusChangedEventHandler(bool isPopUpOpen);

        #endregion Pop up status
    }
}