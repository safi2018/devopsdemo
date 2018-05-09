using System.Collections.Generic;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.ViewModels.Helpers;

namespace I95Dev.Connector.UI.Base.DataAccess
{
    internal interface IShippingMethodDataAccess
    {
        /// <summary>
        /// Gets the shipment carriers.
        /// </summary>
        /// <returns></returns>
        IList<ShippingCarrierModel> GetShipmentCarriers();

        /// <summary>
        /// Gets the shipment methods list.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <returns></returns>
        IList<ShipmentSettingsModel> GetShipmentMethodsList(ShippingMethodSearchModel searchModel);

        /// <summary>
        /// Saves the shipment details.
        /// </summary>
        /// <param name="shipmentSettingsModel">The shipment settings model.</param>
        /// <returns></returns>
        bool SaveShipmentDetails(ShipmentSettingsModel shipmentSettingsModel);

        /// <summary>
        /// Saves the carrier details.
        /// </summary>
        /// <param name="carrier">The carrier.</param>
        /// <returns></returns>
        bool SaveCarrierDetails(ShippingCarrierModel carrier);

        /// <summary>
        /// Determines whether [is shipping method exists] [the specified shipping method].
        /// </summary>
        /// <param name="shippingMethod">The shipping method.</param>
        /// <returns>
        ///   <c>true</c> if [is shipping method exists] [the specified shipping method]; otherwise, <c>false</c>.
        /// </returns>
        bool IsShippingMethodExists(ShipmentSettingsModel shippingMethod);

        /// <summary>
        /// Determines whether [is carrier exists] [the specified carrier].
        /// </summary>
        /// <param name="carrier">The carrier.</param>
        /// <returns>
        ///   <c>true</c> if [is carrier exists] [the specified carrier]; otherwise, <c>false</c>.
        /// </returns>
        bool IsCarrierExists(ShippingCarrierModel carrier);

        /// <summary>
        /// Determines whether [is shipping method exists in gp] [the specified shipping method].
        /// </summary>
        /// <param name="shippingMethod">The shipping method.</param>
        /// <returns>
        ///   <c>true</c> if [is shipping method exists in gp] [the specified shipping method]; otherwise, <c>false</c>.
        /// </returns>
        bool IsShippingMethodExistsInGp(ShipmentSettingsModel shippingMethod);

        /// <summary>
        /// Determines whether [is tables exists].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is tables exists]; otherwise, <c>false</c>.
        /// </returns>
        bool IsShippingMethodTableExists();

        /// <summary>
        /// Determines whether [is shipping carrier table exists].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is shipping carrier table exists]; otherwise, <c>false</c>.
        /// </returns>
        bool IsShippingCarrierTableExists();

        /// <summary>
        /// Gets the gp shipping methods.
        /// </summary>
        /// <returns></returns>
        IList<StringComboBoxModel> GetGpShippingMethods();
    }
}