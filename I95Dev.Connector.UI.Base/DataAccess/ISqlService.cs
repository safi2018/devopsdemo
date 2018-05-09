using System.Collections.Generic;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.ViewModels.Helpers;

namespace I95Dev.Connector.UI.Base.DataAccess
{
    public interface ISqlService
    {
        /// <summary>
        /// Gets the ecommerce message summary.
        /// </summary>
        /// <param name="searchData">The search data.</param>
        /// <returns></returns>
        IList<MessageCountModel> GetEcommerceMessageSummary(MessageSearchModel searchData);

        /// <summary>
        /// Gets the erp message summary.
        /// </summary>
        /// <param name="searchData">The search data.</param>
        /// <returns></returns>
        IList<MessageCountModel> GetErpMessageSummary(MessageSearchModel searchData);

        /// <summary>
        /// Gets the forward entities.
        /// </summary>
        /// <returns></returns>
        IList<IntegerComboBoxModel> GetForwardEntities();

        /// <summary>
        /// Gets the forward statuses.
        /// </summary>
        /// <returns></returns>
        IList<IntegerComboBoxModel> GetForwardStatuses();

        /// <summary>
        /// Gets the ecommerce messages.
        /// </summary>
        /// <param name="searchData">The search data.</param>
        /// <returns></returns>
        IList<MessageReportModel> GetEcommerceMessages(MessageSearchModel searchData);

        /// <summary>
        /// Gets the reverse entities.
        /// </summary>
        /// <returns></returns>
        IList<IntegerComboBoxModel> GetReverseEntities();

        /// <summary>
        /// Gets the reverse statuses.
        /// </summary>
        /// <returns></returns>
        IList<IntegerComboBoxModel> GetReverseStatuses();

        /// <summary>
        /// Gets the erp messages.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <returns></returns>
        IList<MessageReportModel> GetErpMessages(MessageSearchModel searchModel);

        /// <summary>
        /// Gets the ecommerce record count.
        /// </summary>
        /// <param name="searchData">The search data.</param>
        /// <returns></returns>
        int GetEcommerceRecordCount(MessageSearchModel searchData);

        /// <summary>
        /// Gets the erp record count.
        /// </summary>
        /// <param name="searchData">The search data.</param>
        /// <returns></returns>
        int GetErpRecordCount(MessageSearchModel searchData);

        /// <summary>
        /// Gets the erp order value.
        /// </summary>
        /// <param name="searchData">The search data.</param>
        /// <returns></returns>
        decimal GetErpOrderValue(MessageSearchModel searchData);

        /// <summary>
        /// Gets the Magento orders value.
        /// </summary>
        /// <param name="searchData">The search data.</param>
        /// <returns></returns>
        decimal GetMagentoOrderValue(MessageSearchModel searchData);
    }
}