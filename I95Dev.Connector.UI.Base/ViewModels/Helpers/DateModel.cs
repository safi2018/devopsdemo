using System;
using I95Dev.Connector.UI.Base.Models;

namespace I95Dev.Connector.UI.Base.ViewModels.Helpers
{
    public class DateModel : BaseModel
    {
        private DateTime date;

        /// <summary>
        /// Gets or sets the created from.
        /// </summary>
        /// <value>
        /// The created from.
        /// </value>
        public DateTime Date
        {
            get { return date; }
            set { SetProperty(ref date, value); }
        }

        private bool hasValue;

        /// <summary>
        /// Gets or sets a value indicating whether this instance has value.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has value; otherwise, <c>false</c>.
        /// </value>
        public bool HasValue
        {
            get { return hasValue; }
            set { SetProperty(ref hasValue, value); }
        }

        public DateModel(DateTime time) : this()
        {
            Date = time;
        }

        public DateModel()
        { }
    }
}