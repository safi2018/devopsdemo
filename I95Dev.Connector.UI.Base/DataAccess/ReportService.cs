using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using I95Dev.Connector.Base.Common;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.ViewModels.Helpers;

namespace I95Dev.Connector.UI.Base.DataAccess
{
    internal class ReportService : IReportService
    {
        /// <inheritdoc />
        public IList<IntegerComboBoxModel> GetCategories()
        {
            IList<IntegerComboBoxModel> categories = new List<IntegerComboBoxModel>();

            using (var sqlConnection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = "SELECT Id,CategoryName from EventCategories";
                using (var sqlCommand = new SqlCommand(commandText, sqlConnection))
                {
                    sqlConnection.Open();
                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            categories.Add(new IntegerComboBoxModel
                            {
                                Value = dataReader.GetInt32(0),
                                Name = dataReader.GetString(1),
                            });
                        }
                    }
                }
            }

            return categories;
        }

        /// <inheritdoc />
        public IList<ExclusionModel> GetExclusionsData(ExclusionsSearchModel searchModel)
        {
            var exclusions = new List<ExclusionModel>();
            using (var connection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = @"SELECT EX.Id,EX.RecordId,EX.CreatedTime,EX.IsJson,EX.Parameters, IE.EntityName
                                                        FROM Exclusions EX
                                                        JOIN EntityMaster IE ON ex.EntityId=IE.EntityId
                                                        WHERE EX.RecordId LIKE '%'+ISNULL(@RECORDID,EX.RecordId)+'%'
                                                        AND EX.CreatedTime >=ISNULL(@CREATEDTIMES,EX.CreatedTime)
                                                        AND EX.CreatedTime <=ISNULL(@CREATEDTIMEE,EX.CreatedTime)
                                                        order by EX.CreatedTime DESC";

                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@ENTITYID", searchModel.EntityId == 0 ? DBNull.Value : (object)searchModel.EntityId);
                    command.Parameters.AddWithValue("@RECORDID", string.IsNullOrEmpty(searchModel.RecordNumber) ? DBNull.Value : (object)searchModel.RecordNumber);
                    command.Parameters.AddWithValue("@CREATEDTIMES", searchModel.FromDate.HasValue ? (object)searchModel.FromDate.Date : DBNull.Value);
                    command.Parameters.AddWithValue("@CREATEDTIMEE", searchModel.ToDate.HasValue ? (object)searchModel.ToDate.Date : DBNull.Value);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    int i = 1;
                    while (reader.Read())
                    {
                        exclusions.Add(new ExclusionModel
                        {
                            Index = i++,
                            Id = reader.GetInt64(0),
                            RecordId = reader.GetString(1),
                            CreatedTime = reader.GetDateTime(2),
                            IsJson = reader.GetBoolean(3),
                            Parameters = reader.GetString(4),
                            EntityName = reader.GetString(5)
                        });
                    }
                }
            }
            return exclusions;
        }

        /// <inheritdoc />
        public IList<NotificationModel> GetNotificationsData(NotificationsSearchModel searchModel)
        {
            var notifications = new List<NotificationModel>();
            using (var connection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = @"SELECT ED.Id,ED.EventCategoryId,ec.CategoryName,ED.CreatedTime,ED.IsMailSent,ED.Description,ED.LongDescription
                                                        FROM EventDetails ED
                                                        join EventCategories EC on ED.EventCategoryId=ec.Id
                                                        WHERE ED.IsMailSent =ISNULL(@IsMailSent,ED.IsMailSent)
                                                        AND ED.EventCategoryId =ISNULL(@EventCategoryId,ED.EventCategoryId)
                                                        AND ED.CreatedTime >=ISNULL(@CreatedTimeS,ED.CreatedTime)
                                                        AND ED.CreatedTime <=ISNULL(@CreatedTimeE,ED.CreatedTime)
                                                        order by ED.CreatedTime DESC";

                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@IsMailSent", searchModel.IsAll ? DBNull.Value : (object)searchModel.IsYes);
                    command.Parameters.AddWithValue("@EventCategoryId", searchModel.CategoryId == 0 ? DBNull.Value : (object)searchModel.CategoryId);
                    command.Parameters.AddWithValue("@CreatedTimeS", searchModel.FromDate.HasValue ? (object)searchModel.FromDate.Date : DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedTimeE", searchModel.ToDate.HasValue ? (object)searchModel.ToDate.Date : DBNull.Value);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    int i = 1;
                    while (reader.Read())
                    {
                        notifications.Add(new NotificationModel
                        {
                            Index = i++,
                            Id = reader.GetInt64(0),
                            CategoryId = reader.GetInt32(1),
                            CategoryName = reader.GetString(2),
                            CreatedTime = reader.GetDateTime(3),
                            IsMailSent = reader.GetBoolean(4) ? "Yes" : "No",
                            Description = reader.GetString(5),
                            DetailDescription = reader.GetString(6),
                            ViewRecord = "View"
                        });
                    }
                }
            }
            return notifications;
        }
    }
}