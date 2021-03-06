﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;

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
                return string.Format(
                    "Data Source={3};Initial Catalog={0};User Id={1};Password={2};default command timeout=600",
                    this.DbName, this.UserName, this.Password, this.HostName); 
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
