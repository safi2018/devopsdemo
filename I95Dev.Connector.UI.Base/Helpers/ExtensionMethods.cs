using System;
using I95Dev.Connector.Base.Common;

namespace I95Dev.Connector.UI.Base.Helpers
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        internal static string FormatDateTime(DateTime value)
        {
            return value.ToString(@"yyyy/MM/dd HH:mm:ss ttt", Constants.DefaultCulture);
        }
    }
}