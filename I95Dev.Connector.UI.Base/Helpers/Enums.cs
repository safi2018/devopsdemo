using System.ComponentModel;
using I95Dev.Connector.Base.Helpers;

namespace I95Dev.Connector.UI.Base.Helpers
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Comparison
    {
        /// <summary>
        /// The greater than
        /// </summary>
        [Description(">")]
        GreaterThan = 0,

        /// <summary>
        /// The less than
        /// </summary>
        [Description("<")]
        LessThan = 1,

        /// <summary>
        /// The equal
        /// </summary>
        [Description("=")]
        Equal = 2,

        /// <summary>
        /// The greater than or equal to
        /// </summary>
        [Description(">=")]
        GreaterThanOrEqualTo = 3,

        /// <summary>
        /// The less than or equal to
        /// </summary>
        [Description("<=")]
        LessThanOrEqualTo = 4
    }

    public enum Entity
    {
        None = 0,
        Product = 1,
        Customer = 6,
        SalesOrder = 7
    }
}