using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCF.DataLayer;
using System.Configuration;
using DCF.Common;
using DCF.XmlRules;
using DCF.Lib;

namespace DCF.DemoRules.Test
{
    class TestXmlReading
    {
        public TestXmlReading(string filename)
        {
            m_filename = filename;
            DBName = ConfigurationManager.AppSettings["DBName"];
            DBUsername = ConfigurationManager.AppSettings["DBUserName"];
            DBPassword = ConfigurationManager.AppSettings["DBPassword"];
            HostName = ConfigurationManager.AppSettings["HostName"];

            foreach (string settingName in CleaningConfiguration.SettingNames)
            {
                string settingValue = ConfigurationManager.AppSettings[settingName];
                if (settingValue != null)
                {
                    CleaningConfiguration.Instance[settingName] = settingValue;
                }
            }
        }

        public void init()
        {
            m_sqlUtils = new MySqlNativeClientUtils(DBUsername, DBPassword, DBName, HostName);
            SqlUtils.Connect();
            Logger.TraceWriteLine(string.Format("Connected to {0}/{1}@{2} - {3}", DBUsername, DBPassword, DBName, HostName));
            m_supplier = new XmlRuleSupplier(SqlUtils);
            m_supplier.parseFile(m_filename);
            m_cleansingManager = new CleansingManager(m_supplier);

        }

        public void DoTestFlow()
        {
            m_cleansingManager.cleanData(null);
        }

        public string DBUsername { get; private set; }
        public string DBPassword { get; private set; }
        public string DBName { get; private set; }
        public string HostName { get; private set; }
        public MySqlUtils SqlUtils { get { return m_sqlUtils; } }

        private string m_filename;
        private MySqlUtils m_sqlUtils;
        private XmlRuleSupplier m_supplier;
        private CleansingManager m_cleansingManager;
    }
}
