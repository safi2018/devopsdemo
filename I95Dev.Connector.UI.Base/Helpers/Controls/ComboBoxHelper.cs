using System.Collections.Generic;
using I95Dev.Connector.UI.Base.Models;

namespace I95Dev.Connector.UI.Base.Helpers.Controls
{
    internal static class ComboBoxHelper
    {
        /// <summary>
        /// Prepares the ComboBox dates.
        /// </summary>
        /// <returns></returns>
        internal static IList<IntegerComboBoxModel> PrepareComboBoxDates()
        {
            var list = new List<IntegerComboBoxModel>
            {
                new IntegerComboBoxModel { Name = "Today", Value = 0 },
                new IntegerComboBoxModel { Name = "Last week", Value = 7 },
                new IntegerComboBoxModel { Name = "Last 2 weeks", Value = 14 },
                new IntegerComboBoxModel { Name = "Last 3 weeks", Value = 21 },
                new IntegerComboBoxModel { Name = "Last month", Value = 30 }
            };
            return list;
        }
    }
}