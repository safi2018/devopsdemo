using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using I95Dev.Connector.Base.Common;
using I95Dev.Connector.Base.Helpers;
using I95Dev.Connector.Base.Models.MessageQueue;
using I95Dev.Connector.UI.Base.Models;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace I95Dev.Connector.UI.Base.Services
{
    internal static class DiagnosticsService
    {
        /// <summary>
        /// Checks the SQL permission.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        internal static DiagnosticsResponseModel CheckSqlPermission(DiagnosticsModel data)
        {
            var response = new DiagnosticsResponseModel
            {
                Code = "SqlServer",
                Name = "Sql server access permissions working?",
                Comment = "Access permissions are working",
            };
            using (var sqlConnection = new SqlConnection(data.ConnectionString))
            {
                try
                {
                    sqlConnection.Open();
                    sqlConnection.Close();
                    response.IsSuccess = true;
                }
                catch (Exception exception)
                {
                    response.Comment = exception.Message;
                }
            }
            return response;
        }

        internal static DiagnosticsResponseModel CheckGPeConnect(DiagnosticsModel data)
        {
            var response = new DiagnosticsResponseModel
            {
                Code = "GPeConnect",
                Name = "GP eConnect working?",
                Comment = "GP eConnect working",
            };
            try
            {
                ServiceController eConnectService = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName.Contains("eConnect"));

                if (eConnectService != null)
                {
                    data.GPVersion = eConnectService.ServiceName;
                    if (eConnectService.Status.Equals(ServiceControllerStatus.Running))
                    {
                        response.Comment = "GP eConnect is running";
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.Comment = "GP eConnect is available but not running";
                        response.IsSuccess = false;
                    }
                }
                else
                {
                    response.Comment = "eConnect not available ";
                }
            }
            catch (Exception ex)
            {
                response.Comment = ex.Message;
                return response;
            }
            return response;
        }

        internal static DiagnosticsResponseModel CheckWebServicesRunning(DiagnosticsModel data)
        {
            var response = new DiagnosticsResponseModel
            {
                Code = "WebServices",
                Name = "WebServices working?",
                Comment = "WebServices are working",
            };
            if (IsWebservicesInstalledInSystem(data.GPVersion))
            {
                string dbName = string.Empty;
                int count = 0;
                var sqlConnection = new SqlConnection(data.ConnectionString);
                try
                {
                    sqlConnection.Open();
                    var companyCommand = new SqlCommand("SELECT name FROM master.sys.databases WHERE name = '" + data.SelectedDatabase + "'", sqlConnection);
                    SqlDataReader sqldatareader = companyCommand.ExecuteReader();
                    if (sqldatareader.Read())
                    {
                        dbName = sqldatareader[0].ToString();
                    }
                    sqldatareader.Close();
                    sqlConnection.Close();
                }
                catch (Exception ex)
                {
                    response.Comment = ex.Message;
                }
                if (string.IsNullOrEmpty(dbName)) return response;
                {
                    var sqlConnection1 = new SqlConnection(data.ConnectionString);
                    try
                    {
                        sqlConnection1.Open();
                        SqlCommand companyCommand = new SqlCommand
                        {
                            Connection = sqlConnection1,
                            CommandText = "SELECT Count(*) FROM eConnect_Out_Setup",
                            CommandType = CommandType.Text
                        };
                        SqlDataReader sqldatareader = companyCommand.ExecuteReader();
                        if (sqldatareader.Read())
                        {
                            count = Convert.ToInt16(sqldatareader[0], CultureInfo.CurrentCulture);
                        }
                        sqlConnection1.Close();
                        if (count >= 464)
                        {
                            response.IsSuccess = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Comment = ex.Message;
                    }
                }
            }
            else
            {
                response.Comment = "WebServices not installed ";
            }
            return response;
        }

        private static bool IsWebservicesInstalledInSystem(string serv)
        {
            string servName = "";
            switch (serv)
            {
                case "DynGP9eConnect":
                    servName = "GP 9 ";
                    break;

                case "DynGP10eConnect":
                    servName = "GP 10 ";
                    break;

                case "DynGP11eConnect":
                    servName = "GP 2010";
                    break;

                case "DynGP12eConnect":
                    servName = "GP 2013";
                    break;

                case "DynGP14eConnect":
                    servName = "GP 2015";
                    break;

                case "DynGP16eConnect":
                    servName = "GP 2016";
                    break;

                case "DynGP18eConnect":
                    servName = "GP 2018";
                    break;
            }
            string pName = "eConnect for Microsoft Dynamics " + servName;

            const string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey))
            {
                if (key == null) return false;
                foreach (string subkeyName in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkeyName))
                    {
                        if (subkey == null) continue;
                        string displayName = subkey.GetValue("DisplayName") as string;
                        if (pName.Equals(displayName, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks the magento is running.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        internal static DiagnosticsResponseModel CheckMagentoIsRunning(DiagnosticsModel data)
        {
            var response = new DiagnosticsResponseModel
            {
                Code = "Magento",
                Name = "Magento is working?",
                Comment = "Magento is working fine"
            };
            try
            {
                HttpStatusCode responseCode = GetHttpResponse(data.ServiceUrl);
                if (responseCode == HttpStatusCode.OK)
                    response.IsSuccess = true;
                else
                {
                    response.Comment = "Magento services returned status code :" + responseCode;
                }
            }
            catch (WebException exception)
            {
                if (exception.Response != null)
                {
                    using (Stream stream = exception.Response.GetResponseStream())
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                response.Comment = reader.ReadToEnd();
                            }
                        else
                        {
                            response.Comment = exception.Message;
                        }
                }
                else
                    response.Comment = exception.Message;
            }
            catch (Exception exception)
            {
                response.Comment = exception.Message;
            }
            return response;
        }

        /// <summary>
        /// Checks the magento connector is running.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        internal static DiagnosticsResponseModel CheckMagentoConnectorIsRunning(DiagnosticsModel data)
        {
            var response = new DiagnosticsResponseModel
            {
                Code = "MagentoConnector",
                Name = "i95Dev Connector is installed in Magento?",
                Comment = "i95Dev Connector is working fine"
            };
            try
            {
                if (!data.ServiceUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase)) data.ServiceUrl += "/";

                string token = GetToken(data);

                response.IsSuccess = CheckExtension(data, token);
            }
            catch (WebException exception)
            {
                if (exception.Response != null)
                {
                    using (Stream stream = exception.Response.GetResponseStream())
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                string magentoResponse = reader.ReadToEnd();
                                if (IsJson(magentoResponse))
                                {
                                    var responseMessage = JsonConvert.DeserializeObject<Dictionary<string, string>>(magentoResponse);
                                    response.Comment = responseMessage.ContainsKey("message") ? responseMessage["message"] : magentoResponse;
                                }
                                else
                                    response.Comment = magentoResponse;
                            }
                }
                else
                    response.Comment = exception.Message;
            }
            catch (Exception exception)
            {
                response.Comment = exception.Message;
            }
            return response;
        }

        /// <summary>
        /// Determines whether the specified data is json.
        /// </summary>
        /// <param name="jsonData">The json data.</param>
        /// <returns>
        ///   <c>true</c> if the specified data is json; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsJson(string jsonData)
        {
            return jsonData.Trim().Substring(0, 1).IndexOfAny(new[] { '[', '{' }) == 0;
        }

        /// <summary>
        /// Saves to database.
        /// </summary>
        /// <param name="responses">The responses.</param>
        /// <param name="validationId">The validation identifier.</param>
        internal static void SaveToDatabase(IEnumerable<DiagnosticsResponseModel> responses, string validationId)
        {
            try
            {
                const string commandText = @"IF NOT EXISTS (SELECT Id from DiagnosticResults WHERE Id=@Id and ValidationCode=@ValidationCode)
                                                INSERT INTO DiagnosticResults(Id,CreatedTime,UpdatedTime,ValidationCode,ValidationName,ValidationResult,IsSuccess)
                                                     VALUES(@Id,@CreatedTime,@UpdatedTime,@ValidationCode,@ValidationName,@ValidationResult,@IsSuccess)
                                                ELSE UPDATE DiagnosticResults SET ValidationName=@ValidationName,UpdatedTime=@UpdatedTime,ValidationResult=@ValidationResult,IsSuccess=@IsSuccess
                                                WHERE Id=@Id and ValidationCode=@ValidationCode";
                using (var connection = new SqlConnection(Constants.ConnectorDatabase))
                {
                    using (var command = new SqlCommand(commandText, connection))
                    {
                        connection.Open();
                        foreach (DiagnosticsResponseModel response in responses)
                        {
                            try
                            {
                                command.Parameters.AddWithValue("@Id", validationId);
                                command.Parameters.AddWithValue("@ValidationCode", response.Code);
                                command.Parameters.AddWithValue("@ValidationName", response.Name);
                                command.Parameters.AddWithValue("@ValidationResult", response.Comment);
                                command.Parameters.AddWithValue("@IsSuccess", response.IsSuccess);
                                command.Parameters.AddWithValue("@CreatedTime", DateTime.Now);
                                command.Parameters.AddWithValue("@UpdatedTime", DateTime.Now);

                                command.ExecuteNonQuery();
                                command.Parameters.Clear();
                            }
                            catch (Exception exception)
                            {
                                Logger.LogMessage(exception.Message, "SaveToDatabase", LogType.Error, exception);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogMessage(exception.Message, "SaveToDatabase", LogType.Error, exception);
            }
        }

        /// <summary>
        /// Gets the HTTP response.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <returns></returns>
        private static HttpStatusCode GetHttpResponse(string requestUrl)
        {
            var httpReq = (HttpWebRequest)WebRequest.Create(requestUrl);
            httpReq.AllowAutoRedirect = false;
            httpReq.Timeout = 30000;
            httpReq.Credentials = CredentialCache.DefaultNetworkCredentials;
            using (var httpRes = (HttpWebResponse)httpReq.GetResponse())
                return httpRes.StatusCode;
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private static string GetToken(DiagnosticsModel data)
        {
            string result = string.Empty;
            string boundary = string.Format(Constants.DefaultCulture, "----------{0:N}", Guid.NewGuid());
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(data.ServiceUrl + "rest/V1/integration/admin/token");
            httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 150000;

            var sb = new StringBuilder();

            sb.AppendLine(string.Format(Constants.DefaultCulture, "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}", boundary, "username", data.MagentoUserName));
            sb.AppendLine(string.Format(Constants.DefaultCulture, "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}", boundary, "password", data.MagentoPassword));

            string inputBody = sb.ToString();

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(inputBody);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (Stream stream = httpResponse.GetResponseStream())
            {
                if (stream == null) return result;
                using (var streamReader = new StreamReader(stream))
                {
                    string webResponse = streamReader.ReadToEnd();
                    result = JsonConvert.DeserializeObject<string>(webResponse);
                }
            }
            return result;
        }

        /// <summary>
        /// Checks the extension.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        private static bool CheckExtension(DiagnosticsModel data, string token)
        {
            bool result = false;
            var request = (HttpWebRequest)WebRequest.Create(data.ServiceUrl + "rest/V1/modules");
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token);
            request.Timeout = 150000;

            var response = (HttpWebResponse)request.GetResponse();
            using (Stream stream = response.GetResponseStream())
                if (stream != null)
                    using (var streamReader = new StreamReader(stream))
                    {
                        string responseData = streamReader.ReadToEnd();
                        if (string.IsNullOrEmpty(responseData)) return false;
                        var modulesList = JsonConvert.DeserializeObject<string[]>(responseData);
                        result = modulesList.Any(c1 => c1.Contains("I95DevConnect"));
                    }
            return result;
        }

        /// <summary>
        /// Checks the cloud is running.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static DiagnosticsResponseModel CheckCloudIsRunning(DiagnosticsModel data)
        {
            var response = new DiagnosticsResponseModel
            {
                Code = "Cloud",
                Name = "Cloud is working?",
                Comment = "Cloud is working fine"
            };
            try
            {
                HttpStatusCode responseCode = GetPostHttpResponse(data.ServiceUrl);
                if (responseCode == HttpStatusCode.OK)
                    response.IsSuccess = true;
                else
                {
                    response.Comment = "Cloud Server returned status code :" + responseCode;
                }
            }
            catch (WebException exception)
            {
                if (exception.Response != null)
                {
                    using (Stream stream = exception.Response.GetResponseStream())
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                response.Comment = reader.ReadToEnd();
                            }
                        else
                        {
                            response.Comment = exception.Message;
                        }
                }
                else
                    response.Comment = exception.Message;
            }
            catch (Exception exception)
            {
                response.Comment = exception.Message;
            }
            return response;
        }

        private static HttpStatusCode GetPostHttpResponse(string cloudUrl)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(cloudUrl + "/index");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 150000;
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write("");
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            return httpResponse.StatusCode;
        }

        public static DiagnosticsResponseModel CheckSubscription(DiagnosticsModel data)
        {
            var response = new DiagnosticsResponseModel
            {
                Code = "Subscription",
                Name = "Subscription is active?",
                Comment = "Subscription is active"
            };

            try
            {
                bool responseCode = CheckSubscriptionStatus(data);
                if (responseCode)
                    response.IsSuccess = true;
                else
                {
                    response.Comment = "Subscription is not active";
                }
            }
            catch (WebException exception)
            {
                if (exception.Response != null)
                {
                    using (Stream stream = exception.Response.GetResponseStream())
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                response.Comment = reader.ReadToEnd();
                            }
                        else
                        {
                            response.Comment = exception.Message;
                        }
                }
                else
                    response.Comment = exception.Message;
            }
            catch (Exception exception)
            {
                response.Comment = exception.Message;
            }
            return response;
        }

        private static bool CheckSubscriptionStatus(DiagnosticsModel data)
        {
            var requestModel = new RequestModel()
            {
                Context = new ContextModel()
                {
                    ClientId = ConfigurationHelper.GetConfigurationValue(ConfigurationConstants.CustomerId),
                    SubscriptionKey = ConfigurationHelper.GetConfigurationValue(ConfigurationConstants.SubscriptionKey),
                    InstanceType = Constants.InstanceType,
                    RequestType = "Target",
                    SchedulerType = "PullData"
                }
            };

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(data.ServiceUrl + "/index/subscription");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 150000;
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(JsonHelper.JsonEncode(requestModel));
                streamWriter.Flush();
                streamWriter.Close();
            }
            var response = (HttpWebResponse)httpWebRequest.GetResponse();

            using (Stream stream = response.GetResponseStream())
                if (stream != null)
                    using (var streamReader = new StreamReader(stream))
                    {
                        string responseData = streamReader.ReadToEnd();
                        if (string.IsNullOrEmpty(responseData)) return false;
                        var responseModel = JsonConvert.DeserializeObject<ResponseModel>(responseData);
                        if (!responseModel.Result && !string.IsNullOrEmpty(responseModel.Message)) throw new Exception(responseModel.Message);
                        return responseModel.Result;
                    }

            return true;
        }
    }
}