using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCF.Lib;
using System.Xml;
using System.Diagnostics;
using DCF.DataLayer;

namespace DCF.XmlRules
{
    public class XmlRuleSupplier : IRuleSupplier
    {
        public XmlRuleSupplier(MySqlUtils utils)
        {
            SqlUtils = utils;
        }

        public void parseFile(string fileName)
        {
            XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(fileName);
            XmlElement root = doc.DocumentElement;
            Debug.Assert(root.Name == "Rules");
            XmlNode rulesTypeElement = root.FirstChild;
            populateRules(m_cleaningRules, rulesTypeElement);
            populateRules(m_sampleRules, rulesTypeElement.NextSibling);
        }

        private void populateRules(List<Rule> rules, XmlNode xmlNode)
        {
            foreach (XmlNode ruleElement in xmlNode.ChildNodes)
            {
                XmlMySqlRule rule = new XmlMySqlRule(SqlUtils, this);
                rule.parse(ruleElement);
                rules.Add(rule);
            }
        }
        #region IRuleSupplier Members

        public IList<Rule> GetCleaningRules()
        {
            return m_cleaningRules;
        }

        public IList<Rule> GetSampleRules()
        {
            return m_sampleRules;
        }

        #endregion
        public MySqlUtils SqlUtils { get; private set; }

        private List<Rule> m_cleaningRules = new List<Rule>();
        private List<Rule> m_sampleRules = new List<Rule>();
    }
}
