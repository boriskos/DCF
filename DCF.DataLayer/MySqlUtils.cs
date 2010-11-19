using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Collections;
using DCF.Common;

namespace DCF.DataLayer
{
    public abstract class MySqlUtils: IDisposable
    {
        
        #region Public methods
        public MySqlUtils(string userName, string passwd, string dbName)
        {
            m_userName = userName;
            m_password = passwd;
            m_dbName = dbName;
            BatchSize = 10; // initial value
        }

        public void Connect()
        {
            using (new PerformanceCounter(SqlUtilsTimerName))
            {
                if (m_sqlConnection != null)
                {
                    throw new InvalidOperationException("The connection is already obtained");
                }
                m_sqlConnection = CreateConnection(ConnectionString);
                m_sqlConnection.Open();
            }
        }

        public void ExecuteQuery(string sqlStmnt, DataSet ds)
        {
            using (new PerformanceCounter(SqlUtilsTimerName))
            {
                using (DbDataAdapter adapter = CreateDataAdapter(sqlStmnt))
                {
                    adapter.Fill(ds);
                }
            }
        }

        public int ExecuteNonQuery(string sqlStmnt)
        {
            using (new PerformanceCounter(SqlUtilsTimerName))
            {
                Logger.WriteLineIf(Logger.ShowSQLs, sqlStmnt);
                using (DbCommand cmd = m_sqlConnection.CreateCommand())
                {
                    cmd.CommandText = sqlStmnt;
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Executes given SQL query and returns the first colunm of the first row element.
        /// The rest information is ignored
        /// </summary>
        /// <param name="sqlStmnt">query statement</param>
        /// <returns>the first colunm of the first row element as an object</returns>
        public object ExecuteScalar(string sqlStmnt)
        {
            using (new PerformanceCounter(SqlUtilsTimerName))
            {
                Logger.WriteLineIf(Logger.ShowSQLs, sqlStmnt);
                using (DbCommand cmd = m_sqlConnection.CreateCommand())
                {
                    cmd.CommandText = sqlStmnt;
                    return cmd.ExecuteScalar();
                }
            }
        }

        public int ExecuteNonQuery(string sqlStmnt, DbTransaction transaction)
        {
            using (new PerformanceCounter(SqlUtilsTimerName))
            {
                Logger.WriteLineIf(Logger.ShowSQLs, sqlStmnt);
                using (DbCommand cmd = m_sqlConnection.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = sqlStmnt;
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public void ReadTableSchema(string tablename, DataTable table)
        {
            using (new PerformanceCounter(SqlUtilsTimerName))
            {
                table.TableName = tablename;
                using (DbDataAdapter da = CreateDataAdapter(string.Format(
                    "select * from {0}", table.TableName)))
                {
                    da.FillSchema(table, SchemaType.Mapped);
                }
            }
        }

        public int DropTableIfExists(string tableName)
        {
            return ExecuteNonQuery(string.Format("DROP TABLE IF EXISTS {0}", tableName));
        }
        public int TruncateTable(string tableName)
        {
            return ExecuteNonQuery(string.Format("TRUNCATE TABLE {0}", tableName));
        }

        public abstract void InstertNewTable(DataTable tbl);

        public void RePopulateExistingTable(DataTable tbl)
        {
            using (new PerformanceCounter(SqlUtilsTimerName))
            using (new PerformanceCounter("RePopulateExistingTable"))
            {
                Logger.Assert(!string.IsNullOrEmpty(tbl.TableName), "Table Name must be set");
                DbCommand cmdDrop = m_sqlConnection.CreateCommand();
                cmdDrop.CommandText = string.Format("TRUNCATE TABLE {0}", tbl.TableName);
                int res = cmdDrop.ExecuteNonQuery();

                foreach (DataRow row in tbl.Rows)
                {
                    row.AcceptChanges();
                    row.SetAdded();
                }
                using (DbDataAdapter da = CreateDataAdapter(string.Format(
                    "select * from {0}", tbl.TableName)))
                using (DbCommandBuilder cb = CreateCommandBuilder(da))
                {
                    da.UpdateBatchSize = 10;
                    da.Update(tbl);
                }
            }
        }
        public void RePopulateExistingTable(DataSet ds)
        {
            using (new PerformanceCounter(SqlUtilsTimerName))
            using (new PerformanceCounter("RePopulateExistingTable"))
            {
                foreach (DataTable tbl in ds.Tables)
                {
                    Logger.Assert(!string.IsNullOrEmpty(tbl.TableName), "Table Name must be set");
                    DbCommand cmdDrop = m_sqlConnection.CreateCommand();
                    cmdDrop.CommandText = string.Format("TRUNCATE TABLE {0}", tbl.TableName);
                    int res = cmdDrop.ExecuteNonQuery();

                    foreach (DataRow row in tbl.Rows)
                    {
                        row.AcceptChanges();
                        row.SetAdded();
                    }
                    using (DbDataAdapter da = CreateDataAdapter(string.Format(
                        "select * from {0}", tbl.TableName)))
                    using (DbCommandBuilder cb = CreateCommandBuilder(da))
                    {
                        da.UpdateBatchSize = 10;
                        da.Update(tbl);
                    }
                }
            }
        }
        public void RePopulateExistingTable(DataTable tbl, DbTransaction transaction)
        {
            using (new PerformanceCounter(SqlUtilsTimerName))
            using (new PerformanceCounter("RePopulateExistingTable"))
            {
                Logger.Assert(!string.IsNullOrEmpty(tbl.TableName), "Table Name must be set");
                DbCommand cmdDrop = m_sqlConnection.CreateCommand();
                cmdDrop.Transaction = transaction;
                cmdDrop.CommandText = string.Format("TRUNCATE TABLE {0}", tbl.TableName);
                int res = cmdDrop.ExecuteNonQuery();

                using (DbDataAdapter da = CreateDataAdapter(string.Format(
                    "select * from {0}", tbl.TableName)))
                using (DbCommandBuilder cb = CreateCommandBuilder(da))
                {
                    da.UpdateBatchSize = 10;
                    da.Update(tbl);
                }
            }
        }

        internal void ManualRePopulateExistingTable(DataTable tbl, DbTransaction transaction)
        {
            using (new PerformanceCounter(SqlUtilsTimerName))
            using (new PerformanceCounter("ManualRePopulateExistingTable"))
            {
                if (string.IsNullOrEmpty(tbl.TableName))
                    throw new ArgumentException("Table Name must be set in argument tbl");
                // drop table first
                DbCommand cmdDrop = m_sqlConnection.CreateCommand();
                cmdDrop.CommandText = string.Format("TRUNCATE TABLE {0}", tbl.TableName);
                cmdDrop.Transaction = transaction;
                int res = cmdDrop.ExecuteNonQuery();

                // building SQL statement from table
                StringBuilder columnNames = new StringBuilder();
                foreach (DataColumn col in tbl.Columns)
                {
                    if (columnNames.Length > 0)
                    {
                        columnNames.Append(',');
                    }
                    columnNames.Append(col.ColumnName);
                }
                // insert command
                DbCommand cmdIns = m_sqlConnection.CreateCommand();
                cmdIns.Transaction = transaction;

                // string that keeps parameters of an SQL statement
                StringBuilder paramsString = new StringBuilder();

                int currentRow = 0; 
                foreach ( DataRow row in tbl.Rows)
                {
                    // current row modulo batch size
                    if (currentRow++ % BatchSize > 0) paramsString.Append(',');
                    // adds the current row values in form of parameters to the command
                    AddParamertersForRow(tbl, cmdIns, paramsString, currentRow % BatchSize, row);
                    if (currentRow % BatchSize == 0) // reached the batch size
                    { //////////////// send the current command to DB //////////////////////
                        // set the command text
                        cmdIns.CommandText = string.Format("insert into {0} ({1}) values {2}",
                            tbl.TableName, columnNames.ToString(), paramsString.ToString());
                        Logger.Assert(BatchSize == cmdIns.ExecuteNonQuery(), 
                            "ExecuteNonQuery returned non expected number of affected rows");
                        paramsString.Remove(0, paramsString.Length); // cleat the parameters object
                        // reset parameters 
                        cmdIns.Parameters.Clear();
                    }
                }
                int rowsLeft = currentRow % BatchSize;
                if ( rowsLeft != 0) // finished the loop and did not send the command
                { // send the current command to DB
                    // set the command text
                    cmdIns.CommandText = string.Format("insert into {0} ({1}) values {2}",
                        tbl.TableName, columnNames.ToString(), paramsString.ToString());
                    Logger.Assert(rowsLeft == cmdIns.ExecuteNonQuery(), 
                        "ExecuteNonQuery returned non expected number of affected rows");
                }
            }
        }

        private static void AddParamertersForRow(DataTable tbl, DbCommand cmdIns, StringBuilder paramsString, int currentRow, DataRow row)
        {
            paramsString.Append('(');
            bool first = true;
            foreach (DataColumn col in tbl.Columns)
            {
                // parameter name is of form @<col name><row num>
                string paramName = string.Format("@{0}{1}", col.ColumnName, currentRow);

                // add this name to the list of parameters for this row
                if (!first)
                {
                    paramsString.Append(',');
                }
                first = false;
                paramsString.AppendFormat(paramName);

                // add this column value to the command under the current name
                DbParameter dbparam = cmdIns.CreateParameter();
                dbparam.ParameterName = paramName;
                dbparam.Direction = ParameterDirection.Input;
                dbparam.Value = row[col];

                cmdIns.Parameters.Add(dbparam);
            }
            paramsString.Append(')');
        }

        public DbConnection Connection
        {
            get
            {
                return m_sqlConnection;
            }
        }

        public string DbName { get { return m_dbName; } }
        public string Password { get { return m_password; } }
        public string UserName { get { return m_userName; } }

        public System.Diagnostics.Stopwatch Stopper 
        { 
            get 
            { 
                return PerformanceCounterStatic.GetStopwatchTimer(SqlUtilsTimerName); 
            } 
        }

        public int BatchSize { get; set; }
        #endregion

        #region Abstract Methods

        public abstract string ConnectionString { get; }
        public abstract string SqlUtilsTimerName { get; }

        protected abstract DbCommandBuilder CreateCommandBuilder(DataAdapter da);
        protected abstract DbConnection CreateConnection(string connectionString);
        protected abstract DbDataAdapter CreateDataAdapter(string sqlStmnt);

        #endregion Abstract Methods

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MySqlUtils()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (m_sqlConnection != null)
            {
                m_sqlConnection.Close();
                m_sqlConnection = null;
            }
        }

        public void Close()
        {
            Dispose();
        }

        #endregion

        #region Private Members
        
        private string m_userName;
        private string m_password;
        private string m_dbName;
        private DbConnection m_sqlConnection;

        #endregion
    }
}
