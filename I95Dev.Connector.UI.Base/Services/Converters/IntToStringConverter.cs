using System;
using System.Globalization;
using System.Windows.Data;
using I95Dev.Connector.Base.Common;

namespace I95Dev.Connector.UI.Base.Services.Converters
{
    public class IntToStringConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int entityId = parameter == null ? 0 : System.Convert.ToInt32(parameter, Constants.DefaultCulture);
            return entityId.ToString(Constants.DefaultCulture);
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = (string)value;
            return System.Convert.ToInt32(strValue, Constants.DefaultCulture);
        }
    }
}