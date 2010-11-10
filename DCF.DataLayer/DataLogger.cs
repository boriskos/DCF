using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DCF.Common;

namespace DCF.DataLayer
{
    public class DataLogger
    {
        public static void PrintDbTable(string tableName, MySqlUtils sqlUtils)
        {
            if (Logger.ShowTableContents)
            {
                DataSet ds = new DataSet();
                sqlUtils.ExecuteQuery("select * from " + tableName, ds);
                ds.Tables[0].TableName = tableName;
                Logger.PrintTable(ds.Tables[0]);
            }
        }

        public static void PrintTableSize(string tableName, MySqlUtils sqlUtils)
        {
            Logger.DebugWriteLine(string.Format(
                "Table {0} has {1} elements.",
                tableName,
                sqlUtils.ExecuteScalar(string.Format("SELECT COUNT(*) FROM {0}", tableName))));
        } 

    }
}
