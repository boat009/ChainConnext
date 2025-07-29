using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Helpers
{
    public class SqlServerDataConnection : IDisposable
    {
        #region ========== Global Variables ==========
        private SqlConnection sqlConnection;
        private SqlCommand sqlCommand;
        private CommandType sqlCommandType = CommandType.Text;
        private SqlTransaction sqlTransaction;
        private List<string> errorCommand;

        private string sqlConnectionString = string.Empty;
        private string sqlCommandString = string.Empty;
        private string commandMessage = "You must execute command.";
        private bool commandSuccess = false;
        private bool transaction = false;
        private int rowsAffected = 0;

        #endregion

        #region ========= Constructor ==========
        /// <summary>
        /// Use sql connection string from web.config configulation.
        /// </summary>
        public SqlServerDataConnection()
        {
            sqlConnectionString = string.Format("Data Source={0};Initial Catalog={1};Persist Security Info={2};User ID={3};Password={4};Application Name={5};"
                , BaseSettup.ServerName
                , BaseSettup.DatabaseName
                , BaseSettup.PersistSecurityInfo
                , BaseSettup.DatabaseUserName
                , BaseSettup.DatabasePassword
                , BaseSettup.ApplicationName);
            sqlConnection = new SqlConnection(sqlConnectionString);
            errorCommand = new List<string>();
        }

        /// <summary>
        /// Use sql connection string from web.config configulation by key name.
        /// </summary>
        public SqlServerDataConnection(string SqlConnectionString)
        {
            sqlConnectionString = SqlConnectionString;
            sqlConnection = new SqlConnection(sqlConnectionString);
            errorCommand = new List<string>();
        }
        #endregion

        #region ========== Property ==========
        /// <summary>
        /// Gets or sets Sql connection.
        /// </summary>
        public virtual string ConnectionString
        {
            get { return sqlConnectionString; }
            set
            {
                sqlConnectionString = value;
                sqlConnection = new SqlConnection(sqlConnectionString);
            }
        }
        public virtual CommandType SqlCommandType
        {
            get { return sqlCommandType; }
            set
            {
                sqlCommandType = value;
            }
        }
        public virtual bool IsTransaction
        {
            get { return transaction; }
        }
        /// <summary>
        /// Gets or sets Sql command.
        /// </summary>
        public virtual string CommandString
        {
            get { return sqlCommandString; }
            set
            {
                sqlCommandString = ConvertDateCommand(value);
                sqlCommand = new SqlCommand(sqlCommandString, sqlConnection);
                sqlCommand.CommandType = sqlCommandType;
                sqlCommand.CommandTimeout = 590000000;
                commandMessage = "You must execute command.";
                commandSuccess = false;
                rowsAffected = 0;

                if (transaction)
                    sqlCommand.Transaction = sqlTransaction;
            }
        }

        /// <summary>
        /// Check for Sql command.
        /// </summary>
        public virtual bool IsSuccess
        {
            get { return commandSuccess; }
        }

        /// <summary>
        /// Gets message from Sql command.
        /// </summary>
        public virtual string Message
        {
            get { return commandMessage; }
        }

        /// <summary>
        /// Gets Number of rows affected.
        /// </summary>
        public virtual int RowsAffected
        {
            get { return rowsAffected; }
        }
        #endregion

        #region ========== Method ==========
        /// <summary>
        /// Add the parameter value to the sql command.
        /// </summary>
        /// <param name="ParameterName">The name of Parameter.</param>
        /// <param name="ParameterValue">The value to be added.</param>
        public virtual void AddParameter(string ParameterName, object ParameterValue)
        {
            sqlCommand.Parameters.AddWithValue(ParameterName, CheckDateValue(ParameterValue));
        }
        public virtual void AddParameter(string ParameterName, object ParameterValue, ParameterDirection ParameterDirect)
        {
            sqlCommand.Parameters.AddWithValue(ParameterName, CheckDateValue(ParameterValue)).Direction = ParameterDirect;
        }
        public virtual void AddParameterNoCheck(string ParameterName, object ParameterValue)
        {
            sqlCommand.Parameters.AddWithValue(ParameterName, ParameterValue);
        }
        private object CheckDateValue(object ParameterValues)
        {
            if (ParameterValues is DateTime)
            {
                DateTime DateCheck = Convert.ToDateTime(ParameterValues);
                int YearCheck = Convert.ToInt32(string.Format(new CultureInfo("en-US", true), "{0:yyyy}", DateCheck));
                while (YearCheck < 1753)
                {
                    YearCheck += 543;
                }
                int iYear = GetiYear();
                if (YearCheck > (iYear + 300))
                {
                    YearCheck -= 543;
                }
                Calendar cld = Thread.CurrentThread.CurrentCulture.Calendar;
                DateTime dtNew = new DateTime(YearCheck, DateCheck.Month, DateCheck.Day, DateCheck.Hour, DateCheck.Minute, DateCheck.Second);
                ParameterValues = dtNew;
            }
            else if (ParameterValues is string)
            {
                ParameterValues = ParameterValues.ToString().Trim().Replace("'", string.Empty);
            }

            return ParameterValues;
        }
        private int GetiYear()
        {
            if (BaseSettup.dbYear == 0)
            {
                using (SqlServerDataConnection jtCon = new SqlServerDataConnection())
                {
                    jtCon.CommandString = "SELECT YEAR(GETDATE());";
                    BaseSettup.dbYear = Convert.ToInt32(jtCon.ExecuteScalar());
                }
            }
            return BaseSettup.dbYear;
        }
        private void SetErrorSqlServer(SqlException ex)
        {
            if (ex.Class == 20 || ex.Number == 10060)
            {
                commandMessage = "ไม่สามารถติดต่อฐานข้อมูลได้";
                BaseSettup.IsNotConnect = true;
            }
            else
            {
                commandMessage = ErrorMessage(ex.Message);
                BaseSettup.SaveErrorData(BaseSettup.UserID
                    , "Sql Error : " + sqlCommand.CommandText
                    , ex.Message
                    , BaseSettup.ProVersion);
            }
            Console.WriteLine(commandMessage);
            BaseSettup.SqlMassager = commandMessage;
            commandSuccess = false;
            AddErrorCommand(sqlCommandString, ex.Message);
        }
        private void OpenConnection()
        {
            try
            {
                if (sqlConnection.State != ConnectionState.Open)
                    sqlConnection.Open();
                BaseSettup.IsNotConnect = false;
            }
            catch (SqlException ex)
            {
                SetErrorSqlServer(ex);
                BaseSettup.IsNotConnect = true;
            }
            catch (Exception ex)
            {
                commandMessage = ErrorMessage(ex.Message);
                Console.WriteLine(commandMessage);
                BaseSettup.SqlMassager = commandMessage;
                commandSuccess = false;
                BaseSettup.IsNotConnect = true;
            }
        }
        private void CheckSnapshotState()
        {
            SqlServerDataConnection sqlCon = new SqlServerDataConnection();
            sqlCon.CommandString = "Select snapshot_isolation_state_desc from sys.databases where name=@name";
            sqlCon.AddParameter("@name", BaseSettup.DatabaseName);
            object obj = sqlCon.ExecuteScalar();
            if (obj != null)
            {
                if (obj != DBNull.Value)
                {
                    string state = Convert.ToString(obj);
                    if (state == "OFF")
                    {
                        sqlCon = new SqlServerDataConnection();
                        sqlCon.CommandString = @"ALTER DATABASE " + BaseSettup.DatabaseName + " SET ALLOW_SNAPSHOT_ISOLATION ON";
                        sqlCon.ExecuteNonQuery();
                    }
                }
            }
        }
        private void SetlockTimeout()
        {
            using (SqlServerDataConnection xCon = new SqlServerDataConnection())
            {
                xCon.sqlCommandType = CommandType.StoredProcedure;
                xCon.CommandString = "Set_LockTimeOut";
                xCon.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Start Sql Transaction.
        /// </summary>
        public virtual void TransactionStart()
        {
            //SetlockTimeout();
            CheckSnapshotState();
            transaction = true;
            errorCommand = new List<string>();

            OpenConnection();
            //Read uncommitted SQL Server 2014 Unsupported
            //Read committed SQL Server 2014 Supported***
            //Repeatable read SQL Server 2014 Unsupported
            //Serializable SQL Server 2014 Unsupported
            //Read committed snapshot SQL Server 2014 Supported***
            //Snapshot snapshot SQL Server 2014 Supported***
            sqlTransaction = sqlConnection.BeginTransaction(IsolationLevel.ReadUncommitted);
        }
        public virtual void TransactionStart(IsolationLevel IsoLvl)
        {
            //SetlockTimeout();
            CheckSnapshotState();
            transaction = true;
            errorCommand = new List<string>();

            OpenConnection();
            //Read uncommitted SQL Server 2014 Unsupported
            //Read committed SQL Server 2014 Supported***
            //Repeatable read SQL Server 2014 Unsupported
            //Serializable SQL Server 2014 Unsupported
            //Read committed snapshot SQL Server 2014 Supported***
            //Snapshot snapshot SQL Server 2014 Supported***
            sqlTransaction = sqlConnection.BeginTransaction(IsoLvl);
        }

        /// <summary>
        /// Execute Sql Transaction.
        /// </summary>
        /// <returns>Result of transaction.</returns>
        public virtual bool ExecuteTransaction()
        {
            transaction = false;

            if (errorCommand.Count == 0)
            {
                sqlTransaction.Commit();

                commandMessage = "All command is successfully. Transaction Commited.";
                commandSuccess = true;
            }
            else
            {
                sqlTransaction.Rollback();

                string ErrorText = "Some command has error. Transaction RollBack.";

                foreach (string aErrorSqlCommand in errorCommand)
                {
                    ErrorText += aErrorSqlCommand + "\r\n";
                }

                commandMessage = ErrorText;
                commandSuccess = false;
            }

            errorCommand.Clear();

            if (sqlConnection.State == ConnectionState.Open)
                sqlConnection.Close();

            sqlTransaction.Dispose();
            if (sqlCommand != null)
            {
                sqlCommand.Dispose();
            }
            sqlConnection.Dispose();
            return commandSuccess;
        }
        public async Task<bool> ExecuteTransactionAsync()
        {
            transaction = false;

            if (errorCommand.Count == 0)
            {
                await sqlTransaction.CommitAsync();

                commandMessage = "All command is successfully. Transaction Commited.";
                commandSuccess = true;
            }
            else
            {
                sqlTransaction.Rollback();

                string ErrorText = "Some command has error. Transaction RollBack.";

                foreach (string aErrorSqlCommand in errorCommand)
                {
                    ErrorText += aErrorSqlCommand + "\r\n";
                }

                commandMessage = ErrorText;
                commandSuccess = false;
            }

            errorCommand.Clear();

            if (sqlConnection.State == ConnectionState.Open)
                sqlConnection.Close();

            sqlTransaction.Dispose();
            if (sqlCommand != null)
            {
                sqlCommand.Dispose();
            }
            sqlConnection.Dispose();
            return commandSuccess;
        }

        public virtual void CloseConnect()
        {
            if (!transaction)
            {
                sqlConnection.Close();
                sqlCommand.Dispose();
                sqlConnection.Dispose();
            }
            else
            {
                if (sqlConnection.State == ConnectionState.Open)
                    sqlConnection.Close();

                sqlTransaction.Dispose();
                sqlCommand.Dispose();
                sqlConnection.Dispose();
            }
        }
        public virtual DataSet ExecuteQueries()
        {
            DataSet ds = new DataSet();

            try
            {
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(ds);
                sqlDataAdapter.Dispose();

                commandMessage = "Command is successfully.";
                commandSuccess = true;
                BaseSettup.SqlMassager = commandMessage;
            }
            catch (SqlException ex)
            {
                SetErrorSqlServer(ex);
            }
            catch (Exception ex)
            {
                commandMessage = ErrorMessage(ex.Message);
                commandSuccess = false;
                AddErrorCommand(sqlCommandString, ex.Message);
                Console.WriteLine(commandMessage);
                BaseSettup.SqlMassager = commandMessage;
            }
            finally
            {
                if (!transaction)
                {
                    sqlConnection.Close();
                    sqlCommand.Dispose();
                    sqlConnection.Dispose();
                }
            }

            rowsAffected = ds.Tables.Count;

            return ds;
        }
        /// <summary>
        /// Execute Query Sql command.
        /// </summary>
        /// <returns>Query data in DataTable.</returns>

        public virtual DataTable ExecuteQuery()
        {
            DataTable dataTable = new DataTable("Data");

            try
            {
                if (transaction)
                {
                    using (SqlDataReader SqlRd = sqlCommand.ExecuteReader())
                    {
                        dataTable.Load(SqlRd);
                    }
                }
                else
                {
                    OpenConnection();

                    using (SqlDataReader SqlRd = sqlCommand.ExecuteReader())
                    {
                        dataTable.Load(SqlRd);
                    }
                }

                commandMessage = "Command is successfully.";
                commandSuccess = true;
                BaseSettup.SqlMassager = commandMessage;
            }
            catch (SqlException ex)
            {
                SetErrorSqlServer(ex);
            }
            catch (Exception ex)
            {
                commandMessage = ErrorMessage(ex.Message);
                Console.WriteLine(commandMessage);
                commandSuccess = false;
                AddErrorCommand(sqlCommandString, ex.Message);
                BaseSettup.SqlMassager = commandMessage;
            }
            finally
            {
                if (!transaction)
                {
                    sqlConnection.Close();
                    sqlCommand.Dispose();
                    sqlConnection.Dispose();
                }
            }

            rowsAffected = dataTable.Rows.Count;

            return dataTable;
        }
        public async Task<DataTable> ExecuteQueryAsync()
        {
            DataTable dataTable = new DataTable("Data");

            try
            {
                if (transaction)
                {
                    using (SqlDataReader SqlRd = await sqlCommand.ExecuteReaderAsync())
                    {
                        dataTable.Load(SqlRd);
                    }
                }
                else
                {
                    OpenConnection();

                    using (SqlDataReader SqlRd = await sqlCommand.ExecuteReaderAsync())
                    {
                        dataTable.Load(SqlRd);
                    }
                }

                commandMessage = "Command is successfully.";
                commandSuccess = true;
                BaseSettup.SqlMassager = commandMessage;
            }
            catch (SqlException ex)
            {
                SetErrorSqlServer(ex);
            }
            catch (Exception ex)
            {
                commandMessage = ErrorMessage(ex.Message);
                Console.WriteLine(commandMessage);
                commandSuccess = false;
                AddErrorCommand(sqlCommandString, ex.Message);
                BaseSettup.SqlMassager = commandMessage;
            }
            finally
            {
                if (!transaction)
                {
                    sqlConnection.Close();
                    sqlCommand.Dispose();
                    sqlConnection.Dispose();
                }
            }

            rowsAffected = dataTable.Rows.Count;

            return dataTable;
        }
        public async Task<List<T>> ExecuteQueryListAsync<T>()
        {
            DataTable dataTable = new DataTable("Data");
            List<T> list = new List<T>();
            T obj = default(T);
            string error_at = "";
            try
            {
                string columnname = "";
                string? value = "";
                List<string> columnsNames = new List<string>();

                if (transaction)
                {
                    using (SqlDataReader SqlRd = await sqlCommand.ExecuteReaderAsync())
                    {
                        columnsNames = Enumerable.Range(0, SqlRd.FieldCount).Select(SqlRd.GetName).ToList();
                        while (SqlRd.Read())
                        {
                            obj = Activator.CreateInstance<T>();
                            foreach (PropertyInfo prop in obj.GetType().GetProperties())
                            {
                                error_at = prop.Name;


                                try
                                {
                                    columnname = columnsNames.Find(name => name.ToLower() == prop.Name.ToLower());
                                    if (!string.IsNullOrEmpty(columnname))
                                    {
                                        if (SqlRd[prop.Name] != null)
                                        {
                                            value = SqlRd[prop.Name]?.ToString()?.Trim();
                                        }
                                        //if (value != null)
                                        ////if (!object.Equals(SqlRd[prop.Name], DBNull.Value))
                                        //{
                                        //    prop.SetValue(obj, SqlRd[prop.Name], null);
                                        //}
                                        if (value != null)
                                        {
                                            if (!string.IsNullOrEmpty(value))
                                            {
                                                if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
                                                {
                                                    if (value == "System.Byte[]")
                                                    {
                                                        prop.SetValue(obj, Convert.ChangeType(SqlRd[prop.Name], Type.GetType(Nullable.GetUnderlyingType(prop.PropertyType).ToString())), null);
                                                    }
                                                    else
                                                    {
                                                        prop.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(prop.PropertyType).ToString())), null);
                                                    }
                                                }
                                                else
                                                {
                                                    if (value == "System.Byte[]")
                                                    {
                                                        prop.SetValue(obj, Convert.ChangeType(SqlRd[prop.Name], Type.GetType(prop.PropertyType.ToString())), null);
                                                    }
                                                    else
                                                    {
                                                        prop.SetValue(obj, Convert.ChangeType(value, Type.GetType(prop.PropertyType.ToString())), null);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                    throw;
                                }
                            }
                            list.Add(obj);
                        }
                    }
                }
                else
                {
                    OpenConnection();

                    using (SqlDataReader SqlRd = await sqlCommand.ExecuteReaderAsync())
                    {
                        columnsNames = Enumerable.Range(0, SqlRd.FieldCount).Select(SqlRd.GetName).ToList();
                        while (SqlRd.Read())
                        {
                            obj = Activator.CreateInstance<T>();
                            foreach (PropertyInfo prop in obj.GetType().GetProperties())
                            {
                                error_at = prop.Name;
                                

                                try
                                {
                                    columnname = columnsNames.Find(name => name.ToLower() == prop.Name.ToLower());
                                    if (!string.IsNullOrEmpty(columnname))
                                    {
                                        if (SqlRd[prop.Name] != null)
                                        {
                                            value = SqlRd[prop.Name]?.ToString()?.Trim();
                                        }
                                        //if (value != null)
                                        ////if (!object.Equals(SqlRd[prop.Name], DBNull.Value))
                                        //{
                                        //    prop.SetValue(obj, SqlRd[prop.Name], null);
                                        //}
                                        if (value != null)
                                        {
                                            if (!string.IsNullOrEmpty(value))
                                            {
                                                if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
                                                {
                                                    if (value == "System.Byte[]")
                                                    {
                                                        prop.SetValue(obj, Convert.ChangeType(SqlRd[prop.Name], Type.GetType(Nullable.GetUnderlyingType(prop.PropertyType).ToString())), null);
                                                    }
                                                    else
                                                    {
                                                        prop.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(prop.PropertyType).ToString())), null);
                                                    }
                                                }
                                                else
                                                {
                                                    if (value == "System.Byte[]")
                                                    {
                                                        prop.SetValue(obj, Convert.ChangeType(SqlRd[prop.Name], Type.GetType(prop.PropertyType.ToString())), null);
                                                    }
                                                    else
                                                    {
                                                        prop.SetValue(obj, Convert.ChangeType(value, Type.GetType(prop.PropertyType.ToString())), null);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                    throw;
                                }
                            }
                            list.Add(obj);
                        }
                    }
                }

                commandMessage = "Command is successfully.";
                commandSuccess = true;
                BaseSettup.SqlMassager = commandMessage;
            }
            catch (SqlException ex)
            {
                SetErrorSqlServer(ex);
            }
            catch (Exception ex)
            {
                commandMessage = ErrorMessage(ex.Message + " // Error At : " + error_at);
                Console.WriteLine(commandMessage);
                commandSuccess = false;
                AddErrorCommand(sqlCommandString, ex.Message);
                BaseSettup.SqlMassager = commandMessage;
            }
            finally
            {
                if (!transaction)
                {
                    sqlConnection.Close();
                    sqlCommand.Dispose();
                    sqlConnection.Dispose();
                }
            }

            rowsAffected = list.Count;
            if (rowsAffected > 0)
            {
                return list;
            }
            else
            {
                return new List<T>();
            }
        }
        public virtual List<object> ExecuteQuery(string OutputPara)
        {
            DataTable dataTable = new DataTable("Data");
            object outp = new object();
            List<object> listob = new List<object>();
            try
            {
                if (transaction)
                {
                    using (SqlDataReader SqlRd = sqlCommand.ExecuteReader())
                    {
                        dataTable.Load(SqlRd);
                    }
                }
                else
                {
                    OpenConnection();

                    using (SqlDataReader SqlRd = sqlCommand.ExecuteReader())
                    {
                        dataTable.Load(SqlRd);
                    }
                }

                commandMessage = "Command is successfully.";
                commandSuccess = true;
                if (commandSuccess)
                {
                    outp = sqlCommand.Parameters[OutputPara].Value;

                    listob.Add(outp);
                    listob.Add(dataTable);
                }
                BaseSettup.SqlMassager = commandMessage;
            }
            catch (SqlException ex)
            {
                SetErrorSqlServer(ex);
            }
            catch (Exception ex)
            {
                commandMessage = ErrorMessage(ex.Message);
                commandSuccess = false;
                AddErrorCommand(sqlCommandString, ex.Message);
                Console.WriteLine(commandMessage);
            }
            finally
            {
                if (!transaction)
                {
                    sqlConnection.Close();
                    sqlCommand.Dispose();
                    sqlConnection.Dispose();
                }
            }

            rowsAffected = dataTable.Rows.Count;
            return listob;
        }
        /// <summary>
        /// Execute Scalar Sql command.
        /// </summary>
        /// <returns>Object of value.</returns>
        public virtual object ExecuteScalar()
        {
            object Result = 0;

            try
            {
                if (transaction)
                {
                    Result = sqlCommand.ExecuteScalar();
                }
                else
                {
                    OpenConnection();

                    Result = sqlCommand.ExecuteScalar();
                }

                commandMessage = "Command is successfully.";
                commandSuccess = true;
                BaseSettup.SqlMassager = commandMessage;
            }
            catch (SqlException ex)
            {
                SetErrorSqlServer(ex);
            }
            catch (Exception ex)
            {
                commandMessage = ErrorMessage(ex.Message);
                commandSuccess = false;
                AddErrorCommand(sqlCommandString, ex.Message);
                Console.WriteLine(commandMessage);
                BaseSettup.SqlMassager = commandMessage;
            }
            finally
            {
                if (!transaction)
                {
                    sqlConnection.Close();
                    sqlCommand.Dispose();
                    sqlConnection.Dispose();
                }
            }

            return Result;
        }

        /// <summary>
        /// Execute Non Query Sql command.
        /// </summary>
        /// <returns>Result of execute command.</returns>
        public virtual bool ExecuteErrorSaveNonQuery()
        {
            rowsAffected = 0;

            try
            {
                if (transaction)
                {
                    rowsAffected = sqlCommand.ExecuteNonQuery();
                }
                else
                {
                    OpenConnection();

                    rowsAffected = sqlCommand.ExecuteNonQuery();
                }

                commandMessage = "Command is successfully.";
                commandSuccess = true;
                BaseSettup.SqlMassager = commandMessage;
            }
            catch (Exception ex)
            {
                commandMessage = ErrorMessage(ex.Message);
                commandSuccess = false;
                AddErrorCommand(sqlCommandString, ex.Message);
                Console.WriteLine(commandMessage);
                BaseSettup.SqlMassager = commandMessage;
            }
            finally
            {
                if (!transaction)
                {
                    sqlConnection.Close();
                    sqlCommand.Dispose();
                    sqlConnection.Dispose();
                }
            }

            return commandSuccess;
        }
        public virtual bool ExecuteNonQuery()
        {
            rowsAffected = 0;

            try
            {
                if (transaction)
                {
                    rowsAffected = sqlCommand.ExecuteNonQuery();
                }
                else
                {
                    OpenConnection();

                    rowsAffected = sqlCommand.ExecuteNonQuery();
                }

                commandMessage = "Command is successfully.";
                commandSuccess = true;
                BaseSettup.SqlMassager = commandMessage;
            }
            catch (SqlException ex)
            {
                SetErrorSqlServer(ex);
            }
            catch (Exception ex)
            {
                commandMessage = ErrorMessage(ex.Message);
                commandSuccess = false;
                AddErrorCommand(sqlCommandString, ex.Message);
                Console.WriteLine(commandMessage);
                BaseSettup.SqlMassager = commandMessage;
            }
            finally
            {
                if (!transaction)
                {
                    sqlConnection.Close();
                    sqlCommand.Dispose();
                    sqlConnection.Dispose();
                }
            }

            return commandSuccess;
        }
        public async Task<bool> ExecuteNonQueryAsync()
        {
            rowsAffected = 0;

            try
            {
                if (transaction)
                {
                    rowsAffected = await sqlCommand.ExecuteNonQueryAsync();
                }
                else
                {
                    OpenConnection();

                    rowsAffected = await sqlCommand.ExecuteNonQueryAsync();
                }

                commandMessage = "Command is successfully.";
                commandSuccess = true;
                BaseSettup.SqlMassager = commandMessage;
            }
            catch (SqlException ex)
            {
                SetErrorSqlServer(ex);
            }
            catch (Exception ex)
            {
                commandMessage = ErrorMessage(ex.Message);
                commandSuccess = false;
                AddErrorCommand(sqlCommandString, ex.Message);
                Console.WriteLine(commandMessage);
                BaseSettup.SqlMassager = commandMessage;
            }
            finally
            {
                if (!transaction)
                {
                    sqlConnection.Close();
                    sqlCommand.Dispose();
                    sqlConnection.Dispose();
                }
            }

            return commandSuccess;
        }
        public virtual int ExecuteNonQuery(string strReturn)
        {
            rowsAffected = 0;
            int ReturnValues = 0;
            try
            {
                SqlParameter retval = sqlCommand.Parameters.Add(strReturn, SqlDbType.VarChar);
                retval.Direction = ParameterDirection.ReturnValue;

                if (transaction)
                {
                    rowsAffected = sqlCommand.ExecuteNonQuery();
                    ReturnValues = (int)sqlCommand.Parameters[strReturn].Value;
                }
                else
                {
                    OpenConnection();

                    rowsAffected = sqlCommand.ExecuteNonQuery();
                    ReturnValues = (int)sqlCommand.Parameters[strReturn].Value;
                }

                commandMessage = "Command is successfully.";
                commandSuccess = true;
                BaseSettup.SqlMassager = commandMessage;
            }
            catch (SqlException ex)
            {
                SetErrorSqlServer(ex);
            }
            catch (Exception ex)
            {
                commandMessage = ErrorMessage(ex.Message);
                commandSuccess = false;
                AddErrorCommand(sqlCommandString, ex.Message);
                BaseSettup.SqlMassager = commandMessage;
            }
            finally
            {
                if (!transaction)
                {
                    sqlConnection.Close();
                    sqlCommand.Dispose();
                    sqlConnection.Dispose();
                }
            }

            return ReturnValues;
        }
        public virtual List<object> ExecuteNonQuery(string[] strOutput)
        {
            List<object> ObjOutput = new List<object>();
            try
            {
                for (int i = 0; i < strOutput.Length; i++)
                {
                    if (!sqlCommand.Parameters.Contains(strOutput[i]))
                    {
                        SqlParameter retval = sqlCommand.Parameters.Add(strOutput[i], SqlDbType.VarChar, -1);
                        retval.Direction = ParameterDirection.Output;
                    }
                }
                if (transaction)
                {
                    rowsAffected = sqlCommand.ExecuteNonQuery();
                }
                else
                {
                    OpenConnection();

                    rowsAffected = sqlCommand.ExecuteNonQuery();
                }

                commandMessage = "Command is successfully.";
                commandSuccess = true;
                if (commandSuccess)
                {
                    for (int i = 0; i < strOutput.Length; i++)
                    {
                        ObjOutput.Add(sqlCommand.Parameters[strOutput[i]].Value);
                    }
                }
                BaseSettup.SqlMassager = commandMessage;
            }
            catch (SqlException ex)
            {
                SetErrorSqlServer(ex);
            }
            catch (Exception ex)
            {
                commandMessage = ErrorMessage(ex.Message);
                commandSuccess = false;
                AddErrorCommand(sqlCommandString, ex.Message);
                Console.WriteLine(commandMessage);
                BaseSettup.SqlMassager = commandMessage;
            }
            finally
            {
                if (!transaction)
                {
                    sqlConnection.Close();
                    sqlCommand.Dispose();
                    sqlConnection.Dispose();
                }
            }

            return ObjOutput;
        }
        public virtual List<object> ExecuteNonQuery(string strReturn, string[] strOutput)
        {
            List<object> ObjOutput = new List<object>();
            try
            {
                SqlParameter retval = sqlCommand.Parameters.Add(strReturn, SqlDbType.VarChar);
                retval.Direction = ParameterDirection.ReturnValue;

                for (int i = 0; i < strOutput.Length; i++)
                {
                    if (!sqlCommand.Parameters.Contains(strOutput[i]))
                    {
                        retval = sqlCommand.Parameters.Add(strReturn, SqlDbType.VarChar, -1);
                        retval.Direction = ParameterDirection.Output;
                    }
                }
                if (transaction)
                {
                    rowsAffected = sqlCommand.ExecuteNonQuery();
                }
                else
                {
                    OpenConnection();

                    rowsAffected = sqlCommand.ExecuteNonQuery();
                }

                commandMessage = "Command is successfully.";
                commandSuccess = true;
                if (commandSuccess)
                {
                    ObjOutput.Add(sqlCommand.Parameters[strReturn].Value);
                    for (int i = 0; i < strOutput.Length; i++)
                    {
                        ObjOutput.Add(sqlCommand.Parameters[strOutput[i]].Value);
                    }
                }
            }
            catch (SqlException ex)
            {
                SetErrorSqlServer(ex);
            }
            catch (Exception ex)
            {
                commandMessage = ErrorMessage(ex.Message);
                commandSuccess = false;
                AddErrorCommand(sqlCommandString, ex.Message);
                Console.WriteLine(commandMessage);
                BaseSettup.SqlMassager = commandMessage;
            }
            finally
            {
                if (!transaction)
                {
                    sqlConnection.Close();
                    sqlCommand.Dispose();
                    sqlConnection.Dispose();
                }
            }

            return ObjOutput;
        }

        /// <summary>
        /// Build error message.
        /// </summary>
        /// <param name="Message">Message string.</param>
        /// <returns>Error message string.</returns>
        protected virtual string ErrorMessage(string MessageString)
        {
            return "Command error " + MessageString;
        }

        /// <summary>
        /// Add error sql command to string collections.
        /// </summary>
        /// <param name="commandString">The sql command.</param>
        /// <param name="errorMessage">The error message.</param>
        public virtual void AddErrorCommand(string commandString, string errorMessage)
        {
            errorCommand.Add(commandString + "[Error message: " + errorMessage + "]");
        }

        /// <summary>
        /// Convert native command to sql command.
        /// </summary>
        /// <param name="commandString">The native sql command.</param>
        /// <returns>The standard sql command.</returns>
        protected virtual string ConvertDateCommand(string commandString)
        {
            string SmallDateTimePattern = "[sS][mM][aA][lL][lL][dD][aA][tT][eE][tT][iI][mM][eE]\\([@][0-9a-zA-Z\\s]{1,}\\)";
            Regex SmallDateTimeRgx = new Regex(SmallDateTimePattern);

            foreach (Match SmallDateTimeMatchCase in SmallDateTimeRgx.Matches(commandString))
            {
                string MatchCasePattern = "^[sS][mM][aA][lL][lL][dD][aA][tT][eE][tT][iI][mM][eE]";
                Regex MatchCaseRgx = new Regex(MatchCasePattern);
                Match RemoveMatch = MatchCaseRgx.Match(SmallDateTimeMatchCase.Value);
                string TempMatchCase = SmallDateTimeMatchCase.Value.Replace(RemoveMatch.Value, "");

                commandString = commandString.Replace(SmallDateTimeMatchCase.Value, TempMatchCase.Replace("(", "Convert(SmallDateTime, ").Replace(")", ", 103)"));
            }

            string DateTimePattern = "[dD][aA][tT][eE][tT][iI][mM][eE]\\([@][0-9a-zA-Z\\s]{1,}\\)";
            Regex DateTimeRgx = new Regex(DateTimePattern);

            foreach (Match DateTimeMatchCase in DateTimeRgx.Matches(commandString))
            {
                string MatchCasePattern = "^[dD][aA][tT][eE][tT][iI][mM][eE]";
                Regex MatchCaseRgx = new Regex(MatchCasePattern);
                Match RemoveMatch = MatchCaseRgx.Match(DateTimeMatchCase.Value);
                string TempMatchCase = DateTimeMatchCase.Value.Replace(RemoveMatch.Value, "");

                commandString = commandString.Replace(DateTimeMatchCase.Value, TempMatchCase.Replace("(", "Convert(DateTime, ").Replace(")", ", 103)"));
            }

            return commandString;
        }
        #endregion

        #region IDisposable Members
        private IntPtr handle;
        private Component component = new Component();
        private bool disposed = false;
        public SqlServerDataConnection(IntPtr handle)
        {
            this.handle = handle;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //sqlConnection.Close();
                    //sqlConnection.Dispose();

                    component.Dispose();
                }
                CloseHandle(handle);
                handle = IntPtr.Zero;
                disposed = true;

            }
        }
        [System.Runtime.InteropServices.DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);
        ~SqlServerDataConnection()
        {
            Dispose(false);
        }

        #endregion
    }

    #region Class SqlConvert
    /// <summary>
    /// Summary description for SqlConvert
    /// </summary>
    public class SqlConvert
    {
        /// <summary>
        /// Convert to byte[].
        /// </summary>
        public static byte[] ToVarBinary(Stream BinaryStream, int StreamLength)
        {
            BinaryReader BinaryRead = new BinaryReader(BinaryStream);
            byte[] binaryData = BinaryRead.ReadBytes(StreamLength);

            return binaryData;
        }

        /// <summary>
        /// Convert to DataTime DataType with d/M/yyyy format.
        /// </summary>
        public static DateTime ToDateTime(string DateString)
        {
            //ควรกำหนด culture ใน web.config เป็น th
            //
            //ตัวอย่างใน web.config
            //<globalization requestEncoding="utf-8" responseEncoding="utf-8" culture="th-TH" uiCulture="th-TH"/>
            string DatePattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            return DateTime.ParseExact(DateString, DatePattern, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert to DataTime DataType with user define format.
        /// </summary>
        public static DateTime ToDateTime(string DateString, string DateFormat)
        {
            //ควรกำหนด culture ใน web.config เป็น th
            //
            //ตัวอย่างใน web.config
            //<globalization requestEncoding="utf-8" responseEncoding="utf-8" culture="th-TH" uiCulture="th-TH"/>

            return DateTime.ParseExact(DateString, DateFormat, CultureInfo.InvariantCulture);
        }
    }
    #endregion
}
