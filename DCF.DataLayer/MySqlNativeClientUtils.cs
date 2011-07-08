﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace DCF.DataLayer
{
    public class MySqlNativeClientUtils: MySqlUtils
    {

        public MySqlNativeClientUtils(string userName, string passwd, string dbName, string hostName) :
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
                string connStringTempl = ConfigurationManager.AppSettings["DBConnectionString"];
				if (connStringTempl == null)
                {
					connStringTempl = 
						"Data Source={0};Initial Catalog={1};User Id={2};Password={3};default command timeout=6000;Allow User Variables=true";
				}
                string res = string.Format( connStringTempl, 
					this.HostName, this.DbName, this.UserName, this.Password );
                return res;
            }
        }

        public override string SqlUtilsTimerName
        {
            get { return "MySqlNativeClient"; }
        }

        protected override DbCommandBuilder CreateCommandBuilder(DataAdapter da)
        {
            if (da==null) 
                throw new ArgumentNullException("Data Adapter argument is null");
            MySqlDataAdapter msda = da as MySqlDataAdapter;
            if (msda == null) 
                throw new ArgumentOutOfRangeException("da must be MySqlDataAdapter");
            return new MySqlCommandBuilder(msda);
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        protected override DbDataAdapter CreateDataAdapter(
            string sqlStmnt)
        {
            if (Connection == null) 
                throw new NullReferenceException("Connection");
            MySqlConnection connection = Connection as MySqlConnection;
            if (connection == null)
                throw new ArgumentOutOfRangeException("The class Connection is not a MySqlConnection");
            return new MySqlDataAdapter(sqlStmnt, connection);
        }
        protected override DbCommand CreateCommand(string sqlStmnt)
        {
            if (Connection == null) 
                throw new NullReferenceException("Connection");
            MySqlConnection connection = Connection as MySqlConnection;
            if (connection == null)
                throw new ArgumentOutOfRangeException("The class Connection is not a MySqlConnection");
            return new MySqlCommand(sqlStmnt, connection);
        }
    }
}
