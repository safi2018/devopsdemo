using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace I95Dev.Connector.UI.Base.Services.Converters
{
    public class EntityVisibilityConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Converts the value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int entityId = (int)parameter;
            return entityId > 0;
        }

        /// <summary>
        /// Converts the value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool strValue = (bool)value;
            return strValue ? 1 : 0;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}