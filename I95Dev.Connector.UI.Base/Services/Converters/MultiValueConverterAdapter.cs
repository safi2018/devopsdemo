using System;
using System.Globalization;
using System.Windows.Data;

namespace I95Dev.Connector.UI.Base.Services.Converters
{
    public class MultiValueConverterAdapter : IMultiValueConverter
    {
        public IValueConverter Converter { get; set; }

        #region IMultiValueConverter Members

        private object lastParameter;
        private IValueConverter lastConverter;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            lastConverter = Converter;
            if (values.Length > 1) lastParameter = values[1];
            if (values.Length > 2) lastConverter = (IValueConverter)values[2];
            if (Converter == null) return values[0];
            return Converter.Convert(values[0], targetType, lastParameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (lastConverter == null) return new object[] { value };
            return new object[] { lastConverter.ConvertBack(value, targetTypes[0], lastParameter, culture) };
        }

        #endregion IMultiValueConverter Members
    }
}