using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.Data.Common;
using System.Data;
using System.Diagnostics;

namespace DCF.DataLayer
{
    public class MySqlOdbcUtils: MySqlUtils
    {

        public MySqlOdbcUtils(string userName, string passwd, string dbName) :
            base(userName, passwd, dbName)
        {
        }

        public override void InstertNewTable(DataTable tbl)
        {
            throw new NotImplementedException();
        }

        public override string ConnectionString
        {
            get 
            { 
                return string.Format(ConnectionFormatString, 
                    DbName, UserName, Password); 
            }
        }

        public override string SqlUtilsTimerName
        {
            get { return "MySqlOdbc"; }
        }

        protected override DbCommandBuilder CreateCommandBuilder(DataAdapter da)
        {
            if (da == null)
                throw new ArgumentNullException("da");
            OdbcDataAdapter oda = da as OdbcDataAdapter;
            if (oda == null)
                throw new ArgumentOutOfRangeException("da must be OdbcDataAdapter");
            return new OdbcCommandBuilder(oda);
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new OdbcConnection(connectionString);
        }

        protected override DbDataAdapter CreateDataAdapter(string sqlStmnt)
        {
            OdbcConnection connection = Connection as OdbcConnection;
            if (connection == null)
                throw new InvalidCastException("Connecton must be of type OdbcConnection");
            return new OdbcDataAdapter(sqlStmnt, connection);
        }
        protected override DbCommand CreateCommand(string sqlStmnt)
        {
            OdbcConnection connection = Connection as OdbcConnection;
            if (connection == null)
                throw new InvalidCastException("Connecton must be of type OdbcConnection");
            return new OdbcCommand(sqlStmnt, connection);
        }

        private const string ConnectionFormatString =
            "Driver={{MySQL ODBC 3.51 Driver}};Server=localhost;Database={0};User={1};pwd={2};Option=3";
    }
}

        
        
        
        
        /*IDisposable
    {
        public class SqlTableCreator
        {

            #region Instance Variables

            private DbConnection _connection;

            public DbConnection Connection
            {

                get { return _connection; }

                set { _connection = value; }

            }

            private DbTransaction _transaction;

            public DbTransaction Transaction
            {

                get { return _transaction; }

                set { _transaction = value; }

            }

            private string _tableName;

            public string DestinationTableName
            {

                get { return _tableName; }

                set { _tableName = value; }

            }

            #endregion

            #region Constructor

            public SqlTableCreator() { }

            public SqlTableCreator(DbConnection connection) : this(connection, null) { }

            public SqlTableCreator(DbConnection connection, DbTransaction transaction)
            {

                _connection = connection;

                _transaction = transaction;

            }

            #endregion

            #region Instance Methods

            public object Create(DataTable schema)
            {
                return Create(schema, null);
            }

            public object Create(DataTable schema, int numKeys)
            {
                int[] primaryKeys = new int[numKeys];

                for (int i = 0; i < numKeys; i++)
                {

                    primaryKeys[i] = i;

                }

                return Create(schema, primaryKeys);
            }

            public int Create(DataTable schema, int[] primaryKeys)
            {
                string tableName = _tableName;
                if (tableName==null)
                    tableName = schema.TableName;
                string sql = GetCreateSQL( tableName, schema, primaryKeys);
                return CreateFromSql(sql);
            }

            public int CreateFromDataTable(DataTable table)
            {
                string tableName = _tableName;
                if (tableName==null)
                    tableName = table.TableName;
                string sql = GetCreateFromDataTableSQL(tableName, table);
                int res = CreateFromSql(sql);
                return res;
            }

            #endregion

            #region Static Methods

            public static string GetCreateSQL(string tableName, DataTable schema, int[] primaryKeys)
            {

                string sql = "CREATE TABLE " + tableName + " (\n";



                // columns

                foreach (DataRow column in schema.Rows)
                {

                    if (!(schema.Columns.Contains("IsHidden") && (bool)column["IsHidden"]))

                        sql += column["ColumnName"].ToString() + " " + SQLGetType(column) + ",\n";

                }

                sql = sql.TrimEnd(new char[] { ',', '\n' }) + "\n";



                // primary keys

                string pk = "CONSTRAINT PK_" + tableName + " PRIMARY KEY CLUSTERED (";

                bool hasKeys = (primaryKeys != null && primaryKeys.Length > 0);

                if (hasKeys)
                {

                    // user defined keys

                    foreach (int key in primaryKeys)
                    {

                        pk += schema.Rows[key]["ColumnName"].ToString() + ", ";

                    }

                }

                else
                {

                    // check schema for keys

                    string keys = string.Join(", ", GetPrimaryKeys(schema));

                    pk += keys;

                    hasKeys = keys.Length > 0;

                }

                pk = pk.TrimEnd(new char[] { ',', ' ', '\n' }) + ")\n";

                if (hasKeys) sql += pk;

                sql += ")";



                return sql;

            }



            public static string GetCreateFromDataTableSQL(string tableName, DataTable table)
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat("CREATE TABLE {0} (\n",tableName);

                // columns
                foreach (DataColumn column in table.Columns)
                {
                    sql.AppendFormat("{0} {1},\n", column.ColumnName, SQLGetType(column));
                }

                sql.Remove(sql.Length-2, 1);
                sql.AppendLine(")");

                // primary keys

                //if (table.PrimaryKey.Length > 0)
                //{

                //    sql += "CONSTRAINT [PK_" + tableName + "] PRIMARY KEY CLUSTERED (";

                //    foreach (DataColumn column in table.PrimaryKey)
                //    {

                //        sql += "[" + column.ColumnName + "],";

                //    }

                //    sql = sql.TrimEnd(new char[] { ',' }) + "))\n";

                //}



                return sql.ToString();

            }



            public static string[] GetPrimaryKeys(DataTable schema)
            {

                List<string> keys = new List<string>();



                foreach (DataRow column in schema.Rows)
                {

                    if (schema.Columns.Contains("IsKey") && (bool)column["IsKey"])

                        keys.Add(column["ColumnName"].ToString());

                }



                return keys.ToArray();

            }



            // Return T-SQL data type definition, based on schema definition for a column

            public static string SQLGetType(object type, int columnSize, int numericPrecision, int numericScale)
            {

                switch (type.ToString())
                {

                    case "System.String":

                        return "VARCHAR(" + ((columnSize == -1) ? 255 : columnSize) + ")";



                    case "System.Decimal":

                        if (numericScale > 0)

                            return "REAL";

                        else if (numericPrecision > 10)

                            return "BIGINT";

                        else

                            return "INT";



                    case "System.Double":

                    case "System.Single":

                        return "REAL";



                    case "System.Int64":

                        return "BIGINT";



                    case "System.Int16":

                    case "System.Int32":

                        return "INT";



                    case "System.DateTime":

                        return "DATETIME";



                    default:

                        throw new Exception(type.ToString() + " not implemented.");

                }

            }



            // Overload based on row from schema table

            public static string SQLGetType(DataRow schemaRow)
            {

                return SQLGetType(schemaRow["DataType"],

                                    int.Parse(schemaRow["ColumnSize"].ToString()),

                                    int.Parse(schemaRow["NumericPrecision"].ToString()),

                                    int.Parse(schemaRow["NumericScale"].ToString()));

            }

            // Overload based on DataColumn from DataTable type

            public static string SQLGetType(DataColumn column)
            {
                return SQLGetType(column.DataType, column.MaxLength, 10, 2);
            }

            #endregion

            #region Private Helping methods
		            
            private int CreateFromSql(string sql)
            {
                DbCommand cmd = _connection.CreateCommand();
                cmd.CommandText = sql;

                if (_transaction != null && _transaction.Connection != null)
                {
                    cmd.Transaction = _transaction;
                }

                return cmd.ExecuteNonQuery();
            }
            #endregion        
        }
        #region Public methods
        public MySqlOdbcUtils(string userName, string passwd, string dbName)
        {
            m_userName = userName;
            m_password = passwd;
            m_dbName = dbName;
        }

        public void Connect()
        {
            using (new PerformanceCounter(SqlUtilsTimerName))
            {
                if (m_sqlConnection != null)
                {
                    throw new InvalidOperationException("The connection is already obtained");
                }
                string connectionString = string.Format(ConnectionFormatString,
                    m_dbName, m_userName, m_password);
                m_sqlConnection = new OdbcConnection(connectionString);
                //m_sqlConnection = new SqlConnection(connectionString);
                m_sqlConnection.Open();
            }
        }

        public void Close()
        {
            Dispose();
        }

        public void ExecuteQuery(string sqlStmnt, DataSet ds)
        {
            using (new PerformanceCounter(SqlUtilsTimerName))
            {
                using (OdbcDataAdapter adapter = new OdbcDataAdapter(sqlStmnt, m_sqlConnection))
                //using (SqlDataAdapter adapter = new SqlDataAdapter(sqlStmnt, m_sqlConnection))
                {
                    adapter.Fill(ds);
                }
            }
        }
        public int ExecuteNonQuery(string sqlStmnt)
        {
            using (new PerformanceCounter(SqlUtilsTimerName))
            {
                Debug.WriteLineIf(Logger.ShowSQLs, sqlStmnt);
                using (OdbcCommand cmd = m_sqlConnection.CreateCommand())
                //using (SqlCommand cmd = m_sqlConnection.CreateCommand())
                {
                    cmd.CommandText = sqlStmnt;
                    return cmd.ExecuteNonQuery();
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

        public void InstertNewTable(DataTable tbl)
        {
            using (new PerformanceCounter(SqlUtilsTimerName))
            {
                int res = DropTableIfExists(tbl.TableName);

                SqlTableCreator tc = new SqlTableCreator(m_sqlConnection);
                tc.CreateFromDataTable(tbl);

                using (DataSet ds = new DataSet())
                using (OdbcCommand cmd = new OdbcCommand(string.Format("select * from {0}", tbl.TableName), m_sqlConnection))
                //using (SqlCommand cmd = new SqlCommand(string.Format("select * from {0}", tbl.TableName), m_sqlConnection))
                using (OdbcDataAdapter da = new OdbcDataAdapter(cmd))
                //using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                using (OdbcCommandBuilder cb = new OdbcCommandBuilder(da))
                {
                    da.FillSchema(ds, SchemaType.Source);
                    foreach (DataRow row in tbl.Rows)
                    {
                        ds.Tables[0].Rows.Add(row.ItemArray);
                    }
                    da.Update(ds);
                }
            }
        }
        public void RePopulateExistingTable(DataTable tbl)
        {
            using (new PerformanceCounter(SqlUtilsTimerName))
            using (new PerformanceCounter("RePopulateExistingTable"))
            {
                Debug.Assert(!string.IsNullOrEmpty(tbl.TableName), "Table Name must be set");
                OdbcCommand cmdDrop = m_sqlConnection.CreateCommand();
                cmdDrop.CommandText = string.Format("TRUNCATE TABLE {0}", tbl.TableName);
                int res = cmdDrop.ExecuteNonQuery();

                //using (DataSet ds = new DataSet())
                using (OdbcCommand cmd = new OdbcCommand(string.Format("select * from {0}", tbl.TableName), m_sqlConnection))
                using (OdbcDataAdapter da = new OdbcDataAdapter(cmd))
                using (OdbcCommandBuilder cb = new OdbcCommandBuilder(da))
                {
                    //da.FillSchema(ds, SchemaType.Source);
                    //foreach (DataRow row in tbl.Rows)
                    //{
                    //    ds.Tables[0].Rows.Add(row.ItemArray);
                    //    //ds.Tables[0].LoadDataRow(row.ItemArray, false);
                    //}
                    //da.UpdateBatchSize = 0;
                    da.Update(tbl);
                }
            }
        }

        public DbConnection Connection
        {
            get
            {
                return m_sqlConnection;
            }
        }
        public Stopwatch Stopper 
        { 
            get 
            { 
                return PerformanceCounterStatic.GetStopwatchTimer(SqlUtilsTimerName); 
            } 
        }
        //public void Update(DataSet ds)
        //{
        //    OdbcDataAdapter adapter = new OdbcDataAdapter();
        //    adapter.UpdateCommand = new OdbcCommand();
        //    adapter.UpdateCommand.
        //    adapter.Update(ds);
        //}
        #endregion
        
        #region IDisposable Members

        public void Dispose()
        {
            if (m_sqlConnection != null)
            {
                m_sqlConnection.Close();
                m_sqlConnection = null;
            }
        }

        #endregion

        #region Private Members
        private const string ConnectionFormatString =
            //@"Data Source=.\SQLEXPRESS;Initial Catalog={0};Integrated Security=True";
            "Driver={{MySQL ODBC 3.51 Driver}};Server=localhost;Database={0};User={1};pwd={2};Option=3";
        private string m_userName;
        private string m_password;
        private string m_dbName;
        private OdbcConnection m_sqlConnection;
        //private SqlConnection m_sqlConnection;


        private const string SqlUtilsTimerName = "SQLUtils";

        #endregion
    }
} */
