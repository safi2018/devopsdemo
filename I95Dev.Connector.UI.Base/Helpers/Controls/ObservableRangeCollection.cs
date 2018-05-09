using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace I95Dev.Connector.UI.Base.Helpers.Controls
{
    /// <inheritdoc />
    public class ObservableRangeCollection<T> : ObservableCollection<T>
    {
        public bool SuppressNotification { get; set; }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!SuppressNotification)
                base.OnCollectionChanged(e);
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="list">The list.</param>
        internal void AddRange(IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            SuppressNotification = true;

            foreach (T item in list)
            {
                Add(item);
            }
            SuppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}