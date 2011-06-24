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

        public MySqlOdbcUtils(string userName, string passwd, string dbName, string hostName) :
            base(userName, passwd, dbName, hostName)
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
