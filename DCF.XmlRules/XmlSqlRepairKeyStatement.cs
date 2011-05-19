using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCF.DataLayer;
using System.Data;
using DCF.Lib;

namespace DCF.XmlRules
{
    public class XmlSqlRepairKeyStatement : XmlSqlStatement
    {
        public XmlSqlRepairKeyStatement(string stmnt, MySqlUtils utils, 
            string idFields, string keyFields, string probField, string targetTable) :
            base(stmnt, utils)
        {
            m_idFields = idFields.Split(',');
            for (int i = 0; i < m_idFields.Length; i++) m_idFields[i] = m_idFields[i].Trim();
            m_keyFields = keyFields;
            m_probField = probField;
            m_targetTableName = targetTable;
        }

        public override void execute(Dictionary<string, object> dataHashTable)
        {
            Func<int> curIterationGetter = dataHashTable["CurrentIteration"] as Func<int>;
            int curIteration = curIterationGetter();

            // select source data from the database
            DataSet ds = new DataSet();
            SqlUtils.ExecuteQuery(string.Format(SqlStatement, curIteration), ds);

            // Apply Repair-Key
            var repairedFacts = ds.Tables[0].AsEnumerable().RepairKey(
                row => row[m_keyFields],
                row => row.Field<double>(m_probField));

            // Produce the result
            DataTable tbl = new DataTable(m_targetTableName);
            for (int i = 0; i < m_idFields.Length; i++)
            {
                tbl.Columns.Add(m_idFields[i], typeof(UInt32));
            }

            foreach (var row in repairedFacts)
            {
                DataRow newRow = tbl.NewRow();
                for (int i = 0; i < m_idFields.Length; i++)
                {
                    newRow[m_idFields[i]] = row[m_idFields[i]];
                }
                tbl.Rows.Add(newRow);
            }
            SqlUtils.RePopulateExistingTable(tbl);
        }
        
        private string[] m_idFields;
        private string m_keyFields;
        private string m_probField;
        private string m_targetTableName;
    }
}
