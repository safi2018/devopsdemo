using System.Collections.Generic;
using I95Dev.Connector.UI.Base.Models;

namespace I95Dev.Connector.UI.Base.DataAccess
{
    internal interface IPaymentMethodDataAccess
    {
        /// <summary>
        /// Gets the payment methods list.
        /// </summary>
        /// <returns></returns>
        IList<PaymentSettingsModel> GetPaymentMethodsList();

        /// <summary>
        /// Saves the payment details.
        /// </summary>
        /// <param name="paymentMethod">The payment method.</param>
        /// <returns></returns>
        bool SavePaymentDetails(PaymentSettingsModel paymentMethod);

        /// <summary>
        /// Checks the payment combination.
        /// </summary>
        /// <param name="paymentMethod">The payment method.</param>
        /// <returns></returns>
        bool CheckPaymentCombination(PaymentSettingsModel paymentMethod);

        /// <summary>
        /// Determines whether [is payment method table exists].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is payment method table exists]; otherwise, <c>false</c>.
        /// </returns>
        bool IsPaymentMethodTableExists();
    }
}