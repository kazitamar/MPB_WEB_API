using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Net;

namespace web_api_me
{
    public class MPB
    {
        private readonly string stringConnection;
        private readonly SqlConnection MConn;
        public MPB()
        {
            var configuation = GetConfiguration();
            stringConnection = configuation.GetSection("AppSettings").GetSection("DbConnection").Value;
            MConn = new SqlConnection(stringConnection);
            /*try
            {
                MConn.Open();
            }
            catch (Exception)
            {
                throw new Exception("DB");
            }
            finally
            {
                MConn.Close();
            }*/
        }

        static System.Text.Encoding ISO_8859_1_ENCODING = System.Text.Encoding.GetEncoding("ISO-8859-1");
        public (string, string) GetUsernameAndPasswordFromAuthorizeHeader(string authorizeHeader)
        {
            if (authorizeHeader == null || !authorizeHeader.Contains("Basic "))
                return (null, null);

            string encodedUsernamePassword = authorizeHeader.Substring("Basic ".Length).Trim();
            string usernamePassword = ISO_8859_1_ENCODING.GetString(Convert.FromBase64String(encodedUsernamePassword));

            string username = usernamePassword.Split(':')[0];
            string password = usernamePassword.Split(':')[1];

            return (username, password);
        }

        public bool isAuthorize(string authorizeHeader)
        {
            try
            {
                (string userid, string pass) = GetUsernameAndPasswordFromAuthorizeHeader(authorizeHeader);
                if (userid == null || pass == null)
                    return false;
                if (IsPassMatchUser(userid, pass))
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool IsPassMatchUser(string userId, string pass)
        {
            try
            {
                int UserId = int.Parse(userId);
                if (UserId == 0)
                    return false;
                if (!IsUserIdExists(UserId))
                    return false;
                if (pass == "")
                    return false;
                if (pass.Length > 20)
                    return false;
                SqlCommand command = new SqlCommand("select 1 from USERS where USER_ID = @userId and PASSWORD = @pass");
                command.Parameters.Add("@userId", SqlDbType.Int);
                command.Parameters["@userId"].Value = UserId;
                command.Parameters.Add("@pass", SqlDbType.NVarChar);
                command.Parameters["@pass"].Value = pass;
                DataTable dt = GetRawData(command);
                if (dt.Rows.Count > 0)
                    return true;
                return false;
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MPB.IsPassMatchUser", e.Message, 0);
                throw e;
            }
        }

        public IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }

        private bool IsValidSql(string sqlCommand)
        {
            try
            { 
                string[] ForbiddenWords = new string[] { "DROP", "CREATE", "ALTER", "GRANT", "DENY", "BEGIN", "--", "TABLE", "KILL", "OPEN", "EXEC", "@@", "CURSOR", "FETCH" };
                foreach (var forbiddenWord in ForbiddenWords)
                {
                    if (sqlCommand.ToUpper().Contains(forbiddenWord))
                        return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MPB.IsValidSql", e.Message, 0);
                return false;
            }
        }

        protected string GetData(SqlCommand command)
        {
            try
            {
                DataTable dt = GetRawData(command);
                if (dt.Rows.Count > 0)
                    return JsonConvert.SerializeObject(dt);
                else
                    return "No Data";
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MPB.GetData", e.Message, 0);
                return "Error";
            }
}

        protected DataTable GetRawData(SqlCommand command)
        {
            try
            { 
                if (!IsValidSql(command.CommandText))
                    throw new Exception("SQL Injection attack prevented");
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(command.CommandText, MConn);
                foreach (SqlParameter param in command.Parameters)
                {
                    da.SelectCommand.Parameters.Add(param.ParameterName, param.SqlDbType);
                    da.SelectCommand.Parameters[param.ParameterName].Value = param.Value;
                }
                da.Fill(dt);
                return dt;
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MPB.GetRawData", e.Message, 0);
                throw e;
            }
}

        protected int EditData(SqlCommand command)
        {
            try
            { 
                if (!IsValidSql(command.CommandText))
                    throw new Exception("SQL Injection attack prevented");
                command.Connection = MConn;
                MConn.Open();
                int rowsAffected = command.ExecuteNonQuery();
                MConn.Close();
                return rowsAffected;
            }
            catch (Exception e)
            {
                MConn.Close();
                Insert2ErrorTable("MPB.EditData", e.Message, 0);
                throw e;
            }
}

        protected bool IsUserIdExists(int userId)
        {
            try
            {
                SqlCommand command = new SqlCommand("select 1 from USERS where USER_ID = @UserId");
                command.Parameters.Add("@UserId", SqlDbType.Int);
                command.Parameters["@UserId"].Value = userId;
                DataTable dt = GetRawData(command);
                if (dt.Rows.Count > 0)
                    return true;
                return false;
            }
            catch (Exception e)
            {
                Insert2ErrorTable("MPB.IsUserIdExists", e.Message, userId);
                throw e;
            }
        }

        public void Insert2ErrorTable(string functionName, string error,int userId)
        {
            try
            {
                if (error.Contains("The server was not found or was not accessible"))
                {
                    return;
                }
                SqlCommand command = new SqlCommand("insert into ERRORS (DATE_TIME,FUNCTION_NAME,ERROR,USER_ID) values (GETDATE(),@FunctionName,@Error,@UserId)");
                command.Parameters.Add("@FunctionName", SqlDbType.NVarChar);
                command.Parameters["@FunctionName"].Value = functionName;
                command.Parameters.Add("@Error", SqlDbType.NText);
                command.Parameters["@Error"].Value = error;
                command.Parameters.Add("@UserId", SqlDbType.Int);
                command.Parameters["@UserId"].Value = userId;
                command.Connection = MConn;
                MConn.Open();
                command.ExecuteNonQuery();
                MConn.Close();
            }
            catch (Exception)
            {
                MConn.Close();
            }
        }

        public HttpResponseMessage CreateHttpResponseMessage(string dataForReturn)
        {
            if (dataForReturn == "Error")
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            if (dataForReturn == "No Data")
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            //return new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(dataForReturn) };
            HttpResponseMessage m = new HttpResponseMessage(HttpStatusCode.OK);
            m.Headers.Add("result", dataForReturn);
            return m;
        }
    }
}