using System.Collections.Generic;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.ViewModels.Helpers;

namespace I95Dev.Connector.UI.Base.DataAccess
{
    public interface IReportService
    {
        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <returns></returns>
        IList<IntegerComboBoxModel> GetCategories();

        /// <summary>
        /// Gets the notifications data.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <returns></returns>
        IList<NotificationModel> GetNotificationsData(NotificationsSearchModel searchModel);

        /// <summary>
        /// Gets the exclusions data.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <returns></returns>
        IList<ExclusionModel> GetExclusionsData(ExclusionsSearchModel searchModel);
    }
}