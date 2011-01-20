using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCF.Lib;
using System.Configuration;
using DCF.Common;
using DCF.DataLayer;
using DCF.PaperRules;
using MySql.Data.MySqlClient;

namespace DCF.DemoRules.Test
{
    public class DatabaseCleaningManager: IDisposable
    {
        /// <summary>
        /// Gets the default value of generation parameters,
        /// parses the argument string and overrides default parameters where needed
        /// </summary>
        /// <param name="args"></param>
        public void ParseArgs(string[] args)
        {
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
            // override settings with command line arguments
            try
            {
                foreach (string arg in args)
                {
                    // update connection parameters 
                    if (arg.StartsWith(ArgName("DBName"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        DBName = arg.Split('=')[1];
                    }
                    else if (arg.StartsWith(ArgName("DBPassword"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        DBPassword = arg.Split('=')[1];
                    }
                    else if (arg.StartsWith(ArgName("DBUserName"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        DBUsername = arg.Split('=')[1];
                    }
                    else if (arg.StartsWith(ArgName("HostName"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        HostName = arg.Split('=')[1];
                    }
                    else if (CleaningConfiguration.SettingNames.Contains(ExtractName(arg), StringComparer.InvariantCultureIgnoreCase))
                    {
                        CleaningConfiguration.Instance[ExtractName(arg)] = arg.Split('=')[1];
                    }
                    else
                    {
                        Logger.TraceWriteLine(string.Format("Unknown command line parameter: {0}", arg));
                        Program.Usage();
                        Environment.Exit(1);
                    }
                }
            }
            catch (Exception)
            {
                Logger.TraceWriteLine("Incorrect argumets");
                Program.Usage();
            }
            // display the current configuration
            TraceConfiguration();
        }

        private string ExtractName(string arg)
        {
            if (arg.StartsWith("/") && arg.Contains('='))
            { // valid argument
                return arg.Substring(1).Split('=')[0];
            }
            return string.Empty; // incorrect argument
        }

        /// <summary>
        /// Initializes the cleaning process
        /// </summary>
        /// <remarks>
        /// Connects to the database and creates <see cref="CleansingManager"/>
        /// </remarks>
        public void InitFlow()
        {
            m_sqlUtils = new MySqlNativeClientUtils(DBUsername, DBPassword, DBName, HostName);
            SqlUtils.Connect();
            Logger.TraceWriteLine(string.Format("Connected to {0}/{1}@{2} - {3}", DBUsername, DBPassword, DBName, HostName));
            m_cleansingManager = new CleansingManager(new PaperRuleProvider(SqlUtils));

            // remove previous run data
            try
            {
                Logger.TraceWriteLine("Truncating table " + TableConstants.ScoredFacts);
                SqlUtils.TruncateTable(TableConstants.ScoredFacts);
                Logger.TraceWriteLine("Truncating table " + TableConstants.UserScores);
                SqlUtils.TruncateTable(TableConstants.UserScores);
            }
            catch (MySqlException)
            {
                Logger.TraceWriteLine(string.Format("Tables {0} and {1} do not exit", 
                    TableConstants.ScoredFacts, TableConstants.UserScores));
            }
        }

        public void DoTestFlow()
        {
            m_cleansingManager.cleanData(null);
            // measure the quality
            string innerSelect = 
                "select a.* from scoredfacts a, " +
                "(SELECT topicid, max(Score) as score FROM scoredfacts group by topicid) b " +
                "where a.topicid = b.topicid and a.score = b.score group by a.topicid";
            string qualityMeasurement = string.Format(
                "select " +
                "(select count(*) from ({0}) d, correctfacts c where c.itemid=d.itemid) / " +
                "(select count(*) from ({0}) e) as Quality", innerSelect
            );
            object qualityRes = SqlUtils.ExecuteScalar(qualityMeasurement);
            Logger.TraceWriteLine(string.Format("The quality of the run is {0}", qualityRes.ToString()));
        }

        public void FinishFlow()
        {
            Dispose();
        }

        #region Disposable Pattern
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_sqlUtils != null)
                {
                    m_sqlUtils.Dispose();
                    m_sqlUtils = null;
                }
            }
        }
        #endregion

        #region Public Properties

        public string DBUsername { get; private set; }
        public string DBPassword { get; private set; }
        public string DBName { get; private set; }
        public string HostName { get; private set; }

        public MySqlUtils SqlUtils { get { return m_sqlUtils; } }

        #endregion

        #region Private Helping methods

        private static string ArgName(string name)
        {
            return "/" + name.ToLower();
        }

        private void TraceConfiguration()
        {
            Logger.TraceWriteLine("Cleansing Configuration for current run is:");
            Logger.TraceIndent();
            CleaningConfiguration.Instance.TraceCurrentConfiguration();
            Logger.TraceUnindent();
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////
        //"ShowSQL",
        //"ShowTableContents",
        //"DBUserName",
        //"DBPassword",
        //"DBName",

        private MySqlUtils m_sqlUtils = null;
        private CleansingManager m_cleansingManager = null;

    }
}
