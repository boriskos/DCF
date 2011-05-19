using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCF.DataLayer;

namespace DCF.XmlRules
{
    public class XmlSqlStatement
    {
        public XmlSqlStatement(string statement, MySqlUtils utils)
        {
            SqlStatement = statement;
            SqlUtils = utils;
        }

        public virtual void execute(Dictionary<string, object> dataHashTable)
        {
            Func<int> curIterationGetter = dataHashTable["CurrentIteration"] as Func<int>;
            int curIteration = curIterationGetter();

            SqlUtils.ExecuteNonQuery(string.Format(SqlStatement, curIteration));
        }

        public string SqlStatement { get; protected set; }
        public MySqlUtils SqlUtils { get; protected set; }
    }
}
