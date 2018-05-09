using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using I95Dev.Connector.Base.Common;
using I95Dev.Connector.Base.Helpers;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.Resources.Messages;
using I95Dev.Connector.UI.Base.ViewModels.Helpers;
using SqlKata;
using SqlKata.Compilers;

namespace I95Dev.Connector.UI.Base.DataAccess
{
    internal class SqlService : ISqlService
    {
        /// <inheritdoc />
        public IList<MessageReportModel> GetEcommerceMessages(MessageSearchModel searchData)
        {
            IList<MessageReportModel> reports = new List<MessageReportModel>();
            using (var sqlConnection = new SqlConnection(Constants.ConnectorDatabase))
            {
                try
                {
                    Query query = new Query("InboundQueue as MQE")
                        .Select("MQE.Id", "MQE.ErpId", "MQE.EcommerceId", "MQE.CreatedTime", "MQE.UpdatedTime", "MQE.StatusId", "MQE.SyncCounter", "MQE.EntityId", "MQE.ErrorMessage", "IE.EntityName", "MSL.StatusName", "MQE.EcommerceMessageId", "MQE.StatusId")
                        .Join("EntityMaster as IE", "MQE.EntityId", "IE.EntityId")
                        .Join("StatusMaster as MSL", "MQE.StatusId", "MSL.StatusId");

                    PrepareRequestParameters(query, searchData);
                    SqlResult sqlResult = new SqlServerCompiler().Compile(query);

                    using (var command = new SqlCommand(sqlResult.Sql, sqlConnection))
                    {
                        foreach (KeyValuePair<string, object> binding in sqlResult.Bindings)
                        {
                            command.Parameters.AddWithValue(binding.Key, binding.Value);
                        }
                        sqlConnection.Open();
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            int i = 1;
                            while (dataReader.Read())
                            {
                                var record = new MessageReportModel
                                {
                                    Index = i++,
                                    MessageId = dataReader.GetInt64(0),
                                    ErpId = dataReader.IsDBNull(1) ? string.Empty : dataReader.GetString(1),
                                    EcommerceId = dataReader.GetString(2),
                                    CreatedDate = dataReader.GetDateTime(3),
                                    ModifiedDate = dataReader.GetDateTime(4),
                                    SyncCounter = dataReader.GetByte(6),
                                    EntityId = dataReader.GetByte(7),
                                    ErrorMessage = dataReader.IsDBNull(8) ? "" : dataReader.GetString(8),
                                    EntityName = dataReader.GetString(9),
                                    MessageStatus = dataReader.GetString(10),
                                    CloudMessageId = dataReader.GetInt64(11),
                                    StatusId = dataReader.GetByte(12),
                                };
                                record.IsCheckBoxEnabled = record.StatusId == (int)MessageStatus.RequestProcessed || record.StatusId == (int)MessageStatus.Error;
                                record.ShowActionLink = !string.IsNullOrEmpty(record.ErrorMessage);
                                reports.Add(record);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogMessage(exception.Message, LogMessages.GetEcommerceMessages, LogType.Error, exception);
                    throw;
                }
                return reports;
            }
        }

        /// <inheritdoc />
        public IList<MessageReportModel> GetErpMessages(MessageSearchModel searchData)
        {
            IList<MessageReportModel> reports = new List<MessageReportModel>();
            using (var sqlConnection = new SqlConnection(Constants.ConnectorDatabase))
            {
                try
                {
                    Query query = new Query("OutboundQueue as MQE")
                        .Select("MQE.Id", "MQE.ErpId", "MQE.EcommerceId", "MQE.RecordTime", "MQE.CreatedTime", "MQE.UpdatedTime", "Mqe.StatusId as MessageStatusId", "MSL.StatusName as MessageStatus", "Mqe.SyncCounter", "Mqe.EntityId", "ie.EntityName as EntityName", "MQE.ErrorMessage", "MQE.StatusId", "MQE.EcommerceMessageId")
                        .Join("EntityMaster as IE", "MQE.EntityId", "IE.EntityId")
                        .Join("StatusMaster as MSL", "MQE.StatusId", "MSL.StatusId");

                    PrepareRequestParameters(query, searchData);
                    SqlResult sqlResult = new SqlServerCompiler().Compile(query);

                    using (var command = new SqlCommand(sqlResult.Sql, sqlConnection))
                    {
                        foreach (KeyValuePair<string, object> binding in sqlResult.Bindings)
                        {
                            command.Parameters.AddWithValue(binding.Key, binding.Value);
                        }
                        sqlConnection.Open();
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            int i = 1;
                            while (dataReader.Read())
                            {
                                var record = new MessageReportModel
                                {
                                    Index = i++,
                                    MessageId = dataReader.GetInt64(0),
                                    ErpId = dataReader.GetString(1),
                                    EcommerceId = dataReader.IsDBNull(2) ? null : dataReader.GetString(2),
                                    ReferenceTime = dataReader.GetDateTime(3),
                                    CreatedDate = dataReader.GetDateTime(4),
                                    ModifiedDate = dataReader.GetDateTime(5),
                                    MessageStatus = dataReader.GetString(7),
                                    EntityId = dataReader.GetByte(9),
                                    SyncCounter = dataReader.GetByte(8),
                                    EntityName = dataReader.GetString(10),
                                    ErrorMessage = dataReader.IsDBNull(11) ? "" : dataReader.GetString(11),
                                    StatusId = dataReader.GetByte(12),
                                    CloudMessageId = dataReader.IsDBNull(13) ? 0 : dataReader.GetInt64(13),
                                };

                                record.IsCheckBoxEnabled = record.StatusId == (int)MessageStatus.TransferPending || record.StatusId == (int)MessageStatus.Error || record.StatusId == (int)MessageStatus.RequestTransferred;
                                record.ShowActionLink = !string.IsNullOrEmpty(record.ErrorMessage);
                                reports.Add(record);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogMessage(exception.Message, LogMessages.GetErpMessages, LogType.Error, exception);
                    throw;
                }
                return reports;
            }
        }

        /// <summary>
        /// Prepares the request parameters.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="searchEntity">The search entity.</param>
        /// <returns></returns>
        private static void PrepareRequestParameters(Query query, MessageSearchModel searchEntity)
        {
            if (searchEntity.EntityId != 0) query.Where("Mqe.EntityId", searchEntity.EntityId);
            if (!string.IsNullOrEmpty(searchEntity.ErpId)) query.WhereLike("Mqe.ErpId", "%" + searchEntity.ErpId + "%");
            if (!string.IsNullOrEmpty(searchEntity.EcommerceId)) query.WhereLike("Mqe.EcommerceId", "%" + searchEntity.EcommerceId + "%");
            if (searchEntity.CreatedFromHasValue) query.Where("MQE.CreatedTime", ">=", searchEntity.CreatedFrom);
            if (searchEntity.CreatedToHasValue) query.Where("MQE.CreatedTime", "<=", searchEntity.CreatedTo);
            if (searchEntity.ModifiedFromHasValue) query.Where("MQE.UpdatedTime", ">=", searchEntity.ModifiedFrom);
            if (searchEntity.ModifiedToHasValue) query.Where("MQE.UpdatedTime", ">=", searchEntity.ModifiedTo);
            if (searchEntity.StatusId != 0) query.Where("Mqe.StatusId", searchEntity.StatusId);
            if (!string.IsNullOrEmpty(searchEntity.MessageId)) query.WhereLike("Mqe.Id", searchEntity.MessageId);
            if (searchEntity.SyncCounter.HasValue) query.Where("Mqe.SyncCounter", searchEntity.SyncCounter);
        }

        /// <inheritdoc />
        public IList<MessageCountModel> GetEcommerceMessageSummary(MessageSearchModel searchData)
        {
            var summary = new List<MessageCountModel>();
            using (var connection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = @"SELECT IE.EntityId,IE.EntityName,MS.StatusId,MS.StatusName,ISNULL(COUNT(MQ.Id),0) RECORDCOUNT FROM EntityMaster IE
                                                            JOIN InboundQueue MQ ON MQ.EntityId=IE.EntityId
                                                            JOIN StatusMaster MS ON MQ.StatusId=MS.StatusId AND MS.IsInboundActive=1
                                                            WHERE MQ.CreatedTime BETWEEN @FROMDATE AND @TODATE
                                                            GROUP BY IE.EntityId,IE.EntityName,MS.StatusId,MS.StatusName ORDER BY IE.EntityName";

                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@FROMDATE", searchData.CreatedFrom);
                    command.Parameters.AddWithValue("@TODATE", searchData.CreatedTo);

                    connection.Open();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            summary.Add(new MessageCountModel
                            {
                                EntityId = dataReader.GetByte(0),
                                EntityName = dataReader.GetString(1),
                                StatusId = dataReader.IsDBNull(2) ? 0 : dataReader.GetByte(2),
                                StatusName = dataReader.IsDBNull(3) ? "" : dataReader.GetString(3),
                                Count = dataReader.GetInt32(4),
                            });
                        }
                    }

                    connection.Close();
                }
            }
            return summary;
        }

        /// <inheritdoc />
        public IList<MessageCountModel> GetErpMessageSummary(MessageSearchModel searchData)
        {
            var summary = new List<MessageCountModel>();
            using (var connection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = @"SELECT IE.ENTITYID,IE.EntityName,MS.STATUSID,MS.STATUSNAME,ISNULL(COUNT(MQ.ID),0) RECORDCOUNT FROM EntityMaster IE
                                                            JOIN OutboundQueue MQ ON MQ.ENTITYID=IE.ENTITYID
                                                            JOIN StatusMaster MS ON MQ.StatusId=MS.STATUSID AND MS.IsOutboundActive=1
                                                            WHERE MQ.CREATEDTIME BETWEEN @FROMDATE AND @TODATE
                                                            GROUP BY IE.ENTITYID,IE.EntityName,MS.STATUSID,MS.STATUSNAME ORDER BY IE.EntityName";

                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@FROMDATE", searchData.CreatedFrom);
                    command.Parameters.AddWithValue("@TODATE", searchData.CreatedTo);

                    connection.Open();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            summary.Add(new MessageCountModel
                            {
                                EntityId = dataReader.GetByte(0),
                                EntityName = dataReader.GetString(1),
                                StatusId = dataReader.IsDBNull(2) ? 0 : dataReader.GetByte(2),
                                StatusName = dataReader.IsDBNull(3) ? "" : dataReader.GetString(3),
                                Count = dataReader.GetInt32(4),
                            });
                        }
                    }

                    connection.Close();
                }
            }

            return summary;
        }

        /// <inheritdoc />
        public IList<IntegerComboBoxModel> GetForwardEntities()
        {
            var entities = new List<IntegerComboBoxModel>();
            using (var sqlConnection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = "Select EntityId,EntityName from EntityMaster where IsInboundActive=1 order by EntityName";
                using (var sqlCommand = new SqlCommand(commandText, sqlConnection))
                {
                    sqlConnection.Open();
                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            entities.Add(new IntegerComboBoxModel
                            {
                                Value = dataReader.GetByte(0),
                                Name = dataReader.GetString(1),
                            });
                        }
                    }
                }
            }
            return entities;
        }

        /// <inheritdoc />
        public IList<IntegerComboBoxModel> GetForwardStatuses()
        {
            var statuses = new List<IntegerComboBoxModel>();
            using (var sqlConnection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = "Select StatusId,StatusName from StatusMaster where IsInboundActive=1";
                using (var sqlCommand = new SqlCommand(commandText, sqlConnection))
                {
                    sqlConnection.Open();
                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            statuses.Add(new IntegerComboBoxModel
                            {
                                Value = dataReader.GetByte(0),
                                Name = dataReader.GetString(1),
                            });
                        }
                    }
                }
            }
            return statuses;
        }

        /// <inheritdoc />
        public IList<IntegerComboBoxModel> GetReverseEntities()
        {
            var entities = new List<IntegerComboBoxModel>();
            using (var sqlConnection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = "Select EntityId,EntityName from EntityMaster where IsOutboundActive=1 order by EntityName";
                using (var sqlCommand = new SqlCommand(commandText, sqlConnection))
                {
                    sqlConnection.Open();
                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            entities.Add(new IntegerComboBoxModel
                            {
                                Value = dataReader.GetByte(0),
                                Name = dataReader.GetString(1),
                            });
                        }
                    }
                }
            }
            return entities;
        }

        /// <inheritdoc />
        public IList<IntegerComboBoxModel> GetReverseStatuses()
        {
            var statuses = new List<IntegerComboBoxModel>();
            using (var sqlConnection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = "Select StatusId,StatusName from StatusMaster where IsOutboundActive=1";
                using (var sqlCommand = new SqlCommand(commandText, sqlConnection))
                {
                    sqlConnection.Open();
                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            statuses.Add(new IntegerComboBoxModel
                            {
                                Value = dataReader.GetByte(0),
                                Name = dataReader.GetString(1),
                            });
                        }
                    }
                }
            }
            return statuses;
        }

        /// <inheritdoc />
        public decimal GetErpOrderValue(MessageSearchModel searchData)
        {
            try
            {
                decimal orderValue = 0;
                using (var sqlConnection = new SqlConnection(Constants.ConnectorDatabase))
                {
                    sqlConnection.Open();
                    using (var command = new SqlCommand("SELECT object_id FROM sys.all_columns WHERE name='GrandTotal' and object_id = OBJECT_ID(N'[dbo].[Order]')", sqlConnection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            goto FetchData;
                        }
                        return orderValue;
                    }
                    FetchData:
                    using (var command = new SqlCommand("SELECT ISNULL(SUM(ISNULL(GrandTotal,0)),0) OrderValue FROM [Order] WHERE CreateDateTime BETWEEN @StartTime and @EndTime AND Origin=@Origin", sqlConnection))
                    {
                        command.Parameters.AddWithValue("@StartTime", searchData.CreatedFrom);
                        command.Parameters.AddWithValue("@EndTime", searchData.CreatedTo);
                        command.Parameters.AddWithValue("@Origin", Constants.OriginErp);

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            if (dataReader.Read())
                            {
                                orderValue = Convert.ToDecimal(dataReader["OrderValue"], CultureInfo.CurrentCulture);
                            }
                        }
                    }
                }
                return orderValue;
            }
            catch (Exception exception)
            {
                Logger.LogMessage(exception.Message, LogMessages.GetErpOrderValue, LogType.Error, exception);
                throw;
            }
        }

        /// <inheritdoc />
        public decimal GetMagentoOrderValue(MessageSearchModel searchData)
        {
            try
            {
                decimal orderValue = 0;
                using (var sqlConnection = new SqlConnection(Constants.ConnectorDatabase))
                {
                    sqlConnection.Open();

                    using (var command = new SqlCommand("SELECT object_id FROM sys.all_columns WHERE name='GrandTotal' and object_id = OBJECT_ID(N'[dbo].[Order]')", sqlConnection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            goto FetchData;
                        }
                        return orderValue;
                    }
                    FetchData:
                    using (var command = new SqlCommand("SELECT ISNULL(SUM(ISNULL(GrandTotal,0)),0) OrderValue FROM [Order] WHERE CreateDateTime BETWEEN @StartTime and @EndTime AND Origin=@Origin", sqlConnection))
                    {
                        command.Parameters.AddWithValue("@StartTime", searchData.CreatedFrom);
                        command.Parameters.AddWithValue("@EndTime", searchData.CreatedTo);
                        command.Parameters.AddWithValue("@Origin", Constants.OriginEcommerce);

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            if (dataReader.Read())
                            {
                                orderValue = Convert.ToDecimal(dataReader["OrderValue"], CultureInfo.CurrentCulture);
                            }
                        }
                    }
                }
                return orderValue;
            }
            catch (Exception exception)
            {
                Logger.LogMessage(exception.Message, LogMessages.GetMagentoOrderValue, LogType.Error, exception);
                throw;
            }
        }

        /// <inheritdoc />
        public int GetEcommerceRecordCount(MessageSearchModel searchData)
        {
            try
            {
                int recordCount = 0;
                using (var sqlConnection = new SqlConnection(Constants.ConnectorDatabase))
                {
                    sqlConnection.Open();

                    const string commandText = @"SELECT SUM(ISNULL(A.RecordCount,0)) RecordCount from (
                                                    SELECT SUM(RecordCount) RecordCount from InboundQueueHistory WHERE [Date] BETWEEN @StartTime AND @EndTime
                                                    UNION SELECT COUNT(MQ.Id) from InboundQueue MQ WHERE MQ.CreatedTime>(SELECT ISNULL(MAX(CreatedTime),'1/1/1990') from InboundQueueHistory) AND MQ.CreatedTime BETWEEN @StartTime and @EndTime ) AS A";

                    using (var command = new SqlCommand(commandText, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@StartTime", searchData.CreatedFrom);
                        command.Parameters.AddWithValue("@EndTime", searchData.CreatedTo);

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            if (dataReader.Read())
                            {
                                recordCount = Convert.ToInt32(dataReader["RecordCount"], CultureInfo.CurrentCulture);
                            }
                        }
                    }
                }
                return recordCount;
            }
            catch (Exception exception)
            {
                Logger.LogMessage(exception.Message, LogMessages.GetEcommerceRecordCount, LogType.Error, exception);
                throw;
            }
        }

        /// <inheritdoc />
        public int GetErpRecordCount(MessageSearchModel searchData)
        {
            try
            {
                int recordCount = 0;
                using (var sqlConnection = new SqlConnection(Constants.ConnectorDatabase))
                {
                    sqlConnection.Open();

                    const string commandText = @"SELECT SUM(ISNULL(A.RecordCount,0)) RecordCount from (
                                                    SELECT SUM(RecordCount) RecordCount from OutboundQueueHistory WHERE [Date] BETWEEN @StartTime AND @EndTime
                                                    UNION SELECT COUNT(MQ.Id) from OutboundQueue MQ WHERE MQ.CreatedTime>(SELECT ISNULL(MAX(CreatedTime),'1/1/1990') from OutboundQueueHistory) AND MQ.CreatedTime BETWEEN @StartTime and @EndTime ) AS A";

                    using (var command = new SqlCommand(commandText, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@StartTime", searchData.CreatedFrom);
                        command.Parameters.AddWithValue("@EndTime", searchData.CreatedTo);

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            if (dataReader.Read())
                            {
                                recordCount = Convert.ToInt32(dataReader["RecordCount"], CultureInfo.CurrentCulture);
                            }
                        }
                    }
                }
                return recordCount;
            }
            catch (Exception exception)
            {
                Logger.LogMessage(exception.Message, LogMessages.GetErpRecordCount, LogType.Error, exception);
                throw;
            }
        }
    }
}