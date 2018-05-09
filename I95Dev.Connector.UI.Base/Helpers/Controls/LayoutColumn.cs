using System;
using System.Windows;
using System.Windows.Controls;

namespace I95Dev.Connector.UI.Base.Helpers.Controls
{
    public abstract class LayoutColumn
    {
        protected static bool HasPropertyValue(GridViewColumn column, DependencyProperty dp)
        {
            if (column == null)
            {
                throw new ArgumentNullException("column");
            }
            object value = column.ReadLocalValue(dp);
            return value.GetType() == dp.PropertyType;
        }

        protected static double? GetColumnWidth(GridViewColumn column, DependencyProperty dp)
        {
            if (column == null)
            {
                throw new ArgumentNullException("column");
            }
            object value = column.ReadLocalValue(dp);
            if (value.GetType() == dp.PropertyType)
            {
                return (double)value;
            }

            return null;
        }
    }
}