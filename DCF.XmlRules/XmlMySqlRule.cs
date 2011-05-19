using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCF.DataLayerAwareLib;
using DCF.DataLayer;
using DCF.Lib;
using System.Xml;
using System.Diagnostics;

namespace DCF.XmlRules
{
    public class XmlMySqlRule : MySqlRule
    {
        public XmlMySqlRule(MySqlUtils utils, IRuleSupplier ruleSupplier):
            base(utils, ruleSupplier)
        {
            m_ruleExecuter += new RuleExecuterDelegate(XmlMySqlRuleExecuter);
            m_ruleInitializer += new RuleExecuterDelegate(XmlMySqlRuleInitializer);
        }

        public void parse(XmlNode ruleElement)
        {
            Debug.Assert(ruleElement.Name == "Rule");
            Id = ruleElement.Attributes["Id"].Value;
            XmlNode initNode = ruleElement.FirstChild;
            Debug.Assert(initNode.Name == "Initialization");
            populateStatements(m_initStatements, initNode);
            XmlNode bodyNode = initNode.NextSibling;
            Debug.Assert(bodyNode.Name == "Body");
            populateStatements(m_bodyStatements, bodyNode);
        }

        private void populateStatements(List<XmlSqlStatement> statements, XmlNode parentNode)
        {
            foreach (XmlNode statementNode in parentNode.ChildNodes)
            {
                string actualStatement = 
                    statementNode.InnerText.Replace("$__CleaningInternals__.CurrentIteration", "{0}");
                switch (statementNode.Name)
                {
                    case "NonQueryStatement":
                        XmlSqlStatement stmnt = new XmlSqlStatement(
                            actualStatement, SqlUtils);
                        statements.Add(stmnt);
                        break;
                    case "EventStatement":
                        //string eventName = statementNode.Attributes["Raise"];
                        XmlSqlEventStatement evtstmnt = new XmlSqlEventStatement(
                            actualStatement, SqlUtils, OnStopCleaningProcess);
                        statements.Add(evtstmnt);
                        break;
                    case "RepairKey":
                        string idAttr = statementNode.Attributes["IdField"].Value;
                        string keyAttr = statementNode.Attributes["KeyField"].Value;
                        string probabilityAttr = statementNode.Attributes["ProbabilityField"].Value;
                        string targetTableName = statementNode.Attributes["TargetTable"].Value;
                        XmlSqlRepairKeyStatement rpkstmnt = new XmlSqlRepairKeyStatement(actualStatement, SqlUtils,
                            idAttr, keyAttr, probabilityAttr, targetTableName);
                        statements.Add(rpkstmnt);
                        break;
                    default:
                        Debug.Fail("Undefined node in statements " + statementNode.Name);
                        break;
                }
            }
        }

        internal void XmlMySqlRuleInitializer(Dictionary<string, object> dataHashTable)
        {
            if (m_initStatements != null)
            {
                foreach (var stmnt in m_initStatements)
                {
                    stmnt.execute(dataHashTable);
                }
            }
        }
        internal void XmlMySqlRuleExecuter(Dictionary<string, object> dataHashTable)
        {
            if (m_bodyStatements != null)
            {
                foreach (var stmnt in m_bodyStatements)
                {
                    stmnt.execute(dataHashTable);
                }
            }
        }
        
        private List<XmlSqlStatement> m_initStatements = new List<XmlSqlStatement>();
        private List<XmlSqlStatement> m_bodyStatements = new List<XmlSqlStatement>();
    }
}
