using System;
using I95Dev.Connector.UI.Base.Models;

namespace I95Dev.Connector.UI.Base.ViewModels.Helpers
{
    public class DateSearchModel : BaseModel
    {
        private DateTime createdFrom;

        /// <summary>
        /// Gets or sets the created from.
        /// </summary>
        /// <value>
        /// The created from.
        /// </value>
        public DateTime CreatedFrom
        {
            get { return createdFrom; }
            set
            {
                if (SetProperty(ref createdFrom, value))
                {
                    DataModified = true;
                }
            }
        }

        private bool createdFromHasValue;

        /// <summary>
        /// Gets or sets a value indicating whether [created from has value].
        /// </summary>
        /// <value>
        /// <c>true</c> if [created from has value]; otherwise, <c>false</c>.
        /// </value>
        public bool CreatedFromHasValue
        {
            get { return createdFromHasValue; }
            set { SetProperty(ref createdFromHasValue, value); }
        }

        private DateTime createdTo;

        /// <summary>
        /// Gets or sets the created to.
        /// </summary>
        /// <value>
        /// The created to.
        /// </value>
        public DateTime CreatedTo
        {
            get { return createdTo; }
            set
            {
                if (SetProperty(ref createdTo, value))
                {
                    DataModified = true;
                }
            }
        }

        private bool createdToHasValue;

        /// <summary>
        /// Gets or sets a value indicating whether [created to has value].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [created to has value]; otherwise, <c>false</c>.
        /// </value>
        public bool CreatedToHasValue
        {
            get { return createdToHasValue; }
            set { SetProperty(ref createdToHasValue, value); }
        }

        private DateTime modifiedFrom;

        /// <summary>
        /// Gets or sets the modified from.
        /// </summary>
        /// <value>
        /// The modified from.
        /// </value>
        public DateTime ModifiedFrom
        {
            get { return modifiedFrom; }
            set { SetProperty(ref modifiedFrom, value); }
        }

        private bool modifiedFromHasValue;

        /// <summary>
        /// Gets or sets a value indicating whether [modified from has value].
        /// </summary>
        /// <value>
        /// <c>true</c> if [modified from has value]; otherwise, <c>false</c>.
        /// </value>
        public bool ModifiedFromHasValue
        {
            get { return modifiedFromHasValue; }
            set { SetProperty(ref modifiedFromHasValue, value); }
        }

        private DateTime modifiedTo;

        /// <summary>
        /// Gets or sets the modified to.
        /// </summary>
        /// <value>
        /// The modified to.
        /// </value>
        public DateTime ModifiedTo
        {
            get { return modifiedTo; }
            set { SetProperty(ref modifiedTo, value); }
        }

        private bool modifiedToHasValue;

        /// <summary>
        /// Gets or sets a value indicating whether [modified to has value].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [modified to has value]; otherwise, <c>false</c>.
        /// </value>
        public bool ModifiedToHasValue
        {
            get { return modifiedToHasValue; }
            set { SetProperty(ref modifiedToHasValue, value); }
        }

        private bool dataModified;

        /// <summary>
        /// Gets or sets a value indicating whether [data modified].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [data modified]; otherwise, <c>false</c>.
        /// </value>
        public bool DataModified
        {
            get { return dataModified; }
            set { SetProperty(ref dataModified, value); }
        }
    }
}