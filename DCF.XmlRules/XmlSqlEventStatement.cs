using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCF.DataLayer;
using DCF.Lib;

namespace DCF.XmlRules
{
    public class XmlSqlEventStatement: XmlSqlStatement
    {
        public XmlSqlEventStatement(string statement, MySqlUtils utils, Rule.RuleStoppingDelegate raiseEvent):
            base (statement, utils)
        {
            SqlUtils = utils;
            m_event = raiseEvent;
        }

        public override void execute(Dictionary<string, object> dataHashTable)
        {
            Func<int> curIterationGetter = dataHashTable["CurrentIteration"] as Func<int>;
            int curIteration = curIterationGetter();

            object res = SqlUtils.ExecuteScalar(string.Format(SqlStatement, curIteration));
            if ((res is long) && ((long)res != 0))
            {
                m_event();
            }
        }

        private Rule.RuleStoppingDelegate m_event;
    }
}
