using System.Collections.Generic;
using System.Data.SqlClient;
using I95Dev.Connector.Base.Common;
using I95Dev.Connector.UI.Base.Models;

namespace I95Dev.Connector.UI.Base.DataAccess
{
    internal class PaymentMethodDataAccess : IPaymentMethodDataAccess
    {
        /// <inheritdoc />
        public IList<PaymentSettingsModel> GetPaymentMethodsList()
        {
            var paymentMethodDetails = new List<PaymentSettingsModel>();
            using (var connection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = @"Select EcommercePaymentMethod,ERPPaymentMethod,ERPPaymentType,PaymentMethodId,IsEcommerceDefault,IsErpDefault from PaymentMethod";

                using (var command = new SqlCommand(commandText, connection))
                {
                    connection.Open();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            paymentMethodDetails.Add(new PaymentSettingsModel
                            {
                                MagentoPaymentName = dataReader["EcommercePaymentMethod"].ToString(),
                                GpPaymentName = dataReader["ERPPaymentMethod"].ToString(),
                                GpPaymentId = dataReader.GetInt32(2),
                                Id = dataReader.GetInt32(3),
                                IsEcommerceDefault = !dataReader.IsDBNull(4) && dataReader.GetBoolean(4),
                                IsErpDefault = !dataReader.IsDBNull(5) && dataReader.GetBoolean(5)
                            });
                        }
                    }
                }
            }
            return paymentMethodDetails;
        }

        /// <inheritdoc />
        public bool SavePaymentDetails(PaymentSettingsModel paymentMethod)
        {
            using (var connection = new SqlConnection(Constants.ConnectorDatabase))
            {
                var cmd = new SqlCommand
                {
                    CommandType = System.Data.CommandType.Text,
                    CommandText = @"IF(@Id>0)
                    Update PaymentMethod SET EcommercePaymentMethod=@MagentoPaymentMethod,ERPPaymentMethod=@ERPPaymentMethod,ERPPaymentType=@ERPPaymentType,IsEcommerceDefault=@IsEcommerceDefault,IsErpDefault=@IsErpDefault where PaymentMethodId=@Id
                    else
                    Insert into PaymentMethod (EcommercePaymentMethod,ERPPaymentMethod,ERPPaymentType,IsEcommerceDefault,IsErpDefault) values(@MagentoPaymentMethod,@ERPPaymentMethod,@ERPPaymentType,@IsEcommerceDefault,@IsErpDefault);"
                };
                cmd.Parameters.AddWithValue("@MagentoPaymentMethod", paymentMethod.MagentoPaymentName);
                cmd.Parameters.AddWithValue("@ERPPaymentMethod", paymentMethod.GpPaymentName);
                cmd.Parameters.AddWithValue("@ERPPaymentType", paymentMethod.GpPaymentId);
                cmd.Parameters.AddWithValue("@Id", paymentMethod.Id);
                cmd.Parameters.AddWithValue("@IsEcommerceDefault", paymentMethod.IsEcommerceDefault);
                cmd.Parameters.AddWithValue("@IsErpDefault", paymentMethod.IsErpDefault);
                cmd.Connection = connection;
                connection.Open();
                int i = cmd.ExecuteNonQuery();
                if (i <= 0) return false;
                UpdateDefaultValues(paymentMethod, connection);
                return true;
            }
        }

        private static void UpdateDefaultValues(PaymentSettingsModel paymentMethod, SqlConnection connection)
        {
            if (paymentMethod.IsErpDefault)
            {
                UpdateErpDefaultValues(paymentMethod, connection);
            }

            if (paymentMethod.IsEcommerceDefault)
            {
                UpdateEcommerceDefaultValues(paymentMethod, connection);
            }
        }

        private static void UpdateErpDefaultValues(PaymentSettingsModel paymentMethod, SqlConnection connection)
        {
            const string commandText = "update PaymentMethod SET IsErpDefault=0 where EcommercePaymentMethod=@EcommercePaymentMethod AND ERPPaymentMethod<>@ERPPaymentMethod";
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@ERPPaymentMethod", paymentMethod.GpPaymentName);
                command.Parameters.AddWithValue("@EcommercePaymentMethod", paymentMethod.MagentoPaymentName);
                command.ExecuteNonQuery();
            }
        }

        private static void UpdateEcommerceDefaultValues(PaymentSettingsModel paymentMethod, SqlConnection connection)
        {
            const string commandText = "update PaymentMethod SET IsEcommerceDefault=0 where EcommercePaymentMethod<>@EcommercePaymentMethod AND ERPPaymentMethod=@ERPPaymentMethod";
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@ERPPaymentMethod", paymentMethod.GpPaymentName);
                command.Parameters.AddWithValue("@EcommercePaymentMethod", paymentMethod.MagentoPaymentName);
                command.ExecuteNonQuery();
            }
        }

        /// <inheritdoc />
        public bool CheckPaymentCombination(PaymentSettingsModel paymentMethod)
        {
            bool result;
            using (var connection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = @"Select PaymentMethodId from PaymentMethod where EcommercePaymentMethod=@MagentoPaymentMethod AND ERPPaymentType=@ERPPaymentType AND PaymentMethodId<>@Id";
                using (var command = new SqlCommand(commandText, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@MagentoPaymentMethod", paymentMethod.MagentoPaymentName);
                    command.Parameters.AddWithValue("@ERPPaymentType", paymentMethod.GpPaymentId);
                    command.Parameters.AddWithValue("@Id", paymentMethod.Id);

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        result = dataReader.Read();
                    }
                    connection.Close();
                }
            }
            return result;
        }

        public bool IsPaymentMethodTableExists()
        {
            using (var connection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = @"select name from sys.tables where name='PaymentMethod'";
                using (var command = new SqlCommand(commandText, connection))
                {
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    return reader.HasRows;
                }
            }
        }
    }
}