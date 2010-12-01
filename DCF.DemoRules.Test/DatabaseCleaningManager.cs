using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCF.Lib;
using System.Configuration;
using DCF.Common;
using DCF.DataLayer;

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
                    else if (CleaningConfiguration.SettingNames.Contains(arg))
                    {
                        CleaningConfiguration.Instance[arg] = arg.Split('=')[1];
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
            m_cleansingManager = new CleansingManager(new OfflineCleaningRuleProvider(SqlUtils));
        }

        public void DoTestFlow()
        {
            m_cleansingManager.cleanData(null);
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
