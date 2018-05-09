using System.Collections.Generic;
using System.Data.SqlClient;
using I95Dev.Connector.Base.Common;
using I95Dev.Connector.UI.Base.Models;

namespace I95Dev.Connector.UI.Base.DataAccess
{
    internal class DocumentTypeDataAccess : IDocumentTypeDataAccess
    {
        public IList<DocumentType> GetDocumentMappings()
        {
            var comboBoxGpDocuments = new List<DocumentType>();
            using (var connection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = @"select Id,SopType,ERPDocId,Origin from DocumentTypes order by ERPDocId";
                using (var command = new SqlCommand(commandText, connection))
                {
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        comboBoxGpDocuments.Add(new DocumentType
                        {
                            Id = reader.GetInt32(0),
                            SopType = reader.GetInt32(1),
                            ErpDocumentId = reader.GetString(2),
                            Orgin = reader.GetString(3)
                        });
                    }
                }
            }
            return comboBoxGpDocuments;
        }

        /// <inheritdoc />
        public bool CheckDocumentTypeCombination(DocumentType documentType)
        {
            bool result;
            using (var connection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = @"Select Id from DocumentTypes where SopType=@SopType AND ERPDocId=@ERPDocId AND Origin=@Origin AND Id<>@Id";
                using (var command = new SqlCommand(commandText, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@Origin", documentType.Orgin);
                    command.Parameters.AddWithValue("@ERPDocId", documentType.ErpDocumentId);
                    command.Parameters.AddWithValue("@SopType", documentType.SopType);
                    command.Parameters.AddWithValue("@Id", documentType.Id);

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        result = dataReader.Read();
                    }
                    connection.Close();
                }
            }
            return result;
        }

        /// <inheritdoc />
        public bool SaveDocumentTypeDetails(DocumentType documentType)
        {
            using (var connection = new SqlConnection(Constants.ConnectorDatabase))
            {
                var cmd = new SqlCommand
                {
                    CommandType = System.Data.CommandType.Text,
                    CommandText = @"IF(@Id>0)
                    Update DocumentTypes SET Origin=@Origin,ERPDocId=@ERPDocId,SopType=@SopType where Id=@Id
                    else
                    Insert into DocumentTypes (Origin,ERPDocId,SopType) values(@Origin,@ERPDocId,@SopType);"
                };
                cmd.Parameters.AddWithValue("@Origin", documentType.Orgin);
                cmd.Parameters.AddWithValue("@ERPDocId", documentType.ErpDocumentId);
                cmd.Parameters.AddWithValue("@SopType", documentType.SopType);
                cmd.Parameters.AddWithValue("@Id", documentType.Id);
                cmd.Connection = connection;
                connection.Open();
                int i = cmd.ExecuteNonQuery();
                return i > 0;
            }
        }

        /// <inheritdoc />
        public bool IsDocumentTypeTableExists()
        {
            using (var connection = new SqlConnection(Constants.ConnectorDatabase))
            {
                const string commandText = @"select name from sys.tables where name='DocumentTypes'";
                using (var command = new SqlCommand(commandText, connection))
                {
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    return reader.HasRows;
                }
            }
        }

        public IList<DocumentType> LoadDocumentTypes()
        {
            var comboBoxGpDocuments = new List<DocumentType>();
            using (var connection = new SqlConnection(Constants.ErpDatabase))
            {
                const string commandText = @"select SOPTYPE,DOCID from SOP40200 ORDER by DOCID";
                using (var command = new SqlCommand(commandText, connection))
                {
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        comboBoxGpDocuments.Add(new DocumentType
                        {
                            ErpDocumentId = reader.GetString(1).TrimEnd(),
                            SopType = reader.GetInt16(0)
                        });
                    }
                }
            }
            return comboBoxGpDocuments;
        }
    }
}