using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using DCF.Common;
using DCF.DataLayer;
using System.Data;

namespace DCF.DemoRules.Test
{
    public class TestDataGenerationManager: IDisposable
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

            m_initSection = ConfigurationManager.GetSection("InitSection") as InitSection;
            if (m_initSection == null)
            {
                Logger.TraceWriteLine("InitSection is not found in app config file");
                Environment.Exit(1); // terminate
            }

            // override settings with command line arguments
            try
            {
                foreach (string arg in args)
                {
                    if (arg.StartsWith(ArgName("NumberOfFacts"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        m_initSection.NumberOfFacts = long.Parse(arg.Split('=')[1]);
                    }
                    else if (arg.StartsWith(ArgName("TopicsVariabilityProfile")))
                    {
                        m_initSection.TopicsVariabilityProfile = arg.Split('=')[1];
                    }
                    else if (arg.StartsWith(ArgName("UserProfiles"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        m_initSection.UserProfiles = arg.Split('=')[1];
                    }
                    else if (arg.StartsWith(ArgName("GenerateBasisTables"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        m_initSection.GenerateBasisTables = bool.Parse(arg.Split('=')[1]);
                    }
                    else if (arg.StartsWith(ArgName("ItemsDefinitionFile"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        m_initSection.ItemsDefinitionFile = arg.Split('=')[1];
                    }
                    else if (arg.StartsWith(ArgName("TopicsDefinitionFile"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        m_initSection.TopicsDefinitionFile = arg.Split('=')[1];
                    }
                    // update connection parameters 
                    else if (arg.StartsWith(ArgName("DBName"), StringComparison.InvariantCultureIgnoreCase))
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
                    else
                    {
                        Logger.TraceWriteLine(string.Format("Unknown command line parameter: {0}", arg));
                        Environment.Exit(1);
                    }
                }
            }
            catch (Exception)
            {
                Logger.TraceWriteLine("Incorrect argumets");
                Logger.TraceWriteLine("Usage: MySQLTest [parameter=value] ...");
            }
            // display the current configuration
            TraceConfiguration();
        }

        /// <summary>
        /// Initializes the generation process - connection to the database and creation of <see cref="TestDataGenerator"/>
        /// </summary>
        public void InitFlow()
        {
            m_sqlUtils = new MySqlNativeClientUtils(DBUsername, DBPassword, DBName, HostName);
            SqlUtils.Connect();
            Logger.TraceWriteLine(string.Format("Connected to {0}/{1}@{2}", DBUsername, DBPassword, DBName));
            m_testDataGenerator = new TestDataGenerator(m_initSection);
        }

        public void DoTestFlow()
        {
            // regenerate Correct Facts table if necessary
            if (m_initSection.GenerateBasisTables)
            {
                Logger.DebugWriteLine(string.Format("Recreating {0} table...", TableConstants.CorrectFacts));
                SqlUtils.DropTableIfExists(TableConstants.CorrectFacts);
                SqlUtils.ExecuteNonQuery("CREATE TABLE CorrectFacts AS (SELECT * FROM Items WHERE 1=0)");
                Logger.DebugWriteLine("Done!");
            }

            DataTable dtTopics = new DataTable(TableConstants.Topics);
            DataTable dtItems = new DataTable(TableConstants.Items);
            DataTable dtCorrectFacts = new DataTable(TableConstants.CorrectFacts);
            SqlUtils.ReadTableSchema(TableConstants.Topics, dtTopics);
            SqlUtils.ReadTableSchema(TableConstants.Items, dtItems);
            SqlUtils.ReadTableSchema(TableConstants.CorrectFacts, dtCorrectFacts);

            /////////////////////////////////////////////////////////////////
            // generated the basis tables if necessary and update the database
            if (m_testDataGenerator.GenerateBasisTables(dtTopics, dtItems, dtCorrectFacts))
            {
                Logger.DebugWrite("Replacing Basic Tables in the database...");
                // ItemsMentions has a foreign key to Itmes
                SqlUtils.TruncateTable(TableConstants.ItemsMentions); 
                SqlUtils.TruncateTable(dtItems.TableName);
                SqlUtils.TruncateTable(dtCorrectFacts.TableName);
                SqlUtils.TruncateTable(dtTopics.TableName);

                SqlUtils.RePopulateExistingTable(dtTopics);
                SqlUtils.RePopulateExistingTable(dtItems);
                SqlUtils.RePopulateExistingTable(dtCorrectFacts);
                Logger.DebugWriteLine("Done!");
            }
            else // otherwise, read the database tables
            {
                Logger.DebugWrite("Populating Basic Tables from the database...");
                SqlUtils.PopulateTableFromDB(dtItems);
                SqlUtils.PopulateTableFromDB(dtCorrectFacts);
                SqlUtils.PopulateTableFromDB(dtTopics);
                Logger.DebugWriteLine("Done!");
            }

            /////////////////////////////////////////////////////////////////
            // now simulate the user activities according to the parameters
            DataTable dtItemsMentions = new DataTable(TableConstants.ItemsMentions);
            DataTable dtUsers = new DataTable(TableConstants.Users);
            SqlUtils.ReadTableSchema(TableConstants.ItemsMentions, dtItemsMentions);
            SqlUtils.ReadTableSchema(TableConstants.Users, dtUsers);

            m_testDataGenerator.SimulateUsersActivity(
                dtTopics, dtItems, dtCorrectFacts, dtItemsMentions, dtUsers);
            // commit changes into the database
            Logger.DebugWrite("Populating database with users and their items mentions...");
            SqlUtils.TruncateTable(dtItemsMentions.TableName);
            SqlUtils.TruncateTable(dtUsers.TableName);

            SqlUtils.RePopulateExistingTable(dtUsers);
            SqlUtils.RePopulateExistingTable(dtItemsMentions);
            Logger.DebugWriteLine("Done!");
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

        public  string DBUsername { get; private set; }
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
            Logger.TraceWriteLine("Init Section for current run is:");
            Logger.TraceIndent();
            m_initSection.TraceContents();
            Logger.TraceUnindent();
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////
        //"ShowSQL",
        //"ShowTableContents",
        //"DBUserName",
        //"DBPassword",
        //"DBName",
        //"NumberOfFacts",
        //"NumberOfIncorrectFactsInUse",
        //"GenerateBasisTables",
        //"UserProfiles",
        //"ItemsDefinitionFile",
        //"TopicsDefinitionFile"

        private InitSection m_initSection = null;
        private MySqlUtils m_sqlUtils = null;
        private TestDataGenerator m_testDataGenerator = null;

    }
}
