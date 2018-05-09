using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using I95Dev.Connector.Base.Common;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.ViewModels.Helpers;
using SqlKata;
using SqlKata.Compilers;

namespace I95Dev.Connector.UI.Base.DataAccess
{
    internal class ShippingMethodDataAccess : IShippingMethodDataAccess
    {
        public IList<ShippingCarrierModel> GetShipmentCarriers()
        {
            var carriersList = new List<ShippingCarrierModel>();
            using (var sqlConnection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = "Select CarrierId,CarrierName,CarrierDescription from ShippingCarrier order by CarrierDescription";
                using (var sqlCommand = new SqlCommand(commandText, sqlConnection))
                {
                    sqlConnection.Open();
                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            carriersList.Add(new ShippingCarrierModel
                            {
                                CarrierId = Convert.ToInt32(dataReader["CarrierId"], CultureInfo.CurrentCulture),
                                CarrierCode = dataReader["CarrierName"].ToString(),
                                CarrierDescription = dataReader["CarrierDescription"].ToString()
                            });
                        }
                    }
                }
            }
            return carriersList;
        }

        public IList<ShipmentSettingsModel> GetShipmentMethodsList(ShippingMethodSearchModel searchModel)
        {
            var shipmentMethodDetails = new List<ShipmentSettingsModel>();
            using (var connection = new SqlConnection(Constants.ConnectorDatabase))
            {
                SqlResult sqlResult = PrepareQuery(searchModel);

                using (var command = new SqlCommand(sqlResult.Sql, connection))
                {
                    connection.Open();

                    foreach (KeyValuePair<string, object> resultBinding in sqlResult.Bindings)
                    {
                        command.Parameters.AddWithValue(resultBinding.Key, resultBinding.Value);
                    }

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            shipmentMethodDetails.Add(new ShipmentSettingsModel
                            {
                                ShipmentMethodId = Convert.ToInt32(dataReader["ShippingMethodId"], Constants.DefaultCulture),
                                MagentoShipmentId = dataReader["EcommerceShippingMethod"].ToString(),
                                GpShipmentId = dataReader["ERPShippingMethod"].ToString(),
                                Description = dataReader["Description"].ToString(),
                                CarrierName = dataReader["CarrierDescription"].ToString(),
                                CarrierId = Convert.ToInt32(dataReader["CarrierId"], Constants.DefaultCulture),
                                IsEcommerceDefault = !dataReader.IsDBNull(4) && dataReader.GetBoolean(4),
                                IsErpDefault = !dataReader.IsDBNull(5) && dataReader.GetBoolean(5)
                            });
                        }
                    }
                }
            }
            return shipmentMethodDetails;
        }

        private static SqlResult PrepareQuery(ShippingMethodSearchModel searchModel)
        {
            Query query = new Query("ShippingMethod as S").Select("ShippingMethodId", "EcommerceShippingMethod", "ERPShippingMethod", "Description", "IsEcommerceDefault", "IsErpDefault");
            query.Join("ShippingCarrier as C", "S.CarrierId", "C.CarrierId").Select("C.CarrierDescription", "C.CarrierId");
            if (!string.IsNullOrEmpty(searchModel.ErpShippingMethod)) query.WhereLike("S.ERPShippingMethod", "%" + searchModel.ErpShippingMethod + "%");
            if (!string.IsNullOrEmpty(searchModel.EcommerceShippingMethod)) query.WhereLike("S.EcommerceShippingMethod", "%" + searchModel.EcommerceShippingMethod + "%");
            if (searchModel.CarrierId > 0) query.Where("S.CarrierId", searchModel.CarrierId);

            query.OrderBy("C.CarrierDescription");
            query.OrderBy("S.ShippingMethodId");
            return new SqlServerCompiler().Compile(query);
        }

        public bool SaveShipmentDetails(ShipmentSettingsModel shipmentMethod)
        {
            using (var connection = new SqlConnection(Constants.ConnectorDatabase))
            {
                var cmd = new SqlCommand
                {
                    CommandText =
                        @"if @Id<=0
                        Insert into ShippingMethod(CarrierId, EcommerceShippingMethod, ERPShippingMethod,[Description],IsEcommerceDefault,IsErpDefault)
                        values(@CarrierId, @MagnetoShippingMethod, @ERPShippingMethod, @Description,@IsEcommerceDefault,@IsErpDefault)
                        ELSE
                        update ShippingMethod SET CarrierId=@CarrierId,IsEcommerceDefault=@IsEcommerceDefault,IsErpDefault=@IsErpDefault,Description=@Description,EcommerceShippingMethod=@MagnetoShippingMethod,ERPShippingMethod=@ERPShippingMethod
                        where ShippingMethodId=@Id"
                };
                cmd.Parameters.AddWithValue("@Id", shipmentMethod.ShipmentMethodId);
                cmd.Parameters.AddWithValue("@CarrierId", shipmentMethod.CarrierId);
                cmd.Parameters.AddWithValue("@MagnetoShippingMethod", shipmentMethod.MagentoShipmentId);
                cmd.Parameters.AddWithValue("@ERPShippingMethod", shipmentMethod.GpShipmentId);
                cmd.Parameters.AddWithValue("@Description", shipmentMethod.Description);
                cmd.Parameters.AddWithValue("@IsEcommerceDefault", shipmentMethod.IsEcommerceDefault);
                cmd.Parameters.AddWithValue("@IsErpDefault", shipmentMethod.IsErpDefault);
                cmd.Connection = connection;
                connection.Open();
                int i = cmd.ExecuteNonQuery();
                if (i <= 0) return false;
                UpdateDefaultFlags(shipmentMethod, connection);
                return true;
            }
        }

        private static void UpdateDefaultFlags(ShipmentSettingsModel shipmentMethod, SqlConnection connection)
        {
            if (shipmentMethod.IsEcommerceDefault)
            {
                UpdateEcommerceDefaults(shipmentMethod, connection);
            }

            if (shipmentMethod.IsErpDefault)
            {
                UpdateErpDefaults(shipmentMethod, connection);
            }
        }

        private static void UpdateEcommerceDefaults(ShipmentSettingsModel shipmentMethod, SqlConnection connection)
        {
            const string commandText = "update ShippingMethod SET IsEcommerceDefault=0 where EcommerceShippingMethod<>@EcommerceShippingMethod AND ERPShippingMethod=@ERPShippingMethod";
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@ERPShippingMethod", shipmentMethod.GpShipmentId);
                command.Parameters.AddWithValue("@EcommerceShippingMethod", shipmentMethod.MagentoShipmentId);
                command.ExecuteNonQuery();
            }
        }

        private static void UpdateErpDefaults(ShipmentSettingsModel shipmentMethod, SqlConnection connection)
        {
            const string commandText = "update ShippingMethod SET IsErpDefault=0 where EcommerceShippingMethod=@EcommerceShippingMethod AND ERPShippingMethod<>@ERPShippingMethod";
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@ERPShippingMethod", shipmentMethod.GpShipmentId);
                command.Parameters.AddWithValue("@EcommerceShippingMethod", shipmentMethod.MagentoShipmentId);
                command.ExecuteNonQuery();
            }
        }

        public bool IsShippingMethodExists(ShipmentSettingsModel shipmentSettingsModel)
        {
            bool result;
            using (var connection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = @"Select ShippingMethodId from ShippingMethod where EcommerceShippingMethod=@MagnetoShippingMethod AND ERPShippingMethod=@ERPShippingMethod AND ShippingMethodId<>@ShippingMethodId";
                using (var command = new SqlCommand(commandText, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@MagnetoShippingMethod", shipmentSettingsModel.MagentoShipmentId);
                    command.Parameters.AddWithValue("@ERPShippingMethod", shipmentSettingsModel.GpShipmentId);
                    command.Parameters.AddWithValue("@ShippingMethodId", shipmentSettingsModel.ShipmentMethodId);
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        result = dataReader.Read();
                    }
                    connection.Close();
                }
            }
            return result;
        }

        public bool SaveCarrierDetails(ShippingCarrierModel carrier)
        {
            using (var connection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = @"IF @CarrierId<=0
                                                    Insert into [ShippingCarrier] (CarrierName,CarrierDescription) values (@CarrierName,@CarrierDescription);
                                                    ELSE
                                                    update ShippingCarrier set CarrierName=@CarrierName,CarrierDescription=@CarrierDescription where CarrierId=@CarrierId";
                var cmd = new SqlCommand(commandText, connection);
                cmd.Parameters.AddWithValue("@CarrierId", carrier.CarrierId);
                cmd.Parameters.AddWithValue("@CarrierName", carrier.CarrierCode);
                cmd.Parameters.AddWithValue("@CarrierDescription", carrier.CarrierDescription);
                connection.Open();
                int i = cmd.ExecuteNonQuery();
                return i > 0;
            }
        }

        public bool IsCarrierExists(ShippingCarrierModel carrier)
        {
            using (var connection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = @"Select CarrierId from ShippingCarrier where CarrierName=@CarrierName and CarrierId<>@CarrierId";
                using (var command = new SqlCommand(commandText, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@CarrierName", carrier.CarrierCode);
                    command.Parameters.AddWithValue("@CarrierId", carrier.CarrierId);

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        return dataReader.Read();
                    }
                }
            }
        }

        public bool IsShippingMethodExistsInGp(ShipmentSettingsModel shippingMethod)
        {
            using (var connection = new SqlConnection(Constants.ErpDatabase))
            {
                const string commandText = @"Select SHIPMTHD from SY03000 where SHIPMTHD=@SHIPMTHD";
                using (var command = new SqlCommand(commandText, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@SHIPMTHD", shippingMethod.GpShipmentId);

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        return dataReader.Read();
                    }
                }
            }
        }

        /// <inheritdoc />
        public bool IsShippingMethodTableExists()
        {
            using (var connection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = @"select name from sys.tables where name='ShippingMethod'";
                using (var command = new SqlCommand(commandText, connection))
                {
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    return reader.HasRows;
                }
            }
        }

        /// <inheritdoc />
        public bool IsShippingCarrierTableExists()
        {
            using (var connection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = @"select name from sys.tables where name='ShippingCarrier'";
                using (var command = new SqlCommand(commandText, connection))
                {
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    return reader.HasRows;
                }
            }
        }

        /// <inheritdoc />
        public IList<StringComboBoxModel> GetGpShippingMethods()
        {
            var entities = new List<StringComboBoxModel>();
            using (var sqlConnection = new SqlConnection(Constants.ErpDatabase))
            {
                const string commandText = "Select SHIPMTHD,SHMTHDSC,CARRIER from SY03000 order by SHMTHDSC";
                using (var sqlCommand = new SqlCommand(commandText, sqlConnection))
                {
                    sqlConnection.Open();
                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            string carrier = dataReader.GetString(2).TrimEnd();
                            if (!string.IsNullOrEmpty(carrier)) carrier += " - ";
                            entities.Add(new StringComboBoxModel
                            {
                                Value = dataReader.GetString(0).TrimEnd(),
                                Name = carrier + dataReader.GetString(1).TrimEnd(),
                            });
                        }
                    }
                }
            }
            return entities;
        }
    }
}