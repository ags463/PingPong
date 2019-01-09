//---------------------------------------------------------------------------------------------------
// General Helper Methods for working with System.Data.Common data connections.
// All shared functions are thread safe.
//---------------------------------------------------------------------------------------------------
// Created: 07 Mar 2018, Alan G. Stewart
// Changed: o8 Jan 2019, Alan G. Stewart
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Data.Common;
using System.Reflection;
using System.Security.Permissions;

namespace PingPong.DBLib
{
    public class Utility
    {
        /// <summary>
        /// Create a database connnection.
        /// </summary>
        /// <param name="ConnectionName">The name of the connection string in the config file.</param>
        /// <returns>The new connection.</returns>
        public static DbConnection CreateConnection(string ConnectionName)
        {
            ConnectionStringSettings oSettings = ConfigurationManager.ConnectionStrings[ConnectionName];
            DbProviderFactory oFactory = DbProviderFactories.GetFactory(oSettings.ProviderName);
            DbConnection oCon = oFactory.CreateConnection();

            oCon.ConnectionString = oSettings.ConnectionString;
            return oCon;
        }
        /// <summary>
        /// Create a data adapter.
        /// </summary>
        /// <param name="ConnectionName">The name of the connection string in the config file.</param>
        /// <param name="SPName">The name of the stored procedure to use for the SelectCommand.</param>
        /// <returns>The new data adapter.</returns>
        public static DbDataAdapter CreateDataAdapter(string ConnectionName, string SPName)
        {
            ConnectionStringSettings oSettings = ConfigurationManager.ConnectionStrings[ConnectionName];
            DbProviderFactory oFactory = DbProviderFactories.GetFactory(oSettings.ProviderName);
            DbConnection oCon = oFactory.CreateConnection();

            oCon.ConnectionString = oSettings.ConnectionString;

            DbCommand oCmd = oFactory.CreateCommand();

            oCmd.CommandText = SPName;
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.Connection = oCon;

            DbDataAdapter oDa = oFactory.CreateDataAdapter();

            oDa.SelectCommand = oCmd;

            return oDa;
        }
        /// <summary>
        /// Create a data adapter.
        /// </summary>
        /// <param name="ConnectionName">The name of the connection string in the config file.</param>
        /// <param name="SPName">The name of the stored procedure to use for the SelectCommand.</param>
        /// <returns>The new data adapter.</returns>
        public static DbDataAdapter CreateDataAdapter(string ConnectionName, DbCommand oCmd)
        {
            ConnectionStringSettings oSettings = ConfigurationManager.ConnectionStrings[ConnectionName];
            DbProviderFactory oFactory = DbProviderFactories.GetFactory(oSettings.ProviderName);
            DbConnection oCon = oFactory.CreateConnection();

            oCon.ConnectionString = oSettings.ConnectionString;

            oCmd.Connection = oCon;

            DbDataAdapter oDa = oFactory.CreateDataAdapter();

            oDa.SelectCommand = oCmd;

            return oDa;
        }
        /// <summary>
        /// Create a command on a new connection.
        /// </summary>
        /// <param name="ConnectionName">The name of the connection string in the config file.</param>
        /// <param name="SPName">The name of the stored procedure to use for the Command.</param>
        /// <returns>The new command.</returns>
        public static DbCommand CreateSPCommand(string ConnectionName, string SPName)
        {
            return CreateSPCommand(CreateConnection(ConnectionName), SPName);
        }
        /// <summary>
        /// Create a command on an existing connect.
        /// </summary>
        /// <param name="Connection">The connection to use.</param>
        /// <param name="SPName">The name of the stored procedure to use for the command.</param>
        /// <returns>The new command.</returns>
        public static DbCommand CreateSPCommand(DbConnection Connection, string SPName)
        {
            DbCommand oCmd = Connection.CreateCommand();

            oCmd.CommandText = SPName;
            oCmd.CommandType = CommandType.StoredProcedure;
            //** Set the command timeout for all commands to 5 minutes.
            oCmd.CommandTimeout = 300;
            return oCmd;
        }
        /// <summary>
        /// Add a parameter to a command.
        /// </summary>
        /// <param name="Command">The command to add the parameter to.</param>
        /// <param name="ParameterName">The name of the parameter to add.</param>
        /// <param name="Type">The type of the parameter to add.</param>
        /// <param name="Value">The value of the parameter to add.</param>
        public static void AddParameter(ref DbCommand Command, string ParameterName, DbType Type, object Value)
        {
            AddParameter(ref Command, ParameterName, Type, Value, 0);
        }
        /// <summary>
        /// Add a string parameter to a command.
        /// </summary>
        /// <param name="Command">The command to add the parameter to.</param>
        /// <param name="ParameterName">The name of the parameter to add.</param>
        /// <param name="Type">The type of the parameter to add.</param>
        /// <param name="Value">The value of the parameter to add.</param>
        /// <param name="Length">The maximum length of the string parameter to add.</param>
        public static void AddParameter(ref DbCommand Command, string ParameterName, DbType Type, object Value, int Length)
        {
            DbParameter oPrm = Command.CreateParameter();

            oPrm.ParameterName = ParameterName;
            oPrm.DbType = Type;
            if (Value == null)
            {
                oPrm.Value = DBNull.Value;
            }
            else
            {
                switch (Type)
                {
                    case DbType.Int32:
                        int i = Convert.ToInt32(Value);
                        if (i == -1)
                        {
                            oPrm.Value = DBNull.Value;
                        }
                        else
                        {
                            oPrm.Value = i;
                        }
                        break;
                    case DbType.String:
                    case DbType.StringFixedLength:
                        string s = Convert.ToString(Value);
                        if (s.Length == 0)
                        {
                            oPrm.Value = DBNull.Value;
                        }
                        else
                        {
                            oPrm.Value = s;
                            oPrm.Size = Length;
                        }
                        break;
                    case DbType.Boolean:
                        oPrm.Value = Convert.ToBoolean(Value);
                        break;
                    case DbType.DateTime:
                        System.DateTime d = Convert.ToDateTime(Value);
                        if (d == Convert.ToDateTime("1/1/1900"))
                        {
                            oPrm.Value = DBNull.Value;
                        }
                        else
                        {
                            oPrm.Value = d;
                        }
                        break;
                    default:
                        oPrm.Value = Value;
                        break;
                }
            }
            Command.Parameters.Add(oPrm);
        }
        /// <summary>
        /// Execute a non-query command, retrieving the integer return value.
        /// </summary>
        /// <param name="Command">The command to execute.</param>
        /// <returns>The integer returned.</returns>
        public static int ExecuteNonQueryForReturnValue(DbCommand Command)
        {
            DbParameter oPrm = Command.CreateParameter();

            oPrm.ParameterName = "RETURN_VALUE";
            oPrm.DbType = DbType.Int32;
            oPrm.Direction = ParameterDirection.ReturnValue;
            var _with1 = Command;
            try
            {
                _with1.Parameters.Add(oPrm);
                if (_with1.Connection.State != ConnectionState.Open)
                    _with1.Connection.Open();
                _with1.ExecuteNonQuery();
            }
            finally
            {
                if (_with1.Connection.State != ConnectionState.Closed)
                    _with1.Connection.Close();
            }

            return Convert.ToInt32(oPrm.Value);
        }
        /// <summary>
        /// Execute a non-query command.
        /// </summary>
        /// <param name="Command">The command to execute.</param>
        /// <returns>True if at least one row was affected.</returns>
        public static bool ExecuteNonQuery(DbCommand Command)
        {
            int iCount = 0;

            var _with2 = Command;
            try
            {
                if (_with2.Connection.State != ConnectionState.Open)
                    _with2.Connection.Open();
                iCount = _with2.ExecuteNonQuery();
            }
            finally
            {
                if (_with2.Connection.State != ConnectionState.Closed)
                    _with2.Connection.Close();
            }
            return (iCount > 0);
        }
        /// <summary>
        /// Execute a query command that returns one boolean value in the first row.
        /// </summary>
        /// <param name="Command">The command to execute.</param>
        /// <returns>The integer returned.</returns>
        public static bool ExecuteScalarBoolean(DbCommand Command)
        {
            bool iValue = false;

            var _with3 = Command;
            try
            {
                if (_with3.Connection.State != ConnectionState.Open)
                    _with3.Connection.Open();
                iValue = Convert.ToBoolean(_with3.ExecuteScalar());
            }
            finally
            {
                if (_with3.Connection.State != ConnectionState.Closed)
                    _with3.Connection.Close();
            }
            return iValue;
        }
        /// <summary>
        /// Execute a query command that returns one integer value in the first row.
        /// </summary>
        /// <param name="Command">The command to execute.</param>
        /// <returns>The integer returned.</returns>
        public static int ExecuteScalarInt(DbCommand Command)
        {
            int iValue = 0;

            var _with4 = Command;
            try
            {
                if (_with4.Connection.State != ConnectionState.Open)
                    _with4.Connection.Open();
                iValue = Convert.ToInt32(_with4.ExecuteScalar());
            }
            finally
            {
                if (_with4.Connection.State != ConnectionState.Closed)
                    _with4.Connection.Close();
            }
            return iValue;
        }
        /// <summary>
        /// Execute a query command that returns one string value in the first row.
        /// </summary>
        /// <param name="Command">The command to execute.</param>
        /// <returns>The string returned.</returns>
        public static string ExecuteScalarString(DbCommand Command)
        {
            string sValue = "";

            var _with5 = Command;
            try
            {
                if (_with5.Connection.State != ConnectionState.Open)
                    _with5.Connection.Open();
                sValue = _with5.ExecuteScalar().ToString();
            }
            finally
            {
                if (_with5.Connection.State != ConnectionState.Closed)
                    _with5.Connection.Close();
            }
            return sValue;
        }
        /// <summary>
        /// Execute a query command that returns one Guid value in the first row.
        /// </summary>
        /// <param name="Command">The command to execute.</param>
        /// <returns>The string returned.</returns>
        public static Guid ExecuteScalarGuid(DbCommand Command)
        {
            Guid sValue = default(Guid);

            try
            {
                if (Command.Connection.State != ConnectionState.Open)
                    Command.Connection.Open();
                sValue = (Guid)Command.ExecuteScalar();
            }
            finally
            {
                if (Command.Connection.State != ConnectionState.Closed)
                    Command.Connection.Close();
            }
            return sValue;
        }
        /// <summary>
        /// Execute a query command that returns one DateTime value in the first row.
        /// </summary>
        /// <param name="Command">The command to execute.</param>
        /// <returns>The string returned.</returns>
        public static DateTime ExecuteScalarDateTime(DbCommand Command)
        {
            DateTime dtValue = default(DateTime);

            try
            {
                if (Command.Connection.State != ConnectionState.Open)
                    Command.Connection.Open();
                dtValue = (DateTime)Command.ExecuteScalar();
            }
            finally
            {
                if (Command.Connection.State != ConnectionState.Closed)
                    Command.Connection.Close();
            }
            return dtValue;
        }
        /// <summary>
        /// Execute a query command and return the resulting datareader.
        /// </summary>
        /// <param name="Command">The command to execute.</param>
        /// <returns>The resulting datareader.</returns>
        public static DbDataReader ExecuteReader(DbCommand Command)
        {
            if (Command.Connection.State != ConnectionState.Open)
                Command.Connection.Open();
            return Command.ExecuteReader(CommandBehavior.CloseConnection);
        }
        /// <summary>
        /// Create and execute a dataadapter and return the resulting dataset.
        /// </summary>
        /// <param name="ConnectionName">The name of the connection string in the config file.</param>
        /// <param name="SPName">The name of the store procedure to use for the command.</param>
        /// <returns>The resulting dataset.</returns>
        public static System.Data.DataSet ExecuteDataSet(string ConnectionName, string SPName)
        {
            DbDataAdapter oDA = CreateDataAdapter(ConnectionName, SPName);
            return ExecuteDataSet(oDA);
        }
        /// <summary>
        /// Execute a data adapter and return the resulting dataset.
        /// </summary>
        /// <param name="DataAdapter">The dataadapter to use to fill the dataset.</param>
        /// <returns>The resulting dataset.</returns>
        public static DataSet ExecuteDataSet(DbDataAdapter DataAdapter)
        {
            System.Data.DataSet oDS = new System.Data.DataSet();
            DataAdapter.Fill(oDS, "Table");
            return oDS;
        }
        /// <summary>
        /// Get a named boolean column from the current row of a datareader, handling Null values.
        /// </summary>
        /// <param name="Reader">The data reader.</param>
        /// <param name="ColumnName">The name of the column to get.</param>
        /// <returns>The actual value or False if NULL or missing.</returns>
        public static bool GetBoolean(DbDataReader Reader, string ColumnName)
        {
            bool bValue = false;

            try
            {
                int iColumn = Reader.GetOrdinal(ColumnName);
                if (!Reader.IsDBNull(iColumn))
                {
                    bValue = Reader.GetBoolean(iColumn);
                }
            }
            catch (Exception)
            {
            }

            return bValue;
        }
        /// <summary>
        /// Get a named datetime column from the current row of a datareader, handling Null values.
        /// </summary>
        /// <param name="Reader">The data reader.</param>
        /// <param name="ColumnName">The name of the column to get.</param>
        /// <returns>The actual value or #1/1/1900# if NULL or missing.</returns>
        public static System.DateTime GetDate(DbDataReader Reader, string ColumnName)
        {
            System.DateTime dValue = Convert.ToDateTime("1/1/1900");

            try
            {
                int iColumn = Reader.GetOrdinal(ColumnName);
                if (!Reader.IsDBNull(iColumn))
                {
                    dValue = Reader.GetDateTime(iColumn);
                }
            }
            catch (Exception)
            {
            }

            return dValue;
        }
        /// <summary>
        /// Get a named decimal column from the current row of a datareader, handling Null values.
        /// </summary>
        /// <param name="Reader">The data reader.</param>
        /// <param name="ColumnName">The name of the column to get.</param>
        /// <returns>The actual value or -1.0 if NULL or missing.</returns>
        public static decimal GetDecimal(DbDataReader Reader, string ColumnName)
        {
            decimal sValue = -1m;

            try
            {
                int iColumn = Reader.GetOrdinal(ColumnName);
                if (!Reader.IsDBNull(iColumn))
                {
                    sValue = Reader.GetDecimal(iColumn);
                }
            }
            catch (Exception)
            {
            }

            return sValue;
        }
        /// -------------------------------------------------------------------------------------------
        /// <summary>
        /// Get a named Guid column from the current row of a datareader, handling Null values.
        /// </summary>
        /// <param name="Reader">The data reader.</param>
        /// <param name="ColumnName">The name of the column to get.</param>
        /// <returns>The actual value or -1 if NULL or missing.</returns>
        public static Guid GetGuid(IDataReader Reader, string ColumnName)
        {
            Guid gValue = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

            try
            {
                int iColumn = Reader.GetOrdinal(ColumnName);
                if (!Reader.IsDBNull(iColumn))
                {
                    gValue = Reader.GetGuid(iColumn);
                }
            }
            catch (Exception)
            {
            }

            return gValue;
        }
        /// -------------------------------------------------------------------------------------------
        /// <summary>
        /// Get a named integer column from the current row of a datareader, handling Null values.
        /// </summary>
        /// <param name="Reader">The data reader.</param>
        /// <param name="ColumnName">The name of the column to get.</param>
        /// <returns>The actual value or -1 if NULL or missing.</returns>
        public static int GetInteger(DbDataReader Reader, string ColumnName)
        {
            int iValue = -1;

            try
            {
                int iColumn = Reader.GetOrdinal(ColumnName);
                if (!Reader.IsDBNull(iColumn))
                {
                    iValue = Reader.GetInt32(iColumn);
                }
            }
            catch (Exception)
            {
            }

            return iValue;
        }
        /// -------------------------------------------------------------------------------------------
        /// <summary>
        /// Get a named long column from the current row of a datareader, handling Null values.
        /// </summary>
        /// <param name="Reader">The data reader.</param>
        /// <param name="ColumnName">The name of the column to get.</param>
        /// <returns>The actual value or -1 if NULL or missing.</returns>
        public static long GetLong(DbDataReader Reader, string ColumnName)
        {
            long iValue = -1;

            try
            {
                int iColumn = Reader.GetOrdinal(ColumnName);
                if (!Reader.IsDBNull(iColumn))
                {
                    iValue = Reader.GetInt64(iColumn);
                }
            }
            catch (Exception)
            {
            }

            return iValue;
        }
        /// <summary>
        /// Get a named single column from the current row of a datareader, handling Null values.
        /// </summary>
        /// <param name="Reader">The data reader.</param>
        /// <param name="ColumnName">The name of the column to get.</param>
        /// <returns>The actual value or -1.0 if NULL or missing.</returns>
        public static float GetSingle(DbDataReader Reader, string ColumnName)
        {
            float sValue = -1.0F;

            try
            {
                int iColumn = Reader.GetOrdinal(ColumnName);
                if (!Reader.IsDBNull(iColumn))
                {
                    string sDataTypeName = Reader.GetDataTypeName(iColumn);
                    //Dim strValue As String _
                    //    = Reader.GetFloat(iColumn).ToString()

                    object oValue = Reader.GetValue(iColumn);

                    //** After googling, there appears to be an issue with GetFloat - it doesn't work.
                    //** I tested the datatypename and it definitely was 'float'
                    //** GetDouble appears to work - gotta love Microsoft.
                    sValue = (float)Reader.GetDouble(iColumn);
                }
            }
            catch (Exception)
            {
                sValue = -1.0F;
            }

            return sValue;
        }
        /// <summary>
        /// Get a named string column from the current row of a datareader, handling Null values.
        /// </summary>
        /// <param name="Reader">The data reader.</param>
        /// <param name="ColumnName">The name of the column to get.</param>
        /// <returns>The actual value or the empty string if NULL or missing.</returns>
        public static string GetString(DbDataReader Reader, string ColumnName)
        {
            string sValue = "";

            try
            {
                int iColumn = Reader.GetOrdinal(ColumnName);
                if (!Reader.IsDBNull(iColumn))
                {
                    sValue = Reader.GetString(iColumn);
                }
            }
            catch (Exception)
            {
            }

            return sValue;
        }

        public static object GetObject(DbDataReader Reader, string ColumnName)
        {
            object oReturn = null;

            try
            {
                int iColumn = Reader.GetOrdinal(ColumnName);
                if (!Reader.IsDBNull(iColumn))
                {
                    oReturn = Reader.GetValue(iColumn);
                }
            }
            catch (Exception)
            {
            }

            return oReturn;
        }
        /// <summary>
        /// Get a named boolean column from the current row of a datareader, handling Null values.
        /// </summary>
        /// <param name="Reader">The data reader.</param>
        /// <param name="ColumnName">The name of the column to get.</param>
        /// <returns>The actual value or False if NULL or missing.</returns>
        public static byte[] GetByte(DbDataReader Reader, string ColumnName)
        {
            byte[] bValue = null;


            try
            {
                int iColumn = Reader.GetOrdinal(ColumnName);
                if (!Reader.IsDBNull(iColumn))
                {
                    bValue = (byte[])(Reader.GetValue(iColumn));
                }
            }
            catch (Exception)
            {
            }

            return bValue;
        }
        /// <summary>
        /// Initialize an object with standard default values.
        /// </summary>
        public static void InitFields(ref object Instance, ref Array Fields)
        {
            foreach (PropertyInfo f in Fields)
            {
                if (f.CanWrite)
                {
                    switch (f.PropertyType.Name)
                    {
                        case "Int32":
                        case "Int64":
                        case "Long":
                            f.SetValue(Instance, -1);
                            break;
                        case "String":
                            f.SetValue(Instance, "");
                            break;
                        case "DateTime":
                            f.SetValue(Instance, Convert.ToDateTime("1/1/1900"));
                            break;
                        case "Boolean":
                            f.SetValue(Instance, false);
                            break;
                        case "Decimal":
                            f.SetValue(Instance, -1m);
                            break;
                        case "Single":
                            f.SetValue(Instance, -1);
                            break;
                        case "Object":
                            f.SetValue(Instance, null);
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Initialize an object from the current row of a data reader.
        /// </summary>
        public static void ReadFields(ref object Instance, ref Array Fields, ref DbDataReader Reader)
        {
            foreach (PropertyInfo f in Fields)
            {
                if (f.CanWrite)
                {
                    switch (f.PropertyType.Name)
                    {
                        case "Int32":
                            f.SetValue(Instance, GetInteger(Reader, f.Name));
                            break;
                        case "Int64":
                        case "Long":
                            f.SetValue(Instance, GetLong(Reader, f.Name));
                            break;
                        case "String":
                            f.SetValue(Instance, GetString(Reader, f.Name));
                            break;
                        case "DateTime":
                            f.SetValue(Instance, GetDate(Reader, f.Name));
                            break;
                        case "Boolean":
                            f.SetValue(Instance, GetBoolean(Reader, f.Name));
                            break;
                        case "Decimal":
                            f.SetValue(Instance, GetDecimal(Reader, f.Name));
                            break;
                        case "Single":
                            f.SetValue(Instance, GetSingle(Reader, f.Name));
                            break;
                        case "Object":
                            f.SetValue(Instance, GetObject(Reader, f.Name));
                            break;
                        case "Byte[]":
                            f.SetValue(Instance, GetByte(Reader, f.Name));
                            break;
                        case "Guid":
                            f.SetValue(Instance, GetGuid(Reader, f.Name));
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Copy all of the internal fields.
        /// </summary>
        public void CopyFields(ref object Instance, ref Array Fields, ref object Source)
        {
            try
            {
                //** When used with .Net Remoting hosted by IIS, even with Full trust,
                //** f.SetValue fails with a permission exception.
                //** This asset grants the required permissions so that the f.SetValue works.
                ReflectionPermission rp = new ReflectionPermission(PermissionState.Unrestricted);
                rp.Assert();
                foreach (PropertyInfo f in Fields)
                {
                    if (f.CanWrite)
                    {
                        f.SetValue(Instance, f.GetValue(Source));
                    }
                }
            }
            finally
            {
                ReflectionPermission.RevertAssert();
            }
        }


        public static object ReadSingle(ref DbCommand Command, ref ConstructorInfo Constructor)
        {
            object oData = null;
            DbDataReader oRdr = null;
            int iSQLReturnValue = 0;
            DbParameter oPrm = Command.CreateParameter();

            oPrm.ParameterName = "@RETURN_VALUE";
            oPrm.DbType = DbType.Int32;
            oPrm.Direction = ParameterDirection.ReturnValue;
            Command.Parameters.Insert(0, oPrm);
            try
            {
                if (!(Command.Connection.State == ConnectionState.Open))
                {
                    Command.Connection.Open();
                }
                oRdr = Command.ExecuteReader(CommandBehavior.CloseConnection);
                if (oRdr.HasRows)
                {
                    if (oRdr.Read())
                    {
                        object[] oParms = { oRdr };
                        oData = Constructor.Invoke(oParms);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if ((oRdr != null))
                {
                    if (!oRdr.IsClosed)
                    {
                        oRdr.Close();
                    }
                    try
                    {
                        //** Only AFTER the reader is closed can we access the return value
                        iSQLReturnValue = Convert.ToInt32(oPrm.Value);
                    }
                    catch (Exception)
                    {
                        //** Ignore errors here
                    }
                }
                if (Command.Connection.State != ConnectionState.Closed)
                {
                    Command.Connection.Close();
                }
            }
            return oData;
        }

        public static List<T> ReadList<T>(ref DbCommand Command, ref ConstructorInfo Constructor)
        {
            List<T> aList = new List<T>();
            T oData = default(T);
            DbDataReader oRdr = null;
            int iSQLReturnValue = 0;
            DbParameter oPrm = Command.CreateParameter();

            oPrm.ParameterName = "@RETURN_VALUE";
            oPrm.DbType = DbType.Int32;
            oPrm.Direction = ParameterDirection.ReturnValue;
            Command.Parameters.Insert(0, oPrm);
            try
            {
                if (!(Command.Connection.State == ConnectionState.Open))
                {
                    Command.Connection.Open();
                }
                oRdr = Command.ExecuteReader(CommandBehavior.CloseConnection);
                if (oRdr.HasRows)
                {
                    while (oRdr.Read())
                    {
                        object[] oParms = { oRdr };
                        oData = (T)Constructor.Invoke(oParms);
                        aList.Add(oData);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if ((oRdr != null))
                {
                    if (!oRdr.IsClosed)
                    {
                        oRdr.Close();
                    }
                    try
                    {
                        //** Only AFTER the reader is closed can we access the return value
                        iSQLReturnValue = Convert.ToInt32(oPrm.Value);
                        foreach (T oData_loopVariable in aList)
                        {
                            oData = oData_loopVariable;
                        }
                    }
                    catch (Exception)
                    {
                        //** Ignore errors here
                    }
                }
                if (Command.Connection.State != ConnectionState.Closed)
                {
                    Command.Connection.Close();
                }
            }
            return aList;
        }
    }
}